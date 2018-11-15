# Catalyst Import (Timesheets)

Entry point: App\ExternalData\Catalyst-Timesheet

Controller/Action:

* `ExternalData\TimesheetUpload`
* `ExternalData\CatalystTimesheetProcess(filename)`

Workhorse:

* `DomainServices.Integrations.Catalyst.TimesheetImporter(fullPath)`

**cata.ProviderMappings should be periodically manually checked for nulls**

**cata.CaseMappings should be periodically manually checked for nulls**

## Process Steps

General:
* Load columns to datatable
* Map columns to catalyst preload entities
* Remove any prior to last import date, validate notes are present
* Insert entities to catalyst preload table
* Run provider and cases mapping sprocs

Detailed:

1. Get the excel data into a DataTable (imports columns A-M, all later columns ignored)
2. Map columns to entities (see Mapped Columns below)
3. Remove entities prior to `Context.CatalystPreloadEnties.Max(x => x.ResponseDate)`
4. Remove entities without notes values
5. Insert rows to `Context.CatalystPreloadEntries`
6. Run MapProviders via `Context.CatalystProcedures.MapTimesheetProviders()` (see section)
7. Run MapCases via `Context.CatalystProcedures.MapTimesheetCases()` (see section)


### Mapped Columns

	ColIdx	Col	SourceColName		Destination
	--------------------------------------------------------
	5		F	Date				Date
	8		I	Notes				Notes
	11		L	Parent Agrees		Parent Agreed
	3		D	Student Name		Patient Name		[PatientFirstName] + [PatientLastName]
	9		J	ProviderAgrees		Provider Agreed
	4		E	Therapist			Provider Name		[ProviderLastName], [ProviderFirstName]
	1 		A	Response Date		Response Date

### MapProviders Steps

Via `cata.MapTimesheetProviders`

1. Inserts TimesheetProviderName to cata.ProviderMappings table, using frustrated join to insert only those that don't already exist.
2. Attempts to update ProviderIDs from dbo.Providers based on `[ProviderLastName], [ProviderFirstName] = TimesheetProviderName`
3. Updates Timesheet Preload with ProviderIDs from mappings table

**cata.ProviderMappings should be periodically manually checked for nulls**

### MapCases Steps

Via `cata.MapTimesheetCases`

1. Inserts TimesheetPatientName into cata.CaseMappings if they don't yet exist in the mapping table.
2. Tries to find the Case ID based on `PatientFirstName + PatientLastName = TimesheetPatientName` to fill any missing IDs
3. Updates Timesheet Preload with Case ID based on match to TimesheetPatientName

**cata.CaseMappings should be periodically manually checked for nulls**


## Integration with Provider Portal

Catalyst preload entries are not part of the CaseAuthHours system, but are brought into the list of Provider Portal calendar displays.  Upon "entering" the hours in the Provider Portal, the catalyst preload is set to resolved and the usual hour entry is created for the rest of the system.