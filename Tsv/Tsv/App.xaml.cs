using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tsv.Service;
using Tsv.ViewModel;

namespace Tsv
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Logger log = Log.GetLogger(() => typeof(App));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Ioc.Default.GetInstance<MainViewModel>().Show();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error("Unhanlded Domain Exception: {0}", (e.ExceptionObject as Exception).StackTrace);
        }

        private void CurrentDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            log.Error("Unhanlded Dispatcher Exception: {0}", e.Exception.StackTrace);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Application.Current.DispatcherUnhandledException -= CurrentDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledException;
            base.OnExit(e);
        }
    }
}
