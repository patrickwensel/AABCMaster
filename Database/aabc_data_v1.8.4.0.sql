/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.3.0
dym:TargetEndingVersion: 1.8.4.0
---------------------------------------------------------------------

	-- Access Reporting Views
	
	
---------------------------------------------------------------------*/




CREATE VIEW dbo.FailedHoursBreakdowns AS

	SELECT  
		cah.ID AS HourID,
		cah.CaseID,
		cah.CaseProviderID AS ProviderID,
		cah.HoursDate,
		cah.HoursStatus,
		cah.HoursBillable,
		cah.HoursServiceID,
		p.PatientInsuranceID AS InsuranceID,
		s.ServiceCode,
		i.InsuranceName

	FROM dbo.CaseAuthHours AS cah
	LEFT JOIN dbo.CaseAuthHoursBreakdown AS cahb ON cah.ID = cahb.ID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	LEFT JOIN dbo.Services AS s ON s.ID = cah.HoursServiceID
	LEFT JOIN dbo.Insurances AS i ON i.ID = p.PatientInsuranceID

	WHERE cahb.ID IS NULL
	-- ORDER BY cah.HoursDate DESC
	;
	
GO

CREATE VIEW dbo.FailedHoursBreakdownsByInsurance AS

	SELECT 
		COUNT(*) AS CountOfEntries, 
		x.InsuranceID,
		x.InsuranceName, 
		MIN(x.HoursDate) AS MinDate, 
		MAX(x.HoursDate) AS MaxDate
	FROM dbo.FailedHoursBreakdowns AS x
	GROUP BY x.InsuranceID, x.InsuranceName
	--ORDER BY COUNT(*) DESC
	;

GO
	
	
	
	
	
CREATE VIEW dbo.FailedHoursBreakdownsByInsuranceAndService AS

	SELECT 
		COUNT(*) AS CountOfEntries, 
		x.InsuranceName, 		
		x.ServiceCode,
		MIN(x.HoursDate) AS MinDate, 
		MAX(x.HoursDate) AS MaxDate,
		x.InsuranceID,
		x.HoursServiceID
	FROM dbo.FailedHoursBreakdowns AS x
	GROUP BY 
		x.InsuranceID, 
		x.InsuranceName, 
		x.HoursServiceID, 
		x.ServiceCode
	--ORDER BY x.InsuranceName, x.ServiceCode, COUNT(*) DESC
	;

GO
	
	
	
	
	
CREATE PROCEDURE dbo.FailedHoursBreakdownsByCase (@InsuranceID INT) AS

	/* TEST DATA
		DECLARE @InsuranceID INT = 73
	-- */

	SELECT 
		x.InsuranceID,
		x.InsuranceName,
		COUNT(*) AS CountOfEntries,
		x.CaseID, 
		p.PatientFirstName,
		p.PatientLastName,
		MIN(x.HoursDate) AS MinFailureDate,
		MAX(x.HoursDate) AS MaxFailureDate
	FROM dbo.FailedHoursBreakdowns AS x
	INNER JOIN dbo.Cases AS c ON c.ID = x.CaseID
	INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID
	WHERE x.InsuranceID = @InsuranceID
	GROUP BY 
		x.InsuranceID,
		x.InsuranceName,
		x.CaseID,
		p.PatientFirstName,
		p.PatientLastName
	ORDER BY COUNT(*) DESC
	;
	
GO






-- Insurance Credentials

CREATE TABLE dbo.ProviderInsuranceCredentials
	(
	Id int NOT NULL IDENTITY (1, 1),
	ProviderId int NOT NULL,
	InsuranceId int NOT NULL,
	StartDate datetime NULL,
	EndDate datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ProviderInsuranceCredentials ADD CONSTRAINT
	PK_ProviderInsuranceCredentials PRIMARY KEY CLUSTERED 
	(
	Id
	)
GO
ALTER TABLE dbo.ProviderInsuranceCredentials ADD CONSTRAINT
	FK_ProviderInsuranceCredentials_Providers FOREIGN KEY
	(
	ProviderId
	) REFERENCES dbo.Providers
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ProviderInsuranceCredentials ADD CONSTRAINT
	FK_ProviderInsuranceCredentials_Insurances FOREIGN KEY
	(
	InsuranceId
	) REFERENCES dbo.Insurances
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO



-- Task Refinements
Alter Table CaseNoteTasks Add CompletedRemarks nvarchar(2000)






GO
EXEC meta.UpdateVersion '1.8.4.0';
GO

