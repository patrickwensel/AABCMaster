/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.8.0
dym:TargetEndingVersion: 2.3.0.0
---------------------------------------------------------------------

	Various updates, performaance improvements, etc

---------------------------------------------------------------------*/


-- =======================
-- Update so the report returns correct records
-- =======================

ALTER VIEW [hoursEntry].[AuthorizationsWithoutRules] AS
	SELECT 
		ea.CaseID,
		ea.CaseAuthorizationID,
		ea.AuthCodeID,
		ea.AuthCode
	FROM hoursEntry.EligibleAuthorizations AS ea
		INNER JOIN dbo.Cases AS c ON c.ID = ea.CaseID
		INNER JOIN dbo.CaseInsurances AS ci ON ci.CaseID = c.ID
	WHERE (ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE())
		AND (
			/* HAS NO MATCHING RULE */
			NOT EXISTS(				
				SELECT *
				FROM AuthMatchRules AS MR
				WHERE (MR.InsuranceID = ci.InsuranceID) AND
					  (MR.RuleInitialAuthID = ea.AuthCodeID OR mr.RuleFinalAuthID = ea.AuthCodeID) AND 
					  (MR.RuleEffectiveDate IS NULL OR MR.RuleEffectiveDate <= GETDATE()) AND 
					  (MR.RuleDefectiveDate IS NULL OR MR.RuleDefectiveDate >= GETDATE())
			) OR 
			/* HAS A FINAL MATCHING RULE, BUT CASE DOESNT CONTAIN AN AUTH MATCHING THE INITIAL RULE  */
			EXISTS(
				SELECT *
				FROM AuthMatchRules AS MR
				WHERE (MR.InsuranceID = ci.InsuranceID) AND
					  (MR.RuleFinalAuthID = ea.AuthCodeID) AND 
					  (MR.RuleEffectiveDate IS NULL OR MR.RuleEffectiveDate <= GETDATE()) AND 
					  (MR.RuleDefectiveDate IS NULL OR MR.RuleDefectiveDate >= GETDATE()) AND 
					  (NOT EXISTS(
							SELECT *
							FROM hoursEntry.EligibleAuthorizations AS EA2
							WHERE EA2.CaseID = EA.CaseID AND EA2.AuthCodeID = MR.RuleInitialAuthID 
							-- TODO: SHOULD WE CHECK THAT THE CASE INSURANCE DATES ARE VALID???
					  )
			)
		)
	)
	GROUP BY
		ea.CaseID,
		ea.CaseAuthorizationID,
		ea.AuthCodeID,
		ea.AuthCode
GO



-- =======================
-- Add provider subtype to list
-- =======================

ALTER PROCEDURE [dbo].[GetSelectableProvidersForStaffingLog] 
	@staffingLogId int
AS 
BEGIN
	SELECT
		p.ID,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode + IIF(pst.ProviderSubTypeName IS NOT NULL, ' / ' + pst.ProviderSubTypeCode, '') AS ProviderTypeCode,
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
		ProviderServiceCounties = dbo.GetCountiesForProvider(p.ID, NULL),
		ProviderLanguages = STUFF(
			(
				SELECT ',' + cl.Description
				FROM dbo.CommonLanguages AS cl
				INNER JOIN dbo.ProviderLanguages AS pl ON cl.ID = pl.LanguageID
				WHERE pl.ProviderID = p.ID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		p.ProviderGender,
		p.ProviderStatus
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN dbo.ProviderSubTypes AS pst ON pst.ID = p.ProviderSubTypeID
	WHERE p.ProviderStatus IN (1, 2) AND NOT EXISTS(
		SELECT *
		FROM dbo.StaffingLogProviders as slp
		WHERE slp.ProviderID = p.ID AND slp.StaffingLogID = @staffingLogId
	)
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO

-- =======================
-- Add provider subtype to list
-- =======================

ALTER PROCEDURE [dbo].[GetSelectedProvidersByStaffingLog]
	@staffingLogID int
AS 
BEGIN
	WITH ContactLog AS (
		SELECT	slpcl.StaffingLogProviderID,
				slpcl.Notes,
				slps.StatusName,
				slpcl.FollowUpDate,
				ROW_NUMBER() OVER (PARTITION BY slpcl.StaffingLogProviderID ORDER BY slpcl.ContactDate DESC) AS rowNumber
		FROM dbo.StaffingLogProviderContactLog AS slpcl
			JOIN dbo.StaffingLogProviderStatuses AS slps ON slpcl.StatusID = slps.ID
	)
	SELECT
		p.ID as ProviderID,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode + IIF(pst.ProviderSubTypeName IS NOT NULL, ' / ' + pst.ProviderSubTypeCode, '') AS ProviderTypeCode,
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
		ProviderServiceCounties = dbo.GetCountiesForProvider(p.ID, NULL),
		ProviderLanguages = STUFF(
			(
				SELECT ',' + cl.Description
				FROM dbo.CommonLanguages AS cl
				INNER JOIN dbo.ProviderLanguages AS pl ON cl.ID = pl.LanguageID
				WHERE pl.ProviderID = p.ID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		slp.ID as StaffingLogProviderID,
		CONVERT(BIT, CASE WHEN cl.StaffingLogProviderID IS NULL THEN 0 ELSE 1 END) AS HasBeenContacted,  
		cl.StatusName AS Status,  
		cl.Notes,
		cl.FollowUpDate
	FROM dbo.Providers AS p 
		INNER JOIN dbo.StaffingLogProviders AS slp ON p.ID = slp.ProviderID
		LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
		LEFT JOIN dbo.ProviderSubTypes AS pst ON pst.ID = p.ProviderSubTypeID
		LEFT JOIN ContactLog AS cl ON cl.StaffingLogProviderID = slp.ID AND cl.rowNumber = 1
	WHERE slp.StaffingLogID = @staffingLogID
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO


-- =======================
-- Include additional join fields
-- =======================
CREATE VIEW [dbo].[ProvidersDump]
AS
	SELECT
		ID
		, DateCreated
		, ProviderType
		, ProviderSubTypeID
		, ProviderFirstName
		, ProviderLastName
		, ProviderCompanyName
		, ProviderPrimaryPhone
		, ProviderPrimaryEmail
		, ProviderAddress1
		, ProviderAddress2
		, ProviderCity
		, ProviderState
		, ProviderZip
		, dbo.GetCountiesForProvider(ID, NULL) AS ProviderServiceAreas
		, ProviderNPI
		, ProviderRate
		, ProviderPhone2
		, ProviderFax
		, ProviderNotes
		, ProviderAvailability
		, ProviderHasBackgroundCheck
		, ProviderHasReferences
		, ProviderHasResume
		, ProviderCanCall
		, ProviderCanReachByPhone
		, ProviderCanEmail
		, ProviderDocumentStatus
		, ProviderLBA
		, ProviderCertificationID
		, ProviderCertificationState
		, ProviderCertificationRenewalDate
		, ProviderW9Date
		, ProviderCAQH
		, ProviderNumber
		, ProviderStatus
		, CASE [ProviderStatus]
			WHEN 0 THEN 'Inactive' 
			WHEN 1 THEN 'Active'
			WHEN 2 THEN 'Potential'
			ELSE NULL
			END AS ProviderStatusText
		, ProviderIsHired
		, ResumeFileName
		, ResumeLocation
		, PayrollID
		, ProviderGender
		, ProviderHireDate
	FROM dbo.Providers
GO



-- =======================
-- 
-- =======================
ALTER PROCEDURE [webreports].[AuthorizationUtilization] 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM (
		SELECT 
			M.PatientLastName, 
			M.PatientFirstName, 
			M.CodeCode,
			M.CodeDescription,
			M.DaysSinceAuthStart,
			M.TotalDaysInAuth,
			M.CurrentAuthProgressPercent,
			CAST(M.TotalUtilizedMin / 60 AS decimal(18,2)) AS CurrentUtilizatedHours,
			M.TotalAllowed AS TotalAllowedHours,
			CASE
				WHEN M.TotalAllowed > 0 THEN CAST(M.TotalUtilizedMin / 60 / M.TotalAllowed * 100 AS decimal(18,2))
				ELSE NULL 
			END AS UtilizationPercentage,
			CASE
				WHEN M.TotalAllowed > 0 AND M.DaysSinceAuthStart > 0 THEN CAST(M.TotalDaysInAuth * M.TotalUtilizedMin / M.DaysSinceAuthStart / 60 / M.TotalAllowed * 100 AS decimal(18,2)) 
				ELSE NULL 
			END	AS ExpectedFinalUtilization
		FROM 
		(
			SELECT 
				P.PatientLastName, 
				P.PatientFirstName, 
				AUTH.CodeCode, 
				AUTH.CodeDescription,
				CAC.AuthStartDate, 
				CAC.AuthEndDate,
				DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS DaysSinceAuthStart,
				DATEDIFF(day, CAC.AuthStartDate, CAC.AuthEndDate) AS TotalDaysInAuth,
				CAST((CAST(DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS decimal(18,2)) / DATEDIFF(day, CAC.AuthStartDate, CAC.AuthEndDate) * 100) AS decimal(18,2)) AS CurrentAuthProgressPercent,
				CAC.AuthTotalHoursApproved AS TotalAllowed,
				(
					SELECT CAST(COALESCE(SUM(Minutes),0) AS decimal(18,2))
					FROM dbo.CaseAuthHoursBreakdown AS BR
					WHERE BR.CaseAuthID = CAC.ID 
				) AS TotalUtilizedMin
			FROM dbo.Patients AS P
			INNER JOIN dbo.Cases AS C ON C.PatientID = P.ID
			INNER JOIN dbo.CaseAuthCodes AS CAC ON CAC.CaseID = C.ID
			INNER JOIN dbo.AuthCodes AS AUTH ON	AUTH.ID = CAC.AuthCodeID
			WHERE CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()
		) AS M
	) AS T
	ORDER BY ExpectedFinalUtilization DESC
END
GO


-- =======================
-- Add additional fields
-- =======================
ALTER PROCEDURE [webreports].[LatestTasksAndNotesByPatient] AS
BEGIN
	SELECT 
		c.ID as CaseID,
		p.PatientFirstName,
		p.PatientLastName,
		b.EntryDate, 
		t.Completed AS TaskCompleted,
		t.DueDate AS TaskDueDate,
		REPLACE(REPLACE(n.Comments, CHAR(13), ''), CHAR(10), '')  AS Note,
		REPLACE(REPLACE(t.Description, CHAR(13), ''), CHAR(10), '') AS Task,
		LTRIM(RTRIM((COALESCE(s.StaffLastName,'') + ' ' + COALESCE(s.StaffFirstName,'')))) AS CaseManager
	FROM dbo.Cases as C 
	INNER JOIN dbo.Patients AS p ON c.PatientID = p.ID
	INNER JOIN (
		SELECT
			cn.CaseID,
			MAX(cn.EntryDate) AS EntryDate	
		FROM dbo.CaseNotes AS cn
		GROUP BY cn.CaseID
	) AS b ON c.ID = b.CaseID
	INNER JOIN dbo.CaseNotes AS n ON n.CaseID = b.CaseID AND n.EntryDate = b.EntryDate
	LEFT JOIN dbo.CaseNoteTasks AS t ON n.ID = t.NoteID
	LEFT JOIN dbo.Staff as S ON c.CaseAssignedStaffID = s.ID
	WHERE c.CaseStatus >= 0
	RETURN
END
GO


-- =======================
-- Create function to help with performance
-- =======================
CREATE FUNCTION [dbo].[GetTypeForProvider]
(
	@providerId int,
	@delimiter nvarchar(5)
)
RETURNS nvarchar(MAX)
AS
BEGIN
	DECLARE @Result nvarchar(MAX)
	SET @delimiter = COALESCE(@delimiter,' / ')
	SELECT @Result = STUFF(
			(
			SELECT @delimiter + T.T
			FROM
			(
				SELECT TOP(1) 1 AS O, PT.ProviderTypeCode AS T
				FROM ProviderTypes AS PT
				WHERE EXISTS(
					SELECT *
					FROM Providers AS P
					WHERE P.ID = @providerId AND PT.ID = P.ProviderType
				)
				UNION
				SELECT TOP(1) 2 AS O, PST.ProviderSubTypeCode AS T
				FROM ProviderSubTypes AS PST
				WHERE EXISTS(
					SELECT *
					FROM Providers AS P
					WHERE P.ID = @providerId AND PST.ID = P.ProviderSubTypeID
				)
				ORDER BY O
			) AS T
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, LEN(@delimiter)+1, ''
		)
	RETURN @Result
END
GO


-- =======================
-- Create function to help with performance
-- =======================
CREATE FUNCTION [dbo].[GetZipCodesForProvider]
(
	@providerId int,
	@delimiter nvarchar(5)
)
RETURNS nvarchar(MAX)
AS
BEGIN
	DECLARE @Result nvarchar(MAX)
	SET @delimiter = COALESCE(@delimiter,', ')
	SELECT @Result = STUFF(
			(
			SELECT DISTINCT  @delimiter + PZC.ZipCode
			FROM            ProviderServiceZipCodes AS PZC
			WHERE        (PZC.ProviderID = @providerId)
			ORDER BY @delimiter + PZC.ZipCode
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, LEN(@delimiter), ''
		)
	RETURN @Result
END
GO


-- =======================
-- Create function to help with performance
-- =======================
ALTER FUNCTION [dbo].[GetCountiesForProvider]
(
	@providerId int,
	@delimiter nvarchar(5)
)
RETURNS nvarchar(MAX)
AS
BEGIN
	DECLARE @Result nvarchar(MAX)
	SET @delimiter = COALESCE(@delimiter,', ')
	SELECT @Result = STUFF(
			(
			SELECT DISTINCT  @delimiter + ZipCodes.ZipCounty
			FROM            ZipCodes INNER JOIN
									 ProviderServiceZipCodes ON ZipCodes.ZipCode = ProviderServiceZipCodes.ZipCode
			WHERE        (ProviderServiceZipCodes.ProviderID = @providerId)
			ORDER BY @delimiter + ZipCodes.ZipCounty
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, LEN(@delimiter), ''
		)
	RETURN @Result
END
GO


-- =======================
-- 
-- =======================
CREATE PROCEDURE [dbo].[GetProvidersSearch]
	@providerId int = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		P.ID,
		P.ProviderStatus AS Status,
		P.ProviderLastName AS LastName,
		P.ProviderFirstName AS FirstName,
		dbo.GetTypeForProvider(P.ID, NULL) AS TypeCode,
		P.ProviderCity AS City,
		P.ProviderState AS State,
		P.ProviderZip AS Zip,
		P.ProviderPrimaryPhone AS Phone,
		P.ProviderPrimaryEmail AS Email,
		(
			SELECT COUNT(*)
			FROM CaseProviders AS CP
			WHERE CP.ProviderID = P.ID AND EXISTS(
				SELECT *
				FROM Cases AS C
				WHERE C.ID = CP.CaseID AND C.CaseStatus != -1
			)
		) AS ActiveCaseCount,
		dbo.GetZipCodesForProvider(P.ID, NULL) AS ZipCodes,
		dbo.GetCountiesForProvider(P.ID, NULL) AS Counties
	FROM Providers AS P
	WHERE P.ID = COALESCE(@providerID, p.ID)
	ORDER BY P.ProviderLastName, P.ProviderFirstName
END
GO


-- =======================
-- 
-- =======================
CREATE PROCEDURE [dbo].[GetPatientSearch]
(
	@patientId int= NULL
)
AS 
BEGIN
	DECLARE @ABATypeID int
	SET @ABATypeID = 17
	SELECT
		c.ID,
		p.ID AS PatientID,
		p.PatientFirstName AS FirstName,
		p.PatientLastName AS LastName,
		p.PatientCity AS City,
		p.PatientState AS State,
		p.PatientZip AS Zip,
		z.ZipCounty AS County,
		c.CaseStatus AS Status,
		c.CaseStatusReason AS StatusReason,
		auth.LatestStartDate AS StartDate,
		auth.LatestEndDate AS EndingAuthDate,
		c.CaseHasPrescription AS HasPrescription,
		c.CaseHasAssessment AS HasAssessment,
		c.CaseHasIntake AS HasIntake,
		CAST(CASE WHEN sup.CaseID IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS HasSupervisor,
		sup.ProviderID AS PrimaryBCBAID,
		sup.ProviderFirstName AS BCBAFirstName,
		sup.ProviderLastName AS BCBALastName,
		aide.ProviderID AS PrimaryAideID,
		aide.ProviderFirstName AS AideFirstName,
		aide.ProviderLastName AS AideLastName,
		i.InsuranceName AS InsuranceCompanyName,
		ilc.CarrierName,
		s.ID AS AssignedStaffID,
		s.StaffFirstName AS AssignedStaffFirstName,
		s.StaffLastName AS AssignedStaffLastName,
		c.CaseNeedsStaffing AS NeedsStaffing,
		c.CaseNeedsRestaffing AS NeedsRestaffing,
		c.CaseRestaffingReason AS RestaffingReason,
		c.CaseRestaffReasonID AS RestaffingReasonID	
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON c.ID = pci.CaseID
	LEFT JOIN dbo.InsuranceLocalCarriers AS ilc ON pci.CaseInsuranceCarrierID = ilc.ID
	LEFT JOIN dbo.Insurances AS i ON i.ID = pci.InsuranceID
	LEFT JOIN dbo.ZipCodes AS z ON z.ZipCode = p.PatientZip
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
	WHERE p.ID = COALESCE(@patientID, p.ID)
	ORDER BY LastName, FirstName
	RETURN
END
GO


-- =======================
-- 
-- =======================
ALTER PROCEDURE [dbo].[GetDischargedPatientSearchViewData](@ABATypeID INT) AS BEGIN
	/* THIS STORED PROCEDURE SHOULD NOT LONGER BE USED, IT HAS BEEN REPLACED BY GetPatientSearch */
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
		z.ZipCounty AS County,
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
	LEFT JOIN dbo.ZipCodes AS z ON z.ZipCode = p.PatientZip
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


-- =======================
-- 
-- =======================
ALTER PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
	/* THIS STORED PROCEDURE SHOULD NOT LONGER BE USED, IT HAS BEEN REPLACED BY GetPatientSearch */
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
		z.ZipCounty AS County,
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
		ilc.CarrierName,
		s.ID AS AssignedStaffID,
		s.StaffFirstName AS AssignedStaffFirstName,
		s.StaffLastName AS AssignedStaffLastName,
		c.CaseNeedsStaffing AS NeedsStaffing,
		c.CaseNeedsRestaffing AS NeedsRestaffing,
		c.CaseRestaffingReason AS RestaffingReason,
		c.CaseRestaffReasonID AS RestaffingReasonID
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON c.ID = pci.CaseID
	LEFT JOIN dbo.InsuranceLocalCarriers AS ilc ON pci.CaseInsuranceCarrierID = ilc.ID
	LEFT JOIN dbo.Insurances AS i ON i.ID = pci.InsuranceID
	LEFT JOIN dbo.ZipCodes AS z ON z.ZipCode = p.PatientZip
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

-- =======================
-- Add +45 day window to results
-- =======================
ALTER PROCEDURE [dbo].[GetAuthMatrixSource] 
	@caseId int,
	@ignoreBCBAAuths bit = 0,
	@numberOfMonthsRecentlyTerminated int = 0,
	@numberOfDaysUpcomingAuthWindow int = 0,
	@maxMonthSpread int = 12
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @start datetime
	DECLARE @end datetime
	DECLARE @months int
	DECLARE @sqltable NVARCHAR(MAX)
	DECLARE @sqlpivot NVARCHAR(MAX)
	DECLARE @cols1 NVARCHAR(MAX)
	DECLARE @cols2 NVARCHAR(MAX)
	DECLARE @cols3 NVARCHAR(MAX)

	-- Calculation of @start and @end
	SELECT @start = COALESCE(MIN(AuthStartDate),GETDATE()) FROM dbo.CaseAuthCodes AS CAC
	WHERE CaseID = @caseId AND CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()

	SELECT @end = COALESCE(MAX(AuthEndDate),GETDATE()) FROM dbo.CaseAuthCodes AS CAC
	WHERE CaseID = @caseId AND CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)

	SET @start = DATEFROMPARTS(YEAR(@start),MONTH(@start),1)
	SET @end = DATEADD(day,-1,DATEADD(month,1,DATEFROMPARTS(YEAR(@end),MONTH(@end),1)))

	SELECT @months = DATEDIFF(month, @start, @end)

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)

	IF (@months > @maxMonthSpread)
		SET @start = DATEADD(day,1,DATEADD(month,@maxMonthSpread*-1,@end))

	SELECT @months = DATEDIFF(month, @start, @end)

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)

	-- Building column lists
	SET @cols1 = N''
	SET @cols2 = N''
	SET @cols3 = N''

	SELECT @cols1 += N', ' + QUOTENAME(x.Month) + ' decimal(18,2) NULL'
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	SELECT @cols2 += N', COALESCE(p.' + QUOTENAME(x.Month) + ',0) ' + QUOTENAME(x.Month)
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	SELECT @cols3 += N', ' + QUOTENAME(x.Month)
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	PRINT 'COL1: ' + @cols1
	PRINT 'COL2: ' + @cols2
	PRINT 'COL3: ' + @cols3
	PRINT CHAR(13)+CHAR(10)

	SET @sqltable = 'ALTER TABLE #T ADD ' + STUFF(@cols1, 1, 2, '')
	PRINT @sqltable

	-- creating temp table
	IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T
	CREATE TABLE #T ( CaseAuthID int )
	EXEC sys.sp_executesql @sqltable

	-- filling temp table with pivotted data
	SET @sqlpivot = N'
	INSERT INTO #T
	SELECT CaseAuthID, ' + STUFF(@cols2, 1, 2, '') + '
	FROM
	(
	  SELECT 
			HB.CaseAuthID, 
			DATEFROMPARTS(YEAR(H.HoursDate),MONTH(H.HoursDate), 1) AS [Month], 
			CAST(HB.Minutes AS decimal(18,2)) / 60 AS Hours
		FROM dbo.CaseAuthHoursBreakdown AS HB
		INNER JOIN dbo.CaseAuthHours AS H ON HB.HoursID = H.ID
		WHERE H.HoursDate >= @start AND H.HoursDate <= @end AND H.CaseId = @CaseId
	) AS j
	PIVOT
	(
	  SUM(Hours) FOR Month IN ('
	  + STUFF(@cols3, 1, 1, '')
	  + ')
	) AS p'
	PRINT @sqlpivot
	EXEC sp_executesql @sqlpivot , N'@start datetime, @end datetime, @CaseId int', @start, @end, @CaseId

	-- outputting joined data
	SELECT
		A.AuthID, 
		(
			SELECT TOP (1) I.InsuranceName
			FROM Insurances AS I INNER JOIN CaseInsurances AS CI ON CI.InsuranceID = I.ID AND CI.CaseID = A.CaseID
			WHERE COALESCE(CI.DatePlanEffective,A.AuthEndDate) <= A.AuthEndDate AND COALESCE(CI.DatePlanTerminated,A.AuthStartDate) >= A.AuthStartDate
			ORDER BY CI.DatePlanTerminated, CI.DatePlanEffective DESC
		) AS InsuranceName,
		A.AuthType,
		A.AuthCode, 
		A.AuthDescription, 
		A.AuthStartDate, 
		A.AuthEndDate, 
		(
			SELECT TOP(1) P.ProviderFirstName + ' ' + P.ProviderLastName
			FROM Providers AS P INNER JOIN CaseProviders AS CP ON CP.ProviderID = P.ID AND CP.CaseID = A.CaseID
			WHERE CP.Active = 1 AND CP.IsInsuranceAuthorizedBCBA = 1 AND COALESCE(CP.ActiveStartDate,A.AuthStartDate) <= A.AuthStartDate AND COALESCE(CP.ActiveEndDate,A.AuthEndDate) >= A.AuthEndDate
			ORDER BY CP.ActiveEndDate, CP.ActiveStartDate DESC
		) AS AuthBCBA,
		A.AuthTotalHoursApproved, 
		A.AuthTotalHoursUtilized,
		CAST(A.AuthTotalHoursApproved - A.AuthTotalHoursUtilized AS decimal(18,2)) AS AuthTotalHoursRemaining,
		A.AuthTotalDays,
		A.AuthUtilizedDays,
		A.AuthRemainingDays,
		CAST(A.AuthTotalHoursApproved / (A.AuthTotalDays / 7) AS decimal(18,2)) AS ExpectedAvgHoursWeek,
		CASE 
			WHEN A.AuthUtilizedDays = 0 THEN NULL
			ELSE CAST(A.AuthTotalHoursUtilized / (A.AuthUtilizedDays / 7) AS decimal(18,2))
		END AS ActualAvgHoursWeek,
		CASE 
			WHEN A.AuthRemainingDays = 0 THEN NULL
			ELSE CAST(CAST(A.AuthTotalHoursApproved - A.AuthTotalHoursUtilized AS decimal(18,2)) / (A.AuthRemainingDays / 7) AS decimal(18,2))
		END AS AvgRemainingHoursWeek,
		B.*
	FROM (
		SELECT        
		CAC.ID AS AuthID, 
		CAC.CaseID, 
		CACL.AuthClassCode AS AuthType,
		AC.CodeCode AS AuthCode, 
		AC.CodeDescription AS AuthDescription, 
		CAST(CAC.AuthStartDate AS date) AS AuthStartDate, 
		CAST(CAC.AuthEndDate AS date) AS AuthEndDate, 
		CAC.AuthTotalHoursApproved, 
		(
			SELECT CAST(CAST(COALESCE(SUM(CAHB.Minutes),0) AS decimal(18,2)) / 60 AS decimal(18,2))
			FROM CaseAuthHoursBreakdown AS CAHB
			WHERE CAHB.CaseAuthID = CAC.ID
		) AS AuthTotalHoursUtilized,
		CAST(DATEDIFF(day, CAC.AuthStartDate,CAC.AuthEndDate) AS decimal(18,2)) AS AuthTotalDays,
		CASE 
			WHEN GETDATE() < CAC.AuthStartDate THEN 0
			WHEN GETDATE() > CAC.AuthEndDate THEN CAST(DATEDIFF(day, CAC.AuthStartDate,CAC.AuthEndDate) AS decimal(18,2))
			ELSE CAST(DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS decimal(18,2))
		END AS AuthUtilizedDays,
		CASE 
			WHEN GETDATE() > CAC.AuthEndDate THEN 0
			ELSE CAST(DATEDIFF(day, GETDATE(), CAC.AuthEndDate) AS decimal(18,2))
		END AS AuthRemainingDays
		FROM CaseAuthCodes AS CAC
		INNER JOIN AuthCodes AS AC ON AC.ID = CAC.AuthCodeID
		INNER JOIN CaseAuthClasses AS CACL ON CAC.AuthClassID = CACL.ID
		WHERE CAC.CaseID = @CaseId AND CAC.AuthStartDate <= DATEADD(DAY, ABS(@numberOfDaysUpcomingAuthWindow), GETDATE()) AND CAC.AuthEndDate >= DATEADD(MONTH, ABS(@numberOfMonthsRecentlyTerminated)*-1, GETDATE())
	) AS A
	LEFT JOIN #T AS B ON A.AuthID = B.CaseAuthID
	WHERE A.AuthType =
	CASE
		WHEN @ignoreBCBAAuths > 0 THEN 'GENERAL'
		ELSE A.AuthType
	END
	ORDER BY A.AuthStartDate, A.AuthCode

	-- deleting temp table
	IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T
END
GO




GO
EXEC meta.UpdateVersion '2.3.0.0'
GO

