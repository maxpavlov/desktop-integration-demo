using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using companion.Models;
using companion.Services;
using companion.Utils;

namespace companion.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ISignalRService _signalR;
        private ActionSettings _settings;
        private string _status = "Processing input...";
        private string _measurementId = "none";

        public DelegateCommand ReadyCommand { get; set; }

        public void OnWindowClosing(object parameter)
        {
            _signalR.SendMessage(_measurementId, "STOPPED");
            base.CloseWindow(parameter);
    }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged("Status"); }
        }

        public MainWindowViewModel(ISignalRService signalR, string queryParams)
        {
            _signalR = signalR;
            this.ReadyCommand = new DelegateCommand(OnReady, null);
            this.CloseCommand = new DelegateCommand(OnWindowClosing, null);    
            ReadParams(queryParams);
        }

        private void ReadParams(string queryParams)
        {
            var action = queryParams.Substring(0, queryParams.IndexOf('/'));
            var rest = queryParams.Substring(queryParams.IndexOf('/') + 1);

            NameValueCollection queryParameters = new NameValueCollection();
            string[] querySegments = rest.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }

            //foreach (string queryParameter in queryParameters)
            //{
            //    MessageBox.Show(queryParameters[queryParameter], queryParameter);
            //}

            _measurementId = queryParameters["measurementId"];

            if (Enum.TryParse(action, out ActionType type))
            {
                _settings = new ActionSettings
                {
                    ActionType = type
                };
            }
            if (_settings == null)
                Status = $"Application start failed: invalid input params.";

            Status = $"Initializing mode: {_settings.ActionType}";

            _signalR.Connect();
            Status = $"Connected to hub...";
            _signalR.SendMessage(_measurementId, "STARTED");
            Status = $"Notified about startup...";
        }

        private void OnReady(object parameter)
        {
            try
            {
                string applicationLocation = typeof(App).Assembly.Location;
                applicationLocation = applicationLocation.Substring(0, applicationLocation.LastIndexOf('\\') + 1);

                if(_settings.ActionType == ActionType.cardiograph) 
                {
                    Process firstProc = new Process();
                    firstProc.StartInfo.FileName = applicationLocation + "third-party-one.exe";
                    firstProc.EnableRaisingEvents = true;

                    firstProc.Exited += FirstProc_Exited;

                    firstProc.Start();
                } else if (_settings.ActionType == ActionType.eyegraph)
                {
                    Process secondProc = new Process();
                    secondProc.StartInfo.FileName = applicationLocation + "third-party-two.exe";
                    secondProc.EnableRaisingEvents = true;

                    secondProc.Exited += SecondProc_Exited;

                    secondProc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FirstProc_Exited(object sender, EventArgs e)
        {
            var results = string.Empty;

            string applicationLocation = typeof(App).Assembly.Location;
            applicationLocation = applicationLocation.Substring(0, applicationLocation.LastIndexOf('\\') + 1);

            var fileLoc = applicationLocation + "cardio.txt";

            if (File.Exists(fileLoc))
            {
                results = File.ReadAllText(fileLoc);
            }

            if (results == string.Empty)
            {
                _signalR.SendMessage(_measurementId, "STOPPED");
                Status = "Failed to collect results. Restart the operation.";
            }

            var payload = "DATA:" + results;

            _signalR.SendMessage(_measurementId, payload);

            Status = "Results sent.";

            Task.Delay(2000).ContinueWith(
                _ =>
                {
                    Environment.Exit(-1);
                } );
        }

        private void SecondProc_Exited(object sender, EventArgs e)
        {
            var results = string.Empty;

            string applicationLocation = typeof(App).Assembly.Location;
            applicationLocation = applicationLocation.Substring(0, applicationLocation.LastIndexOf('\\') + 1);

            var fileLoc = applicationLocation + "sight.txt";

            if (File.Exists(fileLoc))
            {
                results = File.ReadAllText(fileLoc);
            }

            if (results == string.Empty)
            {
                _signalR.SendMessage(_measurementId, "STOPPED");
                Status = "Failed to collect results. Restart the operation.";
            }

            var payload = "DATA:" + results;

            _signalR.SendMessage(_measurementId, payload);

            Status = "Results sent.";

            Task.Delay(2000).ContinueWith(
                _ =>
                {
                    Environment.Exit(-1);
                });
        }
    }
}
