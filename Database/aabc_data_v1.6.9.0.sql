/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.6.8.0
dym:TargetEndingVersion: 1.6.9.1
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/   

   
   
-- ============================
-- Give some info about patient counts per insurance
-- ============================
CREATE VIEW dbo.PatientCountByInsurance AS (
	SELECT i.ID, i.InsuranceName, p.PatientInsuranceID, COUNT(p.PatientInsuranceID) AS PatientsApplied
	FROM dbo.Insurances AS i
	LEFT JOIN dbo.Patients AS p ON i.ID = p.PatientInsuranceID
	-- WHERE p.PatientInsuranceID IS NULL
	GROUP BY i.ID, i.InsuranceName, p.PatientInsuranceID
);

   
   
   
GO
-- ============================
-- Insurances that don't have match rules applied to them
-- ============================
CREATE VIEW dbo.InsurancesWithoutAuthMatchRules AS

	SELECT 
		i.ID AS InsuranceID, 
		i.InsuranceName, 
		m.ID AS MatchRuleID

	FROM dbo.Insurances AS i
	LEFT JOIN dbo.AuthMatchRules AS m ON m.InsuranceID = i.ID

	WHERE m.ID IS NULL;
   
   
   
   
 GO
-- ============================
-- Insurances that don't have match rules applied to them
-- but are currently applied to patients
-- ============================
CREATE VIEW dbo.InsurancesAppliedWithoutAuthMatchRules AS

	SELECT 
		i.ID AS InsuranceID, 
		i.InsuranceName 
		-- m.ID AS MatchRuleID,
		-- pc.PatientsApplied

	FROM dbo.Insurances AS i
	INNER JOIN dbo.PatientCountByInsurance AS pc ON pc.ID = i.ID
	LEFT JOIN dbo.AuthMatchRules AS m ON m.InsuranceID = i.ID
	

	WHERE m.ID IS NULL
		AND pc.PatientsApplied = 0;
   
   
   
   
   
   
   
   
   
   
   
   
   
GO

-- ============================
-- Add some new fields for tracking
-- ============================
ALTER TABLE dbo.CaseMonthlyPeriods ADD WatchComment NVARCHAR(500);
ALTER TABLE dbo.CaseMonthlyPeriods ADD WatchIgnore BIT NOT NULL DEFAULT 0;

GO






-- ============================
-- Cases with Auths but no Hours
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

	RETURN


GO





-- ============================
-- Cases with Hours but no AIDE Billing
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoAideHours](@StartDate DATETIME2, @EndDate DATETIME2) AS

	/*  --TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
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

		-- caseIDs with no Ahide Hours on file for the given date range
		SELECT
			cah.CaseID
		FROM dbo.CaseAuthHours AS cah
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.CaseID NOT IN (	
				-- get list of caseIDs in the date range which have Aide Hours applied
				SELECT 
					h.CaseID
				FROM dbo.CaseAuthHours AS h
				INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
				INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
				WHERE pt.ProviderTypeCode = 'AIDE'
					AND h.HoursDate >= @StartDate
					AND h.HoursDate <= @EndDate
				GROUP BY h.CaseID
				HAVING COUNT(h.CaseProviderID) > 0
			)
		GROUP BY cah.CaseID

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN


GO



-- ============================
-- Cases with Hours but no BCBA Billing
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoBCBAHours](@StartDate DATE, @EndDate DATE) AS

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
			cah.CaseID,
			bcbaHours.CaseID AS bcbaCaseID
		FROM dbo.CaseAuthHours AS cah
		LEFT JOIN (
			SELECT 
				h.CaseID
			FROM dbo.CaseAuthHours AS h
			INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
			WHERE p.ProviderType = 15
				AND h.HoursDate >= @StartDate
				AND h.HoursDate <= @EndDate
			GROUP BY h.CaseID
		) AS bcbaHours ON bcbaHours.CaseID = cah.CaseID
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND bcbaHours.CaseID IS NULL
		GROUP BY cah.CaseID,
			bcbaHours.CaseID 

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN


GO



-- ============================
-- Cases with Hours but no Direct Supervision
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoSupervision](@StartDate DATETIME2, @EndDate DATETIME2) AS

	/* -- TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
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
			cah.CaseID
		FROM dbo.CaseAuthHours AS cah
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.CaseID NOT IN (	
				-- get list of caseIDs in the date range which have DSU Hours applied
				SELECT 
					h.CaseID
				FROM dbo.CaseAuthHours AS h
				INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
				INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
				INNER JOIN dbo.Services AS s ON s.ID = h.HoursServiceID
				WHERE pt.ProviderTypeCode = 'BCBA'
					AND h.HoursDate >= @StartDate
					AND h.HoursDate <= @EndDate
					AND s.ServiceCode = 'DSU'
				GROUP BY h.CaseID
				HAVING COUNT(h.CaseProviderID) > 0
			)
		GROUP BY cah.CaseID

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN


GO



   
		
-- //// END MIGRATION SCRIPT v1.6.8.0-vX.X.X.X
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	

	