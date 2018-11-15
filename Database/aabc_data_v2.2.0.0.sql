/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.7.0
dym:TargetEndingVersion: 2.2.0.0
---------------------------------------------------------------------

	- Provider Statuses
	
---------------------------------------------------------------------*/

-- ============================  
-- Rename dbo.Providers.ProviderActive to dbo.Providers.ProviderStatus and update its type.
-- ============================  
DROP INDEX dbo.Providers.idxProviderActive
GO

ALTER TABLE dbo.Providers
DROP CONSTRAINT DF__Providers__Provi__7814D14C;  

ALTER TABLE dbo.Providers
ALTER COLUMN ProviderActive INT NOT NULL;
GO

EXEC sp_rename 'dbo.Providers.ProviderActive', 'ProviderStatus', 'COLUMN';  
GO  

-- ========================  
-- Drop obsolete stored procedures
-- ======================== 
DROP PROCEDURE [dbo].[GetProviderSearchViewData]
GO

-- ========================  
-- Update GetSelectableProvidersForStaffingLog proc to select Active and Potential providers
-- ======================== 
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetSelectableProvidersForStaffingLog') AND type IN ( N'P', N'PC' )) 
BEGIN
	DROP PROCEDURE [dbo].[GetSelectableProvidersForStaffingLog];
END
GO

CREATE PROCEDURE [dbo].[GetSelectableProvidersForStaffingLog] 
	@staffingLogId int
AS 
BEGIN
	SELECT
		p.ID,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
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
	WHERE p.ProviderStatus IN (1, 2) AND NOT EXISTS(
		SELECT *
		FROM dbo.StaffingLogProviders as slp
		WHERE slp.ProviderID = p.ID AND slp.StaffingLogID = @staffingLogId
	)
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO

-- ========================  
-- Update GetSelectedProvidersByStaffingLog proc
-- ======================== 
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetSelectedProvidersByStaffingLog') AND type IN ( N'P', N'PC' )) 
BEGIN
	DROP PROCEDURE [dbo].[GetSelectedProvidersByStaffingLog];
END
GO

CREATE PROCEDURE [dbo].[GetSelectedProvidersByStaffingLog]
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
		pt.ProviderTypeCode,
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
		LEFT JOIN ContactLog AS cl ON cl.StaffingLogProviderID = slp.ID AND cl.rowNumber = 1
	WHERE slp.StaffingLogID = @staffingLogID
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO





GO
EXEC meta.UpdateVersion '2.2.0.0'
GO

