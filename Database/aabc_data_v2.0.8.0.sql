/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.7.0
dym:TargetEndingVersion: 2.0.8.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/




CREATE PROCEDURE ahr.CataylstPreloadProviderMapAnalysis AS 
	
	IF OBJECT_ID('tempdb..#providers') IS NOT NULL DROP TABLE #providers
	CREATE TABLE #providers (ID INT NULL, ProviderName NVARCHAR(128) NOT NULL);

	-- get distinct providers
	INSERT INTO #providers (ProviderName) SELECT DISTINCT ProviderName FROM dbo.CatalystPreloadEntries;

	
	IF OBJECT_ID('tempdb..#matched') IS NOT NULL DROP TABLE #matched
	CREATE TABLE #matched (SourceName NVARCHAR(128), ProviderID INT)

	INSERT INTO #matched (SourceName, ProviderID)
	SELECT r.SourceName, r.ProviderID
	FROM (
		-- split into first/last name and join to providers table
		SELECT
			s.SourceName,
			s.SourceFirstName,
			s.SourceLastName,
			t.ID AS ProviderID,
			t.ProviderFirstName,
			t.ProviderLastName
		FROM (
			SELECT 
				ProviderName AS SourceName, 
				CHARINDEX(',', ProviderName) AS SepIndex,
				CASE WHEN CHARINDEX(',', ProviderName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(ProviderName, 0, CHARINDEX(',', ProviderName))))
					ELSE NULL
				END AS SourceLastName,
				CASE WHEN CHARINDEX(',', ProviderName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(ProviderName, CHARINDEX(',', ProviderName) + 1, LEN(ProviderName) - CHARINDEX(',', ProviderName))))
					ELSE ProviderName
				END AS SourceFirstName
			FROM #providers
		) AS s
		LEFT JOIN dbo.Providers AS t ON s.SourceFirstName = t.ProviderFirstName AND s.SourceLastName = t.ProviderLastName
		WHERE t.ID IS NOT NULL
	) AS r


	IF OBJECT_ID('tempdb..#unmatched') IS NOT NULL DROP TABLE #unmatched
	CREATE TABLE #unmatched (SourceName NVARCHAR(128), ProviderID INT)

	INSERT INTO #unmatched (SourceName)
	SELECT r.SourceName
	FROM (
		-- split into first/last name and join to providers table
		SELECT
			s.SourceName,
			s.SourceFirstName,
			s.SourceLastName,
			t.ID AS ProviderID,
			t.ProviderFirstName,
			t.ProviderLastName
		FROM (
			SELECT 
				ProviderName AS SourceName, 
				CHARINDEX(',', ProviderName) AS SepIndex,
				CASE WHEN CHARINDEX(',', ProviderName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(ProviderName, 0, CHARINDEX(',', ProviderName))))
					ELSE NULL
				END AS SourceLastName,
				CASE WHEN CHARINDEX(',', ProviderName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(ProviderName, CHARINDEX(',', ProviderName) + 1, LEN(ProviderName) - CHARINDEX(',', ProviderName))))
					ELSE ProviderName
				END AS SourceFirstName
			FROM #providers
		) AS s
		LEFT JOIN dbo.Providers AS t ON s.SourceFirstName = t.ProviderFirstName AND s.SourceLastName = t.ProviderLastName
		WHERE t.ID IS NULL
	) AS r


	-- later we should left join so only inserting if not previously existing
	INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID)
		SELECT SourceName, ProviderID FROM #matched;

	SELECT * FROM #unmatched;
	SELECT * FROM dbo.CatalystProviderMappings;

GO





CREATE PROCEDURE ahr.CataylstPreloadPatientMapAnalysis AS 

	IF OBJECT_ID('tempdb..#patients') IS NOT NULL DROP TABLE #patients
	CREATE TABLE #patients (PatientName NVARCHAR(128))

	-- get distinct patients
	INSERT INTO #patients (PatientName) SELECT DISTINCT PatientName FROM dbo.CatalystPreloadEntries;

	IF OBJECT_ID('tempdb..#matched') IS NOT NULL DROP TABLE #matched
	CREATE TABLE #matched (SourceName NVARCHAR(128), PatientID INT)

	INSERT INTO #matched (SourceName, PatientID)
	SELECT r.SourceName, r.PatientID
	FROM (
		SELECT
			s.SourceName,
			s.SourceFirstName,
			s.SourceLastName,
			t.ID AS PatientID,
			t.PatientFirstName,
			t.PatientLastName
		FROM (
			SELECT 
				PatientName AS SourceName, 
				CHARINDEX(' ', PatientName) AS SepIndex,
				CASE WHEN CHARINDEX(' ', PatientName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(PatientName, 0, CHARINDEX(' ', PatientName))))
					ELSE NULL
				END AS SourceFirstName,
				CASE WHEN CHARINDEX(' ', PatientName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(PatientName, CHARINDEX(' ', PatientName) + 1, LEN(PatientName) - CHARINDEX(' ', PatientName))))
					ELSE PatientName
				END AS SourceLastName
			FROM #patients
		) AS s
		LEFT JOIN dbo.Patients AS t ON s.SourceFirstName = t.PatientFirstName AND s.SourceLastName = t.PatientLastName
		WHERE t.ID IS NOT NULL
	) AS r


	IF OBJECT_ID('tempdb..#unmatched') IS NOT NULL DROP TABLE #unmatched
	CREATE TABLE #unmatched (SourceName NVARCHAR(128), PatientID INT)

	INSERT INTO #unmatched (SourceName, PatientID)
	SELECT r.SourceName, r.PatientID
	FROM (
		SELECT
			s.SourceName,
			s.SourceFirstName,
			s.SourceLastName,
			t.ID AS PatientID,
			t.PatientFirstName,
			t.PatientLastName
		FROM (
			SELECT 
				PatientName AS SourceName, 
				CHARINDEX(' ', PatientName) AS SepIndex,
				CASE WHEN CHARINDEX(' ', PatientName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(PatientName, 0, CHARINDEX(' ', PatientName))))
					ELSE NULL
				END AS SourceFirstName,
				CASE WHEN CHARINDEX(' ', PatientName) > 0
					THEN LTRIM(RTRIM(SUBSTRING(PatientName, CHARINDEX(' ', PatientName) + 1, LEN(PatientName) - CHARINDEX(' ', PatientName))))
					ELSE PatientName
				END AS SourceLastName
			FROM #patients
		) AS s
		LEFT JOIN dbo.Patients AS t ON s.SourceFirstName = t.PatientFirstName AND s.SourceLastName = t.PatientLastName
		WHERE t.ID IS NULL
	) AS r

	-- later we should left join so only inserting if not previously existing
	INSERT INTO dbo.CatalystPatientMappings (CatalystPatientName, PatientID)
	SELECT SourceName, PatientID FROM #matched

	SELECT * FROM #unmatched;
	SELECT * FROM dbo.CatalystPatientMappings;

GO

/*
INSERT INTO dbo.CatalystPatientMappings (CatalystPatientName, PatientID) VALUES ('Beluchi Udakwu', 1955);
INSERT INTO dbo.CatalystPatientMappings (CatalystPatientName, PatientID) VALUES ('Fortune Debbah', 2041);
INSERT INTO dbo.CatalystPatientMappings (CatalystPatientName, PatientID) VALUES ('Henriquez Roberto', 1969);
GO
*/



ALTER TABLE dbo.CatalystPatientMappings ADD CaseID INT;
GO
UPDATE t
	SET t.CaseID = s.ID
FROM dbo.CatalystPatientMappings AS t
INNER JOIN dbo.Cases AS s ON t.PatientID = s.PatientID
GO


--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Okounev, Sheina', 2431);
--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Maggid, Abi', 257);
--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Majekodunmi-Karade, Oluyomi', 2767);
--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Ort, Temy', 867);
--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Levitine, Nina', 2567);
--INSERT INTO dbo.CatalystProviderMappings (CatalystProviderName, ProviderID) VALUES ('Klein, Issac/ Yossi', 2499);
GO



-- Update hours entry diff to work off date created instead of hours date

-- Get a list of difference between entries per provider/case
ALTER PROCEDURE [ahr].[HoursEntryDiff](@StartDate DATETIME2, @EndDate DATETIME, @MaxSeconds INT) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2 = '2017-12-01'
	DECLARE @EndDate DATETIME = '2017-12-31'
	DECLARE @MaxSeconds INT = 2
	-- */

	IF OBJECT_ID('tempdb..#hours') IS NOT NULL DROP TABLE #hours
	CREATE TABLE #hours (ID INT, DateCreated DATETIME2, ProviderID INT, CaseID INT);

	INSERT INTO #hours
		SELECT ID, DateCreated, CaseProviderID, CaseID
		FROM dbo.CaseAuthHours AS cah 
		WHERE cah.DateCreated >= @StartDate 
			AND cah.DateCreated < @EndDate;
		
	SELECT 
		h1.ID, 
		h1.ProviderID, 
		h1.CaseID,
		h1.DateCreated, 
		ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated)) AS Interval
	FROM #hours AS h1
	CROSS JOIN #hours AS h2
	WHERE h1.ProviderID = h2.ProviderID
		AND h1.CaseID = h2.CaseID
		AND h1.ID <> h2.ID
		AND ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated)) < @MaxSeconds
	GROUP BY 
		h1.ID, 
		h1.ProviderID, 
		h1.CaseID,
		h1.DateCreated, 
		ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated))
	ORDER BY h1.ID

	RETURN
GO






-- ========================
-- add zip service areas
-- ========================
ALTER PROCEDURE [dbo].[GetProviderSearchViewData] AS BEGIN

	SELECT
		p.ID,
		p.DateCreated,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
		p.ProviderCity,
		p.ProviderState,
		p.ProviderZip,
		p.ProviderPrimaryEmail,
		p.ProviderPrimaryPhone,
		p.ProviderActive,
		COALESCE(caseCount.CaseCount, 0) AS ProviderActiveCaseCount,
		ProviderServiceAreas = STUFF(
			(
				SELECT ',' + pz.ZipCode
				FROM dbo.ProviderServiceZipCodes AS pz
				WHERE p.ID = pz.ProviderID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		)		
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN (
		SELECT 
			cp.ProviderID, 
			COUNT(*) AS CaseCount
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Cases AS c ON c.ID = cp.CaseID
		WHERE cp.Active = 1 AND c.CaseStatus >= 0
		GROUP BY cp.ProviderID
	) AS caseCount ON p.ID = caseCount.ProviderID
	
	WHERE p.ProviderActive = 1
	
	ORDER BY p.ProviderLastName;	
END
GO




-- Update Insurance MemberIDs from legacy to new
/*
SELECT 
	p.PatientInsuranceMemberID, 
	LegacyInsuranceID,
	ci.InsuranceID,
	ci.MemberID
-- */
 UPDATE ci SET ci.MemberID = p.PatientInsuranceMemberID	
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID
INNER JOIN dbo.CaseInsurances AS ci ON ci.InsuranceID = p.LegacyInsuranceID AND c.ID = ci.CaseID
WHERE ci.MemberID IS NULL
	AND p.PatientInsuranceMemberID IS NOT NULL
	
GO




	
	


	
	

GO
EXEC meta.UpdateVersion '2.0.8.0';
GO

