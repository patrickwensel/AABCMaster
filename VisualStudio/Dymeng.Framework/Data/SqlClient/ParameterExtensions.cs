using Dymeng.Framework.Strings;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dymeng.Framework.Data.SqlClient
{

    public struct ParameterInfo
    {
        public object Value { get; set; }
        public bool Nullable { get; set; }
    }


    public static class ParameterExtensions
    {

        const string ALL_PARAMS = "@[ALLPARAMS]";

        
        public static SqlParameter AddWithNullableValue(this SqlParameterCollection collection, string parameterName, object value) {

            if (value == null) {
                return collection.AddWithValue(parameterName, DBNull.Value);
            } else {
                return collection.AddWithValue(parameterName, value);
            }
        }



        public static void AddParameters(this SqlCommand cmd, ParameterInfo[] paramInfo) {

            // could come in as @[ALLPARAMS] (for inserts)
            // could come in as @ throughout: "@ " "@," "@;" "@)" (updates and other non-specified
            // place parameters in order of paramInfo indices

            if (cmd.CommandText.IndexOf(ALL_PARAMS) > 0) {
                addParamsInsertStyle(cmd, paramInfo);
            } else {
                addParamsUpdateStyle(cmd, paramInfo);
            }
        }


        static void addParamsUpdateStyle(SqlCommand cmd, ParameterInfo[] pi) {

            // here we can't use regular index methods to find the places
            // (could be searching for @ @, @; or @) so standard methods are
            // non-deterministic

            // get indices of unnammed params, pull the lowest one and handle it
            for (int i = 0; i < pi.Length; i++) {

                try {

                    int a = cmd.CommandText.IndexOf("@ ");
                    int b = cmd.CommandText.IndexOf("@,");
                    int c = cmd.CommandText.IndexOf("@;");
                    int d = cmd.CommandText.IndexOf("@)");

                    int[] idxs = { a, b, c, d };
                    int idx = idxs.Where(x => x >= 0).Min();

                    cmd.CommandText = cmd.CommandText.ReplaceAtIndex(idx, "@" + i.ToString(), 1);

                    if (pi[i].Nullable) {
                        cmd.Parameters.AddWithNullableValue("@" + i.ToString(), pi[i].Value);
                    } else {
                        cmd.Parameters.AddWithValue("@" + i.ToString(), pi[i].Value);
                    }
                } catch (Exception e) {
                    Exceptions.Handle(e);
                    throw e;
                }


            }
        }


        static void addParamsInsertStyle(SqlCommand cmd, ParameterInfo[] pi) {

            // generate list of params for text insertions
            string temp = "";

            for (int i = 0; i < pi.Length; i++) {

                temp += ",@" + i.ToString();

                if (pi[i].Nullable) {
                    cmd.Parameters.AddWithNullableValue("@" + i.ToString(), pi[i].Value);
                } else {
                    cmd.Parameters.AddWithValue("@" + i.ToString(), pi[i].Value);
                }

            }

            temp = temp.Substring(1);   // remove trailing comma

            cmd.CommandText = cmd.CommandText.Replace(ALL_PARAMS, temp);

        }






    }




}
