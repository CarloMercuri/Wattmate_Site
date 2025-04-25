using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Wattmate_Site.Utilities
{
    public static class DBUtils
    {
        public static string ConvertDateToDatabaseStandart(DateTime? time)
        {
            string standart = "";
            if (time != null)
                standart = Convert.ToDateTime(time).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return standart;
        }

        public static SqlParameter AddSqlParameter(string name, object value)
        {
            var dt = value as DateTime?;

            if (dt != null)
            {
                DateTime check = (DateTime)dt;

                if (check.Year < 2000)
                {
                    value = null;
                }
            }

            if (value != null)
            {
                return new SqlParameter(name, value);
            }
            else
            {
                return new SqlParameter(name, DBNull.Value);
            }
        }
        public static SqlParameter AddDateTimeSqlParameter(string name, DateTime value)
        {
            if (value != null)
            {
                return new SqlParameter(name, value);
            }
            else
            {
                return new SqlParameter(name, DBNull.Value);
            }
        }


        public static string GetDatabaseName(string connectionString)
        {
            string dbName = "";

            int firstIndex = connectionString.IndexOf("Database=");

            if (firstIndex == -1)
            {
                return dbName;
            }

            firstIndex = firstIndex + 9;

            int lastIndex = connectionString.IndexOf(';', firstIndex);

            if (lastIndex < 0)
            {
                dbName = connectionString.Substring(firstIndex);
            }
            else
            {
                dbName = connectionString.Substring(firstIndex, lastIndex - firstIndex);
            }

            return dbName;
        }

        public static string StringifyParameters(SqlParameter[] parameters)
        {
            string returnString = "";
            bool first = true;
            foreach (SqlParameter parameter in parameters)
            {
                string strng = "";
                if (parameter.Value is null)
                {
                    strng = "null";
                }
                else
                {
                    strng = parameter.Value.ToString();
                }

                if (first)
                {
                    returnString += strng;
                    first = false;
                }
                else
                {
                    returnString += strng;
                }
            }

            return returnString;
        }

        /// <summary>
        /// For billing purposes. Returns a datetime if it exists, null if it is null or empty, 
        /// and DateTime.MinValue if it is not a valid date or any other exeption occurs.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? FetchAsDateTimeSecure(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime dt))
                    {
                        return dt;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime? FetchAsDateTime(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime dt))
                    {
                        return dt;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DateTime FetchAsDateTime(object value, DateTime defaultDate)
        {
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime dt))
                    {
                        return dt;
                    }
                }

                return defaultDate;
            }
            catch (Exception ex)
            {
                return defaultDate;
            }
        }

        public static TimeSpan FetchAsTimeSpan(object value, TimeSpan _default)
        {
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    if (TimeSpan.TryParse(value.ToString(), out TimeSpan ts))
                    {
                        return ts;
                    }
                }

                return _default;
            }
            catch (Exception ex)
            {
                return _default;
            }
        }

        public static string FetchAsDateTimeString(object value, string format = "")
        {
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime dt))
                    {
                        if (format == "") return dt.ToString();
                        return dt.ToString(format);
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static long FetchAsLong(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt64(value);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static int FetchAsInt32(object value, int returnOnNull = 0)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return returnOnNull;
                }
                else
                {
                    return Convert.ToInt32(value);
                }
            }
            catch (Exception ex)
            {
                return returnOnNull;
            }
        }

        public static int? FetchAsInt32Nullable(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(value);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static long FetchAsInt64(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt64(value);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static long? FetchAsInt64Nullable(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt64(value);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool FetchAsEnumerator<T>(string value, out T result)
        {
            try
            {
                bool autoParseResult = Enum.TryParse(typeof(T), value, true, out object res);
                if (autoParseResult)
                {
                    result = (T)res;
                    return true;
                }

                // Attempt Manual
                foreach (string enumName in Enum.GetNames(typeof(T)))
                {
                    if (value.Equals(enumName))
                    {
                        result = (T)Enum.Parse(typeof(T), enumName);
                        return true;
                    }
                }
                result = default;
                return false;
            }
            catch (Exception ex)
            {
                result = default;
                return false;
            }
        }

        public static string FetchAsString(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return "";
                }
                else
                {
                    return Convert.ToString(value);
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string? FetchAsStringNullable(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToString(value);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool FetchAsBool(object value, bool returnTrueOnNull = false)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return returnTrueOnNull;
                }
                else
                {
                    return Convert.ToBoolean(value);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static decimal FetchAsDecimal(object value, bool returnException = false)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return 0M;
                }
                else
                {
                    return Convert.ToDecimal(value);
                }
            }
            catch (Exception ex)
            {
                if (returnException)
                {
                    throw ex;
                }
                else
                {
                    return 0M;
                }
            }
        }

        public static decimal? FetchAsDecimalNull(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDecimal(value);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static double FetchAsDouble(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return 0D;
                }
                else
                {
                    return Convert.ToDouble(value);
                }
            }
            catch (Exception ex)
            {

                return 0D;
            }
        }

        public static double? FetchAsDoubleNullable(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDouble(value);
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static float FetchAsFloat(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return 0f;
                }
                else
                {
                    return (float)Convert.ToDecimal(value);
                }
            }
            catch (Exception ex)
            {

                return 0f;
            }
        }

        public static float? FetchAsFloatNullable(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return (float)Convert.ToDecimal(value);
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }





    }
}
