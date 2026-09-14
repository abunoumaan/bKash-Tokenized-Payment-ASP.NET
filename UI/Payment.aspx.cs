using System;
using System.Web.UI;
using bKashPayment.Models;
using bKashPayment.Services;

namespace bKashPayment
{
    public partial class Payment : Page
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private BkashService _bkashService = new BkashService();
        private PaymentService _paymentService;
        private AgreementService _agreementService;

        protected void Page_Load(object sender, EventArgs e)
        {
            _paymentService = new PaymentService(_context, _bkashService);
            _agreementService = new AgreementService(_context);

            if (!IsPostBack)
            {
                // Check if returning from bKash callback
                if (!string.IsNullOrEmpty(Request.QueryString["paymentID"]))
                {
                    HandleBkashCallback();
                }
            }
        }

        protected void BtnMakePayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (!ValidateInputs())
                    return;

                // Get or create customer
                string mobileNumber = txtMobileNumber.Value + txtMobileNumber.Text;
                string email = txtCustomerEmail.Text.Trim();
                string name = txtCustomerName.Text.Trim();
                decimal amount = decimal.Parse(txtAmount.Text);
                string reference = txtReference.Text.Trim();

                // Check if customer exists
                Customer customer = _context.Customers.FirstOrDefault(c => c.PhoneNumber == mobileNumber);
                
                if (customer == null)
                {
                    // Create new customer (Guest customer)
                    customer = new Customer
                    {
                        Name = name,
                        Email = email,
                        PhoneNumber = mobileNumber,
                        IsActive = true,
                        DateCreated = DateTime.Now
                    };
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                }
                else
                {
                    // Update existing customer info
                    customer.Name = name;
                    customer.Email = email;
                    _context.SaveChanges();
                }

                // Get or create agreement
                BkashAgreement agreement = _agreementService.GetActiveAgreement(customer.CustomerID);
                
                if (agreement == null)
                {
                    // Create new agreement for this customer
                    string payerReference = $"CUST_{customer.CustomerID}_{DateTime.Now.Ticks}";
                    agreement = _agreementService.SaveAgreement(
                        customer.CustomerID,
                        Guid.NewGuid().ToString(),
                        payerReference
                    );
                }

                // Create payment transaction
                PaymentTransaction transaction = _paymentService.CreateTransaction(
                    customer.CustomerID,
                    agreement.AgreementID,
                    amount,
                    reference
                );

                // Initiate bKash payment
                InitiateBkashPayment(customer, transaction, amount);
            }
            catch (Exception ex)
            {
                ShowError("পেমেন্ট প্রক্রিয়ায় সমস্যা হয়েছে: " + ex.Message);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                ShowError("নাম ফিল্ড খালি রাখা যাবে না");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerEmail.Text))
            {
                ShowError("ইমেইল ফিল্ড খালি রাখা যাবে না");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMobileNumber.Text) || txtMobileNumber.Text.Length < 10)
            {
                ShowError("সঠিক মোবাইল নাম্বার দিন (যেমন: 01xxxxxxxxx)");
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount < 10 || amount > 100000)
            {
                ShowError("পেমেন্ট পরিমাণ ১০ থেকে ১,০০,০০০ টাকার মধ্যে হতে হবে");
                return false;
            }

            return true;
        }

        private async void InitiateBkashPayment(Customer customer, PaymentTransaction transaction, decimal amount)
        {
            try
            {
                // Get access token
                string accessToken = await _bkashService.GetAccessTokenAsync();

                // Get customer's agreement
                BkashAgreement agreement = _context.BkashAgreements.Find(transaction.AgreementID);

                // Create tokenized payment
                string paymentId = await _bkashService.CreateTokenizedPaymentAsync(
                    accessToken,
                    agreement.AgreementToken,
                    amount
                );

                // Execute payment
                var result = await _bkashService.ExecuteTokenizedPaymentAsync(accessToken, paymentId);

                if (result["statusCode"].ToString() == "0000")
                {
                    // Payment successful
                    _paymentService.UpdateTransactionStatus(
                        transaction.TransactionID,
                        "COMPLETED",
                        $"Payment completed. Transaction ID: {result["trxID"]}"
                    );

                    ShowSuccess($"পেমেন্ট সফল হয়েছে! আপনার Transaction ID: {result["trxID"]}");
                }
                else
                {
                    // Payment failed
                    _paymentService.LogTransactionError(
                        transaction.TransactionID,
                        result["statusMessage"].ToString()
                    );

                    ShowError($"পেমেন্ট ব্যর্থ: {result["statusMessage"]}");
                }
            }
            catch (Exception ex)
            {
                _paymentService.LogTransactionError(transaction.TransactionID, ex.Message);
                ShowError("পেমেন্ট প্রক্রিয়া ব্যর্থ: " + ex.Message);
            }
        }

        private void HandleBkashCallback()
        {
            try
            {
                string paymentId = Request.QueryString["paymentID"];
                string status = Request.QueryString["status"];

                if (status == "success")
                {
                    ShowSuccess("আপনার পেমেন্ট সফলভাবে সম্পন্ন হয়েছে!");
                }
                else if (status == "cancel")
                {
                    ShowError("আপনি পেমেন্ট বাতিল করেছেন");
                }
                else
                {
                    ShowError("পেমেন্ট ব্যর্থ হয়েছে");
                }
            }
            catch (Exception ex)
            {
                ShowError("Callback প্রক্রিয়ায় ত্রুটি: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblErrorMessage.Text = message;
            errorMessage.Attributes["class"] = "error-message show";
        }

        private void ShowSuccess(string message)
        {
            lblSuccessMessage.Text = message;
            successMessage.Attributes["class"] = "success-message show";
        }
    }
}
