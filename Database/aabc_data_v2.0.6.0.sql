/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.5.0
dym:TargetEndingVersion: 2.0.6.0
---------------------------------------------------------------------

	- Hours Entry analytics
	- Legacy Insurance ID management
	- Docusign Finalization
	- BCBA Notes Display Order
	
---------------------------------------------------------------------*/


-- ========================
-- utility list of patient info
-- ========================
CREATE VIEW dbo.PatientSummary AS
	SELECT 
		p.ID AS PatientID,
		c.ID AS CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		c.CaseStatus AS StatusID,
		cs.Status AS Status,
		pci.InsuranceID AS ActiveInsuranceID,
		pcis.InsuranceName AS ActiveInsuranceName,
		pci.DatePlanEffective AS ActiveInsuranceEffective,
		pci.DatePlanTerminated AS ActiveInsuranceDefective,
		p.PatientInsuranceID AS LegacyInsuranceID,
		pti.InsuranceName AS LegacyInsuranceName
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN dbo.CaseStatuses AS cs ON cs.ID = c.CaseStatus
	LEFT JOIN dbo.Insurances AS pti ON pti.ID = p.PatientInsuranceID
	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON pci.CaseID = c.ID
	LEFT JOIN dbo.Insurances AS pcis ON pcis.ID = pci.InsuranceID;
	
GO




-- ========================
-- handle legacy insurance info
-- ========================
-- Create a legacy insurance ID field for patients
ALTER TABLE dbo.Patients ADD LegacyInsuranceID INT NULL;
GO

-- Move deprecated patient insurance IDs to legacy insurance IDs for archive
UPDATE dbo.Patients SET LegacyInsuranceID = PatientInsuranceID;
GO

-- Remove all deprecated patient insurance ID values
UPDATE dbo.Patients SET PatientInsuranceID = NULL;
GO





-- ========================
-- Get a list of difference between entries per provider/case
-- ========================
CREATE PROCEDURE ahr.HoursEntryDiff(@StartDate DATETIME2, @EndDate DATETIME, @MaxSeconds INT) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2 = '2017-01-01'
	DECLARE @EndDate DATETIME = '2017-02-01'
	DECLARE @MaxSeconds INT = 2
	-- */

	IF OBJECT_ID('tempdb..#hours') IS NOT NULL DROP TABLE #hours
	CREATE TABLE #hours (ID INT, DateCreated DATETIME2, ProviderID INT, CaseID INT);

	INSERT INTO #hours
		SELECT ID, DateCreated, CaseProviderID, CaseID
		FROM dbo.CaseAuthHours AS cah 
		WHERE cah.HoursDate >= @StartDate 
			AND cah.HoursDate < @EndDate;
		
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
-- tracking for docusign sigs on finalization
-- ========================
ALTER TABLE [dbo].[CaseMonthlyPeriodProviderFinalizations] ALTER COLUMN DateFinalized DATETIME2(7) NULL
GO

ALTER TABLE [dbo].[CaseMonthlyPeriodProviderFinalizations] ADD 
	IsComplete INT NOT NULL DEFAULT 1,
	EnvelopeID VARCHAR(50);
GO





-- ========================
-- add Sequence for Hours Note Templates
-- ========================
ALTER TABLE dbo.HoursNoteTemplates ADD TemplateDisplaySequence INT NOT NULL DEFAULT 0;
GO

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateDisplaySequence) VALUES (125, 10, 15, 'Indirect observations', 1);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateDisplaySequence) VALUES (126, 10, 15, 'Preference assessment', 2);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateDisplaySequence) VALUES (127, 10, 15, 'Records reviewed', 3);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateDisplaySequence) VALUES (128, 10, 15, 'Behaviors observed', 4);
GO

UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 5 WHERE ID = 120;

UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 1 WHERE ID = 102;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 2 WHERE ID = 103;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 3 WHERE ID = 104;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 4 WHERE ID = 105;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 5 WHERE ID = 121;

UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 1 WHERE ID = 106;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 2 WHERE ID = 107;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 3 WHERE ID = 108;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 4 WHERE ID = 109;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 5 WHERE ID = 122;

UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 1 WHERE ID = 111;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 2 WHERE ID = 112;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 3 WHERE ID = 113;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 4 WHERE ID = 114;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 5 WHERE ID = 123;

UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 1 WHERE ID = 116;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 2 WHERE ID = 117;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 3 WHERE ID = 118;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 4 WHERE ID = 119;
UPDATE dbo.HoursNoteTemplates SET TemplateDisplaySequence = 5 WHERE ID = 124;
GO




GO
EXEC meta.UpdateVersion '2.0.6.0';
GO

