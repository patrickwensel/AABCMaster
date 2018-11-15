/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.9.0.0
dym:TargetEndingVersion: 1.9.1.0
---------------------------------------------------------------------

	
	Exports schema and proc
	RBT Payment Transaction Log
	
---------------------------------------------------------------------*/




CREATE TABLE [dbo].[TransactionLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Phone] [varchar](50) NOT NULL,
	[Email] [varchar](200) NOT NULL,
	[TransactionId] [varchar](50) NULL,
 CONSTRAINT [PK_TransactionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TransactionLog] ADD  CONSTRAINT [DF_TransactionLog_Date]  DEFAULT (getdate()) FOR [Date]
GO




CREATE SCHEMA exports;
GO

CREATE PROCEDURE exports.ProviderPayByCaseByMonth (@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN

	 /* TEST DATA
	DECLARE @StartDate DATETIME2 = '2015-01-01'
	DECLARE @EndDate DATETIME2 = '2999-01-01'
	-- */

	SELECT

		CAST(YEAR(cah.HoursDate) AS VARCHAR(4)) + '-' + RIGHT('00' + CAST(MONTH(cah.HoursDate) AS VARCHAR(2)), 2) AS HoursPeriod,
		p.ID AS ProviderID,
		c.ID AS CaseID,
		pt.ID AS PatientID,
		i.ID AS InsuranceID,
		ptypes.ID AS ProviderTypeID,
		ptypes.ProviderTypeCode AS ProviderType,
		i.InsuranceName,
		p.ProviderFirstName,
		p.ProviderLastName,
		pt.PatientFirstName,
		pt.PatientLastName,		
		SUM(cah.HoursPayable) AS PayableHours,
		p.ProviderRate,
		SUM(cah.HoursPayable * COALESCE(p.ProviderRate, 0)) AS AmountPaid
	
	

	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
	LEFT JOIN dbo.Insurances AS i ON i.ID = pt.PatientInsuranceID
	LEFT JOIN dbo.ProviderTypes AS ptypes ON ptypes.ID = p.ProviderType

	WHERE cah.HoursStatus = 3 -- finalized only
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate

	GROUP BY
		CAST(YEAR(cah.HoursDate) AS VARCHAR(4)) + '-' + RIGHT('00' + CAST(MONTH(cah.HoursDate) AS VARCHAR(2)), 2),
		p.ID,
		c.ID,
		pt.ID,
		i.ID,
		ptypes.ID,
		ptypes.ProviderTypeCode,
		i.InsuranceName,
		p.ProviderFirstName,
		p.ProviderLastName,
		pt.PatientFirstName,
		pt.PatientLastName,
		p.ProviderRate;

	RETURN

END 
GO
	



GO
EXEC meta.UpdateVersion '1.9.1.0';
GO

