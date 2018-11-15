/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.5.0
dym:TargetEndingVersion: 1.8.6.0
---------------------------------------------------------------------

	
---------------------------------------------------------------------*/



CREATE TABLE [dbo].[CaseInsurancePayments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CaseInsuranceId] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[PaymentDate] [datetime] NOT NULL,
	[Description] [nvarchar](50) NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

ALTER TABLE [dbo].[CaseInsurancePayments]  WITH CHECK ADD  CONSTRAINT [FK_CaseInsurancePayments_CaseInsurances] FOREIGN KEY([CaseInsuranceId])
REFERENCES [dbo].[CaseInsurances] ([ID])
GO

ALTER TABLE [dbo].[CaseInsurancePayments] CHECK CONSTRAINT [FK_CaseInsurancePayments_CaseInsurances]
GO






-- ========================
-- Case Billing Correspondence
-- ========================
CREATE TABLE dbo.CaseBillingCorrespondences
	(
	Id int NOT NULL IDENTITY (1, 1),
	CaseId int NOT NULL,
	CorrespondenceDate datetime NOT NULL,
	CorrespondenceTypeId int NOT NULL,
	ContactPerson nvarchar(50) NULL,
	StaffId int NOT NULL,
	Notes nvarchar(2000) NULL
	)  
GO
ALTER TABLE dbo.CaseBillingCorrespondences ADD CONSTRAINT
	FK_CaseBillingCorrespondences_Cases FOREIGN KEY
	(
	CaseId
	) REFERENCES dbo.Cases
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.CaseBillingCorrespondences ADD CONSTRAINT
	FK_CaseBillingCorrespondences_Staff FOREIGN KEY
	(
	StaffId
	) REFERENCES dbo.Staff
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
CREATE TABLE dbo.CaseBillingCorrespondenceTypes
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name nvarchar(50) NOT NULL
	)
GO
ALTER TABLE dbo.CaseBillingCorrespondenceTypes ADD CONSTRAINT
	PK_CaseBillingCorrespondenceTypes PRIMARY KEY CLUSTERED 
	(
	Id
	)

GO
ALTER TABLE dbo.CaseBillingCorrespondences ADD CONSTRAINT
	FK_CaseBillingCorrespondences_CaseBillingCorrespondenceTypes FOREIGN KEY
	(
	CorrespondenceTypeId
	) REFERENCES dbo.CaseBillingCorrespondenceTypes
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
SET IDENTITY_INSERT [dbo].[CaseBillingCorrespondenceTypes] ON 
GO
INSERT [dbo].[CaseBillingCorrespondenceTypes] ([Id], [Name]) VALUES (1, N'Email')
GO
INSERT [dbo].[CaseBillingCorrespondenceTypes] ([Id], [Name]) VALUES (2, N'Phone')
GO
SET IDENTITY_INSERT [dbo].[CaseBillingCorrespondenceTypes] OFF
GO
Alter Table CaseBillingCorrespondences add AttachmentFilename nvarchar(200) NULL
go
Alter Table Patients add HighRisk bit NULL
go






-- ========================
-- Analytics/Reporting
-- ========================
DROP VIEW dbo.FailedHoursBreakdowns;
GO

CREATE VIEW dbo.FailedHoursBreakdowns AS

	SELECT  
		cah.ID AS HourID,
		cah.CaseID,
		cah.CaseProviderID AS ProviderID,
		cah.HoursDate,
		cah.HoursStatus,
		cah.HoursBillable,
		cah.HoursServiceID,
		p.PatientInsuranceID AS InsuranceID,
		s.ServiceCode,
		i.InsuranceName

	FROM dbo.CaseAuthHours AS cah
	LEFT JOIN dbo.CaseAuthHoursBreakdown AS cahb ON cah.ID = cahb.HoursID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	LEFT JOIN dbo.Services AS s ON s.ID = cah.HoursServiceID
	LEFT JOIN dbo.Insurances AS i ON i.ID = p.PatientInsuranceID

	WHERE cahb.ID IS NULL
		AND cah.HoursBillable > 0.25
		AND cah.HoursDate >= '2017-01-01'
	-- ORDER BY cah.HoursDate DESC
	;
	

GO

CREATE VIEW dbo.HoursTotalsByPatientByMonth AS
	SELECT 
		p.PatientFirstName,
		p.PatientLastName,
		YEAR(cah.HoursDate) AS HoursYear,
		MONTH(cah.HoursDate) AS HoursMonth,
		SUM(cah.HoursBillable) AS SumOfHoursBillable,
		SUM(cah.HoursPayable) AS SumOfHoursPayable,
		SUM(cah.HoursTotal) AS SumOfHoursTotal
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON cah.CaseID = c.ID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	GROUP BY 
		p.PatientFirstName,
		p.PatientLastName,
		YEAR(cah.HoursDate),
		MONTH(cah.HoursDate)
	;
GO




CREATE VIEW [dbo].[HoursTotalsByPatientByProviderTypeByMonth] AS
	SELECT 
		p.PatientFirstName,
		p.PatientLastName,
		pt.ProviderTypeCode AS ProviderType,
		YEAR(cah.HoursDate) AS HoursYear,
		MONTH(cah.HoursDate) AS HoursMonth,
		SUM(cah.HoursBillable) AS SumOfHoursBillable,
		SUM(cah.HoursPayable) AS SumOfHoursPayable,
		SUM(cah.HoursTotal) AS SumOfHoursTotal
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON cah.CaseID = c.ID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	INNER JOIN dbo.Providers AS pv ON cah.CaseProviderID = pv.ID
	INNER JOIN dbo.ProviderTypes AS pt ON pv.ProviderType = pt.ID
	GROUP BY 
		p.PatientFirstName,
		p.PatientLastName,
		pt.ProviderTypeCode,
		YEAR(cah.HoursDate),
		MONTH(cah.HoursDate)
	;

GO




CREATE VIEW [dbo].[HoursTotalsByPatientByProviderMonth] AS
	SELECT 
		p.PatientFirstName,
		p.PatientLastName,
		pt.ProviderTypeCode AS ProviderType,
		pv.ProviderFirstName,
		pv.ProviderLastName,
		YEAR(cah.HoursDate) AS HoursYear,
		MONTH(cah.HoursDate) AS HoursMonth,
		SUM(cah.HoursBillable) AS SumOfHoursBillable,
		SUM(cah.HoursPayable) AS SumOfHoursPayable,
		SUM(cah.HoursTotal) AS SumOfHoursTotal
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON cah.CaseID = c.ID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	INNER JOIN dbo.Providers AS pv ON cah.CaseProviderID = pv.ID
	INNER JOIN dbo.ProviderTypes AS pt ON pv.ProviderType = pt.ID
	GROUP BY 
		p.PatientFirstName,
		p.PatientLastName,
		pt.ProviderTypeCode,
		pv.ProviderFirstName,
		pv.ProviderLastName,
		YEAR(cah.HoursDate),
		MONTH(cah.HoursDate)
	;


GO





CREATE VIEW HoursBreakdownsFailurePercentageByMonth AS 
	SELECT 
		results.DateGroup,
		COUNT(*) AS TotalEntries,
		SUM(results.HasBreakdown) AS TotalWithBreakdown,
		COUNT(*) - SUM(results.HasBreakdown) AS TotalWithoutBreakdown,
		CAST(100 * ((CAST(COUNT(*) - SUM(results.HasBreakdown) AS DECIMAL(10,4))) / CAST(COUNT(*) AS DECIMAL(10,4))) AS INT) AS FailurePercentage
	FROM (
		SELECT
			DATEADD(MONTH, DATEDIFF(MONTH, 0, cah.HoursDate), 0) AS DateGroup,
			cah.ID,
			CASE WHEN b.HoursID IS NOT NULL THEN 1 ELSE 0 END AS HasBreakdown	
		FROM dbo.CaseAuthHours AS cah
		LEFT JOIN (		
			SELECT cahb.HoursID, COUNT(*) AS TotalCount
			FROM dbo.CaseAuthHoursBreakdown AS cahb
			GROUP BY cahb.HoursID
		) AS b ON cah.ID = b.HoursID
	--	WHERE cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
	) AS results
	GROUP BY results.DateGroup;
	
	
	
	
GO
	
	
	






GO
EXEC meta.UpdateVersion '1.8.6.0';
GO

