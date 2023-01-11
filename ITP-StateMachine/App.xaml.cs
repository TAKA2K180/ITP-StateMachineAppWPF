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
        SingleQueueHelper singleQueue = new SingleQueueHelper();
        EventRecordManager events = new EventRecordManager();
        public static String[] arg;
        
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            singleQueue.SendToQueue($"{e.Exception.Message}");
            LogHelper.SendLogToText("Exception message: " + e.Exception.Message);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            singleQueue.DeleteAllMessages();
            singleQueue.SendToQueue($"Program initialize");
            LogHelper.SendLogToText($"Program initialize");
            events.ReceiveQueue();
            //IDTechReader.CardReader cardReader = new IDTechReader.CardReader(arg);
            //cardReader.Show();
            
            base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            singleQueue.SendToQueue("Program exit");
            LogHelper.SendLogToText("Program exit");
            events.ReceiveQueue();

            singleQueue.DeleteAllMessages();

            foreach (var processes in Process.GetProcessesByName("iTellerPlus.IDTechReader"))
            {
                processes.Kill();
            }
        }
    }
}
