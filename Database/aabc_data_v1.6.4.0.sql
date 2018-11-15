/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.6.0.0
dym:TargetEndingVersion: 1.6.4.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/
   

   
  -- ============================
-- Create logins for patient portal
-- ============================
CREATE TABLE dbo.PatientPortalLogins (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	LoginEmail NVARCHAR(128) NOT NULL,
	LoginFirstName NVARCHAR(128) NOT NULL,
	LoginLastName NVARCHAR(128) NOT NULL,
	LoginPassword NVARCHAR(255) NOT NULL
);
GO

CREATE UNIQUE INDEX idxUniquePatientPortalEmail ON dbo.PatientPortalLogins (LoginEmail);
GO


-- ============================
-- Junction between Logins and Patients
-- ============================
CREATE TABLE dbo.PatientPortalLoginPatients (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	LoginID INT NOT NULL REFERENCES dbo.PatientPortalLogins (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	PatientID INT NOT NULL REFERENCES dbo.Patients (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
GO
CREATE INDEX idxPatientPortalLoginPatientID ON dbo.PatientPortalLoginPatients (PatientID);
CREATE INDEX idxPatientPortalLoginLoginID ON dbo.PatientPortalLoginPatients (LoginID);
GO



   

  CREATE PROCEDURE [webreports].[PatientHoursReportDetail] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 

	 /*	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 480
	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */


		
	SELECT 
		@StartDate AS StartDate,
		@EndDate AS EndDate,

		signoff.SignoffDate,
		COALESCE(authd.ID, supervisor.ID, bcba.ID) AS reportedBCBAID,
		COALESCE(authd.FirstName, supervisor.FirstName, bcba.FirstName) AS reportedBCBAFirstName,
		COALESCE(authd.Lastname, supervisor.LastName, bcba.LastName) AS reportedBCBALastName,

		

		pnt.PatientLastName,
		pnt.PatientFirstName,
		cah.HoursDate,
		CAST(cah.HoursTimeIn AS DATETIME) AS HoursTimeIn,
		CAST(cah.HoursTimeOut AS DATETIME) AS HoursTimeOut,
		cah.HoursTotal,
		p.ProviderLastName,
		p.ProviderFirstName,
		pt.ProviderTypeCode,
		s.ServiceCode,
		cah.HoursNotes
		
		
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pnt ON pnt.ID = c.PatientID
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN dbo.Services AS s ON s.ID = cah.HoursServiceID
	LEFT JOIN (

		SELECT
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaCP.ProviderID

		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaCP.IsSupervisor = -1
			--AND bcbaCAH.HoursDate >= @Startdate
			--AND bcbaCAH.HoursDate <= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)

	) AS supervisor ON supervisor.CaseID = cah.CaseID

	LEFT JOIN (

		SELECT TOP 1
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		-- INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaP.ID
		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaP.ProviderType = 15
			AND bcbaCP.IsInsuranceAuthorizedBCBA = -1
			--AND bcbaCAH.HoursDate >= @StartDate
			--AND bcbaCAH.HoursDate >= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)
			
	) AS authd ON authd.CaseID = cah.CaseID

	LEFT JOIN (

		SELECT TOP 1
			bcbaCP.CaseID,
			bcbaP.ID,
			bcbaP.ProviderFirstName AS FirstName,
			bcbaP.ProviderLastName AS LastName
		FROM dbo.CaseProviders AS bcbaCP
		INNER JOIN dbo.Providers AS bcbaP ON bcbaP.ID = bcbaCP.ProviderID
		INNER JOIN dbo.CaseAuthHours AS bcbaCAH ON bcbaCAH.CaseProviderID = bcbaP.ID
		WHERE bcbaCP.CaseID = @CaseID
			AND bcbaP.ProviderType = 15
			--AND bcbaCAH.HoursDate >= @StartDate
			--AND bcbaCAH.HoursDate >= @EndDate
			AND ((bcbaCP.ActiveEndDate IS NULL AND bcbaCP.ActiveStartDate IS NOT NULL) OR bcbaCP.ActiveEndDate >= @EndDate)
			AND ((bcbaCP.ActiveStartDate IS NULL AND bcbaCP.ActiveEndDate IS NOT NULL) OR bcbaCP.ActiveStartDate <= @StartDate)

	) AS bcba ON bcba.CaseID = cah.CaseID


	LEFT JOIN (

		SELECT TOP 1
			signoffCMP.CaseID,
			signoffCMPPF.DateCreated AS SignoffDate

		FROM dbo.CaseMonthlyPeriods AS signoffCMP
		INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS signoffCMPPF ON signoffCMP.ID = signoffCMPPF.CaseMonthlyPeriodID

		WHERE signoffCMP.CaseID = @CaseID
			AND signoffCMP.PeriodFirstDayOfMonth >= @StartDate
			AND signoffCMP.PeriodFirstDayOfMonth <= @EndDate

	) AS signoff ON signoff.CaseID = cah.CaseID

	
	WHERE cah.CaseID = @CaseID
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		
	ORDER BY cah.HoursDate, cah.HoursTimeIn
	
	
	
	RETURN
	



GO
   
   
   
   
   
  -- //// END MIGRATION SCRIPT v1.6.0.0-v1.6.1.0

  
  
  
  
  
  
  
  
  
  

/* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.1.0-v1.6.2.0		
   ====================================================== */
   
   
   
   
   
   
-- ============================
-- Update BCBA Questions
-- ============================

DELETE FROM dbo.CaseAuthHoursNotes;
DELETE FROM dbo.HoursNoteTemplates;
DELETE FROM dbo.HoursNoteTemplateGroups;

INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (10, 'Assessment');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (11, 'Direct');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (12, 'Supervision');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (13, 'Treatment Planning');
INSERT INTO dbo.HoursNoteTemplateGroups (ID, GroupName) VALUES (14, 'Parent/Caregiver Training');
GO

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (100, 10, 15, 'See assessment notes in treatment plan');

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (101, 11, 15, 'Implemented functional behavior assessment');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (102, 11, 15, 'Observed, assessed or did functional analysis for behaviors');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (103, 11, 15, 'Determined appropriate ABA targets');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (104, 11, 15, 'Collected data using Catalyst programs');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (105, 11, 15, 'Assessed the following ABA techniques/teaching methods');

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (106, 12, 15, 'Provided therapist supervision for the following ABA principles');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (107, 12, 15, 'Programs addressed');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (108, 12, 15, 'Potential behavior modifications discussed with therapist');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (109, 12, 15, 'Therapist feedback/future goals');

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (110, 13, 15, 'Programs reviewed');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (111, 13, 15, 'Behaviors reviewed');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (112, 13, 15, 'Child made progress in the following area(s)');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (113, 13, 15, 'Area of concern');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (114, 13, 15, 'Behavior plan updates');

INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (115, 14, 15, 'ABA principles reviewed with parents');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (116, 14, 15, 'Programs reviewed');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (117, 14, 15, 'Behavioral concerns');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (118, 14, 15, 'Generalization goal for parents');
INSERT INTO dbo.HoursNoteTemplates (ID, TemplateGroupID, TemplateProviderTypeID, TemplateText) VALUES (119, 14, 15, 'Feedback/concerns');

GO
   
   
  -- //// END MIGRATION SCRIPT v1.6.1.0-v1.6.2.0
  
  
  
 /* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.2.0-vX		
   ====================================================== */

   
   
   
  
ALTER TABLE dbo.PatientPortalLogins ADD Active BIT NOT NULL DEFAULT 0;
GO

CREATE TABLE dbo.PatientPortalWebMembership (
	[ID] [int] NOT NULL PRIMARY KEY CLUSTERED,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[rv] [timestamp] NOT NULL,
	[MemberPassword] [nvarchar](500) NOT NULL,
	[MemberPasswordQuestion] [nvarchar](500) NULL,
	[MemberPasswordAnswer] [nvarchar](128) NULL,
	[MemberIsApproved] [bit] NOT NULL DEFAULT 0,
	[MemberLastActivityDateUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberLastLoginDateUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberLastPasswordChangedDateUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberCreationDateUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberIsLockedOut] [bit] NOT NULL DEFAULT 0,
	[MemberLastLockoutDateUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberFailedPasswordAttemptCount] [int] NOT NULL DEFAULT 0,
	[MemberFailedPasswordWindowStartUTC] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[MemberFailedPasswordAnswerAttemptCount] [int] NOT NULL DEFAULT 0,
	[MemberFailedPasswordAnswerAttemptWindowStartUTC] [datetime2](7) NOT NULL DEFAULT GETDATE()
);
GO

   
   
   
   
   
  
-- ============================
-- Add Parent Approvals
-- ============================

CREATE TABLE dbo.CaseMonthlyPeriodParentApprovals (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	MonthlyPeriodID INT NOT NULL REFERENCES dbo.CaseMonthlyPeriods (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	PatientPortalLoginID INT NOT NULL REFERENCES dbo.PatientPortalLogins (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ApprovalDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO


ALTER TABLE dbo.CaseAuthHours ADD ParentApprovalID INT;
GO 


CREATE TABLE dbo.PatientPortalLoginSignatures (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	LoginID INT NOT NULL REFERENCES dbo.PatientPortalLogins (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	SignatureData NVARCHAR(MAX),
	SignatureDate DATETIME2 NOT NULL,
);
GO
   
   
   
   
   
   
   
   
   
   
   
   
    -- //// END MIGRATION SCRIPT v1.6.1.0-v1.6.2.0
	
	
	
	
	
	
	
	
	
/* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.2.0-v1.6.3.0
   ====================================================== */
	
	
	
	
	
	

CREATE TABLE [dbo].[CaseNotes](
	[ID] [int] NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	[CaseID] [int] NOT NULL,
	[CorrespondenceName] [nvarchar](255) NULL,
	[Comments] [nvarchar](2000) NULL,
	[CorrespondenceType] [int] NULL,
	[EntryDate] [datetime2](7) NULL,
	[EnteredByUserID] [int] NULL)

GO

ALTER TABLE [dbo].[CaseNotes]  WITH CHECK ADD FOREIGN KEY([CaseID])
REFERENCES [dbo].[Cases] ([ID])
GO

ALTER TABLE [dbo].[CaseNotes]  WITH CHECK ADD FOREIGN KEY([EnteredByUserID])
REFERENCES [dbo].[WebUsers] ([ID])
GO	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
    -- //// END MIGRATION SCRIPT v1.6.2.0-v1.6.3.0
	
		
/* ======================================================
-- //// BEGIN MIGRATION SCRIPT v1.6.3.0-v1.6.4.0
   ====================================================== */
	
	
	
	
	
	
	

-- ============================
-- Track rules for services to auths by insurance and provider type
-- ============================

CREATE TABLE dbo.AuthMatchRules (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	InsuranceID INT NOT NULL REFERENCES dbo.Insurances (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ProviderTypeID INT NOT NULL REFERENCES dbo.ProviderTypes (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ServiceID INT NOT NULL REFERENCES dbo.Services (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	
	RuleBillingMethod INT NOT NULL DEFAULT 0, -- 0:Time, 1:Service
	RuleInitialAuthID INT, -- REFERENCES dbo.AuthCodes (ID) ON UPDATE CASCADE ON DELETE SET NULL,
	RuleInitialMinimumMinutes INT,
	RuleInitialUnitSize INT,
	RuleFinalAuthID INT, -- REFERENCES dbo.AuthCodes (ID) ON UPDATE CASCADE ON DELETE SET NULL,
	RuleFinalMinimumMinutes INT,
	RuleFinalUnitSize INT,
	RuleAllowOverlapping BIT NOT NULL DEFAULT 0,
	RuleRequiresAuthorizedBCBA BIT NOT NULL DEFAULT 0,
	RuleRequiresPreAuthorization BIT NOT NULL DEFAULT 1
);
GO
CREATE INDEX idxAuthMatchRuleInsurance ON dbo.AuthMatchRules (InsuranceID);
CREATE INDEX idxAuthMatchRuleProviderType ON dbo.AuthMatchRules (ProviderTypeID);
CREATE INDEX idxAuthMatchRuleService ON dbo.AuthMatchRules (ServiceID);
GO
	
	
	
	
	

-- ============================
-- Add Insurance Tracking to Patient
-- ============================
ALTER TABLE dbo.Patients ADD PatientInsuranceID INT REFERENCES dbo.Insurances (ID) ON UPDATE CASCADE ON DELETE SET NULL;
GO
CREATE INDEX idxPatientInsurance ON dbo.Patients (PatientInsuranceID);
GO

-- ============================
-- Clean up pre-existing and unused objects
-- ============================
DROP INDEX idxInsuranceCodes ON dbo.Insurances;
GO
ALTER TABLE dbo.Insurances DROP COLUMN InsuranceCode;
GO
DROP TABLE dbo.InsuranceAuths;
GO

-- ============================
-- Populate the Insurances table
-- ============================
INSERT INTO dbo.Insurances (InsuranceName) 
SELECT DISTINCT PatientInsuranceCompanyName FROM dbo.Patients WHERE PatientInsuranceCompanyName IS NOT NULL;
GO
	
-- ============================
-- Update Patient Insurance IDs to match Insurance Names
-- ============================
UPDATE t SET t.PatientInsuranceID = s.ID
FROM dbo.Patients AS t
INNER JOIN dbo.Insurances AS s ON s.InsuranceName = t.PatientInsuranceCompanyName;
GO


-- ============================
-- Update Patient search views to pull insurance name from table instead of field
-- ============================

 ALTER PROCEDURE [dbo].[GetDischargedPatientSearchViewData](@ABATypeID INT) AS BEGIN
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
	-- p.PatientInsuranceCompanyName,
	i.InsuranceName AS PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName,
	c.CaseNeedsStaffing AS NeedsStaffing,
	c.CaseNeedsRestaffing AS NeedsRestaffing,
	c.CaseRestaffingReason AS RestaffingReason
		
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
	c.CaseRestaffingReason AS RestaffingReason
		
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





-- ============================
-- Add Permission for InsuranceEdit
-- ============================

INSERT INTO dbo.WebPermissions (ID, WebPermissionGroupID, WebPermissionDescription) VALUES (7, 1, 'InsuranceEdit');
GO
EXEC dbo.GenerateWebUserPermissionsForAll;
GO
-- verify 1,4,11 is jack, kim and yakov
UPDATE dbo.WebUserPermissions SET isAllowed = 1 WHERE WebUserID IN (1,4,11) AND WebPermissionID = 7;
GO

	
	

-- ============================
-- Updates for new case ntoes functionality
-- ============================	
Alter Table CaseNotes
Add Foreign Key (EnteredByUserID)
References WebUsers(ID)

Alter Table CaseNotes
Add 
RequiresFollowup bit not null default(0),
FollowupDate DateTime2(7) null,
FollowupComplete bit not null default(0),
FollowupCompleteDate DateTime2(7) null,
FollowupUserID int null,
FollowupComment nvarchar(2000) null


Create Table dbo.CaseNoteTasks(
[ID] [int] NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
[NoteID] int Not NULL,
[Description] nvarchar(255),
[DueDate] datetime2(7),
[AssignedTo] int,
Completed bit not null,
CompletedBy int null
)

Alter Table [dbo].CaseNoteTasks With Check Add Foreign Key ([AssignedTo])
References [dbo].Staff (ID)

Alter Table [dbo].CaseNoteTasks With Check Add Foreign Key (NoteID)
References [dbo].CaseNotes (ID)

Alter Table CaseNoteTasks With Check Add Foreign Key (CompletedBy)
References WebUsers(ID)


GO

	
	
	
	
	
	
    -- //// END MIGRATION SCRIPT v1.6.3.0-v1.6.4.0
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	

	