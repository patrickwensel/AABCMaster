/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.0.0
dym:TargetEndingVersion: 2.1.1.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/






-- ========================
-- Create a view for duplicate hours entry analysis
-- ========================
CREATE VIEW ahr.DupeHoursEntryAnalysis AS 
	SELECT
		h.HoursStatus AS Status,
		d.HoursID,
		d.CaseID,
		d.ProviderID,
		p.ProviderType,
		LEFT(p.ProviderFirstName + ' ' + p.ProviderLastName, 35) AS ProviderName,
		h.HoursServiceID AS sv1,
		s.ServiceCode,
		h.ServiceLocationID AS Loc,
		d.DateCreated,
		d.Interval,
		COALESCE(n.CountOfNotes, 0) AS CountOfNotes,
		IIF(h.HoursNotes IS NOT NULL, 'Has Single Entry', NULL) AS HoursNotes
	FROM (
		SELECT 
			h1.ID AS HoursID, 
			h1.CaseProviderID AS ProviderID, 
			h1.CaseID,
			h1.DateCreated, 
			ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated)) AS Interval
		FROM (SELECT * FROM dbo.CaseAuthHours WHERE HoursDate >= '2018-01-01') AS h1
		CROSS JOIN (SELECT * FROM dbo.CaseAuthHours WHERE HoursDate >= '2018-01-01') AS h2
		WHERE h1.CaseProviderID = h2.CaseProviderID
			AND h1.CaseID = h2.CaseID
			AND h1.ID <> h2.ID
			AND ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated)) < 2
		GROUP BY 
			h1.ID, 
			h1.CaseProviderID, 
			h1.CaseID,
			h1.DateCreated, 
			ABS(DATEDIFF(ss, h1.DateCreated, h2.DateCreated))
	) AS d
	INNER JOIN dbo.Providers AS p ON d.ProviderID = p.ID
	INNER JOIN dbo.Cases AS c ON c.ID = d.CaseID
	INNER JOIN dbo.CaseAuthHours AS h ON h.ID = d.HoursID
	INNER JOIN dbo.Services AS s ON s.ID = h.HoursServiceID
	LEFT JOIN (	
		SELECT notes.HoursID, COUNT(*) AS CountOfNotes
		FROM dbo.CaseAuthHoursNotes AS notes
		WHERE notes.NotesAnswer IS NOT NULL
		GROUP BY notes.HoursID	
	) AS n ON n.HoursID = h.ID

	WHERE h.DateCreated >= '2018-01-01'

	-- ORDER BY d.HoursID

GO





-- ========================
-- Definalize Provider
-- ========================
CREATE PROCEDURE dbo.DefinalizeProvider (@ProviderID INT, @CaseID INT, @FirstDayOfPeriod DATE) AS 

	 /* TEST DATA
		DECLARE @ProviderID INT = 257;
		DECLARE @CaseID INT = 522;
		DECLARE @FirstDayOfPeriod DATE = '2018-02-01'
	-- */

	DECLARE @FirstDayOfNextPeriod DATETIME = DATEADD(MONTH, 1, @FirstDayOfPeriod)

	-- get list of periods
	-- SELECT p.CaseID, p.PeriodFirstDayOfMonth, f.*
	DELETE f
	FROM dbo.Cases AS c
	INNER JOIN dbo.CaseMonthlyPeriods AS p ON c.ID = p.CaseID
	INNER JOIN dbo.CaseMonthlyPeriodProviderFinalizations AS f ON p.ID = f.CaseMonthlyPeriodID

	WHERE p.PeriodFirstDayOfMonth = @FirstDayOfPeriod
		AND c.ID = @CaseID
		AND f.ProviderID = @ProviderID;


	-- SELECT h.*
	UPDATE h SET h.HoursStatus = 1
	FROM dbo.CaseAuthHours AS h
	WHERE h.HoursDate >= @FirstDayOfPeriod
		AND h.HoursDate < @FirstDayOfNextPeriod
		AND h.HoursStatus >= 2 -- finalize
		AND h.CaseProviderID = @ProviderID
		AND h.CaseID = @CaseID


GO




-- ========================
-- Add PayrollID tracking to Providers
-- ========================
ALTER TABLE dbo.Providers ADD PayrollID INT;
GO



ALTER PROCEDURE [webreports].[PayablesByPeriod](@FirstDayOfMonth DATETIME2) AS

	 /*	TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2
	SET @FirstDayOfMonth = '2016-05-01'
	-- */
	
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)
	
	SELECT
		p.ID,
		COALESCE(p.PayrollID, p.ID) AS PayrollID,
		p.ProviderFirstName,
		p.ProviderLastName,
		SUM(h.HoursPayable) AS TotalPayable
	FROM dbo.Providers AS p
	INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
	WHERE h.HoursStatus = 3 -- finalized hours only
		AND h.HoursDate >= @StartDate
		AND h.HoursDate <= @EndDate
		AND h.HoursPayableRef IS NULL
	GROUP BY 
		p.ID,
		COALESCE(p.PayrollID, p.ID),
		p.ProviderFirstName,
		p.ProviderLastName
	ORDER BY p.ProviderLastName
	
	
	RETURN

GO








-- ========================
-- Add Staffing Log Support
-- ========================
CREATE TABLE [dbo].[StaffingLog] (
    [ID] [int] NOT NULL,
    [ParentalRestaffRequest] [nvarchar](2000),
    [HoursOfABATherapy] [decimal](5,2),
    [AidesRespondingNo] [nvarchar](2000),
    [AidesRespondingMaybe] [nvarchar](2000),
	[ScheduleRequest] [int] NOT NULL DEFAULT 0,
	[DateWentToRestaff] [date],
    CONSTRAINT [PK_dbo.StaffingLog] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_ID] ON [dbo].[StaffingLog]([ID])
ALTER TABLE [dbo].[StaffingLog] ADD CONSTRAINT [FK_dbo.StaffingLog_dbo.Cases_ID] FOREIGN KEY ([ID]) REFERENCES [dbo].[Cases] ([ID])
GO


CREATE TABLE [dbo].[StaffingLogProviders] (
    [ID] [int] NOT NULL IDENTITY,
    [StaffingLogID] [int] NOT NULL,
    [ProviderID] [int] NOT NULL,
    [HasBeenContacted] [bit] NOT NULL,
    [Response] [int],
    [Notes] [nvarchar](2000),
	[FollowUpDate] [date],
    CONSTRAINT [PK_dbo.StaffingLogProviders] PRIMARY KEY ([ID], [StaffingLogID], [ProviderID])
)
CREATE INDEX [IX_StaffingLogID] ON [dbo].[StaffingLogProviders]([StaffingLogID])
CREATE INDEX [IX_ProviderID] ON [dbo].[StaffingLogProviders]([ProviderID])
ALTER TABLE [dbo].[StaffingLogProviders] ADD CONSTRAINT [FK_dbo.StaffingLogProviders_dbo.Providers_ProviderID] FOREIGN KEY ([ProviderID]) REFERENCES [dbo].[Providers] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[StaffingLogProviders] ADD CONSTRAINT [FK_dbo.StaffingLogProviders_dbo.StaffingLog_StaffingLogID] FOREIGN KEY ([StaffingLogID]) REFERENCES [dbo].[StaffingLog] ([ID]) ON DELETE CASCADE
GO



CREATE FUNCTION GetCountiesForProvider
(
	@providerId int,
	@delimiter nvarchar(5)
)
RETURNS nvarchar(MAX)
AS
BEGIN
	DECLARE @Result nvarchar(MAX)
	SET @delimiter = COALESCE(@delimiter,', ')
	SELECT @Result = STUFF(
			(
			SELECT DISTINCT  @delimiter + ZipCodes.ZipCounty
			FROM            ZipCodes INNER JOIN
									 ProviderServiceZipCodes ON ZipCodes.ZipCode = ProviderServiceZipCodes.ZipCode
			WHERE        (ProviderServiceZipCodes.ProviderID = @providerId)
			ORDER BY @delimiter + ZipCodes.ZipCounty
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		)

	RETURN @Result

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
		)
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




CREATE PROCEDURE [dbo].[GetSelectedProvidersByStaffingLog]
	@staffingLogID int,
	@staffingLogProviderID int = NULL
	AS 
BEGIN

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
		slp.HasBeenContacted,
		slp.Response,
		slp.Notes
	FROM dbo.Providers AS p INNER JOIN dbo.StaffingLogProviders AS slp ON p.ID = slp.ProviderID
	LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
	WHERE p.ProviderActive = 1 AND slp.StaffingLogID = @staffingLogID AND slp.ID = COALESCE(@staffingLogProviderID, slp.ID)
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
	
END
GO



CREATE TABLE [dbo].[FunctioningLevels] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_dbo.FunctioningLevels] PRIMARY KEY ([ID])
)
ALTER TABLE [dbo].[Cases] ADD [FunctioningLevelID] [int]
CREATE INDEX [IX_FunctioningLevelID] ON [dbo].[Cases]([FunctioningLevelID])
ALTER TABLE [dbo].[Cases] ADD CONSTRAINT [FK_dbo.Cases_dbo.FunctioningLevels_FunctioningLevelID] FOREIGN KEY ([FunctioningLevelID]) REFERENCES [dbo].[FunctioningLevels] ([ID])
GO

SET IDENTITY_INSERT [dbo].[FunctioningLevels] ON 
INSERT INTO [dbo].[FunctioningLevels] ([ID],[Name]) VALUES (1, 'Young High Functioning')
INSERT INTO [dbo].[FunctioningLevels] ([ID],[Name]) VALUES (2, 'Old High Functioning')
INSERT INTO [dbo].[FunctioningLevels] ([ID],[Name]) VALUES (3, 'Young Low Functioning')
INSERT INTO [dbo].[FunctioningLevels] ([ID],[Name]) VALUES (4, 'Old Low Functioning')
SET IDENTITY_INSERT [dbo].[FunctioningLevels] OFF
GO






-- ========================
-- Handle services by insurance
-- ========================

CREATE TABLE dbo.InsuranceServices (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	InsuranceID INT NOT NULL REFERENCES dbo.Insurances (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ServiceID INT NOT NULL REFERENCES dbo.Services (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ProviderTypeID INT NOT NULL REFERENCES dbo.ProviderTypes (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ServiceEffectiveDate DATETIME2 NULL,
	ServiceDefectiveDate DATETIME2 NULL
);
CREATE INDEX idxInsuranceServicesInsuranceID ON dbo.InsuranceServices (InsuranceID);
CREATE INDEX idxInsuranceServicesServiceID ON dbo.InsuranceServices (ServiceID);
CREATE INDEX idxInsuranceServicesProviderTypeID ON dbo.InsuranceServices (ProviderTypeID);
CREATE INDEX idxInsuranceServicesStartEndDate ON dbo.InsuranceServices (ServiceEffectiveDate, ServiceDefectiveDate);
GO


CREATE TABLE dbo.ServiceTypeEnum (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	TypeName NVARCHAR(50) NOT NULL
);
CREATE UNIQUE INDEX idxServiceTypeName ON dbo.ServiceTypeEnum (TypeName);
GO
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (0, 'General');
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (1, 'Assessment');
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (2, 'Care');
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (3, 'Social');
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (4, 'Supervision');
INSERT INTO dbo.ServiceTypeEnum (ID, TypeName) VALUES (5, 'Management');
GO


ALTER TABLE dbo.Services ADD IsFixed BIT NOT NULL DEFAULT 0;
ALTER TABLE dbo.Services ADD ServiceTypeID INT NOT NULL DEFAULT 0;
GO

UPDATE dbo.Services SET IsFixed = 1;	-- all existing services are fixed for now (until we determine which can be safely released)
UPDATE dbo.Services SET ServiceTypeID = 2 WHERE ID = 9; -- DR		(care)
UPDATE dbo.Services SET ServiceTypeID = 0 WHERE ID = 10; -- PRT		(general)
UPDATE dbo.Services SET ServiceTypeID = 1 WHERE ID = 11; -- I-ASS	(assessment)
UPDATE dbo.Services SET ServiceTypeID = 5 WHERE ID = 12; -- TP		(management)
UPDATE dbo.Services SET ServiceTypeID = 4 WHERE ID = 13; -- DSU		(supervision)
UPDATE dbo.Services SET ServiceTypeID = 3 WHERE ID = 14; -- SSG		(social)
UPDATE dbo.Services SET ServiceTypeID = 4 WHERE ID = 15; -- SDR		(supervision)
UPDATE dbo.Services SET ServiceTypeID = 5 WHERE ID = 16; -- TM		(management)
UPDATE dbo.Services SET ServiceTypeID = 1 WHERE ID = 17; -- AS 		(assessment)
UPDATE dbo.Services SET ServiceTypeID = 1 WHERE ID = 18; -- FUA 	(assessment)
GO






GO
EXEC meta.UpdateVersion '2.1.1.0'
GO

