# Hours Resolution Logging

This service adds logging capabilities to the Hours Resolution process. In order to understand why a particular hour entry was resolved to a particular authorization, or failed to be resolved, key input parameters and contextual data are being logged.

This information can be accessed through the Time And Billing Tab of the Case Manager, by clicking on the "Routing" link in the Actions column. All logged information connected to any hour entry is displayed in a modal window.


# Configuration

Hours Resolution Logging can be turned on/off through the application setting named "EnableResolutionServiceLogging" in the app config file.


# Data being logged

This information is being stored in a single flat table named CaseAuthHoursBreakdownLog. This table is not normalized and referential integrity is not enforced due to the static, historical nature of the data. The fields are:

    | Field                   | Data Type    | Description           |
    | ------------------------|--------------|-------------|
    | WasResolved             | Boolean      | Indicates if the resolution was successful. |
    | HoursID                 | int          | Id of the Hour Entry.  |
    | HoursDate               | DateTime     | Date of the Hour Entry being inserted. |
    | ServiceID               | int          | Id of the service linked to the Hour Entry. |
    | BillableHours           | decimal      | Amount of billable hours. |
    | ProviderTypeID          | int          | Id of the provider type linked to the hour entry. |
    | InsuranceID             | int          | Id of the insurance company linked to the hour entry. |
    | AuthMatchRuleDetailJSON | string (JSON)| Key fields of the rule that was matched. If no rule was matched, then it lists all the rules that were present at the time. |
    | ActiveAuthorizationsJSON| string (JSON)| Key fields of the active authorizations present at the time of the resolution. It is present only if the resolution found a matching rule, but it could not break down the hours. |
    | ResolvedAuthID          | int?         | Id of the authorization code to which the hours were applied. Present only if WasResolved is true. |
    | ResolvedAuthCode        | string       | Code of the authorization code to which the hours were applied. Present only if WasResolved is true. |
    | ResolvedCaseAuthID      | int?         | Id of the case authorization to wchi the hours were applied. Present only if WasResolved is true. |
    | ResolvedCaseAuthStart   | DateTime?    | Start date of the case authorization. Present only if WasResolved is true.   |
    | ResolvedCaseAuthEndDate | DateTime?    | End date of the case authorization. Present only if WasResolved is true.   |
    | ResolvedAuthMatchRuleID | int?         | Id of the matched rule |
    | ResolvedMinutes         | int?         | Number of minutes that were applied to this case authorization. Present only if WasResolved is true. |
1. 

# Implementation Details

The core functionality basically resides in the class HoursResolutionLoggingService, within AABC.DomainServices.

It uses plain SQL to optimize performance.

The resolution process does all data operations within a transaction. For that reason, all potential exceptions within the logging module are not propagated to the caller in order to avoid the transaction to be rolled back.

Also, worth noting is that rows are being added to the log regardless of the final outcome of the transaction. This means that the table might contain rows of resolution operations that were rolled back.


# Dependencies

The only class that actually uses HoursResolutionLoggingService is AuthorizationResolution. AuthorizationResolution will work just as fine even if the HoursResolutionLoggingService is null (although it won't log anything). 

HoursResolutionLoggingService has no dependencies on other modules.



