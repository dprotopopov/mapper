using System;
using System.Globalization;
using NDbfReader;

namespace FiasServer.Extensions
{
    public static class DbfExtensions
    {
        private const string Null = "null";

        public static string TypeAsText(this IColumn column)
        {
            if (column.Type == typeof(string))
                return $"VARCHAR({column.Size})";
            if (column.Type == typeof(decimal?))
                return $"NUMERIC({column.Size})";
            if (column.Type == typeof(DateTime?))
                return "TIMESTAMP";
            if (column.Type == typeof(int))
                return "INTEGER";
            if (column.Type == typeof(bool))
                return "BOOLEAN";

            throw new NotImplementedException();
        }

        public static string ValueAsText(this IColumn column, Reader reader)
        {
            if (column.Type == typeof(string))
                return reader.GetString(column) != null
                    ? reader.GetString(column).TextEscape()
                    : string.Empty;
            if (column.Type == typeof(decimal?))
                return reader.GetDecimal(column).HasValue
                    ? reader.GetDecimal(column).Value.ToString("G", CultureInfo.InvariantCulture)
                    : string.Empty;
            if (column.Type == typeof(DateTime?))
                return reader.GetDateTime(column).HasValue
                    ? reader.GetDateTime(column).Value.ToString("u", CultureInfo.InvariantCulture)
                    : string.Empty;
            return reader.GetValue(column) != null
                ? reader.GetValue(column).ToString()
                : string.Empty;
        }
        public static string ValueAsString(this IColumn column, Reader reader)
        {
            if (column.Type == typeof(string))
                return reader.GetString(column) != null
                    ? $"'{reader.GetString(column).StringEscape()}'"
                    : Null;
            if (column.Type == typeof(decimal?))
                return reader.GetDecimal(column).HasValue
                    ? reader.GetDecimal(column).Value.ToString("G", CultureInfo.InvariantCulture)
                    : Null;
            if (column.Type == typeof(DateTime?))
                return reader.GetDateTime(column).HasValue
                    ? $"'{reader.GetDateTime(column).Value.ToString("u", CultureInfo.InvariantCulture)}'"
                    : Null;
            return reader.GetValue(column) != null
                ? reader.GetValue(column).ToString()
                : Null;
        }

        public static string TextEscape(this string s)
        {
            return s
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\v", "\\v")
                ;
        }
        public static string StringEscape(this string s)
        {
            return s
                    .Replace("'", "''")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\v", "\\v")
                ;
        }
    }
}