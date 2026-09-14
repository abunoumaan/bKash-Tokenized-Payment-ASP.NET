using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using bKashPayment.Models;

namespace bKashPayment
{
    public partial class PaymentHistory : Page
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPaymentHistory();
                LoadStatistics();
            }
        }

        private void LoadPaymentHistory()
        {
            try
            {
                string phoneFilter = txtSearchPhone.Text.Trim();
                string statusFilter = ddlStatus.SelectedValue;

                var query = _context.PaymentTransactions.AsQueryable();

                // Phone filter
                if (!string.IsNullOrEmpty(phoneFilter))
                {
                    query = query.Where(p => p.Customer.PhoneNumber.Contains(phoneFilter));
                }

                // Status filter
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    query = query.Where(p => p.Status == statusFilter);
                }

                var payments = query.OrderByDescending(p => p.CreatedDate).ToList();
                gvPayments.DataSource = payments;
                gvPayments.DataBind();
            }
            catch (Exception ex)
            {
                // Log error
                Response.Write($"<script>alert('ত্রুটি: {ex.Message}');</script>");
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var transactions = _context.PaymentTransactions.ToList();

                lblTotalTransactions.Text = transactions.Count.ToString();
                lblCompletedTransactions.Text = transactions.Where(t => t.Status == "COMPLETED").Count().ToString();

                decimal totalAmount = transactions
                    .Where(t => t.Status == "COMPLETED")
                    .Sum(t => t.Amount);

                lblTotalAmount.Text = totalAmount.ToString("N2") + " ৳";
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('ত্রুটি: {ex.Message}');</script>");
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadPaymentHistory();
        }

        protected void GvPayments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPayments.PageIndex = e.NewPageIndex;
            LoadPaymentHistory();
        }

        public string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "COMPLETED" => "<span class='status-badge status-completed'>✓ সফল</span>",
                "PENDING" => "<span class='status-badge status-pending'>⏳ অপেক্ষমাণ</span>",
                "FAILED" => "<span class='status-badge status-failed'>✗ ব্যর্থ</span>",
                _ => $"<span class='status-badge'>{status}</span>"
            };
        }
    }
}
