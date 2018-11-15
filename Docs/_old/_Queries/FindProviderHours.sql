 /* 
SELECT * FROM dbo.Providers WHERE ProviderFirstName LIKE '%Yeh%'
SELECT c.ID FROM dbo.Patients AS p INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID WHERE PatientFirstName LIKE '%Yeh%'
-- */

--SELECT * FROM dbo.CaseAuthHours WHERE CaseProviderID = 827;

--SELECT * FROM dbo.CaseAuthCodes WHERE ID IN (934, 50);

--SELECT * FROM dbo.Patients WHERE ID = 707;

SELECT * FROM dbo.Providers WHERE ID = 827;