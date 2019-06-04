using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace SerienBenennen
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string lastClipboardText;
        private DispatcherTimer checkClipboardTimer;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = ViewModel.Current;

            checkClipboardTimer = new DispatcherTimer(DispatcherPriority.Normal);
            checkClipboardTimer.Tick += CheckClipboardTimer_Tick;
            checkClipboardTimer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string dropText = e.Data.GetData(DataFormats.Text).ToString();

                SetEpisodeName(dropText);
            }
            catch { }
        }

        private void CheckClipboardTimer_Tick(object sender, System.EventArgs e)
        {
            string clipboardText = Clipboard.GetText();

            if (lastClipboardText == clipboardText) return;

            SetEpisodeName(clipboardText);
            lastClipboardText = clipboardText;
        }

        private void ChangeEpisodenNames_Click(object sender, RoutedEventArgs e)
        {
            if (lbxEpisodesNames.SelectedIndex == -1) lbxEpisodesNames.SelectedIndex = 0;

            new EpisodenNameWindow(lbxEpisodesNames.SelectedItem as EpisodeName).ShowDialog();
        }

        private void SetEpisodeName(string name)
        {
            if (lbxEpisodesNames.SelectedIndex == -1) lbxEpisodesNames.SelectedIndex = 0;

            ViewModel.Current.EpisodesNames[lbxEpisodesNames.SelectedIndex].Name = name;

            lbxEpisodesNames.SelectedIndex = (lbxEpisodesNames.SelectedIndex + 1) % lbxEpisodesNames.Items.Count;
        }

        private void DeleteEpisodenNames_Click(object sender, RoutedEventArgs e)
        {
            List<EpisodeName> eNames = new List<EpisodeName>();

            foreach (EpisodeName eName in lbxEpisodesNames.SelectedItems)
            {
                eNames.Add(eName);
            }

            foreach (EpisodeName eName in eNames)
            {
                ViewModel.Current.EpisodesNames.Remove(eName);
            }
        }

        private void CheckClipboard_Checked(object sender, RoutedEventArgs e)
        {
            checkClipboardTimer.IsEnabled = true;
        }

        private void UncheckClipboard_Checked(object sender, RoutedEventArgs e)
        {
            checkClipboardTimer.IsEnabled = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (EpisodeName eName in ViewModel.Current.EpisodesNames)
            {
                eName.Save();
            }

            ViewModel.Current.Folder.Refresh();
            //ViewModel.Current.UpdateEpisodenNamesFromStorage();
        }
    }
}
