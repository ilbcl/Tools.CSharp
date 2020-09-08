using System;
using System.Data;
using System.Data.Common;
using Tools.Extensions.Exceptions;

namespace Tools.Extensions
{
    public static class DataReaderExtensions
    {
        public static T GetValue<T>(this IDataReader reader, string columnName)
        {
            // Read the value out of the reader by string (column name); returns object
            object value;
            try
            {
                value = reader[columnName];
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new DataReaderColumnNotFoundException(columnName, $"Не удалось прочитать колонку '{columnName}' из DataReader'a", ex);
            }

            return Utility.ConvertToTypeOf<T>(value);
        }

        public static T GetFieldValue<T>(this DbDataReader reader, string columnName)
        {
            return reader.GetFieldValue<T>(reader.GetOrdinal(columnName));
        }
    }
}
