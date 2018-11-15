/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.6.10.0
dym:TargetEndingVersion: 1.7.0.0
---------------------------------------------------------------------

	
---------------------------------------------------------------------*/



ALTER TABLE CaseAuthHours 
	ADD ParentReported BIT NOT NULL DEFAULT 0;
	
GO