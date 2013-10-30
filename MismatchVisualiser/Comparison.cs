using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MismatchVisualiser
{
    class Comparison
    {

        private Dictionary<int, string> notes;

        public Comparison(string reference, string query)
        {
            ReferenceId = reference;
            QueryId = query;
            notes = new Dictionary<int, string>();
        }

        public string ReferenceId
        {
            get;
            private set;
        }

        public string QueryId
        {
            get;
            private set;
        }

        public void MakeNote(int mismatch, string note)
        {
            if (notes.ContainsKey(mismatch))
            {
                if (String.IsNullOrEmpty(note))
                {
                    notes.Remove(mismatch);
                }
                else
                {
                    notes[mismatch] = note;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(note))
                {
                    notes.Add(mismatch, note);
                }
            }
        }

        public string GetNote(int mismatch)
        {
            if (notes.ContainsKey(mismatch))
            {
                return notes[mismatch];
            }
            else
            {
                return "";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Comparison)
            {
                var other = obj as Comparison;
                return other.QueryId.Equals(QueryId) && other.ReferenceId.Equals(ReferenceId);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (ReferenceId + QueryId).GetHashCode();
        }

    }
}
