/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.9.2.0
dym:TargetEndingVersion: 1.9.8.0
---------------------------------------------------------------------

	Hours Eligibility	
	
---------------------------------------------------------------------*/



-- =============================
-- Clean up more insurance crap (why wasn't this done when the table was created??)
-- =============================
DELETE FROM dbo.CaseInsurances WHERE CaseID IS NULL;
DELETE FROM dbo.CaseInsurances WHERE InsuranceID IS NULL;
UPDATE dbo.CaseInsurances SET HardshipWaiverLike = 0 WHERE HardshipWaiverLike IS NULL;
UPDATE dbo.CaseInsurances SET HardshipWaiverApplied = 0 WHERE HardshipWaiverApplied IS NULL;
UPDATE dbo.CaseInsurances SET HardshipWaiverApproved = 0 WHERE HardshipWaiverApproved IS NULL;
UPDATE dbo.CaseInsurances SET PaymentPlanLikeToDiscuss = 0 WHERE PaymentPlanLikeToDiscuss IS NULL;
GO
ALTER TABLE dbo.CaseInsurances ALTER COLUMN CaseID INT NOT NULL;
ALTER TABLE dbo.CaseInsurances ALTER COLUMN InsuranceID INT NOT NULL;
ALTER TABLE dbo.CaseInsurances DROP COLUMN InsurancePhoneNumber;
ALTER TABLE dbo.CaseInsurances ALTER COLUMN HardshipWaiverLike BIT NOT NULL;
ALTER TABLE dbo.CaseInsurances ALTER COLUMN HardshipWaiverApplied BIT NOT NULL;
ALTER TABLE dbo.CaseInsurances ALTER COLUMN HardshipWaiverApproved BIT NOT NULL;
ALTER TABLE dbo.CaseInsurances ALTER COLUMN PaymentPlanLikeToDiscuss BIT NOT NULL;
ALTER TABLE dbo.CaseInsurances DROP COLUMN Active;
GO



-- =============================
-- Clean up duplicate note entries
-- =============================
-- Obtain visual list of problem rows
-- /*
SELECT t.*
FROM dbo.CaseAuthHoursNotes AS t
INNER JOIN (
	SELECT n.HoursID, n.NotesTemplateID, COUNT(n.NotesTemplateID) AS CountOfTemplates
	FROM dbo.CaseAuthHoursNotes AS n
	GROUP BY HoursID, NotesTemplateID
	HAVING COUNT(n.NotesTemplateID) > 1
) AS s ON t.HoursID = s.HoursID AND t.NotesTemplateID = s.NotesTemplateID
ORDER BY t.HoursID, t.NotesTemplateID;
-- */

-- with select, this should match less the first row...
-- when confirmed, change select to delete
;WITH cte AS (
	SELECT t.*,
		ROW_NUMBER() OVER(PARTITION BY t.HoursID, t.NotesTemplateID ORDER BY t.HoursID, t.NotesTemplateID) AS rownum 
	FROM dbo.CaseAuthHoursNotes AS t
)
SELECT * FROM cte WHERE rownum > 1;
GO


-- now toss a unique index on so this doesn't happen again
CREATE UNIQUE INDEX idxCaseAuthHoursNotesHoursTemplateID ON dbo.CaseAuthHoursNotes (HoursID, NotesTemplateID);
GO





-- ============================
-- Work up schema and views for hours entry eligibility
-- ============================
CREATE SCHEMA hoursEntry;
GO

CREATE VIEW hoursEntry.EligibleProviders AS
	SELECT 
		ProviderID 
	FROM dbo.ProviderPortalUsers;
GO

CREATE VIEW hoursEntry.EligibleCases AS
	SELECT 
		c.ID AS CaseID
	FROM dbo.CaseProviders AS cp
		INNER JOIN hoursEntry.EligibleProviders AS ep ON cp.ProviderID = ep.ProviderID
		INNER JOIN dbo.Cases AS c ON c.ID = cp.CaseID
	WHERE cp.Active = 1	-- provider active
		AND c.CaseStatus <> -1	-- not history/discharged
		AND (cp.ActiveStartDate IS NULL OR cp.ActiveStartDate <= GETDATE())	-- provider active by date
		AND (cp.ActiveEndDate IS NULL OR cp.ActiveEndDate >= GETDATE())	-- provider active by date
	GROUP BY c.ID
GO

CREATE VIEW hoursEntry.CasesWithActiveInsurance AS 
	SELECT 
		ec.CaseID
	FROM hoursEntry.EligibleCases AS ec
		INNER JOIN dbo.CaseInsurances AS ci ON ci.CaseID = ec.CaseID
	WHERE (ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE())
	GROUP BY ec.CaseID
GO

CREATE VIEW hoursEntry.CasesWithoutActiveInsurance AS 
	SELECT ec.CaseID
	FROM hoursEntry.EligibleCases AS ec
		LEFT JOIN hoursEntry.CasesWithActiveInsurance AS cwi ON cwi.CaseID = ec.CaseID
	WHERE cwi.CaseID IS NULL
	GROUP BY ec.CaseID;
GO

CREATE VIEW hoursEntry.CasesWithActiveAuthorizations AS
	SELECT ec.CaseID
	FROM hoursEntry.EligibleCases AS ec
		INNER JOIN dbo.CaseAuthCodes AS cac ON ec.CaseID = cac.CaseID
	WHERE cac.AuthStartDate <= GETDATE()
		AND cac.AuthEndDate >= GETDATE()
	GROUP BY ec.CaseID
GO

CREATE VIEW hoursEntry.CasesWithoutActiveAuthorizations AS
	SELECT ec.CaseID
	FROM hoursEntry.EligibleCases AS ec
		LEFT JOIN hoursEntry.CasesWithActiveAuthorizations AS cwa ON ec.CaseID = cwa.CaseID
	WHERE cwa.CaseID IS NULL
	GROUP BY ec.CaseID;
GO

CREATE VIEW hoursEntry.EligibleInsurances AS 
	SELECT  ci.InsuranceID
	FROM hoursEntry.CasesWithActiveInsurance AS cwi
		INNER JOIN dbo.Cases AS c ON cwi.CaseID = c.ID
		INNER JOIN dbo.CaseInsurances AS ci ON c.ID = ci.CaseID
	WHERE
		(ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE())
	GROUP BY ci.InsuranceID;
GO
	
CREATE VIEW hoursEntry.EligibleAuthorizations AS 
	SELECT 
		cac.CaseID,
		cac.ID AS CaseAuthorizationID,
		ac.ID AS AuthCodeID,
		ac.CodeCode AS AuthCode
	FROM hoursEntry.CasesWithActiveAuthorizations AS cwi
		INNER JOIN dbo.CaseAuthCodes AS cac ON cwi.CaseID = cac.CaseID
		INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID
	WHERE
		cac.AuthStartDate <= GETDATE()
		AND cac.AuthEndDate >= GETDATE()
	GROUP BY 
		cac.CaseID, 
		cac.ID,
		ac.ID,
		ac.CodeCode;
GO

ALTER TABLE dbo.AuthMatchRules ADD RuleEffectiveDate DATETIME2;
ALTER TABLE dbo.AuthMatchRules ADD RuleDefectiveDate DATETIME2;
GO

CREATE VIEW hoursEntry.AuthorizationsWithoutRules AS
	SELECT 
		ea.CaseID,
		ea.CaseAuthorizationID,
		ea.AuthCodeID,
		ea.AuthCode
	
	FROM hoursEntry.EligibleAuthorizations AS ea
		INNER JOIN dbo.Cases AS c ON c.ID = ea.CaseID
		INNER JOIN dbo.CaseInsurances AS ci ON ci.CaseID = c.ID
		LEFT JOIN dbo.AuthMatchRules AS mr ON 
			mr.InsuranceID = ci.InsuranceID
			AND mr.RuleInitialAuthID = ea.AuthCodeID
		
	WHERE (ci.DatePlanEffective IS NULL OR ci.DatePlanEffective <= GETDATE())
		AND (ci.DatePlanTerminated IS NULL OR ci.DatePlanTerminated >= GETDATE())
		AND (mr.RuleEffectiveDate IS NULL OR mr.RuleEffectiveDate <= GETDATE())
		AND (mr.RuleDefectiveDate IS NULL OR mr.RuleDefectiveDate >= GETDATE())
		AND mr.ID IS NULL

	GROUP BY
		ea.CaseID,
		ea.CaseAuthorizationID,
		ea.AuthCodeID,
		ea.AuthCode;
GO




CREATE INDEX [Missing_IXNC_CaseAuthHoursBreakdown_CaseAuthID_AF196] ON [dbo].[CaseAuthHoursBreakdown] ([CaseAuthID]);
CREATE INDEX [Missing_IXNC_CaseAuthHoursNotes_HoursID_459F1] ON [dbo].[CaseAuthHoursNotes] ([HoursID]) INCLUDE ([ID]);
CREATE INDEX [Missing_IXNC_CaseAuthHoursNotes_HoursID_C30AC] ON [dbo].[CaseAuthHoursNotes] ([HoursID]) INCLUDE ([ID], [DateCreated], [rv], [NotesTemplateID], [NotesAnswer]);
CREATE INDEX [Missing_IXNC_ZipCodes_ZipState_ZipCity_10074] ON [dbo].[ZipCodes] ([ZipState],[ZipCity]);
CREATE INDEX [Missing_IXNC_ZipCodes_ZipState_ZipCounty_A9E3B] ON [dbo].[ZipCodes] ([ZipState],[ZipCounty]);
CREATE INDEX [Missing_IXNC_CaseAuthHoursNotes_HoursID_B01D8] ON [dbo].[CaseAuthHoursNotes] ([HoursID]) INCLUDE ([ID], [NotesTemplateID], [NotesAnswer]);
GO





GO
EXEC meta.UpdateVersion '1.9.8.0';
GO

