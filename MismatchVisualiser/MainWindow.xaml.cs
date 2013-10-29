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
using System.IO;

using Bio;
using Bio.IO;
using Bio.Algorithms.Mismatcher;

namespace MismatchVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Mismatcher mismatcher;
        private ISequence query;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void loadFiles(string[] files)
        {

            foreach (var file in files)
            {

                var parser = SequenceParsers.FindParserByFileName(file);
                //Default to the simplest format
                if (parser == null) parser = new Bio.IO.FastA.FastAParser(file);

                foreach (ISequence seq in parser.Parse())
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = seq.ID;
                    item.Tag = seq;
                    filesBox.Items.Add(item);
                }
            }
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            loadFiles(dialog.FileNames);
        }

        private ISequence getCurrentSequence()
        {
            var item = filesBox.SelectedItem as ListViewItem;
            if (item == null) return null;
            var sequence = item.Tag as ISequence;
            if (sequence == null) throw new Exception("Sequence not stored");
            return sequence;
        }

        private void referenceButton_Click(object sender, RoutedEventArgs e)
        {
            ISequence seq = getCurrentSequence();
            if (seq == null) return;

            mismatcher = new Mismatcher(seq);
            referencePanel.Sequence = seq;
            referenceBar.Sequence = seq;

            if (query != null)
            {
                var mismatches = mismatcher.GetMismatches(query);
                referenceBar.Mismatches = mismatches;
                queryBar.Mismatches = mismatches;
            }
        }

        private void queryButton_Click(object sender, RoutedEventArgs e)
        {
            ISequence seq = getCurrentSequence();
            if (seq == null) return;

            query = seq;
            queryPanel.Sequence = seq;
            queryBar.Sequence = seq;

            if (mismatcher != null)
            {
                var mismatches = mismatcher.GetMismatches(query);
                referenceBar.Mismatches = mismatches;
                queryBar.Mismatches = mismatches;
            }
        }

        private void onMismatchSelected(object sender, MismatchEventArgs e)
        {
            queryPanel.CurrentMismatch = e.Mismatch;
            referencePanel.CurrentMismatch = e.Mismatch;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            filesBox.Items.Clear();
        }

        private void stackPanel1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                zoomBars(e.Delta > 0);
            }
            else
            {
                scrollBars(e.Delta > 0);
            }
        }

        private void zoomBars(bool zoomIn)
        {
            const double zoomFactor = 1.1;
            double total = zoomIn ? zoomFactor : (1 / zoomFactor);
            referenceBar.ZoomFactor = referenceBar.ZoomFactor * total;
            queryBar.ZoomFactor = queryBar.ZoomFactor * total;
        }

        private void scrollBars(bool scrollLeft)
        {
            if (scrollLeft)
            {
                scrollViewer1.LineLeft();
            }
            else
            {
                scrollViewer1.LineRight();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {
                switch (e.Key)
                {
                    case Key.Add:
                    case Key.OemPlus:
                        zoomBars(true); break;

                    case Key.OemMinus:
                    case Key.Subtract:
                        zoomBars(false); break;

                    default: break;
                }
            }
        }


    }
}
