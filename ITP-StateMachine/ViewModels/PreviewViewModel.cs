using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using iTellerPlus.IDTechReader;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Commands;
using ITP_StateMachine.Helpers;
using ITP_StateMachine.IDTechReader;
using ITP_StateMachine.Views;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ITP_StateMachine.ViewModels
{
    public class PreviewViewModel : BaseViewModel
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        MsmqHelper msmq = new MsmqHelper();
        EventRecordManager eventRecordManager = new EventRecordManager();


        private string _cardNumber;
        private int _corpId;


        private string prevNumber;
        private int prevCorpId;

        public string CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; OnPropertyChanged("CardNumber"); }
        }

        public int CorpId
        {
            get { return _corpId; }
            set { _corpId = value; OnPropertyChanged("CorpId"); }
        }

        public RelayCommand LoadedEvent { get; set; }
        public RelayCommand CloseEvent { get; set; }
        public RelayCommand IdleTimerCommand { get; set; }


        public PreviewViewModel()
        {
            this._cardNumber = CardDetails.CardNumber;
            this._corpId = CardDetails.CorpId;
            prevCorpId = this._corpId;
            prevNumber = this._cardNumber;

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (CardDetails.CardNumber != prevNumber)
            {
                this.CardNumber = CardDetails.CardNumber;
                this.CorpId = CardDetails.CorpId;
            }
            prevNumber = CardNumber;
        }
    }
}
