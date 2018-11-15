/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.3.0.0
dym:TargetEndingVersion: 2.3.1.0
---------------------------------------------------------------------

	Staffing log improvements, timeline

---------------------------------------------------------------------*/



-- =======================
-- Add new staffing log summary to pull from fully-realized records
-- =======================
CREATE PROCEDURE dbo.GetStaffingLogSummary
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		SL.ID,
		P.ID AS PatientID,
		P.PatientFirstName AS FirstName,
		P.PatientLastName AS LastName,
		P.PatientAddress1 AS Address,
		P.PatientCity AS City,
		p.PatientState AS State,
		P.PatientZip AS Zip,
		(
			SELECT ZipCounty
			FROM ZipCodes
			WHERE ZipCode = P.PatientZip
		) AS County,
		P.PatientDateOfBirth AS DateOfBirth,
		(
			SELECT RTRIM(LTRIM(COALESCE(S.StaffFirstName,'') + ' ' + COALESCE(S.StaffLastName,'')))
			FROM Staff AS S
			WHERE S.ID = C.CaseAssignedStaffID
		) AS CaseManager,
		SL.DateWentToRestaff
	FROM StaffingLog AS SL 
		INNER JOIN Cases AS C ON C.ID = SL.ID
		INNER JOIN Patients AS P ON P.ID = C.PatientID
	ORDER BY P.PatientLastName, P.PatientFirstName
END
GO



-- =======================
-- Move prior calculated staffing log rows into staffing log table
-- =======================
INSERT INTO StaffingLog (ID,DateWentToRestaff)
SELECT C.ID, GETDATE()
FROM Cases AS C
WHERE (
	(NOT EXISTS(SELECT * FROM StaffingLog AS SL WHERE SL.ID = C.ID))) AND
	(
		(C.CaseStatus <> -1 AND C.CaseNeedsRestaffing = 1 AND (C.CaseRestaffReasonID = 14 OR C.CaseRestaffReasonID = 15)) OR
		(NOT EXISTS(
			SELECT *
			FROM Providers AS P INNER JOIN CaseProviders AS CP ON P.ID = CP.ProviderID
			WHERE P.ProviderType = 17 AND CP.Active = 1 AND COALESCE(CP.ActiveStartDate, GETDATE()) <= GETDATE() AND COALESCE(CP.ActiveEndDate, GETDATE()) >= GETDATE()
		)
	  )
	)

GO






-- =======================
-- Create a dataset for viewing case timeline
-- =======================
CREATE PROCEDURE [dbo].[GetTimelineEntries] 
	@caseID int,
	@includeNotes bit = 1
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM (
		SELECT
			'StaffingLogParentContactLog' AS Source,
			ID,
			CreatedByUserID AS AuthorId,
			(
				SELECT WU.WebUserFirstName +' ' + WU.WebUserLastName 
				FROM WebUsers AS WU
				WHERE WU.ID = SLParent.CreatedByUserID
				--SELECT S.StaffFirstName + ' ' + S.StaffLastName
				--FROM Staff AS S
				--WHERE EXISTS(
				--	SELECT * FROM WebUsers AS WU
				--	WHERE WU.ID = SLParent.CreatedByUserID AND WU.StaffID = S.ID
				--)
			) AS Author,
			ContactDate AS Date,
			ContactedPersonName + ' (' + 
			(
				SELECT RelationshipName 
				FROM GuardianRelationships 
				WHERE ID = SLParent.GuardianRelationshipID
			) + ')' + ' contacted by ' + CASE ContactMethodType WHEN 0 THEN 'phone' WHEN 1 THEN 'email' END + ' (' + ContactMethodValue + ')' AS Title,
			Notes,
			(
				SELECT 
					ContactedPersonName,
					ContactMethodType,
					ContactMethodValue
				FROM StaffingLogParentContactLog AS C
				WHERE C.ID = SLParent.ID
				FOR XML PATH ('data')
			) AS Data
		FROM StaffingLogParentContactLog AS SLParent
		WHERE StaffingLogID = @caseID
		UNION ALL
		SELECT
			'StaffingLogProviderContactLog' AS Source,
			ID,
			CreatedByUserID AS AuthorId,
			(
				SELECT WU.WebUserFirstName +' ' + WU.WebUserLastName 
				FROM WebUsers AS WU
				WHERE WU.ID = SLProvider.CreatedByUserID
				--SELECT S.StaffFirstName + ' ' + S.StaffLastName
				--FROM Staff AS S
				--WHERE EXISTS(
				--	SELECT * FROM WebUsers AS WU
				--	WHERE WU.ID = SLProvider.CreatedByUserID AND WU.StaffID = S.ID
				--)
			) AS Author,
			ContactDate AS Date,
			(
				SELECT ProviderFirstName + ' ' + ProviderLastName
				FROM Providers AS P
				WHERE EXISTS (
					SELECT *
					FROM StaffingLogProviders AS SLP
					WHERE SLP.ProviderID = P.ID AND SLP.ID = SLProvider.StaffingLogProviderID
				)
			) AS Title,
			Notes,
			(
					SELECT 
					(
						SELECT SLP.ProviderID
						FROM StaffingLogProviders AS SLP
						WHERE SLP.ID = C.StaffingLogProviderID
					) AS ProviderId,
					(
						SELECT ProviderFirstName + ' ' + ProviderLastName
						FROM Providers AS P
						WHERE EXISTS (
							SELECT *
							FROM StaffingLogProviders AS SLP
							WHERE SLP.ProviderID = P.ID AND SLP.ID = C.StaffingLogProviderID
						)
					) AS ProviderName,
					FollowUpDate
				FROM StaffingLogProviderContactLog AS C
				WHERE C.ID = SLProvider.ID
				FOR XML PATH ('data')
			) AS Data
		FROM StaffingLogProviderContactLog AS SLProvider
		WHERE EXISTS (
			SELECT *
			FROM StaffingLogProviders AS SLP
			WHERE SLP.ID = SLProvider.StaffingLogProviderID AND SLP.StaffingLogID = @caseID
		)
		UNION ALL
		SELECT 
			'CaseNotes' AS Source,
			ID,
			EnteredByUserID AS AuthorId,
			(
				SELECT WU.WebUserFirstName +' ' + WU.WebUserLastName 
				FROM WebUsers AS WU
				WHERE WU.ID = CN.EnteredByUserID
				--SELECT S.StaffFirstName + ' ' + S.StaffLastName
				--FROM Staff AS S
				--WHERE EXISTS(
				--	SELECT * FROM WebUsers AS WU
				--	WHERE WU.ID = CN.EnteredByUserID AND WU.StaffID = S.ID
				--)
			) AS Author,
			EntryDate AS Date,
			CorrespondenceName + ' ' + CorrespondenceName AS Title,
			Comments AS Notes,
			(
				SELECT
					CorrespondenceName,
					CorrespondenceType,
					RequiresFollowup,
					FollowUpDate,
					FollowupComplete,
					FollowupCompleteDate,
					FollowupComment
				FROM CaseNotes AS C
				WHERE C.ID = CN.ID
				FOR XML PATH ('data')
			) AS Data
		FROM CaseNotes AS CN
		WHERE CaseID = @caseID AND @includeNotes = 1
	) AS U
	ORDER BY U.Date DESC, U.Source
END


GO
EXEC meta.UpdateVersion '2.3.1.0'
GO

