/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.1.0
dym:TargetEndingVersion: 2.1.2.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/






-- =========================
-- Seed existing insurances with defaults
-- =========================
DELETE FROM dbo.InsuranceServices;
-- insert BCBA Services
INSERT INTO dbo.InsuranceServices (ServiceID, InsuranceID, ProviderTypeID)
	SELECT s.ID, i.ID, 15
	FROM dbo.Insurances AS i 
	CROSS JOIN dbo.Services AS s
	WHERE s.ID IN (11, 9, 13, 10, 12, 16, 17);
-- insert AIDE services
INSERT INTO dbo.InsuranceServices (ServiceID, InsuranceID, ProviderTypeID)
	SELECT s.ID, i.ID, 17
	FROM dbo.Insurances AS i 
	CROSS JOIN dbo.Services AS s
	WHERE s.ID IN (9, 15, 14);
-- */




-- =========================
-- Seed existing insurances with defaults
-- =========================
CREATE TABLE dbo.InsuranceServiceDefaults (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	ServiceID INT NOT NULL,
	ProviderTypeID INT NOT NULL
);
CREATE UNIQUE INDEX idxInsuranceServiceDefaultsServiceProviderType ON dbo.InsuranceServiceDefaults (ServiceID, ProviderTypeID);
GO

-- insert BCBA Services
INSERT INTO dbo.InsuranceServiceDefaults (ServiceID, ProviderTypeID)
	SELECT s.ID, 15
	FROM dbo.Services AS s
	WHERE s.ID IN (11, 9, 13, 10, 12, 16, 17);
-- insert AIDE services
INSERT INTO dbo.InsuranceServiceDefaults (ServiceID, ProviderTypeID)
	SELECT s.ID, 17
	FROM dbo.Services AS s
	WHERE s.ID IN (9, 15, 14);
GO



GO
EXEC meta.UpdateVersion '2.1.2.0'
GO

