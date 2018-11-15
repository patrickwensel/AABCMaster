/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.7.0
dym:TargetEndingVersion: 2.2.8.0
---------------------------------------------------------------------

	Provider SubTypes
	Auth Views

---------------------------------------------------------------------*/


-- ============================
-- Add support for view all auths by case
-- ============================

CREATE PROCEDURE [dbo].[GetAllAuthsByCase]
	@CaseID int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		CAC.ID,
		CASE 
			WHEN GETDATE() > CAC.AuthEndDate THEN 'Historic'
			WHEN GETDATE() <= CAC.AuthEndDate AND GETDATE() >= CAC.AuthStartDate THEN 'Active'
			WHEN GETDATE() < CAC.AuthStartDate THEN 'Future'
			ELSE 'Unknown'
		END AS [Status],
		(
			SELECT TOP (1) I.InsuranceName
			FROM Insurances AS I INNER JOIN CaseInsurances AS CI ON CI.InsuranceID = I.ID AND CI.CaseID = CAC.CaseID
			WHERE COALESCE(CI.DatePlanEffective,CAC.AuthEndDate) <= CAC.AuthEndDate AND COALESCE(CI.DatePlanTerminated,CAC.AuthStartDate) >= CAC.AuthStartDate
			ORDER BY CI.DatePlanTerminated, CI.DatePlanEffective DESC
		) AS InsuranceName,
		CACL.AuthClassCode AS [Type],
		AC.CodeCode AS Code,
		AC.CodeDescription AS [Description],
		CAST(CAC.AuthStartDate AS date) AS StartDate,
		CAST(CAC.AuthEndDate AS date) AS EndDate,
		CAC.AuthTotalHoursApproved AS TotalHoursApproved
	FROM dbo.CaseAuthCodes AS CAC
	INNER JOIN AuthCodes AS AC ON AC.ID = CAC.AuthCodeID
	INNER JOIN CaseAuthClasses AS CACL ON CAC.AuthClassID = CACL.ID
	WHERE CAC.CaseID = @CaseID
	ORDER BY AuthStartDate DESC
END

GO




-- ============================
-- Add support for Provider Sub-types
-- ============================
CREATE TABLE [dbo].[ProviderSubTypes] (
    [ID] [int] NOT NULL IDENTITY,
    [DateCreated] [datetime] NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
    [ProviderParentTypeID] [int] NOT NULL,
    [ProviderSubTypeCode] [nvarchar](32) NOT NULL,
    [ProviderSubTypeName] [nvarchar](255),
    CONSTRAINT [PK_dbo.ProviderSubTypes] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_ProviderParentTypeID] ON [dbo].[ProviderSubTypes]([ProviderParentTypeID])
ALTER TABLE [dbo].[ProviderSubTypes] ADD CONSTRAINT [FK_dbo.ProviderSubTypes_dbo.ProviderTypes_ProviderParentTypeID] FOREIGN KEY ([ProviderParentTypeID]) REFERENCES [dbo].[ProviderTypes] ([ID]) ON DELETE CASCADE
	
ALTER TABLE [dbo].[Providers] ADD [ProviderSubTypeID] [int] NULL
CREATE INDEX [IX_ProviderSubTypeID] ON [dbo].[Providers]([ProviderSubTypeID])
ALTER TABLE [dbo].[Providers] ADD CONSTRAINT [FK_dbo.Providers_dbo.ProviderSubTypes_ProviderSubTypeID] FOREIGN KEY ([ProviderSubTypeID]) REFERENCES [dbo].[ProviderSubTypes] ([ID])
GO

INSERT INTO [dbo].[ProviderSubTypes] ([ProviderParentTypeID],[ProviderSubTypeCode],[ProviderSubTypeName])
     VALUES   (17 ,'TECH' ,'Technician')
INSERT INTO [dbo].[ProviderSubTypes] ([ProviderParentTypeID],[ProviderSubTypeCode],[ProviderSubTypeName])
     VALUES   (17 ,'RBT' ,'Registered Behavioral Aide')
INSERT INTO [dbo].[ProviderSubTypes] ([ProviderParentTypeID],[ProviderSubTypeCode],[ProviderSubTypeName])
     VALUES   (15 ,'LBA' ,'Licensed Behavior Analyst')
INSERT INTO [dbo].[ProviderSubTypes] ([ProviderParentTypeID],[ProviderSubTypeCode],[ProviderSubTypeName])
     VALUES   (15 ,'BCBA' ,'Board Certified Behavior Analyst')
INSERT INTO [dbo].[ProviderSubTypes] ([ProviderParentTypeID],[ProviderSubTypeCode],[ProviderSubTypeName])
     VALUES   (15 ,'COBA' ,'Certified Ohio Behavior Analyst')	 
	 
GO


GO
EXEC meta.UpdateVersion '2.2.8.0'
GO

