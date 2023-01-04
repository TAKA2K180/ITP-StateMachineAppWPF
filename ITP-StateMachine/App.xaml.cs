using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Helpers;
using ITP_StateMachine.ViewModels;

namespace ITP_StateMachine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MsmqHelper msmq = new MsmqHelper();
        EventRecordManager events = new EventRecordManager();
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            msmq.SendHardwareQueue($"{e.Exception.Message}");
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Window window = new MainWindow(null);
            window.DataContext = new MainViewModel(null);
            window.Show();
            //base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            msmq.SendCommandQueue("Program exit");
            events.ReceiveCommand(null);
        }
    }
}
