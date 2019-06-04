using FolderFile;
using System.Windows;

namespace SerienBenennen
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                if (e.Args.Length > 0) ViewModel.Current.Folder = new Folder(e.Args[0], SubfolderType.This, true);
            }
            catch { }
        }
    }
}
