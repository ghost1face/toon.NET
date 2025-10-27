using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toon.Extensions;

namespace Toon
{
    /// <summary>
    /// Provides functionality to serialize objects or value types to TOON (Token-Oriented Object Notation) format.
    /// </summary>
    public class ToonSerializer
    {
        private readonly ToonSerializerSettings toonSerializerSettings;
        private static readonly Regex isNumericRegex = new Regex("^-?\\d+(\\.\\d+)?$");
        private static readonly Regex containsBracketsOrBraces = new Regex("[[\\]{}]");
        private static readonly Regex containsControlCharacters = new Regex("[\\n\\r\\t]");
        private static readonly Regex isValidUnquotedKeyRegex = new Regex("^[A-Z_][\\w.]*$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of <see cref="ToonSerializer"/> with default serialization settings.
        /// </summary>
        public ToonSerializer()
        {
            toonSerializerSettings = new ToonSerializerSettings
            {
                Indent = 2,
                Delimiter = Constants.Comma,
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ToonSerializer"/> with provided serialization settings.
        /// </summary>
        /// <param name="serializerSettings">The <see cref="ToonSerializerSettings"/> for serialization.</param>
        public ToonSerializer(ToonSerializerSettings serializerSettings)
        {
            toonSerializerSettings = serializerSettings;
        }

        /// <summary>
        /// Serializes the input object to the Toon serialization format.
        /// </summary>
        /// <param name="value">The input object.</param>
        /// <returns>A string representation of the serialized object.</returns>
        public string Serialize(object value)
        {
            if (Utils.IsJsonPrimitive(value))
                return EncodePrimitive(value, toonSerializerSettings.Delimiter);

            var writer = new LineWriter(toonSerializerSettings.Indent);

            if (Utils.IsArrayLike(value))
            {
                EncodeArray(null, value as IEnumerable, writer, 0, toonSerializerSettings);
            }
            else
            {
                EncodeObject(value, writer, 0, toonSerializerSettings);
            }

            return writer.ToString();
        }

        private static void EncodeObject(object? value, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            // this should not be possible since we handle nulls earlier
            // safety checks
            if (value == null)
                return;

            if (Utils.IsDictionary(value))
            {
                var dictionary = value as IDictionary;

                foreach (var key in dictionary!.Keys)
                {
                    if (key is null)
                        continue;

                    var theKey = key.ToString() ?? string.Empty;
                    var theValue = dictionary[theKey];

                    EncodeKeyValuePair(theKey, theValue, writer, depth, settings);
                }
            }
            else
            {
                var propertyData = value.GetSerializableProperties();
                if (propertyData == null || propertyData.Length == 0)
                    return;

                foreach (var property in propertyData)
                {
                    var theKey = property.SerializedName;
                    var theValue = value.GetValue(property.PropertyInfo.Name);

                    EncodeKeyValuePair(theKey, theValue, writer, depth, settings);
                }
            }
        }

        private static string FormatHeader(int length, string? key, string[]? fields, ToonSerializerSettings settings)
        {
            var theKey = key;
            var theFields = fields;
            var delimiter = settings.Delimiter;
            var lengthMarker = settings.LengthMarker == null ? settings.LengthMarker.ToString() : "";

            var header = new StringBuilder();

            if (theKey is not null)
            {
                header.Append(EncodeKey(theKey));
            }

            header.Append(Constants.OpenBracket)
                .Append(lengthMarker)
                .Append(length);

            if (delimiter != Delimiters.DefaultDelimiter)
            {
                header.Append(delimiter);
            }

            header.Append(Constants.CloseBracket);

            if (fields != null)
            {
                var quotedFields = fields.Select(EncodeKey);

                header
                    .Append(Constants.OpenBrace)
                    .Append(string.Join(delimiter, quotedFields))
                    .Append(Constants.CloseBrace);
            }

            header.Append(Constants.Colon);

            return header.ToString();
        }

        private static string FormatInlineArray(object[] primitiveValues, string? prefix, ToonSerializerSettings settings)
        {
            var header = FormatHeader(primitiveValues.Length, prefix, null, settings);

            // only add space if there are values
            if (primitiveValues.Length == 0)
            {
                return header;
            }

            var joinedValue = JoinEncodedValues(primitiveValues, settings.Delimiter);

            return $"{header} {joinedValue}";
        }

        private static string JoinEncodedValues(object?[] primitiveValues, char delimiter)
        {
            return string.Join(",", primitiveValues.Select(v => EncodePrimitive(v, delimiter)));
        }

        private static void EncodeKeyValuePair(string key, object? value, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var encodedKey = EncodeKey(key);

            if (Utils.IsJsonPrimitive(value))
            {
                writer.Push($"{encodedKey}: {EncodePrimitive(value, settings.Delimiter)}", depth);
            }
            else if (Utils.IsArrayLike(value))
            {
                EncodeArray(key, value as IEnumerable, writer, depth, settings);
            }
            // TODO: Handle Dictionary
            else
            {
                // object type
                // value is not null here because it is handled by IsJsonPrimitive
                var nestedPropertyData = value!.GetSerializableProperties();
                if (nestedPropertyData == null || nestedPropertyData.Length == 0)
                {
                    // empty object
                    writer.Push($"{encodedKey}:", depth);
                }
                else
                {
                    writer.Push($"{encodedKey}:", depth);

                    EncodeObject(value, writer, depth + 1, settings);
                }
            }
        }

        private static void EncodeArray(string? key, IEnumerable? enumerable, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            if (enumerable is null)
                return;

            var enumerated = enumerable.Cast<object>().ToArray();
            if (enumerated is null)
                return;

            if (enumerated.Length == 0)
            {
                var header = FormatHeader(0, key, null, settings);
                writer.Push(header, depth);

                return;
            }

            // primitive array
            if (Utils.IsArrayOfPrimitives(enumerated))
            {
                EncodeInlinePrimitiveArray(key, enumerated, writer, depth, settings);

                return;
            }

            // array of arrays (all primitives)
            if (Utils.IsArrayOfArrays(enumerated))
            {
                var allPrimitiveArrays = enumerated.All(arr =>
                {
                    if (arr is not IEnumerable enumerableArr)
                    {
                        return true;
                    }

                    var castedArr = enumerableArr.Cast<object>().ToArray();

                    return Utils.IsArrayOfPrimitives(castedArr);
                });

                if (allPrimitiveArrays)
                {
                    EncodeArrayOfArraysAsListItems(key, enumerated, writer, depth, settings);

                    return;
                }
            }

            // array of objects
            if (Utils.IsArrayOfObjects(enumerated))
            {
                var header = DetectTabularHeader(enumerated);
                if (header != null)
                {
                    EncodeArrayOfObjectsAsTabular(key, enumerated, header, writer, depth, settings);
                }
                else
                {
                    EncodeMixedArrayAsListItems(key, enumerated, writer, depth, settings);
                }

                return;
            }

            EncodeMixedArrayAsListItems(key, enumerated, writer, depth, settings);
        }

        private static string EncodeKey(string key)
        {
            if (IsValidUnquotedKey(key))
                return key;

            return $"{Constants.DoubleQuote}{key}{Constants.DoubleQuote}";
        }

        private static string EncodePrimitive(object? value, char delimiter)
        {
            switch (value)
            {
                case null:
                    return Constants.NullLiteral;
                case bool boolValue:
                    return boolValue ? bool.TrueString : bool.FalseString;
                default:
                    {
                        var val = value.ToString();

                        if (value.IsNumber())
                        {
                            return val!.ToString();
                        }

                        return val != null ? EncodeStringLiteral(val, delimiter) : Constants.NullLiteral;
                    }
            }
        }

        private static void EncodeMixedArrayAsListItems(string? prefix, object[] values, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var header = FormatHeader(values.Length, prefix, null, settings);
            writer.Push(header, depth);

            foreach (var value in values)
            {
                if (Utils.IsJsonPrimitive(value))
                {
                    // direct primitive as list item
                    writer.Push($"{Constants.ListItemPrefix}{EncodePrimitive(value, settings.Delimiter)}", depth + 1);
                }
                else if (Utils.IsArrayLike(value))
                {
                    if (value is not IEnumerable enumerableValue)
                    {
                        continue;
                    }

                    var enumerableOfObjects = enumerableValue.Cast<object>().ToArray();
                    if (Utils.IsArrayOfPrimitives(enumerableOfObjects))
                    {
                        var inline = FormatInlineArray(enumerableOfObjects, null, settings);

                        writer.Push($"{Constants.ListItemPrefix}{inline}", depth + 1);
                    }
                }
                else
                {
                    // object as a list item
                    EncodeObjectAsListItem(value, writer, depth + 1, settings);
                }
            }
        }

        private static void EncodeObjectAsListItem(object obj, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var objectProperties = obj.GetSerializableProperties();
            if (objectProperties.Length == 0)
            {
                writer.Push(Constants.ListItemMarker, depth);

                return;
            }

            // first key-value on the same line as "- "
            var firstObjectProperties = objectProperties[0];
            var firstKey = firstObjectProperties.SerializedName;
            var encodedKey = EncodeKey(firstKey);
            var firstValue = obj.GetValue(firstObjectProperties.PropertyInfo.Name);

            if (Utils.IsJsonPrimitive(firstValue))
            {
                writer.Push($"{Constants.ListItemPrefix}{encodedKey}: {EncodePrimitive(firstValue, settings.Delimiter)}", depth);
            }
            else if (Utils.IsArrayLike(firstValue))
            {
                var enumerableFirstValue = (firstValue as IEnumerable);
                if (enumerableFirstValue != null)
                {
                    var enumerableOfObjects = enumerableFirstValue.Cast<object>().ToArray();

                    if (Utils.IsArrayOfPrimitives(enumerableOfObjects))
                    {
                        var formatted = FormatInlineArray(enumerableOfObjects, firstKey, settings);

                        writer.Push($"{Constants.ListItemPrefix}{formatted}", depth);
                    }
                    else if (Utils.IsArrayOfObjects(enumerableOfObjects))
                    {
                        // Check if array of objects can use tabular format
                        var header = DetectTabularHeader(enumerableOfObjects);
                        if (header != null)
                        {
                            // Tabular format for uniform arrays of objects
                            var headerString = FormatHeader(enumerableOfObjects.Length, firstKey, header, settings);

                            writer.Push($"{Constants.ListItemPrefix}{headerString}", depth);

                            WriteTabularRows(enumerableOfObjects, header, writer, depth + 1, settings);
                        }
                        else
                        {
                            // fall back to list format for non-uniform arrays of objects
                            writer.Push($"{Constants.ListItemPrefix}{encodedKey}[{enumerableOfObjects.Length}]:", depth);

                            foreach (var item in enumerableOfObjects)
                            {
                                EncodeObjectAsListItem(item, writer, depth + 1, settings);
                            }
                        }
                    }
                    else if (Utils.IsArrayOfArrays(enumerableOfObjects))
                    {
                        // complex arrays on separate lines (array of arrays, etc.)
                        writer.Push($"{Constants.ListItemPrefix}{encodedKey}[{enumerableOfObjects.Length}]:", depth);

                        // encode array contents at depth + 1
                        foreach (var item in enumerableOfObjects)
                        {
                            if (Utils.IsJsonPrimitive(item))
                            {
                                writer.Push($"{Constants.ListItemPrefix}{EncodePrimitive(item, settings.Delimiter)}", depth + 1);
                            }
                            else if (Utils.IsArrayLike(item))
                            {
                                var itemArrayLike = (item as IEnumerable);
                                if (itemArrayLike != null)
                                {
                                    var itemArrayLikeOfObjects = itemArrayLike.Cast<object>().ToArray();

                                    if (Utils.IsArrayOfPrimitives(itemArrayLikeOfObjects))
                                    {
                                        var inline = FormatInlineArray(itemArrayLikeOfObjects, null, settings);

                                        writer.Push($"{Constants.ListItemPrefix}{inline}", depth + 1);
                                    }
                                }
                            }
                            else
                            {
                                // must be an object
                                EncodeObjectAsListItem(item, writer, depth + 1, settings);
                            }
                        }
                    }
                }
            }
            else
            {
                // must be an object/poco type
                // firstValue cannot be null here because IsJsonPrimitive handles nulls
                var nestedPropertyData = firstValue!.GetSerializableProperties();
                if (nestedPropertyData == null || nestedPropertyData.Length == 0)
                {
                    writer.Push($"{Constants.ListItemPrefix}{encodedKey}:", depth);
                }
                else
                {
                    writer.Push($"{Constants.ListItemPrefix}{encodedKey}:", depth);

                    EncodeObject(firstValue, writer, depth + 2, settings);
                }
            }

            // remaining keys on indented lines
            for (var i = 1; i < objectProperties.Length; i++)
            {
                var objectProperty = objectProperties[i];
                var theValue = obj.GetValue(objectProperty.PropertyInfo.Name);

                EncodeKeyValuePair(objectProperty.SerializedName, theValue, writer, depth + 1, settings);
            }
        }

        private static void EncodeInlinePrimitiveArray(string? prefix, object[] values, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var formatted = FormatInlineArray(values, prefix, settings);

            writer.Push(formatted, depth);
        }

        private static void EncodeArrayOfArraysAsListItems(string? prefix, object[] values, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var header = FormatHeader(values.Length, prefix, null, settings);

            writer.Push(header, depth);

            foreach (var arr in values)
            {
                var arrayOfPossiblePrimitives = ((IEnumerable)arr).Cast<object>().ToArray();

                if (Utils.IsArrayOfPrimitives(arrayOfPossiblePrimitives))
                {
                    var inline = FormatInlineArray(arrayOfPossiblePrimitives, null, settings);

                    writer.Push($"{Constants.ListItemPrefix}{inline}", depth + 1);
                }
            }
        }

        private static void EncodeArrayOfObjectsAsTabular(string? prefix, object[] data, string[] header, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            var headerStr = FormatHeader(data.Length, prefix, header, settings);

            writer.Push(headerStr, depth);

            WriteTabularRows(data, header, writer, depth + 1, settings);
        }

        private static void WriteTabularRows(object[] rows, string[] headers, LineWriter writer, int depth, ToonSerializerSettings settings)
        {
            PropertyData[]? propertyData = null;
            foreach (var row in rows)
            {
                if (propertyData == null)
                {
                    propertyData = row.GetSerializableProperties();
                }

                var values = propertyData.Select(p => row.GetValue(p.PropertyInfo.Name)).ToArray();
                if (values == null)
                    continue;

                var joinedValues = JoinEncodedValues(values, settings.Delimiter);

                writer.Push(joinedValues, depth);
            }
        }

        private static string[]? DetectTabularHeader(object[]? rowsOfData)
        {
            if (rowsOfData == null || rowsOfData.Length == 0)
                return null;

            var firstRow = rowsOfData[0];
            var firstProperties = firstRow.GetSerializableProperties();
            if (firstProperties == null || firstProperties.Length == 0)
                return null;

            if (IsTabularArray(rowsOfData, firstProperties))
            {
                return firstProperties.Select(p => p.SerializedName).ToArray();
            }

            return null;
        }

        private static bool IsTabularArray(object[] rowsOfData, PropertyData[] propertyData)
        {
            foreach (var row in rowsOfData)
            {
                var keys = row.GetPublicProperties();

                // all objects must have the same keys
                if (keys == null || keys.Length != propertyData.Length)
                    return false;

                // check that all header keys exist in the row and all values are primitives
                foreach (var key in propertyData)
                {
                    var propertyKey = key.PropertyInfo.Name;
                    if (!row.HasProperty(propertyKey))
                        return false;

                    var propertyValue = row.GetValue(propertyKey);
                    if (!Utils.IsJsonPrimitive(propertyValue))
                        return false;

                }
            }

            return true;
        }

        private static string EncodeStringLiteral(string value, char delimiter)
        {
            if (IsSafeUnquoted(value, delimiter))
                return value;


            return $"{Constants.DoubleQuote}{EscapeString(value)}{Constants.DoubleQuote}";
        }

        private static bool IsValidUnquotedKey(string key)
        {
            return isValidUnquotedKeyRegex.IsMatch(key);
        }

        private static bool IsSafeUnquoted(string value, char delimiter)
        {
            if (IsPaddedWithWhitespace(value))
                return false;

            if (value == Constants.TrueLilteral
                || value == Constants.FalseLilteral
                || value == Constants.NullLiteral)
                return false;

            if (IsNumeric(value))
                return false;

            if (IsStructural(value))
                return false;

            if (value.Contains(Constants.DoubleQuote) || value.Contains(Constants.BackSlash))
                return false;

            if (ContainsControlChars(value))
                return false;

            if (value.Contains(delimiter))
                return false;

            if (value.StartsWith(Constants.ListItemMarker.ToString()))
                return false;

            return true;
        }

        private static bool ContainsControlChars(string value)
        {
            return containsControlCharacters.IsMatch(value);
        }

        private static bool IsStructural(string value)
        {
            return containsBracketsOrBraces.IsMatch(value) || value.Contains(':');
        }

        private static bool IsNumeric(string value)
        {
            return isNumericRegex.IsMatch(value);
        }

        private static bool IsPaddedWithWhitespace(string value)
        {
            return value.Trim().Length != value.Length;
        }

        private static string EscapeString(string value)
        {
            return value
                .Replace("\\", $"{Constants.BackSlash}${Constants.BackSlash}")
                .Replace("\"", $"{Constants.BackSlash}${Constants.DoubleQuote}")
                .Replace("\n", $"{Constants.BackSlash}n")
                .Replace("\r", $"{Constants.BackSlash}r")
                .Replace("\t", $"{Constants.BackSlash}t");
        }
    }
}
