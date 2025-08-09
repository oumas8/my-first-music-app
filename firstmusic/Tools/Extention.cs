using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace firstmusic.Tools
{
    internal static class CollectionExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
            where T : class
        {
            foreach (var i in source)
            {
                if (i is not null)
                    yield return i;
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
            where T : struct
        {
            foreach (var i in source)
            {
                if (i is not null)
                    yield return i.Value;
            }
        }

        public static T? ElementAtOrNull<T>(this IEnumerable<T> source, int index)
            where T : struct
        {
            var sourceAsList = source as IReadOnlyList<T> ?? source.ToArray();
            return index < sourceAsList.Count ? sourceAsList[index] : null;
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> source)
            where T : struct
        {
            foreach (var i in source)
                return i;

            return null;
        }
    }

    internal static class JsonExtensions
    {
        public static JsonElement? GetPropertyOrNull(this JsonElement element, string propertyName)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (
                element.TryGetProperty(propertyName, out var result)
                && result.ValueKind != JsonValueKind.Null
                && result.ValueKind != JsonValueKind.Undefined
            )
            {
                return result;
            }

            return null;
        }

        public static bool? GetBooleanOrNull(this JsonElement element) =>
            element.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null,
            };

        public static string? GetStringOrNull(this JsonElement element) =>
            element.ValueKind == JsonValueKind.String ? element.GetString() : null;

        public static int? GetInt32OrNull(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var result)
                ? result
                : null;

        public static long? GetInt64OrNull(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var result)
                ? result
                : null;

        public static JsonElement.ArrayEnumerator? EnumerateArrayOrNull(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Array ? element.EnumerateArray() : null;

        public static JsonElement.ArrayEnumerator EnumerateArrayOrEmpty(this JsonElement element) =>
            element.EnumerateArrayOrNull() ?? default;

        public static JsonElement.ObjectEnumerator? EnumerateObjectOrNull(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Object ? element.EnumerateObject() : null;

        public static JsonElement.ObjectEnumerator EnumerateObjectOrEmpty(this JsonElement element) =>
            element.EnumerateObjectOrNull() ?? default;

        public static IEnumerable<JsonElement> EnumerateDescendantProperties(
            this JsonElement element,
            string propertyName
        )
        {
            // Check if this property exists on the current object
            var property = element.GetPropertyOrNull(propertyName);
            if (property is not null)
                yield return property.Value;

            // Recursively check on all array children (if current element is an array)
            var deepArrayDescendants = element
                .EnumerateArrayOrEmpty()
                .SelectMany(j => j.EnumerateDescendantProperties(propertyName));

            foreach (var deepDescendant in deepArrayDescendants)
                yield return deepDescendant;

            // Recursively check on all object children (if current element is an object)
            var deepObjectDescendants = element
                .EnumerateObjectOrEmpty()
                .SelectMany(j => j.Value.EnumerateDescendantProperties(propertyName));

            foreach (var deepDescendant in deepObjectDescendants)
                yield return deepDescendant;
        }
    }
    internal static class StringExtensions
    {
        public static string? NullIfWhiteSpace(this string str) =>
            !string.IsNullOrWhiteSpace(str) ? str : null;

        public static string SubstringUntil(
            this string str,
            string sub,
            StringComparison comparison = StringComparison.Ordinal
        )
        {
            var index = str.IndexOf(sub, comparison);
            return index < 0 ? str : str[..index];
        }

        public static string SubstringAfter(
            this string str,
            string sub,
            StringComparison comparison = StringComparison.Ordinal
        )
        {
            var index = str.IndexOf(sub, comparison);

            return index < 0
                ? string.Empty
                : str.Substring(index + sub.Length, str.Length - index - sub.Length);
        }

        public static string StripNonDigit(this string str)
        {
            var buffer = new StringBuilder();

            foreach (var c in str.Where(char.IsDigit))
                buffer.Append(c);

            return buffer.ToString();
        }

        public static string Reverse(this string str)
        {
            var buffer = new StringBuilder(str.Length);

            for (var i = str.Length - 1; i >= 0; i--)
                buffer.Append(str[i]);

            return buffer.ToString();
        }

        public static string SwapChars(this string str, int firstCharIndex, int secondCharIndex) =>
            new StringBuilder(str)
            {
                [firstCharIndex] = str[secondCharIndex],
                [secondCharIndex] = str[firstCharIndex],
            }.ToString();

        public static int? ParseIntOrNull(this string str) =>
            int.TryParse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
                ? result
                : null;

        public static int ParseInt(this string str) =>
            ParseIntOrNull(str)
            ?? throw new FormatException($"Cannot parse integer number from string '{str}'.");

        public static long? ParseLongOrNull(this string str) =>
            long.TryParse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
                ? result
                : null;

        public static double? ParseDoubleOrNull(this string str) =>
            double.TryParse(
                str,
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out var result
            )
                ? result
                : null;

        public static TimeSpan? ParseTimeSpanOrNull(this string str, string[] formats) =>
            TimeSpan.TryParseExact(str, formats, DateTimeFormatInfo.InvariantInfo, out var result)
                ? result
                : null;

        public static DateTimeOffset? ParseDateTimeOffsetOrNull(this string str) =>
            DateTimeOffset.TryParse(
                str,
                DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.None,
                out var result
            )
                ? result
                : null;

        public static string ConcatToString<T>(this IEnumerable<T> source) => string.Concat(source);
    }
}
