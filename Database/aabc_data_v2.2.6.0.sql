/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.5.0
dym:TargetEndingVersion: 2.2.6.0
---------------------------------------------------------------------

	Auth Matrix Sourcing/summary

---------------------------------------------------------------------*/



CREATE PROCEDURE dbo.GetAuthMatrixSource 
	@caseId int,
	@ignoreBCBAAuths bit = 0,
	@numberOfMonthsRecentlyTerminated int = 0,
	@maxMonthSpread int = 12
AS
BEGIN

	/* TEST DATA
	DECLARE @caseId INT = 1986
	DECLARE @ignoreBCBAAuths BIT = 0
	DECLARE @numberofMonthsRecentlyTerminated INT = 0
	DECLARE @maxMonthSpread INT = 12
	-- */

	SET NOCOUNT ON;

	DECLARE @start datetime
	DECLARE @end datetime
	DECLARE @months int
	DECLARE @sqltable NVARCHAR(MAX)
	DECLARE @sqlpivot NVARCHAR(MAX)
	DECLARE @cols1 NVARCHAR(MAX)
	DECLARE @cols2 NVARCHAR(MAX)
	DECLARE @cols3 NVARCHAR(MAX)


	-- Calculation of @start and @end
	SELECT @start = COALESCE(MIN(AuthStartDate),GETDATE()) FROM dbo.CaseAuthCodes AS CAC
	WHERE CaseID = @caseId AND CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()

	SELECT @end = COALESCE(MAX(AuthEndDate),GETDATE()) FROM dbo.CaseAuthCodes AS CAC
	WHERE CaseID = @caseId AND CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)

	SET @start = DATEFROMPARTS(YEAR(@start),MONTH(@start),1)
	SET @end = DATEADD(day,-1,DATEADD(month,1,DATEFROMPARTS(YEAR(@end),MONTH(@end),1)))

	SELECT @months = DATEDIFF(month, @start, @end)

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)

	IF (@months > @maxMonthSpread)
		SET @start = DATEADD(day,1,DATEADD(month,@maxMonthSpread*-1,@end))

	SELECT @months = DATEDIFF(month, @start, @end)

	PRINT 'Start: ' + CONVERT(varchar, @start, 111)
	PRINT 'End: ' + CONVERT(varchar, @end, 111)
	PRINT 'Months: ' + CONVERT(varchar, @months)
	PRINT CHAR(13)+CHAR(10)


	-- Building column lists
	SET @cols1 = N''
	SET @cols2 = N''
	SET @cols3 = N''

	SELECT @cols1 += N', ' + QUOTENAME(x.Month) + ' decimal(18,2) NULL'
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	SELECT @cols2 += N', COALESCE(p.' + QUOTENAME(x.Month) + ',0) ' + QUOTENAME(x.Month)
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	SELECT @cols3 += N', ' + QUOTENAME(x.Month)
	  FROM (SELECT  CONVERT(varchar, DATEADD(MONTH, x.number - 1, @start),111) AS [Month]
			FROM    dbo.Numbers x
			WHERE   x.number <= DATEDIFF(MONTH, @start, @end) + 1) AS x

	PRINT 'COL1: ' + @cols1
	PRINT 'COL2: ' + @cols2
	PRINT 'COL3: ' + @cols3
	PRINT CHAR(13)+CHAR(10)

	SET @sqltable = 'ALTER TABLE #T ADD ' + STUFF(@cols1, 1, 2, '')
	PRINT @sqltable

	-- creating temp table
	IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T
	CREATE TABLE #T ( CaseAuthID int )
	EXEC sys.sp_executesql @sqltable

	-- filling temp table with pivotted data
	SET @sqlpivot = N'
	INSERT INTO #T
	SELECT CaseAuthID, ' + STUFF(@cols2, 1, 2, '') + '
	FROM
	(
	  SELECT 
			HB.CaseAuthID, 
			DATEFROMPARTS(YEAR(H.HoursDate),MONTH(H.HoursDate), 1) AS [Month], 
			CAST(HB.Minutes AS decimal(18,2)) / 60 AS Hours
		FROM dbo.CaseAuthHoursBreakdown AS HB
		INNER JOIN dbo.CaseAuthHours AS H ON HB.HoursID = H.ID
		WHERE H.HoursDate >= @start AND H.HoursDate <= @end AND H.CaseId = @CaseId
	) AS j
	PIVOT
	(
	  SUM(Hours) FOR Month IN ('
	  + STUFF(@cols3, 1, 1, '')
	  + ')
	) AS p'
	PRINT @sqlpivot
	EXEC sp_executesql @sqlpivot , N'@start datetime, @end datetime, @CaseId int', @start, @end, @CaseId

	-- outputting joined data
	SELECT
		A.AuthID, 
		(
			SELECT TOP (1) I.InsuranceName
			FROM Insurances AS I INNER JOIN CaseInsurances AS CI ON CI.InsuranceID = I.ID AND CI.CaseID = A.CaseID
			WHERE COALESCE(CI.DatePlanEffective,A.AuthEndDate) <= A.AuthEndDate AND COALESCE(CI.DatePlanTerminated,A.AuthStartDate) >= A.AuthStartDate
			ORDER BY CI.DatePlanTerminated, CI.DatePlanEffective DESC
		) AS InsuranceName,
		A.AuthType,
		A.AuthCode, 
		A.AuthDescription, 
		A.AuthStartDate, 
		A.AuthEndDate, 
		(
			SELECT TOP(1) P.ProviderFirstName + ' ' + P.ProviderLastName
			FROM Providers AS P INNER JOIN CaseProviders AS CP ON CP.ProviderID = P.ID AND CP.CaseID = A.CaseID
			WHERE CP.Active = 1 AND CP.IsInsuranceAuthorizedBCBA = 1 AND COALESCE(CP.ActiveStartDate,A.AuthStartDate) <= A.AuthStartDate AND COALESCE(CP.ActiveEndDate,A.AuthEndDate) >= A.AuthEndDate
			ORDER BY CP.ActiveEndDate, CP.ActiveStartDate DESC
		) AS AuthBCBA,
		A.AuthTotalHoursApproved, 
		A.AuthTotalHoursUtilized,
		CAST(A.AuthTotalHoursApproved - A.AuthTotalHoursUtilized AS decimal(18,2)) AS AuthTotalHoursRemaining,
		A.AuthTotalDays,
		A.AuthUtilizedDays,
		A.AuthRemainingDays,
		CAST(A.AuthTotalHoursApproved / (A.AuthTotalDays / 7) AS decimal(18,2)) AS ExpectedAvgHoursWeek,
		CASE 
			WHEN A.AuthUtilizedDays = 0 THEN NULL
			ELSE CAST(A.AuthTotalHoursUtilized / (A.AuthUtilizedDays / 7) AS decimal(18,2))
		END AS ActualAvgHoursWeek,
		CASE 
			WHEN A.AuthRemainingDays = 0 THEN NULL
			ELSE CAST(CAST(A.AuthTotalHoursApproved - A.AuthTotalHoursUtilized AS decimal(18,2)) / (A.AuthRemainingDays / 7) AS decimal(18,2))
		END AS AvgRemainingHoursWeek,
		B.*
	FROM (
		SELECT        
		CAC.ID AS AuthID, 
		CAC.CaseID, 
		CACL.AuthClassCode AS AuthType,
		AC.CodeCode AS AuthCode, 
		AC.CodeDescription AS AuthDescription, 
		CAST(CAC.AuthStartDate AS date) AS AuthStartDate, 
		CAST(CAC.AuthEndDate AS date) AS AuthEndDate, 
		CAC.AuthTotalHoursApproved, 
		(
			SELECT CAST(CAST(COALESCE(SUM(CAHB.Minutes),0) AS decimal(18,2)) / 60 AS decimal(18,2))
			FROM CaseAuthHoursBreakdown AS CAHB
			WHERE CAHB.CaseAuthID = CAC.ID
		) AS AuthTotalHoursUtilized,
		CAST(DATEDIFF(day, CAC.AuthStartDate,CAC.AuthEndDate) AS decimal(18,2)) AS AuthTotalDays,
		CASE 
			WHEN GETDATE() < CAC.AuthStartDate THEN 0
			WHEN GETDATE() > CAC.AuthEndDate THEN CAST(DATEDIFF(day, CAC.AuthStartDate,CAC.AuthEndDate) AS decimal(18,2))
			ELSE CAST(DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS decimal(18,2))
		END AS AuthUtilizedDays,
		CASE 
			WHEN GETDATE() > CAC.AuthEndDate THEN 0
			ELSE CAST(DATEDIFF(day, GETDATE(), CAC.AuthEndDate) AS decimal(18,2))
		END AS AuthRemainingDays
		FROM CaseAuthCodes AS CAC
		INNER JOIN AuthCodes AS AC ON AC.ID = CAC.AuthCodeID
		INNER JOIN CaseAuthClasses AS CACL ON CAC.AuthClassID = CACL.ID
		WHERE CAC.CaseID = @CaseId AND CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= DATEADD(month, ABS(@numberOfMonthsRecentlyTerminated)*-1, GETDATE())
	) AS A
	LEFT JOIN #T AS B ON A.AuthID = B.CaseAuthID
	WHERE A.AuthType =
	CASE
		WHEN @ignoreBCBAAuths > 0 THEN 'GENERAL'
		ELSE A.AuthType
	END
	ORDER BY A.AuthStartDate, A.AuthCode

	-- deleting temp table
	IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T

END
GO


-- SP TO REMOVE: dbo.UpsertGeneralHours




CREATE PROCEDURE ahr.AuthSummary (@CaseID INT) AS

	-- DECLARE @CaseID INT = 1986

	SELECT 
		cac.ID AS AuthID,
		ac.CodeCode, 
		cac.AuthStartDate, 
		cac.AuthEndDate, 
		cac.AuthTotalHoursApproved,
		cahb.Minutes,
		cah.HoursDate,
		cah.HoursBillable
	FROM dbo.CaseAuthCodes AS cac
	INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID
	LEFT JOIN dbo.CaseAuthHoursBreakdown AS cahb ON cahb.CaseAuthID = cac.ID
	LEFT JOIN dbo.CaseAuthHours AS cah ON cah.ID = cahb.HoursID
	WHERE cac.CaseID = @CaseID
		AND cac.AuthStartDate <= GETDATE()
		AND cac.AuthEndDate >= GETDATE() ;


	SELECT 
		cac.ID AS AuthID,
		ac.CodeCode, 
		cac.AuthStartDate, 
		cac.AuthEndDate, 
		cac.AuthTotalHoursApproved,
		SUM(COALESCE(cahb.[Minutes], 0)) / 60 AS HoursUtilized
	FROM dbo.CaseAuthCodes AS cac
	INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID
	LEFT JOIN dbo.CaseAuthHoursBreakdown AS cahb ON cahb.CaseAuthID = cac.ID
	WHERE cac.CaseID = @CaseID
		AND cac.AuthStartDate <= GETDATE()
		AND cac.AuthEndDate >= GETDATE()
	GROUP BY
		cac.ID,
		ac.CodeCode, 
		cac.AuthStartDate, 
		cac.AuthEndDate, 
		cac.AuthTotalHoursApproved;

GO



GO
EXEC meta.UpdateVersion '2.2.6.0'
GO

