# Hours Resolution

Hours resolution is a complex process handled by the server.  It has to check multiple data points, including provider cross-hours, case cross-hours, active insurance, authorizations, services, etc.

Hours resolution is shared by multiple applications, including the Manage app (AABC.Web), the Provider Portal (AABC.ProviderPortal) and the Provider App (AABC.Mobile).

The Hours Resolution and related logic is contained in the `AABC.DomainServices.HoursResolution` namespace.

## Resolution Modes

Hours resolution can operate in a few different primary modes to handle varying consumption cases.  For example, hours entered by management staff have less strict validation requirements than those entered by providers, and hours entered by the Provider App may be pre-validation type of hours for the hours pre-check feature.  The Hours Resolution services inspect the validation modes to act accordingly.

## Resolution Return

The resolution process returns a `ValidationIssueCollection`, which is a list of issues categorized into different types:

* Error: prevents the entry from being processed
* Warning: allows processing of the entry, but gives a warning (typically used for management entries)

The caller can examine the returned issue collection to determine whether to discard the entry.

## General Process Steps

The following are the steps processed to validate an hours entry (by calling `HoursResolutionService.Resolve()`.  This can be processed for a single entry or a collection of entries.

**A. Normalize Core Entry**

1. Formats dates and times, records the entry app, calculates the total hours.
2. Resolve payable hours (equal to TotalHours if UseExplicitPayableValue is not flagged)
3. Resolve billable hours (equal to TotalHours if UseExplicitBillableValue is not flagged, except if it's a training entry, in which case billable hours are set to 0)

**B. Validate Core**

If the entry is an Social entry (more than one attending student):

1. Ensure the service is SSG
2. Ensure there's more than one case applied to the social entry
3. Ensure none of the cases in question are finalized (if management entry mode, this is a warning, otherwise it's an error)

If the entry is not a Social entry:
1. Checks that the entry date is not in the future, except for Precheck entries, which allows them up to X days in the future.
2. Ensures that the start time is less than the end time
3. Ensures that a service is applied to the hours
4. Validates the notes (for `EntryType.Full` (non-precheck) entries only, `Basic` entries bypass  note checks
5. Ensures the target period has not been finalized (warning for management entries, error for provider entries)

**C. Persist Entries to Temp Store**

At this point, all entries are saved to the database to ensure they have IDs which will be used for later validation steps.  Note that the database context in use here is a dedicated context and the entire operation should be wrapped in a transaction, as if there's a failure and the caller should determine the transaction not to be committed, they can roll back the context changes.

**D. Resolve SSG Entries**

(this must happen prior to case/provider validations/authorization checks)

For each of the proposed entries:

1. Remove any orphaned SSG records (this must happen for every entry, regardless of whether it's SSG: this prevents entries that were SSG and changed to a different service from maintaining it's linked cases)
2. If this entry isn't SSG, continue to the next proposed entry, else:
3. Create new entries for each SSG ID submitted with this entry.  This adds it to the internal ProposedEntry tracking (for later cross-validations) and also to the repository so they can be saved later with the package if the caller opts to save.  Note also that Payable Hours is set to 0 for each created entry.

At this point, all possible entries have been determined and are being tracked for later validation procedures.

**E. Case & Provider Validations**

For each of the proposed entries:

1. Provider Self-Overlap **(Ignored for BCBAs)**: Get all hours the provider has entered on this date (including other proposed entries) and validate that this won't create an overlap of times.  This check ignores any non-master SSG entries.  For management entries, this is a warning; for providers, an error.
2. No DR Overlap on Case **(Ignored for BCBAs and Management-Entered Training Entries)**: Validate the Aide isn't overlapping a DR (Direct Session) service already applied to the case.  This is a time-based overlap check.
3. Max Hours per Aide per Case per Day **(Ignored for BCBAs and Management-Entered Training Entries)**: Tallys all hours entered to this case for this provider and ensures that they are under the limit of X hours (4).  If a management entry, this is a warning; if provider entry, an error
4. BCBA Max Hours per Entry **(Ignored for non-BCBAs and ignored if it's an Assessment service type)**: If the entry total hours is over the limit, issue a warning (management entry) or an error (provider entry)
5. BCBA Max Assessment Hour per Case per Day **(Ignored for non-BCBAs, ignored if a training entry entered by management, ignored for non-Assessment type services)**: Obtain a list of all proposed case hours of Assessment services and tally them.  If over max, issue a warning (management entry) or error (provider entry).

**F. Authorization Resolution**

Authorization Breakdowns are not required for the entry and do not issue any `ValidationIssue`.  The purpose is to resolve these for authorization countdowns.

For each of the proposed entries:

1. Remove any existing breakdowns associated with the entry.
2. Determine the active insurance.  If no active insurance, abort breakdown for this entry.
3. Determine the Match Rules for this insurance.  If none, abort the breakdown for this entry.
4. Retrieve the Match Rules applicable to this entry's Service and Provider Type.  If none, abort the breakdown for this entry.
5. Retrieve a list of active authorizations for this case (based on the entry date and auth dates)
6. If this rule's *Initial* auth code is not set, or has no initial minimum minutes, or has no initial unit size, abort the process and move to the next proposed hours entry (no breakdowns applied to this entry)
7. If this rule's total minutes is less than the *Initial* match rule's minimum minutes, abort the process and move to the next proposed hours entry (no breakdowns applied to this entry)
8. Retrieve the active authorization from the case that matches the auth code of the initial authorization in the match rule.  If no matching active auth on file for the case, abort the process and proceed to the next proposed entry (no breakdown applied)
9. Create a new breakdown entry for the initial portion, applying the applicable initial minutes amount based on the total minutes of the entry (see Calculating Applicable Initial Minutes below)
10. Check if the Final auth is applicable (if none on file, or no min minutes or no unit size, exit the process and record only the initial auth)
11. Retrieve the active authorization from the case whose code matches the final auth rule set by the insurance.  If none, exit the process and record only the initial auth.
12. Create a new final auth breakdown record, applying the applicable final minutes amount based on the total minutes of the entry (see Calculating Applicable Final Minutes below)
13. Proceed to the next entry. 


*Calculating Applicable Initial Minutes*

The following algorithm is used to calculate the number of minutes to apply to an *Initial* authorization breakdown.

    let result = 0
    let remainder = totalMinutes MOD initialUnitSize
    let quotient = (totalMinutes / InitialUnitSize) * InitialUnitSize

    if (remainder < initialMinimumMinutes) {
        result = quotient
    } else {
        result = quotient + initialUnitSize
    }

    // if there's a final auth on file and the result is over our initial unit size
    // restrict this initial minutes to the unit size, otherwise let it overflow
    if (finalizeAuthorizationOnFile = true AND result > initialUnitSize) {
        result = initialUnitSize
    }
    return result

*Calculating Applicable Final Minutes*

The following algorithm is used to calculate the number of minutes to apply to a *Final* authorization breakdown:

    let result = 0
    let applicableMinutes = totalMinutes - applicableInitialMinutes

    if applicableMinutes < finalMinimumMinutes {
        result = 0
        return result
    }

    if (applicableMinutes <= finalUnitSize) {
        result = finalUnitSize
        return result
    }

    let remainder = applicableMinutes MOD finalUnitSize
    let quotient = (applicableMinutes / finalUnitSize) * finalUnitSize

    if (remainder < finalMinimumMinutes) {
        result = quotient
    } else {
        result = quotient + finalUnitSize
    }
    return result



**G. Schedule Resolution**

Currently schedule resolution is not implemented.  Intended use is for when provider schedule features are fully implemented and we want to restrict/warn if they're entering hours outside their schedule.

When this becomes implemented, we may opt to reposition this check within the resolution pipeline.



## Notes Validation

Validation of notes is unique among the hours resolution process in that it needs to be handled locally by the Provider App to support offline validation as well as the full server validation routines.  As such, the resolution validation is controlled by a regex string stored in the database where all applications can access a single setting that controls it. 




