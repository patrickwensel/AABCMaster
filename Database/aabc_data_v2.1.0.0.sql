/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.0.9.0
dym:TargetEndingVersion: 2.1.0.0
---------------------------------------------------------------------

	Patient Hours Report Locations
	
---------------------------------------------------------------------*/



-- =========================
-- Get a list of locations for the patient hours report
-- =========================
CREATE PROCEDURE [webreports].[PatientHoursReportLocations] (@CaseID INT, @StartDate DATETIME2, @EndDate DATETIME2, @ProviderID INT) AS 

	 
	 /* --	TEST DATA
	DECLARE @CaseID INT
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	DECLARE @ProviderID INT
	
	SET @CaseID = 411
	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-31'
	SET @ProviderID = 0
	-- */

	DECLARE @Locations NVARCHAR(MAX)

	SELECT @Locations = COALESCE(@Locations + ', ', '') + a.LocationName
	FROM (
		SELECT DISTINCT l.LocationName
		FROM dbo.CaseAuthHours AS cah
		INNER JOIN dbo.ServiceLocations AS l ON l.ID = cah.ServiceLocationID
		WHERE cah.HoursDate >= @StartDate
			AND cah.HoursDate <= @EndDate
			AND cah.CaseID = @CaseID
			AND cah.CaseProviderID = @ProviderID
			AND cah.ServiceLocationID IS NOT NULL
	) AS a
	
	SELECT @Locations AS LocationName

	RETURN

GO





GO
EXEC meta.UpdateVersion '2.1.0.0'
GO

