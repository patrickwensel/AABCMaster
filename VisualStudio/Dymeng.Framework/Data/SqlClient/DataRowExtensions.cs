using System;

using System.Data;

namespace Dymeng.Framework.Data.SqlClient
{
    public static class DataRowExtensions
    {



        public static string ToStringValue(this DataRow row, string fieldName) {
            return DBConvert.ToString(row[fieldName]);
        }
        public static string ToStringValue(this DataRow row, int fieldIndex) {
            return DBConvert.ToString(row[fieldIndex]);
        }

        public static int ToInt(this DataRow row, string fieldName) {
            return DBConvert.ToInt(row[fieldName]);
        }
        public static int ToInt(this DataRow row, int fieldIndex) {
            return DBConvert.ToInt(row[fieldIndex]);
        }

        public static int? ToIntOrNull(this DataRow row, string fieldName) {
            return DBConvert.ToIntOrNull(row[fieldName]);
        }
        public static int? ToIntOrNull(this DataRow row, int fieldIndex) {
            return DBConvert.ToIntOrNull(row[fieldIndex]);
        }

        public static double ToDouble(this DataRow row, string fieldName) {
            return DBConvert.ToDouble(row[fieldName]);
        }
        public static double ToDouble(this DataRow row, int fieldIndex) {
            return DBConvert.ToDouble(row[fieldIndex]);
        }

        public static double? ToDoubleOrNull(this DataRow row, string fieldName) {
            return DBConvert.ToDoubleOrNull(row[fieldName]);
        }
        public static double? ToDoubleOrNull(this DataRow row, int fieldIndex) {
            return DBConvert.ToDoubleOrNull(row[fieldIndex]);
        }

        public static DateTime ToDateTime(this DataRow row, string fieldName) {
            return DBConvert.ToDateTime(row[fieldName]);
        }
        public static DateTime ToDateTime(this DataRow row, int fieldIndex) {
            return DBConvert.ToDateTime(row[fieldIndex]);
        }

        public static DateTime? ToDateTimeOrNull(this DataRow row, string fieldName) {
            return DBConvert.ToDateTimeOrNull(row[fieldName]);
        }
        public static DateTime? ToDateTimeOrNull(this DataRow row, int fieldIndex) {
            return DBConvert.ToDateTimeOrNull(row[fieldIndex]);
        }

        public static bool ToBool(this DataRow row, string fieldName) {
            return DBConvert.ToBool(row[fieldName]);
        }
        public static bool ToBool(this DataRow row, int fieldIndex) {
            return DBConvert.ToBool(row[fieldIndex]);
        }

        public static short ToShort(this DataRow row, string fieldName) {
            return DBConvert.ToShort(row[fieldName]);
        }
        public static short ToShort(this DataRow row, int fieldIndex) {
            return DBConvert.ToShort(row[fieldIndex]);
        }
        public static char? ToCharOrNull(this DataRow row, string fieldName)
        {
            var val = DBConvert.ToString(row[fieldName]);
            return string.IsNullOrEmpty(val) ? default(char?) : val[0];
        }

    }
}
