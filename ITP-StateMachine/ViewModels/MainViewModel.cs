using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using iTellerPlus.IDTechReader;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Commands;
using ITP_StateMachine.Helpers;
using ITP_StateMachine.IDTechReader;
using ITP_StateMachine.Views;

namespace ITP_StateMachine.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        public System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer deviceStatusTimer = new System.Windows.Threading.DispatcherTimer();
        MsmqHelper msmq = new MsmqHelper();
        EventRecordManager eventRecordManager = new EventRecordManager();

        private bool _deviceStatus;
        private bool _loadingVisibility;
        private string _cardNumber;
        private int _counter;
        private string prevNumber;
        private bool prevDeviceStatus;

        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }
        private string _hardwareStatus;

        public string HardwareStatus
        {
            get { return _hardwareStatus; }
            set { _hardwareStatus = value; OnPropertyChanged(HardwareStatus); }
        }


        public bool DeviceStatus
        {
            get { return CardDetails.MachineState; }
            set { CardDetails.MachineState = value; OnPropertyChanged("DeviceStatus"); }
        }
        public bool LoadingVisibility
        {
            get { return _loadingVisibility; }
            set { _loadingVisibility = value; OnPropertyChanged("Loading"); }
        }
        public string CardNumber
        {
            get { return CardDetails.CardNumber; }
            set { CardDetails.CardNumber = value; OnPropertyChanged(CardNumber); }
        }

        public static Action CloseAction { get; set; }

        public MainViewModel()
        {
            LoadingVisibility = true;

            this.HardwareStatus = CardDetails.HardwareStatus;
            this.CardNumber = CardDetails.CardNumber;
            prevNumber = CardDetails.CardNumber;
            prevDeviceStatus = CardDetails.MachineState;
            WindowChecker.WindowCheck = false;

            

            //deviceStatusTimer.Tick += deviceStatusTimer_Tick;
            //deviceStatusTimer.Interval = new TimeSpan(0, 0, 5);
            //deviceStatusTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.CardNumber = CardDetails.CardNumber;
            if (CardDetails.CardNumber != CardDetails.PrevCardNumber)
            {
                msmq.SendCommandQueue("Card swiped by user");
                eventRecordManager.ReceiveCommand();
                LogHelper.SendLogToText($"Card swiped by user\nCard details:\nCard Number: {CardDetails.CardNumber}\nCorp ID: {CardDetails.CorpId}");
                //msmq.DeleteMessages();
            }
            prevNumber = CardNumber;
        }

        private void deviceStatusTimer_Tick(object sender, EventArgs e)
        {
            if (CardDetails.MachineState != this.prevDeviceStatus)
            {
                msmq.SendHardwareQueue("Device search initialize");
                eventRecordManager.ReceiveHardwareQueue();
                this.CardNumber = CardDetails.CardNumber;
                prevDeviceStatus = CardDetails.MachineState;
            }
        }

        public void DataReceived(object sender, string e)
        {
            msmq.SendCommandQueue("Card finished reading");
            eventRecordManager.ReceiveCommand();

            msmq.SendTimerQueue("Timer start");
            eventRecordManager.ReceiveTimerQueue(10,0);
        }

        public void Exit()
        {
            
        }

        public void OnLoad()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
    }
}
