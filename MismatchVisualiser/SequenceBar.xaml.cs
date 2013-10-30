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


        //Converts distance along the sequence to distance along the bar
        double zoomFactor = 1.0;


        public SequenceBar()
        {
            Mismatches = new List<Mismatch>();
            InitializeComponent();
        }

        #region Properties

        public ISequence Sequence
        {
            get { return (ISequence)GetValue(SequenceProperty); }
            set {
                SetValue(SequenceProperty, value);
                this.Width = value.Count * ZoomFactor;
            }
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

        public double ZoomFactor
        {
            get { return zoomFactor; }
            set 
            { 
                zoomFactor = value;
                this.Width = Sequence.Count * ZoomFactor;
                this.InvalidateVisual();
            }
        }

        private Brush BackgroundBrush = Brushes.White;
        public Brush DeletionBrush = Brushes.IndianRed;
        public Brush InsertionBrush = Brushes.DodgerBlue;
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

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //Rect bgRect = this.Clip.Bounds;
            Rect bgRect = new Rect(0, 0, this.Width, this.ActualHeight);
            drawingContext.DrawRectangle(BackgroundBrush, new Pen(Brushes.Black, 1), bgRect );

            if (Sequence == null) return;

            foreach (Mismatch mismatch in Mismatches)
            {
                Rect mmRect;
                if(IsReference) {
                    mmRect = new Rect(mismatch.ReferenceSequenceOffset * ZoomFactor, 0, mismatch.ReferenceSequenceLength * ZoomFactor, bgRect.Height);
                } else {
                    mmRect = new Rect(mismatch.QuerySequenceOffset * ZoomFactor, 0, mismatch.QuerySequenceLength * ZoomFactor, bgRect.Height);
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

        #region Event Handlers

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

        private void FrameworkElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            double n = pos.X / zoomFactor;

            int i = -1; //Iterate with index
            foreach (var mismatch in Mismatches)
            {
                long start;
                long end;
                i++;

                if (IsReference)
                {
                    start = mismatch.ReferenceSequenceOffset;
                    end = start + mismatch.ReferenceSequenceLength;
                }
                else
                {
                    start = mismatch.QuerySequenceOffset;
                    end = start + mismatch.QuerySequenceLength;
                }

                if ( n >= end ) {
                    continue;
                }
                else if (n >= start)
                {
                    if (MismatchSelected != null)
                    {
                        MismatchSelected(this, new MismatchEventArgs(mismatch, i));
                    }
                    return;
                }
            }
        }

        public event EventHandler<MismatchEventArgs> MismatchSelected;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Sequence != null)
            {
                zoomFactor = this.ActualWidth / Sequence.Count;
            }
        }
    }
}
