/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.7.0
dym:TargetEndingVersion: 1.8.8.0
---------------------------------------------------------------------

	Scheduling
	
	
---------------------------------------------------------------------*/





-- =======================
-- Scheduling
-- =======================

CREATE TABLE [dbo].[CaseProviderAppointments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[ProviderId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[RecurringAppointmentId] [int] NULL,
 CONSTRAINT [PK_dbo.CaseProviderAppointments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [IX_RecurringAppointmentId]    Script Date: 08/09/2017 12:12:51 p.m. ******/
CREATE NONCLUSTERED INDEX [IX_RecurringAppointmentId] ON [dbo].[CaseProviderAppointments]
(
	[RecurringAppointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CaseProviderAppointments]  WITH CHECK ADD  CONSTRAINT [FK_CaseProviderAppointments_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CaseProviderAppointments] CHECK CONSTRAINT [FK_CaseProviderAppointments_Cases]
GO

ALTER TABLE [dbo].[CaseProviderAppointments]  WITH CHECK ADD  CONSTRAINT [FK_CaseProviderAppointments_Providers] FOREIGN KEY([ProviderId])
REFERENCES [dbo].[Providers] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CaseProviderAppointments] CHECK CONSTRAINT [FK_CaseProviderAppointments_Providers]
GO

ALTER TABLE [dbo].[CaseProviderAppointments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CaseProviderAppointments_dbo.CaseProviderAppointments_RecurringAppointmentId] FOREIGN KEY([RecurringAppointmentId])
REFERENCES [dbo].[CaseProviderAppointments] ([Id])
GO

ALTER TABLE [dbo].[CaseProviderAppointments] CHECK CONSTRAINT [FK_dbo.CaseProviderAppointments_dbo.CaseProviderAppointments_RecurringAppointmentId]
GO






GO
EXEC meta.UpdateVersion '1.8.8.0';
GO

