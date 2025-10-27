namespace Toon
{
    public class ToonSerializerSettings
    {
        /// <summary>
        /// Number of spaces per indentation level.
        /// </summary>
        public byte Indent { get; set; }

        /// <summary>
        /// Delimiter to use for tabular array rows and inline primitive arrays.
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Optional marker to prefix array lengths in headers.
        /// </summary>
        /// <remarks>When set to #, arrays render as [#N] instead of [N]</remarks>
        public char? LengthMarker { get; set; }
    }
}
