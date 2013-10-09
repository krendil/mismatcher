using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.Mismatcher
{
	public class Mismatcher
	{

		MUMmer.MUMmer mummer;

        /// <summary>
        /// Constructs a Mismatcher object to compare against a refreence sequence.
        /// </summary>
        /// <param name="referenceSequence">The sequence against which query sequences will be compared,</param>
		public Mismatcher(ISequence referenceSequence)
		{
			if (referenceSequence == null)
			{
				throw new ArgumentNullException("referenceSequence");
			}

			this.mummer = new MUMmer.MUMmer(referenceSequence);
			this.ReferenceSequence = referenceSequence;
		}

		#region -- Properties  --
		public ISequence ReferenceSequence { get; set; }
		#endregion

        /// <summary>
        /// Finds all the places where the given query sequence differs from the reference sequence.
        /// </summary>
        /// <param name="querySequence">The sequence to compare with the reference</param>
        /// <returns>A collection of Mismatch objects, each one corresponding to a place where the two sequences don't match</returns>
		public IList<Mismatch> GetMismatches(ISequence querySequence)
		{
			if (querySequence == null)
			{
				throw new ArgumentNullException("querySequence");
			}

			IEnumerable<Match> matches;

			matches = mummer.GetMatchesUniqueInReference(querySequence);

			var mismatches = GetMismatches(matches, querySequence);

			return mismatches;

		}

        /// <summary>
        /// Returns true if the given mismatch is an inversion, that is, whether the query subsequence matches the complement of the reference subsequence.
        /// </summary>
        /// <param name="mismatch">The Mismatch to test</param>
        /// <param name="querySequence">The sequence that is being compared to the reference</param>
        /// <returns>True if the complement of the subsequence in the reference is equal to the subsequence in the given query.</returns>
		private bool isInversion(Mismatch mismatch, ISequence querySequence)
		{
			var complement = querySequence.GetSubSequence(mismatch.QuerySequenceOffset, mismatch.QuerySequenceLength)
                                .GetComplementedSequence();

            return ConvertToString(complement, 0, complement.Count).Equals(
                ConvertToString(ReferenceSequence, mismatch.ReferenceSequenceOffset, mismatch.ReferenceSequenceLength)
            );

		}



		/// <summary>
        /// Determines whether the given mismatch is an insertion, deletion or inversion (if supported), and assigns it the appropriate MismatchType
		/// </summary>
		/// <param name="mismatch">The Mismatch to classify</param>
		/// <param name="querySequence">The sequence that is being compared to the reference</param>
        private void classifyMismatches(ref Mismatch mismatch, ISequence querySequence)
		{
            //Not all alphabet types have inversions, so we check before calling isInversion
			bool canInvert = querySequence.Alphabet.IsComplementSupported;

			if (mismatch.QuerySequenceLength == 0) //The subsequence was removed in the query
			{
				mismatch.Type = MismatchType.Deletion;
			}
			else if (mismatch.ReferenceSequenceLength == 0) //The subsequence was never there in the query
			{
				mismatch.Type = MismatchType.Insertion;
			}
			else if (canInvert && isInversion(mismatch, querySequence)) //The subsequences are complements of eachother
			{
				mismatch.Type = MismatchType.Inversion;
			}
		}

        /// <summary>
        /// Determines whether the given mismatch os a translocation, that is, if that particular subsequence appears
        /// in both the reference and query sequences, but in different locations.
        /// </summary>
        /// <param name="mismatch">The new mismatch to test</param>
        /// <param name="querySequence">The sequence being compared ot the reference</param>
        /// <param name="mismatches">The list of previous mismatches between the sequences</param>
        /// <param name="seenRefFragments">A map of reference subsequence content to an index into mismatches</param>
        /// <param name="seenQueFragments">A map of query subsequence content to an index into mismatches</param>
		private void findTranslocation(ref Mismatch mismatch, ISequence querySequence, List<Mismatch> mismatches, Dictionary<string, int> seenRefFragments, Dictionary<string, int> seenQueFragments)
		{ 
			if (mismatch.ReferenceSequenceLength > 0)
			{
                // First, see if we have found the current reference subsequence in a previous part of the query
				string fragment = ConvertToString(ReferenceSequence, mismatch.ReferenceSequenceOffset, mismatch.ReferenceSequenceLength);
				if (seenQueFragments.ContainsKey(fragment))
				{
                    //If we have, set the previously found mismatch's reference half to point to the current reference subsequence
					var previous = mismatches[seenQueFragments[fragment]];
					previous.ReferenceSequenceOffset = mismatch.ReferenceSequenceOffset;
					previous.ReferenceSequenceLength = mismatch.ReferenceSequenceLength;
                    previous.Type = MismatchType.Translocation;
                    //Also set the current mismath's query half to point to the previously found subsequence
					mismatch.QuerySequenceLength = previous.QuerySequenceLength;
					mismatch.QuerySequenceOffset = previous.QuerySequenceOffset;
                    mismatch.Type = MismatchType.Translocation;
                    //Update the previous mismatch
					mismatches[seenQueFragments[fragment]] = previous;
				}
                
                if (seenRefFragments.ContainsKey(fragment))
                {
                    //If we have already seen this fragment in the reference, just forget about the last one
                    seenRefFragments[fragment] = mismatches.Count;
                }
                else
                {
                    seenRefFragments.Add(fragment, mismatches.Count);
                }
			}
			if (mismatch.QuerySequenceLength > 0)
            {
                // Next, see if we have found the current query subsequence in a previous part of the reference
				string fragment = ConvertToString(querySequence, mismatch.QuerySequenceOffset, mismatch.QuerySequenceLength);
				if (seenRefFragments.ContainsKey(fragment) && seenRefFragments[fragment] < mismatches.Count)
                {
                    //If we have, set the previously found mismatch's query half to point to the current query subsequence
					var previous = mismatches[seenRefFragments[fragment]];
					previous.QuerySequenceOffset = mismatch.QuerySequenceOffset;
                    previous.QuerySequenceLength = mismatch.QuerySequenceLength;
                    previous.Type = MismatchType.Translocation;
                    //Also set the current mismath's reference half to point to the previously found subsequence
					mismatch.ReferenceSequenceLength = previous.ReferenceSequenceLength;
                    mismatch.ReferenceSequenceOffset = previous.ReferenceSequenceOffset;
                    mismatch.Type = MismatchType.Translocation;
                    //Update the previous mismatch
					mismatches[seenRefFragments[fragment]] = previous;
				}
                if (seenQueFragments.ContainsKey(fragment))
                {
                    seenQueFragments[fragment] = mismatches.Count;
                }
                else
                {
                    seenQueFragments.Add(fragment, mismatches.Count);
                }
			}
		}

        /// <summary>
        /// Detects if the mismatch is a translocation within itself, that is, if the first half of the query matches
        /// the second half of the reference. This is equivalent to finding if the query subsequence is a rotation
        /// of the reference subsequence.
        /// </summary>
        /// <param name="mismatch">The mismatch to test</param>
        /// <param name="querySequence">THe sequence being compared against</param>
        private void findSelfTranslocation(ref Mismatch mismatch, ISequence querySequence)
        {
            if (mismatch.QuerySequenceLength != mismatch.ReferenceSequenceLength)
            {
                return; //They are different lengths, it cannot be a rotation
            }
            var querySub = ConvertToString(querySequence, mismatch.QuerySequenceOffset, mismatch.QuerySequenceLength);
            var refSub = ConvertToString(ReferenceSequence, mismatch.ReferenceSequenceOffset, mismatch.ReferenceSequenceLength);

            //Find if it is a rotation, and the rotation point
            int i = (refSub + refSub).IndexOf(querySub);
            if ( i >= 0 && i < refSub.Length )
            {
                //Update the mismatch so that it only points to one matching half
                mismatch.Type = MismatchType.Translocation;
                mismatch.ReferenceSequenceLength = i;
                mismatch.QuerySequenceOffset += i;
                mismatch.QuerySequenceLength -= i;
            }

        }

		/// <summary>
		/// Converts a portion of an ISequence to a string. Intended to match the behaviour of Sequence.ConvertToString.
		/// </summary>
		/// <returns>A string </returns>
		/// <param name="sequence">Sequence.</param>
		/// <param name="startIndex">Start index.</param>
		/// <param name="length">Length.</param>
		private static string ConvertToString(ISequence sequence, long startIndex, long length)
		{
			StringBuilder sb = new StringBuilder((int)length);
			for (long i = startIndex; i < startIndex + length; i++)
			{
				sb.Append(sequence[i]);
			}
			return sb.ToString();
		}

        /// <summary>
        /// Goes throught a list of matches between the query and reference sequences, and extracts and classifies the mismatches
        /// </summary>
        /// <param name="matches">The places where the query and the reference match</param>
        /// <param name="querySequence">The sequence being compared</param>
        /// <returns>A list of places where the two sequences don't match</returns>
		private List<Mismatch> GetMismatches(IEnumerable<Match> matches, ISequence querySequence)
		{
			List<Mismatch> mismatches = new List<Mismatch>();

            //Store all the found mismatching subsequences, for finding translocations
			Dictionary<string, int> seenRefFragments = new Dictionary<string, int>();
			Dictionary<string, int> seenQueFragments = new Dictionary<string, int>();
           
			var matchEnum = matches.GetEnumerator();
            
			Match match = matchEnum.Current;

			long gapStartRef = 0;
			long gapStartQue = 0;
			bool finished = false;

            Mismatch mismatch = new Mismatch();
 
			while (!finished)
			{
                //'Open' the mismatch by setting the starting point, based on the end of the last match
				mismatch.QuerySequenceOffset = gapStartQue;
				mismatch.ReferenceSequenceOffset = gapStartRef;

                //Look at the next match
				if (matchEnum.MoveNext())
				{
                    //'Close' the match by setting the lengths
					match = matchEnum.Current;
					mismatch.QuerySequenceLength = match.QuerySequenceOffset - gapStartQue;
					mismatch.ReferenceSequenceLength = match.ReferenceSequenceOffset - gapStartRef;

                    //Update the end position of the last match
					gapStartQue = match.QuerySequenceOffset + match.Length;
					gapStartRef = match.ReferenceSequenceOffset + match.Length;

				}
				else
				{ //End of the sequence, no more matches, close the last match
					mismatch.QuerySequenceLength = querySequence.Count - gapStartQue;
					mismatch.ReferenceSequenceLength = ReferenceSequence.Count - gapStartRef;
					finished = true;
				}


				if (mismatch.ReferenceSequenceLength > 0 || mismatch.QuerySequenceLength > 0)
                { //Ignore zero-length mismatches

                    //Attempt to classify the mismatch as inversion, insertion, deletion
                    classifyMismatches(ref mismatch, querySequence);
                    //See if it is a translocation from earlier
                    findTranslocation(ref mismatch, querySequence, mismatches, seenRefFragments, seenQueFragments);
                    //See if is a translocation within itself (one half swapped with the other half)
                    findSelfTranslocation(ref mismatch, querySequence);
					mismatches.Add(mismatch);
				}
			}

			return mismatches;
		}

	}
}
