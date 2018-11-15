/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 0.0.0.0
dym:TargetEndingVersion: 1.2.1.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/





/****** Object:  Table [dbo].[CaseHours]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseHours](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[CaseID] [int] NOT NULL,
	[ProviderID] [int] NULL,
	[HoursDate] [date] NOT NULL,
	[HoursTimeIn] [time](7) NOT NULL,
	[HoursTimeOut] [time](7) NOT NULL,
	[HoursPayable] [decimal](4, 2) NULL,
	[HoursBillable] [decimal](4, 2) NULL,
	[HoursAuthCodes] [nvarchar](256) NULL,
	[HoursAuthUnits] [int] NULL,
	[HoursNumberOfWeeks] [int] NULL,
	[HoursAvgWeeklyHoursAllowance] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CaseProviderNotes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseProviderNotes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[CaseProviderID] [int] NOT NULL,
	[ProviderNoteDate] [datetime2](7) NOT NULL,
	[ProviderNote] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CaseProviders]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseProviders](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[CaseID] [int] NOT NULL,
	[ProviderID] [int] NOT NULL,
	[Active] [bit] NOT NULL DEFAULT ((1)),
	[IsSupervisor] [bit] NOT NULL DEFAULT ((0)),
	[IsAssessor] [bit] NOT NULL DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Cases]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cases](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[PatientID] [int] NOT NULL,
	[CaseGeneratingReferralID] [int] NULL,
	[CaseStatus] [int] NOT NULL DEFAULT ((0)),
	[CaseStatusNotes] [nvarchar](1000) NULL,
	[CaseStartDate] [datetime2](7) NULL,
	[CaseAssignedStaffID] [int] NULL,
	[CaseRequiredHoursNotes] [nvarchar](1000) NULL,
	[CaseRequiredServicesNotes] [nvarchar](1000) NULL,
	[CaseHasPrescription] [bit] NOT NULL DEFAULT ((0)),
	[CaseHasAssessment] [bit] NOT NULL DEFAULT ((0)),
	[CaseHasAuthorization] [bit] NOT NULL,
	[CaseAuthorizationEndDate] [datetime2](7) NULL,
	[CaseHasIntake] [bit] NOT NULL DEFAULT ((0)),
	[CaseStatusReason] [int] NOT NULL CONSTRAINT [statusreasondefault]  DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CaseStatuses]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseStatuses](
	[ID] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CaseStatusReasons]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseStatusReasons](
	[ID] [int] NOT NULL,
	[StatusReason] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CaseTasks]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseTasks](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[CaseID] [int] NOT NULL,
	[TaskEnteredOn] [datetime2](7) NOT NULL,
	[TaskDescription] [nvarchar](255) NOT NULL,
	[TaskComplete] [bit] NOT NULL,
	[TaskCompletedDate] [datetime2](7) NULL,
	[TaskCompletedByStaffID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Insurances]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Insurances](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[InsuranceCode] [nvarchar](10) NOT NULL,
	[InsuranceName] [nvarchar](128) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Languages]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Languages](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[LangIsActive] [bit] NOT NULL DEFAULT ((1)),
	[LangBiblioCode] [nvarchar](3) NOT NULL,
	[LangTermCode] [nvarchar](3) NULL,
	[LangCommonCode] [nvarchar](2) NULL,
	[LangEnglishName] [nvarchar](255) NOT NULL,
	[LangFrenchName] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LanguageStrengthTypes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LanguageStrengthTypes](
	[ID] [int] NOT NULL,
	[TypeDescription] [nvarchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Patients]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patients](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[PatientGeneratingReferralID] [int] NULL,
	[PatientFirstName] [nvarchar](64) NOT NULL,
	[PatientLastName] [nvarchar](64) NOT NULL,
	[PatientDateOfBirth] [date] NULL,
	[PatientGender] [nvarchar](5) NULL,
	[PatientPrimarySpokenLangauge] [nvarchar](64) NULL,
	[PatientGuardianFirstName] [nvarchar](64) NULL,
	[PatientGuardianLastName] [nvarchar](64) NULL,
	[PatientGuardianRelationship] [nvarchar](64) NULL,
	[PatientEmail] [nvarchar](128) NULL,
	[PatientPhone] [nvarchar](30) NULL,
	[PatientAddress1] [nvarchar](255) NULL,
	[PatientAddress2] [nvarchar](255) NULL,
	[PatientCity] [nvarchar](255) NULL,
	[PatientState] [nvarchar](2) NULL,
	[PatientZip] [nvarchar](20) NULL,
	[PatientInsuranceCompanyName] [nvarchar](255) NULL,
	[PatientInsuranceMemberID] [nvarchar](64) NULL,
	[PatientInsurancePrimaryCardholderDateOfBirth] [date] NULL,
	[PatientInsuranceCompanyProviderPhone] [nvarchar](30) NULL,
	[PatientPhone2] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderInsuranceBlacklist]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderInsuranceBlacklist](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[ProviderID] [int] NOT NULL,
	[InsuranceID] [int] NOT NULL,
	[BlacklistReason] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderLanguages]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderLanguages](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[ProviderID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[LanguageStrength] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Providers]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Providers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ProviderType] [int] NULL,
	[ProviderFirstName] [nvarchar](64) NOT NULL,
	[ProviderLastName] [nvarchar](64) NOT NULL,
	[ProviderCompanyName] [nvarchar](255) NULL,
	[ProviderPrimaryPhone] [nvarchar](20) NULL,
	[ProviderPrimaryEmail] [nvarchar](64) NULL,
	[ProviderAddress1] [nvarchar](255) NULL,
	[ProviderAddress2] [nvarchar](255) NULL,
	[ProviderCity] [nvarchar](255) NULL,
	[ProviderState] [nvarchar](2) NULL,
	[ProviderZip] [nvarchar](20) NULL,
	[ProviderNPI] [nvarchar](30) NULL,
	[ProviderRate] [decimal](6, 2) NULL,
	[ProviderPhone2] [nvarchar](30) NULL,
	[ProviderFax] [nvarchar](30) NULL,
	[ProviderNotes] [nvarchar](1000) NULL,
	[ProviderAvailability] [nvarchar](256) NULL,
	[ProviderHasBackgroundCheck] [bit] NOT NULL DEFAULT ((0)),
	[ProviderHasReferences] [bit] NOT NULL DEFAULT ((0)),
	[ProviderHasResume] [bit] NOT NULL DEFAULT ((0)),
	[ProviderCanCall] [bit] NOT NULL DEFAULT ((0)),
	[ProviderCanReachByPhone] [bit] NOT NULL DEFAULT ((0)),
	[ProviderCanEmail] [bit] NOT NULL DEFAULT ((0)),
	[ProviderDocumentStatus] [nvarchar](500) NULL,
	[ProviderLBA] [nvarchar](50) NULL,
	[ProviderCertificationID] [nvarchar](50) NULL,
	[ProviderCertificationState] [nvarchar](20) NULL,
	[ProviderCertificationRenewalDate] [datetime2](7) NULL,
	[ProviderW9Date] [datetime2](7) NULL,
	[ProviderCAQH] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderServiceZipCodes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderServiceZipCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[ProviderID] [int] NOT NULL,
	[ZipCode] [nvarchar](5) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderTypes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ProviderTypeCode] [nvarchar](32) NOT NULL,
	[ProviderTypeName] [nvarchar](255) NULL,
	[ProviderTypeIsOutsourced] [bit] NOT NULL,
	[ProviderTypeCanSuperviseCase] [bit] NOT NULL CONSTRAINT [dfProvTypeCanSuper]  DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralChecklist]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralChecklist](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ReferralID] [int] NOT NULL,
	[ChecklistItemID] [int] NOT NULL,
	[ItemIsComplete] [bit] NOT NULL DEFAULT ((0)),
	[ItemCompletedByStaffID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralChecklistItems]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralChecklistItems](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ItemDescription] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralDismissalReasons]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralDismissalReasons](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ReasonCode] [nvarchar](10) NOT NULL,
	[ReasonName] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Referrals]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Referrals](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[ReferralFirstName] [nvarchar](64) NOT NULL,
	[ReferralLastName] [nvarchar](64) NOT NULL,
	[ReferralDateOfBirth] [date] NULL,
	[ReferralGender] [nvarchar](5) NULL,
	[ReferralPrimarySpokenLangauge] [nvarchar](64) NULL,
	[ReferralGuardianFirstName] [nvarchar](64) NULL,
	[ReferralGuardianLastName] [nvarchar](64) NULL,
	[ReferralGuardianRelationship] [nvarchar](64) NULL,
	[ReferralEmail] [nvarchar](128) NULL,
	[ReferralPhone] [nvarchar](40) NULL,
	[ReferralAddress1] [nvarchar](255) NULL,
	[ReferralAddress2] [nvarchar](255) NULL,
	[ReferralCity] [nvarchar](255) NULL,
	[ReferralState] [nvarchar](2) NULL,
	[ReferralZip] [nvarchar](20) NULL,
	[ReferralAreaOfConcern] [nvarchar](1000) NULL,
	[ReferralInsuranceCompanyName] [nvarchar](255) NULL,
	[ReferralInsuranceMemberID] [nvarchar](64) NULL,
	[ReferralInsurancePrimaryCardholderDateOfBirth] [date] NULL,
	[ReferralInsuranceCompanyProviderPhone] [nvarchar](30) NULL,
	[ReferralSourceType] [int] NULL,
	[ReferralSourceName] [nvarchar](255) NULL,
	[ReferralReferrerNotes] [nvarchar](2000) NULL,
	[ReferralStatus] [int] NOT NULL DEFAULT ((0)),
	[ReferralInsuranceVerificationResult] [nvarchar](255) NULL,
	[ReferralDismissalReasonID] [int] NULL,
	[ReferralDismissalReason] [nvarchar](128) NULL,
	[ReferralDismissalReasonNotes] [nvarchar](2000) NULL,
	[ReferralEnteredByStaffID] [int] NULL,
	[ReferralFollowup] [bit] NOT NULL DEFAULT ((0)),
	[ReferralFollowupDate] [date] NULL,
	[ReferralAssignedStaffID] [int] NULL,
	[ReferralGeneratedCaseID] [int] NULL,
	[ReferralGeneratedPatientID] [int] NULL,
	[ReferralServicesRequested] [nvarchar](75) NULL,
	[ReferralPrimaryCardholderName] [nvarchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralSettings]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralSettings](
	[ID] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[rv] [timestamp] NOT NULL,
	[SettingName] [nvarchar](64) NOT NULL,
	[SettingDescription] [nvarchar](1000) NULL,
	[SettingValue] [nvarchar](2000) NULL,
	[SettingValueType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralSourceTypes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralSourceTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[TypeName] [nvarchar](64) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReferralStatuses]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferralStatuses](
	[ID] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[StatusCode] [nvarchar](5) NOT NULL,
	[StatusName] [nvarchar](64) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SettingValueTypes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SettingValueTypes](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Staff]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staff](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[rv] [timestamp] NOT NULL,
	[StaffActive] [bit] NOT NULL DEFAULT ((1)),
	[StaffFirstName] [nvarchar](64) NOT NULL,
	[StaffLastName] [nvarchar](64) NOT NULL,
	[StaffPrimaryPhone] [nvarchar](30) NULL,
	[StaffPrimaryEmail] [nvarchar](128) NULL,
	[StaffHireDate] [date] NULL,
	[StaffTerminatedDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZipCodes]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZipCodes](
	[ZipCode] [nvarchar](5) NOT NULL,
	[ZipCity] [nvarchar](64) NULL,
	[ZipState] [nvarchar](2) NULL,
	[ZipType] [nvarchar](1) NULL,
	[ZipTimeZone] [int] NULL,
	[ZipDaylightSavings] [bit] NULL,
	[ZipLatitude] [decimal](20, 10) NULL,
	[ZipLongitude] [decimal](20, 10) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ZipCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [idxCaseProvidersActive]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxCaseProvidersActive] ON [dbo].[CaseProviders]
(
	[Active] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxCaseProvidersCase]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxCaseProvidersCase] ON [dbo].[CaseProviders]
(
	[CaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxCaseProvidersJuncIDs]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxCaseProvidersJuncIDs] ON [dbo].[CaseProviders]
(
	[CaseID] ASC,
	[ProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxCaseTasksEnteredOn]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxCaseTasksEnteredOn] ON [dbo].[CaseTasks]
(
	[TaskEnteredOn] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxInsuranceCodes]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxInsuranceCodes] ON [dbo].[Insurances]
(
	[InsuranceCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxPatientFirstName]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxPatientFirstName] ON [dbo].[Patients]
(
	[PatientFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxPatientLastName]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxPatientLastName] ON [dbo].[Patients]
(
	[PatientLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxPatientsZip]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxPatientsZip] ON [dbo].[Patients]
(
	[PatientZip] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxProviderInsuranceBlacklistProviderInsurance]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxProviderInsuranceBlacklistProviderInsurance] ON [dbo].[ProviderInsuranceBlacklist]
(
	[ProviderID] ASC,
	[InsuranceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxProviderLanguagesProviderLanguage]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxProviderLanguagesProviderLanguage] ON [dbo].[ProviderLanguages]
(
	[ProviderID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxProviderFirstName]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxProviderFirstName] ON [dbo].[Providers]
(
	[ProviderFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxProviderLastName]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxProviderLastName] ON [dbo].[Providers]
(
	[ProviderLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxProviderType]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxProviderType] ON [dbo].[Providers]
(
	[ProviderType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxProviderServiceZipCodesIsPrimary]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxProviderServiceZipCodesIsPrimary] ON [dbo].[ProviderServiceZipCodes]
(
	[IsPrimary] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxProviderZipCodeProviderZip]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxProviderZipCodeProviderZip] ON [dbo].[ProviderServiceZipCodes]
(
	[ProviderID] ASC,
	[ZipCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxReferralChecklistReferralID]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxReferralChecklistReferralID] ON [dbo].[ReferralChecklist]
(
	[ReferralID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxReferralChecklistRefferalItemJunc]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxReferralChecklistRefferalItemJunc] ON [dbo].[ReferralChecklist]
(
	[ReferralID] ASC,
	[ChecklistItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxReferralChecklistItemDescription]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxReferralChecklistItemDescription] ON [dbo].[ReferralChecklistItems]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxReferralDismissalReasonCode]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxReferralDismissalReasonCode] ON [dbo].[ReferralDismissalReasons]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxReferralDismissalReasonsCode]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxReferralDismissalReasonsCode] ON [dbo].[ReferralDismissalReasons]
(
	[ReasonCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxReferralStatusCode]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxReferralStatusCode] ON [dbo].[ReferralStatuses]
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idxSettingValueTypeName]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [idxSettingValueTypeName] ON [dbo].[SettingValueTypes]
(
	[TypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idxStaffActive]    Script Date: 4/3/2016 9:24:45 AM ******/
CREATE NONCLUSTERED INDEX [idxStaffActive] ON [dbo].[Staff]
(
	[StaffActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CaseProviderNotes] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[CaseTasks] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[CaseTasks] ADD  DEFAULT (getdate()) FOR [TaskEnteredOn]
GO
ALTER TABLE [dbo].[CaseTasks] ADD  DEFAULT ((0)) FOR [TaskComplete]
GO
ALTER TABLE [dbo].[Insurances] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ProviderInsuranceBlacklist] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ProviderLanguages] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ProviderLanguages] ADD  DEFAULT ((1)) FOR [LanguageStrength]
GO
ALTER TABLE [dbo].[ProviderServiceZipCodes] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ProviderServiceZipCodes] ADD  DEFAULT ((1)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[ReferralSettings] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[CaseHours]  WITH CHECK ADD FOREIGN KEY([CaseID])
REFERENCES [dbo].[Cases] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CaseHours]  WITH CHECK ADD FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[CaseProviderNotes]  WITH CHECK ADD FOREIGN KEY([CaseProviderID])
REFERENCES [dbo].[CaseProviders] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CaseProviders]  WITH CHECK ADD FOREIGN KEY([CaseID])
REFERENCES [dbo].[Cases] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CaseProviders]  WITH CHECK ADD FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cases]  WITH CHECK ADD FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patients] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CaseTasks]  WITH CHECK ADD FOREIGN KEY([CaseID])
REFERENCES [dbo].[Cases] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CaseTasks]  WITH CHECK ADD FOREIGN KEY([TaskCompletedByStaffID])
REFERENCES [dbo].[Staff] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ProviderInsuranceBlacklist]  WITH CHECK ADD FOREIGN KEY([InsuranceID])
REFERENCES [dbo].[Insurances] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProviderInsuranceBlacklist]  WITH CHECK ADD FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProviderLanguages]  WITH CHECK ADD FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Languages] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProviderLanguages]  WITH CHECK ADD FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Providers]  WITH CHECK ADD  CONSTRAINT [fkProviderProviderType] FOREIGN KEY([ProviderType])
REFERENCES [dbo].[ProviderTypes] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Providers] CHECK CONSTRAINT [fkProviderProviderType]
GO
ALTER TABLE [dbo].[ProviderServiceZipCodes]  WITH CHECK ADD FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProviderServiceZipCodes]  WITH CHECK ADD FOREIGN KEY([ZipCode])
REFERENCES [dbo].[ZipCodes] ([ZipCode])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReferralChecklist]  WITH CHECK ADD FOREIGN KEY([ChecklistItemID])
REFERENCES [dbo].[ReferralChecklistItems] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReferralChecklist]  WITH CHECK ADD  CONSTRAINT [FK__ReferralC__Refer__6C190EBB] FOREIGN KEY([ReferralID])
REFERENCES [dbo].[Referrals] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReferralChecklist] CHECK CONSTRAINT [FK__ReferralC__Refer__6C190EBB]
GO
ALTER TABLE [dbo].[Referrals]  WITH CHECK ADD FOREIGN KEY([ReferralSourceType])
REFERENCES [dbo].[ReferralSourceTypes] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Referrals]  WITH CHECK ADD FOREIGN KEY([ReferralDismissalReasonID])
REFERENCES [dbo].[ReferralDismissalReasons] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Referrals]  WITH CHECK ADD FOREIGN KEY([ReferralEnteredByStaffID])
REFERENCES [dbo].[Staff] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ReferralSettings]  WITH CHECK ADD FOREIGN KEY([SettingValueType])
REFERENCES [dbo].[SettingValueTypes] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
/****** Object:  StoredProcedure [dbo].[GetPatientSearchViewData]    Script Date: 4/3/2016 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
/* TEST DATA
DECLARE @ABATypeID INT
SET @ABATypeID = (SELECT ID FROM dbo.ProviderTypes WHERE ProviderTypeCode = 'AIDE')
-- */

SELECT
	c.ID,
	p.ID AS PatientID,
	p.PatientFirstName AS FirstName,
	p.PatientLastName AS LastName,
	p.PatientCity,
	p.PatientState,
	p.PatientZip AS Zip,
	c.CaseStatus AS Status,
	c.CaseStatusReason AS StatusReason,
	c.CaseStartDate AS StartDate,
	c.CaseAuthorizationEndDate AS EndingAuthDate,
	c.CaseHasPrescription AS HasPrescription,
	c.CaseHasAssessment AS HasAssessment,
	c.CaseHasIntake AS HasIntake,
	CASE WHEN sup.CaseID IS NOT NULL THEN 1 ELSE 0 END AS HasSupervisor,
	sup.ProviderID AS PrimaryBCBAID,
	sup.ProviderFirstName AS BCBAFirstName,
	sup.ProviderLastName AS BCBALastName,
	aide.ProviderID AS PrimaryAideID,
	aide.ProviderFirstName AS AideFirstName,
	aide.ProviderLastName AS AideLastName,
	p.PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName
		
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT 
			cp.CaseID,
			MAX(subP.ID) AS ProviderID			
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND cp.IsSupervisor = 1 
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS sup ON sup.CaseID = c.ID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT
			cp.CaseID,
			MIN(subP.ID) AS ProviderID		
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND subP.ProviderType = @ABATypeID
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS aide ON aide.CaseID = c.ID

LEFT JOIN dbo.Staff AS s ON s.ID = c.CaseAssignedStaffID

ORDER BY FirstName, LastName;

RETURN
END;
GO

 
/* (manually entered scripts for static data insertion) */
 
INSERT INTO [dbo].[CaseStatuses] ([ID], [Status]) VALUES (-1, N'History');
INSERT INTO [dbo].[CaseStatuses] ([ID], [Status]) VALUES (0, N'Not Ready');
INSERT INTO [dbo].[CaseStatuses] ([ID], [Status]) VALUES (1, N'Good');
INSERT INTO [dbo].[CaseStatuses] ([ID], [Status]) VALUES (2, N'Consider for Discharge');
GO


INSERT INTO [dbo].[LanguageStrengthTypes] ([ID], [TypeDescription]) VALUES (1, N'Native');
INSERT INTO [dbo].[LanguageStrengthTypes] ([ID], [TypeDescription]) VALUES (2, N'Adequate');
INSERT INTO [dbo].[LanguageStrengthTypes] ([ID], [TypeDescription]) VALUES (3, N'Poor');
GO


INSERT INTO [dbo].[ReferralStatuses] ([ID], [DateCreated], [StatusCode], [StatusName]) VALUES (0, '2016-01-31 21:51:51.1500000', N'N', N'New Referral');
INSERT INTO [dbo].[ReferralStatuses] ([ID], [DateCreated], [StatusCode], [StatusName]) VALUES (1, '2016-01-31 21:51:51.2030000', N'IP', N'In Process');
INSERT INTO [dbo].[ReferralStatuses] ([ID], [DateCreated], [StatusCode], [StatusName]) VALUES (2, '2016-01-31 21:51:51.2330000', N'D', N'Dismissed');
INSERT INTO [dbo].[ReferralStatuses] ([ID], [DateCreated], [StatusCode], [StatusName]) VALUES (3, '2016-01-31 21:51:51.2330000', N'A', N'Accepted');
GO

-- //// END MIGRATION SCRIPT v0.0.0.0-v0.1.3.0
































-- //// BEGIN MIGRATION SCRIPT v0.1.3.0-v1.0.0.0

/* ======================================================

		SCRIPTS v0.1.3.0 to v0.1.4.0
		
		Basic implementation of Case Auth Codes
		
   ====================================================== */



CREATE TABLE dbo.AuthCodes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	CodeCode NVARCHAR(20) NOT NULL,
	CodeDescription NVARCHAR(256)
);

CREATE UNIQUE INDEX idxAuthCodeCode ON dbo.AuthCodes(CodeCode);
GO


CREATE TABLE dbo.CaseAuthCodes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	CaseID INT NOT NULL REFERENCES dbo.Cases (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	AuthCodeID INT NOT NULL REFERENCES dbo.AuthCodes (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
GO
CREATE UNIQUE INDEX idxCaseAuthCodeCaseIDCodeID ON dbo.CaseAuthCodes (CaseID, AuthCodeID);
CREATE INDEX idxCaseAuthCodeCaseID ON dbo.CaseAuthCodes (CaseID);
CREATE INDEX idxCaseAuthCodeAuthCodeID ON dbo.CaseAuthCodes (AuthCodeID);
GO


-- Drop hours-specific authorization stuff
ALTER TABLE dbo.CaseHours DROP COLUMN HoursAuthCodes;
ALTER TABLE dbo.CaseHours DROP COLUMN HoursNumberOfWeeks;
ALTER TABLE dbo.CaseHours DROP COLUMN HoursAvgWeeklyHoursAllowance;
GO


-- //// END MIGRATION SCRIPT v0.1.3.0-v1.0.0.0
































-- //// BEGIN MIGRATION SCRIPT v1.0.0.0-v1.2.0.0

/* ======================================================

		SCRIPTS v1.0.0.0 to v1.2.0.0
		
		Primarily addresses Case Authorizations
		
   ====================================================== */


CREATE TABLE dbo.CaseAuthClasses (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AuthClassCode NVARCHAR(12) NOT NULL,
	AuthClassName NVARCHAR(50) NOT NULL,
	AuthClassDescription NVARCHAR(500)
);
CREATE UNIQUE INDEX idxCaseAuthClassCode ON dbo.CaseAuthClasses (AuthClassCode);
GO

INSERT INTO dbo.CaseAuthClasses (AuthClassCode, AuthClassName) VALUES (N'ASSESS', N'BCBA Assessment');
INSERT INTO dbo.CaseAuthClasses (AuthClassCode, AuthClassName) VALUES (N'TREAT', N'BCBA Treatment');
INSERT INTO dbo.CaseAuthClasses (AuthClassCode, AuthClassName) VALUES (N'GENERAL', N'Aide/General');
GO

-- ============================
-- Update the CaseAuthCodes to put auth specific info here
-- this will include Start/End dates and total hours allowed
-- as well as the auth class
-- ============================
ALTER TABLE dbo.CaseAuthCodes ADD AuthClassID INT NOT NULL REFERENCES dbo.CaseAuthClasses (ID) ON UPDATE CASCADE ON DELETE CASCADE;
ALTER TABLE dbo.CaseAuthCodes ADD AuthStartDate DATETIME2;
ALTER TABLE dbo.CaseAuthCodes ADD AuthEndDate DATETIME2;
ALTER TABLE dbo.CaseAuthCodes ADD AuthTotalHoursApproved INT NOT NULL;
GO


-- ============================
-- This is a "fudge it" table for a quick implementation of users to
-- add rough hours by month without requiring that all providers have
-- detailed hours which can be tallied.  This is a child of the CaseAuthCodes
-- table and is used for general display/quick snapshot information only
-- ============================
CREATE TABLE dbo.CaseAuthCodeGeneralHours (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CaseAuthID INT NOT NULL REFERENCES dbo.CaseAuthCodes (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	HoursYear INT NOT NULL,
	HoursMonth INT NOT NULL,
	HoursApplied INT NOT NULL
);
GO

-- ============================
-- Migrate CaseHours to CaseAuthHours
-- ============================
DROP TABLE dbo.CaseHours;	-- we can drop this completely, they haven't started using it yet
GO

CREATE TABLE dbo.CaseAuthHours (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CaseAuthID INT NOT NULL,
	CaseProviderID INT NOT NULL, -- soft reference REFERENCES dbo.CaseProviders (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	HoursDate DATE NOT NULL,
	HoursTimeIn TIME NOT NULL,
	HoursTimeOut TIME NOT NULL
);
GO


-- ============================
-- Update the patient list generation procedure
-- Pull EndingAuthDate from CaseAuths instead of Cases
-- ============================
DROP PROCEDURE dbo.GetPatientSearchViewData;
GO


CREATE PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
 /* TEST DATA
DECLARE @ABATypeID INT
SET @ABATypeID = (SELECT ID FROM dbo.ProviderTypes WHERE ProviderTypeCode = 'AIDE')
-- */

SELECT
	c.ID,
	p.ID AS PatientID,
	p.PatientFirstName AS FirstName,
	p.PatientLastName AS LastName,
	p.PatientCity,
	p.PatientState,
	p.PatientZip AS Zip,
	c.CaseStatus AS Status,
	c.CaseStatusReason AS StatusReason,
	c.CaseStartDate AS StartDate,
	auth.LatestEndDate AS EndingAuthDate,
	c.CaseHasPrescription AS HasPrescription,
	c.CaseHasAssessment AS HasAssessment,
	c.CaseHasIntake AS HasIntake,
	CASE WHEN sup.CaseID IS NOT NULL THEN 1 ELSE 0 END AS HasSupervisor,
	sup.ProviderID AS PrimaryBCBAID,
	sup.ProviderFirstName AS BCBAFirstName,
	sup.ProviderLastName AS BCBALastName,
	aide.ProviderID AS PrimaryAideID,
	aide.ProviderFirstName AS AideFirstName,
	aide.ProviderLastName AS AideLastName,
	p.PatientInsuranceCompanyName,
	s.ID AS AssignedStaffID,
	s.StaffFirstName AS AssignedStaffFirstName,
	s.StaffLastName AS AssignedStaffLastName
		
FROM dbo.Patients AS p
INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT 
			cp.CaseID,
			MAX(subP.ID) AS ProviderID			
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND cp.IsSupervisor = 1 
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS sup ON sup.CaseID = c.ID

LEFT JOIN (
	SELECT
		cac.CaseID,
		MAX(cac.AuthEndDate) AS LatestEndDate
	FROM dbo.CaseAuthCodes AS cac
	WHERE cac.AuthEndDate IS NOT NULL
	GROUP BY cac.CaseID
) AS auth ON c.ID = auth.CaseID

LEFT JOIN (
	SELECT base.CaseID, base.ProviderID, tmp.ProviderFirstName, tmp.ProviderLastName
	FROM (
		SELECT
			cp.CaseID,
			MIN(subP.ID) AS ProviderID		
		FROM dbo.CaseProviders AS cp 
		INNER JOIN dbo.Providers AS subP ON cp.ProviderID = subP.ID
		WHERE cp.Active = 1 AND subP.ProviderType = @ABATypeID
		GROUP BY cp.CaseID
	) AS base
	INNER JOIN dbo.Providers AS tmp ON base.ProviderID = tmp.ID
) AS aide ON aide.CaseID = c.ID

LEFT JOIN dbo.Staff AS s ON s.ID = c.CaseAssignedStaffID

ORDER BY FirstName, LastName;

RETURN
END;
GO



-- ============================
-- Migrate existing Case auth info into CaseAuths table
-- ============================

-- current live data has no codes on file
-- create a temp placeholder that they can reconfigure later
INSERT INTO dbo.AuthCodes (CodeCode, CodeDescription) VALUES  (N'0000', N'Temp/Placeholder Auth Code');
GO


-- insert relevant data pulled from Cases based on Start/End date
-- apply the temp/placeholder auth code and the general auth code class
DECLARE @AuthCodeID INT
DECLARE @AuthClassID INT
SET @AuthCodeID = (SELECT TOP 1 ID FROM dbo.AuthCodes WHERE CodeCode = N'0000');
SET @AuthClassID = (SELECT TOP 1 ID FROM dbo.CaseAuthClasses WHERE AuthClassCode = N'GENERAL');

INSERT INTO dbo.CaseAuthCodes (CaseID, AuthCodeID, AuthClassID, AuthStartDate, AuthEndDate)
SELECT 
	c.ID, 
	@AuthCodeID,
	@AuthClassID,
	c.CaseStartDate, 
	c.CaseAuthorizationEndDate 
FROM dbo.Cases AS c
WHERE c.CaseStartDate IS NOT NULL 
	OR c.CaseAuthorizationEndDate IS NOT NULL;
	
GO


-- ============================
-- Drop previous auths objects
-- ============================
ALTER TABLE dbo.Cases DROP COLUMN CaseAuthorizationEndDate;
GO

ALTER TABLE dbo.Cases DROP COLUMN CaseHasAuthorization;
GO

-- ============================
-- Reconfigure the unique index on CaseAuthCodes
-- ============================
DROP INDEX dbo.CaseAuthCodes.idxCaseAuthCodeCaseIDCodeID;
GO
CREATE UNIQUE INDEX idxCaseAuthCodeCaseCodeClass ON dbo.CaseAuthCodes (CaseID, AuthCodeID, AuthClassID);
GO



-- ============================
-- Create an UPSERT proc for GeneralHours
-- (tracking the hours ID is difficult through the UI implementation...
-- we'll just handle it on this end)
-- ============================
CREATE PROCEDURE dbo.UpsertGeneralHours(@CaseAuthID INT, @Year INT, @Month INT, @Hours INT) AS

	BEGIN TRANSACTION

		IF EXISTS(
			SELECT * 
			FROM dbo.CaseAuthCodeGeneralHours 
			WHERE CaseAuthID = @CaseAuthID
				AND HoursYear = @Year
				AND HoursMonth = @Month
			)
		
			BEGIN

				UPDATE dbo.CaseAuthCodeGeneralHours 
				SET HoursApplied = @Hours
				WHERE CaseAuthID = @CaseAuthID
					AND HoursYear = @Year
					AND HoursMonth = @Month

			END

		ELSE

			BEGIN

				INSERT INTO dbo.CaseAuthCodeGeneralHours
				        (CaseAuthID,
				         HoursYear,
				         HoursMonth,
				         HoursApplied
				        )
				VALUES  (@CaseAuthID, -- CaseAuthID - int
				         @Year, -- HoursYear - int
				         @Month, -- HoursMonth - int
				         @Hours  -- HoursApplied - int
				        )

			END

	COMMIT TRANSACTION

GO



-- //// END MIGRATION SCRIPT v1.0.0.0-v1.2.0.0


























-- //// BEGIN MIGRATION SCRIPT v1.2.0.0-v1.2.1.0

/* ======================================================

		SCRIPTS v1.2.0.0 to v1.2.1.0
		
   ====================================================== */


   
-- ============================
-- Tables for Users, Permissions, Options etc
-- ============================


CREATE TABLE dbo.WebPermissionGroups(
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	WebGroupDescription NVARCHAR(MAX) NOT NULL
);
GO

CREATE TABLE dbo.WebPermissions(
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	WebPermissionGroupID INT NULL FOREIGN KEY REFERENCES dbo.WebPermissionGroups(ID),
	WebPermissionDescription NVARCHAR(MAX)
);

CREATE TABLE dbo.WebUsers (
	ID int NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1,1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	AspNetUserID INT NOT NULL,
	StaffID INT NULL FOREIGN KEY REFERENCES dbo.Staff(ID),
	UserName NVARCHAR(128) NOT NULL, 
	WebUserFirstName NVARCHAR(64) NOT NULL,
	WebUserLastName NVARCHAR(64) NOT NULL,
	WebUserEmail NVARCHAR(128) NOT NULL
);
GO

CREATE TABLE dbo.WebUserPermissions(
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1,1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	WebUserID INT NOT NULL FOREIGN KEY REFERENCES dbo.WebUsers(ID),
	WebPermissionID INT NOT NULL FOREIGN KEY REFERENCES dbo.WebPermissions(ID),
);
GO

CREATE TABLE dbo.WebOptionGroups(
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	WebOptionDescription NVARCHAR(max) NOT NULL
);
GO

CREATE TABLE dbo.WebOptions(
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	WebOptionGroupID INT NULL FOREIGN KEY REFERENCES dbo.WebOptionGroups(ID),
	WebOptionDescription NVARCHAR(max),
);
GO

CREATE TABLE dbo.WebUserOptions(
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1,1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	WebUserID INT NOT NULL FOREIGN KEY REFERENCES dbo.WebUsers(ID),
	WebOptionID INT NULL FOREIGN KEY REFERENCES dbo.WebOptions(ID),
	WebOptionValue NVARCHAR(max),
);

GO

INSERT INTO dbo.WebPermissionGroups VALUES (1, 'Administrator');
INSERT INTO dbo.WebPermissionGroups VALUES (2, 'Staff');
GO

/* 
INSERT INTO dbo.WebUsers (AspNetUserID, StaffID, UserName, WebUserFirstName, WebUserLastName, WebUserEmail)
	VALUES (1, 1012, 'rgrant', 'Riley', 'Grant', 'riley.grant@gmail.com');
INSERT INTO dbo.WebUsers (AspNetUserID, StaffID, UserName, WebUserFirstName, WebUserLastName, WebUserEmail)
	VALUES (2, 1013, 'tester', 'Test', 'Testing', 'test@tested.com');
GO
 */


INSERT INTO dbo.WebPermissions VALUES (0, 1, 'UserManagement');
INSERT INTO dbo.WebPermissions VALUES (1, 1, 'ReferralDelete');
INSERT INTO dbo.WebPermissions VALUES (2, 1, 'PatientDelete');
INSERT INTO dbo.WebPermissions VALUES (3, 2, 'OfficeStaffDelete');
INSERT INTO dbo.WebPermissions VALUES (4, 2, 'ProviderHoursView');
INSERT INTO dbo.WebPermissions VALUES (5, 2, 'CaseHourView');
GO


/* 
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,0);
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,1);
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,2);
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,3);
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,4);
INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID) VALUES (1,5);
GO
 */

INSERT INTO dbo.WebOptionGroups VALUES (1, 'OptionGroup1');
INSERT INTO dbo.WebOptionGroups VALUES (2, 'OptionGroup2');
INSERT INTO dbo.WebOptionGroups VALUES (3, 'OptionGroup3');
GO


INSERT INTO dbo.WebOptions VALUES (1, 1, 'PatientSearchDefaultSort');
INSERT INTO dbo.WebOptions VALUES (2, 2, 'PatientSearchDefaultFilter');
GO

/* 

INSERT INTO dbo.WebUserOptions (WebUserID, WebOptionID) VALUES (1,1);
INSERT INTO dbo.WebUserOptions (WebUserID, WebOptionID) VALUES (1,2);
GO
 */
   
   
  
  /*
 
-- ============================
-- Setup an email holding table in a new service database
-- ============================

EXEC MANUAL -- CREATE DATABASE [AABC-Dev-Services];
EXEC MANUAL -- USE [AABC-Dev-Services]


CREATE TABLE dbo.IncomingEmailTypes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	TypeName NVARCHAR(50) NOT NULL
);
GO

INSERT INTO dbo.IncomingEmailTypes (ID, TypeName) VALUES (1, N'Referrals');
GO

CREATE TABLE dbo.IncomingEmails (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	EmailType INT NOT NULL REFERENCES dbo.IncomingEmailTypes (ID),
	EmailIsParsed BIT NOT NULL DEFAULT 0,
	EmailDateReceived DATETIME2 NOT NULL, -- see index
	EmailSubject NVARCHAR(500),
	EmailRecipients NVARCHAR(500),
	EmailBody NVARCHAR(MAX)
);
CREATE INDEX idxIncomingEmailTypeParsedDate ON dbo.IncomingEmails(EmailDateReceived DESC, EmailType, EmailIsParsed);
GO


EXEC MANUAL -- USE [AABC-Dev]

   
 */
 






-- ============================
-- Create Langauges table and view
-- ============================

CREATE TABLE [dbo].[ISO639_2_Lang](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ISO_639_2_Bibliographic] [nvarchar](50) NULL,
	[ISO_639_2_Terminology] [nvarchar](50) NULL,
	[ISO_639_1] [nvarchar](50) NULL,
	[EnglishName] [nvarchar](50) NULL,
	[FrenchName] [nvarchar](50) NULL
 CONSTRAINT [PK_ISO639_2_Lang] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ISO_639_1] ON [dbo].[ISO639_2_Lang]
(
	[ISO_639_1] ASC
)
WHERE [ISO_639_1] IS NOT NULL
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ISO_639_2_Bibliographic] ON [dbo].[ISO639_2_Lang]
(
	[ISO_639_2_Bibliographic] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ISO_639_2_Terminology] ON [dbo].[ISO639_2_Lang]
(
	[ISO_639_2_Terminology] ASC
)
WHERE [ISO_639_2_Terminology] IS NOT NULL
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO




INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('﻿aar',Null,'aa','Afar','afar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('abk',Null,'ab','Abkhazian','abkhaze')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ace',Null,Null,'Achinese','aceh')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ach',Null,Null,'Acoli','acoli')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ada',Null,Null,'Adangme','adangme')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ady',Null,Null,'Adyghe; Adygei','adyghé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('afa',Null,Null,'Afro-Asiatic languages','afro-asiatiques, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('afh',Null,Null,'Afrihili','afrihili')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('afr',Null,'af','Afrikaans','afrikaans')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ain',Null,Null,'Ainu','aïnou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('aka',Null,'ak','Akan','akan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('akk',Null,Null,'Akkadian','akkadien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('alb','sqi','sq','Albanian','albanais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ale',Null,Null,'Aleut','aléoute')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('alg',Null,Null,'Algonquian languages','algonquines, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('alt',Null,Null,'Southern Altai','altai du Sud')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('amh',Null,'am','Amharic','amharique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ang',Null,Null,'English, Old (ca.450-1100)','anglo-saxon (ca.450-1100)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('anp',Null,Null,'Angika','angika')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('apa',Null,Null,'Apache languages','apaches, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ara',Null,'ar','Arabic','arabe')
-- INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arc',Null,Null,'Official Aramaic (700-300 BCE); Imperial Aramaic (700-300 BCE)','araméen d''empire (700-300 BCE)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arg',Null,'an','Aragonese','aragonais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arm','hye','hy','Armenian','arménien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arn',Null,Null,'Mapudungun; Mapuche','mapudungun; mapuche; mapuce')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arp',Null,Null,'Arapaho','arapaho')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('art',Null,Null,'Artificial languages','artificielles, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('arw',Null,Null,'Arawak','arawak')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('asm',Null,'as','Assamese','assamais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ast',Null,Null,'Asturian; Bable; Leonese; Asturleonese','asturien; bable; léonais; asturoléonais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ath',Null,Null,'Athapascan languages','athapascanes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('aus',Null,Null,'Australian languages','australiennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ava',Null,'av','Avaric','avar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ave',Null,'ae','Avestan','avestique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('awa',Null,Null,'Awadhi','awadhi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('aym',Null,'ay','Aymara','aymara')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('aze',Null,'az','Azerbaijani','azéri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bad',Null,Null,'Banda languages','banda, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bai',Null,Null,'Bamileke languages','bamiléké, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bak',Null,'ba','Bashkir','bachkir')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bal',Null,Null,'Baluchi','baloutchi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bam',Null,'bm','Bambara','bambara')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ban',Null,Null,'Balinese','balinais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('baq','eus','eu','Basque','basque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bas',Null,Null,'Basa','basa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bat',Null,Null,'Baltic languages','baltes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bej',Null,Null,'Beja; Bedawiyet','bedja')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bel',Null,'be','Belarusian','biélorusse')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bem',Null,Null,'Bemba','bemba')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ben',Null,'bn','Bengali','bengali')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ber',Null,Null,'Berber languages','berbères, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bho',Null,Null,'Bhojpuri','bhojpuri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bih',Null,'bh','Bihari languages','langues biharis')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bik',Null,Null,'Bikol','bikol')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bin',Null,Null,'Bini; Edo','bini; edo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bis',Null,'bi','Bislama','bichlamar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bla',Null,Null,'Siksika','blackfoot')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bnt',Null,Null,'Bantu (Other)','bantoues, autres langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bos',Null,'bs','Bosnian','bosniaque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bra',Null,Null,'Braj','braj')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bre',Null,'br','Breton','breton')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('btk',Null,Null,'Batak languages','batak, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bua',Null,Null,'Buriat','bouriate')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bug',Null,Null,'Buginese','bugi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bul',Null,'bg','Bulgarian','bulgare')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('bur','mya','my','Burmese','birman')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('byn',Null,Null,'Blin; Bilin','blin; bilen')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cad',Null,Null,'Caddo','caddo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cai',Null,Null,'Central American Indian languages','amérindiennes de L''Amérique centrale, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('car',Null,Null,'Galibi Carib','karib; galibi; carib')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cat',Null,'ca','Catalan; Valencian','catalan; valencien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cau',Null,Null,'Caucasian languages','caucasiennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ceb',Null,Null,'Cebuano','cebuano')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cel',Null,Null,'Celtic languages','celtiques, langues; celtes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cha',Null,'ch','Chamorro','chamorro')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chb',Null,Null,'Chibcha','chibcha')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('che',Null,'ce','Chechen','tchétchène')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chg',Null,Null,'Chagatai','djaghataï')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chi','zho','zh','Chinese','chinois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chk',Null,Null,'Chuukese','chuuk')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chm',Null,Null,'Mari','mari')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chn',Null,Null,'Chinook jargon','chinook, jargon')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cho',Null,Null,'Choctaw','choctaw')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chp',Null,Null,'Chipewyan; Dene Suline','chipewyan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chr',Null,Null,'Cherokee','cherokee')
--INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chu',Null,'cu','Church Slavic; Old Slavonic; Church Slavonic; Old Bulgarian; Old Church Slavonic','slavon d''église; vieux slave; slavon liturgique; vieux bulgare')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chv',Null,'cv','Chuvash','tchouvache')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('chy',Null,Null,'Cheyenne','cheyenne')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cmc',Null,Null,'Chamic languages','chames, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cop',Null,Null,'Coptic','copte')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cor',Null,'kw','Cornish','cornique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cos',Null,'co','Corsican','corse')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cpe',Null,Null,'Creoles and pidgins, English based','créoles et pidgins basés sur l''anglais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cpf',Null,Null,'Creoles and pidgins, French-based','créoles et pidgins basés sur le français')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cpp',Null,Null,'Creoles and pidgins, Portuguese-based','créoles et pidgins basés sur le portugais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cre',Null,'cr','Cree','cree')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('crh',Null,Null,'Crimean Tatar; Crimean Turkish','tatar de Crimé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('crp',Null,Null,'Creoles and pidgins','créoles et pidgins')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('csb',Null,Null,'Kashubian','kachoube')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cus',Null,Null,'Cushitic languages','couchitiques, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('cze','ces','cs','Czech','tchèque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dak',Null,Null,'Dakota','dakota')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dan',Null,'da','Danish','danois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dar',Null,Null,'Dargwa','dargwa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('day',Null,Null,'Land Dayak languages','dayak, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('del',Null,Null,'Delaware','delaware')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('den',Null,Null,'Slave (Athapascan)','esclave (athapascan)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dgr',Null,Null,'Dogrib','dogrib')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('din',Null,Null,'Dinka','dinka')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('div',Null,'dv','Divehi; Dhivehi; Maldivian','maldivien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('doi',Null,Null,'Dogri','dogri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dra',Null,Null,'Dravidian languages','dravidiennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dsb',Null,Null,'Lower Sorbian','bas-sorabe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dua',Null,Null,'Duala','douala')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dum',Null,Null,'Dutch, Middle (ca.1050-1350)','néerlandais moyen (ca. 1050-1350)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dut','nld','nl','Dutch; Flemish','néerlandais; flamand')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dyu',Null,Null,'Dyula','dioula')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('dzo',Null,'dz','Dzongkha','dzongkha')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('efi',Null,Null,'Efik','efik')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('egy',Null,Null,'Egyptian (Ancient)','égyptien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('eka',Null,Null,'Ekajuk','ekajuk')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('elx',Null,Null,'Elamite','élamite')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('eng',Null,'en','English','anglais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('enm',Null,Null,'English, Middle (1100-1500)','anglais moyen (1100-1500)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('epo',Null,'eo','Esperanto','espéranto')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('est',Null,'et','Estonian','estonien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ewe',Null,'ee','Ewe','éwé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ewo',Null,Null,'Ewondo','éwondo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fan',Null,Null,'Fang','fang')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fao',Null,'fo','Faroese','féroïen')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fat',Null,Null,'Fanti','fanti')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fij',Null,'fj','Fijian','fidjien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fil',Null,Null,'Filipino; Pilipino','filipino; pilipino')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fin',Null,'fi','Finnish','finnois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fiu',Null,Null,'Finno-Ugrian languages','finno-ougriennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fon',Null,Null,'Fon','fon')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fre','fra','fr','French','français')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('frm',Null,Null,'French, Middle (ca.1400-1600)','français moyen (1400-1600)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fro',Null,Null,'French, Old (842-ca.1400)','français ancien (842-ca.1400)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('frr',Null,Null,'Northern Frisian','frison septentrional')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('frs',Null,Null,'Eastern Frisian','frison oriental')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fry',Null,'fy','Western Frisian','frison occidental')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ful',Null,'ff','Fulah','peul')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('fur',Null,Null,'Friulian','frioulan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gaa',Null,Null,'Ga','ga')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gay',Null,Null,'Gayo','gayo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gba',Null,Null,'Gbaya','gbaya')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gem',Null,Null,'Germanic languages','germaniques, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('geo','kat','ka','Georgian','géorgien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ger','deu','de','German','allemand')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gez',Null,Null,'Geez','guèze')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gil',Null,Null,'Gilbertese','kiribati')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gla',Null,'gd','Gaelic; Scottish Gaelic','gaélique; gaélique écossais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gle',Null,'ga','Irish','irlandais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('glg',Null,'gl','Galician','galicien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('glv',Null,'gv','Manx','manx; mannois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gmh',Null,Null,'German, Middle High (ca.1050-1500)','allemand, moyen haut (ca. 1050-1500)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('goh',Null,Null,'German, Old High (ca.750-1050)','allemand, vieux haut (ca. 750-1050)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gon',Null,Null,'Gondi','gond')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gor',Null,Null,'Gorontalo','gorontalo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('got',Null,Null,'Gothic','gothique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('grb',Null,Null,'Grebo','grebo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('grc',Null,Null,'Greek, Ancient (to 1453)','grec ancien (jusqu''à 1453)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gre','ell','el','Greek, Modern (1453-)','grec moderne (après 1453)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('grn',Null,'gn','Guarani','guarani')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gsw',Null,Null,'Swiss German; Alemannic; Alsatian','suisse alémanique; alémanique; alsacien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('guj',Null,'gu','Gujarati','goudjrati')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('gwi',Null,Null,'Gwich''in','gwich''in')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hai',Null,Null,'Haida','haida')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hat',Null,'ht','Haitian; Haitian Creole','haïtien; créole haïtien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hau',Null,'ha','Hausa','haoussa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('haw',Null,Null,'Hawaiian','hawaïen')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('heb',Null,'he','Hebrew','hébreu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('her',Null,'hz','Herero','herero')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hil',Null,Null,'Hiligaynon','hiligaynon')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('him',Null,Null,'Himachali languages; Western Pahari languages','langues himachalis; langues paharis occidentales')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hin',Null,'hi','Hindi','hindi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hit',Null,Null,'Hittite','hittite')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hmn',Null,Null,'Hmong; Mong','hmong')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hmo',Null,'ho','Hiri Motu','hiri motu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hrv',Null,'hr','Croatian','croate')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hsb',Null,Null,'Upper Sorbian','haut-sorabe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hun',Null,'hu','Hungarian','hongrois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('hup',Null,Null,'Hupa','hupa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('iba',Null,Null,'Iban','iban')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ibo',Null,'ig','Igbo','igbo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ice','isl','is','Icelandic','islandais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ido',Null,'io','Ido','ido')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('iii',Null,'ii','Sichuan Yi; Nuosu','yi de Sichuan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ijo',Null,Null,'Ijo languages','ijo, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('iku',Null,'iu','Inuktitut','inuktitut')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ile',Null,'ie','Interlingue; Occidental','interlingue')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ilo',Null,Null,'Iloko','ilocano')
--INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ina',Null,'ia','Interlingua (International Auxiliary Language Association)','interlingua (langue auxiliaire internationale)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('inc',Null,Null,'Indic languages','indo-aryennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ind',Null,'id','Indonesian','indonésien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ine',Null,Null,'Indo-European languages','indo-européennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('inh',Null,Null,'Ingush','ingouche')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ipk',Null,'ik','Inupiaq','inupiaq')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ira',Null,Null,'Iranian languages','iraniennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('iro',Null,Null,'Iroquoian languages','iroquoises, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ita',Null,'it','Italian','italien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('jav',Null,'jv','Javanese','javanais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('jbo',Null,Null,'Lojban','lojban')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('jpn',Null,'ja','Japanese','japonais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('jpr',Null,Null,'Judeo-Persian','judéo-persan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('jrb',Null,Null,'Judeo-Arabic','judéo-arabe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kaa',Null,Null,'Kara-Kalpak','karakalpak')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kab',Null,Null,'Kabyle','kabyle')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kac',Null,Null,'Kachin; Jingpho','kachin; jingpho')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kal',Null,'kl','Kalaallisut; Greenlandic','groenlandais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kam',Null,Null,'Kamba','kamba')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kan',Null,'kn','Kannada','kannada')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kar',Null,Null,'Karen languages','karen, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kas',Null,'ks','Kashmiri','kashmiri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kau',Null,'kr','Kanuri','kanouri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kaw',Null,Null,'Kawi','kawi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kaz',Null,'kk','Kazakh','kazakh')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kbd',Null,Null,'Kabardian','kabardien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kha',Null,Null,'Khasi','khasi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('khi',Null,Null,'Khoisan languages','khoïsan, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('khm',Null,'km','Central Khmer','khmer central')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kho',Null,Null,'Khotanese; Sakan','khotanais; sakan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kik',Null,'ki','Kikuyu; Gikuyu','kikuyu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kin',Null,'rw','Kinyarwanda','rwanda')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kir',Null,'ky','Kirghiz; Kyrgyz','kirghiz')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kmb',Null,Null,'Kimbundu','kimbundu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kok',Null,Null,'Konkani','konkani')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kom',Null,'kv','Komi','kom')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kon',Null,'kg','Kongo','kongo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kor',Null,'ko','Korean','coréen')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kos',Null,Null,'Kosraean','kosrae')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kpe',Null,Null,'Kpelle','kpellé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('krc',Null,Null,'Karachay-Balkar','karatchai balkar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('krl',Null,Null,'Karelian','carélien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kro',Null,Null,'Kru languages','krou, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kru',Null,Null,'Kurukh','kurukh')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kua',Null,'kj','Kuanyama; Kwanyama','kuanyama; kwanyama')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kum',Null,Null,'Kumyk','koumyk')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kur',Null,'ku','Kurdish','kurde')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('kut',Null,Null,'Kutenai','kutenai')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lad',Null,Null,'Ladino','judéo-espagnol')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lah',Null,Null,'Lahnda','lahnda')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lam',Null,Null,'Lamba','lamba')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lao',Null,'lo','Lao','lao')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lat',Null,'la','Latin','latin')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lav',Null,'lv','Latvian','letton')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lez',Null,Null,'Lezghian','lezghien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lim',Null,'li','Limburgan; Limburger; Limburgish','limbourgeois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lin',Null,'ln','Lingala','lingala')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lit',Null,'lt','Lithuanian','lituanien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lol',Null,Null,'Mongo','mongo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('loz',Null,Null,'Lozi','lozi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ltz',Null,'lb','Luxembourgish; Letzeburgesch','luxembourgeois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lua',Null,Null,'Luba-Lulua','luba-lulua')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lub',Null,'lu','Luba-Katanga','luba-katanga')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lug',Null,'lg','Ganda','ganda')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lui',Null,Null,'Luiseno','luiseno')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lun',Null,Null,'Lunda','lunda')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('luo',Null,Null,'Luo (Kenya and Tanzania)','luo (Kenya et Tanzanie)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('lus',Null,Null,'Lushai','lushai')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mac','mkd','mk','Macedonian','macédonien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mad',Null,Null,'Madurese','madourais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mag',Null,Null,'Magahi','magahi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mah',Null,'mh','Marshallese','marshall')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mai',Null,Null,'Maithili','maithili')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mak',Null,Null,'Makasar','makassar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mal',Null,'ml','Malayalam','malayalam')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('man',Null,Null,'Mandingo','mandingue')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mao','mri','mi','Maori','maori')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('map',Null,Null,'Austronesian languages','austronésiennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mar',Null,'mr','Marathi','marathe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mas',Null,Null,'Masai','massaï')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('may','msa','ms','Malay','malais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mdf',Null,Null,'Moksha','moksa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mdr',Null,Null,'Mandar','mandar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('men',Null,Null,'Mende','mendé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mga',Null,Null,'Irish, Middle (900-1200)','irlandais moyen (900-1200)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mic',Null,Null,'Mi''kmaq; Micmac','mi''kmaq; micmac')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('min',Null,Null,'Minangkabau','minangkabau')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mis',Null,Null,'Uncoded languages','langues non codées')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mkh',Null,Null,'Mon-Khmer languages','môn-khmer, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mlg',Null,'mg','Malagasy','malgache')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mlt',Null,'mt','Maltese','maltais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mnc',Null,Null,'Manchu','mandchou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mni',Null,Null,'Manipuri','manipuri')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mno',Null,Null,'Manobo languages','manobo, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('moh',Null,Null,'Mohawk','mohawk')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mon',Null,'mn','Mongolian','mongol')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mos',Null,Null,'Mossi','moré')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mul',Null,Null,'Multiple languages','multilingue')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mun',Null,Null,'Munda languages','mounda, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mus',Null,Null,'Creek','muskogee')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mwl',Null,Null,'Mirandese','mirandais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('mwr',Null,Null,'Marwari','marvari')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('myn',Null,Null,'Mayan languages','maya, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('myv',Null,Null,'Erzya','erza')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nah',Null,Null,'Nahuatl languages','nahuatl, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nai',Null,Null,'North American Indian languages','nord-amérindiennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nap',Null,Null,'Neapolitan','napolitain')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nau',Null,'na','Nauru','nauruan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nav',Null,'nv','Navajo; Navaho','navaho')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nbl',Null,'nr','Ndebele, South; South Ndebele','ndébélé du Sud')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nde',Null,'nd','Ndebele, North; North Ndebele','ndébélé du Nord')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ndo',Null,'ng','Ndonga','ndonga')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nds',Null,Null,'Low German; Low Saxon; German, Low; Saxon, Low','bas allemand; bas saxon; allemand, bas; saxon, bas')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nep',Null,'ne','Nepali','népalais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('new',Null,Null,'Nepal Bhasa; Newari','nepal bhasa; newari')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nia',Null,Null,'Nias','nias')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nic',Null,Null,'Niger-Kordofanian languages','nigéro-kordofaniennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('niu',Null,Null,'Niuean','niué')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nno',Null,'nn','Norwegian Nynorsk; Nynorsk, Norwegian','norvégien nynorsk; nynorsk, norvégien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nob',Null,'nb','Bokmål, Norwegian; Norwegian Bokmål','norvégien bokmål')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nog',Null,Null,'Nogai','nogaï; nogay')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('non',Null,Null,'Norse, Old','norrois, vieux')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nor',Null,'no','Norwegian','norvégien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nqo',Null,Null,'N''Ko','n''ko')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nso',Null,Null,'Pedi; Sepedi; Northern Sotho','pedi; sepedi; sotho du Nord')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nub',Null,Null,'Nubian languages','nubiennes, langues')
--INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nwc',Null,Null,'Classical Newari; Old Newari; Classical Nepal Bhasa','newari classique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nya',Null,'ny','Chichewa; Chewa; Nyanja','chichewa; chewa; nyanja')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nym',Null,Null,'Nyamwezi','nyamwezi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nyn',Null,Null,'Nyankole','nyankolé')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nyo',Null,Null,'Nyoro','nyoro')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('nzi',Null,Null,'Nzima','nzema')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('oci',Null,'oc','Occitan (post 1500); Provençal','occitan (après 1500); provençal')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('oji',Null,'oj','Ojibwa','ojibwa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ori',Null,'or','Oriya','oriya')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('orm',Null,'om','Oromo','galla')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('osa',Null,Null,'Osage','osage')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('oss',Null,'os','Ossetian; Ossetic','ossète')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ota',Null,Null,'Turkish, Ottoman (1500-1928)','turc ottoman (1500-1928)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('oto',Null,Null,'Otomian languages','otomi, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('paa',Null,Null,'Papuan languages','papoues, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pag',Null,Null,'Pangasinan','pangasinan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pal',Null,Null,'Pahlavi','pahlavi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pam',Null,Null,'Pampanga; Kapampangan','pampangan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pan',Null,'pa','Panjabi; Punjabi','pendjabi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pap',Null,Null,'Papiamento','papiamento')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pau',Null,Null,'Palauan','palau')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('peo',Null,Null,'Persian, Old (ca.600-400 B.C.)','perse, vieux (ca. 600-400 av. J.-C.)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('per','fas','fa','Persian','persan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('phi',Null,Null,'Philippine languages','philippines, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('phn',Null,Null,'Phoenician','phénicien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pli',Null,'pi','Pali','pali')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pol',Null,'pl','Polish','polonais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pon',Null,Null,'Pohnpeian','pohnpei')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('por',Null,'pt','Portuguese','portugais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pra',Null,Null,'Prakrit languages','prâkrit, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pro',Null,Null,'Provençal, Old (to 1500)','provençal ancien (jusqu''à 1500)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('pus',Null,'ps','Pushto; Pashto','pachto')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('qaa-qtz',Null,Null,'Reserved for local use','réservée à l''usage local')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('que',Null,'qu','Quechua','quechua')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('raj',Null,Null,'Rajasthani','rajasthani')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rap',Null,Null,'Rapanui','rapanui')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rar',Null,Null,'Rarotongan; Cook Islands Maori','rarotonga; maori des îles Cook')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('roa',Null,Null,'Romance languages','romanes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('roh',Null,'rm','Romansh','romanche')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rom',Null,Null,'Romany','tsigane')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rum','ron','ro','Romanian; Moldavian; Moldovan','roumain; moldave')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('run',Null,'rn','Rundi','rundi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rup',Null,Null,'Aromanian; Arumanian; Macedo-Romanian','aroumain; macédo-roumain')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('rus',Null,'ru','Russian','russe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sad',Null,Null,'Sandawe','sandawe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sag',Null,'sg','Sango','sango')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sah',Null,Null,'Yakut','iakoute')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sai',Null,Null,'South American Indian (Other)','indiennes d''Amérique du Sud, autres langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sal',Null,Null,'Salishan languages','salishennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sam',Null,Null,'Samaritan Aramaic','samaritain')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('san',Null,'sa','Sanskrit','sanskrit')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sas',Null,Null,'Sasak','sasak')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sat',Null,Null,'Santali','santal')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('scn',Null,Null,'Sicilian','sicilien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sco',Null,Null,'Scots','écossais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sel',Null,Null,'Selkup','selkoupe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sem',Null,Null,'Semitic languages','sémitiques, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sga',Null,Null,'Irish, Old (to 900)','irlandais ancien (jusqu''à 900)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sgn',Null,Null,'Sign Languages','langues des signes')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('shn',Null,Null,'Shan','chan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sid',Null,Null,'Sidamo','sidamo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sin',Null,'si','Sinhala; Sinhalese','singhalais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sio',Null,Null,'Siouan languages','sioux, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sit',Null,Null,'Sino-Tibetan languages','sino-tibétaines, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sla',Null,Null,'Slavic languages','slaves, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('slo','slk','sk','Slovak','slovaque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('slv',Null,'sl','Slovenian','slovène')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sma',Null,Null,'Southern Sami','sami du Sud')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sme',Null,'se','Northern Sami','sami du Nord')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('smi',Null,Null,'Sami languages','sames, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('smj',Null,Null,'Lule Sami','sami de Lule')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('smn',Null,Null,'Inari Sami','sami d''Inari')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('smo',Null,'sm','Samoan','samoan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sms',Null,Null,'Skolt Sami','sami skolt')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sna',Null,'sn','Shona','shona')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('snd',Null,'sd','Sindhi','sindhi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('snk',Null,Null,'Soninke','soninké')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sog',Null,Null,'Sogdian','sogdien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('som',Null,'so','Somali','somali')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('son',Null,Null,'Songhai languages','songhai, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sot',Null,'st','Sotho, Southern','sotho du Sud')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('spa',Null,'es','Spanish; Castilian','espagnol; castillan')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('srd',Null,'sc','Sardinian','sarde')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('srn',Null,Null,'Sranan Tongo','sranan tongo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('srp',Null,'sr','Serbian','serbe')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('srr',Null,Null,'Serer','sérère')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ssa',Null,Null,'Nilo-Saharan languages','nilo-sahariennes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ssw',Null,'ss','Swati','swati')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('suk',Null,Null,'Sukuma','sukuma')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sun',Null,'su','Sundanese','soundanais')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sus',Null,Null,'Susu','soussou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('sux',Null,Null,'Sumerian','sumérien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('swa',Null,'sw','Swahili','swahili')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('swe',Null,'sv','Swedish','suédois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('syc',Null,Null,'Classical Syriac','syriaque classique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('syr',Null,Null,'Syriac','syriaque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tah',Null,'ty','Tahitian','tahitien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tai',Null,Null,'Tai languages','tai, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tam',Null,'ta','Tamil','tamoul')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tat',Null,'tt','Tatar','tatar')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tel',Null,'te','Telugu','télougou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tem',Null,Null,'Timne','temne')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ter',Null,Null,'Tereno','tereno')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tet',Null,Null,'Tetum','tetum')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tgk',Null,'tg','Tajik','tadjik')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tgl',Null,'tl','Tagalog','tagalog')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tha',Null,'th','Thai','thaï')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tib','bod','bo','Tibetan','tibétain')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tig',Null,Null,'Tigre','tigré')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tir',Null,'ti','Tigrinya','tigrigna')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tiv',Null,Null,'Tiv','tiv')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tkl',Null,Null,'Tokelau','tokelau')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tlh',Null,Null,'Klingon; tlhIngan-Hol','klingon')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tli',Null,Null,'Tlingit','tlingit')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tmh',Null,Null,'Tamashek','tamacheq')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tog',Null,Null,'Tonga (Nyasa)','tonga (Nyasa)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ton',Null,'to','Tonga (Tonga Islands)','tongan (Îles Tonga)')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tpi',Null,Null,'Tok Pisin','tok pisin')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tsi',Null,Null,'Tsimshian','tsimshian')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tsn',Null,'tn','Tswana','tswana')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tso',Null,'ts','Tsonga','tsonga')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tuk',Null,'tk','Turkmen','turkmène')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tum',Null,Null,'Tumbuka','tumbuka')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tup',Null,Null,'Tupi languages','tupi, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tur',Null,'tr','Turkish','turc')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tut',Null,Null,'Altaic languages','altaïques, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tvl',Null,Null,'Tuvalu','tuvalu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('twi',Null,'tw','Twi','twi')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('tyv',Null,Null,'Tuvinian','touva')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('udm',Null,Null,'Udmurt','oudmourte')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('uga',Null,Null,'Ugaritic','ougaritique')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('uig',Null,'ug','Uighur; Uyghur','ouïgour')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ukr',Null,'uk','Ukrainian','ukrainien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('umb',Null,Null,'Umbundu','umbundu')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('und',Null,Null,'Undetermined','indéterminée')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('urd',Null,'ur','Urdu','ourdou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('uzb',Null,'uz','Uzbek','ouszbek')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('vai',Null,Null,'Vai','vaï')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ven',Null,'ve','Venda','venda')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('vie',Null,'vi','Vietnamese','vietnamien')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('vol',Null,'vo','Volapük','volapük')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('vot',Null,Null,'Votic','vote')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wak',Null,Null,'Wakashan languages','wakashanes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wal',Null,Null,'Walamo','walamo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('war',Null,Null,'Waray','waray')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('was',Null,Null,'Washo','washo')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wel','cym','cy','Welsh','gallois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wen',Null,Null,'Sorbian languages','sorabes, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wln',Null,'wa','Walloon','wallon')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('wol',Null,'wo','Wolof','wolof')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('xal',Null,Null,'Kalmyk; Oirat','kalmouk; oïrat')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('xho',Null,'xh','Xhosa','xhosa')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('yao',Null,Null,'Yao','yao')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('yap',Null,Null,'Yapese','yapois')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('yid',Null,'yi','Yiddish','yiddish')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('yor',Null,'yo','Yoruba','yoruba')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('ypk',Null,Null,'Yupik languages','yupik, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zap',Null,Null,'Zapotec','zapotèque')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zbl',Null,Null,'Blissymbols; Blissymbolics; Bliss','symboles Bliss; Bliss')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zen',Null,Null,'Zenaga','zenaga')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zgh',Null,Null,'Standard Moroccan Tamazight','amazighe standard marocain')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zha',Null,'za','Zhuang; Chuang','zhuang; chuang')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('znd',Null,Null,'Zande languages','zandé, langues')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zul',Null,'zu','Zulu','zoulou')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zun',Null,Null,'Zuni','zuni')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zxx',Null,Null,'No linguistic content; Not applicable','pas de contenu linguistique; non applicable')
INSERT INTO dbo.ISO639_2_Lang ( ISO_639_2_Bibliographic, ISO_639_2_Terminology, ISO_639_1, EnglishName, FrenchName ) VALUES ('zza',Null,Null,'Zaza; Dimili; Dimli; Kirdki; Kirmanjki; Zazaki','zaza; dimili; dimli; kirdki; kirmanjki; zazaki')
GO

ALTER TABLE dbo.ISO639_2_Lang ADD Active BIT NOT NULL DEFAULT 1;
GO
CREATE INDEX IX_ISO_639_2_Active ON dbo.ISO639_2_Lang (Active) WHERE Active = 1;
GO


CREATE VIEW dbo.AllLanguages AS 
	SELECT
		l.ID,
		l.ISO_639_2_Bibliographic AS BiblioCode,
		l.ISO_639_2_Terminology AS TermCode,
		l.ISO_639_1 AS Code,
		l.EnglishName,
		l.FrenchName
	FROM dbo.ISO639_2_Lang AS l
	WHERE Active = 1
;
GO

CREATE VIEW dbo.CommonLanguages AS
	SELECT
		l.ID,
		l.ISO_639_1 AS Code,
		l.EnglishName AS Description
	FROM dbo.ISO639_2_Lang AS l
	WHERE Active = 1
		AND l.ISO_639_1 IS NOT NULL
;
GO
   
   
   
   
   
-- ============================
-- Clean up existing Languages stuff
-- ============================

ALTER TABLE dbo.Referrals ADD ReferralPrimarySpokenLanguageID INT; -- soft ref to dbo.ISO639_2_Lang
GO

ALTER TABLE dbo.Patients ADD PatientPrimarySpokenLanguageID INT; -- soft ref to dbo.ISO639_2_Lang
GO

DROP TABLE dbo.ProviderLanguages;
GO

CREATE TABLE dbo.ProviderLanguages (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProviderID INT NOT NULL REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	LanguageID INT NOT NULL REFERENCES dbo.ISO639_2_Lang (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	LanguageStrength INT NOT NULL REFERENCES dbo.LanguageStrengthTypes (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE UNIQUE INDEX idxProviderLanguagesProvLang ON dbo.ProviderLanguages(ProviderID, LanguageID);
GO
  
  
  
  
  
  
  
 
-- ============================
-- Provider Portal Users
-- ============================

CREATE TABLE dbo.ProviderPortalUsers (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AspNetUserID INT, -- references provider portal project's aspnet db ID
	ProviderID INT REFERENCES dbo.Providers (ID) ON UPDATE CASCADE ON DELETE SET NULL, -- to link up with the core db's Provider record
	ProviderUserNumber NVARCHAR(10)  -- unique number they'll log in with
);
CREATE UNIQUE INDEX idxProviderPortalUserNumer ON dbo.ProviderPortalUsers (ProviderUserNumber);
CREATE UNIQUE INDEX idxProviderPortalAspNetID ON dbo.ProviderPortalUsers (AspNetUserID);
CREATE UNIQUE INDEX idxProviderPortalProviderID ON dbo.ProviderPortalUsers (ProviderID);
GO
  
 
 

-- ============================
-- Cleanup Auth Dates
-- ============================

UPDATE dbo.CaseAuthCodes SET AuthEndDate = DATEADD(MONTH, 6, AuthStartDate) WHERE AuthEndDate IS NULL;
UPDATE dbo.CaseAuthCodes SET AuthStartDate = DATEADD(MONTH, -6, AuthEndDate) WHERE AuthStartDate IS NULL;
GO

ALTER TABLE dbo.CaseAuthCodes ALTER COLUMN AuthStartDate DATETIME2 NOT NULL;
ALTER TABLE dbo.CaseAuthCodes ALTER COLUMN AuthEndDate DATETIME2 NOT NULL;
GO

 
-- ============================
-- Update General Hours to support decimal precision
-- ============================

-- EXEC MANUAL -- DropTotalHoursDefaultConstraint
GO
ALTER TABLE dbo.CaseAuthCodes ALTER COLUMN AuthTotalHoursApproved DECIMAL(6,2) NOT NULL;
GO
ALTER TABLE dbo.CaseAuthCodes ADD CONSTRAINT DF_AuthCode_TotalHours DEFAULT 0 FOR AuthTotalHoursApproved;
GO

-- EXEC MANUAL -- DropHoursAppliedDefaultConstraint
GO
ALTER TABLE dbo.CaseAuthCodeGeneralHours ALTER COLUMN HoursApplied DECIMAL(6,2) NOT NULL;
GO
ALTER TABLE dbo.CaseAuthCodeGeneralHours ADD CONSTRAINT DF_AuthGenHr_HrsAppl DEFAULT 0 FOR HoursApplied;
GO

 DROP PROCEDURE [dbo].[UpsertGeneralHours]
GO

CREATE PROCEDURE [dbo].[UpsertGeneralHours](@CaseAuthID INT, @Year INT, @Month INT, @Hours DECIMAL(6,2)) AS

	BEGIN TRANSACTION

		IF EXISTS(
			SELECT * 
			FROM dbo.CaseAuthCodeGeneralHours 
			WHERE CaseAuthID = @CaseAuthID
				AND HoursYear = @Year
				AND HoursMonth = @Month
			)
		
			BEGIN

				UPDATE dbo.CaseAuthCodeGeneralHours 
				SET HoursApplied = @Hours
				WHERE CaseAuthID = @CaseAuthID
					AND HoursYear = @Year
					AND HoursMonth = @Month

			END

		ELSE

			BEGIN

				INSERT INTO dbo.CaseAuthCodeGeneralHours
				        (CaseAuthID,
				         HoursYear,
				         HoursMonth,
				         HoursApplied
				        )
				VALUES  (@CaseAuthID, -- CaseAuthID - int
				         @Year, -- HoursYear - int
				         @Month, -- HoursMonth - int
				         @Hours  -- HoursApplied - decimal(6,2)
				        )

			END

	COMMIT TRANSACTION
GO



 -- CREATE A PROVIDER NUMBER FIELD
 ALTER TABLE dbo.Providers ADD ProviderNumber NVARCHAR(30);
 

--	EXEC MANUAL -- create aabc-aspnet-providers;
 
   
   
  -- //// END MIGRATION SCRIPT v1.2.0.0-v1.2.1.0
  