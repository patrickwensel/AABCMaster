/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 2.1.4.0
dym:TargetEndingVersion: 2.1.5.0
---------------------------------------------------------------------


	
---------------------------------------------------------------------*/



-- =========================
-- Add Insurance Local Carriers Support
-- =========================

CREATE TABLE dbo.InsuranceLocalCarriers (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	InsuranceID INT NOT NULL REFERENCES dbo.Insurances (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	CarrierName NVARCHAR(128) NOT NULL
);
CREATE INDEX idxInsuranceLocalCarrierInsuranceFK ON dbo.InsuranceLocalCarriers (InsuranceID);
CREATE UNIQUE INDEX idxInsuranceLocalCarrierInsuranceName ON dbo.InsuranceLocalCarriers (InsuranceID, CarrierName);
GO

-- allow setting carrier for a case insurance
ALTER TABLE dbo.CaseInsurances ADD CaseInsuranceCarrierID INT;
GO

-- add carrier info to base insurance view

ALTER VIEW [dbo].[PatientCurrentInsurance] AS
	SELECT	
		p.ID AS PatientID,
		c.ID AS CaseID,
		ci.InsuranceID,
		ci.DatePlanEffective,
		ci.DatePlanTerminated,
		ci.CaseInsuranceCarrierID
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID
	LEFT JOIN dbo.CaseInsurances AS ci 
		ON c.ID = ci.CaseID 
		AND (ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE());

GO


-- Update search results to include carrier name

ALTER PROCEDURE [dbo].[GetPatientSearchViewData](@ABATypeID INT) AS BEGIN
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
		auth.LatestStartDate AS StartDate,
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
		-- p.PatientInsuranceCompanyName,
		i.InsuranceName AS PatientInsuranceCompanyName,
		ilc.CarrierName,
		s.ID AS AssignedStaffID,
		s.StaffFirstName AS AssignedStaffFirstName,
		s.StaffLastName AS AssignedStaffLastName,
		c.CaseNeedsStaffing AS NeedsStaffing,
		c.CaseNeedsRestaffing AS NeedsRestaffing,
		c.CaseRestaffingReason AS RestaffingReason,
		c.CaseRestaffReasonID AS RestaffingReasonID

		
	FROM dbo.Patients AS p
	INNER JOIN dbo.Cases AS c ON p.ID = c.PatientID
	
	LEFT JOIN dbo.PatientCurrentInsurance AS pci ON pci.CaseID = c.ID

	LEFT JOIN dbo.InsuranceLocalCarriers AS ilc ON pci.CaseInsuranceCarrierID = ilc.ID

	LEFT JOIN dbo.Insurances AS i ON i.ID = pci.InsuranceID

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
			MAX(cac.AuthEndDate) AS LatestEndDate,
			MAX(cac.AuthStartDate) AS LatestStartDate
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

	WHERE c.CaseStatus <> -1

	ORDER BY LastName, FirstName;

	RETURN
	END

GO








GO
EXEC meta.UpdateVersion '2.1.5.0'
GO

