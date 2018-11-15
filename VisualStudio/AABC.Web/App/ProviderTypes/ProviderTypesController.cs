using AABC.Web.App.ProviderTypes.Models;
using System.Web.Mvc;

namespace AABC.Web.App.ProviderTypes
{
    public class ProviderTypesController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {
        private readonly IProviderTypeService service;

        public ProviderTypesController()
        {
            service = new ProviderTypeService(AppService.Current.DataContextV2);
        }


        [HttpGet]
        public JsonResult GetProviderTypes()
        {
            var providerTypes = service.GetProviderTypes();
            return this.CamelCaseJson(providerTypes, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetProviderSubTypes(int providerTypeId)
        {
            var providerSubTypes = service.GetProviderSubTypes(providerTypeId);
            return this.CamelCaseJson(providerSubTypes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult InsertProviderSubType(ProviderSubTypeDTO providerSubType)
        {
            var result = service.InsertProviderSubType(providerSubType);
            return this.CamelCaseJson(result);
        }


        [HttpPost]
        public JsonResult SaveProviderSubType(ProviderSubTypeDTO providerSubType)
        {
            var result = service.SaveProviderSubType(providerSubType);
            return this.CamelCaseJson(result);
        }


        [HttpPost]
        public JsonResult DeleteProviderSubType(int providerSubTypeId)
        {
            var result = service.DeleteProviderSubType(providerSubTypeId);
            return this.CamelCaseJson(result);
        }
    }
}