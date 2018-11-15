/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.0.5
dym:TargetEndingVersion: 1.7.1.0
---------------------------------------------------------------------

	Auth Breakdowns
	
---------------------------------------------------------------------*/



   
   
-- ============================
-- Give some info about patient counts per insurance
-- ============================
DROP VIEW dbo.PatientCountByInsurance;
GO
CREATE VIEW dbo.PatientCountByInsurance AS (
	SELECT i.ID, i.InsuranceName, p.PatientInsuranceID, COUNT(p.PatientInsuranceID) AS PatientsApplied
	FROM dbo.Insurances AS i
	LEFT JOIN dbo.Patients AS p ON i.ID = p.PatientInsuranceID
	-- WHERE p.PatientInsuranceID IS NULL
	GROUP BY i.ID, i.InsuranceName, p.PatientInsuranceID
);

GO
   
   
   

   
-- ============================
-- Create a table to hold the auth information broken down per hour entry
-- ============================
CREATE TABLE dbo.CaseAuthHoursBreakdown (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	HoursID INT NOT NULL REFERENCES dbo.CaseAuthHours (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	CaseAuthID INT NOT NULL,
	Minutes INT NOT NULL
);
GO

CREATE INDEX idxCaseAuthHoursBreakdownHoursID ON dbo.CaseAuthHoursBreakdown (HoursID);
CREATE UNIQUE INDEX idxCaseAuthHoursBreakdownUniqueHoursAuth ON dbo.CaseAuthHoursBreakdown (HoursID, CaseAuthID);
GO

   
  
  

   