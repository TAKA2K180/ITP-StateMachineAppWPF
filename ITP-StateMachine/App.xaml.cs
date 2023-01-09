using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using iTellerPlus.IDTechReader;
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
        LogHelper log = new LogHelper();
        public static String[] arg;
        
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            msmq.SendHardwareQueue($"{e.Exception.Message}");
            LogHelper.SendLogToText("Exception message: " + e.Exception.Message);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            msmq.SendHardwareQueue($"Program initialize");
            events.ReceiveHardwareQueue();
            LogHelper.SendLogToText($"Program initialize");
            ITP_StateMachine.IDTechReader.CardReader cardReader = new IDTechReader.CardReader(arg);
            cardReader.Show();
            Window window = new MainWindow();
            window.Show();
            base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            msmq.SendCommandQueue("Program exit");
            LogHelper.SendLogToText("Program exit");
            events.ReceiveCommand(null);

            foreach (var processes in Process.GetProcessesByName("iTellerPlus.IDTechReader"))
            {
                processes.Kill();
            }
        }
    }
}
