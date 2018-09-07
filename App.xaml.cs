using FolderFile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            var list = e.Args.ToArray();

            try
            {
                if (list.Length > 0) ViewModel.Current.Folder = new Folder(list[0], SubfolderType.This);
            }
            catch { }
        }
    }
}
