using System;
using System.Collections.Generic;
using System.Linq;
using bKashPayment.Models;

namespace bKashPayment.Services
{
    /// <summary>
    /// Agreement ম্যানেজমেন্ট সেবা
    /// </summary>
    public class AgreementService
    {
        private readonly ApplicationDbContext _context;

        public AgreementService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// নতুন Agreement সংরক্ষণ করুন
        /// </summary>
        public BkashAgreement SaveAgreement(int customerId, string agreementToken, string payerReference)
        {
            try
            {
                var agreement = new BkashAgreement
                {
                    CustomerID = customerId,
                    AgreementToken = agreementToken,
                    PayerReference = payerReference,
                    Status = "ACTIVE",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.BkashAgreements.Add(agreement);
                _context.SaveChanges();

                return agreement;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save Agreement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// গ্রাহকের সক্রিয় Agreement পান
        /// </summary>
        public BkashAgreement GetActiveAgreement(int customerId)
        {
            try
            {
                return _context.BkashAgreements
                    .Where(a => a.CustomerID == customerId && a.Status == "ACTIVE")
                    .OrderByDescending(a => a.CreatedDate)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Agreement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// গ্রাহকের সকল Agreement পান
        /// </summary>
        public List<BkashAgreement> GetCustomerAgreements(int customerId)
        {
            try
            {
                return _context.BkashAgreements
                    .Where(a => a.CustomerID == customerId)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Agreements: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Agreement বাতিল করুন
        /// </summary>
        public bool RevokeAgreement(int agreementId)
        {
            try
            {
                var agreement = _context.BkashAgreements.FirstOrDefault(a => a.AgreementID == agreementId);
                if (agreement != null)
                {
                    agreement.Status = "REVOKED";
                    agreement.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to revoke Agreement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// চেক করুন Agreement এখনও সক্রিয় কিনা
        /// </summary>
        public bool IsAgreementValid(int customerId)
        {
            try
            {
                var agreement = GetActiveAgreement(customerId);
                return agreement != null && agreement.IsActive;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to validate Agreement: {ex.Message}", ex);
            }
        }
    }
}
