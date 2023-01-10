using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITP_StateMachine.IDTechReader
{
    public partial class CardReader : Form
    {
        HIDInterface hid = new HIDInterface();
        public static String[] mArgs;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        struct connectStatusStruct
        {
            public bool preStatus;
            public bool curStatus;
        }

        connectStatusStruct connectStatus = new connectStatusStruct();
        public delegate void isConnectedDelegate(bool isConnected);
        public isConnectedDelegate isConnectedFunc;
        public HIDInterface.HidDevice hidDevice = new HIDInterface.HidDevice();
        public delegate void PushReceiveDataDele(byte[] datas);
        public PushReceiveDataDele pushReceiveData;
        MsmqIDtech msmq = new MsmqIDtech();

        public CardReader(string[] arg)
        {
            InitializeComponent();

            GetStartUp(arg);
            //CardPreview cardPreview = new CardPreview();
            //cardPreview.Show();

            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Interval = 5000;
            timer.Start();
        }
        public void TimerEventProcessor(Object sender, EventArgs args)
        {
        //    timer.Stop();
        //    SetStatus();
        //    timer.Start();
        }
        private void GetStartUp(string[] data)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.SetVisibleCore(false);

            //string[] arr = null;

            //if (data.Length > 0)
            //{
            //    string magtechdetails = data[0].ToString().ToUpper();

            //    arr = magtechdetails.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            //}

            //string vid = arr[0];
            //string pid = arr[1];

            string vid = "0ACD";
            string pid = "0500";

            int intvid = Convert.ToInt32(vid, 16);
            int intpid = Convert.ToInt32(pid, 16);

            ushort uvid = Convert.ToUInt16(intvid);
            ushort upid = Convert.ToUInt16(intpid);

            hid.StatusConnected = StatusConnected;
            hid.DataReceived = DataReceived;

            hidDevice.vID = uvid;
            hidDevice.pID = upid;

            hidDevice.serial = "";
            hid.AutoConnect(hidDevice);
        }


        public bool SendBytes(byte[] data)
        {
            return hid.Send(data);
        }

        public void DataReceived(object sender, string e)
        {
            if (e.Contains("3%"))
            {
                string[] arr = e.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("0000" + "|" + arr[1].ToString());
                CardDetails.CardNumber = Convert.ToString("0000000000" + arr[1].TrimStart('6', '0', '='));
                var Id = new string(arr[1].Take(4).ToArray());
                CardDetails.CorpId = Convert.ToInt32(Id);
            }
            else
            {
                Console.WriteLine(e);
                CardDetails.CardNumber = e;
            }
            //msmq.SendCommandQueue("Card swiped by user");
        }

        public void StatusConnected(object sender, bool isConnect)
        {
            connectStatus.curStatus = isConnect;
            if (connectStatus.curStatus == connectStatus.preStatus)  //connect
                return;
            connectStatus.preStatus = connectStatus.curStatus;
            if (CardDetails.counter == 0)
            {
                CardDetails.counter++;
                StatusCheck();
            }
        }

        public void StatusCheck()
        {
            if (connectStatus.curStatus != connectStatus.preStatus)
            {
                SetStatus();
            }
        }

        public void SetStatus()
        {
            if (connectStatus.curStatus == false)
            {
                CardDetails.MachineState = false;
                CardDetails.HardwareStatus = "Device not detected.";
                msmq.SendHardwareQueue("Device not detected");
            }
            else if (connectStatus.curStatus == true)
            {
                CardDetails.MachineState = true;
                CardDetails.HardwareStatus = "Device online, please swipe your card.";
                msmq.SendHardwareQueue("Device online");
            }
        }

    }
}
