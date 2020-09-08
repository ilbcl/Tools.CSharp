using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Tools
{
    public static class Utility
    {
        private static readonly DateTime _minDbDate = new DateTime(1900, 1, 1);
        private static readonly Regex _internetExplorerBrowserTemplate = new Regex("(IE)|(InternetExplorer)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static T ConvertToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T ConvertToEnum<T>(string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T ConvertToTypeOf<T>(object value)
        {
            // Check for null value from the database
            if (value != null && !ReferenceEquals(value, DBNull.Value))
            {
                // Cast to the generic type applied to this method (i.e. int?)
                Type valueType = typeof(T);

                // We have a null, do we have a nullable type for T?
                if (!IsNullableType(valueType))
                {
                    // No, this is not a nullable type so just change the value's type from object to T
                    return (T)Convert.ChangeType(value, valueType);
                }
                else
                {
                    // Yes, this is a nullable type so change the value's type from object to the underlying type of T
                    NullableConverter theNullableConverter = new NullableConverter(valueType);

                    return (T)Convert.ChangeType(value, theNullableConverter.UnderlyingType);
                }
            }

            return default(T);
        }

        public static List<T> GetPortion<T>(List<T> source, int stageIndex, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            
            if (stageIndex < 0 || count <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int index = stageIndex * count;
            if (index >= source.Count)
            {
                return null;
            }

            int endPos = (stageIndex + 1) * count;

            if (endPos > source.Count)
            {
                endPos = source.Count;
            }

            return source.GetRange(index, endPos - index);
        }

        public static DateTime MinDbDate
        {
            get { return _minDbDate; }
        }

        public static bool IsDbDateAssigned(DateTime dt)
        {
            return dt.Date > _minDbDate;
        }

        public static bool IsDateTimeAssigned(DateTime dt)
        {
            return dt.Date > DateTime.MinValue; // MIN_DATE;
        }

        public static string ConvertDateToDbString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ConvertDateToString(object date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            DateTime? nullableDate = date as DateTime?;
            if (nullableDate != null)
            {
                return ConvertDateToString(nullableDate.Value);
            }

            return ConvertDateToString(Convert.ToDateTime(date));
        }

        public static string ConvertDateToString(DateTime? date)
        {
            return date.HasValue ? ConvertDateToString(date.Value) : string.Empty;
        }

        public static string ConvertDateToString(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static DateTime? ConvertToNullableDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(value);
        }

        public static DateTime? ConvertToNullableDate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return Convert.ToDateTime(value);
        }

        public static T ConvertTo<T>(object value)
        {
            Type valueType = typeof(T);

            if (value != DBNull.Value)
            {
                // We have a null, do we have a nullable type for T?
                if (!IsNullableType(valueType))
                {
                    // No, this is not a nullable type so just change the value's type from object to T
                    return (T)Convert.ChangeType(value, valueType);
                }
                else
                {
                    // Yes, this is a nullable type so change the value's type from object to the underlying type of T
                    NullableConverter converter = new NullableConverter(valueType);

                    return (T)Convert.ChangeType(value, converter.UnderlyingType);
                }
            }

            // The value was null in the database, so return the default value for T; this will vary based on what T is (i.e. int has a default of 0)
            return default(T);
        }

        public static string ConvertToMoneyWithTriads(decimal value)
        {
            string result = value.ToString("0,0.00", CultureInfo.GetCultureInfo("ru")).TrimStart('0');
            if (result.StartsWith(","))
            {
                return "0" + result;
            }

            return result;
        }

        public static string ConvertToMoneyWithTriads4Chrs(decimal value)
        {
            string result = value.ToString("0,0.0000", CultureInfo.GetCultureInfo("ru")).TrimStart('0');
            if (result.StartsWith(","))
            {
                return "0" + result;
            }

            return result;
        }

        public static string ConvertToMoneyWithTriads3Chrs(decimal value)
        {
            string result = value.ToString("0,0.000", CultureInfo.GetCultureInfo("ru")).TrimStart('0');
            if (result.StartsWith(","))
            {
                return "0" + result;
            }

            return result;
        }

        public static string ConvertToString(decimal? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Value.ToString(CultureInfo.InvariantCulture).TrimEnd('0', ',');
        }

        public static string RemoveRedundantWhitespaces(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            // Удаляем ненужные пробелы
            Regex regex = new Regex("\\s{2,}", RegexOptions.Compiled);
            return regex.Replace(value.Trim(), " ");
        }

        public static string RemoveWhitespaces(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            // Удаляем ненужные пробелы
            Regex regex = new Regex("\\s+", RegexOptions.Compiled);
            return regex.Replace(value, string.Empty);
        }

        public static decimal ConvertToDecimal(string value)
        {
            decimal number = 0m;
            if (!string.IsNullOrWhiteSpace(value))
            {
                decimal.TryParse(RemoveWhitespaces(value.Replace(',', '.')), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
            }

            return number;
        }

        public static int ConvertToInt32(string value)
        {
            int number = 0;
            if (!string.IsNullOrWhiteSpace(value))
            {
                int.TryParse(RemoveWhitespaces(value), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
            }

            return number;
        }

        public static int GetPercent(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                // Находим значение процента в строке
                Regex regex = new Regex(@"(\d+)\%", RegexOptions.Compiled);
                Match match = regex.Match(value);
                if (match.Success)
                {
                    return int.Parse(match.Groups[1].Value);
                }
            }

            return 0;
        }

        public static bool IsNullableType(Type valueType)
        {
            return valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsInternetExplorer(string browser)
        {
            return _internetExplorerBrowserTemplate.IsMatch(browser);
        }

        public static Range<T> GetNewIdentitiesRange<T>(string sequenceName, int identitiesCount, SqlConnection connection, SqlTransaction transaction = null) where T : IComparable
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sys.sp_sequence_get_range";
                command.Parameters.AddWithValue("@sequence_name", sequenceName);
                command.Parameters.AddWithValue("@range_size", identitiesCount);

                //Specify an output parameter to retreive the first value of the generated range.
                SqlParameter firstValueInRange = new SqlParameter("@range_first_value", SqlDbType.Variant);
                SqlParameter lastValueInRange = new SqlParameter("@range_last_value", SqlDbType.Variant);

                firstValueInRange.Direction = ParameterDirection.Output;
                lastValueInRange.Direction = ParameterDirection.Output;

                command.Parameters.Add(firstValueInRange);
                command.Parameters.Add(lastValueInRange);

                command.ExecuteNonQuery();
                return new Range<T>(ConvertToTypeOf<T>(firstValueInRange.Value), ConvertToTypeOf<T>(lastValueInRange.Value));
            }
        }

        public static string MakeXmlFromModel<T>(T model) where T : class
        {
            string xml = string.Empty;

            if (null != model)
            {
                StringBuilder sb = new StringBuilder();

                using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer, model);
                }

                xml = sb.ToString();
            }

            return xml;
        }

        public static string MakeXmlFromModelV2<T>(T obj, bool allowNamespaces = true) where T : class
        {
            string xml = string.Empty;

            if (null != obj)
            {
                StringBuilder sb = new StringBuilder();

                using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    if (!allowNamespaces)
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        serializer.Serialize(writer, obj, ns);
                    }
                    else
                        serializer.Serialize(writer, obj);
                }

                xml = sb.ToString();
            }

            return xml;
        }

        public static T MakeModelFromXml<T>(string xml) where T : class
        {
            T model = default(T);

            if (null != xml)
            {

                using (StringReader input = new StringReader(xml))
                using (XmlTextReader reader = new XmlTextReader(input))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    model = (T)serializer.ReadObject(reader);
                }
            }

            return model;
        }

        public static T MakeModelFromXmlV2<T>(string xml) where T : class
        {
            T model = default(T);

            if (null != xml)
            {
                using (StringReader input = new StringReader(xml))
                //using (XmlTextReader reader = new XmlTextReader(input))
                using (XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    model = (T)serializer.Deserialize(reader);
                }
            }

            return model;
        }

        public static string MakeJsonFromModel<T>(T model) where T : class
        {
            string json = string.Empty;

            if (null != model)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(stream, model);
                    json = Encoding.UTF8.GetString(stream.GetBuffer());
                }
            }

            return json;
        }

        public static T MakeModelFromJson<T>(string json) where T : class
        {
            T model = default(T);

            if (null != json)
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    model = (T)serializer.ReadObject(stream);
                }
            }

            return model;
        }
    }
    // for async Task ref out
    public static class Wrapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Wrapper<T> Create<T>(T value) => new Wrapper<T>(value);
    }
    public class Wrapper<T>
    {
        public Wrapper() { }
        public Wrapper(T value) { Value = value; }
        public T Value { get; set; }
        public static implicit operator T(Wrapper<T> wrap) => wrap.Value;
        public static implicit operator Wrapper<T>(T value) => new Wrapper<T>(value);
    }
}
