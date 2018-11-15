using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api.Mappings
{
    interface IDomainMapper<Domain, Local> : IDomainMapperReadonly<Domain, Local>
    {
        Domain ToDomain(Local source);
    }
}