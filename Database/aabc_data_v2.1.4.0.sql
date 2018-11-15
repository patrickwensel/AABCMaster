/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.3.0
dym:TargetEndingVersion: 2.1.4.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/




-- Create table dbo.StaffingLogProviderStatuses
CREATE TABLE dbo.StaffingLogProviderStatuses (
	ID INT NOT NULL,
	StatusName NVARCHAR(50) NOT NULL,
	StatusDescription NVARCHAR(100) NULL,
	Active BIT NOT NULL,
	CONSTRAINT PK_StaffingLogProviderStatuses PRIMARY KEY (ID)
);
GO

INSERT INTO dbo.StaffingLogProviderStatuses (ID, StatusName, StatusDescription, Active)
VALUES	(1, 'Not Interested', NULL, 1),
		(2, 'Interested', NULL, 1),
		(3, 'Will Take Case', NULL, 1),
		(4, 'Not Answered', NULL, 1),
		(5, 'Docusign Complete', NULL, 1),
		(6, 'Background Check OK', NULL, 1);
GO

-- Create table dbo.StaffingLogProviderContactLog
CREATE TABLE dbo.StaffingLogProviderContactLog (
	ID INT IDENTITY(1,1) NOT NULL,
	StaffingLogProviderID INT NOT NULL,
	StatusID INT NOT NULL,
	ContactDate DATETIME2 NOT NULL,
	Notes NVARCHAR(2000) NULL,
	FollowUpDate DATETIME2 NULL,
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	CreatedByUserID INT NOT NULL,
	CONSTRAINT PK_StaffingLogProviderContactLog PRIMARY KEY (ID),
	CONSTRAINT [FK_StaffingLogProviderContactLog_StaffingLogProviderStatuses] 
		FOREIGN KEY (StatusID) REFERENCES dbo.StaffingLogProviderStatuses (ID)
);
GO


-- Remove obsolete columns from dbo.StaffingLogProviders
ALTER TABLE dbo.StaffingLogProviders
DROP COLUMN HasBeenContacted;

ALTER TABLE dbo.StaffingLogProviders
DROP COLUMN Response;

ALTER TABLE dbo.StaffingLogProviders
DROP COLUMN Notes;

ALTER TABLE dbo.StaffingLogProviders
DROP COLUMN FollowUpDate;
GO


-- Update stored procs affected by dbo.StaffingLogProviders updates
ALTER PROCEDURE [dbo].[GetSelectedProvidersByStaffingLog]
	@staffingLogID int
AS 
BEGIN
	WITH ContactLog AS (
		SELECT	slpcl.StaffingLogProviderID,
				slpcl.Notes,
				slps.StatusName,
				slpcl.FollowUpDate,
				ROW_NUMBER() OVER (PARTITION BY slpcl.StaffingLogProviderID ORDER BY slpcl.ContactDate DESC) AS rowNumber
		FROM dbo.StaffingLogProviderContactLog AS slpcl
			JOIN dbo.StaffingLogProviderStatuses AS slps ON slpcl.StatusID = slps.ID
	)
	SELECT
		p.ID as ProviderID,
		p.ProviderFirstname,
		p.ProviderLastname,
		pt.ProviderTypeCode,
		p.ProviderCity,
		p.ProviderState,
		p.ProviderZip,
		ProviderServiceAreas = STUFF(
			(
				SELECT ',' + pz.ZipCode
				FROM dbo.ProviderServiceZipCodes AS pz
				WHERE p.ID = pz.ProviderID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		ProviderServiceCounties = dbo.GetCountiesForProvider(p.ID, NULL),
		ProviderLanguages = STUFF(
			(
				SELECT ',' + cl.Description
				FROM dbo.CommonLanguages AS cl
				INNER JOIN dbo.ProviderLanguages AS pl ON cl.ID = pl.LanguageID
				WHERE pl.ProviderID = p.ID
				FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)'), 1, 1, ''
		),
		slp.ID as StaffingLogProviderID,
		CONVERT(BIT, CASE WHEN cl.StaffingLogProviderID IS NULL THEN 0 ELSE 1 END) AS HasBeenContacted,  
		cl.StatusName AS Status,  
		cl.Notes,
		cl.FollowUpDate
	FROM dbo.Providers AS p 
		INNER JOIN dbo.StaffingLogProviders AS slp ON p.ID = slp.ProviderID
		LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType
		LEFT JOIN ContactLog AS cl ON cl.StaffingLogProviderID = slp.ID AND cl.rowNumber = 1
	WHERE p.ProviderActive = 1 AND slp.StaffingLogID = @staffingLogID
	ORDER BY p.ProviderLastName, p.ProviderFirstName;
END
GO


-- Create table dbo.StaffingLogParentContactLog
CREATE TABLE dbo.StaffingLogParentContactLog (
	ID INT IDENTITY(1,1) NOT NULL,
	StaffingLogID INT NOT NULL,
	GuardianRelationshipID INT NOT NULL,
	ContactedPersonName NVARCHAR(100) NULL,
	ContactDate DATETIME2 NOT NULL,
	ContactMethodType INT NOT NULL,
	ContactMethodValue NVARCHAR(100) NOT NULL,
	Notes NVARCHAR(2000) NOT NULL,
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	CreatedByUserID INT NOT NULL,
	CONSTRAINT PK_StaffingLogParentContactLog PRIMARY KEY (ID),
	CONSTRAINT [FK_StaffingLogParentContactLog_GuardianRelationships]
		FOREIGN KEY (GuardianRelationshipID) REFERENCES dbo.GuardianRelationships (ID),
	CONSTRAINT [FK_StaffingLogParentContactLog_StaffingLog] 
		FOREIGN KEY (StaffingLogID) REFERENCES dbo.StaffingLog (ID)
);
GO






CREATE VIEW dbo.ProviderLoginInfo AS 
	SELECT 
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		u.ProviderUserNumber,
		u.AspNetUserID
	FROM dbo.Providers AS p 
	INNER JOIN dbo.ProviderPortalUsers AS u ON p.ID = u.ProviderID
GO





CREATE PROCEDURE dbo.GetProvidersWithoutBilling (@StartDate DATETIME2, @EndDate DATETIME2) AS

	/* Test Data
	DECLARE @StartDate DATETIME2
	DECLARE @EndDate DATETIME2
	SET @StartDate = '2018-03-01'
	SET @EndDate = '2018-03-31'
	-- */

	SELECT
		base.PatientID,
		base.CaseID,
		base.ProviderID,
		base.PatientFirstName,
		base.PatientLastName,
		base.ProviderFirstName,
		base.ProviderLastName,
		base.ProviderType,
		base.Active,
		base.ActiveStartDate,
		base.ActiveEndDate,
		COALESCE(cah.TotalBillable, 0) AS Billable
	FROM (
		SELECT
			p.ID AS PatientID,
			c.ID AS CaseID,
			pv.ID AS ProviderID,
			p.PatientFirstName,
			p.PatientLastName,
			pt.ProviderTypeCode AS ProviderType,
			pv.ProviderFirstName,
			pv.ProviderLastName,
			cp.Active, 
			cp.ActiveStartDate, 
			cp.ActiveEndDate
		FROM dbo.Patients AS p
			INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
			INNER JOIN dbo.CaseProviders AS cp ON c.ID = cp.CaseID
			INNER JOIN dbo.Providers AS pv ON cp.ProviderID = pv.ID
			INNER JOIN dbo.ProviderTypes AS pt ON pt.ID = pv.ProviderType
		WHERE (cp.ActiveStartDate IS NULL OR cp.ActiveStartDate <= @EndDate)
			AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= @StartDate)
			AND cp.Active = 1
			AND c.CaseStatus > -1
	) AS base

	LEFT JOIN (
		SELECT h.CaseID, h.CaseProviderID AS ProviderID, SUM(h.HoursBillable) AS TotalBillable
		FROM dbo.CaseAuthHours AS h
		WHERE h.HoursDate >= @StartDate AND h.HoursDate <= @EndDate
		GROUP BY h.CaseID, h.CaseProviderID
	) AS cah ON cah.CaseID = base.CaseID AND cah.ProviderID = base.ProviderID

	WHERE COALESCE(cah.TotalBillable, 0) = 0

	GROUP BY
		base.PatientID,
		base.CaseID,
		base.ProviderID,
		base.PatientFirstName,
		base.PatientLastName,
		base.ProviderFirstName,
		base.ProviderLastName,
		base.ProviderType,
		base.Active,
		base.ActiveStartDate,
		base.ActiveEndDate,
	COALESCE(cah.TotalBillable, 0)

	ORDER BY base.CaseID, base.ProviderID

GO
	

	
	
	
	
	
 CREATE VIEW exports.ProviderPayablesByInsAuthdBCBA AS

	/* WARNING - the hours payable that are shown here are not necessarily the
		direct hours that the noted provider put it: the noted provider is either
		the actual provider that provided the service, OR is the authorized BCBA
		on file for the case, if there is an auth'd BCBA.
		For use with PowerBI financial analytics only!
	*/

	SELECT
		cah.HoursDate,
		
		/* for comparison purposes
		p.ID AS ProviderID,
		p.ProviderFirstName,
		p.ProviderLastName,
		-- */

		/* for comparison purposes
		authedBcbaByCase.ProviderID AS AuthdProviderID,
		authedBcbaByCase.ProviderFirstName AS AuthdProviderFirstName,
		authedBcbaByCase.ProviderLastName AS AuthdProviderLastName,
		-- */ 

		/* -- 
		CASE WHEN authedBcbaByCase.ProviderID IS NULL THEN p.ID ELSE authedBcbaByCase.ProviderID END AS CalcdProviderID,
		CASE WHEN authedBcbaByCase.ProviderFirstName IS NULL THEN p.ProviderFirstName ELSE authedBcbaByCase.ProviderFirstName END AS CalcdProviderFirstName,
		CASE WHEN authedBcbaByCase.ProviderLastName IS NULL THEN p.ProviderLastName ELSE authedBcbaByCase.ProviderLastName END AS CalcdProviderLastName,
		-- */

		CASE WHEN authedBcbaByCase.ProviderID IS NULL THEN p.ID ELSE authedBcbaByCase.ProviderID END AS ProviderID,
		CASE WHEN authedBcbaByCase.ProviderFirstName IS NULL THEN p.ProviderFirstName ELSE authedBcbaByCase.ProviderFirstName END AS ProviderFirstName,
		CASE WHEN authedBcbaByCase.ProviderLastName IS NULL THEN p.ProviderLastName ELSE authedBcbaByCase.ProviderLastName END AS ProviderLastName,

		cah.HoursPayable,
		p.ProviderRate,
		pt.ID AS PatientID,
		pt.PatientFirstName,
		pt.PatientLastName,
		i.ID AS InsuranceID,
		i.InsuranceName,
		p.ProviderType AS ProviderTypeID,
		ptypes.ProviderTypeCode AS ProviderTypeCode,
		ptypes.ProviderTypeName AS ProviderTypeName
		
	FROM dbo.Providers AS p
		INNER JOIN dbo.CaseAuthHours AS cah ON cah.CaseProviderID = p.ID
		INNER JOIN dbo.Cases AS c ON c.ID = cah.CaseID
		INNER JOIN dbo.Patients AS pt ON pt.ID = c.PatientID
		LEFT JOIN dbo.Insurances AS i ON pt.PatientInsuranceID = i.ID
		LEFT JOIN dbo.ProviderTypes AS ptypes ON ptypes.ID = p.ProviderType
			
	LEFT JOIN (
		SELECT
			sc.ID AS CaseID,
			scp.ProviderID AS ProviderID,
			scp.ActiveStartDate,
			scp.ActiveEndDate,
			scpp.ProviderFirstName,
			scpp.ProviderLastName
		FROM dbo.Cases AS sc
		INNER JOIN dbo.CaseProviders AS scp ON sc.ID = scp.CaseID
		INNER JOIN dbo.Providers AS scpp ON scpp.ID = scp.ProviderID
		WHERE scp.IsInsuranceAuthorizedBCBA = 1
	) AS authedBcbaByCase ON 
		c.ID = authedBcbaByCase.CaseID
		AND (authedBcbaByCase.ActiveEndDate IS NULL OR authedBcbaByCase.ActiveEndDate >= cah.HoursDate)
		AND (authedBcbaByCase.ActiveStartDate IS NULL OR authedBcbaByCase.ActiveStartDate <= cah.HoursDate)

	

	WHERE cah.HoursStatus = 3 -- finalized only

	

GO







GO
EXEC meta.UpdateVersion '2.1.4.0'
GO

