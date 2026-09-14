using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bKashPayment.Services
{
    /// <summary>
    /// bKash API সেবা ক্লাস
    /// </summary>
    public class BkashService
    {
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public BkashService()
        {
            _appKey = ConfigurationManager.AppSettings["bKashAppKey"];
            _appSecret = ConfigurationManager.AppSettings["bKashAppSecret"];
            _username = ConfigurationManager.AppSettings["bKashUsername"];
            _password = ConfigurationManager.AppSettings["bKashPassword"];
            _baseUrl = ConfigurationManager.AppSettings["bKashBaseUrl"];
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// bKash থেকে Access Token পান
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var credentials = new
                {
                    app_key = _appKey,
                    app_secret = _appSecret,
                    username = _username,
                    password = _password
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(credentials),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/v1.2.0-beta/tokenized/checkout/token/grant",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"bKash Token API Error: {response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                if (result.statusCode == "0000")
                {
                    return result.id_token;
                }

                throw new Exception($"bKash Token Error: {result.statusMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get bKash Access Token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// নতুন Agreement তৈরি করুন
        /// </summary>
        public async Task<string> CreateAgreementAsync(string accessToken, string payerReference, string callbackUrl)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                client.DefaultRequestHeaders.Add("X-APP-Key", _appKey);

                var agreementRequest = new
                {
                    mode = "0011",
                    payerReference = payerReference,
                    callbackURL = callbackUrl
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(agreementRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    $"{_baseUrl}/v1.2.0-beta/tokenized/checkout/create",
                    content
                );

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                if (result.statusCode == "0000")
                {
                    return result.bkashURL;
                }

                throw new Exception($"Agreement Creation Error: {result.statusMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Agreement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// টোকেনাইজড পেমেন্ট তৈরি করুন
        /// </summary>
        public async Task<string> CreateTokenizedPaymentAsync(string accessToken, string agreementId, decimal amount)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                client.DefaultRequestHeaders.Add("X-APP-Key", _appKey);

                var paymentRequest = new
                {
                    amount = amount.ToString("F2"),
                    currency = "BDT",
                    intent = "sale",
                    agreementID = agreementId,
                    merchantInvoiceNumber = GenerateInvoiceNumber()
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(paymentRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    $"{_baseUrl}/v1.2.0-beta/tokenized/checkout/payment/create",
                    content
                );

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                if (result.statusCode == "0000")
                {
                    return result.paymentID;
                }

                throw new Exception($"Payment Creation Error: {result.statusMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Tokenized Payment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// পেমেন্ট এক্সিকিউট করুন
        /// </summary>
        public async Task<JObject> ExecuteTokenizedPaymentAsync(string accessToken, string paymentId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                client.DefaultRequestHeaders.Add("X-APP-Key", _appKey);

                var payload = new { paymentID = paymentId };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    $"{_baseUrl}/v1.2.0-beta/tokenized/checkout/payment/execute",
                    content
                );

                var responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to execute Tokenized Payment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// পেমেন্ট স্ট্যাটাস চেক করুন
        /// </summary>
        public async Task<JObject> QueryPaymentAsync(string accessToken, string paymentId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                client.DefaultRequestHeaders.Add("X-APP-Key", _appKey);

                var payload = new { paymentID = paymentId };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    $"{_baseUrl}/v1.2.0-beta/tokenized/checkout/payment/query",
                    content
                );

                var responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to query Payment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ইনভয়েস নম্বর তৈরি করুন
        /// </summary>
        private string GenerateInvoiceNumber()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }
    }
}
