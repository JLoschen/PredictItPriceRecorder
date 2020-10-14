using System;
using System.AddIn.Hosting;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Menards.Merch.BOA.Core;
using Menards.Merch.DataEngine.API;
using Menards.Merch.DataEngine.Plugin;
using Newtonsoft.Json;
using PluginGUI.Models;

namespace PluginGUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PluginViewModel : ViewModelBase
    {
        private readonly Plugin _plugin;
        private readonly DataEngineRunner _dataEngineRunner;

        public PluginViewModel()
        {
            CanRun = false;
        }

        public PluginViewModel(AddInToken pluginObject)
        {
            _plugin = new Plugin(pluginObject);
            TestEmail = $"{Environment.UserName}@menards.net";
            HostType = HostServerType.Dev;
            TeradataHostType = HostServerType.Dev;

            _dataEngineRunner = new DataEngineRunner(_plugin);

            StopCommand = new RelayCommand(Stop);
            RunCommand = new RelayCommand(Run);
            SkumanEmailCommand = new RelayCommand(() => TestEmail = "isboappsteam@menards.net");
            PomanEmailCommand = new RelayCommand(() => TestEmail = "ispoteam@menards.net");
            UserEmailCommand = new RelayCommand(() => TestEmail = Environment.UserName + "@menards.net");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public async void Activate()
        {
            CanRun = false;
            await _plugin.Activate();
            RaisePropertyChanged(() => RestartDelay);
            CanRun = true;
        }

        public ICommand RunCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SkumanEmailCommand { get; set; }
        public ICommand UserEmailCommand { get; set; }
        public ICommand PomanEmailCommand { get; set; }

        [JsonProperty]
        public HostServerType HostType
        {
            get { return _plugin.HostServerType; }
            set
            {
                _plugin.HostServerType = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public HostServerType TeradataHostType
        {
            get { return _plugin.TeradataHostServerType; }
            set
            {
                _plugin.TeradataHostServerType = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public string TestEmail
        {
            get { return _plugin.TestEmail; }
            set
            {
                _plugin.TestEmail = value;
                RaisePropertyChanged(); 
            }
        }

        [JsonProperty]
        public int RestartDelay
        {
            get { return _plugin.RestartDelay; }
            set
            {
                _plugin.RestartDelay = value;
                RaisePropertyChanged();
            }
        }

        public string OutputText
        {
            get { return _outputText; }
            set { Set(ref _outputText, value); }
        } 
        private string _outputText;

        public bool CanRun
        {
            get { return _canRun; }
            set { Set(ref _canRun, value); }
        }
        private bool _canRun = true;

        public bool CanStop
        {
            get { return _canStop; }
            set { Set(ref _canStop, value); }
        }
        private bool _canStop;


        [JsonProperty]
        public string PluginName => _plugin.Name;

        public string Prefix => PluginName.Contains(".") ? PluginName.Split('.')[0] : "Other";

        public string PluginDescription => _plugin.Description;

        private void Run()
        {
            CanRun = false;

            Task.Run(() =>
            {                   
                Logging.ClearMemoryAppender();
                var task = _dataEngineRunner.Run();
                CanStop = true;

                while (!task.IsCompleted)
                {
                    Task.Delay(100).Wait();
                    OutputText = Logging.GetMemoryAppenderBlankLayout(_dataEngineRunner.LoggingEvents.ToArray());
                }

                OutputText = Logging.GetMemoryAppenderBlankLayout(_dataEngineRunner.LoggingEvents.ToArray());
            }).ContinueWith(r => CanRun = true);
        }

        private void Stop()
        {
            CanStop = false;

            Task.Run(() =>
            {
                var task = _dataEngineRunner.Stop();

                while (!task.IsCompleted)
                {
                    Task.Delay(100).Wait();
                    OutputText = Logging.GetMemoryAppenderBlankLayout(_dataEngineRunner.LoggingEvents.ToArray());
                }

                OutputText = Logging.GetMemoryAppenderBlankLayout(_dataEngineRunner.LoggingEvents.ToArray());
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OutputText += $"Plugin {_plugin} has a critical error on {Environment.MachineName}.";
        }
    }
}