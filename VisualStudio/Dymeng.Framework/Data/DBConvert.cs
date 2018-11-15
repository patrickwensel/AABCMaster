using System;

namespace Dymeng.Framework.Data
{
    public class DBConvert
    {
        
        
        public static int ToInt(Object obj) {
            return Int32.Parse(obj.ToString());
        }

        public static int? ToIntOrNull(Object obj) {
            if (obj == DBNull.Value) {
                return null;
            } else {
                return Int32.Parse(obj.ToString());
            }
        }



        public static short ToShort(object p) {
            return short.Parse(p.ToString());
        }

        public static short? ToShortOrNull(object o) {
            if (o == DBNull.Value) {
                return null;
            } else {
                return short.Parse(o.ToString());
            }
        }



        public static double ToDouble(Object obj) {
            return double.Parse(obj.ToString());
        }

        public static double? ToDoubleOrNull(Object obj) {
            if (obj == DBNull.Value) {
                return null;
            } else {
                return double.Parse(obj.ToString());
            }
        }
        
        


        public static string ToString(Object obj) {
            if (obj == DBNull.Value) {
                return null;
            }
            return obj.ToString();
        }

        
        

        public static bool ToBool(Object obj) {

            try {
                int i;
                bool isNumeric = int.TryParse(obj.ToString(), out i);

                if (isNumeric) {

                    if (i == 0) {
                        return false;
                    } else {
                        return true;
                    }

                } else {

                    string s = ToString(obj);

                    if (s == "TRUE" || s == "true" || s == "True" || s == "y" || s == "Y" || s == "YES" || s == "yes" || s == "Yes") {
                        return true;
                    }

                    return false;
                }
            }
            catch {
                return false;
            }
        }

        



        public static DateTime ToDateTime(Object obj) {
            return DateTime.Parse(obj.ToString());
        }
        
        public static DateTime? ToDateTimeOrNull(Object obj) {
            if (obj == DBNull.Value) {
                return null;
            } else {
                return DateTime.Parse(obj.ToString());
            }
        }




        
    }
}

