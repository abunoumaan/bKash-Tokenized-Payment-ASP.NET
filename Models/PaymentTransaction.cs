using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bKashPayment.Models
{
    /// <summary>
    /// পেমেন্ট ট্রানজ্যাকশন মডেল
    /// </summary>
    [Table("PaymentTransactions")]
    public class PaymentTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("Agreement")]
        public int AgreementID { get; set; }

        [Display(Name = "পরিমাণ")]
        [Range(10, 100000, ErrorMessage = "পরিমাণ ১০ থেকে ১০০,০০০ টাকার মধ্যে হতে হবে")]
        public decimal Amount { get; set; }

        [StringLength(5)]
        [Display(Name = "মুদ্রা")]
        public string Currency { get; set; } = "BDT";

        [Required]
        [StringLength(100)]
        [Display(Name = "পেমেন্ট আইডি")]
        public string PaymentID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "স্ট্যাটাস")]
        public string Status { get; set; }

        [StringLength(100)]
        [Display(Name = "রেফারেন্স")]
        public string Reference { get; set; }

        [Display(Name = "ত্রুটি বার্তা")]
        public string ErrorMessage { get; set; }

        [Display(Name = "সৃষ্টির তারিখ")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "সম্পন্ন হওয়ার তারিখ")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "মন্তব্য")]
        public string Remarks { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("AgreementID")]
        public virtual BkashAgreement Agreement { get; set; }

        public PaymentTransaction()
        {
            CreatedDate = DateTime.Now;
            Currency = "BDT";
            Status = "PENDING";
        }

        /// <summary>
        /// চেক করুন যে পেমেন্ট সফল হয়েছে কিনা
        /// </summary>
        public bool IsSuccessful
        {
            get { return Status.ToUpper() == "COMPLETED"; }
        }

        /// <summary>
        /// চেক করুন যে পেমেন্ট ব্যর্থ হয়েছে কিনা
        /// </summary>
        public bool IsFailed
        {
            get { return Status.ToUpper() == "FAILED"; }
        }
    }
}
