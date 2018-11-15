/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.8.0
dym:TargetEndingVersion: 2.0.9.0
---------------------------------------------------------------------

	Catalyst hardening
	Parent Portal AHRs
	
---------------------------------------------------------------------*/



	
-- ======================
-- Add some catalyst specific tables for timesheet and hasData tracking
-- ======================
-- create a schema for catalyst stuff
CREATE SCHEMA cata;
GO

CREATE TABLE cata.CaseMappings (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),	-- required for EF mapping
	TimesheetPatientName NVARCHAR(500),
	CaseID INT
);
GO

CREATE TABLE cata.ProviderMappings (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),	-- required for EF mapping
	TimesheetProviderName NVARCHAR(500),
	ProviderID INT
)
GO

CREATE TABLE cata.TimesheetPreload (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	ResponseDate DATETIME2 NOT NULL,
	[Date] DATETIME2,
	PatientName NVARCHAR(128) NOT NULL,	
	ProviderName NVARCHAR(129) NOT NULL,
	Notes NVARCHAR(MAX) NOT NULL,
	ProviderAgreed BIT NOT NULL DEFAULT 0,
	PatientAgreed BIT NOT NULL DEFAULT 0,
	MappedProviderID INT,
	MappedCaseID INT,
	IsResolved BIT NOT NULL DEFAULT 0
);
CREATE INDEX idxCataTimesheetPreloadCaseID ON cata.TimesheetPreload (MappedCaseID);
CREATE INDEX idxCataTimesheetPreloadProviderID ON cata.TimesheetPreload (MappedProviderID);
CREATE INDEX idxCataTimesheetPreloadIsResolved ON cata.TimesheetPreload (IsResolved);
GO


-- port all preload entries into new table
DELETE FROM cata.TimesheetPreload;
INSERT INTO cata.TimesheetPreload (
	ResponseDate, [Date], PatientName, ProviderName, Notes,
	ProviderAgreed, PatientAgreed, MappedProviderID, MappedCaseID, IsResolved
) SELECT
	ResponseDate, [Date], PatientName, ProviderName, Notes,
	ProviderAgreed, PatientAgreed, MappedProviderID, MappedCaseID, IsResolved
FROM dbo.CatalystPreloadEntries;
GO
-- drop the preload table
DROP TABLE dbo.CatalystPreloadEntries;
GO

-- port the case and provider mappings
DELETE FROM cata.CaseMappings;
INSERT INTO cata.CaseMappings (TimesheetPatientName, CaseID)
	SELECT CatalystPatientName, CaseID FROM dbo.CatalystPatientMappings WHERE CaseID IS NOT NULL;
GO
DROP TABLE dbo.CatalystPatientMappings;
GO

DELETE FROM cata.ProviderMappings;
INSERT INTO cata.ProviderMappings (TimesheetProviderName, ProviderID)
	SELECT CatalystProviderName, ProviderID FROM dbo.CatalystProviderMappings WHERE ProviderID IS NOT NULL;
GO
DROP TABLE dbo.CatalystProviderMappings;
GO




CREATE PROCEDURE cata.MapTimesheetCases AS 

	-- make sure the mappings list is up to date with the most recent entries
	INSERT INTO cata.CaseMappings (TimesheetPatientName)
		SELECT DISTINCT p.PatientName
		FROM cata.TimesheetPreload AS p
		LEFT JOIN cata.CaseMappings AS cm ON cm.TimesheetPatientName = p.PatientName
		WHERE cm.ID IS NULL;
	
	-- update unmapped based on patient name
	-- SELECT * 
	UPDATE cm SET cm.CaseID = c.ID
	FROM cata.CaseMappings AS cm
	LEFT JOIN dbo.Patients AS p ON p.PatientFirstName + ' ' + p.PatientLastName = cm.TimesheetPatientName
	LEFT JOIN dbo.Cases AS c ON c.PatientID = p.ID
	WHERE cm.CaseID IS NULL
		AND p.ID IS NOT NULL;
		
	-- now that the mappings are good, update the timesheet table accordingly
	UPDATE ts SET ts.MappedCaseID = cm.CaseID
	FROM cata.TimesheetPreload AS ts
	LEFT JOIN cata.CaseMappings AS cm ON cm.TimesheetPatientName = ts.PatientName
	WHERE ts.MappedCaseID IS NULL
		AND cm.ID IS NOT NULL
		AND cm.CaseID IS NOT NULL;
	
GO



CREATE PROCEDURE cata.MapTimesheetProviders AS 

	-- map sure the mapping list is up to date
	INSERT INTO cata.ProviderMappings (TimesheetProviderName)
		SELECT DISTINCT p.ProviderName
		FROM cata.TimesheetPreload AS p
		LEFT JOIN cata.ProviderMappings AS pm ON pm.TimesheetProviderName = p.ProviderName
		WHERE pm.ID IS NULL;

	-- update unmapped base on provider name
	--SELECT * 
	UPDATE pm SET pm.ProviderID = p.ID
	FROM cata.ProviderMappings AS pm
	LEFT JOIN dbo.Providers AS p ON pm.TimesheetProviderName = p.ProviderLastName + ', ' + p.ProviderFirstName
	WHERE pm.ProviderID IS NULL
		AND p.ID IS NOT NULL;

	-- now that the mappings are good, update the timesheet table accordingly
	UPDATE ts SET ts.MappedProviderID = pm.ProviderID
	FROM cata.TimesheetPreload AS ts
	LEFT JOIN cata.ProviderMappings AS pm ON ts.ProviderName = pm.TimesheetProviderName
	WHERE ts.MappedProviderID IS NULL
		AND pm.ID IS NOT NULL
		AND pm.ProviderID IS NOT NULL;
		
GO








-- ======================
-- HasData tracking
-- ======================
CREATE TABLE cata.HasDataImportTemp (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	ImportName NVARCHAR(500),
	ImportDate DATETIME2,
	ImportInitials NVARCHAR(50)
);

CREATE TABLE cata.HasDataFlat (
	FirstName NVARCHAR(500),
	LastName NVARCHAR(500),
	CaseID INT,
	ImportDate DATETIME2,
	ProviderInitials NVARCHAR(50),
	ProviderID INT
);
GO


CREATE PROCEDURE cata.GenerateHasDataResults AS

	-- =========================
	-- Create temps for:
	--		Results (any errors, success logs) 
	--		Matches (or base working set)
	-- =========================
	IF OBJECT_ID('tempdb..#catalystHasDataResults') IS NOT NULL DROP TABLE #catalystHasDataResults
	CREATE TABLE #catalystHasDataResults (
		Result NVARCHAR(50),
		CaseID INT, 
		ProviderID INT,
		VisitDate DATETIME2,
		ProviderInitials NVARCHAR(255),
		StudentName NVARCHAR(255)
	)

	IF OBJECT_ID('tempdb..#catalystHasDataMatches') IS NOT NULL DROP TABLE #catalystHasDataMatches
	CREATE TABLE #catalystHasDataMatches (
		CaseID INT,
		ProviderID INT,
		VisitDate DATETIME2		
	)


	-- =========================
	-- Parse out the import data into a flat table
	-- =========================
	DELETE FROM cata.HasDataFlat;
	INSERT INTO cata.HasDataFlat (FirstName, LastName, CaseID, ImportDate, ProviderInitials, ProviderID)
	SELECT
		RTRIM(LTRIM(SUBSTRING(i.ImportName, 1, CHARINDEX(' ', i.ImportName)))),
		RTRIM(LTRIM(SUBSTRING(i.ImportName, CHARINDEX(' ', i.ImportName) + 1, LEN(i.ImportName) - CHARINDEX(' ', i.ImportName)))),
		NULL,
		i.ImportDate,
		RTRIM(LTRIM(raw.RawInitials)) AS Initials,
		NULL
	FROM (
		SELECT ID,
			Split.a.value('.', 'VARCHAR(100)') AS RawInitials
		FROM (
			SELECT ID, CAST('<M>' + REPLACE(ImportInitials, ',', '</M><M>') + '</M>' AS XML) AS Data
			FROM cata.HasDataImportTemp
		) AS a CROSS APPLY Data.nodes ('/M') AS Split(a)
	) AS raw
	INNER JOIN cata.HasDataImportTemp AS i ON i.ID = raw.ID


	
	-- =========================
	-- Resolve CaseIDs, log and remove failures
	-- =========================
	UPDATE t SET t.CaseID = s.ID
	FROM cata.HasDataFlat AS t
	INNER JOIN dbo.Patients AS p ON p.PatientFirstName = t.FirstName AND p.PatientLastName = t.LastName
	INNER JOIN dbo.Cases AS s ON s.PatientID = p.ID

	-- log the failures
	INSERT INTO #catalystHasDataResults (Result, StudentName)
	SELECT 'Patient Not Found', r.PatientName FROM (
		SELECT f.FirstName + ' ' + f.LastName AS PatientName
		FROM cata.HasDataFlat AS f
		WHERE f.CaseID IS NULL
	) AS r
	GROUP BY r.PatientName
	
	-- remove the failures
	DELETE FROM cata.HasDataFlat WHERE CaseID IS NULL
	


	-- =========================
	-- Log multiple providers per initials issues
	-- =========================
	INSERT INTO #catalystHasDataResults (Result, CaseID, ProviderInitials, VisitDate, StudentName)
	SELECT 'Multiple Providers', nep.CaseID, nep.SysInitials, nep.ImportDate, nep.StudentName
	FROM (
		-- Get provider initials where there's more than one provider matched
		SELECT 
			d.CaseID,
			d.FirstName + ' ' + d.LastName AS StudentName,
			d.ImportDate,
			-- cp.ActiveStartDate, cp.ActiveEndDate,
			UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) AS SysInitials,
			COUNT (DISTINCT p.ID) AS CountOfProviders
		FROM cata.HasDataFlat AS d 
		LEFT JOIN dbo.CaseProviders AS cp ON cp.CaseID = d.CaseID
		LEFT JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE COALESCE(cp.ActiveStartDate, '2000-01-01') <= d.ImportDate 
			AND COALESCE(cp.ActiveEndDate, '2100-01-01') >= d.ImportDate
			AND UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) = UPPER(d.ProviderInitials)
		GROUP BY
			d.CaseID,
			d.ImportDate,
			d.FirstName + ' ' + d.LastName,
			UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1))
		HAVING COUNT(DISTINCT p.ID) > 1
	) AS nep -- non-eligible providers

		
	-- =========================
	-- Log provider not found issues
	-- =========================	
	INSERT INTO #catalystHasDataResults (Result, CaseID, ProviderInitials, VisitDate, StudentName)
	SELECT 'Provider Not Found', nop.caseID, nop.ProviderInitials, nop.ImportDate, nop.StudentName FROM (
		SELECT 
			d.CaseID,
			d.ImportDate,
			d.ProviderInitials,
			d.FirstName + ' ' + d.LastName AS StudentName,
			COUNT (DISTINCT p.ID) AS CountOfProviders
		FROM cata.HasDataFlat AS d 
		LEFT JOIN dbo.CaseProviders AS cp ON cp.CaseID = d.CaseID
		LEFT JOIN dbo.Providers AS p ON p.ID = cp.ProviderID AND UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) = UPPER(d.ProviderInitials)
		WHERE COALESCE(cp.ActiveStartDate, '2000-01-01') <= d.ImportDate 
			AND COALESCE(cp.ActiveEndDate, '2100-01-01') >= d.ImportDate
		GROUP BY
			d.CaseID,
			d.ImportDate,
			d.FirstName + ' ' + d.LastName,
			d.ProviderInitials
		HAVING COUNT(DISTINCT p.ID) = 0
	) AS nop
	GROUP BY nop.caseID, nop.ImportDate, nop.ProviderInitials, nop.StudentName


	-- =========================
	-- Work up the case, provider and date for matched entries
	-- =========================
	INSERT INTO #catalystHasDataMatches (CaseID, ProviderID, VisitDate)
	SELECT 
		ep.CaseID, 
		pd.ProviderID, 
		ep.ImportDate
	FROM (
		-- Get provider initials where there's only one provider matched
		SELECT 
			d.CaseID,
			d.ImportDate,
			UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) AS SysInitials,
			COUNT (DISTINCT p.ID) AS CountOfProviders
		FROM cata.HasDataFlat AS d 
		LEFT JOIN dbo.CaseProviders AS cp ON cp.CaseID = d.CaseID
		LEFT JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE COALESCE(cp.ActiveStartDate, '2000-01-01') <= d.ImportDate 
			AND COALESCE(cp.ActiveEndDate, '2100-01-01') >= d.ImportDate
			AND UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) = UPPER(d.ProviderInitials)
		GROUP BY
			d.CaseID,
			d.ImportDate,
			UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1))
		HAVING COUNT(DISTINCT p.ID) = 1
	) AS ep -- eligible providers

	-- join eligible providers with provider details
	INNER JOIN (
		-- get the provider details and return those matched on case and initials
		SELECT 
			cp.CaseID, 
			p.ID AS ProviderID,
			p.ProviderFirstName, 
			p.ProviderLastName, 
			cp.ActiveStartDate, 
			cp.ActiveEndDate,
			UPPER(SUBSTRING(p.ProviderFirstName, 1, 1) + SUBSTRING(p.ProviderLastName, 1, 1)) AS Initials
		FROM dbo.Providers AS p
		INNER JOIN dbo.CaseProviders AS cp ON p.ID = cp.ProviderID

	)AS	pd -- provider details
		ON ep.CaseID = pd.CaseID AND ep.SysInitials = pd.Initials



	-- =========================
	-- Now we have a list of matches, update the hours entries accordingly
	-- =========================
	UPDATE h SET h.HoursHasCatalystData = 1 
	FROM #catalystHasDataMatches AS hd
	LEFT JOIN dbo.CaseAuthHours AS h 
		ON hd.CaseID = h.CaseID
			AND hd.ProviderID = h.CaseProviderID
			AND hd.VisitDate = h.HoursDate
	WHERE h.ID IS NOT NULL;


	-- =========================
	-- Log the sucesses
	-- =========================
	INSERT INTO #catalystHasDataResults (Result, CaseID, ProviderID, VisitDate, StudentName, ProviderInitials)
	SELECT 'Success', hd.CaseID, hd.ProviderID, hd.VisitDate, p.PatientFirstName + ' ' + p.PatientLastName, pv.ProviderFirstName + ' ' + pv.ProviderLastName
	FROM #catalystHasDataMatches AS hd
	LEFT JOIN dbo.CaseAuthHours AS h 
		ON hd.CaseID = h.CaseID
			AND hd.ProviderID = h.CaseProviderID
			AND hd.VisitDate = h.HoursDate
	LEFT JOIN dbo.Cases AS c ON c.ID = h.CaseID
	LEFT JOIN dbo.Patients AS p ON p.ID = c.PatientID
	LEFT JOIN dbo.Providers AS pv ON pv.ID = h.CaseProviderID
	WHERE h.ID IS NOT NULL;



	-- =========================
	-- Return the results
	-- =========================
	SELECT Result, CaseID, ProviderID, VisitDate, ProviderInitials, StudentName FROM #catalystHasDataResults;
	
		
GO

















-- ======================
-- Update the patient portal login/signature view to include ID
-- ======================

ALTER VIEW [ahr].[PatientPortalLoginActivity] AS
SELECT 
	l.ID AS LoginID,
	p.PatientFirstName,
	p.PatientLastName,
	l.LoginFirstName,
	l.LoginLastName,
	MAX(si.SignInDate) AS LatestSignIn,
	COUNT(*) AS NumberOfSessions
FROM dbo.PatientPortalSignIns AS si
INNER JOIN dbo.PatientPortalLogins AS l ON l.ID = si.UserId
INNER JOIN dbo.PatientPortalLoginPatients AS pp ON pp.LoginID = l.ID
INNER JOIN dbo.Patients AS p ON pp.PatientID = p.ID
GROUP BY 
	l.ID,
	p.PatientFirstName,
	p.PatientLastName,
	l.LoginFirstName,
	l.LoginLastName
GO



CREATE VIEW ahr.ParentLoginAndSignatureActivity AS
	SELECT 
		a.LoginID,
		a.PatientFirstName,
		a.PatientLastName,
		a.LoginFirstName,
		a.LoginLastName,
		a.LatestSignIn,
		a.NumberOfSessions,
		COALESCE(s.SignatureCount, 0) AS NumberOfSignatures,
		s.LastSignature
	FROM ahr.PatientPortalLoginActivity AS a
	LEFT JOIN (
		SELECT LoginID, COUNT(*) AS SignatureCount, MAX(SignatureDate) AS LastSignature FROM dbo.PatientPortalLoginSignatures GROUP BY LoginID
	) AS s ON a.LoginID = s.LoginID

GO


	

GO
EXEC meta.UpdateVersion '2.0.9.0';
GO

