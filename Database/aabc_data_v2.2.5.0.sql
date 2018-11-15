/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.4.0
dym:TargetEndingVersion: 2.2.5.0
---------------------------------------------------------------------

		Auth Reporting
		Provider Portal Admin
		Referral Insurance Update

---------------------------------------------------------------------*/



-- ========================  
-- Add a view for Provider Portal Users
-- ======================== 
CREATE VIEW dbo.ProviderPortalUserAdminList AS
	SELECT
		p.ID AS ProviderID,
		u.AspNetUserID,
		p.ProviderFirstName AS FirstName,
		p.ProviderLastName AS LastName,
		p.ProviderPrimaryPhone AS Phone,
		p.ProviderPrimaryEmail AS Email,
		u.ProviderUserNumber AS UserNumber,
		u.ProviderHasAppAccess AS HasAppAccess
	FROM dbo.Providers AS p
	LEFT JOIN dbo.ProviderPortalUsers AS u ON u.ProviderID = p.ID
	-- WHERE p.ProviderStatus = 1
GO




-- ========================  
-- Referral insurance update 
-- ======================== 
ALTER TABLE [dbo].[Referrals] ADD [InsuranceCompanyID] [int]
ALTER TABLE [dbo].[Referrals] ADD [ReferralFundingType] [nvarchar](20)
ALTER TABLE [dbo].[Referrals] ADD [ReferralBenefitType] [nvarchar](20)
ALTER TABLE [dbo].[Referrals] ADD [ReferralCoPayAmount] [money]
ALTER TABLE [dbo].[Referrals] ADD [ReferralCoInsuranceAmount] [money]
ALTER TABLE [dbo].[Referrals] ADD [ReferralDeductibleTotal] [money]
CREATE INDEX [IX_InsuranceCompanyID] ON [dbo].[Referrals]([InsuranceCompanyID])
ALTER TABLE [dbo].[Referrals] ADD CONSTRAINT [FK_dbo.Referrals_dbo.Insurances_InsuranceCompanyID] FOREIGN KEY ([InsuranceCompanyID]) REFERENCES [dbo].[Insurances] ([ID])
GO





-- ========================  
-- Add auth utilization report source
-- ======================== 
CREATE PROCEDURE webreports.AuthorizationUtilization 
AS
BEGIN
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
			CAST(M.TotalUtilizedMin / 60 / M.TotalAllowed * 100 AS decimal(18,2)) AS UtilizationPercentage,
			CAST(M.TotalDaysInAuth * M.TotalUtilizedMin / M.DaysSinceAuthStart / 60 / M.TotalAllowed * 100 AS decimal(18,2)) AS ExpectedFinalUtilization
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




GO
EXEC meta.UpdateVersion '2.2.5.0'
GO

