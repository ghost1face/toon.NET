namespace Toon
{
    /// <summary>
    /// Provides options to be used with <see cref="ToonSerializer"/>.
    /// </summary>
    public class ToonSerializerSettings
    {
        /// <summary>
        /// Number of spaces per indentation level.
        /// </summary>
        public byte Indent { get; set; } = 2;

        /// <summary>
        /// Delimiter to use for tabular array rows and inline primitive arrays.
        /// </summary>
        public char Delimiter { get; set; } = Delimiters.DefaultDelimiter;

        /// <summary>
        /// Optional marker to prefix array lengths in headers.
        /// </summary>
        /// <remarks>When set to #, arrays render as [#N] instead of [N]</remarks>
        public char? LengthMarker { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the policy used to convert a property's name on an object to another format, such as
        /// camel-casing, or `null` to leave property names unchanged.
        /// </summary>
        public ToonNamingPolicy? PropertyNamingPolicy { get; set; }
    }
}
