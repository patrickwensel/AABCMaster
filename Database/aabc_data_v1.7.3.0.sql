/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.1.0
dym:TargetEndingVersion: 1.7.3.0
---------------------------------------------------------------------

	CaseStatusNotes size increase
	Add resolution fields to CaseAuthHoursReportLog
	
---------------------------------------------------------------------*/




-- ============================
-- Increase size of CaseStatusNotes
-- ============================
ALTER TABLE dbo.Cases ALTER COLUMN CaseStatusNotes NVARCHAR(2000) NULL;
GO


-- ============================
-- Add resolution fields to HoursReportLog
-- ============================
ALTER TABLE dbo.CaseAuthHoursReportLog
	ADD LogResolutionNote NVARCHAR(MAX),
		IsResolved BIT NOT NULL DEFAULT 0;

GO