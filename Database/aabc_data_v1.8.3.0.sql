/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.2.0
dym:TargetEndingVersion: 1.8.3.0
---------------------------------------------------------------------

	Refine Billing Ouput
	Fix Cascade to Null (set index unique ignore null)
	
---------------------------------------------------------------------*/



-- =======================
-- Set unique index to ignore nulls so cascade set null from deleted provider is handled
-- =======================
DROP INDEX [idxProviderPortalProviderID] ON [dbo].[ProviderPortalUsers]
GO

CREATE UNIQUE INDEX idxProviderPortalProviderID ON dbo.ProviderPortalUsers (ProviderID) WHERE ProviderID IS NOT NULL;
GO








-- /*
-- ============================
-- Update critieria for billing export and provider end date
-- ============================

Alter PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 
-- */

	 /* TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	DECLARE @BillingRef NVARCHAR(50)
	SET @FirstDayOfMonth = '2016-11-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	--PRINT @StartDate
	--PRINT @EndDate

	SELECT
		cah.ID AS HoursID,
		p.PatientFirstName AS PatientFN,
		p.PatientLastName AS PatientLN,
		pv.ProviderFirstName AS ProviderFN,
		pv.ProviderLastName AS ProviderLN,
		c.ID AS CaseID,
		c.PatientID,
		cah.CaseProviderID AS ProviderID,
		NULL AS SupervisingBCBAID,
		cp.IsSupervisor AS IsBCBATimesheet,
		cah.HoursDate AS DateOfService,
		cah.HoursTimeIn AS StartTime,
		cah.HoursTimeOut AS EndTime,
		cah.HoursBillable AS TotalTime,
		s.ServiceCode,
		sl.LocationName AS PlaceOfService,
		sl.LocationMBHID AS PlaceOfServiceID,
		authedBcbaByCase.ProviderID AS InsurancedAuthorizedProviderID,
		pt.ProviderTypeCode AS ProviderType
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.CaseProviders AS cp ON c.ID = cp.CaseID AND cah.CaseProviderID = cp.ProviderID
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN dbo.Providers AS pv ON pv.ID = cp.ProviderID
	INNER JOIN dbo.Services AS s ON cah.HoursServiceID = s.ID
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = pv.ProviderType
	LEFT JOIN dbo.ServiceLocations AS sl ON sl.ID = COALESCE(cah.ServiceLocationID, c.DefaultServiceLocationID)
	LEFT JOIN (

		SELECT 
			sc.ID AS CaseID,
			scp.ProviderID AS ProviderID,
			scp.ActiveStartDate,
			scp.ActiveEndDate

		FROM dbo.Cases AS sc
		INNER JOIN dbo.CaseProviders AS scp ON sc.ID = scp.CaseID

		WHERE scp.IsInsuranceAuthorizedBCBA = 1


	) AS authedBcbaByCase ON c.ID = authedBcbaByCase.CaseID
	and (authedBcbaByCase.ActiveEndDate IS NULL OR authedBcbaByCase.ActiveEndDate >= cah.HoursDate)
			AND (authedBcbaByCase.ActiveStartDate IS NULL OR authedBcbaByCase.ActiveStartDate <= cah.HoursDate)

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0
		--AND IsNull(cp.ActiveStartDate, '1900-01-01') <= cah.HoursDate
		--AND IsNull(cp.ActiveEndDate, '2199-01-01') >= cah.HoursDate
		--(cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= cah.HoursDate)
				

	ORDER BY cah.HoursDate, cah.HoursTimeIn

-- */




GO


EXEC meta.UpdateVersion '1.8.3.0';
GO

