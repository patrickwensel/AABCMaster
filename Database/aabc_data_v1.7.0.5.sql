/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.0.0
dym:TargetEndingVersion: 1.7.0.5
---------------------------------------------------------------------

	Add parent hours report logging
	
---------------------------------------------------------------------*/


-- ===========================
-- Create a log table for patient reported hours
-- ===========================

CREATE TABLE dbo.CaseAuthHoursReportLog (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	LogLoginID INT NOT NULL,
	LogHoursID INT NOT NULL,
	LogMessage NVARCHAR(MAX)
);