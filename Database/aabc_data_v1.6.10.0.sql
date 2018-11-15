/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.6.9.1
dym:TargetEndingVersion: 1.6.10.0
---------------------------------------------------------------------

	Various minor updates for management portal
	Email Referrals
	
---------------------------------------------------------------------*/







CREATE TABLE ReferralEmails (
	MessageID varchar(250) Primary Key Not NULL,
	MessageSubject nvarchar(250),
	MessageStatus varchar(50),
	ProcessedTime datetime2(7) 
);

ALTER TABLE Referrals ALTER COLUMN ReferralState NVARCHAR(50);
GO







/*
Update hours watch sources to disregard discharged cases
*/



-- ============================
-- Cases with Hours but no Direct Supervision
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoSupervision](@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
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

	WHERE c.CaseStatus >= 0

	RETURN


GO



-- ============================
-- Cases with Hours but no BCBA Billing
-- ============================
  ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoBCBAHours](@StartDate DATE, @EndDate DATE) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-08-01'
	SET @EndDate = '2016-08-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
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

	WHERE c.CaseStatus >= 0

	RETURN


GO







-- ============================
-- Cases with Hours but no AIDE Billing
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoAideHours](@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
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

	WHERE c.CaseStatus >= 0

	RETURN


GO





-- ============================
-- Cases with Auths but no Hours
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithAuthsButNotHours](@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
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








