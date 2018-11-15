using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace AABC.Shared.Web.Helpers
{
    public class SelectListHelper
    {
        public static SelectList GetSelectList<T>(string defaultText = null)
        {
            Array values = Enum.GetValues(typeof(T));

            var items = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(defaultText))
            {
                items.Add(defaultText, string.Empty);
            }

            foreach (object value in values)
            {
                T unknown = (T)value;
                var o = (Enum)Enum.Parse(typeof(T), value.ToString());
                items.Add(o.GetDescription(), Convert.ToInt32(unknown).ToString(CultureInfo.InvariantCulture));
            }

            return new SelectList(items, "Value", "Key");
        }
    }
}
