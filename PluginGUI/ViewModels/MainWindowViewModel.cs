//-- Copyright © 2012 Menard, Inc.  Eau Claire, WI.

using System;
using System.AddIn.Hosting;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Menards.Merch.DataEngine.API.Plugins;
using Menards.Merch.DataEngine.HostView;
using Newtonsoft.Json;
using PluginGUI.Models;
using PluginGUI.Properties;

namespace PluginGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly string PluginDirectory = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Plugins";

        public MainWindowViewModel()
        {
            LoadPluginCollection();
            UpdateAddInCommand = new RelayCommand(UpdateAddInStore);
        }

        private async void LoadPluginCollection()
        {
            try
            {
                LoadingPlugins = true;
                await SetPluginCollection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Loading Plugins{Environment.NewLine}{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingPlugins = false;
            }
        }

        public PluginViewModel SelectedPlugin
        {
            get { return _selectedPlugin; }
            set
            {
                Set(ref _selectedPlugin, value);
                _selectedPlugin?.Activate();
            }
        }
        private PluginViewModel _selectedPlugin;

        public ICollectionView PluginCollection
        {
            get { return _pluginCollection; }
            private set { Set(ref _pluginCollection, value); }
        }
        private ICollectionView _pluginCollection;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if(Set(ref _searchText, value))
                    PluginCollection.Refresh();
            }
        }
        private string _searchText = string.Empty;

        public ICommand UpdateAddInCommand { get; set; }
        
        private async void UpdateAddInStore()
        {
            LoadingPlugins = true;
            try
            {
                await Task.Run(async () =>
                {
                    var pluginManager = new PluginManager(PluginDirectory);
                    pluginManager.RebuildAddInStore();
                    await SetPluginCollection();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Loading Plugins{Environment.NewLine}{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingPlugins = false;
            }
        }

        public bool LoadingPlugins
        {
            get { return _loadingPlugins; }
            set { Set(ref _loadingPlugins, value); }
        }
        private bool _loadingPlugins;

        private async Task SetPluginCollection()
        {
            var pluginList = await Task.Run(() =>
            {
                var pluginManager = new PluginManager(PluginDirectory);
                return pluginManager.ValidPlugins(typeof(IDataEngineHostView))
                    .Select(token => new PluginViewModel(token))
                    .ToList();
            });
            PluginCollection = CollectionViewSource.GetDefaultView(pluginList);
            PluginCollection.GroupDescriptions.Add(new PropertyGroupDescription("Prefix", null, StringComparison.InvariantCultureIgnoreCase));
            PluginCollection.SortDescriptions.Add(new SortDescription("Prefix", ListSortDirection.Ascending));
            PluginCollection.SortDescriptions.Add(new SortDescription("PluginName", ListSortDirection.Ascending));
            PluginCollection.Filter += Filter;
            PluginCollection.MoveCurrentToFirst();

            var selected = JsonConvert.DeserializeObject<PluginConfig>(Settings.Default.SelectedPlugin ?? "");
            SelectedPlugin = pluginList.FirstOrDefault(p => p.PluginName == selected?.PluginName);
            if (SelectedPlugin != null)
            {
                SelectedPlugin.HostType = selected.HostType;
                SelectedPlugin.TeradataHostType = selected.TeradataHostType;
                SelectedPlugin.RestartDelay = selected.RestartDelay;
                SelectedPlugin.TestEmail = selected.TestEmail;
            }
            else
            {
                SelectedPlugin = PluginCollection.CurrentItem as PluginViewModel;
            }
        }

        private bool Filter(object o)
        {
            var plugin = o as PluginViewModel;
            return plugin?.PluginName.ToUpper().Contains(SearchText.ToUpper()) == true;
        }
    }
}