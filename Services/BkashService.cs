using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bKashPayment.Services
{
    /// <summary>
    /// bKash API v2 সেবা ক্লাস
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

            LogDebug("=== BkashService Constructor Started ===");
            LogDebug("App Key: " + (_appKey ?? "NULL"));
            LogDebug("App Secret: " + (_appSecret ?? "NULL"));
            LogDebug("Username: " + (_username ?? "NULL"));
            LogDebug("Password: " + (_password ?? "NULL"));
            LogDebug("Base URL: " + (_baseUrl ?? "NULL"));

            // Enable TLS 1.2 (required for modern APIs)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            
            // SSL certificate validation
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                LogDebug("SSL Certificate Validation Called. Errors: " + sslPolicyErrors.ToString());
                return true;
            };

            // HttpClient handler configuration
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            
            // HttpClient singleton instance তৈরি করুন
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // 60 সেকেন্ড timeout

            LogDebug("=== BkashService Constructor Completed ===");
        }

        /// <summary>
        /// Debug logging helper
        /// </summary>
        private void LogDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine("[BkashService] " + message);
            
            // File logging-ও যোগ করুন
            try
            {
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }
                
                string logFile = logPath + "\\BkashService_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string logMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + message + Environment.NewLine;
                System.IO.File.AppendAllText(logFile, logMessage);
            }
            catch { }
        }

        /// <summary>
        /// bKash থেকে Access Token পান (v2 API - সঠিক endpoint)
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            LogDebug("=== GetAccessTokenAsync Started (v2 API) ===");
            
            try
            {
                // Validate configuration
                if (string.IsNullOrEmpty(_appKey) || string.IsNullOrEmpty(_appSecret))
                {
                    throw new Exception("bKash configuration missing. Check Web.config AppSettings.");
                }

                if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                {
                    throw new Exception("bKash username/password missing. Check Web.config AppSettings.");
                }

                // v2 API - Correct endpoint from bKash documentation
                string url = (_baseUrl ?? "").TrimEnd('/') + "/tokenized-checkout/auth/grant-token";
                LogDebug("Token URL: " + url);

                // Create request with proper headers
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                
                // Add Headers
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("username", _username);
                request.Headers.Add("password", _password);

                // Request Body with app_key and app_secret
                var tokenRequest = new
                {
                    app_key = _appKey,
                    app_secret = _appSecret
                };

                string requestJson = JsonConvert.SerializeObject(tokenRequest);
                LogDebug("Request Headers - username: " + _username);
                LogDebug("Request Headers - password: " + _password);
                LogDebug("Request Body: " + requestJson);

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                LogDebug("Making POST request to bKash...");

                // Make request
                HttpResponseMessage response = null;
                try
                {
                    response = await _httpClient.PostAsync(url, request.Content);
                    LogDebug("Response Status Code: " + response.StatusCode);
                }
                catch (HttpRequestException hexc)
                {
                    LogDebug("HttpRequestException Inner: " + (hexc.InnerException != null ? hexc.InnerException.Message : "No inner exception"));
                    throw new Exception("Connection to bKash failed. Error: " + hexc.Message, hexc);
                }

                // Read response
                string responseString = null;
                try
                {
                    responseString = await response.Content.ReadAsStringAsync();
                    LogDebug("Response Content: " + responseString);
                }
                catch (Exception rexc)
                {
                    LogDebug("Error reading response: " + rexc.Message);
                    throw;
                }

                // Check status code
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("bKash Token API returned error " + response.StatusCode + ": " + responseString);
                }

                // Parse JSON
                JObject result = null;
                try
                {
                    result = JObject.Parse(responseString);
                    LogDebug("Parsed JSON successfully");
                }
                catch (Exception pexc)
                {
                    LogDebug("Error parsing JSON: " + pexc.Message);
                    throw;
                }

                // Check response status
                JToken statusCodeToken = result["statusCode"];
                if (statusCodeToken != null && statusCodeToken.ToString() != "0000")
                {
                    throw new Exception("bKash returned error: " + result["statusMessage"]);
                }

                // Extract token - could be id_token or accessToken
                JToken tokenValue = result["id_token"] ?? result["accessToken"];
                if (tokenValue == null)
                {
                    LogDebug("Response JSON Keys: " + string.Join(", ", result.Properties().Select(p => p.Name)));
                    throw new Exception("No token in response. Check API response format.");
                }

                string token = tokenValue.ToString();
                LogDebug("Token received successfully: " + (token.Length > 10 ? token.Substring(0, 10) + "..." : token));
                LogDebug("=== GetAccessTokenAsync Completed Successfully ===");
                
                return token;
            }
            catch (TaskCanceledException tcex)
            {
                LogDebug("TaskCanceledException: Request timeout. " + tcex.Message);
                throw new Exception("Request timeout - bKash server not responding within 60 seconds", tcex);
            }
            catch (HttpRequestException hexc)
            {
                LogDebug("HttpRequestException: " + hexc.Message);
                if (hexc.InnerException != null)
                {
                    LogDebug("Inner Exception: " + hexc.InnerException.GetType().Name + " - " + hexc.InnerException.Message);
                }
                throw new Exception("Network error: " + hexc.Message, hexc);
            }
            catch (Exception ex)
            {
                LogDebug("General Exception: " + ex.GetType().Name + " - " + ex.Message);
                if (ex.InnerException != null)
                {
                    LogDebug("Inner Exception: " + ex.InnerException.Message);
                }
                throw new Exception("Failed to get bKash Access Token: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// নতুন Agreement তৈরি করুন (v2 API)
        /// </summary>
        public async Task<string> CreateAgreementAsync(string accessToken, string payerReference, string callbackUrl)
        {
            LogDebug("=== CreateAgreementAsync Started ===");
            LogDebug("Payer Reference: " + payerReference);
            LogDebug("Callback URL: " + callbackUrl);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, 
                    (_baseUrl ?? "").TrimEnd('/') + "/tokenized-checkout/create");
                
                request.Headers.Add("Authorization", accessToken);
                request.Headers.Add("X-APP-Key", _appKey);

                var agreementRequest = new
                {
                    mode = "0011",
                    payerReference = payerReference,
                    callbackURL = callbackUrl
                };

                string requestJson = JsonConvert.SerializeObject(agreementRequest);
                LogDebug("Agreement Request: " + requestJson);

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                LogDebug("Agreement Response: " + responseString);
                
                JObject result = JObject.Parse(responseString);

                if (result["statusCode"].ToString() == "0000")
                {
                    LogDebug("Agreement created successfully");
                    return result["bkashURL"].ToString();
                }

                throw new Exception("Agreement Creation Error: " + result["statusMessage"]);
            }
            catch (Exception ex)
            {
                LogDebug("CreateAgreementAsync Error: " + ex.Message);
                throw new Exception("Failed to create Agreement: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// টোকেনাইজড পেমেন্ট তৈরি করুন (v2 API)
        /// </summary>
        public async Task<string> CreateTokenizedPaymentAsync(string accessToken, string agreementId, decimal amount)
        {
            LogDebug("=== CreateTokenizedPaymentAsync Started ===");
            LogDebug("Agreement ID: " + agreementId);
            LogDebug("Amount: " + amount.ToString("F2"));

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, 
                    (_baseUrl ?? "").TrimEnd('/') + "/tokenized-checkout/payment/create");
                
                request.Headers.Add("Authorization", accessToken);
                request.Headers.Add("X-APP-Key", _appKey);

                var paymentRequest = new
                {
                    amount = amount.ToString("F2"),
                    currency = "BDT",
                    intent = "sale",
                    agreementID = agreementId,
                    merchantInvoiceNumber = GenerateInvoiceNumber()
                };

                string requestJson = JsonConvert.SerializeObject(paymentRequest);
                LogDebug("Payment Request: " + requestJson);

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                LogDebug("Payment Create Response: " + responseString);
                
                JObject result = JObject.Parse(responseString);

                if (result["statusCode"].ToString() == "0000")
                {
                    LogDebug("Payment created successfully");
                    return result["paymentID"].ToString();
                }

                throw new Exception("Payment Creation Error: " + result["statusMessage"]);
            }
            catch (Exception ex)
            {
                LogDebug("CreateTokenizedPaymentAsync Error: " + ex.Message);
                throw new Exception("Failed to create Tokenized Payment: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// পেমেন্ট এক্সিকিউট করুন (v2 API)
        /// </summary>
        public async Task<JObject> ExecuteTokenizedPaymentAsync(string accessToken, string paymentId)
        {
            LogDebug("=== ExecuteTokenizedPaymentAsync Started ===");
            LogDebug("Payment ID: " + paymentId);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, 
                    (_baseUrl ?? "").TrimEnd('/') + "/tokenized-checkout/payment/execute");
                
                request.Headers.Add("Authorization", accessToken);
                request.Headers.Add("X-APP-Key", _appKey);

                var payload = new { paymentID = paymentId };
                string requestJson = JsonConvert.SerializeObject(payload);
                LogDebug("Execute Payment Request: " + requestJson);

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                LogDebug("Payment Execute Response: " + responseString);
                
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                LogDebug("ExecuteTokenizedPaymentAsync Error: " + ex.Message);
                throw new Exception("Failed to execute Tokenized Payment: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// পেমেন্ট স্ট্যাটাস চেক করুন (v2 API)
        /// </summary>
        public async Task<JObject> QueryPaymentAsync(string accessToken, string paymentId)
        {
            LogDebug("=== QueryPaymentAsync Started ===");
            LogDebug("Payment ID: " + paymentId);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, 
                    (_baseUrl ?? "").TrimEnd('/') + "/tokenized-checkout/payment/query");
                
                request.Headers.Add("Authorization", accessToken);
                request.Headers.Add("X-APP-Key", _appKey);

                var payload = new { paymentID = paymentId };
                string requestJson = JsonConvert.SerializeObject(payload);
                LogDebug("Query Payment Request: " + requestJson);

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                LogDebug("Payment Query Response: " + responseString);
                
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                LogDebug("QueryPaymentAsync Error: " + ex.Message);
                throw new Exception("Failed to query Payment: " + ex.Message, ex);
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
