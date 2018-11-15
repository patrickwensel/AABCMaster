
DECLARE @StartDate DATETIME2
DECLARE @EndDate DATETIME2

SET @StartDate = '2016-06-01' -- uses >= comparison
SET @EndDate = '2016-07-01'	-- uses < comparison

SELECT a.*,
	CASE WHEN b.ProviderID IS NULL THEN 'N' ELSE 'Y' END AS HasFinalization

FROM (
	SELECT
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryEmail,
		COUNT(h.ID) AS HoursCount
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON p.ID = h.CaseProviderID

	WHERE h.HoursStatus < 2 -- not finalized
		AND h.HoursDate >= @StartDate
		AND h.HoursDate < @EndDate

	GROUP BY 
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryEmail
) AS a 

	LEFT JOIN (

	SELECT
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryEmail,
		COUNT(h.ID) AS HoursCount
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON p.ID = h.CaseProviderID

	WHERE h.HoursStatus = 2	-- finalized hours
		AND h.HoursDate >= @StartDate
		AND h.HoursDate < @EndDate

	GROUP BY 
		p.ID,
		p.ProviderFirstName,
		p.ProviderLastName,
		p.ProviderPrimaryEmail

) AS b ON a.ProviderID = b.ProviderID