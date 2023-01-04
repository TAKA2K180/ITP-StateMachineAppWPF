using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using iTellerPlus.IDTechReader;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Common.Helpers;
using ITP_StateMachine.Helpers;

namespace ITP_StateMachine.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _deviceStatus;

        public string DeviceStatus
        {
            get { return _deviceStatus; }
            set { _deviceStatus = value; OnPropertyChanged("DeviceStatus"); }
        }
        public bool LoadingVisibility
        {
            get { return LoadingVisibility; }
            set { LoadingVisibility = value; OnPropertyChanged("Loading"); }
        }


        MsmqHelper msmq = new MsmqHelper();
        
        EventRecordManager eventRecordManager = new EventRecordManager();
        public MainViewModel(string[] arg)
        {
            this._deviceStatus = CardDetails.CardNumber;
            LoadingVisibility = true;
        }
        

        public void DataReceived(object sender, string e)
        {
            msmq.SendCommandQueue("Card swiped by user");
            eventRecordManager.ReceiveCommand(e);

            msmq.SendCommandQueue("Card finished reading");
            eventRecordManager.ReceiveCommand(null);

            msmq.SendTimerQueue("Timer start");
            eventRecordManager.ReceiveTimerQueue(10,0);
        }

        public void IdleTimer(int start, string message)
        {

        }
    }
}
