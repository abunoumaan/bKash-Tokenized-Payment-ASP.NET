using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bKashPayment.Models
{
    /// <summary>
    /// কাস্টমার মডেল
    /// </summary>
    [Table("Customers")]
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "নাম প্রয়োজন")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "ইমেইল প্রয়োজন")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "বৈধ ইমেইল দিন")]
        public string Email { get; set; }

        [Required(ErrorMessage = "ফোন নম্বর প্রয়োজন")]
        [StringLength(15)]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "বৈধ বাংলাদেশী ফোন নম্বর দিন")]
        public string PhoneNumber { get; set; }

        [Display(Name = "সৃষ্টির তারিখ")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "সক্রিয়")]
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<BkashAgreement> Agreements { get; set; }
        public virtual ICollection<PaymentTransaction> Transactions { get; set; }

        public Customer()
        {
            DateCreated = DateTime.Now;
            IsActive = true;
            Agreements = new List<BkashAgreement>();
            Transactions = new List<PaymentTransaction>();
        }
    }
}
