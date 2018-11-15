using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Web.App.Providers
{
    public class ProviderCommandService
    {


        private Data.V2.CoreContext _context;
        private Repositories.IProviderRepository _repository;

        public ProviderCommandService(Data.V2.CoreContext context, Repositories.IProviderRepository repository) {

            _context = context;
            _repository = repository;
        }

    }
}