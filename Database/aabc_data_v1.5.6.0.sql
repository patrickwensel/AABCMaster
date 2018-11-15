/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.5.5.0
dym:TargetEndingVersion: 1.5.6.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/

-- ============================
-- Update ServiceLocations to include Other
-- ============================

ALTER TABLE dbo.ServiceLocations ADD Active BIT NOT NULL DEFAULT 1;
GO

UPDATE dbo.ServiceLocations SET Active = 0 WHERE LocationName IN ('Center', 'School');
INSERT INTO dbo.ServiceLocations (LocationName, LocationMBHID, Active) VALUES ('Other', 4, 1);
GO

   
   
   
  
-- ============================
-- Add ServiceLocationID to Hours and default for Cases
-- ============================

ALTER TABLE dbo.CaseAuthHours ADD ServiceLocationID INT NULL;
ALTER TABLE dbo.Cases ADD DefaultServiceLocationID INT NULL;
GO



-- ============================
-- Update the procedure for MBH export to include service IDs as applicable
-- also include temporary patient/provider names for Kim to quicksort (we'll have to remove these later after in-app features make them obsolete)
-- ============================

ALTER PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 

	 /* TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	DECLARE @BillingRef NVARCHAR(50)
	SET @FirstDayOfMonth = '2016-05-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	SELECT
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
		sl.LocationMBHID AS PlaceOfServiceID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.CaseProviders AS cp ON c.ID = cp.CaseID AND cah.CaseProviderID = cp.ProviderID
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN dbo.Providers AS pv ON pv.ID = cp.ProviderID
	INNER JOIN dbo.Services AS s ON cah.HoursServiceID = s.ID
	LEFT JOIN dbo.ServiceLocations AS sl ON sl.ID = COALESCE(cah.ServiceLocationID, c.DefaultServiceLocationID)
	

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0

	ORDER BY cah.HoursDate, cah.HoursTimeIn

GO



   
  

-- ============================
-- GetPeriodHoursMatrixByCase (Total, Payable, Billable for All, BCBA, Aide)
-- ============================

CREATE PROCEDURE dbo.GetPeriodHoursMatrixByCase (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 

	/* TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 484
	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31 23:59:59'
	-- */

	SELECT final.* 
	FROM (
		
		SELECT 
			cah.CaseID,
			'AllHours' AS HoursType,
			SUM(cah.HoursTotal) AS TotalHours,
			SUM(cah.HoursBillable) AS BillableHours,
			SUM(cah.HoursPayable) AS PayableHours
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.CaseID = @CaseID
			AND cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.HoursStatus >= 2
		GROUP BY cah.CaseID
	
		UNION ALL
		
		SELECT 
			cah.CaseID,
			'BCBAHours',
			SUM(cah.HoursTotal),
			SUM(cah.HoursBillable),
			SUM(cah.HoursPayable)
		FROM dbo.CaseAuthHours AS cah
		INNER JOIN (
			SELECT
				p.ID AS ProviderID
			FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			INNER JOIN dbo.ProviderTypes AS pt ON p.ProviderType = pt.ID
			WHERE cp.CaseID = @CaseID
				AND pt.ProviderTypeCode = 'BCBA'
		) AS providerInfo ON cah.CaseProviderID = providerInfo.ProviderID
				
		WHERE cah.CaseID = @CaseID
			AND cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.HoursStatus >= 2
			
		GROUP BY cah.CaseID
		
		UNION ALL
		
		SELECT 
			cah.CaseID,
			'AideHours',
			SUM(cah.HoursTotal),
			SUM(cah.HoursBillable),
			SUM(cah.HoursPayable)
		FROM dbo.CaseAuthHours AS cah
		INNER JOIN (
			SELECT
				p.ID AS ProviderID
			FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			INNER JOIN dbo.ProviderTypes AS pt ON p.ProviderType = pt.ID
			WHERE cp.CaseID = @CaseID
				AND pt.ProviderTypeCode = 'AIDE'
		) AS providerInfo ON cah.CaseProviderID = providerInfo.ProviderID
				
		WHERE cah.CaseID = @CaseID
			AND cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.HoursStatus >= 2
			
		GROUP BY cah.CaseID
	
	) AS final
		
	RETURN
GO

   
   
   
   
   
   
 
-- //// END MIGRATION SCRIPT v1.5.5.0-v1.5.5.1


















-- //// BEGIN MIGRATION SCRIPT v1.5.5.1-v1.5.5.2

/* ======================================================




		
   ====================================================== */


-- ============================
-- Update Scrub Sproc to include hours totals and BCBA percent
-- ============================

ALTER PROCEDURE [dbo].[GetCaseTimeScrubOverview](@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN
	
	/* TEST DATA 
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	SET @StartDate = '2016/06/01';
	SET @EndDate = '2016/06/30';
	-- */
	
	SELECT
		c.ID AS CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		COUNT(DISTINCT ppc.ProviderID) AS CountOfActiveProviders,
		COUNT(DISTINCT cpwh.ProviderID) AS CountOfProvidersWithHours,
		COUNT(DISTINCT cpf.ProviderID) AS CountOfProvidersFinalized,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0) AS CountOfScrubbedRecords,
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0) AS CountOfUnscrubbedRecords,
		COALESCE(crpc.CountOfCommittedRecordsPerCase, 0) AS CountOfCommittedRecords,
		COALESCE(brpc.CountOfBilledRecordsPerCase, 0) AS CountOfBilledRecords,
		COALESCE(prpc.CountOfPaidRecordsPerCase, 0) AS CountOfPaidRecords,
		COALESCE(hcth.TotalPayable, 0) AS TotalPayable,
		COALESCE(hcth.TotalBillable, 0) AS TotalBillable,
		COALESCE(hcbh.BCBABillable, 0) AS BCBABillable,
		COALESCE(hcth.TotalBillable, 0) - COALESCE(hcbh.BCBABillable, 0) AS AideBillable,
		(CASE 
			WHEN COALESCE(hcth.TotalBillable, 0) = 0 THEN 0 
			ELSE CAST(COALESCE(hcbh.BCBABillable, 0) / hcth.TotalBillable * 100 AS INT)
			END) AS BCBAPercent
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN (
		-- active providers per case
		SELECT 
			cp.CaseID AS CaseID,
			p.ID AS ProviderID			
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE cp.Active = 1
	) ppc ON c.ID = ppc.CaseID
	LEFT JOIN (
		-- providers with hours per case
		SELECT 
			cah.CaseID AS CaseID,
			cah.CaseProviderID AS ProviderID			
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
	) AS cpwh ON c.ID = cpwh.CaseID
	LEFT JOIN (
		-- case period finalizations in the specified range
		SELECT
			cmp.ID AS PeriodID,
			cmp.CaseID,
			cmppf.ProviderID
		FROM dbo.CaseMonthlyPeriods AS cmp
		INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS cmppf ON cmp.ID = cmppf.CaseMonthlyPeriodID
		WHERE cmp.PeriodFirstDayOfMonth >= @StartDate AND cmp.PeriodFirstDayOfMonth <= @EndDate
	) AS cpf ON c.ID = cpf.CaseID
	LEFT JOIN (
		-- scrubbed records per case
		SELECT
			cah.CaseID,
			COUNT(*) AS CountOfScrubbedRecordsPerCase
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursStatus = 3
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
		GROUP BY cah.CaseID
	) AS srpc ON c.ID = srpc.CaseID
	LEFT JOIN (
		-- billed records per case
		SELECT
			cah.CaseID AS CaseID,
			COUNT(*) AS CountOfBilledRecordsPerCase
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursDate >= @StartDate 
			AND cah.HoursDate <= @EndDate
			AND cah.HoursBillingRef IS NOT NULL
		GROUP BY cah.CaseID
	) AS brpc ON c.ID = brpc.CaseID
	LEFT JOIN (
		SELECT cah.CaseID AS CaseID,
		COUNT(*) AS CountOfPaidRecordsPerCase
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursDate >= @StartDate 
			AND cah.HoursDate <= @EndDate
			AND cah.HoursPayableRef IS NOT NULL
		GROUP BY cah.CaseID
	) AS prpc ON c.ID = prpc.CaseID
	LEFT JOIN (
		-- unscrubbed records per case
		SELECT
			COALESCE(cah.CaseID, cac.CaseID) AS CaseID,
			COUNT(*) AS CountOfUnscrubbedRecordsPerCase
		FROM dbo.CaseAuthHours AS cah
		LEFT JOIN dbo.CaseAuthCodes AS cac ON cah.CaseAuthID = cac.ID
		WHERE cah.HoursStatus < 3
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
		GROUP BY COALESCE(cah.CaseID, cac.CaseID)
	) AS usrpc ON c.ID = usrpc.CaseID
	LEFT JOIN (
		-- committed records per case
		SELECT
			cah.CaseID,
			COUNT(*) AS CountOfCommittedRecordsPerCase
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursStatus = 1
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
		GROUP BY cah.CaseID
	) AS crpc ON c.ID = crpc.CaseID
	LEFT JOIN (
		-- hours count, total hours
		SELECT
			cah.CaseID,
			SUM(cah.HoursBillable) AS TotalBillable,
			SUM(cah.HoursPayable) AS TotalPayable
		FROM dbo.CaseAuthHours AS cah
		WHERE cah.HoursStatus >= 2
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
		GROUP BY cah.CaseID
	) AS hcth ON c.ID = hcth.CaseID
	LEFT JOIN (
		-- hours count, bcba hours (supervisor)
		SELECT
			cah.CaseID,
			SUM(cah.HoursBillable) AS BCBABillable
		FROM dbo.CaseAuthHours AS cah
		INNER JOIN dbo.CaseProviders AS cp ON cah.CaseProviderID = cp.ProviderID AND cp.CaseID = cah.CaseID
		WHERE cah.HoursStatus >= 2
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
			AND cp.IsSupervisor = 1
		GROUP BY cah.CaseID
	) AS hcbh ON c.ID = hcbh.CaseID


	GROUP BY 
		c.ID, 
		p.PatientFirstName, 
		p.PatientLastName,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0),
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0),
		COALESCE(crpc.CountOfCommittedRecordsPerCase, 0),
		COALESCE(brpc.CountOfBilledRecordsPerCase, 0),
		COALESCE(prpc.CountOfPaidRecordsPerCase, 0), 
		COALESCE(hcth.TotalPayable, 0),
		COALESCE(hcth.TotalBillable, 0),
		COALESCE(hcbh.BCBABillable, 0),
		COALESCE(hcth.TotalBillable, 0) - COALESCE(hcbh.BCBABillable, 0),
		(CASE 
			WHEN COALESCE(hcth.TotalBillable, 0) = 0 THEN 0 
			ELSE CAST(COALESCE(hcbh.BCBABillable, 0) / hcth.TotalBillable * 100 AS INT)
			END)
		;

	
	RETURN
 END


GO




-- //// END MIGRATION SCRIPT v1.5.5.0-v1.5.5.2


























-- //// BEGIN MIGRATION SCRIPT v1.5.5.2-v1.5.6.0

/* ======================================================




		
   ====================================================== */



   
   
  
-- ============================
-- Sproc to return detailed hours info (not done via client LINQ for performance reasons...)
-- ============================

CREATE PROCEDURE dbo.GetDetailedHoursByPeriod(@StartDate DATETIME2, @EndDate DATETIME2, @MinStatus INT) AS

	 /* TEST DATA 
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	DECLARE @MinStatus INT	-- set to 2 for finalized only, or 0 for all

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31'
	SET @MinStatus = 2
	-- */

	SELECT 
		cah.ID ,
		cah.DateCreated ,
		cah.rv ,
		cah.CaseAuthID ,
		cah.CaseProviderID ,
		cah.HoursDate ,
		cah.HoursTimeIn ,
		cah.HoursTimeOut ,
		cah.HoursTotal ,
		cah.HoursServiceID ,
		cah.HoursNotes ,
		cah.CaseID ,
		cah.HoursStatus ,
		cah.HoursBillable ,
		cah.HoursPayable ,
		cah.HoursBillingRef ,
		cah.HoursPayableRef ,
		cah.HoursHasCatalystData ,
		cah.HoursWatchEnabled ,
		cah.HoursWatchNote ,
		cah.HoursSSGParentID ,
		cah.HoursCorrelationID ,
		cah.HoursInternalNotes ,
		cah.IsPayrollOrBillingAdjustment ,
		cah.ServiceLocationID,
		pt.PatientFirstName + ' ' + pt.PatientLastName AS PatientName,
		p.ProviderFirstName + ' ' + p.ProviderLastName AS ProviderName,
		s.ServiceCode,
		ac.CodeCode AS AuthCode

	FROM dbo.caseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON cah.CaseID = c.ID
	INNER JOIN dbo.Patients AS pt ON c.PatientID = pt.ID
	INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID
	LEFT JOIN dbo.Services AS s ON cah.HoursServiceID = s.ID
	LEFT JOIN dbo.CaseAuthCodes AS cac ON cah.CaseAuthID = cac.ID
	LEFT JOIN dbo.AuthCodes AS ac ON cac.AuthCodeID = ac.ID

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus >= @MinStatus

	ORDER BY pt.PatientFirstName + ' ' + pt.PatientLastName,
		p.ProviderFirstName + ' ' + p.ProviderLastName,
		cah.HoursDate DESC,
		cah.HoursTimeIn,
		cah.HoursTimeOut

	RETURN

GO
   
   
   
   
   
   
-- ============================
-- Create proc for getting Catalyst No Data by Provider, Case
-- (edit) return only AIDEs, return only non-data dates in list
-- ============================

ALTER PROCEDURE [dbo].[GetCatalystDataMissingReportByProviderAndCase](
	@StartDate DATETIME2, 
	@EndDate DATETIME2
	) AS
BEGIN

	 /*	TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31'
	-- */

	SELECT 
		base.*,
		dates = STUFF((
				SELECT ', ' + LEFT(CONVERT(NVARCHAR, t.HoursDate, 120), 10)
				FROM dbo.CaseAuthHours AS t
				WHERE t.CaseID = base.CaseID 
					AND t.CaseProviderID = base.ProviderID
					AND t.HoursDate >= @StartDate
					AND t.HoursDate <= @EndDate
					AND t.HoursHasCatalystData = 0
				ORDER BY t.HoursDate
				FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
	
	FROM (

		SELECT 
			c.ID AS CaseID,
			p.ID AS ProviderID,
			p.ProviderFirstName,
			p.ProviderLastName,
			ptn.PatientFirstname,
			ptn.PatientLastName,
			p.ProviderPrimaryEmail,
			p.ProviderPrimaryPhone

			
		

		FROM dbo.CaseAuthHours AS cah
			INNER JOIN dbo.Cases AS c ON cah.CaseID = c.ID
			INNER JOIN dbo.Patients AS ptn ON c.PatientID = ptn.ID
			INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
			INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
		
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.HoursHasCatalystData = 0
			AND pt.ProviderTypeCode = 'AIDE'

		GROUP BY 
			c.ID,
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName,
			ptn.PatientFirstname,
			ptn.PatientLastName,
			p.ProviderPrimaryEmail,
			p.ProviderPrimaryPhone

	) AS base

	ORDER BY
		base.ProviderFirstName,
		base.ProviderLastName,
		base.PatientFirstname,
		base.PatientLastName
		
	RETURN

END

GO
   
   
   
   
   
   
   
  -- //// END MIGRATION SCRIPT v1.5.5.2-v1.5.6.0

