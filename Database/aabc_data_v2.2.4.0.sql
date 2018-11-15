/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.3.0
dym:TargetEndingVersion: 2.2.4.0
---------------------------------------------------------------------

	Add reports

---------------------------------------------------------------------*/



CREATE PROCEDURE [webreports].[LatestTasksAndNotesByPatient] AS

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
	RETURN
GO




CREATE PROCEDURE [webreports].[ProviderCaseloads] AS
	SELECT 
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		c.ID AS CaseID,
		pt.ID AS PatientID,
		pt.PatientFirstName,
		pt.PatientLastName,
		cp.ActiveStartDate,
		cp.ActiveEndDate

	FROM dbo.Providers AS p
	LEFT JOIN dbo.CaseProviders AS cp ON cp.ProviderID = p.ID
	LEFT JOIN dbo.Cases AS c ON c.ID = cp.CaseID
	LEFT JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
	WHERE p.ProviderStatus = 1
	ORDER BY p.ProviderLastName;

	RETURN
GO



GO
EXEC meta.UpdateVersion '2.2.4.0'
GO

