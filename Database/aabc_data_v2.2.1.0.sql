/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.0.0
dym:TargetEndingVersion: 2.2.1.0
---------------------------------------------------------------------

	Payment Info, PP Users, Referral Transfer
	
---------------------------------------------------------------------*/



-- ========================  
-- Add a view for Provider Portal Users
-- ======================== 

CREATE VIEW dbo.ProviderPortalUserAdminList AS
	SELECT
		p.ID AS ProviderID,
		u.AspNetUserID,
		p.ProviderFirstName AS FirstName,
		p.ProviderLastName AS LastName,
		p.ProviderPrimaryPhone AS Phone,
		p.ProviderPrimaryEmail AS Email,
		u.ProviderUserNumber AS UserNumber,
		u.ProviderHasAppAccess AS HasAppAccess
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderPortalUsers AS u ON u.ProviderID = p.ID;
GO

-- ========================  
-- Update Patient/Case field sizes to match Referral field sizes
-- ======================== 

-- Alter Phone column length to avoid issue with copying Referral to Patient
ALTER TABLE dbo.Patients
ALTER COLUMN PatientPhone nvarchar(40)
GO
-- Alter Notes column length to avoid issue with copying Referral to Cases
ALTER TABLE dbo.Cases
ALTER COLUMN CaseRequiredHoursNotes nvarchar(2000)
GO


-- ========================  
-- Update Payments info
-- ======================== 

ALTER TABLE [dbo].[Payments] ADD [ManagementUserId] [int]
CREATE INDEX [IX_ManagementUserId] ON [dbo].[Payments]([ManagementUserId])
ALTER TABLE [dbo].[Payments] ADD CONSTRAINT [FK_dbo.Payments_dbo.WebUsers_ManagementUserId] FOREIGN KEY ([ManagementUserId]) REFERENCES [dbo].[WebUsers] ([ID])

ALTER TABLE [dbo].[PaymentCharges] ADD [Result] [varchar](100)
ALTER TABLE [dbo].[PaymentCharges] ADD [ResultDetails] [varchar](MAX)

ALTER TABLE [dbo].[Payments] ADD [PaymentType2] [int]
ALTER TABLE [dbo].[Payments] ADD [RecurringFrequency2] [int]
GO

ALTER TABLE dbo.Payments ALTER COLUMN RecurringFrequency NVARCHAR(20) NULL;
GO

UPDATE dbo.Payments SET PaymentType2 = 0 WHERE PaymentType = 'onetime';
UPDATE dbo.Payments SET PaymentType2 = 1 WHERE PaymentType <> 'onetime';
UPDATE dbo.Payments SET RecurringFrequency2 = 0 WHERE RecurringFrequency = 'weekly';
UPDATE dbo.Payments SET RecurringFrequency2 = 1 WHERE RecurringFrequency <> 'weekly';

UPDATE dbo.Payments
                    SET
                    RecurringFrequency = NULL,
                    RecurringDateStart = NULL,
                    RecurringDateEnd = NULL
                    WHERE PaymentType2 = 0
UPDATE dbo.Payments
                    SET
                    OneTimePaymentDate = NULL
                    WHERE PaymentType2 = 1
GO



-- drop any constraints on paymenttype2 and make it not null
DECLARE @var0 nvarchar(128)

SELECT @var0 = name
	FROM sys.default_constraints
	WHERE parent_object_id = object_id(N'dbo.Payments')
		AND col_name(parent_object_id, parent_column_id) = 'PaymentType2';

IF @var0 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Payments] DROP CONSTRAINT [' + @var0 + ']')
GO
ALTER TABLE [dbo].[Payments] ALTER COLUMN [PaymentType2] [int] NOT NULL
GO



-- Drop constaints on PaymentType, drop the column, and rename PaymentType2 to PaymentType
DECLARE @var1 nvarchar(128)

SELECT @var1 = name
	FROM sys.default_constraints
	WHERE parent_object_id = object_id(N'dbo.Payments')
		AND col_name(parent_object_id, parent_column_id) = 'PaymentType';

IF @var1 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Payments] DROP CONSTRAINT [' + @var1 + ']')
GO

ALTER TABLE [dbo].[Payments] DROP COLUMN [PaymentType]
GO

EXECUTE sp_rename @objname = N'dbo.Payments.PaymentType2', @newname = N'PaymentType', @objtype = N'COLUMN'
GO





-- Drop RecurringFrequency and replace/rename with RecurringFrequency2
DECLARE @var2 nvarchar(128)

SELECT @var2 = name
	FROM sys.default_constraints
	WHERE parent_object_id = object_id(N'dbo.Payments')
		AND col_name(parent_object_id, parent_column_id) = 'RecurringFrequency';
IF @var2 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Payments] DROP CONSTRAINT [' + @var2 + ']')
GO
ALTER TABLE [dbo].[Payments] DROP COLUMN [RecurringFrequency]
GO
EXECUTE sp_rename @objname = N'dbo.Payments.RecurringFrequency2', @newname = N'RecurringFrequency', @objtype = N'COLUMN'
GO




-- Clean up expiry month column
DECLARE @var3 nvarchar(128)

SELECT @var3 = name
	FROM sys.default_constraints
	WHERE parent_object_id = object_id(N'dbo.CreditCards')
		AND col_name(parent_object_id, parent_column_id) = 'ExpiryMonth';
IF @var3 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[CreditCards] DROP CONSTRAINT [' + @var3 + ']')
GO
ALTER TABLE [dbo].[CreditCards] ALTER COLUMN [ExpiryMonth] [varchar](2) NULL
GO




DECLARE @var4 nvarchar(128)

SELECT @var4 = name
	FROM sys.default_constraints
	WHERE parent_object_id = object_id(N'dbo.CreditCards')
		AND col_name(parent_object_id, parent_column_id) = 'ExpiryYear';
IF @var4 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[CreditCards] DROP CONSTRAINT [' + @var4 + ']')
GO
ALTER TABLE [dbo].[CreditCards] ALTER COLUMN [ExpiryYear] [varchar](4) NULL
GO




GO
EXEC meta.UpdateVersion '2.2.1.0'
GO

