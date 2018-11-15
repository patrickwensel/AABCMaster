using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.App.Staffing.Models
{

    [ModelBinder(typeof(ScheduleRequestBinder))]
    public class ScheduleRequestVM
    {
        public const int GROUP_COUNT = 3;
        public const int GROUP_SIZE = 7;

        public bool[] Days { get; set; }
        public bool[] AM { get; set; }
        public bool[] PM { get; set; }

        public ScheduleRequestVM()
        {
            Days = new bool[GROUP_SIZE];
            AM = new bool[GROUP_SIZE];
            PM = new bool[GROUP_SIZE];
        }

        public int ToInt()
        {
            var c = new List<bool>();
            if (Days != null)
            {
                c.AddRange(Days);
            }
            if (AM != null)
            {
                c.AddRange(AM);
            }
            if (PM != null)
            {
                c.AddRange(PM);
            }
            return ToInt(new BitArray(c.ToArray()));
        }

        private static int ToInt(BitArray bitArray)
        {
            if (bitArray.Length > 32)
            {
                throw new ArgumentException("Argument length shall be at most 32 bits.");
            }

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static ScheduleRequestVM FromInt(int val)
        {

            var maxLength = GROUP_COUNT * GROUP_SIZE;
            var s = Convert.ToString(val, 2);
            if (s.Length > maxLength)
            {
                throw new ArgumentException($"value cannot be greater than ${((int)Math.Pow(2, maxLength) - 1).ToString()}");
            }
            var arr = Convert.ToString(val, 2)
                             .PadLeft(maxLength, '0')
                             .Reverse()
                             .Select(c => int.Parse(c.ToString()) != 0)
                             .ToArray();
            var groups = arr
                        .Select((x, i) => new { x, i })
                        .GroupBy(i => i.i / GROUP_SIZE, x => x.x)
                        .Select(g => g.ToArray())
                        .ToArray();
            var model = new ScheduleRequestVM();
            model.Days = groups[0];
            model.AM = groups[1];
            model.PM = groups[2];
            return model;
        }
    }

    public class ScheduleRequestBinder : DefaultModelBinder
    {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ScheduleRequestVM))
            {
                var data = new Dictionary<string, List<bool>>();
                data["Days"] = new List<bool>();
                data["AM"] = new List<bool>();
                data["PM"] = new List<bool>();
                for (var i = 0; i < ScheduleRequestVM.GROUP_SIZE; i++)
                {
                    foreach (var m in bindingContext.PropertyMetadata)
                    {
                        var key = m.Key;
                        var value = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.{key}[{i}]");
                        var str = value != null && !string.IsNullOrEmpty(value.AttemptedValue) ? value.AttemptedValue : "false";
                        var boolvalue = Convert.ToBoolean(str);
                        data[key].Add(boolvalue);
                    }
                }
                return new ScheduleRequestVM
                {
                    Days = data["Days"].ToArray(),
                    AM = data["AM"].ToArray(),
                    PM = data["PM"].ToArray()
                };
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }

    }
}