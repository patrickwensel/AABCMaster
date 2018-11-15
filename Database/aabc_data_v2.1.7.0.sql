/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.6.0
dym:TargetEndingVersion: 2.1.7.0
---------------------------------------------------------------------

	- Insurances, active and base views
	- Hours Reports
	- Date Hired
	
---------------------------------------------------------------------*/



-- ============================  
-- Insurance active and base views
-- ============================  
-- Add active insurance toggle
ALTER TABLE dbo.Insurances ADD Active BIT NOT NULL DEFAULT 1;
GO

-- Some utility views for insurances
CREATE VIEW dbo.InsurancesActive AS
	SELECT ID, InsuranceName
	FROM dbo.Insurances
	WHERE Active = 1;
GO

CREATE VIEW dbo.InsurancesInactive AS
	SELECT ID, InsuranceName
	FROM dbo.Insurances
	WHERE Active = 0;
GO

CREATE VIEW dbo.InsurancesUnused AS
	SELECT i.ID, i.InsuranceName
	FROM dbo.Insurances AS i 
	LEFT JOIN dbo.CaseInsurances AS ci ON i.ID = ci.InsuranceID
	WHERE ci.ID IS NULL;
GO

CREATE VIEW dbo.InsurancesUsed AS 
	SELECT i.ID, i.InsuranceName
	FROM dbo.Insurances AS i
	LEFT JOIN dbo.CaseInsurances AS ci ON i.ID = ci.InsuranceID
	WHERE ci.ID IS NOT NULL
	GROUP BY i.ID, i.InsuranceName;
GO
	

	
	
-- ============================  
-- Cases with no hours billed
-- ============================  
CREATE PROCEDURE [dbo].[GetCasesWithoutHoursBilled] (@StartDate DATE, @EndDate DATE) 
AS  
BEGIN
	SELECT
		base.PatientID,
		base.CaseID,
		base.ProviderID,
		base.PatientFirstName,
		base.PatientLastName,
		base.ProviderFirstName,
		base.ProviderLastName,
		base.ProviderType,
		base.Active,
		base.ActiveStartDate,
		base.ActiveEndDate,
		COALESCE(cah.TotalBillable, 0) AS Billable,
		base.WatchComment,
		base.WatchIgnore 
	FROM (
		SELECT
			p.ID AS PatientID,
			c.ID AS CaseID,
			pv.ID AS ProviderID,
			p.PatientFirstName,
			p.PatientLastName,
			pt.ProviderTypeCode AS ProviderType,
			pv.ProviderFirstName,
			pv.ProviderLastName,
			cp.Active, 
			cp.ActiveStartDate, 
			cp.ActiveEndDate,
			cmp.WatchComment,
			cmp.WatchIgnore 
		FROM dbo.Patients AS p
			INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
			INNER JOIN dbo.CaseProviders AS cp ON c.ID = cp.CaseID
			INNER JOIN dbo.Providers AS pv ON cp.ProviderID = pv.ID
			INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = pv.ProviderType
			LEFT JOIN dbo.CaseMonthlyPeriods AS cmp ON cmp.CaseID = c.ID AND cmp.PeriodFirstDayOfMonth = @StartDate
		WHERE (cp.ActiveStartDate IS NULL OR cp.ActiveStartDate <= @EndDate)
			AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= @StartDate)
			AND cp.Active = 1
			AND c.CaseStatus > -1
	) AS base
		LEFT JOIN (
			SELECT h.CaseID, h.CaseProviderID AS ProviderID, SUM(h.HoursBillable) AS TotalBillable
			FROM dbo.CaseAuthHours AS h
			WHERE h.HoursDate >= @StartDate AND h.HoursDate <= @EndDate
			GROUP BY h.CaseID, h.CaseProviderID
		) AS cah ON cah.CaseID = base.CaseID AND cah.ProviderID = base.ProviderID
	WHERE COALESCE(cah.TotalBillable, 0) = 0
	GROUP BY
		base.PatientID,
		base.CaseID,
		base.ProviderID,
		base.PatientFirstName,
		base.PatientLastName,
		base.ProviderFirstName,
		base.ProviderLastName,
		base.ProviderType,
		base.Active,
		base.ActiveStartDate,
		base.ActiveEndDate,
		COALESCE(cah.TotalBillable, 0),
		base.WatchComment,
		base.WatchIgnore 
	ORDER BY base.CaseID, base.ProviderID
END
GO


-- ============================  
-- Adding DateHired
-- ============================  
ALTER TABLE dbo.Providers
ADD ProviderHireDate DATE NULL
GO



GO
EXEC meta.UpdateVersion '2.1.7.0'
GO

