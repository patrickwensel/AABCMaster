
SELECT
	p.ProviderFirstName + ' '  + p.ProviderLastName AS ProviderName,
	p.ProviderPrimaryEmail,
	pt.PatientFirstName + ' '  + pt.PatientLastName AS PatientName,
	c.ID AS CaseID,
	CASE WHEN c.CaseStatus = 0 THEN
		'NotReady'
	ELSE
		CASE WHEN c.CaseStatus = 1 THEN
			'Good'
		ELSE
			'CFD'
		END
	END AS TextStatus,
	-- c.CaseStatus,
	COUNT(h.ID) AS CountOfHours,
	COUNT(cc.ID) AS CountOfAuthsOnFile

FROM dbo.CaseProviders AS cp 
INNER JOIN dbo.Cases AS c ON c.ID = cp.CaseID
INNER JOIN dbo.Patients AS pt ON c.PatientID = pt.ID
INNER JOIN dbo.Providers AS p ON cp.ProviderID = p.ID
LEFT JOIN dbo.CaseAuthCodes AS cc ON cc.CaseID = c.ID
LEFT JOIN (

	SELECT cah.*, CASE WHEN cah.CaseID IS NULL THEN cac.CaseID ELSE cah.CaseID END AS CalcdCaseID
	FROM dbo.CaseAuthHours AS cah
	LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.AuthCodeID = cah.CaseAuthID

) AS h ON h.CaseID = c.ID

WHERE ((h.HoursDate >= '2016-06-01' AND h.HoursDate < '2016-07-01')
		OR h.ID IS NULL)
	AND c.CaseStatus != -1


GROUP BY 
	p.ProviderFirstName + ' '  + p.ProviderLastName,
	p.ProviderPrimaryEmail,
	pt.PatientFirstName + ' '  + pt.PatientLastName,
	c.ID,
	CASE WHEN c.CaseStatus = 0 THEN
		'NotReady'
	ELSE
		CASE WHEN c.CaseStatus = 1 THEN
			'Good'
		ELSE
			'CFD'
		END
	END
	-- c.CaseStatus,

HAVING COUNT(h.ID) = 0

ORDER BY p.ProviderFirstName + ' '  + p.ProviderLastName