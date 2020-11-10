using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NDbfReader;
using Npgsql;

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

        public static string TextEscape(this string s, int mode = 0)
        {
            switch (mode)
            {
                case 0:
                    return s.Replace("\\", @"\\")
                        .Replace("\'", @"\'")
                        .Replace("\r", @"\r")
                        .Replace("\n", @"\n")
                        .Replace("\t", @"\t")
                        .Replace("\a", @"\a")
                        .Replace("\b", @"\b")
                        .Replace("\f", @"\f")
                        .Replace("\v", @"\v")
                        .Replace("\0", @"\0");
                case 1:
                    return s.Replace("\\", @"\\")
                        .Replace("\"", @"\""")
                        .Replace("\'", @"\'")
                        .Replace("\r", @"\r")
                        .Replace("\n", @"\n")
                        .Replace("\t", @"\t")
                        .Replace("\a", @"\a")
                        .Replace("\b", @"\b")
                        .Replace("\f", @"\f")
                        .Replace("\v", @"\v")
                        .Replace("\0", @"\0");
                case 2:
                    return s.Replace("\\", @"\\\\")
                        .Replace("\"", @"\\\""")
                        .Replace("\'", @"\\\'")
                        .Replace("\r", @"\\\r")
                        .Replace("\n", @"\\\n")
                        .Replace("\t", @"\\\t")
                        .Replace("\a", @"\\\a")
                        .Replace("\b", @"\\\b")
                        .Replace("\f", @"\\\f")
                        .Replace("\v", @"\\\v")
                        .Replace("\0", @"\\\0");
                case 4:
                    return s.Replace("\\", @"\\\\\\\\")
                        .Replace("\"", @"\\\\\\\""")
                        .Replace("\'", @"\\\\\\\'")
                        .Replace("\r", @"\\\\\\\r")
                        .Replace("\n", @"\\\\\\\n")
                        .Replace("\t", @"\\\\\\\t")
                        .Replace("\a", @"\\\\\\\a")
                        .Replace("\b", @"\\\\\\\b")
                        .Replace("\f", @"\\\\\\\f")
                        .Replace("\v", @"\\\\\\\v")
                        .Replace("\0", @"\\\\\\\0");
                default:
                    throw new NotImplementedException();
            }

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

        public static string SafeGetString(this NpgsqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}