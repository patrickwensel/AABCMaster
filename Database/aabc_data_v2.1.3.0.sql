/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.2.0
dym:TargetEndingVersion: 2.1.3.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/



ALTER PROCEDURE [webreports].[PayablesByPeriod](@FirstDayOfMonth DATETIME2) AS

	/*	TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	SET @FirstDayOfMonth = '2017-05-01'
	-- */
	
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)
	
	SELECT
		p.ID,
		COALESCE(p.PayrollID, p.ID) AS PayrollID,
		p.ProviderFirstName,
		p.ProviderLastName,
		SUM(h.HoursPayable) AS TotalPayable,
		COUNT(h.HoursPayable) - SUM(CAST(h.HoursHasCatalystData AS INT)) AS EntriesMissingCatalystData
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
	WHERE h.HoursStatus = 3 -- finalized hours only
		AND h.HoursDate >= @StartDate
		AND h.HoursDate <= @EndDate
		AND h.HoursPayableRef IS NULL
	GROUP BY 
		p.ID,
		COALESCE(p.PayrollID, p.ID),
		p.ProviderFirstName,
		p.ProviderLastName
	ORDER BY p.ProviderLastName
	
	
	RETURN

GO






GO
EXEC meta.UpdateVersion '2.1.3.0'
GO

