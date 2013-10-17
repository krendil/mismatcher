using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bio;
using Bio.Algorithms.Mismatcher;

namespace MismatchVisualiser
{
    /// <summary>
    /// Interaction logic for SequencePanel.xaml
    /// </summary>
    public partial class SequencePanel : UserControl
    {
        private const int leadingChars = 3;
        private Brush greyText = Brushes.DarkGray;
        private Mismatch currentMismatch;
        private TextEffect leading;
        private TextEffect trailing;

        public SequencePanel()
        {
            InitializeComponent();
            leading = new TextEffect(Transform.Identity, greyText, null, 0, 0);
            trailing = new TextEffect(Transform.Identity, greyText, null, 0, 0);
        }

        #region Properties

        public bool IsReference
        {
            get { return (bool)GetValue(IsReferenceProperty); }
            set {
                SetValue(IsReferenceProperty, value);
            }
        }

        public ISequence Sequence
        {
            get { return (ISequence)GetValue(SequenceProperty); }
            set
            {
                SetValue(SequenceProperty, value);
                this.sequenceID.Content = sanitise(value.ID);
            }
        }

        public Mismatch CurrentMismatch
        {
            get { return currentMismatch;  }
            set {
                currentMismatch = value;

                if (IsReference)
                {
                    lengthLabel.Content = currentMismatch.ReferenceSequenceLength.ToString();
                    offsetLabel.Content = currentMismatch.ReferenceSequenceOffset.ToString();
                    SetBlockText(value.ReferenceSequenceOffset, value.ReferenceSequenceLength);
                }
                else
                {
                    lengthLabel.Content = currentMismatch.QuerySequenceLength.ToString();
                    offsetLabel.Content = currentMismatch.QuerySequenceOffset.ToString();
                    SetBlockText(value.QuerySequenceOffset, value.QuerySequenceLength);
                }

                typeLabel.Content = Enum.GetName(typeof(MismatchType), currentMismatch.Type);
            }
        }

        private void SetBlockText(long offset, long length)
        {
            long textBegin = offset;
            long textLength = length;
            sampleBlock.TextEffects.Clear();
            if (offset > 0)
            {
                if (offset <= leadingChars)
                {
                    textBegin -= (leadingChars - offset);
                    textLength += (leadingChars - offset);
                    leading.PositionCount = (int)(leadingChars - offset);
                }
                else
                {
                    textBegin -= leadingChars;
                    textLength += leadingChars;
                    leading.PositionCount = leadingChars;
                }
                sampleBlock.TextEffects.Add(leading);
            }
            if (offset + length < Sequence.Count)
            {
                trailing.PositionStart = (int)textLength;

                long n = (Sequence.Count - (offset + length));
                if (n <= leadingChars)
                {
                    trailing.PositionCount = (int)n;
                    textLength += n;
                }
                else
                {
                    trailing.PositionCount = leadingChars;
                    textLength += leadingChars;
                }
                sampleBlock.TextEffects.Add(trailing);
            }

            sampleBlock.Text = ConvertToString(Sequence, textBegin, textLength);
        }

        #endregion

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
                sb.Append( (char)sequence[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Replaces single underscores with doubles to prevent them from being interpreted as accelerator keys
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string sanitise(string str)
        {
            return str.Replace("_", "__");
        }

        #region Dependency Properties

        public static readonly DependencyProperty SequenceProperty = DependencyProperty.Register(
            "Sequence",
            typeof(ISequence),
            typeof(SequencePanel),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
         );

        public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register(
            "IsReference",
            typeof(bool),
            typeof(SequencePanel),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnIsReferenceChanged)
                )
            );

        private static void OnIsReferenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnSequenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion
    }
}
