# Manage application Project Setup

## Repository, database and configuration

1. Download the repository
2. Obtain a copy of the development databases (as noted in the repository's readme.md)
3. Navigate to the directory `VisualStudio/AABC.Web`
4. Create a Local.Connections.config file
5. Create a Local.ConnectionsTest.config file
6. Create an Local.AppSettings.config file


The local config files are ignored by git and will be specific to your machine.  

### Sample Local.Connections.config and Local.ConnectionsTest.config file:

	<connectionStrings>
	  <add name="DefaultConnection" connectionString="data source=JACK-WORK\SQLEXPRESS2014;initial catalog=aabc_aspnet_dev;integrated security=SSPI" providerName="System.Data.SqlClient" />
	  <add name="CoreConnection" connectionString="data source=JACK-WORK\SQLEXPRESS2014;initial catalog=aabc_data_dev;integrated security=SSPI" providerName="System.Data.SqlClient" />
	  <add name="ProviderPortalConnection" connectionString="data source=JACK-WORK\SQLEXPRESS2014;initial catalog=aabc_aspnet_providerportal_dev;integrated security=SSPI" providerName="System.Data.SqlClient" />
	</connectionStrings>

### Sample Local.AppSettings.config file

(this area under construction, exact format currently unknown - at the time of writing, these settings reside in web.config and have not yet been ported to a local file)

	<add key="TempDirectory" value="C:\Projects\Temp\aabc" />
    <add key="UploadDirectory" value="C:\Projects\Temp\aabc\uploads" />
    <add key="LogPath" value="E:\Projects\AABC\manage.exception.log" />
    <add key="ProviderPortal.DocuSign.Finalizations.Root" value="E:\Projects\AABC\Environment\DocuSign\Finalizations" />

## Configure Database/Users

Refer to [Docs/Dev/Database/DatabaseOperations#CreateManageTestUser](Docs/Dev/Database/DatabaseOperations.md#CreateManageTestUser) for instructions on creating a test user (devadmin/devadmin) for the Manage application.

## Open and Build the Solution

Open the `AABC.Web` solution.  Build all projects.  Set `AABC.Web` as the startup project.  Run the project.