using System;

namespace AABC.Domain2.Cases
{

    public enum CaseStatus
    {
        History = -1,
        NotReady = 0,
        Good = 1,
        ConsiderForDischarge = 2
    }

    [Flags]
    public enum CaseStatusReason
    {
        NotSet = 0,
        NoBCBA = 1,
        NoPrescription = 2,
        NoIntake = 4,
        AuthExpired = 8
    }

    public enum ContactMethodTypes
    {
        Phone = 0,
        Email = 1
    }
}
