echo "Building Databases..."
echo "Compiling database source..."

MKDIR %~do0TMP

REM generate scripts for the core db, place file
SQLDEPLOY outputscript -s %~dp0Database -t %~dp0TMP\dbprovision.sql

REM provision empty dbs and fill aspnet db objects
SQLCMD -s WS-1A\DYMAPPS -d master -i CI\build.databases.aabc.web.sql

REM fill core data with previously compiled script file
SQLCMD -s WS-1A\DYMAPPS -d aabc_data_int -i TMP\dbprovision.sql 

REM fill test data
REM (test data not yet implemented)


