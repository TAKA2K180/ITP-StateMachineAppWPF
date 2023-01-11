using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITP_StateMachine.Helpers;
using System.Diagnostics;
using ITP_StateMachine.Views;
using ITP_StateMachine.IDTechReader;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows;
using ITP_StateMachine.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ITP_StateMachine.Classes
{
    public class EventRecordManager
    {
        #region Variables
        TimeSpan time;
        public static string[] arg;
        SingleQueueHelper SingleQueue = new SingleQueueHelper();
        IDTechReader.CardReader card = new IDTechReader.CardReader(arg);
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        #endregion

        #region Constructor
        public EventRecordManager()
        {

        }
        #endregion

        #region Methods
        public void ReceiveQueue()
        {
            var message = SingleQueue.ReceiveQueue();

            if (message.Contains("Card swiped by user"))
            {
                MainViewModel main = new MainViewModel();
                main.dispatcherTimer.Start();
                if (CardDetails.PrevCardNumber != CardDetails.CardNumber)
                {
                    PreviewWindow instance = Application.Current.Windows.OfType<PreviewWindow>().SingleOrDefault();
                    if (instance == null)
                    {
                        PreviewWindow preview = new PreviewWindow();
                        preview.Show();
                        MainViewModel.CloseAction();
                        CardDetails.PrevCardNumber = CardDetails.CardNumber;
                        CardDetails.PrevCardId = CardDetails.CorpId;
                        LogHelper.SendLogToText("Preview window show");
                    }
                }
                SingleQueue.DeleteAllMessages();
            }
            else if (message.Contains("Card finished reading"))
            {

            }
            else if (message.Contains("Program exit"))
            {
                SingleQueue.DeleteAllMessages();
            }
            else if (message.Contains("Cancel button pressed by user"))
            {
                PreviewViewModel.CloseAction.Invoke();
                SingleQueue.DeleteAllMessages();
                SingleQueue.SendToQueue("Preview window closed");
                this.ReceiveQueue();
            }
            else if (message.Contains("Preview window closed"))
            {
                SingleQueue.DeleteAllMessages();
                SingleQueue.SendToQueue("Device search initialize");
                LogHelper.SendLogToText("Device search initialize");
                this.ReceiveQueue();
            }
            else if (message.Contains("Program initialize") || message.Contains("Device search initialize"))
            {
                card.SetStatus();
                if (CardDetails.MachineState == false)
                {
                    CardDetails.HardwareStatus = "Device not detected.";
                    SingleQueue.SendToQueue("Device not detected");
                    LogHelper.SendLogToText("Device not detected");
                }
                else if (CardDetails.MachineState == true)
                {
                    CardDetails.HardwareStatus = "Device online, please swipe your card.";
                    SingleQueue.SendToQueue("Device online");
                    LogHelper.SendLogToText("Device online");
                }
                if (message.Contains("Program initialize"))
                {
                    MainWindow window = new MainWindow();
                    window.Show();
                    SingleQueue.DeleteAllMessages();
                    SingleQueue.SendToQueue("Main window show");
                    LogHelper.SendLogToText("Main window show");
                }
                SingleQueue.DeleteAllMessages();
            }
            else if (message.Contains("Timer for device detection start"))
            {
                SingleQueue.SendToQueue("Device search initialize");
                this.ReceiveQueue();
                LogHelper.SendLogToText("Device search initialize");
                SingleQueue.DeleteAllMessages();
            }
            else if (message.Contains("Idle timer start"))
            {

            }
        }
        #endregion
    }
}
