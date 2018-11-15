/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.6.0
dym:TargetEndingVersion: 1.8.7.0
---------------------------------------------------------------------

	Hours Watch - Active Cases/Providers
	
	
---------------------------------------------------------------------*/





-- ========================
-- Create an ActiveCaseProviders function
-- ========================
CREATE FUNCTION [dbo].[GetActiveCaseProviders] 
(	
	@StartDate DATETIME2,
	@EndDate DATETIME2
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT *
	FROM [CaseProviders]
	WHERE (
			(ActiveStartDate IS NULL OR ActiveStartDate <= @EndDate) AND 
			(ActiveEndDate IS NULL OR CaseProviders.ActiveEndDate >= @StartDate)
		)
)
GO




-- ============================
-- Cases with Hours but no BCBA Billing
-- ============================
  ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoBCBAHours](@StartDate DATE, @EndDate DATE) AS

	/* -- TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-08-01'
	SET @EndDate = '2016-08-30'
	-- */

	SELECT 		
		C.ID AS CaseID,
		P.ID AS PatientID,
		P.PatientLastName,
		P.PatientFirstName,
		BCBA.ProviderLastName,
		BCBA.ProviderFirstName,
		CMP.WatchComment,
		CMP.WatchIgnore 
	FROM Cases AS C INNER JOIN dbo.Patients AS P ON C.PatientID = P.ID
					LEFT JOIN dbo.CaseMonthlyPeriods AS CMP ON CMP.CaseID = c.ID AND CMP.PeriodFirstDayOfMonth = @StartDate
					LEFT JOIN (
						SELECT 
						CP.CaseID AS CaseID,
						Providers.ID AS ProviderID,
						Providers.ProviderLastName,
						Providers.ProviderFirstName
						FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) AS CP INNER JOIN Providers ON CP.ProviderID = Providers.ID
						WHERE ProviderType = 15
					 ) AS BCBA ON BCBA.CaseID = c.ID
	WHERE C.ID IN (
		-- Cases with :
		--   - no BCBA hours in the requested period
		--   - reported by active providers in the requested period
		--   - with hours reported in the requested period
		SELECT H.CaseID FROM CaseAuthHours as H
		WHERE H.CaseId NOT IN(
			-- CaseId of Reported Hours by BCBAs in the requested period
			SELECT        CaseId
			FROM            CaseAuthHours AS CAH INNER JOIN Providers AS PR ON CAH.CaseProviderID = PR.ID
			WHERE PR.ProviderType = 15 AND CAH.HoursDate >= @StartDate AND  CAH.HoursDate <= @EndDate
		)
		AND EXISTS(SELECT * FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) WHERE CaseId = H.CaseID AND ProviderId = H.CaseProviderID)
		AND H.HoursDate >= @StartDate AND  H.HoursDate <= @EndDate
	)
	ORDER BY PatientLastName, PatientFirstName, ProviderLastName, ProviderFirstName
RETURN
GO

-- ============================
-- Cases with Hours but no AIDE Billing
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoAideHours](@StartDate DATETIME2, @EndDate DATETIME2) AS

	/*  --TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2016-07-01'
	SET @EndDate = '2016-07-30'
	-- */
	SELECT 		
		C.ID AS CaseID,
		P.ID AS PatientID,
		P.PatientLastName,
		P.PatientFirstName,
		BCBA.ProviderLastName,
		BCBA.ProviderFirstName,
		CMP.WatchComment,
		CMP.WatchIgnore 
	FROM Cases AS C INNER JOIN dbo.Patients AS P ON C.PatientID = P.ID
					LEFT JOIN dbo.CaseMonthlyPeriods AS CMP ON CMP.CaseID = c.ID AND CMP.PeriodFirstDayOfMonth = @StartDate
					LEFT JOIN (
						SELECT 
						CP.CaseID AS CaseID,
						Providers.ID AS ProviderID,
						Providers.ProviderLastName,
						Providers.ProviderFirstName
						FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) AS CP INNER JOIN Providers ON CP.ProviderID = Providers.ID
						WHERE ProviderType = 15
					 ) AS BCBA ON BCBA.CaseID = c.ID
	WHERE C.ID IN (
		-- Cases with :
		--   - no Aide hours in the requested period
		--   - reported by active providers in the requested period
		--   - with hours reported in the requested period
		SELECT H.CaseID FROM CaseAuthHours as H
		WHERE H.CaseId NOT IN(
			-- CaseId of Reported Hours by Aides in the requested period
			SELECT        CaseId
			FROM            CaseAuthHours AS CAH 
				INNER JOIN Providers AS PR ON CAH.CaseProviderID = PR.ID
				INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = pr.ProviderType
			WHERE pt.ProviderTypeCode = 'AIDE' AND CAH.HoursDate >= @StartDate AND  CAH.HoursDate <= @EndDate
		) 		
		AND EXISTS(SELECT * FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) WHERE CaseId = H.CaseID AND ProviderId = H.CaseProviderID)
		AND H.HoursDate >= @StartDate AND  H.HoursDate <= @EndDate
	)
	ORDER BY PatientLastName, PatientFirstName, ProviderLastName, ProviderFirstName
	RETURN
	GO

-- ============================
-- Cases with Hours but no Direct Supervision
-- ============================
ALTER PROCEDURE [dbo].[GetCasesWithHoursButNoSupervision](@StartDate DATETIME2, @EndDate DATETIME2) AS

	/*-- TEST DATA
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2

	SET @StartDate = '2017-08-01'
	SET @EndDate = '2017-08-31'
	--*/

	SELECT 		
		C.ID AS CaseID,
		P.ID AS PatientID,
		P.PatientLastName,
		P.PatientFirstName,
		BCBA.ProviderLastName,
		BCBA.ProviderFirstName,
		CMP.WatchComment,
		CMP.WatchIgnore 
	FROM Cases AS C INNER JOIN dbo.Patients AS P ON C.PatientID = P.ID
					LEFT JOIN dbo.CaseMonthlyPeriods AS CMP ON CMP.CaseID = c.ID AND CMP.PeriodFirstDayOfMonth = @StartDate
					LEFT JOIN (
						SELECT 
						CP.CaseID AS CaseID,
						Providers.ID AS ProviderID,
						Providers.ProviderLastName,
						Providers.ProviderFirstName
						FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) AS CP INNER JOIN Providers ON CP.ProviderID = Providers.ID
						WHERE ProviderType = 15
					 ) AS BCBA ON BCBA.CaseID = c.ID
	WHERE C.ID IN (
		-- Cases with :
		--   - no supervision hours in the requested period
		--   - reported by active providers in the requested period
		--   - with hours reported in the requested period
		SELECT H.CaseID FROM CaseAuthHours as H
		WHERE H.CaseId NOT IN(
			-- CaseId of Reported Hours of Supervision in the requested period
			SELECT CAH.CaseID
			FROM CaseAuthHours AS CAH
			INNER JOIN Providers AS p ON CAH.CaseProviderID = p.ID
			INNER JOIN ProviderTypes AS pt ON pt.ID = p.ProviderType
			INNER JOIN Services AS s ON s.ID = CAH.HoursServiceID
			WHERE pt.ProviderTypeCode = 'BCBA'
				AND CAH.HoursDate >= @StartDate
				AND CAH.HoursDate <= @EndDate
				AND s.ServiceCode = 'DSU'
		)
		AND EXISTS(SELECT * FROM dbo.GetActiveCaseProviders(@StartDate,@EndDate) WHERE CaseId = H.CaseID AND ProviderId = H.CaseProviderID)
		AND H.HoursDate >= @StartDate AND  H.HoursDate <= @EndDate
	)
	ORDER BY PatientLastName, PatientFirstName, ProviderLastName, ProviderFirstName
	RETURN
	GO





-- ============================
-- Handle Provider Portal Signins
-- ============================
Create Table PatientPortalSignIns (
	Id int identity not null,
	UserId int not null,
	SignInDate DateTime not null,
	SignInType nvarchar(20) not null
)
GO
ALTER TABLE dbo.PatientPortalSignIns ADD CONSTRAINT
	FK_PatientPortalSignIns_PatientPortalWebMembership FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.PatientPortalWebMembership
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

GO






-- ============================
-- Create procs for better MHD handling
-- ============================
	 
CREATE Procedure GetProvidersExceedingDailyLimit(
	@Limit int,
	@StartDate datetime,
	@EndDate datetime,
	@AideProviderTypeId int = 17
)
AS

	select distinct t.ID, t.ProviderFirstName as FirstName, t.ProviderLastName as LastName, t.ProviderFirstName + ' ' + t.ProviderLastName as [Name] from Providers t
	inner join (
	SELECT 
		p.ID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
	WHERE cah.HoursDate >= @StartDate 
		AND cah.HoursDate < @EndDate
		AND p.ProviderType = @AideProviderTypeId
	GROUP BY
		p.ID,
		cah.HoursDate
	Having Sum(cah.HoursBillable) > @Limit
	) q
	on t.ID = q.ID
	order by t.ProviderLastName


go

CREATE Procedure GetProviderExcessDays(
	@ProviderId int,
	@StartDate datetime,
	@EndDate datetime,
	@Limit int
)
AS

select h.CaseProviderID as ProviderId, h.HoursDate as [Date], sum(h.HoursBillable) as BillableHours
from CaseAuthHours h
where h.CaseProviderID = @ProviderId
and h.HoursDate >= @StartDate
and h.HoursDate < @EndDate
group by  h.CaseProviderID, h.HoursDate
having sum(h.HoursBillable) > @Limit
GO









GO
EXEC meta.UpdateVersion '1.8.7.0';
GO

