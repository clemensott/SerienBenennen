using System.Windows;
using System.Windows.Controls;

namespace SerienBenennen
{
    /// <summary>
    /// Interaktionslogik für EpisodenNameWindow.xaml
    /// </summary>
    public partial class EpisodenNameWindow : Window
    {
        EpisodeName episodeName;

        public EpisodenNameWindow(EpisodeName episodeName)
        {
            InitializeComponent();

            this.episodeName = episodeName;

            tblEpisodeFilename.Text = episodeName.FileNameWithoutExtension + ":";
            tbxEpisodeName.Text = episodeName.Name;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            episodeName.Name = tbxEpisodeName.Text;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
