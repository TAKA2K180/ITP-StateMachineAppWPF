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

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        MsmqHelper msmq = new MsmqHelper();
        EventRecordManager eventRecordManager = new EventRecordManager();

        private bool _deviceStatus;
        private bool _loadingVisibility;
        private string _cardNumber;
        private int _counter;
        private string prevNumber;

        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
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

        public event PropertyChangedEventHandler propertChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertChanged != null)
            {
                propertChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MainViewModel()
        {
            LoadingVisibility = true;

            this.CardNumber = CardDetails.CardNumber;
            prevNumber = CardDetails.CardNumber;

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            PreviewWindow preview = new PreviewWindow();
            //Thread.Sleep(3000);
            this.CardNumber = CardDetails.CardNumber;
            if (CardNumber != prevNumber)
            {
                msmq.SendCommandQueue("Card swiped by user");
                eventRecordManager.ReceiveCommand(null);
            }
            prevNumber = CardNumber;
        }

        public void DataReceived(object sender, string e)
        {
            msmq.SendCommandQueue("Card finished reading");
            eventRecordManager.ReceiveCommand(null);

            msmq.SendTimerQueue("Timer start");
            eventRecordManager.ReceiveTimerQueue(10,0);
        }
    }
}
