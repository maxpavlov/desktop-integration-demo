using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using companion.Services;
using companion.ViewModels;
using companion.Views;

namespace companion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = ContainerConfig.BuildContainer();

            container.Resolve<IUriProtocolService>().RegisterUriScheme();                         

            ShowMainWindow(e.Args, container);
        }

        private void ShowMainWindow(string[] args, IContainer container)
        {
            if (args.Length > 0)
            {

                var queryParams = container.Resolve<IUriProtocolService>().GetUriParams(args);
                var signalRService = container.Resolve<ISignalRService>();
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(signalRService, queryParams)
                };
                mainWindow.Show();
            }
            else
            {
                var splash = new SplashWindow
                {
                    DataContext = new SplashWindowViewModel()
                };
                splash.Show();
            }
        }
    }
}
