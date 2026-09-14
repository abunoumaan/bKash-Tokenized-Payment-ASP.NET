# bKash Tokenized Payment Integration - ASP.NET Web Forms

## 📱 বিবরণ
এটি একটি সম্পূর্ণ bKash One-Click Tokenized Payment সিস্টেম যা ASP.NET Web Forms এবং C# দিয়ে তৈরি। গ্রাহক প্রথমবার তাদের bKash অ্যাকাউন্ট লিঙ্ক করে, তারপর "Make Payment" বাটনে এক ক্লিকে পেমেন্ট করতে পারেন।

## ✨ বৈশিষ্ট্য
- ✅ One-Click Payment সিস্টেম
- ✅ bKash Agreement Management
- ✅ Secure Token Storage
- ✅ Payment History Tracking
- ✅ Error Handling & Logging
- ✅ Sandbox Environment সাপোর্ট
- ✅ Complete Database Schema

## 📋 প্রয়োজনীয়তা
- Visual Studio 2015 বা তার উপরে
- .NET Framework 4.5+
- SQL Server (LocalDB বা Full)
- bKash Merchant Account
- bKash Developer Portal Access

## 🔐 bKash ক্রেডেনশিয়ালস সংগ্রহ করুন

1. [bKash Developer Portal](https://developer.bkash.com/) এ যান
2. আপনার Merchant Account দিয়ে লগইন করুন
3. নিম্নলিখিত তথ্য সংগ্রহ করুন:
   - **App Key**
   - **App Secret**
   - **Username**
   - **Password**

## 🚀 ইনস্টলেশন ধাপ

### Step 1: প্রজেক্ট সেটআপ করুন
```bash
git clone https://github.com/abunoumaan/bKash-Tokenized-Payment-ASP.NET.git
```

### Step 2: ডাটাবেস তৈরি করুন
```sql
-- SQL Server Management Studio এ নিম্নোক্ত স্ক্রিপ্ট চালান
SQLScripts/Database_Setup.sql
```

### Step 3: Web.config আপডেট করুন
```xml
<!-- Web.config এ আপনার ক্রেডেনশিয়ালস যোগ করুন -->
<appSettings>
    <add key="bKashAppKey" value="YOUR_APP_KEY" />
    <add key="bKashAppSecret" value="YOUR_APP_SECRET" />
    <add key="bKashUsername" value="YOUR_USERNAME" />
    <add key="bKashPassword" value="YOUR_PASSWORD" />
    <add key="bKashBaseUrl" value="https://tokenized.sandbox.bka.sh" />
    <!-- Production এর জন্য: https://tokenized.bka.sh -->
    <add key="CallbackUrl" value="http://localhost:xxxx/payment/agreement-callback" />
</appSettings>
```

### Step 4: Packages ইনস্টল করুন
```
Install-Package Newtonsoft.Json
Install-Package EntityFramework
```

## 📁 প্রজেক্ট স্ট্রাকচার

```
bKash-Tokenized-Payment-ASP.NET/
├── SQLScripts/
│   └── Database_Setup.sql          # ডাটাবেস স্কিমা
├── Models/
│   ├── Customer.cs                 # কাস্টমার মডেল
│   ├── bKashAgreement.cs          # Agreement মডেল
│   └── PaymentTransaction.cs       # পেমেন্ট লেনদেন মডেল
├── Services/
│   ├── BkashService.cs            # bKash API সার্ভিস
│   ├── AgreementService.cs        # Agreement ম্যানেজমেন্ট
│   └── PaymentService.cs          # পেমেন্ট প্রসেসিং
├── UI/
│   ├── Default.aspx               # হোম পেজ
│   ├── Payment.aspx               # পেমেন্ট পেজ
│   ├── AgreementSetup.aspx        # Agreement সেটআপ পেজ
│   └── PaymentHistory.aspx        # পেমেন্ট হিস্টরি
├── Web.config                      # কনফিগারেশন ফাইল
└── README.md                       # এই ফাইল
```

## 🎯 ব্যবহার পদ্ধতি

### ১. প্রথম বার - Agreement সেটআপ
```
1. AgreementSetup.aspx পেজে যান
2. "Link bKash Account" বাটনে ক্লিক করুন
3. bKash লগইন স্ক্রিনে আপনার নম্বর ও পিন দিন
4. Agreement অনুমোদন করুন
5. Agreement ID সংরক্ষিত হবে ডাটাবেসে
```

### ২. পরবর্তী বার - One-Click Payment
```
1. Payment.aspx পেজে যান
2. পেমেন্ট পরিমাণ লিখুন
3. "Make Payment" বাটনে ক্লিক করুন
4. পেমেন্ট সম্পন্ন হবে (কোন লগইন প্রয়োজন নেই)
5. পেমেন্ট কনফার্মেশন পাবেন
```

## 💻 কোড উদাহরণ

### bKash Token পাওয়া
```csharp
public async Task<string> GetBkashToken()
{
    var client = new HttpClient();
    var credentials = new
    {
        app_key = ConfigurationManager.AppSettings["bKashAppKey"],
        app_secret = ConfigurationManager.AppSettings["bKashAppSecret"],
        username = ConfigurationManager.AppSettings["bKashUsername"],
        password = ConfigurationManager.AppSettings["bKashPassword"]
    };
    
    var content = new StringContent(
        JsonConvert.SerializeObject(credentials),
        Encoding.UTF8,
        "application/json"
    );
    
    var response = await client.PostAsync(
        ConfigurationManager.AppSettings["bKashBaseUrl"] + "/v1.2.0-beta/tokenized/checkout/token/grant",
        content
    );
    
    var responseString = await response.Content.ReadAsStringAsync();
    dynamic result = JsonConvert.DeserializeObject(responseString);
    return result.id_token;
}
```

### Payment তৈরি করা
```csharp
public async Task<string> CreateTokenizedPayment(string accessToken, string agreementId, decimal amount)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", accessToken);

    var paymentRequest = new
    {
        amount = amount.ToString("F2"),
        currency = "BDT",
        intent = "sale",
        agreementID = agreementId,
        merchantInvoiceNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10)
    };

    var content = new StringContent(
        JsonConvert.SerializeObject(paymentRequest),
        Encoding.UTF8,
        "application/json"
    );
    
    var response = await client.PostAsync(
        ConfigurationManager.AppSettings["bKashBaseUrl"] + "/v1.2.0-beta/tokenized/checkout/payment/create",
        content
    );
    
    var responseString = await response.Content.ReadAsStringAsync();
    dynamic result = JsonConvert.DeserializeObject(responseString);
    return result.paymentID;
}
```

## 🔒 নিরাপত্তা নোট

- ✅ কখনো Customer PIN বা Credentials স্টোর করবেন না
- ✅ Agreement ID এবং Tokens সুরক্ষিতভাবে রক্ষা করুন
- ✅ HTTPS ব্যবহার করুন Production এ
- ✅ API Credentials Web.config এ রাখুন (কখনো Code এ নয়)
- ✅ সকল Payment Transaction Log করুন
- ✅ Error Handling যথাযথভাবে করুন

## 🧪 টেস্টিং

### Sandbox Credentials
```
App Key: YOUR_SANDBOX_APP_KEY
App Secret: YOUR_SANDBOX_APP_SECRET
Username: YOUR_SANDBOX_USERNAME
Password: YOUR_SANDBOX_PASSWORD

Test Phone: ০১৭০০০০০০০০ (bKash এর জন্য)
Test PIN: ১২৩৪৫৬
```

### Test URLs
- Sandbox: `https://tokenized.sandbox.bka.sh`
- Production: `https://tokenized.bka.sh`

## 📞 API এন্ডপয়েন্ট

| এন্ডপয়েন্ট | মেথড | উদ্দেশ্য |
|-----------|------|----------|
| `/v1.2.0-beta/tokenized/checkout/token/grant` | POST | Access Token পান |
| `/v1.2.0-beta/tokenized/checkout/create` | POST | Agreement তৈরি করুন |
| `/v1.2.0-beta/tokenized/checkout/payment/create` | POST | Payment তৈরি করুন |
| `/v1.2.0-beta/tokenized/checkout/payment/execute` | POST | Payment এক্সিকিউট করুন |
| `/v1.2.0-beta/tokenized/checkout/payment/query` | POST | Payment স্ট্যাটাস চেক করুন |

## 📚 রেফারেন্স

- [bKash Developer Docs](https://developer.bkash.com/docs/tokenized-checkout)
- [bKash API Documentation](https://developer.bkash.com/)
- [Postman Collection](https://developer.bkash.com/docs/tokenized-checkout-postman)

## ⚠️ সাধারণ সমস্যা

### Problem: "Invalid app_key or app_secret"
- ✅ Web.config এ সঠিক Credentials চেক করুন
- ✅ Sandbox/Production URL মিলছে কিনা দেখুন

### Problem: "Agreement not found"
- ✅ Agreement ID ডাটাবেসে সঠিকভাবে Save হয়েছে কিনা চেক করুন
- ✅ Customer ID সঠিক কিনা দেখুন

### Problem: "Callback URL not matching"
- ✅ Web.config এর Callback URL সঠিক কিনা দেখুন
- ✅ bKash Dashboard এর Callback URL সেটিংস চেক করুন

## 📄 লাইসেন্স

MIT License - বিনামূল্যে ব্যবহার করুন এবং পরিবর্তন করুন।

## 📧 সহায়তা

কোন প্রশ্ন বা সমস্যা থাকলে Issues খুলুন।

---

**Happy Coding! 🚀**
