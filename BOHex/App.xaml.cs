using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BOHex
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var mapViewModel = new MapViewModel(22, 13);

            var mainWindowViewModel = new MainWindowViewModel(mapViewModel);

            var mainWindow = new MainWindow {DataContext = mainWindowViewModel};
            MainWindow = mainWindow;
            
            mainWindow.Show();
        }
    }
}
