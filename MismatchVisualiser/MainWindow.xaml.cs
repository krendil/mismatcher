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
        private Dictionary<string, ISequence> loadedSequences;
        private List<string> loadedFiles;
        private Dictionary<Comparison, Comparison> noteHolders;
        
        private Comparison currentComparison;
        private string queryId;
        private string refId;
        private int currentMismatch;

        public MainWindow()
        {
            InitializeComponent();
            loadedSequences = new Dictionary<string, ISequence>();
            loadedFiles = new List<string>();
            noteHolders = new Dictionary<Comparison, Comparison>();
        }

        #region Loading sequences

        private void loadFiles(string[] files)
        {

            foreach (string file in files)
            {

                var parser = SequenceParsers.FindParserByFileName(file);
                //Default to the simplest format
                if (parser == null) parser = new Bio.IO.FastA.FastAParser(file);

                foreach (ISequence seq in parser.Parse())
                {
                    string baseTag = file + ":" + seq.ID;
                    string tag = baseTag;
                    for (int i = 1; loadedSequences.ContainsKey(tag); i++ )
                    {
                        tag = baseTag + i.ToString();
                    }
                    ListViewItem item = new ListViewItem();
                    item.Content = seq.ID;
                    item.Tag = tag;
                    filesBox.Items.Add(item);
                    loadedSequences.Add(tag, seq);
                }

                loadedFiles.AddRange(files);
            }
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            loadFiles(dialog.FileNames);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            filesBox.Items.Clear();
            loadedSequences.Clear();
            loadedFiles.Clear();
        }

        #endregion


        #region Displaying sequences

        private string getSelectedSequenceId()
        {
            var item = filesBox.SelectedItem as ListViewItem;
            if (item == null) return null;
            var key = item.Tag.ToString();
            return key;
        }

        private ISequence getSequence(string key)
        {
            try
            {
                return loadedSequences[key];
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception("Sequence not loaded", e);
            }
        }

        private void referenceButton_Click(object sender, RoutedEventArgs e)
        {
            string id = getSelectedSequenceId();
            this.refId = id;
            var seq = getSequence(id);
            if (seq == null) return;

            mismatcher = new Mismatcher(seq);
            referencePanel.Sequence = seq;
            referenceBar.Sequence = seq;

            if (query != null)
            {
                var mismatches = mismatcher.GetMismatches(query);
                referenceBar.Mismatches = mismatches;
                queryBar.Mismatches = mismatches;
                loadComparison();
            }
        }

        private void queryButton_Click(object sender, RoutedEventArgs e)
        {
            string id = getSelectedSequenceId();
            this.queryId = id;
            var seq = getSequence(id);
            if (seq == null) return;

            query = seq;
            queryPanel.Sequence = seq;
            queryBar.Sequence = seq;

            if (mismatcher != null)
            {
                var mismatches = mismatcher.GetMismatches(query);
                referenceBar.Mismatches = mismatches;
                queryBar.Mismatches = mismatches;
                loadComparison();
            }
        }

        private void loadComparison()
        {
            Comparison blank = new Comparison(this.refId, this.queryId);
            if (this.noteHolders.ContainsKey(blank))
            {
                this.currentComparison = noteHolders[blank];
            }
            else
            {
                this.currentComparison = blank;
            }
            this.notesBox.Text = "";
            this.notesBox.IsEnabled = false;
        }

        #endregion


        #region Selecting Mismatches

        private void onMismatchSelected(object sender, MismatchEventArgs e)
        {
            queryPanel.CurrentMismatch = e.Mismatch;
            referencePanel.CurrentMismatch = e.Mismatch;
            this.currentMismatch = e.Index;
            notesBox.Text = currentComparison.GetNote(e.Index);
            notesBox.IsEnabled = true;
        }

        #endregion


        #region Zoom and Scroll

        private void stackPanel1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                zoomBars(e.Delta > 0, true);
            }
            else
            {
                scrollBars(e.Delta > 0);
            }
        }

        private void zoomBars(bool zoomIn, bool followMouse = false)
        {
            const double zoomFactor = 1.1;

            double offset;

            if (followMouse)
            {
                offset = Mouse.GetPosition(scrollPanel).X;
            }
            else
            {
                offset = (scrollPanel.ActualWidth * 0.5);
            }
            double centre = scrollPanel.HorizontalOffset + offset;

            double total = zoomIn ? zoomFactor : (1 / zoomFactor);
            referenceBar.ZoomFactor = referenceBar.ZoomFactor * total;
            queryBar.ZoomFactor = queryBar.ZoomFactor * total;

            scrollPanel.ScrollToHorizontalOffset( (centre*total) - offset);
        }

        private void scrollBars(bool scrollLeft)
        {
            if (scrollLeft)
            {
                scrollPanel.LineLeft();
            }
            else
            {
                scrollPanel.LineRight();
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

        #endregion

        #region Notes

        private void notesBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (notesBox.Text == currentComparison.GetNote(currentMismatch))
            {
                return;
            }
            currentComparison.MakeNote(currentMismatch, notesBox.Text);

            if (!noteHolders.ContainsKey(currentComparison))
            {
                noteHolders.Add(currentComparison, currentComparison);
            }
        }

        #endregion
    }
}
