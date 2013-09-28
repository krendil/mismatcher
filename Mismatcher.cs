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
		public IEnumerable<Mismatch> GetMismatches(ISequence querySequence, bool uniqueInReference = false)
		{
			if (querySequence == null)
			{
				throw new ArgumentNullException("querySequence");
			}

			IEnumerable<Match> matches;

			if (uniqueInReference)
			{
				matches = mummer.GetMatchesUniqueInReference(querySequence);
			}
			else
			{
				matches = mummer.GetMatches(querySequence);
			}

			var mismatches = GetMismatches(matches, querySequence);


			return mismatches;

		}

		private bool isInversion(Mismatch mismatch, ISequence querySequence)
		{
			var querySub = querySequence.GetSubSequence(mismatch.QuerySequenceOffset,
			                                             mismatch.QuerySequenceLength);
			var refSub = ReferenceSequence.GetSubSequence(mismatch.ReferenceSequenceLength,
			                                               mismatch.ReferenceSequenceOffset);

			return querySub.GetComplementedSequence().Equals(refSub);

		}

		/// <summary>
		/// Iterates through the sequence of mismatches and assigns
		/// each of them a MismatchType.
		/// </summary>
		/// <param name="mismatches"></param>
		private void classifyMismatches(ref Mismatch mismatch, ISequence querySequence)
		{
			bool canInvert = querySequence.Alphabet.IsComplementSupported;
			if (mismatch.QuerySequenceLength == 0)
			{
				mismatch.Type = MismatchType.Deletion;
			}
			else if (mismatch.ReferenceSequenceLength == 0)
			{
				mismatch.Type = MismatchType.Insertion;
			}
			else if (canInvert && isInversion(mismatch, querySequence))
			{
				mismatch.Type = MismatchType.Inversion;
			}
		}

		private void findTranslocation(ref Mismatch mismatch, ISequence querySequence, List<Mismatch> mismatches, Dictionary<string, int> seenRefFragments, Dictionary<string, int> seenQueFragments)
		{
			if (mismatch.ReferenceSequenceLength > 0)
			{
				string fragment = ConvertToString(ReferenceSequence, mismatch.ReferenceSequenceOffset, mismatch.ReferenceSequenceLength);
				if (seenQueFragments.ContainsKey(fragment))
				{
					var previous = mismatches[seenQueFragments[fragment]];
					previous.ReferenceSequenceOffset = mismatch.ReferenceSequenceOffset;
					previous.ReferenceSequenceLength = mismatch.ReferenceSequenceLength;
					mismatch.QuerySequenceLength = previous.QuerySequenceLength;
					mismatch.QuerySequenceOffset = previous.QuerySequenceOffset;
					mismatches[seenQueFragments[fragment]] = previous;
				}
				seenRefFragments.Add(ConvertToString(ReferenceSequence, mismatch.ReferenceSequenceOffset, mismatch.ReferenceSequenceOffset), mismatches.Count);
			}
			if (mismatch.QuerySequenceLength > 0)
			{
				string fragment = ConvertToString(querySequence, mismatch.QuerySequenceOffset, mismatch.QuerySequenceLength);
				if (seenRefFragments.ContainsKey(fragment))
				{
					var previous = mismatches[seenRefFragments[fragment]];
					previous.QuerySequenceOffset = mismatch.QuerySequenceOffset;
					previous.QuerySequenceLength = mismatch.QuerySequenceLength;
					mismatch.ReferenceSequenceLength = previous.ReferenceSequenceLength;
					mismatch.ReferenceSequenceOffset = previous.ReferenceSequenceOffset;
					mismatches[seenRefFragments[fragment]] = previous;
				}
				seenQueFragments.Add(ConvertToString(querySequence, mismatch.QuerySequenceOffset, mismatch.QuerySequenceOffset), mismatches.Count);
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

		private List<Mismatch> GetMismatches(IEnumerable<Match> matches, ISequence querySequence)
		{
			List<Mismatch> mismatches = new List<Mismatch>();

			Mismatch mismatch = new Mismatch();
			Dictionary<string, int> seenRefFragments = new Dictionary<string, int>();
			Dictionary<string, int> seenQueFragments = new Dictionary<string, int>();
			var matchEnum = matches.GetEnumerator();
            
			Match match = matchEnum.Current;

			long gapStartRef = 0;
			long gapStartQue = 0;
			bool finished = false;
 
			while (!finished)
			{
				mismatch.QuerySequenceOffset = gapStartQue;
				mismatch.ReferenceSequenceOffset = gapStartRef;

				if (matchEnum.MoveNext())
				{
					match = matchEnum.Current;
					mismatch.QuerySequenceLength = match.QuerySequenceOffset - gapStartQue;
					mismatch.ReferenceSequenceLength = match.ReferenceSequenceOffset - gapStartRef;

					gapStartQue = match.ReferenceSequenceOffset + match.Length;
					gapStartRef = match.QuerySequenceOffset + match.Length;

				}
				else
				{ //End of the sequence
					mismatch.QuerySequenceLength = querySequence.Count - gapStartQue;
					mismatch.ReferenceSequenceLength = ReferenceSequence.Count - gapStartRef;
					finished = true;
				}

				classifyMismatches(ref mismatch, querySequence);
				findTranslocation(ref mismatch, querySequence, mismatches, seenRefFragments, seenQueFragments);

				if (mismatch.ReferenceSequenceLength > 0 || mismatch.QuerySequenceLength > 0)
				{ //Ignore zero-length mismatches
					mismatches.Add(mismatch);
				}
			}

			return mismatches;
		}
	}
}
