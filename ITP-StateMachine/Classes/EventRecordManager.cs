using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITP_StateMachine.Common.Helpers;
using ITP_StateMachine.Helpers;

namespace ITP_StateMachine.Classes
{
    public class EventRecordManager
    {
        MsmqHelper msmq = new MsmqHelper();
        
        DeviceWatcher device = new DeviceWatcher();
        public EventRecordManager()
        {

        }

        public void ReceiveCommand(string CardNumber)
        {
            var message = msmq.ReceiveCommandQueue();
            if (message.Contains("Card swiped by user"))
            {
                device.ApplicationWatcher("IDTech Encrypted", "iTellerPlus.IDTechReader.exe", "0ACD|0500", device.OutputHandler);
                CardNumber = CardDetails.CardNumber;
                if (CardNumber.Contains("3%"))
                {
                    string[] arr = CardNumber.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    CardDetails.CardNumber = Convert.ToString("Card Number:\n" + "0000000000" + arr[1].TrimStart('6', '0', '='));
                    msmq.SendCommandQueue($"Card Number: {CardDetails.CardNumber}");
                }
                else
                {
                    Console.WriteLine(CardNumber);
                    CardDetails.CardNumber = CardNumber;
                }
            }
            else if (message.Contains("Card finished reading"))
            {
                Thread.Sleep(10000);
                msmq.SendTimerQueue("Timer start");
            }
            else if (message.Contains("Program initialize") || message.Contains("Device search initialize"))
            {
                device.ApplicationWatcher("IDTech Encrypted", "iTellerPlus.IDTechReader.exe", "0ACD|0500", device.OutputHandler);

                if (CardDetails.MachineState == false)
                {
                    CardDetails.CardNumber = "Device not detected.";
                    msmq.SendHardwareQueue("Device not detected");
                    //msmq.SendTimerQueue("Timer for device detection start");
                    //Thread.Sleep(2000);
                    //this.ReceiveTimerQueue(0, 10000);
                }
                else if (CardDetails.MachineState == true)
                {
                    CardDetails.CardNumber = "Device online, please swipe your card.";
                    msmq.SendHardwareQueue("Device online");
                }
            }
            else if (message.Contains("Program exit"))
            {
                device.ApplicationExit("IDTech Encrypted", "iTellerPlus.IDTechReader.exe", "0ACD|0500", device.OutputHandler);
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
                //
            }
            
        }
        public void ReceiveTimerQueue(int seconds, int millisecond)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            var message = msmq.ReceiveTimerQueue();
            if (message.Contains("Timer start"))
            {
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, seconds);
                dispatcherTimer.Start();
            }
            else if (message.Contains("Timer for device detection start"))
            {
                Thread.Sleep(millisecond);
                msmq.SendCommandQueue("Device search initialize");
                this.ReceiveCommand(null);
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
