/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.2.1.0
dym:TargetEndingVersion: 2.2.2.0
---------------------------------------------------------------------

	Referrals
	
---------------------------------------------------------------------*/

CREATE TABLE [dbo].[ReferralNotes] (
    [ID] [int] NOT NULL IDENTITY,
    [ReferralID] [int] NOT NULL,
    [UserID] [int] NOT NULL,
    [DateCreated] [datetime] NOT NULL DEFAULT(GETDATE()),
    [Note] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_dbo.ReferralNotes] PRIMARY KEY ([ID], [ReferralID])
)
CREATE INDEX [IX_ReferralID] ON [dbo].[ReferralNotes]([ReferralID])
CREATE INDEX [IX_UserID] ON [dbo].[ReferralNotes]([UserID])
ALTER TABLE [dbo].[ReferralNotes] ADD CONSTRAINT [FK_dbo.ReferralNotes_dbo.Referrals_ReferralID] FOREIGN KEY ([ReferralID]) REFERENCES [dbo].[Referrals] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ReferralNotes] ADD CONSTRAINT [FK_dbo.ReferralNotes_dbo.WebUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[WebUsers] ([ID]) ON DELETE CASCADE
GO

CREATE TABLE [dbo].[ReferralEnumItems] (
    [ID] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL DEFAULT(GETDATE()),
    [StatusType] [varchar](20) NOT NULL,
    [Label] [nvarchar](50) NOT NULL,
    [Order] [int] NOT NULL,
    [ColorCode] [varchar](20),
    CONSTRAINT [PK_dbo.ReferralEnumItems] PRIMARY KEY ([ID])
)
GO

INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (1, 'InsuranceStatus', 'ABA covered OON', 1, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (2, 'InsuranceStatus', 'ABA covered INN', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (3, 'InsuranceStatus', 'ABA not covered  Medicaid', 3, 'red')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (4, 'InsuranceStatus', 'ABA not covered for OON', 4, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (5, 'InsuranceStatus', 'Needs SCA', 5, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (6, 'InsuranceStatus', 'Need corrected ins info from parent', 6, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (7, 'InsuranceStatus', 'Need parent to call optum to authorize us to call', 7, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (101, 'IntakeStatus', 'Need Intake', 1, 'red')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (102, 'IntakeStatus', 'Have Intake', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (201, 'RxStatus', 'Need RX', 1, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (202, 'RxStatus', 'Have RX', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (301, 'InsuranceCardStatus', 'Need Ins card', 1, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (302, 'InsuranceCardStatus', 'Have Ins card', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (401, 'EvaluationStatus', 'Need Full eval', 1, 'red')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (402, 'EvaluationStatus', 'Have Full eval', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (403, 'EvaluationStatus', 'Don’t need Full eval', 3, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (501, 'PolicyBookStatus', 'Need Policy book', 1, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (502, 'PolicyBookStatus', 'Have Policy book', 2, 'green')
INSERT INTO dbo.ReferralEnumItems  (ID, StatusType, Label, [Order], ColorCode) VALUES (503, 'PolicyBookStatus', 'Don’t need Policy Book', 3, 'green')
GO

ALTER TABLE dbo.Referrals ADD ReferralInsuranceStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralIntakeStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralRxStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralInsuranceCardStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralEvaluationStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralPolicyBookStatus INT NULL;
ALTER TABLE dbo.Referrals ADD ReferralBenefitCheck NVARCHAR(2000) NULL;
GO



GO
EXEC meta.UpdateVersion '2.2.2.0'
GO

