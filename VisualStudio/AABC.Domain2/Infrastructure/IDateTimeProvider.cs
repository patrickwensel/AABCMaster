using System;

namespace AABC.Domain2.Infrastructure
{
    public interface IDateTimeProvider
    {

        DateTime Now { get; }
        DateTime UtcNow { get; }

    }
}
