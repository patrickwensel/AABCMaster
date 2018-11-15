/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.9.1.0
dym:TargetEndingVersion: 1.9.2.0
---------------------------------------------------------------------

	Insurance Cleanup
	Provider Pay Rates
	Provider Payables by Month
	Patient Hours Reports, Signoffs All Providers	
	
---------------------------------------------------------------------*/




-- =============================
-- Clean up insurance MOOP and Deductable unused fields 
-- =============================
ALTER TABLE dbo.CaseInsurances DROP COLUMN MaxOutOfPocket;
ALTER TABLE dbo.CaseInsurances DROP COLUMN DeductibleToDate;
ALTER TABLE dbo.CaseInsurances DROP COLUMN MaxOutOfPocketToDate;
ALTER TABLE dbo.CaseInsurances DROP COLUMN DeductibleDateTo;
ALTER TABLE dbo.CaseInsurances DROP COLUMN MaxOutOfPocketDateTo;
GO




-- =============================
-- Add tables for tracking provider rate history
-- =============================
CREATE TABLE dbo.ProviderRates (
	ID INT PRIMARY KEY CLUSTERED NOT NULL IDENTITY (1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProviderID INT NOT NULL REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	RateType INT NOT NULL DEFAULT 0, -- default Hourly (0: Hourly, 1: Salary)
	Rate MONEY NOT NULL,
	EffectiveDate DATETIME2 NOT NULL
	);
GO
CREATE UNIQUE INDEX idxProviderRatesUniqueEffectiveDate  ON dbo.ProviderRates (ProviderID, EffectiveDate);
CREATE INDEX idxProviderRateProviderID ON dbo.ProviderRates (ProviderID);
CREATE INDEX idxProviderRateEffectiveDate ON dbo.ProviderRates (EffectiveDate);
GO

-- Fill with rates from prior system
INSERT INTO dbo.ProviderRates (
    [ProviderID]
    ,[RateType]
    ,[Rate]
    ,[EffectiveDate])
SELECT ID, 0, ProviderRate, '1900-01-01' FROM dbo.Providers
WHERE ProviderRate IS NOT NULL;
GO

-- Drop the previously existing ProviderRate column
-- ALTER TABLE dbo.Providers DROP COLUMN ProviderRate
GO



-- =============================
-- Add provider rates per case and per service
-- =============================

CREATE TABLE dbo.ProviderCaseRates (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProviderID INT NOT NULL REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	CaseID INT NOT NULL REFERENCES dbo.Cases (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	HourlyRate MONEY NOT NULL,
	EffectiveDate DATETIME2 NOT NULL
);
CREATE UNIQUE INDEX idxCaseProviderRatesEffectiveDate ON dbo.ProviderCaseRates (ProviderID, CaseID, EffectiveDate);
CREATE INDEX idxProviderCaseRateProviderID ON dbo.ProviderCaseRates (ProviderID);
CREATE INDEX idxProviderCaseRateCaseID ON dbo.ProviderCaseRates (CaseID);
CREATE INDEX idxProviderCaseRateEffectiveDate ON dbo.ProviderCaseRates (EffectiveDate);
GO

CREATE TABLE dbo.ProviderServiceRates (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProviderID INT NOT NULL REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ServiceID INT NOT NULL REFERENCES dbo.Services (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	HourlyRate MONEY NOT NULL,
	EffectiveDate DATETIME2 NOT NULL
);
CREATE UNIQUE INDEX idxProviderServiceRatesUnique ON dbo.ProviderServiceRates (ProviderID, ServiceID, EffectiveDate);
CREATE INDEX idxProviderServiceRateProviderID ON dbo.ProviderServiceRates (ProviderID);
CREATE INDEX idxProviderServiceRateServiceID ON dbo.ProviderServiceRates (ServiceID);
CREATE INDEX idxProviderServiceRateEffectiveDate ON dbo.ProviderServiceRates (EffectiveDate);
GO




-- =============================
-- Create a view that will get all active rates per hours entry
-- =============================

CREATE VIEW dbo.ActiveRates AS

	SELECT 
		p.ID AS ProviderID,
		cah.ID AS HoursID,
		CASE 
			WHEN cr.HourlyRate IS NOT NULL THEN cr.HourlyRate
			WHEN sr.HourlyRate IS NOT NULL THEN sr.HourlyRate
			WHEN r.Rate IS NOT NULL THEN r.Rate
			WHEN p.ProviderRate IS NOT NULL THEN p.ProviderRate
			ELSE  NULL
		END AS AppliedRate,		
		CASE 
			WHEN cr.HourlyRate IS NOT NULL THEN 0
			WHEN sr.HourlyRate IS NOT NULL THEN 0
			WHEN r.Rate IS NOT NULL THEN r.RateType
			WHEN p.ProviderRate IS NOT NULL THEN 0
			ELSE NULL
		END AS RateType,
		CASE 
			WHEN cr.HourlyRate IS NOT NULL THEN 'CaseRate'
			WHEN sr.HourlyRate IS NOT NULL THEN 'ServiceRate'
			WHEN r.Rate IS NOT NULL THEN 'ProviderRates' 
			WHEN p.ProviderRate IS NOT NULL THEN 'LegacyRate'
			ELSE NULL 
		END AS RateSource

	FROM dbo.CaseAuthHours AS cah		
		INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
		OUTER APPLY ( -- case rates
			SELECT TOP 1 HourlyRate
			FROM dbo.ProviderCaseRates AS cr
			INNER JOIN dbo.Cases AS c ON c.ID = cr.CaseID
			WHERE cr.EffectiveDate <= cah.HoursDate
				AND cr.ProviderID = cah.CaseProviderID
			ORDER BY cr.EffectiveDate DESC
		) AS cr
		OUTER APPLY ( -- service rates
			SELECT TOP 1 HourlyRate
			FROM dbo.ProviderServiceRates AS sr
			WHERE sr.EffectiveDate <= cah.HoursDate
				AND sr.ProviderID = cah.CaseProviderID
			ORDER BY sr.EffectiveDate DESC
		) AS sr
		OUTER APPLY ( -- provider rates
			SELECT TOP 1 ID, Rate, RateType
			FROM dbo.ProviderRates AS pr
			WHERE pr.EffectiveDate <= cah.HoursDate
				AND pr.ProviderID = cah.CaseProviderID
			ORDER BY pr.EffectiveDate DESC
		) AS r;
		
GO



-- =============================
-- Update the dashboard query to include the new active rate
-- =============================
ALTER VIEW [dbo].[HoursProviderCost] AS
	SELECT
		pt.ID,
		cah.HoursDate,
		c.ID AS CaseID,
		pt.PatientInsuranceCompanyName,
		pt.PatientFirstName,
		pt.PatientLastName,
		pv.ProviderRate,
		cah.HoursTotal,
		ar.AppliedRate * cah.HoursPayable AS ProviderCost,
		pt.PatientInsuranceID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS pv ON pv.ID = cah.CaseProviderID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
	INNER JOIN dbo.ActiveRates AS ar ON cah.ID = ar.HoursID;
	
GO







-- =============================
-- Provider Payables, remove older unused sprocs
-- =============================
DROP PROCEDURE exports.ProviderPayByCaseByMonth;
GO

CREATE VIEW exports.ProviderPayables AS

	SELECT
		cah.HoursDate,
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		cah.HoursPayable,
		p.ProviderRate,
		pt.ID AS PatientID,
		pt.PatientFirstName,
		pt.PatientLastName,
		i.ID AS InsuranceID,
		i.InsuranceName,
		p.ProviderType AS ProviderTypeID,
		ptypes.ProviderTypeCode AS ProviderTypeCode,
		ptypes.ProviderTypeName AS ProviderTypeName
	FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS cah ON cah.CaseProviderID = p.ID
		INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
		INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
		LEFT JOIN dbo.Insurances AS i ON pt.PatientInsuranceID = i.ID
		LEFT JOIN dbo.ProviderTypes AS ptypes ON ptypes.ID = p.ProviderType
	WHERE cah.HoursStatus = 3 -- finalized only

GO






-- =============================
-- Add latest signoff dates for all providers
-- =============================

ALTER PROCEDURE [webreports].[PatientHoursReportDetail] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 

	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 481
	SET @StartDate = '2017-01-01'
	SET @EndDate = '2017-01-31'
	-- */


		
	SELECT 
		@StartDate AS StartDate,
		@EndDate AS EndDate,

		signoff.SignoffDate,
		COALESCE(authd.ID, supervisor.ID, bcba.ID) AS reportedBCBAID,
		COALESCE(authd.FirstName, supervisor.FirstName, bcba.FirstName) AS reportedBCBAFirstName,
		COALESCE(authd.Lastname, supervisor.LastName, bcba.LastName) AS reportedBCBALastName,

		pnt.PatientLastName,
		pnt.PatientFirstName,
		cah.HoursDate,
		CAST(cah.HoursTimeIn AS DATETIME) AS HoursTimeIn,
		CAST(cah.HoursTimeOut AS DATETIME) AS HoursTimeOut,
		cah.HoursTotal,

		p.ID AS ProviderID,
		p.ProviderLastName,
		p.ProviderFirstName,
		pt.ProviderTypeCode,
		allSignoffs.LatestSignoffDate,

		s.ServiceCode,
		cah.ID,
		CASE WHEN pt.ID = 15 THEN extendedNotes.NotesField
		ELSE cah.HoursNotes END AS HoursNotes
		
		
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pnt ON pnt.ID = c.PatientID
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN dbo.Services AS s ON s.ID = cah.HoursServiceID
	LEFT JOIN (

		SELECT
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaCP.ProviderID

		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaCP.IsSupervisor = -1
			--AND bcbaCAH.HoursDate >= @Startdate
			--AND bcbaCAH.HoursDate <= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)

	) AS supervisor ON supervisor.CaseID = cah.CaseID

	LEFT JOIN (

		SELECT TOP 1
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		-- INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaP.ID
		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaP.ProviderType = 15
			AND bcbaCP.IsInsuranceAuthorizedBCBA = -1
			--AND bcbaCAH.HoursDate >= @StartDate
			--AND bcbaCAH.HoursDate >= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)
			
	) AS authd ON authd.CaseID = cah.CaseID

	LEFT JOIN (

		SELECT TOP 1
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaP.ID
		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaP.ProviderType = 15
			--AND bcbaCAH.HoursDate >= @StartDate
			--AND bcbaCAH.HoursDate >= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)

	) AS bcba ON bcba.CaseID = cah.CaseID


	LEFT JOIN (

		SELECT TOP 1
			signoffCMP.CaseID,
			signoffCMPPF.DateCreated AS SignoffDate

		FROM dbo.CaseMonthlyPeriods AS signoffCMP
		INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS signoffCMPPF ON signoffCMP.ID = signoffCMPPF.CaseMonthlyPeriodID

		WHERE signoffCMP.CaseID = @CaseID
			AND signoffCMP.PeriodFirstDayOfMonth >= @StartDate
			AND signoffCMP.PeriodFirstDayOfMonth <= @EndDate

	) AS signoff ON signoff.CaseID = cah.CaseID

	LEFT JOIN (

		SELECT MAX(signoffCMPPF.DateCreated) AS LatestSignoffDate,
			signoffCMPPF.ProviderID

		FROM dbo.CaseMonthlyPeriods AS signoffCMP
		INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS signoffCMPPF ON signoffCMP.ID = signoffCMPPF.CaseMonthlyPeriodID

		WHERE signoffCMP.CaseID = @CaseID
			AND signoffCMP.PeriodFirstDayOfMonth >= @StartDate
			AND signoffCMP.PeriodFirstDayOfMonth <= @EndDate

		GROUP BY signoffCMPPF.ProviderID

	) AS allSignoffs ON p.ID = allSignoffs.ProviderID

	--Left Join CaseAuthHoursNotes as extendedNotes on extendedNotes.HoursID = cah.ID

	LEFT JOIN(
		
		SELECT
			HoursID,
			STUFF(
				(SELECT  Char(10) + TemplateText + ':' + Char(10) + NotesAnswer + Char(10)
				From CaseAuthHoursNotes
				Inner Join HoursNoteTemplates
					on HoursNoteTemplates.ID = CaseAuthHoursNotes.NotesTemplateID
				Where HoursID = c.HoursID And NotesAnswer Is Not NULL
				FOR XML PATH (''))
				, 1, 1, '') AS NotesField
		FROM CaseAuthHoursNotes As c
		Group By HoursID


	)As extendedNotes On extendedNotes.HoursID = cah.ID
	
	WHERE cah.CaseID = @CaseID
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		
	ORDER BY cah.HoursDate, cah.HoursTimeIn
	
	RETURN
GO



-- get just a list of signoffs
 CREATE PROCEDURE webreports.PatientHoursReportDetailSignatures (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 
 	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 481
	SET @StartDate = '2017-01-01'
	SET @EndDate = '2017-01-31'
	-- */

	DECLARE @tablevar TABLE(
		StartDate DATETIME2, EndDate DATETIME2, SignoffDate DATETIME2, reportedBCBAID INT, reportedBCBAFirstName NVARCHAR(100), reportedBCBALastName NVARCHAR(100),
		PatientLastName NVARCHAR(100), PatientFirstName NVARCHAR(100), HoursDate DATETIME2, HoursTimeIn DATETIME2, HoursTimeOut DATETIME2, HoursTotal DECIMAL(10,2),
		ProviderID INT, ProviderLastName NVARCHAR(100), ProviderFirstName NVARCHAR(100), ProviderTypeCode NVARCHAR(100), LatestSignoffDate DATETIME2, ServiceCode NVARCHAR(100), ID INT, HoursNotes NVARCHAR(1000));
	INSERT INTO @tablevar (
		StartDate, EndDate, SignoffDate, reportedBCBAID, reportedBCBAFirstName, reportedBCBALastName,
		PatientLastName, PatientFirstName, HoursDate, HoursTimeIn, HoursTimeOut, HoursTotal,
		ProviderID, ProviderLastName, ProviderFirstName, ProviderTypeCode, LatestSignoffDate, ServiceCode, ID, HoursNotes
		) EXEC webreports.PatientHoursReportDetail @CaseID, @StartDate, @EndDate;

	SELECT 
		reportedBCBAID AS ProviderID,
		reportedBCBAFirstName AS ProviderFirstName,
		reportedBCBALastName AS ProviderLastName,
		SignoffDate		
	FROM @tablevar
	GROUP BY 
		reportedBCBAID,
		reportedBCBAFirstName,
		reportedBCBALastName,
		SignoffDate		

	UNION

	SELECT
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate
	FROM @tablevar
	GROUP BY
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate

	RETURN

GO









GO
EXEC meta.UpdateVersion '1.9.2.0';
GO

