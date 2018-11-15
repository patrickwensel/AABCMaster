/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.4.0
dym:TargetEndingVersion: 1.8.0.0
---------------------------------------------------------------------

	Auth Views
	Case Notes Completed Tracking
	Payment Portal	
	
---------------------------------------------------------------------*/





DROP VIEW [dbo].[AppliedAuthorizationAndInsuranceMismatches];
GO


/* ========================================
 Get a list of authorizations that have been
 applied to cases, but which don't have a
 corresponding authorization available in the
 insurance's auth match rules
=========================================== */
CREATE VIEW [dbo].[AppliedAuthorizationAndInsuranceMismatches] AS

	SELECT 
		TempKey = ROW_NUMBER() OVER (ORDER BY patientInfo.CaseID, patientInfo.AuthCodeID),
		patientInfo.*
		--, insuranceInfo.*
	FROM (

		-- patientInfo
		-- get all the cases and auths applied, as well as insurance info
		SELECT 
			p.PatientFirstName,
			p.PatientLastName,
			p.PatientInsuranceID,
			i.InsuranceName,
			c.ID AS CaseID,
			cac.ID AS CaseAuthCodeID,
			cac.AuthCodeID,
			cac.AuthStartDate, 
			cac.AuthEndDate,
			ac.CodeCode,
			ac.CodeDescription
		FROM dbo.Cases AS c
		INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
		INNER JOIN dbo.CaseAuthCodes AS cac ON c.ID = cac.CaseID
		INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID
		LEFT JOIN dbo.Insurances AS i ON p.PatientInsuranceID = i.ID

	) AS patientInfo

	LEFT JOIN (
	
		-- get a list of all insurances and their applicable auth codes
		SELECT 
			i.ID AS InsuranceID,
			i.InsuranceName,
			amr.RuleFinalAuthID AS AuthID
		FROM dbo.Insurances AS i
		INNER JOIN dbo.AuthMatchRules AS amr ON amr.InsuranceID = i.ID
		WHERE amr.RuleFinalAuthID IS NOT NULL

		UNION

		SELECT 
			i.ID AS InsuranceID,
			i.InsuranceName,
			amr.RuleInitialAuthID AS AuthID
		FROM dbo.Insurances AS i
		INNER JOIN dbo.AuthMatchRules AS amr ON amr.InsuranceID = i.ID
		WHERE amr.RuleInitialAuthID IS NOT NULL

	) AS insuranceInfo ON patientInfo.PatientInsuranceID = insuranceInfo.InsuranceID AND patientInfo.AuthCodeID = insuranceInfo.AuthID

	WHERE insuranceInfo.InsuranceID IS NULL

	-- ORDER BY patientInfo.PatientLastName, patientInfo.PatientFirstName, patientInfo.AuthStartDate
	;


GO







-- ============================
-- Procedure to view the application of time 
-- to different auths for hours of a case
-- ============================
CREATE PROCEDURE dbo.GetHoursAuthBreakdownByCase(@StartDate DATE, @EndDate DATE, @CaseID INT) AS

	 /*	TEST DATA
	DECLARE @StartDate DATE = '2017-02-01'
	DECLARE @EndDate DATE = '2017-03-01'
	DECLARE @CaseID INT = 491
	-- */

	SELECT 
		hoursBase.*,
		breakdowns.AuthCode,
		breakdowns.Minutes
	FROM (
		SELECT 
			ROW_NUMBER() OVER (ORDER BY cah.HoursDate) AS HoursNumber,
			cah.ID AS HoursID,
			cah.HoursDate,
			p.ProviderFirstName + ' ' + p.ProviderLastName AS ProviderName,
			s.ServiceCode,
			pt.ProviderTypeCode AS ProviderType
		FROM dbo.CaseAuthHours AS cah
		INNER JOIN dbo.Services AS s ON s.ID = cah.HoursServiceID
		INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
		INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType

		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate < @EndDate
			AND cah.CaseID = @CaseID

	) AS hoursBase

	INNER JOIN (

		SELECT 
			bd.HoursID,
			ac.CodeCode AS AuthCode,
			bd.Minutes
		FROM dbo.CaseAuthHoursBreakdown AS bd
		INNER JOIN dbo.CaseAuthCodes AS cac ON cac.ID = bd.CaseAuthID
		INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID

	) AS breakdowns ON breakdowns.HoursID = hoursBase.HoursID

	ORDER BY hoursBase.HoursDate;

GO












-- ============================
-- 	Add completed tracking
-- ============================
ALTER TABLE dbo.CaseNoteTasks ADD CompletedDate DATETIME2;
GO








-- ============================
-- Add Patient Portal Payment Info
-- ============================

CREATE TABLE [dbo].[CreditCards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoginId] [int] NOT NULL,
	[CardType] [nvarchar](50) NULL,
	[Cardholder] [nvarchar](50) NOT NULL,
	[CardNumber] [nvarchar](50) NOT NULL,
	[ExpiryMonth] [int] NOT NULL,
	[ExpiryYear] [int] NOT NULL,
	[GatewayType] [nvarchar](50) NULL,
	[GatewayCardId] [nvarchar](50) NULL,
 CONSTRAINT [PK_CreditCards] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PaymentCharges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoginId] [int] NOT NULL,
	[PaymentId] [int] NOT NULL,
	[ReferenceType] [nvarchar](50) NOT NULL,
	[ReferenceId] [int] NOT NULL,
	[CreditCardId] [int] NULL,
	[ChargeDate] [datetime] NOT NULL,
	[GatewayChargeId] [nvarchar](50) NOT NULL,
	[Amount] [money] NOT NULL,
	[Description] [nvarchar](100) NULL,
 CONSTRAINT [PK_PaymentCharges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoginId] [int] NOT NULL,
	[PaymentType] [nvarchar](20) NOT NULL,
	[RecurringFrequency] [nvarchar](20) NOT NULL,
	[PatientId] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[OneTimePaymentDate] [datetime] NULL,
	[RecurringDateStart] [datetime] NULL,
	[RecurringDateEnd] [datetime] NULL,
	[Active] [bit] NOT NULL,
	[CreditCardId] [int] NULL,
 CONSTRAINT [PK__Payments__3214EC071247577C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[PaymentSchedules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentId] [int] NOT NULL,
	[ScheduleNumber] [int] NOT NULL,
	[ScheduledDate] [date] NOT NULL,
	[PaymentChargeId] [int] NULL,
 CONSTRAINT [PK_PaymentSchedules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[CreditCards]  WITH CHECK ADD  CONSTRAINT [FK_CreditCards_PatientPortalLogins] FOREIGN KEY([LoginId])
REFERENCES [dbo].[PatientPortalLogins] ([ID])
GO
ALTER TABLE [dbo].[CreditCards] CHECK CONSTRAINT [FK_CreditCards_PatientPortalLogins]
GO
ALTER TABLE [dbo].[PaymentCharges]  WITH CHECK ADD  CONSTRAINT [FK_PaymentCharges_CreditCards] FOREIGN KEY([CreditCardId])
REFERENCES [dbo].[CreditCards] ([Id])
GO
ALTER TABLE [dbo].[PaymentCharges] CHECK CONSTRAINT [FK_PaymentCharges_CreditCards]
GO
ALTER TABLE [dbo].[PaymentCharges]  WITH CHECK ADD  CONSTRAINT [FK_PaymentCharges_PatientPortalLogins] FOREIGN KEY([LoginId])
REFERENCES [dbo].[PatientPortalLogins] ([ID])
GO
ALTER TABLE [dbo].[PaymentCharges] CHECK CONSTRAINT [FK_PaymentCharges_PatientPortalLogins]
GO
ALTER TABLE [dbo].[PaymentCharges]  WITH CHECK ADD  CONSTRAINT [FK_PaymentCharges_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[PaymentCharges] CHECK CONSTRAINT [FK_PaymentCharges_Payments]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_PatientPortalLogins] FOREIGN KEY([LoginId])
REFERENCES [dbo].[PatientPortalLogins] ([ID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_PatientPortalLogins]
GO
ALTER TABLE [dbo].[PaymentSchedules]  WITH CHECK ADD  CONSTRAINT [FK_PaymentSchedules_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[PaymentSchedules] CHECK CONSTRAINT [FK_PaymentSchedules_Payments]
GO










EXEC meta.UpdateVersion '1.8.0.0';
GO












