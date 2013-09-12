using System.Globalization;

namespace Bio.Algorithms.Mismatcher
{
    public struct Mismatch
    {
        /// <summary>
        /// Gets or sets the length of the mismatched section in the reference sequence.
        /// </summary>
        public long ReferenceSequenceLength;


        /// <summary>
        /// Gets or sets the length of the mismatched section in the query sequence.
        /// </summary>
        public long QuerySequenceLength;

        /// <summary>
        /// Gets or sets the start index of this match in reference sequence.
        /// </summary>
        public long ReferenceSequenceOffset;

        /// <summary>
        /// Gets or sets start index of this match in query sequence.
        /// </summary>
        public long QuerySequenceOffset;

        /// <summary>
        /// Gets or sets the type of the mismatch
        /// </summary>
        public MismatchType Type;

        /// <summary>
        /// Converts RefStart, QueryStart, Length of Match to string.
        /// </summary>
        /// <returns>RefStart, QueryStart, Length.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resource.MismatchToStringFormat,
                              this.Type, this.ReferenceSequenceOffset, this.QuerySequenceOffset, this.ReferenceSequenceLength, this.QuerySequenceLength);
        }
    }
}