/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.3.3.0
dym:TargetEndingVersion: 2.3.4.0
---------------------------------------------------------------------

	Add provider notes and tasks tracking

---------------------------------------------------------------------*/


CREATE TABLE [dbo].[ProviderNoteTasks] (
    [ID] [int] NOT NULL IDENTITY,
    [NoteID] [int] NOT NULL,
    [Description] [nvarchar](max),
    [DueDate] [datetime],
    [Completed] [bit] NOT NULL,
    [CompletedDate] [datetime],
    [CompletedRemarks] [nvarchar](max),
    [AssignedToStaffID] [int],
    [CompletedByUserID] [int],
    CONSTRAINT [PK_dbo.ProviderNoteTasks] PRIMARY KEY ([ID], [NoteID])
);
GO
CREATE INDEX [IX_NoteID] ON [dbo].[ProviderNoteTasks]([NoteID])
CREATE INDEX [IX_AssignedToStaffID] ON [dbo].[ProviderNoteTasks]([AssignedToStaffID])
CREATE INDEX [IX_CompletedByUserID] ON [dbo].[ProviderNoteTasks]([CompletedByUserID])
GO
CREATE TABLE [dbo].[ProviderNotes] (
    [ID] [int] NOT NULL IDENTITY,
    [ProviderID] [int] NOT NULL,
    [CorrespondenceType] [int],
    [CorrespondenceName] [nvarchar](max),
    [EntryDate] [datetime] NOT NULL,
    [RequiresFollowup] [bit] NOT NULL,
    [FollowupDate] [datetime],
    [Comments] [nvarchar](max),
    [FollowupComplete] [bit] NOT NULL,
    [FollowupCompleteDate] [datetime],
    [FollowupComment] [nvarchar](max),
    [FollowupUserID] [int],
    [EnteredByUserID] [int] NOT NULL,
    CONSTRAINT [PK_dbo.ProviderNotes] PRIMARY KEY ([ID])
)
GO
CREATE INDEX [IX_ProviderID] ON [dbo].[ProviderNotes]([ProviderID])
CREATE INDEX [IX_FollowupUserID] ON [dbo].[ProviderNotes]([FollowupUserID])
CREATE INDEX [IX_EnteredByUserID] ON [dbo].[ProviderNotes]([EnteredByUserID])
GO
ALTER TABLE [dbo].[ProviderNoteTasks] ADD CONSTRAINT [FK_dbo.ProviderNoteTasks_dbo.Staff_AssignedToStaffID] FOREIGN KEY ([AssignedToStaffID]) REFERENCES [dbo].[Staff] ([ID])
ALTER TABLE [dbo].[ProviderNoteTasks] ADD CONSTRAINT [FK_dbo.ProviderNoteTasks_dbo.WebUsers_CompletedByUserID] FOREIGN KEY ([CompletedByUserID]) REFERENCES [dbo].[WebUsers] ([ID])
ALTER TABLE [dbo].[ProviderNoteTasks] ADD CONSTRAINT [FK_dbo.ProviderNoteTasks_dbo.ProviderNotes_NoteID] FOREIGN KEY ([NoteID]) REFERENCES [dbo].[ProviderNotes] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ProviderNotes] ADD CONSTRAINT [FK_dbo.ProviderNotes_dbo.WebUsers_EnteredByUserID] FOREIGN KEY ([EnteredByUserID]) REFERENCES [dbo].[WebUsers] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ProviderNotes] ADD CONSTRAINT [FK_dbo.ProviderNotes_dbo.WebUsers_FollowupUserID] FOREIGN KEY ([FollowupUserID]) REFERENCES [dbo].[WebUsers] ([ID])
ALTER TABLE [dbo].[ProviderNotes] ADD CONSTRAINT [FK_dbo.ProviderNotes_dbo.Providers_ProviderID] FOREIGN KEY ([ProviderID]) REFERENCES [dbo].[Providers] ([ID]) ON DELETE CASCADE
GO


GO
EXEC meta.UpdateVersion '2.3.4.0'
GO

