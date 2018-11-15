/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.2.1.0
dym:TargetEndingVersion: 1.4.0.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/



   
-- ============================
-- UPDATES FOR PERMISSIONS
-- ============================
    
ALTER TABLE dbo.WebUserPermissions ADD isAllowed BIT
GO
ALTER TABLE dbo.WebUserOptions ADD isAllowed BIT
GO
ALTER TABLE dbo.WebUsers ADD isActive BIT
GO
  
ALTER TABLE dbo.WebOptions ADD WebOptionName NVARCHAR(50);
ALTER TABLE dbo.WebPermissions ADD WebPermissionName NVARCHAR(50);
GO  
  
  
   
   
   
   

-- ============================
-- CREATE SERVICES
-- ============================

CREATE TABLE dbo.Services (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ServiceCode NVARCHAR(10) NOT NULL,
	ServiceName NVARCHAR(50) NOT NULL,
	ServiceDescription NVARCHAR(500)
);
CREATE UNIQUE INDEX idxServiceCode ON dbo.Services (ServiceCode);
GO


CREATE TABLE dbo.ProviderTypeServices (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProviderTypeID INT NOT NULL REFERENCES dbo.ProviderTypes (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ServiceID INT NOT NULL REFERENCES dbo.Services (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE UNIQUE INDEX idxProviderTypeServicesTypeService ON dbo.ProviderTypeServices (ProviderTypeID, ServiceID);
GO


INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'DR', N'Direct Care');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'PRT', N'Parent Training');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'SU', N'Supervision');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'ASS', N'Assessment');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'TP', N'Treatment Planning');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'DSU', N'Direct Supervision');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'DIS', N'Discharge');
INSERT INTO dbo.Services (ServiceCode, ServiceName) VALUES (N'SSG', N'Social Skills Group');
GO

-- INSERT BCBA SERVICES
INSERT INTO dbo.ProviderTypeServices (ProviderTypeID, ServiceID)
	SELECT pt.ID, s.ID
	FROM dbo.Services AS s
	CROSS JOIN dbo.ProviderTypes AS pt
	WHERE ProviderTypeCode = N'BCBA'
		AND s.ServiceCode IN ('DR','PRT','SU','ASS','TP','DSU','DIS','SSG');
GO
		
-- INSERT AIDE SERVICES
INSERT INTO dbo.ProviderTypeServices (ProviderTypeID, ServiceID)
	SELECT pt.ID, s.ID
	FROM dbo.Services AS s
	CROSS JOIN dbo.ProviderTypes AS pt
	WHERE ProviderTypeCode = N'AIDE'
		AND s.ServiceCode IN ('DR','PRT','SU');
GO




-- ============================
-- UPDATE CASE AUTH HOURS
-- ============================

ALTER TABLE dbo.CaseAuthHours ADD HoursTotal DECIMAL(6, 2) NOT NULL DEFAULT 0;
ALTER TABLE dbo.CaseAuthHours ADD HoursServiceID INT -- nullable? soft ref to dbo.Services



-- ============================
-- CREATE AN INSURANCE -> AUTH CODES MAPPING
-- ============================

CREATE TABLE dbo.InsuranceAuths (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	InsuranceID INT NOT NULL REFERENCES dbo.Insurances (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	AuthCodeID INT NOT NULL REFERENCES dbo.AuthCodes (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	AuthHoursPerUnit DECIMAL(6, 2) NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX idxInsuranceAuthsInsAuthIDs ON dbo.InsuranceAuths (InsuranceID, AuthCodeID);
GO


-- ============================
-- ADD NOTES TO AUTH HOURS
-- ============================
ALTER TABLE dbo.CaseAuthHours ADD HoursNotes NVARCHAR(MAX);

  
-- ============================
-- ADD AUTH CLASS ASSOCIATIONS TO SERVICES
-- ============================
ALTER TABLE dbo.ProviderTypeServices ADD AssociatedAuthClassID INT REFERENCES dbo.CaseAuthClasses (ID) ON UPDATE CASCADE ON DELETE SET NULL;
 




 
-- ============================
-- MANUALLY MAP SERVICES TO AUTH CLASSES!!!!
-- ============================
/*
EXEC MANUAL -- RUN CODE BELOW MANUALLY, USE SELECTS TO UPDATE THE UPDATE PARAMS ACCORDINGLY
SELECT * FROM dbo.CaseAuthClasses;
SELECT
	s.ID AS ServiceID,
	s.ServiceCode,
	pt.ID AS TypeID,
	pt.ProviderTypeCode,
	s.ServiceName,
	pts.ID AS MappingID,
	pts.AssociatedAuthClassID
FROM dbo.Services AS s
INNER JOIN dbo.ProviderTypeServices AS pts ON s.ID = pts.ServiceID
INNER JOIN dbo.ProviderTypes AS pt ON pts.ProviderTypeID = pt.ID
*/
  /* 
DECLARE @ID INT
DECLARE @AuthID INT

SET @AuthID = 3

UPDATE dbo.ProviderTypeServices SET AssociatedAuthClassID = @AuthID WHERE ID IN (4,5,9,10,11);
-- */
 
--EXEC ENDMANUAL; -- END MANUAL SCRIPT RUN


 
 
 
 

-- ============================
-- Update Patients List to exclude Discharged/Historic cases
-- ============================

ALTER TABLE dbo.Cases ADD CaseDischargeNotes NVARCHAR(1000);
GO

--DROP VIEW dbo.PatientCaseList;
GO

DROP PROCEDURE dbo.GetPatientSearchViewData;
GO

CREATE PROCEDURE dbo.GetPatientSearchViewData(@ABATypeID INT) AS BEGIN
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

ORDER BY FirstName, LastName;

RETURN
END;

GO



 
 
  -- //// END MIGRATION SCRIPT v1.2.1.0-v1.2.2.0
  
  
  
  
  
  
  
  
  
  
  
  
-- //// BEGIN MIGRATION SCRIPT v1.2.2.0-v1.2.4.0

/* ======================================================




		
====================================================== */


-- ============================
-- Add Cascades for Delete functionality to work properly
-- ============================

ALTER TABLE [dbo].[WebUserPermissions]  WITH CHECK ADD FOREIGN KEY([WebUserID])
REFERENCES [dbo].[WebUsers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WebUserOptions]  WITH CHECK ADD FOREIGN KEY([WebUserID])
REFERENCES [dbo].[WebUsers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

 
-- //// END MIGRATION SCRIPT v1.2.2.0-v1.2.4.0












-- //// BEGIN MIGRATION SCRIPT v1.2.4.0-v1.2.6.0

/* ======================================================




		
====================================================== */



-- ============================
-- Update CaseAuthHours to include hours without an auth
-- ============================

ALTER TABLE dbo.CaseAuthHours ALTER COLUMN CaseAuthID INT NULL;
-- constraint can't be added due to multiple/cyclic updates
-- document soft ref instead
GO

ALTER TABLE dbo.CaseAuthHours ADD CaseID INT; -- used when CaseAuthID is null
GO

 
-- //// END MIGRATION SCRIPT v1.2.4.0-v1.2.6.0



















-- //// BEGIN MIGRATION SCRIPT v1.2.6.0-v1.2.8.0

/* ======================================================




		
====================================================== */



-- ============================
-- Update for Status Notes
-- ============================
ALTER TABLE dbo.Referrals DROP COLUMN ReferralInsuranceVerificationResult;
GO
ALTER TABLE dbo.Referrals ADD ReferralStatusNotes NVARCHAR(2000);
GO

-- ============================
-- Add support for provider/referral Active/Inactive
-- ============================
ALTER TABLE dbo.Providers ADD ProviderActive BIT NOT NULL DEFAULT 1;
GO
CREATE INDEX idxProviderActive ON dbo.Providers(ProviderActive);
GO

ALTER TABLE dbo.Referrals ADD ReferralActive BIT NOT NULL DEFAULT 1;
GO
CREATE INDEX idxReferralActive ON dbo.Referrals (ReferralActive);
GO

-- ============================
-- Update for ProviderHireStatus
-- ============================
ALTER TABLE dbo.Providers ADD ProviderIsHired BIT NOT NULL DEFAULT 0;
GO
CREATE INDEX idxProviderIsHired ON dbo.Providers (ProviderIsHired);
GO



-- ============================
-- Setup Provider Search sproc
-- ============================


CREATE PROCEDURE dbo.GetProviderSearchViewData AS BEGIN

	SELECT
		p.ID,
		p.DateCreated,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
		p.ProviderCity,
		p.ProviderState,
		p.ProviderZip,
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
		)
		
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	
	WHERE p.ProviderActive = 1
	
	ORDER BY p.ProviderLastName;
	
END
GO



-- ============================
-- Setup Referral List sproc
-- ============================

CREATE PROCEDURE dbo.GetReferralSearchViewData AS BEGIN

	SELECT
		r.ID,
		r.DateCreated,
		rs.StatusName,
		r.ReferralFirstName,
		r.ReferralLastName,
		r.ReferralInsuranceCompanyName,
		r.ReferralDismissalReason,
		r.ReferralCity,
		r.ReferralState,
		r.ReferralZip,
		cl.Description AS ReferralLanguage,
		r.ReferralPhone		
	
	FROM dbo.Referrals AS r
	LEFT JOIN dbo.ReferralStatuses AS rs ON r.ReferralStatus = rs.ID
	LEFT JOIN dbo.CommonLanguages AS cl ON cl.ID = r.ReferralPrimarySpokenLanguageID
	
	WHERE r.ReferralActive = 1

	ORDER BY r.ReferralLastName

END
GO



 
-- //// END MIGRATION SCRIPT v1.2.6.0-v1.2.8.0






















-- //// BEGIN MIGRATION SCRIPT v1.2.8.0-v1.2.10.0

/* ======================================================




		
====================================================== */




-- ============================
-- Updates for Patient Contact Info (2nd, 3rd and other minor)
-- ============================

CREATE TABLE dbo.GuardianRelationships (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	RelationshipName NVARCHAR(30)
);
CREATE UNIQUE INDEX idxGuardianRelationshipsName ON dbo.GuardianRelationships (RelationshipName);
GO

INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (0, N'Mother');
INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (1, N'Father');
INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (2, N'Guardian');
INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (3, N'Grandparent');
INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (4, N'Relative');
INSERT INTO dbo.GuardianRelationships (ID, RelationshipName) VALUES (5, N'Other');
GO


ALTER TABLE dbo.Patients ADD PatientGuardianRelationshipID INT REFERENCES dbo.GuardianRelationships (ID) ON UPDATE CASCADE ON DELETE SET NULL;
ALTER TABLE dbo.Patients ADD PatientGuardianEmail NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardianCellPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardianHomePhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardianWorkPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardianNotes NVARCHAR(1000);
GO

ALTER TABLE dbo.Patients ADD PatientGuardian2FirstName NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian2LastName NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian2RelationshipID INT;
ALTER TABLE dbo.Patients ADD PatientGuardian2Email NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian2CellPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian2HomePhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian2WorkPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian2Notes NVARCHAR(1000);
GO

ALTER TABLE dbo.Patients ADD PatientGuardian3FirstName NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian3LastName NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian3RelationshipID INT;
ALTER TABLE dbo.Patients ADD PatientGuardian3Email NVARCHAR(64);
ALTER TABLE dbo.Patients ADD PatientGuardian3CellPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian3HomePhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian3WorkPhone NVARCHAR(30);
ALTER TABLE dbo.Patients ADD PatientGuardian3Notes NVARCHAR(1000);
GO

ALTER TABLE dbo.Patients ADD PatientNotes NVARCHAR(2000);
GO



-- ============================
-- Update Provider search data to include phone and email
-- ============================

/****** Object:  StoredProcedure [dbo].[GetProviderSearchViewData]    Script Date: 5/11/2016 10:55:11 PM ******/
DROP PROCEDURE [dbo].[GetProviderSearchViewData]
GO

/****** Object:  StoredProcedure [dbo].[GetProviderSearchViewData]    Script Date: 5/11/2016 10:55:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetProviderSearchViewData] AS BEGIN


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
		)
		
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	
	WHERE p.ProviderActive = 1
	
	ORDER BY p.ProviderLastName;
	
END

GO


-- ============================
-- Update Referrals View Data sproc
-- ============================

/****** Object:  StoredProcedure [dbo].[GetReferralSearchViewData]    Script Date: 5/11/2016 11:04:58 PM ******/
DROP PROCEDURE [dbo].[GetReferralSearchViewData]
GO

/****** Object:  StoredProcedure [dbo].[GetReferralSearchViewData]    Script Date: 5/11/2016 11:04:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetReferralSearchViewData] AS BEGIN

	SELECT
		r.ID,
		r.DateCreated,
		rs.StatusName,
		r.ReferralFirstName,
		r.ReferralLastName,
		r.ReferralInsuranceCompanyName,
		r.ReferralDismissalReason,
		r.ReferralAddress1,
		r.ReferralCity,
		r.ReferralState,
		r.ReferralZip,
		cl.Description AS ReferralLanguage,
		r.ReferralPhone		
	
	FROM dbo.Referrals AS r
	LEFT JOIN dbo.ReferralStatuses AS rs ON r.ReferralStatus = rs.ID
	LEFT JOIN dbo.CommonLanguages AS cl ON cl.ID = r.ReferralPrimarySpokenLanguageID
	
	WHERE r.ReferralActive = 1

	ORDER BY r.ReferralLastName

END

GO





-- //// END MIGRATION SCRIPT v1.2.8.0-v1.2.10.0

























-- //// BEGIN MIGRATION SCRIPT v1.2.10.0-v1.3.1.0

/* ======================================================




		
====================================================== */




-- ============================
-- Drop the CaseAuth constraint for no dupe case/auth code
-- ============================

DROP INDEX idxCaseAuthCodeCaseCodeClass ON dbo.CaseAuthCodes;
CREATE INDEX idxCaseAuthCodeCaseCodeClass ON dbo.CaseAuthCodes (AuthClassID);


-- //// END MIGRATION SCRIPT v1.2.10.0-v1.3.1.0

















-- //// BEGIN MIGRATION SCRIPT v1.3.1.0-v1.3.2.0

/* ======================================================




		
====================================================== */


-- ============================
-- Add Status info to AuthHours
-- ============================

ALTER TABLE dbo.CaseAuthHours ADD HoursStatus INT NOT NULL DEFAULT 0;
GO

CREATE TABLE dbo.CaseAuthHoursStatuses (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	StatusCode NVARCHAR(5) NOT NULL,
	StatusName NVARCHAR(10) NOT NULL,
	StatusDescription NVARCHAR(35) NOT NULL
);
GO

INSERT INTO dbo.CaseAuthHoursStatuses (ID, StatusCode, StatusName, StatusDescription) VALUES (0, N'P', N'Pending', N'Entered by Provider');
INSERT INTO dbo.CaseAuthHoursStatuses (ID, StatusCode, StatusName, StatusDescription) VALUES (1, N'C', N'Committed', N'Approved by Provider');
INSERT INTO dbo.CaseAuthHoursStatuses (ID, StatusCode, StatusName, StatusDescription) VALUES (2, N'F', N'Final', N'Approved by Management');
GO

-- //// END MIGRATION SCRIPT v1.3.1.0-v1.3.2.0




























-- //// BEGIN MIGRATION SCRIPT v1.3.2.0-v1.3.3.0

/* ======================================================




		
====================================================== */


-- ============================
-- Update Billable and Payable hours (and set defaults)
-- ============================

ALTER TABLE dbo.CaseAuthHours ADD HoursBillable DECIMAL(6, 2);
ALTER TABLE dbo.CaseAuthHours ADD HoursPayable DECIMAL(6, 2);
GO

UPDATE dbo.CaseAuthHours SET HoursBillable = HoursTotal WHERE HoursBillable IS NULL;
UPDATE dbo.CaseAuthHours SET HoursPayable = HoursTotal WHERE HoursPayable IS NULL;
GO

-- //// END MIGRATION SCRIPT v1.3.2.0-v1.3.3.0






































-- //// BEGIN MIGRATION SCRIPT v1.3.3.0-v1.3.4.0

/* ======================================================




		
====================================================== */




-- ============================
-- Create a table to hold monthly Periods for cases
-- ============================

CREATE TABLE dbo.CaseMonthlyPeriods (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CaseID INT NOT NULL REFERENCES dbo.Cases (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	PeriodFirstDayOfMonth DATETIME2 NOT NULL
);
GO
CREATE UNIQUE INDEX idxCaseMonthlyPeriodsCaseMonth ON dbo.CaseMonthlyPeriods (CaseID, PeriodFirstDayOfMonth);
GO


-- ============================
-- Create a Numbers table for cartesian products
-- ============================
CREATE TABLE dbo.Numbers (
	Number INT NOT NULL IDENTITY(1, 1)
);
SET NOCOUNT ON
WHILE COALESCE(SCOPE_IDENTITY(), 0) < 10000
BEGIN
	INSERT dbo.Numbers DEFAULT VALUES
END
SET NOCOUNT OFF
ALTER TABLE dbo.Numbers ADD CONSTRAINT PK_Numbers PRIMARY KEY CLUSTERED (Number)
GO


-- ============================
-- Create a sproc to get months of a specified case and range
-- ============================
CREATE PROCEDURE dbo.GetCaseMonthlyPeriods(@CaseID INT, @StartDate DATETIME, @EndDate DATETIME) AS BEGIN

	 /* TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME
	DECLARE @EndDate DATETIME
	-- */

	DECLARE @MonthDiff INT

	 /* TEST DATA
	SET @CaseID = 383
	SET @StartDate = '2015-12-05'
	SET @EndDate = '2016-06-05'
	-- */

	-- align provider start/end date with first day of month
	SET @StartDate = DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0)
	SET @EndDate = DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate), 0)
	-- get the number of months difference
	SET @MonthDiff = DATEDIFF(MONTH, @StartDate, @EndDate) + 1

	SELECT
		g.ID, 
		g.PeriodFirstDayOfMonth
	FROM (
		-- retrieve existing entries in range
		SELECT
			p.ID,
			p.PeriodFirstDayOfMonth
		FROM dbo.CaseMonthlyPeriods AS p
		WHERE p.CaseID = @CaseID 
			AND p.PeriodFirstDayOfMonth >= @StartDate 
			AND p.PeriodFirstDayOfMonth <= @EndDate

		UNION

		-- generate gap fillers that don't exist in range
		SELECT
			NULL AS ID,
			DATEADD(MONTH, n.Number - 1, @StartDate) AS GeneratedDate
		FROM dbo.Numbers AS n
		WHERE n.Number <= @MonthDiff 
			AND NOT EXISTS(
				SELECT 1 
				FROM dbo.CaseMonthlyPeriods AS t 
				WHERE t.CaseID = @CaseID 
					AND t.PeriodFirstDayOfMonth >= @StartDate
					AND t.PeriodFirstDayOfMonth <= @EndDate
					AND t.PeriodFirstDayOfMonth = DATEADD(MONTH, n.Number - 1, @StartDate)
			)
	) AS g
	ORDER BY g.PeriodFirstDayOfMonth;

	RETURN

END
GO




-- ============================
-- Create a table to hold provider monthly finalizations
-- ============================

CREATE TABLE dbo.CaseMonthlyPeriodProviderFinalizations (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CaseMonthlyPeriodID INT NOT NULL REFERENCES dbo.CaseMonthlyPeriods (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ProviderID INT NOT NULL REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	DateFinalized DATETIME2 NOT NULL
);
CREATE UNIQUE INDEX idxCaseMonthlyPeriodProviderCase ON dbo.CaseMonthlyPeriodProviderFinalizations (CaseMonthlyPeriodID, ProviderID);
GO





-- ============================
-- Summary View for Case Time Scrub
-- ============================

CREATE PROCEDURE dbo.GetCaseTimeScrubOverview(@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN
	
	 /* TEST DATA 
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	SET @StartDate = '2016/05/01';
	SET @EndDate = '2016/05/31';
	-- */
	
	SELECT
		c.ID AS CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		COUNT(DISTINCT ppc.ProviderID) AS CountOfActiveProviders,
		COUNT(DISTINCT cpwh.ProviderID) AS CountOfProvidersWithHours,
		COUNT(DISTINCT cpf.ProviderID) AS CountOfProvidersFinalized,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0) AS CountOfScrubbedRecords,
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0) AS CountOfUnscrubbedRecords
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN (
		-- active providers per case
		SELECT 
			cp.CaseID AS CaseID,
			p.ID AS ProviderID,
			cp.ID AS CaseProviderID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE cp.Active = 1
	) ppc ON c.ID = ppc.CaseID
	LEFT JOIN (
		-- providers with hours per case
		SELECT 
			cp.CaseID AS CaseID,
			p.ID AS ProviderID,
			cp.ID AS CaseProviderID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		INNER JOIN dbo.CaseAuthHours AS cah ON p.ID = cah.CaseProviderID
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
		WHERE cah.HoursStatus = 2
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
		WHERE cah.HoursStatus < 2
			AND cah.HoursDate >= @StartDate AND cah.HoursDate <= @EndDate
		GROUP BY COALESCE(cah.CaseID, cac.CaseID)
	) AS usrpc ON c.ID = usrpc.CaseID

	GROUP BY 
		c.ID, 
		p.PatientFirstName, 
		p.PatientLastName,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0),
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0);

	RETURN
END
GO












-- ============================
-- Add a billing ref so we can track our billing txns
-- ============================
ALTER TABLE dbo.CaseAuthHours ADD HoursBillingRef NVARCHAR(30);



-- ============================
-- Setup for Billing Report sources
-- ============================
GO
CREATE SCHEMA webreports
GO

CREATE PROCEDURE webreports.BillingCaseInfo (@CaseID INT) AS

	 /* TEST DATA
	DECLARE @CaseID INT
	
	SET @CaseID = 383
	-- */
		
	SELECT
		c.ID AS CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		p.PatientPhone
	FROM dbo.Cases AS c
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	WHERE c.ID = @CaseID;

GO
	

-- ============================
-- Provider Info for cases (only those with billable hours)
-- ============================

CREATE PROCEDURE webreports.BillingProviderInfo (@CaseID INT, @FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(30)) AS

	 /*	TEST DATA
	DECLARE @CaseID INT
	DECLARE @FirstDayOfMonth DATETIME2

	SET @CaseID = 383
	SET @FirstDayOfMonth = '2016-05-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	SELECT
		cp.CaseID,
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryPhone,
		pt.ProviderTypeCode,
		cp.IsSupervisor,
		cp.IsAssessor
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON p.ProviderType = pt.ID
	INNER JOIN dbo.CaseProviders AS cp ON p.ID = cp.ProviderID
	INNER JOIN dbo.CaseAuthHours AS cah ON p.ID = cah.CaseProviderID

	WHERE cp.CaseID = @CaseID
		AND cah.CaseID = @CaseID
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 2 -- finalized hours only
		AND (cah.HoursBillingRef IS NULL
			OR cah.HoursBillingRef = @BillingRef)

	GROUP BY 
		cp.CaseID,
		p.ID,
        p.ProviderFirstName,
        p.ProviderLastName,
        p.ProviderPrimaryPhone,
        pt.ProviderTypeCode,
        cp.IsSupervisor,
        cp.IsAssessor

GO



-- ============================
-- Get Billable Hours
-- ============================


CREATE PROCEDURE webreports.BillingHoursInfo (@CaseID INT, @FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(30)) AS

	 /*	TEST DATA
	DECLARE @CaseID INT
	DECLARE @FirstDayOfMonth DATETIME2

	SET @CaseID = 383
	SET @FirstDayOfMonth = '2016-05-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	SELECT
		cah.ID AS HoursID,
		p.ID AS ProviderID,
		cah.HoursDate,
		cah.HoursTimeIn,
		cah.HoursTimeOut,
		cah.HoursBillable,
		svc.ServiceCode,
		svc.ServiceName,
		cah.HoursNotes
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	LEFT JOIN dbo.Services AS svc ON svc.ID = cah.HoursServiceID

	WHERE cah.CaseID = @CaseID
		AND cah.HoursStatus = 2	-- finalized only
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND (cah.HoursBillingRef IS NULL
			OR cah.HoursBillingRef = @BillingRef)

GO






-- ============================
-- Generate a table for Billing Reports
-- ============================

CREATE TABLE dbo.CaseBillingReports (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ReportBaseID INT NOT NULL,
	ReportPeriodID INT NOT NULL REFERENCES dbo.CaseMonthlyPeriods (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ReportID NVARCHAR(30) NOT NULL,
	ReportGeneratedByUserID INT REFERENCES dbo.WebUsers (ID) ON UPDATE CASCADE ON DELETE SET NULL,
	ReportGenerationDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
CREATE UNIQUE INDEX idxCaseBillingReportID ON dbo.CaseBillingReports (ReportID);
GO






-- //// END MIGRATION SCRIPT v1.3.3.0-v1.3.4.0



































-- //// BEGIN MIGRATION SCRIPT v1.3.4.0-v1.3.5.0

/* ======================================================




		
====================================================== */



-- ============================
-- Add Payable Tracking
-- ============================

ALTER TABLE dbo.CaseAuthHours ADD HoursPayableRef NVARCHAR(30);
GO


CREATE PROCEDURE webreports.PayablesByPeriod(@FirstDayOfMonth DATETIME2) AS

	 /*	TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	SET @FirstDayOfMonth = '2016-05-01'
	-- */
	
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)
	
	SELECT
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName,
		SUM(h.HoursPayable) AS TotalPayable
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
	WHERE h.HoursStatus = 2 -- finalized hours only
		AND h.HoursDate >= @StartDate
		AND h.HoursDate <= @EndDate
		AND h.HoursPayableRef IS NULL
	GROUP BY 
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName
	ORDER BY p.ProviderLastName
	
	
	RETURN
GO




CREATE TABLE dbo.CasePayableReports (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ReportBaseID INT NOT NULL,
	ReportID NVARCHAR(30) NOT NULL
	);
CREATE UNIQUE INDEX idxCasePayableReportID ON dbo.CasePayableReports (ReportID);
GO








-- //// END MIGRATION SCRIPT v1.3.4.0-v1.3.5.0

































-- //// BEGIN MIGRATION SCRIPT v1.3.5.0-v1.3.6.0

/* ======================================================




		
====================================================== */




-- ============================
-- SPROCS FOR USER PERMISSIONS/OPTIONS
-- ============================
 CREATE PROCEDURE dbo.GenerateWebUserPermissions (@UserID INT) AS
	/* TEST DATA
	DECLARE @UserID INT
	SET @UserID = 1
	-- */

	INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID, isAllowed)
		SELECT @UserID, p.ID, 0
		FROM dbo.WebPermissions AS p 
		LEFT JOIN (
			SELECT t.*
			FROM dbo.WebUserPermissions AS t
			WHERE t.WebUserID = @UserID
		) AS up ON p.ID = up.WebPermissionID
		WHERE up.ID IS NULL

GO



CREATE PROCEDURE dbo.GenerateWebUserOptions (@UserID INT) AS

	/* TEST DATA
	DECLARE @UserID INT
	SET @UserID = 1
	-- */
	
	INSERT INTO dbo.WebUserOptions (WebUserID, WebOptionID)
		SELECT @UserID, o.ID
		FROM dbo.WebOptions AS o
		LEFT JOIN (
			SELECT t.*
			FROM dbo.WebUserOptions AS t
			WHERE t.WebUserID = @UserID
		) AS op ON o.ID = op.WebOptionID
		WHERE op.ID IS NULL
GO



-- ============================
-- Email Accounts Setup
-- ============================

CREATE TABLE dbo.SMTPAccounts (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AccountName NVARCHAR(50) NOT NULL,
	AccountDisplayName NVARCHAR(50),
	AccountUsername NVARCHAR(50) NOT NULL,
	AccountPassword NVARCHAR(50) NOT NULL,
	AccountServer NVARCHAR(50) NOT NULL,
	AccountPort SMALLINT,
	AccountUseSSL BIT,
	AccountAuthMode SMALLINT,
	AccountDefaultFromAddress NVARCHAR(50),
	AccountDefaultReplyAddress NVARCHAR(50)
);
CREATE UNIQUE INDEX idxSMTPAccountName ON dbo.SMTPAccounts (AccountName);
GO

INSERT INTO dbo.SMTPAccounts (
	AccountName, 
	AccountDisplayName, 
	AccountUsername,
	AccountPassword,
	AccountServer,
	AccountPort,
	AccountUseSSL,
	AccountAuthMode,
	AccountDefaultFromAddress,
	AccountDefaultReplyAddress
) VALUES (
	'Primary',
	NULL,
	'referrals@appliedabc.com',
	'password',
	'smtp.office365.com',
	587,
	1,
	1,
	'system@appliedabc.com',
	'no-reply@appliedabc.com'
);
GO



-- ============================
-- Export for MHB Billing
-- ============================

CREATE PROCEDURE dbo.GetMBHMonthlyBillingExport (@FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(50)) AS 

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
		AND cah.HoursStatus = 2 -- finalized hours only
		AND (cah.HoursBillingRef IS NULL OR cah.HoursBillingRef = @BillingRef)

	ORDER BY cah.HoursDate, cah.HoursTimeIn
GO





-- //// END MIGRATION SCRIPT v1.3.5.0-v1.3.6.0







































-- //// BEGIN MIGRATION SCRIPT v1.3.6.0-v1.3.7.0

/* ======================================================




		
====================================================== */


-- ============================
-- Add Service Locations
-- ============================


CREATE TABLE dbo.ServiceLocations (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	LocationName NVARCHAR(255) NOT NULL,
	LocationMBHID INT
);
CREATE UNIQUE INDEX idxServiceLocationName ON dbo.ServiceLocations (LocationName);
CREATE UNIQUE INDEX idxServiceLocationMBHID ON dbo.ServiceLocations (LocationMBHID) WHERE (LocationMBHID IS NOT NULL);
GO

INSERT INTO dbo.ServiceLocations(LocationName, LocationMBHID) VALUES (N'Home', 1);
INSERT INTO dbo.ServiceLocations(LocationName, LocationMBHID) VALUES (N'Center', 2);
INSERT INTO dbo.ServiceLocations(LocationName, LocationMBHID) VALUES (N'School', 3);
GO




-- //// END MIGRATION SCRIPT v1.3.6.0-v1.3.7.0
























-- //// BEGIN MIGRATION SCRIPT v1.3.7.0-v1.3.8.0

/* ======================================================


	HOURS STATUS UPDATES FOR FINALIZATION, SCRUB STATUS, ETC

		
====================================================== */


-- ============================
-- Update Hours Scrub Overview for new statuses
-- ============================

DROP PROCEDURE [dbo].[GetCaseTimeScrubOverview];
GO

CREATE PROCEDURE [dbo].[GetCaseTimeScrubOverview](@StartDate DATETIME2, @EndDate DATETIME2) AS BEGIN
	
	 /* TEST DATA 
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	SET @StartDate = '2016/05/01';
	SET @EndDate = '2016/05/31';
	-- */
	
	SELECT
		c.ID AS CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		COUNT(DISTINCT ppc.ProviderID) AS CountOfActiveProviders,
		COUNT(DISTINCT cpwh.ProviderID) AS CountOfProvidersWithHours,
		COUNT(DISTINCT cpf.ProviderID) AS CountOfProvidersFinalized,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0) AS CountOfScrubbedRecords,
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0) AS CountOfUnscrubbedRecords
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN (
		-- active providers per case
		SELECT 
			cp.CaseID AS CaseID,
			p.ID AS ProviderID,
			cp.ID AS CaseProviderID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		WHERE cp.Active = 1
	) ppc ON c.ID = ppc.CaseID
	LEFT JOIN (
		-- providers with hours per case
		SELECT 
			cp.CaseID AS CaseID,
			p.ID AS ProviderID,
			cp.ID AS CaseProviderID,
			p.ProviderFirstName,
			p.ProviderLastName
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Providers AS p ON p.ID = cp.ProviderID
		INNER JOIN dbo.CaseAuthHours AS cah ON p.ID = cah.CaseProviderID
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

	GROUP BY 
		c.ID, 
		p.PatientFirstName, 
		p.PatientLastName,
		COALESCE(srpc.CountOfScrubbedRecordsPerCase, 0),
		COALESCE(usrpc.CountOfUnscrubbedRecordsPerCase, 0);

	RETURN
END

GO





-- ============================
-- Update status return to scrubbed only
-- ============================

DROP PROCEDURE [webreports].[BillingHoursInfo];
GO

CREATE PROCEDURE [webreports].[BillingHoursInfo] (@CaseID INT, @FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(30)) AS

	 /*	TEST DATA
	DECLARE @CaseID INT
	DECLARE @FirstDayOfMonth DATETIME2

	SET @CaseID = 383
	SET @FirstDayOfMonth = '2016-05-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	SELECT
		cah.ID AS HoursID,
		p.ID AS ProviderID,
		cah.HoursDate,
		cah.HoursTimeIn,
		cah.HoursTimeOut,
		cah.HoursBillable,
		svc.ServiceCode,
		svc.ServiceName,
		cah.HoursNotes
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	LEFT JOIN dbo.Services AS svc ON svc.ID = cah.HoursServiceID

	WHERE cah.CaseID = @CaseID
		AND cah.HoursStatus = 3	-- srubbed only
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND (cah.HoursBillingRef IS NULL
			OR cah.HoursBillingRef = @BillingRef)


GO








-- ============================
-- Update Provider Info for cases (only those with billable hours)
-- ============================

DROP PROCEDURE [webreports].[BillingProviderInfo];
GO

CREATE PROCEDURE [webreports].[BillingProviderInfo] (@CaseID INT, @FirstDayOfMonth DATETIME2, @BillingRef NVARCHAR(30)) AS

	 /*	TEST DATA
	DECLARE @CaseID INT
	DECLARE @FirstDayOfMonth DATETIME2

	SET @CaseID = 383
	SET @FirstDayOfMonth = '2016-05-01'
	-- */

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)

	SELECT
		cp.CaseID,
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryPhone,
		pt.ProviderTypeCode,
		cp.IsSupervisor,
		cp.IsAssessor
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON p.ProviderType = pt.ID
	INNER JOIN dbo.CaseProviders AS cp ON p.ID = cp.ProviderID
	INNER JOIN dbo.CaseAuthHours AS cah ON p.ID = cah.CaseProviderID

	WHERE cp.CaseID = @CaseID
		AND cah.CaseID = @CaseID
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		AND cah.HoursStatus = 3 -- scrubbed hours only
		AND (cah.HoursBillingRef IS NULL
			OR cah.HoursBillingRef = @BillingRef)

	GROUP BY 
		cp.CaseID,
		p.ID,
        p.ProviderFirstName,
        p.ProviderLastName,
        p.ProviderPrimaryPhone,
        pt.ProviderTypeCode,
        cp.IsSupervisor,
        cp.IsAssessor


GO






-- ============================
-- Update status for MBH Export
-- ============================

DROP PROCEDURE [dbo].[GetMBHMonthlyBillingExport];
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

	ORDER BY cah.HoursDate, cah.HoursTimeIn

GO





DROP PROCEDURE [webreports].[PayablesByPeriod]
GO

CREATE PROCEDURE [webreports].[PayablesByPeriod](@FirstDayOfMonth DATETIME2) AS

	 /*	TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	SET @FirstDayOfMonth = '2016-05-01'
	-- */
	
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)
	
	SELECT
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName,
		SUM(h.HoursPayable) AS TotalPayable
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
	WHERE h.HoursStatus = 3 -- finalized hours only
		AND h.HoursDate >= @StartDate
		AND h.HoursDate <= @EndDate
		AND h.HoursPayableRef IS NULL
	GROUP BY 
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName
	ORDER BY p.ProviderLastName
	
	
	RETURN


GO



-- //// END MIGRATION SCRIPT v1.3.7.0-v1.3.8.0


































-- //// BEGIN MIGRATION SCRIPT v1.3.8.0-v1.4.0.0

/* ======================================================


	HOURS STATUS UPDATES FOR FINALIZATION, SCRUB STATUS, ETC

		
====================================================== */




-- ============================
-- Add Catalyst Data Tracking
-- ============================
ALTER TABLE dbo.CaseAuthHours ADD HoursHasCatalystData BIT NOT NULL DEFAULT 0;








-- //// END MIGRATION SCRIPT v1.3.8.0-v1.4.0.0