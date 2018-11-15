/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.3.1.0
dym:TargetEndingVersion: 2.3.2.0
---------------------------------------------------------------------

	

---------------------------------------------------------------------*/





-- ========================
-- update the temp table notes to NVARCHAR(MAX)
-- ========================
ALTER PROCEDURE [webreports].[PatientHoursReportDetailSignatures] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2) AS 
 	 
	/* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	
	SET @CaseID = 671
	SET @StartDate = '11/1/2017'
	SET @EndDate = '11/30/2017'
	-- */
	
	DECLARE @tablevar TABLE(
		StartDate DATETIME2, EndDate DATETIME2, SignoffDate DATETIME2, reportedBCBAID INT, reportedBCBAFirstName NVARCHAR(100), reportedBCBALastName NVARCHAR(100),
		PatientLastName NVARCHAR(100), PatientFirstName NVARCHAR(100), HoursDate DATETIME2, HoursTimeIn DATETIME2, HoursTimeOut DATETIME2, HoursTotal DECIMAL(10,2),
		ProviderID INT, ProviderLastName NVARCHAR(100), ProviderFirstName NVARCHAR(100), ProviderTypeCode NVARCHAR(100), LatestSignoffDate DATETIME2, ServiceCode NVARCHAR(100), ID INT, HoursNotes NVARCHAR(MAX));
	INSERT INTO @tablevar (
		StartDate, EndDate, SignoffDate, reportedBCBAID, reportedBCBAFirstName, reportedBCBALastName,
		PatientLastName, PatientFirstName, HoursDate, HoursTimeIn, HoursTimeOut, HoursTotal,
		ProviderID, ProviderLastName, ProviderFirstName, ProviderTypeCode, LatestSignoffDate, ServiceCode, ID, HoursNotes
		) EXEC webreports.PatientHoursReportDetail @CaseID, @StartDate, @EndDate;
		
	SELECT 
		reportedBCBAID AS ProviderID,
		reportedBCBAFirstName AS ProviderFirstName,
		reportedBCBALastName AS ProviderLastName,
		SignoffDate		
	FROM @tablevar
	GROUP BY 
		reportedBCBAID,
		reportedBCBAFirstName,
		reportedBCBALastName,
		SignoffDate		

	UNION

	SELECT
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate
	FROM @tablevar
	GROUP BY
		ProviderID,
		ProviderFirstName,
		ProviderLastName,
		LatestSignoffDate

	RETURN
GO



-- ========================
-- Update staffing log to ignore discharged cases
-- ========================
ALTER PROCEDURE [dbo].[GetStaffingLogSummary] AS BEGIN
	SET NOCOUNT ON;
	SELECT 
		SL.ID,
		P.ID AS PatientID,
		P.PatientFirstName AS FirstName,
		P.PatientLastName AS LastName,
		P.PatientAddress1 AS Address,
		P.PatientCity AS City,
		p.PatientState AS State,
		P.PatientZip AS Zip,
		(
			SELECT ZipCounty
			FROM ZipCodes
			WHERE ZipCode = P.PatientZip
		) AS County,
		P.PatientDateOfBirth AS DateOfBirth,
		(
			SELECT RTRIM(LTRIM(COALESCE(S.StaffFirstName,'') + ' ' + COALESCE(S.StaffLastName,'')))
			FROM Staff AS S
			WHERE S.ID = C.CaseAssignedStaffID
		) AS CaseManager,
		SL.DateWentToRestaff
	FROM StaffingLog AS SL 
		INNER JOIN Cases AS C ON C.ID = SL.ID
		INNER JOIN Patients AS P ON P.ID = C.PatientID
	WHERE C.CaseStatus <> -1 -- ignore discharged cases
	ORDER BY P.PatientLastName, P.PatientFirstName
END
GO








-- ========================
-- Add session signature tracking
-- ========================
CREATE TABLE [dbo].[SessionSignatures] (
    [ID] [int] NOT NULL,
    [ParentSignature] [varchar](max),
	[ParentSignatureType] [varchar](20),
    [ProviderSignature] [varchar](max),
	[ProviderSignatureType] [varchar](20),
    [Created] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_dbo.SessionSignatures] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_ID] ON [dbo].[SessionSignatures]([ID])
ALTER TABLE [dbo].[SessionSignatures] ADD CONSTRAINT [FK_dbo.SessionSignatures_dbo.CaseAuthHours_ID] FOREIGN KEY ([ID]) REFERENCES [dbo].[CaseAuthHours] ([ID])
GO




-- ========================
-- Update to include provider credentials
-- ========================
CREATE FUNCTION dbo.GetInsuranceCredentialsForProviders (
	@providerID INT,
	@delimiter NVARCHAR(5)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Result NVARCHAR(MAX)
	SET @delimiter = COALESCE(@delimiter, ', ')
	SELECT @Result = STUFF(
		(
			SELECT DISTINCT @delimiter + i.InsuranceName
			FROM dbo.ProviderInsuranceCredentials AS c
			INNER JOIN dbo.Insurances AS i ON i.ID = c.InsuranceID
			WHERE c.ProviderID = @providerID
				AND (c.StartDate <= GETDATE() OR c.StartDate IS NULL)
				AND (c.EndDate >= GETDATE() OR c.EndDate IS NULL)
			ORDER BY @delimiter + i.InsuranceName
			FOR XML PATH(''), TYPE
		).value('.', 'NVARCHAR(MAX)'), 1, LEN(@delimiter), ''
	)
	RETURN @Result
END
GO


ALTER PROCEDURE [dbo].[GetProvidersSearch]	@providerId int = NULL 
AS 
BEGIN
	-- DECLARE @providerID INT = NULL;

	SET NOCOUNT ON;
	SELECT 
		P.ID,
		P.ProviderStatus AS Status,
		P.ProviderLastName AS LastName,
		P.ProviderFirstName AS FirstName,
		dbo.GetTypeForProvider(P.ID, NULL) AS TypeCode,
		P.ProviderCity AS City,
		P.ProviderState AS State,
		P.ProviderZip AS Zip,
		P.ProviderPrimaryPhone AS Phone,
		P.ProviderPrimaryEmail AS Email,
		(
			SELECT COUNT(*)
			FROM CaseProviders AS CP
			WHERE CP.ProviderID = P.ID AND EXISTS(
				SELECT *
				FROM Cases AS C
				WHERE C.ID = CP.CaseID AND C.CaseStatus != -1
			)
		) AS ActiveCaseCount,
		dbo.GetZipCodesForProvider(P.ID, NULL) AS ZipCodes,
		dbo.GetCountiesForProvider(P.ID, NULL) AS Counties,
		dbo.GetInsuranceCredentialsForProviders(P.ID, NULL) AS [Credentials]
	FROM Providers AS P
	WHERE P.ID = COALESCE(@providerID, p.ID)
	ORDER BY P.ProviderLastName, P.ProviderFirstName
END
GO





-- ========================
-- Update to accept and action on a state filter (NY or non-NY)
-- ========================
ALTER PROCEDURE [webreports].[PayablesByPeriod](@FirstDayOfMonth DATETIME2, @StateFilter INT) AS

	/*	TEST DATA
	DECLARE @FirstDayOfMonth DATETIME2 = '2017-05-01'
	DECLARE @StateFilter INT = 0
	-- */
	
	/* State Filter
		0 = None
		1 = NY Only
		2 = Non-NY
	*/

	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = @FirstDayOfMonth
	SET @EndDate = EOMONTH(@StartDate)
	
	IF @StateFilter = 0 BEGIN	-- no state filter

		SELECT
			p.ID,
			COALESCE(p.PayrollID, p.ID) AS PayrollID,
			p.ProviderFirstName,
			p.ProviderLastName,
			SUM(h.HoursPayable) AS TotalPayable,
			COUNT(h.HoursPayable) - SUM(CAST(h.HoursHasCatalystData AS INT)) AS EntriesMissingCatalystData
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

	END

	IF @StateFilter = 1 BEGIN	-- filter to NY providers only

		SELECT
			p.ID,
			COALESCE(p.PayrollID, p.ID) AS PayrollID,
			p.ProviderFirstName,
			p.ProviderLastName,
			SUM(h.HoursPayable) AS TotalPayable,
			COUNT(h.HoursPayable) - SUM(CAST(h.HoursHasCatalystData AS INT)) AS EntriesMissingCatalystData
		FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
		WHERE h.HoursStatus = 3 -- finalized hours only
			AND h.HoursDate >= @StartDate
			AND h.HoursDate <= @EndDate
			AND h.HoursPayableRef IS NULL
			AND p.ProviderState = 'NY'
		GROUP BY 
			p.ID,
			COALESCE(p.PayrollID, p.ID),
			p.ProviderFirstName,
			p.ProviderLastName
		ORDER BY p.ProviderLastName

	END

	IF @StateFilter = 2 BEGIN	-- filter to non-NY providers

		SELECT
			p.ID,
			COALESCE(p.PayrollID, p.ID) AS PayrollID,
			p.ProviderFirstName,
			p.ProviderLastName,
			SUM(h.HoursPayable) AS TotalPayable,
			COUNT(h.HoursPayable) - SUM(CAST(h.HoursHasCatalystData AS INT)) AS EntriesMissingCatalystData
		FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS h ON h.CaseProviderID = p.ID
		WHERE h.HoursStatus = 3 -- finalized hours only
			AND h.HoursDate >= @StartDate
			AND h.HoursDate <= @EndDate
			AND h.HoursPayableRef IS NULL
			AND (p.ProviderState IS NULL OR p.ProviderState <> 'NY')
		GROUP BY 
			p.ID,
			COALESCE(p.PayrollID, p.ID),
			p.ProviderFirstName,
			p.ProviderLastName
		ORDER BY p.ProviderLastName

	END
	RETURN
GO





-- ========================
-- Add Staffing Active field
-- ========================
ALTER TABLE dbo.StaffingLog ADD StaffingActive BIT NOT NULL DEFAULT 1;
GO
CREATE INDEX idxStaffingLogActive ON dbo.StaffingLog (StaffingActive);
GO

-- Update staffing log list to disregard inactive staffing rows
ALTER PROCEDURE [dbo].[GetStaffingLogSummary] AS BEGIN
	SET NOCOUNT ON;
	SELECT 
		SL.ID,
		P.ID AS PatientID,
		P.PatientFirstName AS FirstName,
		P.PatientLastName AS LastName,
		P.PatientAddress1 AS Address,
		P.PatientCity AS City,
		p.PatientState AS State,
		P.PatientZip AS Zip,
		(
			SELECT ZipCounty
			FROM ZipCodes
			WHERE ZipCode = P.PatientZip
		) AS County,
		P.PatientDateOfBirth AS DateOfBirth,
		(
			SELECT RTRIM(LTRIM(COALESCE(S.StaffFirstName,'') + ' ' + COALESCE(S.StaffLastName,'')))
			FROM Staff AS S
			WHERE S.ID = C.CaseAssignedStaffID
		) AS CaseManager,
		SL.DateWentToRestaff
	FROM StaffingLog AS SL 
		INNER JOIN Cases AS C ON C.ID = SL.ID
		INNER JOIN Patients AS P ON P.ID = C.PatientID
	WHERE C.CaseStatus <> -1 -- ignore discharged cases
		AND SL.StaffingActive = 1
	ORDER BY P.PatientLastName, P.PatientFirstName
END
GO





GO
EXEC meta.UpdateVersion '2.3.2.0'
GO

