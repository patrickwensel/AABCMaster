/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.2.0
dym:TargetEndingVersion: 2.0.3.0
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


CREATE INDEX idxCaseInsuranceEffective ON dbo.CaseInsurances (DatePlanEffective);
CREATE INDEX idxCaseInsuranceDefective ON dbo.CaseInsurances (DatePlanTerminated);
CREATE INDEX idxCaseInsuranceCaseIDFK ON dbo.CaseInsurances (CaseID);
GO


CREATE VIEW dbo.PatientCurrentInsurance AS
	SELECT	
		p.ID AS PatientID,
		c.ID AS CaseID,
		ci.InsuranceID,
		ci.DatePlanEffective,
		ci.DatePlanTerminated
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID
	LEFT JOIN dbo.CaseInsurances AS ci 
		ON c.ID = ci.CaseID 
		AND (ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE());

GO

-- Update patient search procs to use currently active insurance

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
	
	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON pci.CaseID = c.ID

	LEFT JOIN dbo.Insurances AS i ON i.ID = pci.InsuranceID

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
	END

GO






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

	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON c.ID = pci.CaseID

	LEFT JOIN dbo.Insurances AS i ON i.ID = pci.InsuranceID

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
	END
GO










-- get just a list of signoffs
-- update the notes temp field length to 4000 from 1000
ALTER PROCEDURE [webreports].[PatientHoursReportDetailSignatures] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 
 	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 671
	SET @StartDate = '11/1/2017'
	SET @EndDate = '11/30/2017'
	-- */
	
	DECLARE @tablevar TABLE(
		StartDate DATETIME2, EndDate DATETIME2, SignoffDate DATETIME2, reportedBCBAID INT, reportedBCBAFirstName NVARCHAR(100), reportedBCBALastName NVARCHAR(100),
		PatientLastName NVARCHAR(100), PatientFirstName NVARCHAR(100), HoursDate DATETIME2, HoursTimeIn DATETIME2, HoursTimeOut DATETIME2, HoursTotal DECIMAL(10,2),
		ProviderID INT, ProviderLastName NVARCHAR(100), ProviderFirstName NVARCHAR(100), ProviderTypeCode NVARCHAR(100), LatestSignoffDate DATETIME2, ServiceCode NVARCHAR(100), ID INT, HoursNotes NVARCHAR(4000));
	INSERT INTO @tablevar (
		StartDate, EndDate, SignoffDate, reportedBCBAID, reportedBCBAFirstName, reportedBCBALastName,
		PatientLastName, PatientFirstName, HoursDate, HoursTimeIn, HoursTimeOut, HoursTotal,
		ProviderID, ProviderLastName, ProviderFirstName, ProviderTypeCode, LatestSignoffDate, ServiceCode, ID, HoursNotes
		) EXEC webreports.PatientHoursReportDetail @CaseID, @StartDate, @EndDate;
		
	SELECT 
		reportedBCBAID AS ProviderID,
		reportedBCBAFirstName AS ProviderFirstName,
		reportedBCBALastName AS ProviderLastName,
		SignoffDate		
	FROM @tablevar
	GROUP BY 
		reportedBCBAID,
		reportedBCBAFirstName,
		reportedBCBALastName,
		SignoffDate		

	UNION

	SELECT
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate
	FROM @tablevar
	GROUP BY
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate

	RETURN


GO







CREATE SCHEMA ahr;	-- ad-hoc reports
GO

CREATE VIEW ahr.LatestTasksAndNotesByPatient AS
	SELECT 
		b.*, 
		t.Completed AS TaskCompleted,
		t.DueDate AS TaskDueDate,
		REPLACE(REPLACE(n.Comments, CHAR(13), ''), CHAR(10), '')  AS Note,
		REPLACE(REPLACE(t.Description, CHAR(13), ''), CHAR(10), '') AS Task
	
	FROM (
		SELECT
			c.ID AS CaseID,
			p.PatientFirstName AS FirstName,
			p.PatientLastName AS LastName,
			MAX(n.EntryDate) AS EntryDate
			--REPLACE(REPLACE(n.Comments, CHAR(13), ''), CHAR(10), '')  AS NoteComment	
		FROM dbo.Patients AS p
		INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID
		LEFT JOIN dbo.CaseNotes AS n ON n.CaseID = c.ID
		--WHERE n.Comments IS NOT NULL;
		GROUP BY
			c.ID,
			p.PatientFirstName,
			p.PatientLastName
	) AS b
	INNER JOIN dbo.CaseNotes AS n ON n.CaseID = b.CaseID AND n.EntryDate = b.EntryDate
	LEFT JOIN dbo.CaseNoteTasks AS t ON n.ID = t.NoteID
	--WHERE n.Comments IS NOT NULL;





GO
EXEC meta.UpdateVersion '2.0.3.0';
GO

