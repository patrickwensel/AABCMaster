# Database Operations

Use the following for various db operations

## Users & Passwords

### Create Manage Test User

(incomplete - please fill out next time someone has to do this)

### Reset Provider Portal Passwords

(todo: note which user this pulls from and what the passwords are being set to)

	USE [aabc_aspnet_providerportal_dev]
	GO

	DECLARE @Password NVARCHAR(500) = (SELECT TOP 1 Password FROM dbo.webpages_Membership WHERE UserID = 25)
	UPDATE dbo.webpages_Membership SET Password = @Password;
	SELECT * FROM dbo.webpages_Membership

## Scrub Database

Use this operation to scrub a production db copy for development use.  This scrambles names and removes PII.

	--/* 
	-- SCRUB PATIENT NAMES
	-- Takes all names and swaps them around randomly
	-- https://dba.stackexchange.com/questions/11719/scrubbing-names-via-sql-query-batch
	
	IF OBJECT_ID('tempdb..#RandomData') IS NOT NULL DROP TABLE #RandomData
	CREATE TABLE #RandomData (DataID INT NOT NULL, ID INT NOT NULL, FirstName VARCHAR(50), LastName VARCHAR(50));
	
	INSERT INTO #RandomData (ID, DataID)
		SELECT ROW_NUMBER() OVER (ORDER BY NEWID()), ID FROM dbo.Patients

	UPDATE r SET r.FirstName = x.FirstName
	FROM #RandomData AS r
	INNER JOIN (
		SELECT ROW_NUMBER() OVER (ORDER BY NEWID()) AS ID, PatientFirstName AS FirstName FROM dbo.Patients
	) AS x ON x.ID = r.ID

	UPDATE r SET r.LastName = x.LastName
	FROM #RandomData AS r
	INNER JOIN (
		SELECT ROW_NUMBER() OVER(ORDER BY NEWID()) AS ID, PatientLastName AS LastName FROM dbo.Patients
	) AS x ON x.ID = r.ID
	
	--SELECT * FROM #RandomData WHERE DataID = 12345;
	--SELECT * FROM dbo.Patients WHERE ID = 304710

	CREATE CLUSTERED INDEX PK_RandomData ON #RandomData (DataID ASC);

	ALTER TABLE dbo.Patients DISABLE TRIGGER ALL

	UPDATE dt
		SET dt.PatientFirstName = r.FirstName,
		dt.PatientLastName = r.LastName
	FROM dbo.Patients AS dt
	INNER JOIN #RandomData AS r ON dt.ID = r.DataID;

	ALTER TABLE dbo.Patients ENABLE TRIGGER ALL

	DROP TABLE #RandomData;
	-- */


	--/* 
	-- SCRUB PROVIDER NAMES
	-- Takes all names and swaps them around randomly
	-- https://dba.stackexchange.com/questions/11719/scrubbing-names-via-sql-query-batch
	
	IF OBJECT_ID('tempdb..#RandomData') IS NOT NULL DROP TABLE #RandomData
	CREATE TABLE #RandomData (DataID INT NOT NULL, ID INT NOT NULL, FirstName VARCHAR(50), LastName VARCHAR(50));
	
	INSERT INTO #RandomData (ID, DataID)
		SELECT ROW_NUMBER() OVER (ORDER BY NEWID()), ID FROM dbo.Providers

	UPDATE r SET r.FirstName = x.FirstName
	FROM #RandomData AS r
	INNER JOIN (
		SELECT ROW_NUMBER() OVER (ORDER BY NEWID()) AS ID, ProviderFirstName AS FirstName FROM dbo.Providers
	) AS x ON x.ID = r.ID

	UPDATE r SET r.LastName = x.LastName
	FROM #RandomData AS r
	INNER JOIN (
		SELECT ROW_NUMBER() OVER(ORDER BY NEWID()) AS ID, ProviderLastName AS LastName FROM dbo.Providers
	) AS x ON x.ID = r.ID
	
	--SELECT * FROM #RandomData WHERE DataID = 12345;
	--SELECT * FROM dbo.Providers WHERE ID = 304710

	CREATE CLUSTERED INDEX PK_RandomData ON #RandomData (DataID ASC);

	ALTER TABLE dbo.Providers DISABLE TRIGGER ALL

	UPDATE dt
		SET dt.ProviderFirstName = r.FirstName,
		dt.ProviderLastName = r.LastName
	FROM dbo.Providers AS dt
	INNER JOIN #RandomData AS r ON dt.ID = r.DataID;

	ALTER TABLE dbo.Providers ENABLE TRIGGER ALL

	DROP TABLE #RandomData;
	-- */


	-- /* 
	-- SCRUB LESSER FIELDS
	UPDATE dbo.Patients SET 
		PatientDateOfBirth = NULL,
		PatientGuardianFirstName = NULL,
		PatientGuardianLastName = NULL,
		PatientEmail = NULL,
		PatientPhone = NULL,
		PatientPhone2 = NULL,
		PatientAddress1 = NULL,		
		PatientGuardianEmail = NULL,
		PatientGuardianCellPhone = NULL,
		PatientGuardianHomePhone = NULL,
		PatientGuardianWorkPhone = NULL,
		PatientGuardian2FirstName = NULL,
		PatientGuardian2LastName = NULL,
		PatientGuardian2Email = NULL,
		PatientGuardian2CellPhone = NULL,
		PatientGuardian2HomePhone = NULL,
		PatientGuardian2WorkPhone = NULL;
	-- */