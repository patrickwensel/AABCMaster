/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.6.0
dym:TargetEndingVersion: 2.2.7.0
---------------------------------------------------------------------

	

---------------------------------------------------------------------*/



-- add handling for divide by zero errors
ALTER PROCEDURE webreports.AuthorizationUtilization
AS BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM (
		SELECT 
			M.PatientLastName, 
			M.PatientFirstName, 
			M.CodeCode,
			M.CodeDescription,
			M.DaysSinceAuthStart,
			M.TotalDaysInAuth,
			M.CurrentAuthProgressPercent,
			CAST(M.TotalUtilizedMin / 60 AS decimal(18,2)) AS CurrentUtilizatedHours,
			M.TotalAllowed AS TotalAllowedHours,
			IIF(M.TotalAllowed = 0, NULL, CAST(M.TotalUtilizedMin / 60 / M.TotalAllowed * 100 AS decimal(18,2))) AS UtilizationPercentage,
			IIF(M.TotalAllowed = 0 OR M.DaysSinceAuthStart = 0, NULL, CAST(M.TotalDaysInAuth * M.TotalUtilizedMin / M.DaysSinceAuthStart / 60 / M.TotalAllowed * 100 AS decimal(18,2))) AS ExpectedFinalUtilization
		FROM 
		(
			SELECT 
				P.PatientLastName, 
				P.PatientFirstName, 
				AUTH.CodeCode, 
				AUTH.CodeDescription,
				CAC.AuthStartDate, 
				CAC.AuthEndDate,
				DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS DaysSinceAuthStart,
				DATEDIFF(day, CAC.AuthStartDate, CAC.AuthEndDate) AS TotalDaysInAuth,
				CAST((CAST(DATEDIFF(day, CAC.AuthStartDate, GETDATE()) AS decimal(18,2)) / DATEDIFF(day, CAC.AuthStartDate, CAC.AuthEndDate) * 100) AS decimal(18,2)) AS CurrentAuthProgressPercent,
				CAC.AuthTotalHoursApproved AS TotalAllowed,
				(
					SELECT CAST(COALESCE(SUM(Minutes),0) AS decimal(18,2))
					FROM dbo.CaseAuthHoursBreakdown AS BR
					WHERE BR.CaseAuthID = CAC.ID 
				) AS TotalUtilizedMin
			FROM dbo.Patients AS P
			INNER JOIN dbo.Cases AS C ON C.PatientID = P.ID
			INNER JOIN dbo.CaseAuthCodes AS CAC ON CAC.CaseID = C.ID
			INNER JOIN dbo.AuthCodes AS AUTH ON	AUTH.ID = CAC.AuthCodeID
			WHERE CAC.AuthStartDate <= GETDATE() AND CAC.AuthEndDate >= GETDATE()
		) AS M
	) AS T
	ORDER BY ExpectedFinalUtilization DESC
END
GO



-- Add case auth hours breakdown log
CREATE TABLE dbo.CaseAuthHoursBreakdownLog (
	ID int IDENTITY(1,1) PRIMARY KEY,
	DateCreated datetime2(7) NOT NULL DEFAULT (getdate()),
	WasResolved bit NOT NULL,
	HoursID int NOT NULL,
	HoursDate date NOT NULL,
	ServiceID int NOT NULL,
	BillableHours decimal(6,2) NOT NULL,
	ProviderTypeID int NOT NULL,
	InsuranceID int NOT NULL,
	AuthMatchRuleDetailJSON nvarchar(MAX) NULL,
	ActiveAuthorizationsJSON nvarchar(MAX) NULL,
	ResolvedAuthID int NULL,
	ResolvedAuthCode nvarchar(20) NULL,
	ResolvedCaseAuthID int NULL,
	ResolvedCaseAuthStart datetime2(7) NULL,
	ResolvedCaseAuthEndDate datetime2(7) NULL,
	ResolvedAuthMatchRuleID int NULL,
	ResolvedMinutes int NULL
);
GO



GO
EXEC meta.UpdateVersion '2.2.7.0'
GO

