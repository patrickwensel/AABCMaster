using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AABC.ProviderPortal.App.Finalize
{
    public class FinalizeController : Controller
    {

        [HttpGet]
        public DateFormattedJsonResult GetDeepValidationFailures(int caseID, int providerID, int periodYear, int periodMonth) {
            
            bool checkHasData = AppService.Current.Settings.FinalizeDeepValidationHasDataRequired;

            var model = new List<Models.ValidationItem>();
            if (AppService.Current.Settings.FinalizeDeepValidationRequired) {
                model = _service.GetValidationFailures(caseID, new DateTime(periodYear, periodMonth, 1), providerID, checkHasData);
            }             
            return new DateFormattedJsonResult { Data = model };
        }


        private FinalizeService _service;

        public FinalizeController() {
            _service = new FinalizeService();
        }

    }
}