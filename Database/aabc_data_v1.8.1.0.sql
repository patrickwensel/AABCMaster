/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.0.0
dym:TargetEndingVersion: 1.8.1.0
---------------------------------------------------------------------

	PatientPortal Terms
	
---------------------------------------------------------------------*/





CREATE TABLE [dbo].[PatientPortalAcceptedTerms] (
    [LoginId] [int] NOT NULL,
    [TermsId] [int] NOT NULL,
    [Created] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.PatientPortalAcceptedTerms] PRIMARY KEY ([LoginId], [TermsId])
)
CREATE INDEX [IX_LoginId] ON [dbo].[PatientPortalAcceptedTerms]([LoginId])
CREATE INDEX [IX_TermsId] ON [dbo].[PatientPortalAcceptedTerms]([TermsId])
CREATE TABLE [dbo].[PatientPortalTerms] (
    [ID] [int] NOT NULL IDENTITY,
    [Text] [nvarchar](max),
    [Active] [bit] NOT NULL DEFAULT 1,
    [Created] [datetime] NOT NULL default GETDATE(),
    CONSTRAINT [PK_dbo.PatientPortalTerms] PRIMARY KEY ([ID])
)
ALTER TABLE [dbo].[PatientPortalAcceptedTerms] ADD CONSTRAINT [FK_dbo.PatientPortalAcceptedTerms_dbo.PatientPortalLogins_LoginId] FOREIGN KEY ([LoginId]) REFERENCES [dbo].[PatientPortalLogins] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[PatientPortalAcceptedTerms] ADD CONSTRAINT [FK_dbo.PatientPortalAcceptedTerms_dbo.PatientPortalTerms_TermsId] FOREIGN KEY ([TermsId]) REFERENCES [dbo].[PatientPortalTerms] ([ID]) ON DELETE CASCADE


GO

EXEC meta.UpdateVersion '1.8.1.0';
GO












