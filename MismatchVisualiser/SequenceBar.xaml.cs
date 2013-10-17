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

using Bio.Algorithms.Mismatcher;
using Bio;

namespace MismatchVisualiser
{
    /// <summary>
    /// Interaction logic for SequenceBar.xaml
    /// </summary>
    public partial class SequenceBar : FrameworkElement
    {
        public SequenceBar()
        {
            Mismatches = new List<Mismatch>();
            InitializeComponent();
        }

        #region Properties

        public ISequence Sequence
        {
            get { return (ISequence)GetValue(SequenceProperty); }
            set { SetValue(SequenceProperty, value); }
        }
        public IEnumerable<Mismatch> Mismatches
        {
            get { return (IEnumerable<Mismatch>)GetValue(MismatchesProperty); }
            set { SetValue(MismatchesProperty, value); }
        }
        public bool IsReference
        {
            get { return (bool)GetValue(IsReferenceProperty); }
            set { SetValue(IsReferenceProperty, value); }
        }

        private Brush BackgroundBrush = Brushes.White;
        public Brush DeletionBrush = Brushes.IndianRed;
        public Brush InsertionBrush = Brushes.AliceBlue;
        public Brush InversionBrush = Brushes.LightGreen;
        public Brush TranslocationBrush = Brushes.Purple;
        public Brush UnknownBrush = Brushes.Yellow;

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty SequenceProperty = DependencyProperty.Register(
            "Sequence",
            typeof(ISequence),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
         );

        public static readonly DependencyProperty MismatchesProperty = DependencyProperty.Register(
            "Mismatches",
            typeof(IEnumerable<Mismatch>),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(new List<Mismatch>(),
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnMismatchesChanged)
                )
            );

        public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register(
            "IsReference",
            typeof(bool),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnIsReferenceChanged)
                )
            );

        /*
        public readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register(
            "BackgroundBrush",
            typeof(Brush),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
            );
        public readonly DependencyProperty DeletionBrushProperty = DependencyProperty.Register(
            "DeletionBrush",
            typeof(Brush),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
            );
        public readonly DependencyProperty InsertionBrushProperty = DependencyProperty.Register(
            "InsertionBrush",
            typeof(Brush),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
            ); 
        public readonly DependencyProperty TranslocationBrushProperty = DependencyProperty.Register(
            "TranslocationBrush",
            typeof(Brush),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
            );
        public readonly DependencyProperty UnknownBrushProperty = DependencyProperty.Register(
            "UnknownBrush",
            typeof(Brush),
            typeof(SequenceBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSequenceChanged)
                )
            );
        */
        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //Rect bgRect = this.Clip.Bounds;
            Rect bgRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            drawingContext.DrawRectangle(BackgroundBrush, new Pen(Brushes.Black, 1), bgRect );

            if (Sequence == null) return;
            //Converts distance along the sequence to distance along the bar
            double seqToBar = bgRect.Width / Sequence.Count;

            foreach (Mismatch mismatch in Mismatches)
            {
                Rect mmRect;
                if(IsReference) {
                    mmRect = new Rect(mismatch.ReferenceSequenceOffset*seqToBar, 0, mismatch.ReferenceSequenceLength*seqToBar, bgRect.Height);
                } else {
                    mmRect = new Rect(mismatch.QuerySequenceOffset*seqToBar, 0, mismatch.QuerySequenceLength*seqToBar, bgRect.Height);
                }
                Brush brush;

                switch(mismatch.Type) {
                    case MismatchType.Deletion:
                        brush = DeletionBrush; break;
                    case MismatchType.Insertion:
                        brush = InsertionBrush; break;
                    case MismatchType.Inversion:
                        brush = InversionBrush; break;
                    case MismatchType.Translocation:
                        brush = TranslocationBrush; break;
                    case MismatchType.Unknown:
                        brush = UnknownBrush; break;
                    default:
                        brush = UnknownBrush; break;
                }

                drawingContext.DrawRectangle(brush, new Pen(brush, 1), mmRect);
            }
        }

        private static void OnSequenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnIsReferenceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnMismatchesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
