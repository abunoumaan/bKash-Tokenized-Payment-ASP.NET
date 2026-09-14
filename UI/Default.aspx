<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="bKashPayment.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>bKash Payment Gateway</title>
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
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            max-width: 600px;
            width: 100%;
            padding: 60px 40px;
            text-align: center;
        }

        .logo {
            font-size: 64px;
            margin-bottom: 20px;
        }

        h1 {
            color: #333;
            font-size: 32px;
            margin-bottom: 15px;
            font-weight: bold;
        }

        .subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 40px;
            line-height: 1.6;
        }

        .features {
            text-align: left;
            background: #f8f9fa;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 40px;
        }

        .feature-item {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
            font-size: 15px;
            color: #555;
        }

        .feature-item:last-child {
            margin-bottom: 0;
        }

        .feature-icon {
            font-size: 24px;
            margin-right: 15px;
            min-width: 30px;
        }

        .button-group {
            display: flex;
            gap: 15px;
            flex-direction: column;
        }

        .btn {
            padding: 15px 30px;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }

        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
        }

        .btn-secondary {
            background: #ecf0f1;
            color: #333;
            border: 2px solid #667eea;
        }

        .btn-secondary:hover {
            background: #667eea;
            color: white;
        }

        .footer {
            margin-top: 40px;
            padding-top: 30px;
            border-top: 1px solid #eee;
            color: #999;
            font-size: 13px;
        }

        .info-box {
            background: #e8f4f8;
            border-left: 4px solid #667eea;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 30px;
            text-align: left;
            font-size: 14px;
            color: #333;
        }

        @media (max-width: 600px) {
            .container {
                padding: 30px 20px;
            }

            h1 {
                font-size: 26px;
            }

            .logo {
                font-size: 48px;
            }

            .button-group {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="logo">💳</div>
            <h1>bKash Payment Gateway</h1>
            <p class="subtitle">
                সহজ এবং নিরাপদ অনলাইন পেমেন্ট সমাধান<br />
                বাংলাদেশের জন্য সবচেয়ে জনপ্রিয় পেমেন্ট পদ্ধতি
            </p>

            <div class="info-box">
                ℹ️ কোন প্রি-রেজিস্ট্রেশন প্রয়োজন নেই। যেকোনো bKash অ্যাকাউন্ট থেকে সরাসরি পেমেন্ট করুন।
            </div>

            <div class="features">
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>কোন প্রি-রেজিস্ট্রেশন প্রয়োজন নেই</div>
                </div>
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>সরাসরি মোবাইল নাম্বার দিয়ে পেমেন্ট করুন</div>
                </div>
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>One-Click Payment সুবিধা</div>
                </div>
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>সম্পূর্ণ নিরাপদ এবং এনক্রিপ্টেড</div>
                </div>
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>তাৎক্ষণিক পেমেন্ট কনফার্মেশন</div>
                </div>
                <div class="feature-item">
                    <div class="feature-icon">✓</div>
                    <div>সকল পেমেন্ট হিস্টরি দেখুন</div>
                </div>
            </div>

            <div class="button-group">
                <a href="/UI/Payment.aspx" class="btn btn-primary">💰 এখনই পেমেন্ট করুন</a>
                <a href="/UI/PaymentHistory.aspx" class="btn btn-secondary">📊 পেমেন্ট হিস্টরি দেখুন</a>
            </div>

            <div class="footer">
                <p>🔒 আপনার তথ্য সম্পূর্ণ নিরাপদ এবং bKash দ্বারা এনক্রিপ্টেড</p>
                <p style="margin-top: 10px;">© 2024 bKash Payment Gateway. সর্বস্বত্ব সংরক্ষিত।</p>
            </div>
        </div>
    </form>
</body>
</html>
