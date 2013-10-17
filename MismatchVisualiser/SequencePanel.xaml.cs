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

        private Mismatch currentMismatch;

        public SequencePanel()
        {
            InitializeComponent();
        }


        #region Properties

        public ISequence Sequence
        {
            get { return (ISequence)GetValue(SequenceProperty); }
            set {
                SetValue(SequenceProperty, value);
                sequenceBar1.Sequence = value;
            }
        }
        public IEnumerable<Mismatch> Mismatches
        {
            get { return (IEnumerable<Mismatch>)GetValue(MismatchesProperty); }
            set {
                SetValue(MismatchesProperty, value);
                sequenceBar1.Mismatches = value;
                currentMismatch = value.FirstOrDefault();
            }
        }
        public bool IsReference
        {
            get { return (bool)GetValue(IsReferenceProperty); }
            set {
                SetValue(IsReferenceProperty, value);
                sequenceBar1.IsReference = value;
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
                }
                else
                {
                    lengthLabel.Content = currentMismatch.QuerySequenceLength.ToString();
                    offsetLabel.Content = currentMismatch.QuerySequenceOffset.ToString();
                }

                typeLabel.Content = Enum.GetName(typeof(MismatchType), currentMismatch.Type);
            }
        }

        public static readonly DependencyProperty SequenceProperty = DependencyProperty.Register(
            "Sequence",
            typeof(ISequence),
            typeof(SequencePanel),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
         );


        public static readonly DependencyProperty MismatchesProperty = DependencyProperty.Register(
            "Mismatches",
            typeof(IEnumerable<Mismatch>),
            typeof(SequencePanel),
            new FrameworkPropertyMetadata(new List<Mismatch>(),
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnMismatchesChanged)
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

        private static void OnSequenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void OnIsReferenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnMismatchesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion
    }
}
