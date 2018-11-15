using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace AABC.DomainServices.Utils
{

    public static class EnumHelper
    {

        public static IEnumerable<ListItem<int>> ToSelectListItem<TType>() where TType : struct, IConvertible
        {
            var type = typeof(TType);
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{type.Name} must be an enum.");
            }
            var items = Enum.GetValues(type).Cast<TType>()
                            .Select(t => new ListItem<int>
                            {
                                Text = GetTextAsString(t as Enum),
                                Value = GetValueAsInt(t)
                            });
            return items.ToList();
        }


        public static string GetTextAsString(Enum obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var type = obj.GetType();
            var name = Enum.GetName(type, obj);
            var methods = new List<Func<Enum, string>>() { GetDisplay, GetDescription };
            var n = string.Empty;
            foreach (var f in methods)
            {
                n = f(obj);
                if (!string.IsNullOrEmpty(n))
                {
                    name = n;
                    break;
                }
            }
            return name;
        }


        private static int GetValueAsInt(object obj)
        {
            var type = obj.GetType();
            var test = Enum.Parse(type, obj.ToString()) as Enum;
            int x = Convert.ToInt32(test);
            return x;
        }


        private static string GetDisplay(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DisplayAttribute>()
                    ?.Name;
        }


        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }


        private static string GetValueAsString(object obj)
        {
            return GetValueAsInt(obj).ToString();
        }


    }
}
