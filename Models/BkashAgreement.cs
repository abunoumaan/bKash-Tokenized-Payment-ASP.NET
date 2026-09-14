using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bKashPayment.Models
{
    /// <summary>
    /// bKash Agreement মডেল - One-Click Payment এর জন্য টোকেন সংরক্ষণ
    /// </summary>
    [Table("BkashAgreements")]
    public class BkashAgreement
    {
        [Key]
        public int AgreementID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Required]
        public string AgreementToken { get; set; }

        [Required]
        [StringLength(100)]
        public string PayerReference { get; set; }

        [StringLength(50)]
        [Display(Name = "স্ট্যাটাস")]
        public string Status { get; set; } = "ACTIVE";

        [Display(Name = "সৃষ্টির তারিখ")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "আপডেটের তারিখ")]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "মেয়াদ শেষের তারিখ")]
        public DateTime? ExpiryDate { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<PaymentTransaction> Transactions { get; set; }

        public BkashAgreement()
        {
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
            Status = "ACTIVE";
            Transactions = new List<PaymentTransaction>();
        }

        /// <summary>
        /// চেক করুন যে Agreement এখনও সক্রিয় কিনা
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (Status != "ACTIVE")
                    return false;

                if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now)
                    return false;

                return true;
            }
        }
    }
}
