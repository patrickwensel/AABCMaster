/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.3.0
dym:TargetEndingVersion: 2.0.4.0
---------------------------------------------------------------------

	Views, add hours entryapp logging
	
---------------------------------------------------------------------*/


CREATE VIEW ahr.PatientPortalLoginActivity AS
	SELECT 
		p.PatientFirstName,
		p.PatientLastName,
		l.LoginFirstName,
		l.LoginLastName,
		MAX(si.SignInDate) AS LatestSignIn,
		COUNT(*) AS NumberOfSessions
	FROM dbo.PatientPortalSignIns AS si
	INNER JOIN dbo.PatientPortalLogins AS l ON l.ID = si.UserId
	INNER JOIN dbo.PatientPortalLoginPatients AS pp ON pp.LoginID = l.ID
	INNER JOIN dbo.Patients AS p ON pp.PatientID = p.ID
	GROUP BY 
		p.PatientFirstName,
		p.PatientLastName,
		l.LoginFirstName,
		l.LoginLastName
		
	GO
	
	
	
	

-- =============================
-- BCBA Notes fallback to single entry if no multi-entry
-- =============================

ALTER PROCEDURE [webreports].[PatientHoursReportDetail] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 

	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 411
	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31'
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

		p.ID AS ProviderID,
		p.ProviderLastName,
		p.ProviderFirstName,
		pt.ProviderTypeCode,
		allSignoffs.LatestSignoffDate,

		s.ServiceCode,
		cah.ID,
		CASE WHEN pt.ID = 15 THEN COALESCE(extendedNotes.NotesField, cah.HoursNotes)
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

	LEFT JOIN (

		SELECT MAX(signoffCMPPF.DateCreated) AS LatestSignoffDate,
			signoffCMPPF.ProviderID

		FROM dbo.CaseMonthlyPeriods AS signoffCMP
		INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS signoffCMPPF ON signoffCMP.ID = signoffCMPPF.CaseMonthlyPeriodID

		WHERE signoffCMP.CaseID = @CaseID
			AND signoffCMP.PeriodFirstDayOfMonth >= @StartDate
			AND signoffCMP.PeriodFirstDayOfMonth <= @EndDate

		GROUP BY signoffCMPPF.ProviderID

	) AS allSignoffs ON p.ID = allSignoffs.ProviderID

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
		
		--AND cah.CaseProviderID = 446

	ORDER BY cah.HoursDate, cah.HoursTimeIn
	
	RETURN

GO



-- ==========================
-- Add Entry App tracking for Hours
-- ==========================
ALTER TABLE dbo.CaseAuthHours ADD HoursEntryApp NVARCHAR(100);
GO



GO
EXEC meta.UpdateVersion '2.0.4.0';
GO

