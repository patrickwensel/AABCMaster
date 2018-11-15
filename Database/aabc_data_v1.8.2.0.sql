/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.1.0
dym:TargetEndingVersion: 1.8.2.0
---------------------------------------------------------------------

	Insurance Details
	Payment Plans
	Parent Portal Payment Refinements
	
---------------------------------------------------------------------*/



CREATE TABLE [dbo].[CaseInsurances](
	[ID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[CaseID] [int] NULL,
	[InsuranceID] [int] NULL,
	[InsurancePhoneNumber] [nvarchar](50) NULL,
	[MemberID] [nvarchar](50) NULL,
	[MemberName] [nvarchar](50) NULL,
	[PrimaryCardholderName] [nvarchar](50) NULL,
	[PrimaryCardholderDOB] [datetime] NULL,
	[DatePlanEffective] [datetime] NULL,
	[DatePlanTerminated] [datetime] NULL,
	[FundingType] [nvarchar](20) NULL,
	[BenefitType] [nvarchar](20) NULL,
	[CoPayAmount] [money] NULL,
	[CoInsuranceAmount] [money] NULL,
	[DeductibleTotal] [money] NULL,
	[MaxOutOfPocket] [money] NULL,
	[DeductibleDateTo] [datetime] NULL,
	[DeductibleToDate] [money] NULL,
	[MaxOutOfPocketDateTo] [datetime] NULL,
	[MaxOutOfPocketToDate] [money] NULL,
	[OtherNotes] [nvarchar](max) NULL,
	[HardshipWaiverLike] [bit] NULL,
	[HardshipWaiverApplied] [bit] NULL,
	[HardshipWaiverApproved] [bit] NULL,
	[PaymentPlanLikeToDiscuss] [bit] NULL,
	[PaymentPlanStartDate] [datetime] NULL,
	[PaymentPlanMonthlyAmount] [money] NULL,
	[PaymentPlanMethodOfPayment] [nvarchar](20) NULL,
	[Active] [bit] NULL
) 

GO

ALTER TABLE [dbo].[CaseInsurances]  WITH CHECK ADD  CONSTRAINT [FK_CaseInsurances_Cases] FOREIGN KEY([CaseID])
REFERENCES [dbo].[Cases] ([ID])
GO

ALTER TABLE [dbo].[CaseInsurances] CHECK CONSTRAINT [FK_CaseInsurances_Cases]
GO

ALTER TABLE [dbo].[CaseInsurances]  WITH CHECK ADD  CONSTRAINT [FK_CaseInsurances_Insurances] FOREIGN KEY([InsuranceID])
REFERENCES [dbo].[Insurances] ([ID])
GO

ALTER TABLE [dbo].[CaseInsurances] CHECK CONSTRAINT [FK_CaseInsurances_Insurances]
GO










CREATE TABLE [dbo].[CasePaymentPlans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Amount] [money] NOT NULL,
	[Frequency] [nvarchar](20) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CasePaymentPlans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

ALTER TABLE [dbo].[CasePaymentPlans]  WITH CHECK ADD  CONSTRAINT [FK_CasePaymentPlans_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([ID])
GO

ALTER TABLE [dbo].[CasePaymentPlans] CHECK CONSTRAINT [FK_CasePaymentPlans_Cases]
GO










-- ============================
-- Cases with Auths but no Hours
-- (Alter to ignore historic cases)
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithAuthsButNotHours](@StartDate DATETIME2, @EndDate DATETIME2) AS

	/* -- TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-08-01'
	SET @EndDate = '2016-08-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName,
		cmp.WatchComment,
		cmp.WatchIgnore
	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	LEFT JOIN dbo.CaseMonthlyPeriods AS cmp 
		ON cmp.CaseID = c.ID
		AND cmp.PeriodFirstDayOfMonth = @StartDate
	INNER JOIN (
	
		SELECT 
			c.ID AS CaseID
		FROM dbo.Cases AS c
		INNER JOIN dbo.CaseAuthCodes AS cac ON cac.CaseID = c.ID
		LEFT JOIN dbo.CaseAuthHours AS cah ON cah.CaseID = c.ID

		WHERE cac.AuthStartDate <= @EndDate
			AND cac.AuthEndDate <= @StartDate

		GROUP BY c.ID

		HAVING COUNT(cah.ID) = 0

	) AS unmatched ON unmatched.CaseID = c.ID

	WHERE c.CaseStatus >= 0

	RETURN

GO













GO

EXEC meta.UpdateVersion '1.8.2.0';
GO












