/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.7.3.0
dym:TargetEndingVersion: 1.7.3.1
---------------------------------------------------------------------

	
	
---------------------------------------------------------------------*/


DELETE FROM dbo.Services;
SET IDENTITY_INSERT dbo.Services ON;
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (9,	'DR',	'Direct Care');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (10,	'PRT',	'Parent Training');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (11, 'I-ASS',	'Assessment');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (12,	'TP',	'Treatment Planning');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (13,	'DSU',	'Direct Supervision');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (14,	'SSG',	'Social Skills Group');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (15,	'SDR',	'Supervision Received');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (16,	'TM',	'Team Meeting');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (17,	'AS',	'Assessment');
INSERT INTO dbo.Services (ID, ServiceCode, ServiceName) VALUES (18,	'FUA',	'Followup Assessment');
SET IDENTITY_INSERT dbo.Services OFF;

GO


SET IDENTITY_INSERT dbo.ProviderTypes ON;
INSERT INTO dbo.ProviderTypes (ID, ProviderTypeCode, ProviderTypeName, [ProviderTypeCanSuperviseCase], [ProviderTypeIsOutsourced]) VALUES (15, 'BCBA', 'Board Certified Behavioral Analyst', 0, 1);
INSERT INTO dbo.ProviderTypes (ID, ProviderTypeCode, ProviderTypeName, [ProviderTypeCanSuperviseCase], [ProviderTypeIsOutsourced]) VALUES (17, 'AIDE', 'Aide', 0, 0);
SET IDENTITY_INSERT dbo.ProviderTypes OFF;
GO