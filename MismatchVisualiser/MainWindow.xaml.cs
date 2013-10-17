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

            foreach(var file in files) {

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
    }
}
