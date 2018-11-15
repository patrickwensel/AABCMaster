/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.3.1
dym:TargetEndingVersion: 1.7.4.0
---------------------------------------------------------------------

	Hours Reporting Sprocs
	File Uploads (rx and resumes)
	Auth/Ins Reporting
	
---------------------------------------------------------------------*/


  CREATE PROCEDURE [webreports].[ParentApprovalHoursReport] (@ParentApprovalID INT) AS 

	 --	TEST DATA
	--DECLARE @ParentApprovalID INT
	
	--SET @ParentApprovalID = 21
	-- 


		
	SELECT 
		per.PeriodFirstDayOfMonth As StartDate,
		DATEADD(DAY, -1, DATEADD(MONTH, 1, per.PeriodFirstDayOfMonth)) As EndDate,

		approvals.DateCreated As SignoffDate,

		

		pnt.PatientLastName,
		pnt.PatientFirstName,
		parents.LoginFirstName AS ParentFirstName,
		parents.LoginLastName AS ParentLastName,
		cah.HoursDate,
		CAST(cah.HoursTimeIn AS DATETIME) AS HoursTimeIn,
		CAST(cah.HoursTimeOut AS DATETIME) AS HoursTimeOut,
		cah.HoursTotal,
		p.ProviderLastName,
		p.ProviderFirstName,
		pt.ProviderTypeCode AS ProviderType,
		s.ServiceName AS ServiceName,
		cah.HoursNotes
		
		
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.CaseMonthlyPeriodParentApprovals As approvals ON approvals.ID = cah.ParentApprovalID
	INNER JOIN dbo.PatientPortalLogins As parents ON parents.ID = approvals.PatientPortalLoginID
	INNER JOIN dbo.CaseMonthlyPeriods AS per ON per.ID = approvals.MonthlyPeriodID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pnt ON pnt.ID = c.PatientID
	INNER JOIN dbo.Providers AS p ON p.ID = cah.CaseProviderID
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN dbo.[Services] AS s ON s.ID = cah.HoursServiceID

	
	WHERE cah.ParentApprovalID = @ParentApprovalID
		
	ORDER BY cah.HoursDate, cah.HoursTimeIn
	
	
	
	RETURN
	



GO





  ALTER PROCEDURE [webreports].[PatientHoursReportDetail] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 

	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 481
	SET @StartDate = '2017-01-01'
	SET @EndDate = '2017-01-31'
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
		cah.ID,
		CASE WHEN pt.ID = 15 THEN extendedNotes.NotesField
		ELSE cah.HoursNotes END AS HoursNotes
		
		
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

	--Left Join CaseAuthHoursNotes as extendedNotes on extendedNotes.HoursID = cah.ID

	LEFT JOIN(
		
		SELECT
			HoursID,
			STUFF(
				(SELECT  Char(10) + TemplateText + ':' + Char(10) + NotesAnswer + Char(10)
				From CaseAuthHoursNotes
				Inner Join HoursNoteTemplates
					on HoursNoteTemplates.ID = CaseAuthHoursNotes.NotesTemplateID
				Where HoursID = c.HoursID And NotesAnswer Is Not NULL
				FOR XML PATH (''))
				, 1, 1, '') AS NotesField
		FROM CaseAuthHoursNotes As c
		Group By HoursID


	)As extendedNotes On extendedNotes.HoursID = cah.ID
	
	WHERE cah.CaseID = @CaseID
		AND cah.HoursDate >= @StartDate
		AND cah.HoursDate <= @EndDate
		
	ORDER BY cah.HoursDate, cah.HoursTimeIn
	
	
	
	RETURN
	



GO





Alter Table [Patients]
Add 
[PrescriptionFileName] nvarchar(100),
[PrescriptionLocation] varchar(50)
GO


Alter Table [Providers]
Add 
[ResumeFileName] nvarchar(100),
[ResumeLocation] varchar(50)
GO






/* ========================================
 Get a list of authorizations that have been
 applied to cases, but which don't have a
 corresponding authorization available in the
 insurance's auth match rules
=========================================== */
CREATE VIEW dbo.AppliedAuthorizationAndInsuranceMismatches AS

	SELECT 
		patientInfo.*
		--, insuranceInfo.*
	FROM (

		-- patientInfo
		-- get all the cases and auths applied, as well as insurance info
		SELECT 
			p.PatientFirstName,
			p.PatientLastName,
			p.PatientInsuranceID,
			i.InsuranceName,
			c.ID AS CaseID,
			cac.ID AS CaseAuthCodeID,
			cac.AuthCodeID,
			cac.AuthStartDate, 
			cac.AuthEndDate,
			ac.CodeCode,
			ac.CodeDescription
		FROM dbo.Cases AS c
		INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
		INNER JOIN dbo.CaseAuthCodes AS cac ON c.ID = cac.CaseID
		INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID
		LEFT JOIN dbo.Insurances AS i ON p.PatientInsuranceID = i.ID

	) AS patientInfo

	LEFT JOIN (
	
		-- get a list of all insurances and their applicable auth codes
		SELECT 
			i.ID AS InsuranceID,
			i.InsuranceName,
			amr.RuleFinalAuthID AS AuthID
		FROM dbo.Insurances AS i
		INNER JOIN dbo.AuthMatchRules AS amr ON amr.InsuranceID = i.ID
		WHERE amr.RuleFinalAuthID IS NOT NULL

		UNION

		SELECT 
			i.ID AS InsuranceID,
			i.InsuranceName,
			amr.RuleInitialAuthID AS AuthID
		FROM dbo.Insurances AS i
		INNER JOIN dbo.AuthMatchRules AS amr ON amr.InsuranceID = i.ID
		WHERE amr.RuleInitialAuthID IS NOT NULL

	) AS insuranceInfo ON patientInfo.PatientInsuranceID = insuranceInfo.InsuranceID AND patientInfo.AuthCodeID = insuranceInfo.AuthID

	WHERE insuranceInfo.InsuranceID IS NULL

	-- ORDER BY patientInfo.PatientLastName, patientInfo.PatientFirstName, patientInfo.AuthStartDate
	;

GO








-- =============================
-- Create meta.DatabaseSettings for tracking db version
-- =============================

CREATE SCHEMA meta;
GO

CREATE TABLE meta.DatabaseSettings (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	SettingName NVARCHAR(128) NOT NULL,
	SettingValue NVARCHAR(256)
);
GO
CREATE UNIQUE INDEX idxMetaDBSettingsSettingName ON meta.DatabaseSettings (SettingName);
GO

INSERT INTO meta.DatabaseSettings (SettingName, SettingValue) VALUES ('DBVersion', '0.0.0.0');
GO


CREATE PROCEDURE meta.UpdateVersion(@Version NVARCHAR(50)) AS
BEGIN
	UPDATE meta.DatabaseSettings SET SettingValue = @Version WHERE SettingName = 'DBVersion';
END
GO




EXEC meta.UpdateVersion '1.7.4.0';
GO












