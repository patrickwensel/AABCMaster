using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api
{
    public interface IDBContextProvider
    {
        Data.V2.CoreContext CoreContext { get; }
    }
}