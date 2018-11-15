using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Data.V2;

namespace AABC.Mobile.Api
{
    public class DBContextProvider : IDBContextProvider
    {

        public CoreContext CoreContext {
            get {
                if (!HttpContext.Current.Items.Contains(nameof(CoreContext))) {
                    HttpContext.Current.Items.Add(nameof(CoreContext), new CoreContext());
                }
                return HttpContext.Current.Items[nameof(CoreContext)] as CoreContext;
            }
        }
        
    }
}