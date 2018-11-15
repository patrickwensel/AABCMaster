using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AABC.Mobile.Api.Providers
{
    class DataStringProvider
    {

        public static string GetDataString(object input) {

            var serializer = new JsonSerializer();

            StringBuilder stringBuilder = new StringBuilder();

            using (var stringWriter = new System.IO.StringWriter(stringBuilder)) {
                serializer.Serialize(stringWriter, input);
            }

            return stringBuilder.ToString();

        }

    }
}