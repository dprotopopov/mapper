using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NDbfReader;

namespace Mapper.Extensions
{
    public static class PgExtensions
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

        public static string TextEscape(this string s, int doubleQuotes = 0)
        {
            s = s.Replace("\\", @"\\");

            switch (doubleQuotes)
            {
                case 1:
                    s = s.Replace("\\", @"\\").Replace("\"", @"\""");
                    break;
                case 2:
                    s = s.Replace("\\", @"\\\\").Replace("\"", @"\\\""");
                    break;
                case 4:
                    s = s.Replace("\\", @"\\\\\\").Replace("\"", @"\\\\""");
                    break;
                default:
                    s = s.Replace("\\", @"\\");
                    break;
            }

            s = s.Replace("\'", @"\'")
                    .Replace("\r", @"\r")
                    .Replace("\n", @"\n")
                    .Replace("\t", @"\t")
                    .Replace("\a", @"\a")
                    .Replace("\b", @"\b")
                    .Replace("\f", @"\f")
                    .Replace("\v", @"\v")
                    .Replace("\0", @"\0")
                ;

            return s;
        }

        public static string ValueAsText(this double? value)
        {
            return value?.ToString("G", CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static string ValueAsText(this DateTime? value)
        {
            return value != null ? $"{value.Value.ToString("u", CultureInfo.InvariantCulture)}" : string.Empty;
        }

        public static string ValueAsText(this string value)
        {
            return value != null ? $"{value.TextEscape()}" : string.Empty;
        }
    }
}