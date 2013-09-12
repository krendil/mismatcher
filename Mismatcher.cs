using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Algorithms.MUMmer;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.Mismatcher
{
    class Mismatcher
    {

        MUMmer.MUMmer mummer;

        public Mismatcher(ISequence referenceSequence)
        {
            if (referenceSequence == null)
            {
                throw new ArgumentNullException("referenceSequence");
            }

            this.mummer = new MUMmer.MUMmer(referenceSequence);
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

            var mismatches = GetMismatches(matches);

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

        private List<Mismatch> GetMismatches(IEnumerable<Match> matches)
        {
            List<Mismatch> mismatches = new List<Mismatch>();

            //The indexes just past the last known match in the reference and query sequences, respectively
            long refLastMatchEnd = 0;
            long queLastMatchEnd = 0;

            Mismatch currentMismatch = new Mismatch();
            currentMismatch.QuerySequenceOffset = 0;
            currentMismatch.ReferenceSequenceOffset = 0;

            foreach (Match match in matches)
            {
                currentMismatch.ReferenceSequenceLength = match.ReferenceSequenceOffset - refLastMatchEnd;
                currentMismatch.QuerySequenceLength = match.QuerySequenceOffset - queLastMatchEnd;

                mismatches.Add(currentMismatch);
                //Mismatch's are value types, so they get passed by copy, and we can just keep working on the local copy without messing up the one in the list

                currentMismatch.ReferenceSequenceOffset = refLastMatchEnd;
                currentMismatch.QuerySequenceOffset = queLastMatchEnd;

                refLastMatchEnd = match.ReferenceSequenceOffset + match.Length;
                queLastMatchEnd = match.QuerySequenceOffset + match.Length;
            }

            return mismatches;
        }
    }
}
