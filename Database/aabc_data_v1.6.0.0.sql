/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.5.6.0
dym:TargetEndingVersion: 1.6.0.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


   
  -- ============================
-- Add a procedure to generate permissions for all users
-- ============================

CREATE PROCEDURE dbo.GenerateWebUserPermissionsForAll AS BEGIN

	INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID, isAllowed)
		SELECT 
			matrix.UserID, 
			matrix.PermissionID, 
			0
		 FROM (
			SELECT u.ID AS UserID, p.ID AS PermissionID
			FROM dbo.WebPermissions AS p
			CROSS JOIN dbo.WebUsers AS u
		) AS matrix LEFT JOIN dbo.WebUserPermissions AS up ON up.WebUserID = matrix.UserID AND up.WebPermissionID = matrix.PermissionID
		WHERE up.ID IS NULL

END
GO


-- ============================
-- Add permission for viewing provider rates
-- ============================
INSERT INTO dbo.WebPermissions (ID, WebPermissionGroupID, WebPermissionDescription) VALUES (6, 1, N'ProviderRateView');
EXEC dbo.GenerateWebUserPermissionsForAll;
GO




-- ============================
-- Create a view to sum up the cost of an hours entry based on provider and total hours
-- ============================

CREATE VIEW dbo.HoursProviderCost AS
	SELECT
		pt.ID,
		cah.HoursDate,
		c.ID AS CaseID,
		pt.PatientInsuranceCompanyName,
		pt.PatientFirstName,
		pt.PatientLastName,
		pv.ProviderRate,
		cah.HoursTotal,
		pv.ProviderRate * cah.HoursTotal AS ProviderCost
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS pv ON pv.ID = cah.CaseProviderID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID;

GO







   
   
   
   
   
   
   
   
   
   
  -- //// END MIGRATION SCRIPT v1.5.6.0-v1.5.7.0

  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  

-- //// BEGIN MIGRATION SCRIPT v1.5.7.0-v1.5.8.0

/* ======================================================




		
   ====================================================== */
   

  
-- ============================
-- Create a proc and supporting utils to return pivot of billable hours per range by case/month
-- ============================

CREATE FUNCTION [dbo].[FullMonthsSeparation] 
(
    @DateA DATETIME,
    @DateB DATETIME
)
RETURNS INT
AS
BEGIN
    DECLARE @Result INT

    DECLARE @DateX DATETIME
    DECLARE @DateY DATETIME

    IF(@DateA < @DateB)
    BEGIN
    	SET @DateX = @DateA
    	SET @DateY = @DateB
    END
    ELSE
    BEGIN
    	SET @DateX = @DateB
    	SET @DateY = @DateA
    END

    SET @Result = (
    				SELECT 
    				CASE 
    					WHEN DATEPART(DAY, @DateX) > DATEPART(DAY, @DateY)
    					THEN DATEDIFF(MONTH, @DateX, @DateY) - 1
    					ELSE DATEDIFF(MONTH, @DateX, @DateY)
    				END
    				)

    RETURN @Result
END

GO






	

-- /*	
CREATE PROCEDURE dbo.GetBilledHoursByMonthByProviderType(
	@CaseID INT, 
	@StartDate DATE, 
	@EndDate DATE
) AS BEGIN
-- */

	 /* TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE

	SET @CaseID = 386
	SET @StartDate = '2016-01-01'
	SET @EndDate = '2016-12-31'
	-- */
	
	DECLARE @NumMonths INT
	SET @NumMonths = dbo.FullMonthsSeparation(@StartDate, @EndDate)

	SELECT 
		dates.Period AS MonthStart, 
		data.BCBAHours, 
		data.AideHours, 
		data.TotalHours
	
	FROM (

		SELECT 
			pvt.Period AS MonthStart, 
			COALESCE(pvt.BCBA, 0) AS BCBAHours, 
			COALESCE(pvt.Aide, 0) AS AideHours,
			COALESCE(pvt.BCBA, 0) + COALESCE(pvt.Aide, 0) AS TotalHours
		FROM (
			SELECT 
				DATEADD(MONTH, DATEDIFF(MONTH, 0, h.HoursDate), 0) AS Period, 
				pt.ProviderTypeCode,
				SUM(COALESCE(h.HoursBillable, 0)) AS BilledHours
			FROM dbo.CaseAuthHours AS h
			INNER JOIN dbo.Providers AS p ON p.ID = h.CaseProviderID
			INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
			WHERE h.HoursDate >= @StartDate
				AND h.HoursDate <= @EndDate
				AND h.HoursBillingRef IS NOT NULL
				AND h.CaseID = @CaseID
				AND pt.ProviderTypeCode IN ('BCBA', 'AIDE')
			GROUP BY 
				DATEADD(MONTH, DATEDIFF(MONTH, 0, h.HoursDate), 0), 
				pt.ProviderTypeCode
		) AS src
		-- /*
		PIVOT (
			SUM(src.BilledHours)
			FOR src.ProviderTypeCode IN ([BCBA], [AIDE])
		) AS pvt
		-- */

	) data

	RIGHT JOIN (

		SELECT Number, DATEADD(MONTH, Number - 1, @StartDate) AS Period
		FROM dbo.Numbers 
		WHERE Number <= @NumMonths + 1

	) AS dates ON dates.Period = data.MonthStart

	RETURN

-- /*
END
-- */
GO
	
	



   
   
   
   
   
   
   
   
   
   
  -- //// END MIGRATION SCRIPT v1.5.7.0-v1.5.8.0
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  -- //// BEGIN MIGRATION SCRIPT v1.5.8.0-v1.5.9.0

/* ======================================================




		
   ====================================================== */
   
   
   
  
-- add provider active status and active case count
ALTER PROCEDURE [dbo].[GetProviderSearchViewData] AS BEGIN


	SELECT
		p.ID,
		p.DateCreated,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
		p.ProviderCity,
		p.ProviderState,
		p.ProviderZip,
		p.ProviderPrimaryEmail,
		p.ProviderPrimaryPhone,
		ProviderServiceAreas = STUFF(
			(
				SELECT ',' + pz.ZipCode
				FROM dbo.ProviderServiceZipCodes AS pz
				WHERE p.ID = pz.ProviderID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		ProviderLanguages = STUFF(
			(
				SELECT ',' + cl.Description
				FROM dbo.CommonLanguages AS cl
				INNER JOIN dbo.ProviderLanguages AS pl ON cl.ID = pl.LanguageID
				WHERE pl.ProviderID = p.ID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		p.ProviderActive,
		COALESCE(caseCount.CaseCount, 0) AS ProviderActiveCaseCount
		
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN (
		SELECT 
			cp.ProviderID, 
			COUNT(*) AS CaseCount
		FROM dbo.CaseProviders AS cp
		WHERE cp.Active = 1
		GROUP BY cp.ProviderID
	) AS caseCount ON p.ID = caseCount.ProviderID
	
	WHERE p.ProviderActive = 1
	
	ORDER BY p.ProviderLastName;
	
END


GO









 -- add case staffing/restaffing and restaff reason fields
ALTER TABLE dbo.Cases ADD CaseNeedsStaffing BIT NOT NULL DEFAULT 0;
ALTER TABLE dbo.Cases ADD CaseNeedsRestaffing BIT NOT NULL DEFAULT 0;
ALTER TABLE dbo.Cases ADD CaseRestaffingReason NVARCHAR(255);
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
	p.PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName,
	c.CaseNeedsStaffing AS NeedsStaffing,
	c.CaseNeedsRestaffing AS NeedsRestaffing,
	c.CaseRestaffingReason AS RestaffingReason
		
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
   
   
   
   
   
   
   
   
   
  

-- ============================
-- Setup template questions that we'll pull from for new UI entries
-- ============================
CREATE TABLE dbo.HoursNoteTemplateGroups (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	GroupName NVARCHAR(255)
);
GO

INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (1, 'Supervision Notes');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (2, 'Treatment Planning Notes');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (3, 'Parent/Caregiver Notes');
GO

CREATE TABLE dbo.HoursNoteTemplates (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	TemplateGroupID INT NOT NULL REFERENCES dbo.HoursNoteTemplateGroups ON UPDATE CASCADE ON DELETE CASCADE,
	TemplateProviderTypeID INT,
	TemplateServiceTypeID INT,
	TemplateText NVARCHAR(255) NOT NULL,
	TemplateTextDescription NVARCHAR(255),
);
GO

DECLARE @ProviderTypeID INT
SET @ProviderTypeID = (SELECT TOP 1 ID FROM dbo.ProviderTypes WHERE ProviderTypeCode = 'BCBA')

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (1, 1, @ProviderTypeID, 'Provided Supervision', 'Describe some of the activities or goals that were worked on.');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (2, 1, @ProviderTypeID, 'Programs Addressed', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (3, 1, @ProviderTypeID, 'Potential modifications discussed with technician', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (4, 1, @ProviderTypeID, 'Technician feedback/future goals', NULL);
	
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (5, 2, @ProviderTypeID, 'Programs Reviewed', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (6, 2, @ProviderTypeID, 'Behaviors Reviewed', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (7, 2, @ProviderTypeID, 'Child made progress in the following areas', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (8, 2, @ProviderTypeID, 'Areas of concerns', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (9, 2, @ProviderTypeID, 'Behavior plan updates', 'Reinforcement schedule/activity schedule/social story created or modified');
	
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (10, 3, @ProviderTypeID, 'ABA principles reviewed with parents', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (11, 3, @ProviderTypeID, 'Programs reviewed', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (12, 3, @ProviderTypeID, 'Behavioral concerns', NULL);
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (13, 3, @ProviderTypeID, 'Generalization goals for parents', NULL);	
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText, TemplateTextDescription)
	VALUES (14, 3, @ProviderTypeID, 'Feedback/concerns', NULL);
	
GO
	




-- ============================
-- Setup storage for hours notes
-- ============================

CREATE TABLE dbo.CaseAuthHoursNotes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY (1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	HoursID INT NOT NULL REFERENCES dbo.CaseAuthHours (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	NotesTemplateID INT NOT NULL REFERENCES dbo.HoursNoteTemplates (ID) ON UPDATE CASCADE ON DELETE NO ACTION,
	NotesAnswer NVARCHAR(2000)	
);
CREATE INDEX idxCAHNAuthHoursID ON dbo.CaseAuthHoursNotes (ID);
CREATE INDEX idxCAHNNotesTemplateID ON dbo.HoursNoteTemplates (ID);
GO


   
   
   
   
   
   
    -- //// END MIGRATION SCRIPT v1.5.8.0-v1.5.9.0
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
  -- //// BEGIN MIGRATION SCRIPT v1.5.9.0-v1.6.0.0

/* ======================================================




		
   ====================================================== */
	
	

-- ============================
-- Add case provider active range and ins authorized BCBA
-- ============================

ALTER TABLE dbo.CaseProviders ADD IsInsuranceAuthorizedBCBA BIT NOT NULL DEFAULT 0;
ALTER TABLE dbo.CaseProviders ADD ActiveStartDate DATETIME2 NULL;
ALTER TABLE dbo.CaseProviders ADD ActiveEndDate DATETIME2 NULL;
GO
	
	
	
	
	
-- /*
-- ============================
-- Update this to return the Insurance Authorized BCBA on file per case, if any are present
-- ============================

ALTER PROCEDURE [dbo].[GetMBHMonthlyBillingExport] (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 
-- */

	 /* TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	DECLARE @BillingRef NVARCHAR(50)
	SET @FirstDayOfMonth = '2016-07-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

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

		WHERE scp.IsInsuranceAuthorizedBCBA = -1
			AND (scp.ActiveEndDate IS NULL OR scp.ActiveEndDate <= @EndDate)
			AND (scp.ActiveStartDate IS NULL OR scp.ActiveStartDate <= @StartDate)


	) AS authedBcbaByCase ON c.ID = authedBcbaByCase.CaseID
	

	WHERE cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)
		AND cah.HoursBillable <> 0
		AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= @EndDate)
		

	ORDER BY cah.HoursDate, cah.HoursTimeIn


GO






CREATE NONCLUSTERED INDEX idxHoursDateIncludeCaseID ON dbo.CaseAuthHours (HoursDate) INCLUDE (CaseID);
CREATE NONCLUSTERED INDEX idxHoursDateIncludeCaseProviderCaseID ON dbo.CaseAuthHours (HoursDate) INCLUDE (CaseProviderID, CaseID);
CREATE NONCLUSTERED INDEX idxProvIDHoursDateIncludeCaseID ON dbo.CaseAuthHours (CaseProviderID, HoursDate) INCLUDE (CaseID);
GO

-- ============================
-- Cases with Hours but no BCBA Billing
-- ============================
CREATE PROCEDURE [dbo].[GetCasesWithHoursButNoBCBAHours](@StartDate DATE, @EndDate DATE) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-08-01'
	SET @EndDate = '2016-08-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN (
	
		SELECT
			cah.CaseID,
			bcbaHours.CaseID AS bcbaCaseID
		FROM dbo.CaseAuthHours AS cah
		LEFT JOIN (
			SELECT 
				h.CaseID
			FROM dbo.CaseAuthHours AS h
			INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
			WHERE p.ProviderType = 15
				AND h.HoursDate >= @StartDate
				AND h.HoursDate <= @EndDate
			GROUP BY h.CaseID
		) AS bcbaHours ON bcbaHours.CaseID = cah.CaseID
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND bcbaHours.CaseID IS NULL
		GROUP BY cah.CaseID,
			bcbaHours.CaseID 

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN

GO





-- ============================
-- Cases with Hours but no AIDE Billing
-- ============================
CREATE PROCEDURE dbo.GetCasesWithHoursButNoAideHours(@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN (

		-- caseIDs with no Ahide Hours on file for the given date range
		SELECT
			cah.CaseID
		FROM dbo.CaseAuthHours AS cah
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.CaseID NOT IN (	
				-- get list of caseIDs in the date range which have Aide Hours applied
				SELECT 
					h.CaseID
				FROM dbo.CaseAuthHours AS h
				INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
				INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
				WHERE pt.ProviderTypeCode = 'AIDE'
					AND h.HoursDate >= @StartDate
					AND h.HoursDate <= @EndDate
				GROUP BY h.CaseID
				HAVING COUNT(h.CaseProviderID) > 0
			)
		GROUP BY cah.CaseID

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN

GO



-- ============================
-- Cases with Hours but no Direct Supervision
-- ============================
CREATE PROCEDURE dbo.GetCasesWithHoursButNoSupervision(@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN (
	
		SELECT
			cah.CaseID
		FROM dbo.CaseAuthHours AS cah
	
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.CaseID NOT IN (	
				-- get list of caseIDs in the date range which have DSU Hours applied
				SELECT 
					h.CaseID
				FROM dbo.CaseAuthHours AS h
				INNER JOIN dbo.Providers AS p ON h.CaseProviderID = p.ID
				INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
				INNER JOIN dbo.Services AS s ON s.ID = h.HoursServiceID
				WHERE pt.ProviderTypeCode = 'BCBA'
					AND h.HoursDate >= @StartDate
					AND h.HoursDate <= @EndDate
					AND s.ServiceCode = 'DSU'
				GROUP BY h.CaseID
				HAVING COUNT(h.CaseProviderID) > 0
			)
		GROUP BY cah.CaseID

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN

GO




-- ============================
-- Cases with Auths but no Hours
-- ============================
CREATE PROCEDURE dbo.GetCasesWithAuthsButNotHours(@StartDate DATETIME2, @EndDate DATETIME2) AS

	 /* TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


	SELECT 
		c.ID AS CaseID,
		p.ID AS PatientID,
		p.PatientFirstName,
		p.PatientLastName

	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN (
	
		SELECT 
			c.ID AS CaseID
		FROM dbo.Cases AS c
		INNER JOIN dbo.CaseAuthCodes AS cac ON cac.CaseID = c.ID
		LEFT JOIN dbo.CaseAuthHours AS cah ON cah.CaseID = c.ID

		WHERE cac.AuthStartDate <= @EndDate
			AND cac.AuthEndDate <= @StartDate

		GROUP BY c.ID

		HAVING COUNT(cah.ID) = 0

	) AS unmatched ON unmatched.CaseID = c.ID

	RETURN

GO
	
	
	
	
    -- //// END MIGRATION SCRIPT v1.5.9.0-v1.6.0.0	

