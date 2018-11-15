/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.3.2.0
dym:TargetEndingVersion: 2.3.3.0
---------------------------------------------------------------------

	Convert CaseNotes to Case and Referral notes
	Add more fields to Referrals and xfer to patients

---------------------------------------------------------------------*/


/* CaseNotes */
CREATE INDEX [IX_FollowupUserID] ON [dbo].[CaseNotes]([FollowupUserID])
CREATE INDEX [IX_EnteredByUserID] ON [dbo].[CaseNotes]([EnteredByUserID])
ALTER TABLE [dbo].[CaseNotes] ADD CONSTRAINT [FK_dbo.CaseNotes_dbo.WebUsers_EnteredByUserID] FOREIGN KEY ([EnteredByUserID]) REFERENCES [dbo].[WebUsers] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[CaseNotes] ADD CONSTRAINT [FK_dbo.CaseNotes_dbo.WebUsers_FollowupUserID] FOREIGN KEY ([FollowupUserID]) REFERENCES [dbo].[WebUsers] ([ID])
GO

/* CaseNoteTasks */
EXECUTE sp_rename @objname = N'dbo.CaseNoteTasks.AssignedTo', @newname = N'AssignedToStaffID', @objtype = N'COLUMN'
EXECUTE sp_rename @objname = N'dbo.CaseNoteTasks.CompletedBy', @newname = N'CompletedByUserID', @objtype = N'COLUMN'
--EXECUTE sp_rename @objname = N'dbo.CaseNoteTasks.IX_AssignedTo', @newname = N'IX_AssignedToStaffID', @objtype = N'INDEX'
--EXECUTE sp_rename @objname = N'dbo.CaseNoteTasks.IX_CompletedBy', @newname = N'IX_CompletedByUserID', @objtype = N'INDEX'
GO
--ALTER TABLE [dbo].[CaseNoteTasks] DROP CONSTRAINT [PK_dbo.CaseNoteTasks]
--ALTER TABLE [dbo].[CaseNoteTasks] ADD CONSTRAINT [PK_dbo.CaseNoteTasks] PRIMARY KEY ([ID], [NoteID])
--GO

/* ReferralNotes */
ALTER TABLE [dbo].[ReferralNotes] DROP CONSTRAINT [PK_dbo.ReferralNotes]
GO
ALTER TABLE [dbo].[ReferralNotes] ADD CONSTRAINT [PK_dbo.ReferralNotes] PRIMARY KEY ([ID])
GO
ALTER TABLE [dbo].[ReferralNotes] ADD [CorrespondenceType] [int]
ALTER TABLE [dbo].[ReferralNotes] ADD [CorrespondenceName] [nvarchar](max)
ALTER TABLE [dbo].[ReferralNotes] ADD [RequiresFollowup] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[ReferralNotes] ADD [FollowupDate] [datetime]
ALTER TABLE [dbo].[ReferralNotes] ADD [FollowupComplete] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[ReferralNotes] ADD [FollowupCompleteDate] [datetime]
ALTER TABLE [dbo].[ReferralNotes] ADD [FollowupComment] [nvarchar](max)
ALTER TABLE [dbo].[ReferralNotes] ADD [FollowupUserID] [int]
CREATE INDEX [IX_FollowupUserID] ON [dbo].[ReferralNotes]([FollowupUserID])
ALTER TABLE [dbo].[ReferralNotes] ADD CONSTRAINT [FK_dbo.ReferralNotes_dbo.WebUsers_FollowupUserID] FOREIGN KEY ([FollowupUserID]) REFERENCES [dbo].[WebUsers] ([ID])
GO

EXECUTE sp_rename @objname = N'dbo.ReferralNotes.Note', @newname = N'Comments', @objtype = N'COLUMN'
GO
EXECUTE sp_rename @objname = N'dbo.ReferralNotes.UserID', @newname = N'EnteredByUserID', @objtype = N'COLUMN'
GO
EXECUTE sp_rename @objname = N'dbo.ReferralNotes.IX_UserID', @newname = N'IX_EnteredByUserID', @objtype = N'INDEX'
GO

DECLARE @var0 nvarchar(128)
SELECT @var0 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.ReferralNotes')
AND col_name(parent_object_id, parent_column_id) = 'DateCreated';
IF @var0 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[ReferralNotes] DROP CONSTRAINT [' + @var0 + ']')
EXECUTE sp_rename @objname = N'dbo.ReferralNotes.DateCreated', @newname = N'EntryDate', @objtype = N'COLUMN'
ALTER TABLE [dbo].[ReferralNotes] ADD  DEFAULT (getdate()) FOR [EntryDate]
GO


/* CaseNoteTasks */
CREATE TABLE [dbo].[ReferralNoteTasks] (
    [ID] [int] NOT NULL IDENTITY,
    [NoteID] [int] NOT NULL,
    [Description] [nvarchar](max),
    [DueDate] [datetime],
    [Completed] [bit] NOT NULL,
    [CompletedDate] [datetime],
    [CompletedRemarks] [nvarchar](max),
    [AssignedToStaffID] [int],
    [CompletedByUserID] [int],
    CONSTRAINT [PK_dbo.ReferralNoteTasks] PRIMARY KEY ([ID], [NoteID])
)
CREATE INDEX [IX_NoteID] ON [dbo].[ReferralNoteTasks]([NoteID])
CREATE INDEX [IX_AssignedToStaffID] ON [dbo].[ReferralNoteTasks]([AssignedToStaffID])
CREATE INDEX [IX_CompletedByUserID] ON [dbo].[ReferralNoteTasks]([CompletedByUserID])
GO
ALTER TABLE [dbo].[ReferralNoteTasks] ADD CONSTRAINT [FK_dbo.ReferralNoteTasks_dbo.Staff_AssignedToStaffID] FOREIGN KEY ([AssignedToStaffID]) REFERENCES [dbo].[Staff] ([ID])
ALTER TABLE [dbo].[ReferralNoteTasks] ADD CONSTRAINT [FK_dbo.ReferralNoteTasks_dbo.WebUsers_CompletedByUserID] FOREIGN KEY ([CompletedByUserID]) REFERENCES [dbo].[WebUsers] ([ID])
ALTER TABLE [dbo].[ReferralNoteTasks] ADD CONSTRAINT [FK_dbo.ReferralNoteTasks_dbo.ReferralNotes_NoteID] FOREIGN KEY ([NoteID]) REFERENCES [dbo].[ReferralNotes] ([ID]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SessionSignatures] ADD [ParentName] [nvarchar](300)
ALTER TABLE [dbo].[SessionSignatures] ADD [ProviderName] [nvarchar](300)
GO




ALTER TABLE dbo.Referrals
ADD ReferralGuardianRelationshipID int NULL,
	ReferralGuardianEmail nvarchar(64) NULL,
	ReferralGuardianCellPhone nvarchar(30) NULL,
	ReferralGuardianHomePhone nvarchar(30) NULL,
	ReferralGuardianWorkPhone nvarchar(30) NULL,
	ReferralGuardianNotes nvarchar(1000) NULL,
	ReferralGuardian2FirstName nvarchar(64) NULL,
	ReferralGuardian2LastName nvarchar(64) NULL,
	ReferralGuardian2RelationshipID int NULL,
	ReferralGuardian2Email nvarchar(64) NULL,
	ReferralGuardian2CellPhone nvarchar(30) NULL,
	ReferralGuardian2HomePhone nvarchar(30) NULL,
	ReferralGuardian2WorkPhone nvarchar(30) NULL,
	ReferralGuardian2Notes nvarchar(1000) NULL,
	ReferralGuardian3FirstName nvarchar(64) NULL,
	ReferralGuardian3LastName nvarchar(64) NULL,
	ReferralGuardian3RelationshipID int NULL,
	ReferralGuardian3Email nvarchar(64) NULL,
	ReferralGuardian3CellPhone nvarchar(30) NULL,
	ReferralGuardian3HomePhone nvarchar(30) NULL,
	ReferralGuardian3WorkPhone nvarchar(30) NULL,
	ReferralGuardian3Notes nvarchar(1000) NULL;
GO

ALTER TABLE dbo.Referrals
ADD ReferralPhysicianName nvarchar(128) NULL,
	ReferralPhysicianAddress nvarchar(128) NULL,
	ReferralPhysicianPhone nvarchar(50) NULL,
	ReferralPhysicianFax nvarchar(50) NULL,
	ReferralPhysicianEmail nvarchar(128) NULL,
	ReferralPhysicianContact nvarchar(128) NULL,
	ReferralPhysicianNotes nvarchar(2000) NULL;
GO




GO
EXEC meta.UpdateVersion '2.3.3.0'
GO

