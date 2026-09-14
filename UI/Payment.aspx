<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="bKashPayment.Payment" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>bKash One-Click Payment</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .container {
            background: white;
            border-radius: 15px;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
            max-width: 500px;
            width: 100%;
            padding: 40px;
        }

        .header {
            text-align: center;
            margin-bottom: 30px;
        }

        .logo {
            color: #667eea;
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .header h2 {
            color: #333;
            font-size: 24px;
            margin-bottom: 5px;
        }

        .header p {
            color: #999;
            font-size: 14px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: 600;
            font-size: 14px;
        }

        .required {
            color: #e74c3c;
        }

        input[type="text"],
        input[type="email"],
        input[type="number"],
        input[type="tel"],
        select {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 14px;
            transition: border-color 0.3s;
        }

        input[type="text"]:focus,
        input[type="email"]:focus,
        input[type="number"]:focus,
        input[type="tel"]:focus,
        select:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 5px rgba(102, 126, 234, 0.3);
        }

        .input-group {
            display: flex;
            gap: 10px;
        }

        .input-group input {
            flex: 1;
        }

        .input-group select {
            flex: 0.3;
            min-width: 80px;
        }

        .help-text {
            font-size: 12px;
            color: #999;
            margin-top: 5px;
        }

        .payment-methods {
            background: #f5f7fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .payment-methods h4 {
            color: #333;
            margin-bottom: 10px;
            font-size: 14px;
        }

        .method-option {
            margin-bottom: 10px;
        }

        .method-option input[type="radio"] {
            margin-right: 8px;
        }

        .method-option label {
            margin: 0;
            font-weight: normal;
            display: inline;
            cursor: pointer;
        }

        .summary {
            background: #ecf0f1;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            border-left: 4px solid #667eea;
        }

        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            color: #333;
            font-size: 14px;
        }

        .summary-row:last-child {
            margin-bottom: 0;
            border-top: 1px solid #bdc3c7;
            padding-top: 10px;
            font-weight: bold;
            font-size: 16px;
        }

        .btn {
            width: 100%;
            padding: 14px;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            margin-top: 10px;
        }

        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
        }

        .btn-primary:active {
            transform: translateY(0);
        }

        .btn-secondary {
            background: #ecf0f1;
            color: #333;
            border: 1px solid #bdc3c7;
        }

        .btn-secondary:hover {
            background: #dde1e6;
        }

        .error-message {
            background: #f8d7da;
            color: #721c24;
            padding: 12px;
            border-radius: 8px;
            margin-bottom: 20px;
            border: 1px solid #f5c6cb;
            display: none;
        }

        .error-message.show {
            display: block;
        }

        .success-message {
            background: #d4edda;
            color: #155724;
            padding: 12px;
            border-radius: 8px;
            margin-bottom: 20px;
            border: 1px solid #c3e6cb;
            display: none;
        }

        .success-message.show {
            display: block;
        }

        .loading {
            display: none;
            text-align: center;
            margin: 20px 0;
        }

        .spinner {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid #f3f3f3;
            border-top: 3px solid #667eea;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        .footer {
            text-align: center;
            margin-top: 20px;
            color: #999;
            font-size: 12px;
        }

        .footer a {
            color: #667eea;
            text-decoration: none;
        }

        .footer a:hover {
            text-decoration: underline;
        }

        @media (max-width: 600px) {
            .container {
                padding: 25px;
            }

            .header h2 {
                font-size: 20px;
            }

            .input-group {
                flex-direction: column;
            }

            .input-group select {
                width: 100%;
                min-width: auto;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <div class="logo">💳 bKash</div>
                <h2>One-Click Payment</h2>
                <p>যেকোনো মোবাইল নাম্বার থেকে পেমেন্ট করুন</p>
            </div>

            <!-- Error Message -->
            <div class="error-message" id="errorMessage" runat="server">
                <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
            </div>

            <!-- Success Message -->
            <div class="success-message" id="successMessage" runat="server">
                <asp:Label ID="lblSuccessMessage" runat="server"></asp:Label>
            </div>

            <!-- Payment Form -->
            <div class="form-group">
                <label for="txtCustomerName">আপনার নাম <span class="required">*</span></label>
                <asp:TextBox ID="txtCustomerName" runat="server" Placeholder="যেমন: রহিম আহমেদ" required="required"></asp:TextBox>
                <div class="help-text">আপনার সম্পূর্ণ নাম লিখুন</div>
            </div>

            <div class="form-group">
                <label for="txtCustomerEmail">ইমেইল এড়েস <span class="required">*</span></label>
                <asp:TextBox ID="txtCustomerEmail" runat="server" TextMode="Email" Placeholder="example@mail.com" required="required"></asp:TextBox>
                <div class="help-text">পেমেন্ট স্লিপ এই ইমেইলে পাবেন</div>
            </div>

            <div class="form-group">
                <label for="txtMobileNumber">মোবাইল নাম্বার <span class="required">*</span></label>
                <div class="input-group">
                    <select id="countryCode" runat="server">
                        <option value="880">🇧🇩 +880</option>
                        <option value="91">🇮🇳 +91</option>
                        <option value="92">🇵🇰 +92</option>
                    </select>
                    <asp:TextBox ID="txtMobileNumber" runat="server" TextMode="Tel" Placeholder="01xxxxxxxxx" Pattern="01[0-9]{9}" required="required"></asp:TextBox>
                </div>
                <div class="help-text">বাংলাদেশী নাম্বার: 01xxxxxxxxx ফরম্যাটে লিখুন</div>
            </div>

            <div class="form-group">
                <label for="txtAmount">পেমেন্ট পরিমাণ (টাকা) <span class="required">*</span></label>
                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" Min="10" Max="100000" Placeholder="100" required="required"></asp:TextBox>
                <div class="help-text">সর্বনিম্ন ১০ টাকা, সর্বোচ্চ ১,০০,০০০ টাকা</div>
            </div>

            <div class="form-group">
                <label for="txtReference">রেফারেন্স (ঐচ্ছিক)</label>
                <asp:TextBox ID="txtReference" runat="server" Placeholder="যেমন: অর্ডার নাম্বার #12345" TextMode="MultiLine" Rows="2"></asp:TextBox>
                <div class="help-text">পেমেন্টের কোন বিবরণ থাকলে লিখুন</div>
            </div>

            <!-- Payment Summary -->
            <div class="summary">
                <div class="summary-row">
                    <span>পেমেন্ট পদ্ধতি:</span>
                    <strong>bKash</strong>
                </div>
                <div class="summary-row">
                    <span>মোবাইল নাম্বার:</span>
                    <strong><asp:Label ID="lblSummaryPhone" runat="server" Text="-"></asp:Label></strong>
                </div>
                <div class="summary-row">
                    <span>পরিমাণ:</span>
                    <strong><asp:Label ID="lblSummaryAmount" runat="server" Text="0 ৳"></asp:Label></strong>
                </div>
                <div class="summary-row">
                    <span>মোট পরিশোধযোগ্য:</span>
                    <strong><asp:Label ID="lblTotalAmount" runat="server" Text="0 ৳"></asp:Label></strong>
                </div>
            </div>

            <!-- Loading Spinner -->
            <div class="loading" id="loadingSpinner">
                <div class="spinner"></div>
                <p>পেমেন্ট প্রক্রিয়াধীন...</p>
            </div>

            <!-- Buttons -->
            <div>
                <asp:Button ID="btnMakePayment" runat="server" CssClass="btn btn-primary" Text="💰 পেমেন্ট করুন" OnClick="BtnMakePayment_Click" />
                <button type="reset" class="btn btn-secondary">পরিষ্কার করুন</button>
            </div>

            <div class="footer">
                <p>🔒 আপনার তথ্য সম্পূর্ণ নিরাপদ এবং এনক্রিপ্টেড</p>
                <p><a href="#">শর্তাবলী</a> | <a href="#">গোপনীয়তা নীতি</a></p>
            </div>
        </div>
    </form>

    <script>
        // Real-time Summary Update
        document.getElementById('<%= txtMobileNumber.ClientID %>').addEventListener('change', function () {
            document.getElementById('<%= lblSummaryPhone.ClientID %>').innerText = this.value || '-';
        });

        document.getElementById('<%= txtAmount.ClientID %>').addEventListener('change', function () {
            var amount = parseInt(this.value) || 0;
            document.getElementById('<%= lblSummaryAmount.ClientID %>').innerText = amount.toLocaleString('bn-BD') + ' ৳';
            document.getElementById('<%= lblTotalAmount.ClientID %>').innerText = amount.toLocaleString('bn-BD') + ' ৳';
        });

        // Form Validation
        function validateForm() {
            var phone = document.getElementById('<%= txtMobileNumber.ClientID %>').value;
            var amount = document.getElementById('<%= txtAmount.ClientID %>').value;

            if (!phone || phone.length < 10) {
                alert('দয়া করে সঠিক মোবাইল নাম্বার দিন');
                return false;
            }

            if (!amount || amount < 10) {
                alert('সর্বনিম্ন পেমেন্ট পরিমাণ ১০ টাকা');
                return false;
            }

            return true;
        }
    </script>
</body>
</html>
