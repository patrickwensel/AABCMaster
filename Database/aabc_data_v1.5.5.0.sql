/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.4.0.0
dym:TargetEndingVersion: 1.5.5.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


   
   
   
-- ============================
-- Update Patient Search sprocs to allow for discharge searching
-- ============================



-- change sproc return sort order
DROP PROCEDURE dbo.GetPatientSearchViewData;
GO

CREATE PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
 /* TEST DATA
DECLARE @ABATypeID INT
SET @ABATypeID = (SELECT ID FROM dbo.ProviderTypes WHERE ProviderTypeCode = 'AIDE')
-- */

SELECT
	c.ID,
	p.ID AS PatientID,
	p.PatientFirstName AS FirstName,
	p.PatientLastName AS LastName,
	p.PatientCity,
	p.PatientState,
	p.PatientZip AS Zip,
	c.CaseStatus AS Status,
	c.CaseStatusReason AS StatusReason,
	c.CaseStartDate AS StartDate,
	auth.LatestEndDate AS EndingAuthDate,
	c.CaseHasPrescription AS HasPrescription,
	c.CaseHasAssessment AS HasAssessment,
	c.CaseHasIntake AS HasIntake,
	CASE WHEN sup.CaseID IS NOT NULL THEN 1 ELSE 0 END AS HasSupervisor,
	sup.ProviderID AS PrimaryBCBAID,
	sup.ProviderFirstName AS BCBAFirstName,
	sup.ProviderLastName AS BCBALastName,
	aide.ProviderID AS PrimaryAideID,
	aide.ProviderFirstName AS AideFirstName,
	aide.ProviderLastName AS AideLastName,
	p.PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName
		
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT 
			cp.CaseID,
			MAX(subP.ID) AS ProviderID			
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND cp.IsSupervisor = 1 
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS sup ON sup.CaseID = c.ID

LEFT JOIN (
	SELECT
		cac.CaseID,
		MAX(cac.AuthEndDate) AS LatestEndDate
	FROM dbo.CaseAuthCodes AS cac
	WHERE cac.AuthEndDate IS NOT NULL
	GROUP BY cac.CaseID
) AS auth ON c.ID = auth.CaseID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT
			cp.CaseID,
			MIN(subP.ID) AS ProviderID		
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND subP.ProviderType = @ABATypeID
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS aide ON aide.CaseID = c.ID

LEFT JOIN dbo.Staff AS s ON s.ID = c.CaseAssignedStaffID

WHERE c.CaseStatus <> -1

ORDER BY LastName, FirstName;

RETURN
END;


GO




-- create a discharge view

CREATE PROCEDURE [dbo].[GetDischargedPatientSearchViewData](@ABATypeID INT) AS BEGIN
 /* TEST DATA
DECLARE @ABATypeID INT
SET @ABATypeID = (SELECT ID FROM dbo.ProviderTypes WHERE ProviderTypeCode = 'AIDE')
-- */

SELECT
	c.ID,
	p.ID AS PatientID,
	p.PatientFirstName AS FirstName,
	p.PatientLastName AS LastName,
	p.PatientCity,
	p.PatientState,
	p.PatientZip AS Zip,
	c.CaseStatus AS Status,
	c.CaseStatusReason AS StatusReason,
	c.CaseStartDate AS StartDate,
	auth.LatestEndDate AS EndingAuthDate,
	c.CaseHasPrescription AS HasPrescription,
	c.CaseHasAssessment AS HasAssessment,
	c.CaseHasIntake AS HasIntake,
	CASE WHEN sup.CaseID IS NOT NULL THEN 1 ELSE 0 END AS HasSupervisor,
	sup.ProviderID AS PrimaryBCBAID,
	sup.ProviderFirstName AS BCBAFirstName,
	sup.ProviderLastName AS BCBALastName,
	aide.ProviderID AS PrimaryAideID,
	aide.ProviderFirstName AS AideFirstName,
	aide.ProviderLastName AS AideLastName,
	p.PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName
		
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT 
			cp.CaseID,
			MAX(subP.ID) AS ProviderID			
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND cp.IsSupervisor = 1 
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS sup ON sup.CaseID = c.ID

LEFT JOIN (
	SELECT
		cac.CaseID,
		MAX(cac.AuthEndDate) AS LatestEndDate
	FROM dbo.CaseAuthCodes AS cac
	WHERE cac.AuthEndDate IS NOT NULL
	GROUP BY cac.CaseID
) AS auth ON c.ID = auth.CaseID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT
			cp.CaseID,
			MIN(subP.ID) AS ProviderID		
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND subP.ProviderType = @ABATypeID
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS aide ON aide.CaseID = c.ID

LEFT JOIN dbo.Staff AS s ON s.ID = c.CaseAssignedStaffID

WHERE c.CaseStatus = -1

ORDER BY LastName, FirstName;

RETURN
END;


GO
   
   
   
 
-- Normalize Case Auth Hours CaseIDs for finalized cases
UPDATE cah SET cah.CaseID = cac.CaseID
FROM dbo.CaseAuthHours AS cah
INNER JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID
WHERE cah.CaseID IS NULL AND cah.HoursStatus >= 2;
 
 
 
  
  
  
 -- ============================
-- Alter zip table to accept county data
-- ============================

ALTER TABLE dbo.ZipCodes ADD ZipCounty NVARCHAR(100);

--EXEC Manual_CopyUpdatedZipTable -- or skip this and manually fill county field
  
  
 
 
 
-- ============================
-- Fix Count of Providers w/ Hours bug and add CountOfCommittedRecords
-- ============================
 
DROP PROCEDURE [dbo].[GetCaseTimeScrubOverview]
GO

 
CREATE PROCEDURE [dbo].[GetCaseTimeScrubOverview](@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN
	
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
		COALESCE(crpc.CountOfCommittedRecordsPerCase, 0) AS CountOfCommittedRecords
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

	GROUP BY 
		c.ID, 
		p.PatientFirstName, 
		p.PatientLastName,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0),
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0),
		COALESCE(crpc.CountOfCommittedRecordsPerCase, 0) ;

	
	RETURN
END
GO


 
-- //// END MIGRATION SCRIPT v1.4.0.0-v1.5.2.0





















-- //// BEGIN MIGRATION SCRIPT v1.5.2.0-v1.5.3.0

/* ======================================================




		
   ====================================================== */
   



-- ============================
-- Add report source for unfinalized providers
-- ============================
CREATE PROCEDURE dbo.GetUnfinalizedProviders(@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN

	/* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-06-01' -- uses >= comparison
	SET @EndDate = '2016-07-01'	-- uses < comparison
	-- */

	SELECT a.*,
		CASE WHEN b.ProviderID IS NULL THEN 'N' ELSE 'Y' END AS HasFinalization

	FROM (
		SELECT
			p.ID AS ProviderID,
			p.ProviderFirstName,
			p.ProviderLastName,
			p.ProviderPrimaryEmail,
			COUNT(h.ID) AS HoursCount
		FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS h ON p.ID = h.CaseProviderID

		WHERE h.HoursStatus < 2 -- not finalized
			AND h.HoursDate >= @StartDate
			AND h.HoursDate < @EndDate

		GROUP BY 
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName,
			p.ProviderPrimaryEmail
	) AS a 

		LEFT JOIN (

		SELECT
			p.ID AS ProviderID,
			p.ProviderFirstName,
			p.ProviderLastName,
			p.ProviderPrimaryEmail,
			COUNT(h.ID) AS HoursCount
		FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS h ON p.ID = h.CaseProviderID

		WHERE h.HoursStatus = 2	-- finalized hours
			AND h.HoursDate >= @StartDate
			AND h.HoursDate < @EndDate

		GROUP BY 
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName,
			p.ProviderPrimaryEmail

	) AS b ON a.ProviderID = b.ProviderID

	RETURN
END
GO




-- ============================
-- Add additional counts to hours scrub overview
-- ============================

DROP PROCEDURE dbo.GetCaseTimeScrubOverview;
GO

CREATE PROCEDURE [dbo].[GetCaseTimeScrubOverview](@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN
	
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
		COALESCE(prpc.CountOfPaidRecordsPerCase, 0) AS CountOfPaidRecords
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

	GROUP BY 
		c.ID, 
		p.PatientFirstName, 
		p.PatientLastName,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0),
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0),
		COALESCE(crpc.CountOfCommittedRecordsPerCase, 0),
		COALESCE(brpc.CountOfBilledRecordsPerCase, 0),
		COALESCE(prpc.CountOfPaidRecordsPerCase, 0) ;

	
	RETURN
END

GO



-- ============================
-- Alter MBH Export Source sproc to exclude BillableHours = 0
-- ============================

DROP PROCEDURE [dbo].[GetMBHMonthlyBillingExport]
GO


CREATE PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 

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
		NULL AS PlaceOfService,
		NULL AS PlaceOfServiceID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.CaseProviders AS cp ON c.ID = cp.CaseID AND cah.CaseProviderID = cp.ProviderID
	INNER JOIN dbo.Services AS s ON cah.HoursServiceID = s.ID

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0

	ORDER BY cah.HoursDate, cah.HoursTimeIn


GO




   
-- ============================
-- Create proc for getting Catalyst No Data by Provider, Case
-- ============================

CREATE PROCEDURE dbo.GetCatalystDataMissingReportByProviderAndCase(
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





-- //// END MIGRATION SCRIPT v1.5.2.0-v1.5.3.0



































-- //// BEGIN MIGRATION SCRIPT v1.5.3.0-v1.5.5.0

/* ======================================================




		
   ====================================================== */
   
   
   
   
   
   
-- ============================
-- Add new tracking fields to CaseAuthHours
-- ============================

-- watch flag
ALTER TABLE dbo.CaseAuthHours ADD HoursWatchEnabled BIT NOT NULL DEFAULT 0;
ALTER TABLE dbo.CaseAuthHours ADD HoursWatchNote NVARCHAR(255);
-- SSG correlation
ALTER TABLE dbo.CaseAuthHours ADD HoursSSGParentID INT;	-- ID of the main SSG entry for this SSG group
-- general correlation
ALTER TABLE dbo.CaseAuthHours ADD HoursCorrelationID INT;	-- ID of the other hours entry to correlate to: general use
ALTER TABLE dbo.CaseAuthHours ADD HoursInternalNotes NVARCHAR(255);
-- add flag for adjustment indicator
ALTER TABLE dbo.CaseAuthHours ADD IsPayrollOrBillingAdjustment BIT NOT NULL DEFAULT 0;
GO
   
   
   
   
   
   
   
-- ============================
-- Hours Scrub Summary Sproc
-- ============================

CREATE PROCEDURE dbo.GetHoursScrubSummaryProviders (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN

	/* TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 481
	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31'
	-- */

	SELECT results.* FROM (

		SELECT 
			0 AS ReturnTypeID,	-- active providers
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE cp.CaseID = @CaseID
			AND cp.Active = 1


		UNION ALL


		SELECT
			1 AS ReturnTypeID, -- providers with hours
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			INNER JOIN dbo.CaseAuthHours AS cah ON cah.CaseProviderID = cp.ProviderID
		WHERE cah.CaseID = @CaseID
			AND cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
		GROUP BY 
			p.ID, 
			p.ProviderFirstName, 
			p.ProviderLastName


		UNION ALL


		SELECT
			2 AS ReturnTypeID, -- providers without hours
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			LEFT JOIN (
				SELECT cahSubset.*
				FROM dbo.CaseAuthHours AS cahSubset
				WHERE cahSubset.HoursDate >= @StartDate
					AND cahSubset.HoursDate < = @EndDate
					AND cahSubset.CaseID = @CaseID
			) AS cah ON cah.CaseProviderID = cp.ProviderID
		WHERE cp.Active = 1
			AND cp.CaseID = @CaseID
			AND cah.ID IS NULL
		GROUP BY 
			p.ID,
            p.ProviderFirstName,
            p.ProviderLastName


		UNION ALL


		SELECT
			3 AS ReturnTypeID, -- providers finalized 
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			LEFT JOIN (
				SELECT cahSubset.*
				FROM dbo.CaseAuthHours AS cahSubset
				WHERE cahSubset.HoursDate >= @StartDate
					AND cahSubset.HoursDate < = @EndDate
					AND cahSubset.CaseID = @CaseID
			) AS cah ON cah.CaseProviderID = cp.ProviderID
		WHERE cp.Active = 1
			AND cp.CaseID = @CaseID
			AND cah.HoursStatus >= 2
		GROUP BY 
			p.ID,
            p.ProviderFirstName,
            p.ProviderLastName


		UNION ALL


		SELECT
			4 AS ReturnTypeID, -- providers not finalized (only those with hours)
			p.ID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
			INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
			LEFT JOIN (
				SELECT cahSubset.*
				FROM dbo.CaseAuthHours AS cahSubset
				WHERE cahSubset.HoursDate >= @StartDate
					AND cahSubset.HoursDate < = @EndDate
					AND cahSubset.CaseID = @CaseID
			) AS cah ON cah.CaseProviderID = cp.ProviderID
		WHERE cp.Active = 1
			AND cp.CaseID = @CaseID
			AND (cah.HoursStatus < 2)
		GROUP BY 
			p.ID,
            p.ProviderFirstName,
            p.ProviderLastName
			
	) AS results

	ORDER BY results.ReturnTypeID, results.ProviderFirstName, results.ProviderLastName;

	RETURN
END
GO

