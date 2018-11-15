/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.8.0
dym:TargetEndingVersion: 1.9.0.0
---------------------------------------------------------------------

	MaxHoursPerDay Fixes
	
	
---------------------------------------------------------------------*/


CREATE Procedure [dbo].[GetPatientsExceedingDailyLimit](
	@Limit int,
	@StartDate datetime,
	@EndDate datetime,
	@AideProviderTypeId int = 17
)
AS

	select distinct t.ID, t.PatientFirstName as FirstName, t.PatientLastName as LastName from Patients t
	inner join (
	SELECT 
		pt.ID
	FROM dbo.CaseAuthHours AS cah
	INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID
	INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
	INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
	WHERE cah.HoursDate >= @StartDate 
		AND cah.HoursDate < @EndDate
		AND p.ProviderType = @AideProviderTypeId
	GROUP BY
		pt.ID,
		cah.HoursDate
	Having Sum(cah.HoursBillable) > @Limit
	) q
	on t.ID = q.ID
	order by t.PatientLastName


GO
CREATE Procedure [dbo].[GetPatientExcessDays](
	@PatientId int,
	@StartDate datetime,
	@EndDate datetime,
	@Limit int,
	@AideProviderTypeId int = 17
)
AS

select c.PatientID, cah.HoursDate as [Date], sum(cah.HoursBillable) as BillableHours
from dbo.CaseAuthHours AS cah
INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID
inner join Cases c
on cah.CaseID = c.ID
where c.PatientID = @PatientId
and cah.HoursDate >= @StartDate
and cah.HoursDate < @EndDate
AND p.ProviderType = @AideProviderTypeId
group by  c.PatientID, cah.HoursDate
having sum(cah.HoursBillable) > @Limit
GO





GO
EXEC meta.UpdateVersion '1.9.0.0';
GO

