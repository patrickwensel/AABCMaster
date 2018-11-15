using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Domain2.Providers;

namespace AABC.Mobile.Api
{
    public interface ICurrentUserProvider
    {
        int AspNetUserID { get; }
        int ProviderID { get; }
        Provider Provider { get; }
    }
}