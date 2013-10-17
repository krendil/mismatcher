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

        public MismatchEventArgs(Mismatch mismatch)
            : base()
        {
            this.Mismatch = mismatch;
        }
    }
}
