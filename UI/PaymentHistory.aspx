<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentHistory.aspx.cs" Inherits="bKashPayment.PaymentHistory" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>পেমেন্ট হিস্টরি</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #f5f7fa;
            padding: 20px;
        }

        .container {
            max-width: 1000px;
            margin: 0 auto;
        }

        .header {
            background: white;
            padding: 30px;
            border-radius: 10px;
            margin-bottom: 30px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        .header h1 {
            color: #333;
            margin-bottom: 10px;
        }

        .header p {
            color: #666;
            font-size: 14px;
        }

        .search-section {
            background: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        .search-group {
            display: flex;
            gap: 10px;
            margin-bottom: 15px;
        }

        .search-group input,
        .search-group select {
            flex: 1;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 14px;
        }

        .search-group button {
            padding: 10px 20px;
            background: #667eea;
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
        }

        .search-group button:hover {
            background: #764ba2;
        }

        .table-container {
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        thead {
            background: #f8f9fa;
            border-bottom: 2px solid #dee2e6;
        }

        th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #333;
            font-size: 14px;
        }

        td {
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            color: #666;
            font-size: 14px;
        }

        tbody tr:hover {
            background: #f8f9fa;
        }

        .status-badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-completed {
            background: #d4edda;
            color: #155724;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
        }

        .status-failed {
            background: #f8d7da;
            color: #721c24;
        }

        .status-cancelled {
            background: #e2e3e5;
            color: #383d41;
        }

        .amount {
            font-weight: 600;
            color: #333;
        }

        .empty-state {
            text-align: center;
            padding: 40px;
            color: #999;
        }

        .empty-state svg {
            width: 64px;
            height: 64px;
            margin-bottom: 20px;
            opacity: 0.5;
        }

        .pagination {
            display: flex;
            justify-content: center;
            gap: 5px;
            padding: 20px;
            background: white;
            border-radius: 10px;
            margin-top: 20px;
        }

        .pagination a,
        .pagination span {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            cursor: pointer;
            text-decoration: none;
            color: #667eea;
        }

        .pagination a:hover,
        .pagination .active {
            background: #667eea;
            color: white;
            border-color: #667eea;
        }

        .pagination .disabled {
            color: #ccc;
            cursor: not-allowed;
        }

        .action-buttons {
            display: flex;
            gap: 5px;
        }

        .action-buttons button {
            padding: 6px 12px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 12px;
            background: #667eea;
            color: white;
        }

        .action-buttons button:hover {
            background: #764ba2;
        }

        .stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 20px;
        }

        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            text-align: center;
        }

        .stat-label {
            color: #666;
            font-size: 12px;
            margin-bottom: 10px;
        }

        .stat-value {
            font-size: 24px;
            font-weight: bold;
            color: #667eea;
        }

        @media (max-width: 768px) {
            .search-group {
                flex-direction: column;
            }

            table {
                font-size: 12px;
            }

            th, td {
                padding: 10px;
            }

            .stats {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Header -->
            <div class="header">
                <h1>📊 পেমেন্ট হিস্টরি</h1>
                <p>আপনার সকল লেনদেনের তথ্য এখানে দেখুন</p>
            </div>

            <!-- Statistics -->
            <div class="stats">
                <div class="stat-card">
                    <div class="stat-label">মোট পেমেন্ট</div>
                    <div class="stat-value"><asp:Label ID="lblTotalTransactions" runat="server" Text="0"></asp:Label></div>
                </div>
                <div class="stat-card">
                    <div class="stat-label">সফল লেনদেন</div>
                    <div class="stat-value"><asp:Label ID="lblCompletedTransactions" runat="server" Text="0"></asp:Label></div>
                </div>
                <div class="stat-card">
                    <div class="stat-label">মোট পরিমাণ</div>
                    <div class="stat-value"><asp:Label ID="lblTotalAmount" runat="server" Text="0 ৳"></asp:Label></div>
                </div>
            </div>

            <!-- Search Section -->
            <div class="search-section">
                <h3>অনুসন্ধান করুন</h3>
                <div class="search-group">
                    <asp:TextBox ID="txtSearchPhone" runat="server" Placeholder="মোবাইল নাম্বার দিয়ে খুঁজুন"></asp:TextBox>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Value="">সব স্ট্যাটাস</asp:ListItem>
                        <asp:ListItem Value="COMPLETED">সফল</asp:ListItem>
                        <asp:ListItem Value="PENDING">অপেক্ষমাণ</asp:ListItem>
                        <asp:ListItem Value="FAILED">ব্যর্থ</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btnSearch" runat="server" Text="🔍 খুঁজুন" OnClick="BtnSearch_Click" />
                </div>
            </div>

            <!-- Payment History Table -->
            <div class="table-container">
                <asp:GridView ID="gvPayments" runat="server" CssClass="payments-table"
                    AutoGenerateColumns="false" DataKeyNames="TransactionID"
                    AllowPaging="true" PageSize="10" OnPageIndexChanging="GvPayments_PageIndexChanging">
                    <HeaderStyle BackColor="#f8f9fa" ForeColor="#333" Font-Bold="true" />
                    <Columns>
                        <asp:BoundField DataField="TransactionID" HeaderText="আইডি" />
                        <asp:BoundField DataField="Customer.PhoneNumber" HeaderText="মোবাইল নাম্বার" />
                        <asp:BoundField DataField="Amount" HeaderText="পরিমাণ" DataFormatString="{0:N2} ৳" />
                        <asp:BoundField DataField="Status" HeaderText="স্ট্যাটাস">
                            <ItemTemplate>
                                <asp:Label CssClass="status-badge" runat="server">
                                    <%# GetStatusBadgeClass(Eval("Status").ToString()) %>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:BoundField>
                        <asp:BoundField DataField="CreatedDate" HeaderText="সময়" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <p>কোন পেমেন্ট রেকর্ড পাওয়া যায়নি</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>

    <script>
        function GetStatusBadgeClass(status) {
            switch (status) {
                case 'COMPLETED':
                    return 'সফল';
                case 'PENDING':
                    return 'অপেক্ষমাণ';
                case 'FAILED':
                    return 'ব্যর্থ';
                default:
                    return status;
            }
        }
    </script>
</body>
</html>
