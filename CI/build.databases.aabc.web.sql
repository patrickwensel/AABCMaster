-- ========================
-- Drop and recreate an empty db for integration tests
-- ========================
USE [master]

IF DB_ID('aabc_data_int') IS NOT NULL DROP DATABASE aabc_data_int;
IF DB_ID('aabc_aspnet_int') IS NOT NULL DROP DATABASE aabc_aspnet_int;
IF DB_ID('aabc_aspnet_providerportal_int') IS NOT NULL DROP DATABASE aabc_aspnet_providerportal_int;
GO

USE [master]
CREATE DATABASE aabc_data_int;
CREATE DATABASE aabc_aspnet_int;
CREATE DATABASE aabc_aspnet_providerportal_int;
GO



-- for now we have create scripts for the aspnet dbs here
-- later we should move these to actual db source control
-- but they've never changed and it's a 'legacy' approach that we don't use anymore anyway...

-- =====================
-- Create aspnet_providerportal objects
-- =====================
USE [master]
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET COMPATIBILITY_LEVEL = 120
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ARITHABORT OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET  DISABLE_BROKER 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET  MULTI_USER 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET DB_CHAINING OFF 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET DELAYED_DURABILITY = DISABLED 
GO
USE [aabc_aspnet_providerportal_int]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO
USE [master]
GO
ALTER DATABASE [aabc_aspnet_providerportal_int] SET  READ_WRITE 
GO








-- =====================
-- Create aspnet objects
-- =====================
USE [master]
GO
ALTER DATABASE [aabc_aspnet_int] SET COMPATIBILITY_LEVEL = 120
GO
ALTER DATABASE [aabc_aspnet_int] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET ARITHABORT OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [aabc_aspnet_int] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [aabc_aspnet_int] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [aabc_aspnet_int] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET  DISABLE_BROKER 
GO
ALTER DATABASE [aabc_aspnet_int] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [aabc_aspnet_int] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [aabc_aspnet_int] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [aabc_aspnet_int] SET  MULTI_USER 
GO
ALTER DATABASE [aabc_aspnet_int] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [aabc_aspnet_int] SET DB_CHAINING OFF 
GO
ALTER DATABASE [aabc_aspnet_int] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [aabc_aspnet_int] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [aabc_aspnet_int] SET DELAYED_DURABILITY = DISABLED 
GO
USE [aabc_aspnet_int]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 5/4/2018 10:17:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO
USE [master]
GO
ALTER DATABASE [aabc_aspnet_int] SET  READ_WRITE 
GO









-- eof