/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.9.8.0
dym:TargetEndingVersion: 2.0.2.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


ALTER TABLE dbo.CaseAuthHours ADD HoursTrainingEntry BIT NOT NULL DEFAULT 0;
GO



GO
EXEC meta.UpdateVersion '2.0.2.0';
GO

