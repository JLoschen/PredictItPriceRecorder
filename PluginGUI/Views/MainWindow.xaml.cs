
using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using PluginGUI.Properties;
using PluginGUI.ViewModels;

namespace PluginGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindowViewModel MainWindowViewModel { get; set; } = new MainWindowViewModel();

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {           
            Settings.Default.SelectedPlugin = JsonConvert.SerializeObject(MainWindowViewModel.SelectedPlugin);
            Settings.Default.Save();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
