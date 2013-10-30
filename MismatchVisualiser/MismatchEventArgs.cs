using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Algorithms.Mismatcher;

namespace MismatchVisualiser
{
    public class MismatchEventArgs : EventArgs
    {
        public Mismatch Mismatch { get; set; }
        public int Index { get; set; }

        public MismatchEventArgs(Mismatch mismatch, int index)
            : base()
        {
            this.Mismatch = mismatch;
            this.Index = index;
        }
    }
}
