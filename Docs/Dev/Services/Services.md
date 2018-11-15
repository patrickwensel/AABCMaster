# Services

Services describe what type of service the provider rendered to the patient.  Services are driven by insurance (meaning, certain insurances can only have certain services applied to a session), and are categorized by service type.

## Service Types

The list of service types are:

* Assessment
* Care
* General
* Management
* Social
* Supervision

(this list may be updated: see `dbo.ServiceTypesEnum` and/or `AABC.Domain2.Services.Services.cs` for an up to date list).

Certain rules in hours resolution may be driven by the type of service.

## Fixed Services

A `Fixed` service is legacy service that was in the system prior to user-service management capabilities and cannot be edited (editing these services may have negative effects on the codebase).  Non-fixed services can be managed by the user as desired.