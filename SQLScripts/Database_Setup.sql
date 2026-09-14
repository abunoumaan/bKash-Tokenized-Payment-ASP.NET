-- bKash Payment Database Setup Script
-- SQL Server এ এই স্ক্রিপ্ট চালান

-- ডাটাবেস তৈরি করুন
CREATE DATABASE bKashPayment;

USE bKashPayment;
GO

-- ১. কাস্টমার টেবিল
CREATE TABLE [dbo].[Customers] (
    [CustomerID] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [PhoneNumber] NVARCHAR(15) NOT NULL UNIQUE,
    [DateCreated] DATETIME DEFAULT GETDATE(),
    [IsActive] BIT DEFAULT 1
);

-- ২. bKash Agreement টেবিল
CREATE TABLE [dbo].[BkashAgreements] (
    [AgreementID] INT PRIMARY KEY IDENTITY(1,1),
    [CustomerID] INT NOT NULL,
    [AgreementToken] NVARCHAR(MAX) NOT NULL,
    [PayerReference] NVARCHAR(100) NOT NULL,
    [Status] NVARCHAR(50) DEFAULT 'ACTIVE',  -- ACTIVE, INACTIVE, REVOKED
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [UpdatedDate] DATETIME DEFAULT GETDATE(),
    [ExpiryDate] DATETIME,
    FOREIGN KEY ([CustomerID]) REFERENCES [Customers]([CustomerID]) ON DELETE CASCADE
);

-- ৩. পেমেন্ট ট্রানজ্যাকশন টেবিল
CREATE TABLE [dbo].[PaymentTransactions] (
    [TransactionID] INT PRIMARY KEY IDENTITY(1,1),
    [CustomerID] INT NOT NULL,
    [AgreementID] INT NOT NULL,
    [Amount] DECIMAL(10, 2) NOT NULL,
    [Currency] NVARCHAR(5) DEFAULT 'BDT',
    [PaymentID] NVARCHAR(100) NOT NULL UNIQUE,
    [Status] NVARCHAR(50) NOT NULL,  -- PENDING, COMPLETED, FAILED, CANCELLED
    [Reference] NVARCHAR(100),
    [ErrorMessage] NVARCHAR(MAX),
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [CompletedDate] DATETIME,
    [Remarks] NVARCHAR(MAX),
    FOREIGN KEY ([CustomerID]) REFERENCES [Customers]([CustomerID]) ON DELETE CASCADE,
    FOREIGN KEY ([AgreementID]) REFERENCES [BkashAgreements]([AgreementID])
);

-- ৪. API লগ টেবিল (ডিবাগিং এর জন্য)
CREATE TABLE [dbo].[ApiLogs] (
    [LogID] INT PRIMARY KEY IDENTITY(1,1),
    [CustomerID] INT,
    [ApiEndpoint] NVARCHAR(200) NOT NULL,
    [Method] NVARCHAR(10) NOT NULL,  -- GET, POST, PUT, DELETE
    [RequestData] NVARCHAR(MAX),
    [ResponseData] NVARCHAR(MAX),
    [StatusCode] INT,
    [ErrorMessage] NVARCHAR(MAX),
    [CreatedDate] DATETIME DEFAULT GETDATE()
);

-- ৫. ইন্ডেক্স তৈরি করুন (পারফরম্যান্সের জন্য)
CREATE INDEX IX_Customers_Email ON [dbo].[Customers]([Email]);
CREATE INDEX IX_Customers_PhoneNumber ON [dbo].[Customers]([PhoneNumber]);
CREATE INDEX IX_BkashAgreements_CustomerID ON [dbo].[BkashAgreements]([CustomerID]);
CREATE INDEX IX_BkashAgreements_Status ON [dbo].[BkashAgreements]([Status]);
CREATE INDEX IX_PaymentTransactions_CustomerID ON [dbo].[PaymentTransactions]([CustomerID]);
CREATE INDEX IX_PaymentTransactions_Status ON [dbo].[PaymentTransactions]([Status]);
CREATE INDEX IX_PaymentTransactions_CreatedDate ON [dbo].[PaymentTransactions]([CreatedDate]);
CREATE INDEX IX_ApiLogs_CreatedDate ON [dbo].[ApiLogs]([CreatedDate]);

-- ৬. ভিউ তৈরি করুন (রিপোর্টিং এর জন্য)
CREATE VIEW [dbo].[vw_CustomerPaymentHistory] AS
SELECT 
    c.[CustomerID],
    c.[Name],
    c.[Email],
    c.[PhoneNumber],
    pt.[TransactionID],
    pt.[Amount],
    pt.[Status],
    pt.[CreatedDate],
    pt.[CompletedDate],
    ba.[AgreementToken]
FROM [Customers] c
LEFT JOIN [BkashAgreements] ba ON c.[CustomerID] = ba.[CustomerID]
LEFT JOIN [PaymentTransactions] pt ON c.[CustomerID] = pt.[CustomerID];

GO

-- ৭. স্টোরড প্রসিডিউর - কাস্টমার তথ্য পেতে
CREATE PROCEDURE [dbo].[sp_GetCustomerByEmail]
    @Email NVARCHAR(100)
AS
BEGIN
    SELECT * FROM [Customers] WHERE [Email] = @Email AND [IsActive] = 1;
END;

GO

-- ৮. স্টোরড প্রসিডিউর - কাস্টমারের Agreement পেতে
CREATE PROCEDURE [dbo].[sp_GetActiveAgreement]
    @CustomerID INT
AS
BEGIN
    SELECT TOP 1 * FROM [BkashAgreements] 
    WHERE [CustomerID] = @CustomerID AND [Status] = 'ACTIVE'
    ORDER BY [CreatedDate] DESC;
END;

GO

-- ৯. স্টোরড প্রসিডিউর - পেমেন্ট হিস্টরি পেতে
CREATE PROCEDURE [dbo].[sp_GetPaymentHistory]
    @CustomerID INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SELECT * FROM [PaymentTransactions]
    WHERE [CustomerID] = @CustomerID
    ORDER BY [CreatedDate] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;

GO

-- ১০. সাম্পল ডেটা সংযোজন (টেস্টিং এর জন্য)
INSERT INTO [dbo].[Customers] ([Name], [Email], [PhoneNumber])
VALUES 
    (N'আবু নোমান', 'abunoumaan@example.com', '01700000001'),
    (N'টেস্ট কাস্টমার', 'test@example.com', '01700000002');

GO

PRINT 'ডাটাবেস সফলভাবে তৈরি হয়েছে!';
