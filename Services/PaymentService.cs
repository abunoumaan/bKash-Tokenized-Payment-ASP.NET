using System;
using System.Linq;
using bKashPayment.Models;

namespace bKashPayment.Services
{
    /// <summary>
    /// পেমেন্ট প্রসেসিং সেবা
    /// </summary>
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly BkashService _bkashService;

        public PaymentService(ApplicationDbContext context, BkashService bkashService)
        {
            _context = context;
            _bkashService = bkashService;
        }

        /// <summary>
        /// পেমেন্ট ট্রানজ্যাকশন তৈরি করুন
        /// </summary>
        public PaymentTransaction CreateTransaction(int customerId, int agreementId, decimal amount, string reference)
        {
            try
            {
                var transaction = new PaymentTransaction
                {
                    CustomerID = customerId,
                    AgreementID = agreementId,
                    Amount = amount,
                    Currency = "BDT",
                    PaymentID = Guid.NewGuid().ToString(),
                    Status = "PENDING",
                    Reference = reference,
                    CreatedDate = DateTime.Now
                };

                _context.PaymentTransactions.Add(transaction);
                _context.SaveChanges();

                return transaction;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Transaction: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// পেমেন্ট স্ট্যাটাস আপডেট করুন
        /// </summary>
        public bool UpdateTransactionStatus(int transactionId, string status, string remarks = null)
        {
            try
            {
                var transaction = _context.PaymentTransactions.FirstOrDefault(t => t.TransactionID == transactionId);
                if (transaction != null)
                {
                    transaction.Status = status;
                    transaction.Remarks = remarks;

                    if (status == "COMPLETED")
                    {
                        transaction.CompletedDate = DateTime.Now;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update Transaction: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ত্রুটি লিপিবদ্ধ করুন
        /// </summary>
        public bool LogTransactionError(int transactionId, string errorMessage)
        {
            try
            {
                var transaction = _context.PaymentTransactions.FirstOrDefault(t => t.TransactionID == transactionId);
                if (transaction != null)
                {
                    transaction.Status = "FAILED";
                    transaction.ErrorMessage = errorMessage;
                    transaction.CompletedDate = DateTime.Now;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to log error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// পেমেন্ট হিস্টরি পান
        /// </summary>
        public var GetPaymentHistory(int customerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                return _context.PaymentTransactions
                    .Where(t => t.CustomerID == customerId)
                    .OrderByDescending(t => t.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Payment History: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// মোট পেমেন্ট পরিমাণ পান
        /// </summary>
        public decimal GetTotalPaymentAmount(int customerId)
        {
            try
            {
                return _context.PaymentTransactions
                    .Where(t => t.CustomerID == customerId && t.Status == "COMPLETED")
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to calculate total: {ex.Message}", ex);
            }
        }
    }
}
