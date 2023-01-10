using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITP_StateMachine.Helpers;
using iTellerPlus.IDTechReader;
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
        MsmqHelper msmq = new MsmqHelper();
        DeviceWatcher device = new DeviceWatcher();
        IDTechReader.CardReader card = new IDTechReader.CardReader(arg);
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        #endregion

        #region Constructor
        public EventRecordManager()
        {

        }
        #endregion

        #region Methods
        public void ReceiveCommand()
        {
            var message = msmq.ReceiveCommandQueue();

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
                        WindowChecker.WindowCheck = true;
                        CardDetails.PrevCardNumber = CardDetails.CardNumber;
                        CardDetails.PrevCardId = CardDetails.CorpId;
                    }
                }
            }
            else if (message.Contains("Card finished reading"))
            {

            }
            else if (message.Contains("Program exit"))
            {
                msmq.DeleteMessages();
            }
            else if (message.Contains("Cancel button pressed by user"))
            {
                PreviewViewModel.CloseAction.Invoke();
                msmq.DeleteMessages();
                msmq.SendCommandQueue("Preview window closed");
                this.ReceiveCommand();
            }
            else if (message.Contains("Preview window closed"))
            {
                MainViewModel main = new MainViewModel();
                msmq.DeleteMessages();
            }
        }

        public void ReceiveHardwareQueue()
         {
            var message = msmq.ReceiveHardwareQueue().ToString();
            if (message.Contains("Device online"))
            {

            }
            else if (message.Contains("Device not detected"))
            {

            }
            else if (message.Contains("Program initialize") || message.Contains("Device search initialize"))
            {
                card.SetStatus();
                if (CardDetails.MachineState == false)
                {
                    CardDetails.HardwareStatus = "Device not detected.";
                    msmq.SendHardwareQueue("Device not detected");
                    LogHelper.SendLogToText("Device not detected");
                }
                else if (CardDetails.MachineState == true)
                {
                    CardDetails.HardwareStatus = "Device online, please swipe your card.";
                    msmq.SendHardwareQueue("Device online");
                    LogHelper.SendLogToText("Device online");
                }
            }
        }
        public void ReceiveTimerQueue(int seconds, int millisecond)
        {
            var message = msmq.ReceiveTimerQueue();
            if (message.Contains("Timer start"))
            {
                //this.ReceiveCommand(null);
            }
            else if (message.Contains("Timer for device detection start"))
            {
                Thread.Sleep(millisecond);
                msmq.SendCommandQueue("Device search initialize");
                this.ReceiveCommand();
                LogHelper.SendLogToText("Device search initialize");
            }
            else if (message.Contains("Idle timer start"))
            {
                
            }
        }
        #endregion
    }
}
