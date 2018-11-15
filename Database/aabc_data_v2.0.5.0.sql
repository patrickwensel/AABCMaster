/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.4.0
dym:TargetEndingVersion: 2.0.5.0
---------------------------------------------------------------------

	internal analysis queries
	index additions
	refine provider list items view
	
---------------------------------------------------------------------*/


-- ========================
-- some status on PP usage
-- ========================
CREATE PROCEDURE ahr.ProviderPortalUsageStats (@CutoffDate DATETIME2) AS BEGIN
	SELECT 
		(SELECT COUNT(*) FROM dbo.CaseAuthHours WHERE DateCreated > @CutoffDate AND HoursEntryApp = 'Provider Portal') AS PPCount,
		(SELECT COUNT(*) FROM dbo.CaseAuthHours WHERE DateCreated > @CutoffDate AND (HoursEntryApp IS NULL OR HoursEntryApp <> 'Provider Portal')) AS PPOCount,
		(SELECT COUNT(DISTINCT CaseProviderID) FROM dbo.CaseAuthHours WHERE DateCreated > @CutoffDate AND HoursEntryApp = 'Provider Portal') AS PPProvidersCount,
		(SELECT COUNT(DISTINCT CaseProviderID) FROM dbo.CaseAuthHours WHERE DateCreated > @CutoffDate AND (HoursEntryApp IS NULL OR HoursEntryApp <> 'Provider Portal')) AS PPOProvidersCount
		
	SELECT 
		* 
	FROM dbo.CaseAuthHours 
	WHERE DateCreated > @CutoffDate
	ORDER BY DateCreated DESC;

END
GO



-- ========================
-- add some indexes to hours logs and case status
-- ========================
CREATE INDEX idxCaseAuthHoursSSGNotNull ON dbo.CaseAuthHours (HoursSSGParentID) WHERE HoursSSGParentID IS NOT NULL;
CREATE INDEX idxCaseAuthHoursProviderID ON dbo.CaseAuthHours (CaseProviderID);
CREATE INDEX idxCaseAuthHoursCaseID ON dbo.CaseAuthHours (CaseID);
CREATE INDEX idxCaseAuthHoursServiceID ON dbo.CaseAuthHours (HoursServiceID);
CREATE INDEX idxCaseStatus ON dbo.Cases (CaseStatus);
GO


-- ========================
-- remove stuffed fields, active caseload ignores discharged cases
-- ========================
ALTER PROCEDURE [dbo].[GetProviderSearchViewData] AS BEGIN

	SELECT
		p.ID,
		p.DateCreated,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
		p.ProviderCity,
		p.ProviderState,
		p.ProviderZip,
		p.ProviderPrimaryEmail,
		p.ProviderPrimaryPhone,
		p.ProviderActive,
		COALESCE(caseCount.CaseCount, 0) AS ProviderActiveCaseCount
		
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	LEFT JOIN (
		SELECT 
			cp.ProviderID, 
			COUNT(*) AS CaseCount
		FROM dbo.CaseProviders AS cp
		INNER JOIN dbo.Cases AS c ON c.ID = cp.CaseID
		WHERE cp.Active = 1 AND c.CaseStatus >= 0
		GROUP BY cp.ProviderID
	) AS caseCount ON p.ID = caseCount.ProviderID
	
	WHERE p.ProviderActive = 1
	
	ORDER BY p.ProviderLastName;	
END
GO



GO
EXEC meta.UpdateVersion '2.0.5.0';
GO

