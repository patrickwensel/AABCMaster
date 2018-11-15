/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.6.4.0
dym:TargetEndingVersion: 1.6.8.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


	
	
ALTER TABLE dbo.Cases ADD CaseRestaffReasonID INT;
GO




	



-- replace case StartDate with auth LatestStartDate
-- add NeedsStaffing, NeedsRestaffing and RestaffingReason
ALTER PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
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
	auth.LatestStartDate AS StartDate,
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
	-- p.PatientInsuranceCompanyName,
	i.InsuranceName AS PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName,
	c.CaseNeedsStaffing AS NeedsStaffing,
	c.CaseNeedsRestaffing AS NeedsRestaffing,
	c.CaseRestaffingReason AS RestaffingReason,
	c.CaseRestaffReasonID AS RestaffingReasonID
		
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID

LEFT JOIN dbo.Insurances AS i ON i.ID = p.PatientInsuranceID

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
		MAX(cac.AuthEndDate) AS LatestEndDate,
		MAX(cac.AuthStartDate) AS LatestStartDate
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


/* 
	
ALTER TABLE dbo.Cases ADD CaseRestaffReasonID INT;
GO
 */
	
	
	
	
	
	
	
	
	
	
	
    -- //// END MIGRATION SCRIPT v1.6.4.0-v1.6.5.0
	
	
	
	
	
	
	
	/* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.5.0-v1.6.6.0
   ====================================================== */
	
	
	
	


ALTER TABLE dbo.Patients ADD PatientPhysicianName NVARCHAR(128);
ALTER TABLE dbo.Patients ADD PatientPhysicianAddress NVARCHAR(128);
ALTER TABLE dbo.Patients ADD PatientPhysicianPhone NVARCHAR(50);
ALTER TABLE dbo.Patients ADD PatientPhysicianFax NVARCHAR(50);
ALTER TABLE dbo.Patients ADD PatientPhysicianEmail NVARCHAR(128);
ALTER TABLE dbo.Patients ADD PatientPhysicianContact NVARCHAR(128);
ALTER TABLE dbo.Patients ADD PatientPhysicianNotes NVARCHAR(2000);
GO


INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (120, 10, 15, 'Other');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (121, 11, 15, 'Other');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (122, 12, 15, 'Other');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (123, 13, 15, 'Other');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (124, 14, 15, 'Other');
GO

	
	
	
	

	
-- /*
-- ============================
-- 
-- ============================

ALTER PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 
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
			scp.ProviderID AS ProviderID

		FROM dbo.Cases AS sc
		INNER JOIN dbo.CaseProviders AS scp ON sc.ID = scp.CaseID

		WHERE scp.IsInsuranceAuthorizedBCBA = 1
			AND (scp.ActiveEndDate IS NULL OR scp.ActiveEndDate >= @EndDate)
			AND (scp.ActiveStartDate IS NULL OR scp.ActiveStartDate <= @StartDate)


	) AS authedBcbaByCase ON c.ID = authedBcbaByCase.CaseID
	

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0
		AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= @EndDate)
				

	ORDER BY cah.HoursDate, cah.HoursTimeIn

-- */

GO




	
	

-- ============================
-- Sproc to return detailed hours info (not done via client LINQ for performance reasons...)
-- Update to handle extended notes indicator
-- ============================

ALTER PROCEDURE [dbo].[GetDetailedHoursByPeriod](@StartDate DATETIME2, @EndDate DATETIME2, @MinStatus INT) AS

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
		CASE WHEN Exists(Select * From CaseAuthHoursNotes Where HoursID = cah.ID)
			THEN 1
			ELSE 0
		END AS HasExtendedNotes,
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

	
	
	
	
	    -- //// END MIGRATION SCRIPT v1.6.5.0-v1.6.6.0
		
		
		
		
		
		
		
		
		
		
		
/* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.6.0-v1.6.8.0
   ====================================================== */
		
		
		
		
		
		
	
-- /*
-- ============================
-- Update critieria for billing export and provider end date
-- ============================

ALTER PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 
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
			scp.ProviderID AS ProviderID

		FROM dbo.Cases AS sc
		INNER JOIN dbo.CaseProviders AS scp ON sc.ID = scp.CaseID

		WHERE scp.IsInsuranceAuthorizedBCBA = 1
			AND (scp.ActiveEndDate IS NULL OR scp.ActiveEndDate >= @EndDate)
			AND (scp.ActiveStartDate IS NULL OR scp.ActiveStartDate <= @StartDate)


	) AS authedBcbaByCase ON c.ID = authedBcbaByCase.CaseID
	

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0
		AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= cah.HoursDate)
				

	ORDER BY cah.HoursDate, cah.HoursTimeIn

-- */


GO



		
		
		
		

	
	
INSERT INTO CaseAuthHoursStatuses (ID, StatusCode, StatusName, StatusDescription) VALUES (3, 'S', 'Scrubbed', 'Scrubbed');
GO


CREATE PROCEDURE [dbo].[GetHoursForDownload] (@ProviderID INT, @CaseID INT, @Month INT, @Year INT)
AS BEGIN

	SELECT
		stat.StatusName,
		cah.HoursDate,
		cah.HoursTimeIn,
		cah.HoursTimeOut,
		cah.HoursTotal,
		serv.ServiceCode,
		cah.HoursNotes,

		STUFF(
			(SELECT ' | ' + tmp.TemplateText + ': ' + can.NotesAnswer
			 FROM CaseAuthHoursNotes as can
			 INNER JOIN HoursNoteTemplates as tmp
				ON tmp.ID = can.NotesTemplateID
			 WHERE HoursID = cah.ID
			 AND NotesAnswer IS NOT NULL
			 FOR XML PATH (''))
		, 1, 1, '') as ExtendedNotes

	FROM 
		CaseAuthHours as cah
	INNER JOIN
		CaseAuthHoursStatuses as stat
		on cah.HoursStatus = stat.ID
	inner join
		[Services] as serv
		on serv.ID = cah.HoursServiceID

	where cah.CaseProviderID = @ProviderID
	and cah.CaseID = @CaseID

	and MONTH(cah.HoursDate) = @Month
	and YEAR(cah.HoursDate) = @Year

	order by cah.HoursDate asc, HoursTimeIn asc;

RETURN

END
	
	
	
	
	
	
		
		
		
		
		
		
		
GO
-- ============================
-- Create a view to sum up the cost of an hours entry based on provider and total hours
-- altered to include PatientInsuranceID
-- ============================
DROP VIEW [dbo].[HoursProviderCost];
GO

CREATE VIEW [dbo].[HoursProviderCost] AS
	SELECT
		pt.ID,
		cah.HoursDate,
		c.ID AS CaseID,
		pt.PatientInsuranceCompanyName,
		pt.PatientFirstName,
		pt.PatientLastName,
		pv.ProviderRate,
		cah.HoursTotal,
		pv.ProviderRate * cah.HoursTotal AS ProviderCost,
		pt.PatientInsuranceID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS pv ON pv.ID = cah.CaseProviderID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID;


GO		
		
		
		
		
		