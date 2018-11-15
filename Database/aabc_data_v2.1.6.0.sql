/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.5.0
dym:TargetEndingVersion: 2.1.6.0
---------------------------------------------------------------------

	- Provider app login restrictions and signatures
	- Staffing special needs, gender
	
---------------------------------------------------------------------*/


-- =========================
-- Add special needs handling for staffing
-- =========================

-- Create Special Attention Needs table
CREATE TABLE dbo.SpecialAttentionNeeds (
	ID INT NOT NULL,
	Code NVARCHAR(5) NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	Active BIT NOT NULL,
	CONSTRAINT PK_SpecialAttentionNeeds PRIMARY KEY (ID)
);
GO

INSERT INTO dbo.SpecialAttentionNeeds (ID, Code, [Name], Active)
VALUES	(1, 'CIS', 'Crisis Intervention Support', 1),
		(2, 'RA', 'Remote Area', 1),
		(3, 'LC', 'Language or Cultural Specialty', 1);
GO

CREATE TABLE dbo.StaffingLogSpecialAttentionNeeds (
	StaffingLogID INT NOT NULL,
	SpecialAttentionNeedID INT NOT NULL,
	CONSTRAINT PK_StaffingLogSpecialAttentionNeeds PRIMARY KEY (StaffingLogID, SpecialAttentionNeedID),
	CONSTRAINT [FK_StaffingLogSpecialAttentionNeeds_SpecialAttentionNeeds] 
		FOREIGN KEY (SpecialAttentionNeedID) REFERENCES dbo.SpecialAttentionNeeds (ID),
	CONSTRAINT [FK_StaffingLogSpecialAttentionNeeds_StaffingLog] 
		FOREIGN KEY (StaffingLogID) REFERENCES dbo.StaffingLog (ID) ON DELETE CASCADE
);
GO

-- =========================
-- Add provider gender support
-- =========================
ALTER TABLE dbo.Providers
ADD ProviderGender CHAR(1) NULL;
GO

ALTER TABLE dbo.StaffingLog
ADD ProviderGenderPreference CHAR(1) NULL;
GO

ALTER PROCEDURE [dbo].[GetSelectableProvidersForStaffingLog] 
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
		p.ProviderGender
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	WHERE p.ProviderActive = 1 AND NOT EXISTS(
		SELECT *
		FROM dbo.StaffingLogProviders as slp
		WHERE slp.ProviderID = p.ID AND slp.StaffingLogID = @staffingLogId
	)
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO


-- =========================
-- Allow provider app login toggle
-- =========================
ALTER TABLE dbo.ProviderPortalUsers ADD ProviderHasAppAccess BIT NOT NULL DEFAULT 0;
GO

-- =========================
-- Allow storage of signatures for hours
-- =========================
ALTER TABLE dbo.CaseAuthHours ADD HoursParentSignature NVARCHAR(MAX);
ALTER TABLE dbo.CaseAuthHours ADD HoursProviderSignature NVARCHAR(MAX);
GO




GO
EXEC meta.UpdateVersion '2.1.6.0'
GO

