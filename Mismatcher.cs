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

            ClassifyMismatches(mismatches, querySequence);

            return mismatches;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mismatches"></param>
        private void ClassifyMismatches(List<Mismatch> mismatches, ISequence querySequence)
        {
            Mismatch tmp;
            for (int i = 0; i < mismatches.Count; i++ )
            {
                if (mismatches[i].QuerySequenceLength == 0)
                {
                    tmp = mismatches[i];
                    tmp.Type = MismatchType.Deletion;
                    mismatches[i] = tmp;
                    continue;
                }
                else if (mismatches[i].ReferenceSequenceLength == 0)
                {
                    tmp = mismatches[i];
                    tmp.Type = MismatchType.Insertion;
                    mismatches[i] = tmp;
                    continue;
                }
                //TODO: Test inversions by inverting query and attempting partial match of ref mismatch vs inversion
                //TODO: Test translocations by attempting partial match against (nearby?) mismatches7
            }
        }
         
        private List<Mismatch> GetMismatches(IEnumerable<Match> matches, ISequence querySequence)
        {
            List<Mismatch> mismatches = new List<Mismatch>();

            Mismatch currentMismatch = new Mismatch();
            var matchEnum = matches.GetEnumerator();
            
            Match match = matchEnum.Current;

            long gapStartRef = 0;
            long gapStartQue = 0;
            bool finished = false;

            while (!finished)
            {
                currentMismatch.QuerySequenceOffset = gapStartQue;
                currentMismatch.ReferenceSequenceOffset = gapStartRef;

                if (matchEnum.MoveNext())
                {
                    match = matchEnum.Current;
                    currentMismatch.QuerySequenceLength = match.QuerySequenceOffset - gapStartQue;
                    currentMismatch.ReferenceSequenceLength = match.ReferenceSequenceOffset - gapStartRef;

                    gapStartQue = match.ReferenceSequenceOffset + match.Length;
                    gapStartRef = match.QuerySequenceOffset + match.Length;

                }
                else
                { //End of the sequence
                    currentMismatch.QuerySequenceLength = querySequence.Count - gapStartQue;
                    currentMismatch.ReferenceSequenceLength = ReferenceSequence.Count - gapStartRef;
                    finished = true;
                }

                if (currentMismatch.ReferenceSequenceLength > 0 || currentMismatch.QuerySequenceLength > 0)
                { //Ignore zero-length mismatches
                    mismatches.Add(currentMismatch);
                }
            }

            return mismatches;
        }
    }
}
