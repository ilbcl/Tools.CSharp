using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tools.Extensions.Exceptions
{
    [Serializable]
    public class DataReaderColumnNotFoundException : Exception
    {
        private const string COLUMN_NAME_KEY = "ColumnName";

        public DataReaderColumnNotFoundException() : base("При чтении данных из IDataReader не удалось найти колонку с указанным именем")
        {
        }

        public DataReaderColumnNotFoundException(string columnName) : base($"При чтении данных из IDataReader не удалось найти колонку с именем '{columnName}'")
        {
            ColumnName = columnName;
        }

        public DataReaderColumnNotFoundException(string columnName, string message) : base(message)
        {
            ColumnName = columnName;
        }

        public DataReaderColumnNotFoundException(string columnName, string message, Exception inner) : base(message, inner)
        {
            ColumnName = columnName;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected DataReaderColumnNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ColumnName = info.GetString(COLUMN_NAME_KEY);
        }

        public string ColumnName { get; protected set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(COLUMN_NAME_KEY, ColumnName);
            base.GetObjectData(info, context);
        }
    }
}