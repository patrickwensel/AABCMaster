/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.6.0
dym:TargetEndingVersion: 2.0.7.0
---------------------------------------------------------------------

	- Catalyst Preload
	
---------------------------------------------------------------------*/


-- ========================
-- Catalyst Preloading
-- ========================
CREATE TABLE [dbo].[CatalystPreloadEntries](
	[ID] INT IDENTITY(1,1) NOT NULL,
	[ResponseDate] DATETIME2(7) NOT NULL,

	[Date] DATETIME2(7) NOT NULL,
	[PatientName] NVARCHAR(128) NOT NULL,
	[ProviderName] NVARCHAR(128) NOT NULL,

	[Notes] NVARCHAR(4000) NOT NULL,
	[ProviderAgreed] BIT NOT NULL,
	[PatientAgreed] BIT NOT NULL
)

CREATE TABLE [dbo].[CatalystProviderMappings](
	[ID] INT IDENTITY(1,1) NOT NULL,
	[CatalystProviderName] NVARCHAR(128) NOT NULL,
	[ProviderID] INT NOT NULL FOREIGN KEY REFERENCES  [dbo].[Providers](ID)
)

CREATE TABLE [dbo].[CatalystPatientMappings](
	[ID] INT IDENTITY(1,1) NOT NULL,
	[CatalystPatientName] NVARCHAR(128) NOT NULL,
	[PatientID] INT NOT NULL FOREIGN KEY REFERENCES  [dbo].[Patients](ID)
)
GO


ALTER TABLE dbo.CatalystPreloadEntries ADD
	MappedProviderID INT,
	MappedCaseID INT,
	IsResolved BIT NOT NULL DEFAULT 0
GO

CREATE INDEX idxCatalystPreloadEntriesCaseID ON dbo.CatalystPreloadEntries (MappedCaseID);
CREATE INDEX idxCatalystPreloadEntriesProviderID ON dbo.CatalystPreloadEntries (MappedProviderID);
CREATE INDEX idxCatalystPreloadEntriesIsResolved ON dbo.CatalystPreloadEntries (IsResolved);
GO



GO
EXEC meta.UpdateVersion '2.0.7.0';
GO

