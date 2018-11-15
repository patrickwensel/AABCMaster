using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;

namespace Dymeng.Framework.Web.Mvc.Controllers
{
    public class ModelStateHelper
    {


        public static List<ModelStateErrorItem> GetAllModelStateErrors(ModelStateDictionary modelState) {

            var errors = new List<ModelStateErrorItem>();

            int i = 0;

            var values = modelState.Values.ToList();
            var keys = modelState.Keys.ToList();

            foreach (var ms in values) {

                string key = keys[i];

                foreach (ModelError err in ms.Errors) {

                    var item = new ModelStateErrorItem();

                    item.ItemName = key;

                    if (ms.Value != null) {
                        item.AttemptedValue = ms.Value.AttemptedValue;
                    }
                    
                    if (string.IsNullOrEmpty(err.ErrorMessage)) {
                        // get exception message
                        item.ErrorMessage = err.Exception.ToString();
                    } else {
                        // use error message
                        item.ErrorMessage = err.ErrorMessage;    
                    }

                    errors.Add(item);

                }

                i++;

            }

            return errors;

        }

        public static string FormatModelStateErrorsForLogging(List<ModelStateErrorItem> items, string actionName, string controllerName, string user) {

            string s = "------------" + Environment.NewLine;

            foreach (var item in items) {

                s += "MODELSTATE ERROR: " + controllerName + ", " + actionName + "" + Environment.NewLine;
                s += "\tError:\t" + item.ErrorMessage + Environment.NewLine;
                s += "\tValue:\t" + item.ItemName + Environment.NewLine;
                s += "\tAttempt:\t" + item.AttemptedValue + Environment.NewLine;
                s += "\tTime:\t" + DateTime.UtcNow.ToString() + Environment.NewLine;
                s += "\tUser:\t" + user + Environment.NewLine;
            }

            return s;

        }

        public class ModelStateErrorItem
        {
            public string ErrorMessage { get; set; }
            public string ItemName { get; set; }
            public string AttemptedValue { get; set; }
        }

    }

}
