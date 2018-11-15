# AABC Case Manager

This repository contains code and documentation for Applied ABC's Case Manager system.  For other Applied systems (such as ATrack or Atidaynu/OFS), see their respective repositories.

## Repository Core Directories

* **/Assets**: various assets required by the system
* **/CI**: for Continuous Integration.  Don't touch.
* **/Database**: db scripts, test data and the like
* **/Docs**: project documentation
* **/Graphics**: various graphics workup and completed files
* **/VisualStudio**: the main code projects

## Applications

The Case Manager system has a number of frontend applications:

* **Manage**: (AABC.Web): the main administrative web app that the office uses
* **Provider Portal**: Web app that providers log into for entering their hours.
* **Patient Portal**: Web app that parents can log into to check hours, make payments, etc.
* **Provider App**: Mobile application (iOS/Android) that Providers use for realtime session and hours tracking

## Visual Studio Solutions

The codebase is split into two solutions.  Some projects are shared by both solutions.

* **AABC.Web**: This solution contains most of the codebase, including domain, data and web application layers
* **AABC.Mobile**: This solution contains the mobile application and related projects.

The code is split into two solutions to allow developers without a solid Xamarin to setup to work with the more stable web-based projects without having to fight with Xamarin dependencies.

## Databases

All databases can be run on SQL Server 2014 Express or later.  There are three databases in use (whose names of course can be changed to whatever you prefer locally):

* **aabc_data_dev**: The main database where everything except user login info resides
* **aabc_aspnet_dev**: The asp.net web membership database for the Manage (AABC.Web) application
* **aabc_aspnet_providerportal_dev**: The asp.net web membership database for the Provider Portal application

### Initializing the Database

While the database can be scripted via the versioned files in the `/Database` directory (see Dymeng's [SQL Scripting documentation](https://dev.docs.dymeng.com/sql-scripting)), at present the test data isn't sufficient to run the whole project.  Instead, ask your project lead for a copy of a development database (development databases are currently a scrubbed copy of a production or staging database).

## Local Setup

See the appropriate documentation page for application setup instructions.

* **AABC.Web** (Manage):
* [Setup Instructions](Docs/Dev/Web/Manage/ProjectSetup.md)
* [Documentation Home](Docs/Dev/Web/Manage/Index.md)
* **AABC.ProviderPortal**:
* [Setup Instructions](Docs/Dev/Web/ProviderPortal/ProjectSetup.md)
* [Documentation Home](Docs/Dev/Web/ProviderPortal/Index.md)
* **AABC.Mobile** (and related):
* [Setup Instructions](Docs/Dev/Mobile/ProviderApp/ProjectSetup.md)
* [Documentation Home](Docs/Dev/Mobile/ProviderApp/Index.md)
* **AABC.ParentPortal**:
* [Setup Instructions](Docs/Dev/Web/PatientPortal/ProjectSetup.md)
* [Documentation Home](Docs/Dev/Web/PatientPortal/Index.md)

## Android Emulator

Android Emulator uses 10.0.2.2 to access your actual machine by its loopback address (127.0.0.1).

If the web applications are run via IIS Express, you may need to bind to all hostnames instead of just 'localhost'.
 In your .config file (typically %userprofile%\My Documents\IISExpress\config\applicationhost.config, or $(solutionDir).vs\config\applicationhost.config for Visual Studio), find your site's binding element, and add

```XML

#!xml
    <binding protocol="http" bindingInformation="*:[port]:127.0.0.1" />
```

Make sure to add it as a second binding instead of modifying the existing one or VS will just re-add a new site appended with a (1) Also, you may need to run VS as an administrator.

References:
<https://stackoverflow.com/questions/6760585/accessing-localhostport-from-android-emulator>
<https://stackoverflow.com/questions/5528850/how-do-you-connect-localhost-in-the-android-emulator?lq=1>








----------
CONTENT BELOW IS NOT UP TO DATE
----------

----------

## Development Builds

For a development build, do the following:

1. Get a copy of this repository
2. Add `Local.Connections.config` to the AABC.Web project root if this is the a fresh environment setup.  See references below for example.
3. Run the database build scripts if you don't have an up to date database
4. Update the Local.Connections.config file accordingly
5. Open and build the project in Visual Studio 2015

## Creating a User Login

To date, there's a testuser on file, but the password and permissions need to be set.  Typically the testuser ID in the main database (aabc_data_dev.dbo.WebUsers) is 3. Then there's the aabc_aspnet_dev.dbo.webpages_Membership database/table whose corresponding ID is usually 2.

The permissions and password need to be updated in the development environment: run the following script:

```
!#sql
-- reset and apply all permissions (in aabc_data_scrub)
DECLARE @UserID INT = (SELECT TOP 1 ID FROM dbo.WebUsers WHERE UserName = 'testuser') -- usually id is 3
DECLARE @AspNetID INT = (SELECT TOP 1 AspNetUserID FROM dbo.WebUsers WHERE ID = @UserID)

DELETE FROM [aabc_data_scrub].dbo.WebUserPermissions WHERE WebUserID = @UserID
INSERT INTO [aabc_data_scrub].dbo.WebUserPermissions (WebUserID, WebPermissionID)
    SELECT @UserID, p.ID
    FROM [aabc_data_scrub].dbo.WebPermmissions;

-- set the password
UPDATE [aabc_aspnet_dev].dbo.webpages_Membership SET Password = 'AO8EVQL/a6+xhSeDzOlRHJMt+8lual+YWrfJ14cHJ/xThLCIq1lG7ybHgc4ehxKL8g==' WHERE UserId = @AspNetID
GO
```

This sets the password to 'password1' and gives full permissions to the test user. (the test user username is 'testuser')

### Local.Connections.config

```XML
#!xml
    <connectionStrings>
      <add name="DefaultConnection" connectionString="data source=(localdb)\mssqllocaldb;initial catalog=aspnet-AABC_Web-20160111222311;integrated security=SSPI" providerName="System.Data.SqlClient" />
      <add name="CoreConnection" connectionString="data source=JACK-WORK\SQLEXPRESS2014;initial catalog=AABC-Dev;integrated security=SSPI" providerName="System.Data.SqlClient" />
    </connectionStrings>
```