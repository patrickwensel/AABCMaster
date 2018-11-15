echo "Configuring local connections..."

copy /Y CI\conf.connections.aabc.web.config VisualStudio\AABC.Web\Local.ConnectionsTest.config
REM (other project configs here when ready...)


echo "Building AABC.Web.sln..."
CI/assets/nuget.exe restore VisualStudio\AABC.Web.sln
msbuild VisualStudio\AABC.Web.sln /t:Rebuild /p:Configuration=Integration

