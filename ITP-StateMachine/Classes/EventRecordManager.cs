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
        public void ReceiveCommand(string CardNumber)
        {
            var message = msmq.ReceiveCommandQueue();

            if (message.Contains("Card swiped by user"))
            {
                if (CardDetails.PrevCardNumber != CardDetails.CardNumber)
                {
                    if (WindowChecker.WindowCheck == false)
                    {
                        PreviewWindow preview = new PreviewWindow();
                        preview.Show();
                        WindowChecker.WindowCheck = true;
                    }
                }
                CardDetails.PrevCardNumber = CardDetails.CardNumber;
                CardDetails.PrevCardId = CardDetails.CorpId;
            }
            else if (message.Contains("Card finished reading"))
            {

            }
            else if (message.Contains("Program exit"))
            {

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
                //device.ApplicationWatcher("IDTech Encrypted", "iTellerPlus.IDTechReader.exe", "0ACD|0500", OutputHandler);
                card.SetStatus();
                if (CardDetails.MachineState == false)
                {
                    CardDetails.CardNumber = "Device not detected.";
                    msmq.SendHardwareQueue("Device not detected");
                }
                else if (CardDetails.MachineState == true)
                {
                    CardDetails.CardNumber = "Device online, please swipe your card.";
                    msmq.SendHardwareQueue("Device online");
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
                this.ReceiveCommand(null);
            }
            else if (message.Contains("Idle timer start"))
            {
                
            }
        }

        public void TimerStop()
        {
            
        }

        public void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            EventRecordManager events = new EventRecordManager();
            MsmqHelper msmq = new MsmqHelper();
            var details = outLine.Data;
            CardDetails.CardNumber = details;
            msmq.SendCommandQueue("Card swiped by user");
            events.ReceiveCommand(details);
        } 
        #endregion
    }
}
