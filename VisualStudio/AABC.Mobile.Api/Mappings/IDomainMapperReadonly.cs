using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api.Mappings
{
    interface IDomainMapperReadonly<Domain, Local>
    {
        Local FromDomain(Domain source);
    }
}