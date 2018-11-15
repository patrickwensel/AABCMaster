/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 1.8.4.0
dym:TargetEndingVersion: 1.8.5.0
---------------------------------------------------------------------

	
	MOOP basics, 
	insurance credentials required flag
	
---------------------------------------------------------------------*/



alter table insurances add RequireCredentialsForBCBA bit
go
Create Table CaseInsurancesMaxOutOfPocket (
	Id int identity primary key not null,
	CaseInsuranceId int not null,
	MaxOutOfPocket money null,
	EffectiveDate DateTime null
)
go
ALTER TABLE dbo.CaseInsurancesMaxOutOfPocket ADD CONSTRAINT
	FK_CaseInsurancesMaxOutOfPocket_CaseInsurances FOREIGN KEY
	(
	CaseInsuranceId
	) REFERENCES dbo.CaseInsurances
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
go




GO
EXEC meta.UpdateVersion '1.8.5.0';
GO

