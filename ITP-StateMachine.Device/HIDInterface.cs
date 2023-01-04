using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ITP_StateMachine.Device
{
    public class HIDInterface : IDisposable
    {

        public enum MessagesType
        {
            Message,
            Error
        }

        public struct ReusltString
        {
            public bool Result;
            public string message;
        }

        public struct HidDevice
        {
            public UInt16 vID;
            public UInt16 pID;
            public string serial;
        }
        HidDevice lowHidDevice = new HidDevice();

        public delegate void DelegateDataReceived(object sender, string data);
        public DelegateDataReceived DataReceived;

        public delegate void DelegateStatusConnected(object sender, bool isConnect);
        public DelegateStatusConnected StatusConnected;

        public bool bConnected = false;


        public Hid oSp = new Hid();
        private static HIDInterface m_oInstance;

        public struct TagInfo
        {
            public string AntennaPort;
            public string EPC;
        }

        public HIDInterface()
        {
            m_oInstance = this;
            oSp.DataReceived = HidDataReceived;
            oSp.DeviceRemoved = HidDeviceRemoved;
        }

        protected virtual void RaiseEventConnectedState(bool isConnect)
        {
            if (null != StatusConnected) StatusConnected(this, isConnect);
        }

        protected virtual void RaiseEventDataReceived(string buf)
        {
            if (null != DataReceived) DataReceived(this, buf);
        }

        public void AutoConnect(HidDevice hidDevice)
        {
            lowHidDevice = hidDevice;
            ContinueConnectFlag = true;
            ReadWriteThread.DoWork += ReadWriteThread_DoWork;
            ReadWriteThread.WorkerSupportsCancellation = true;
            ReadWriteThread.RunWorkerAsync();
        }

        public void StopAutoConnect()
        {
            try
            {
                ContinueConnectFlag = false;
                Dispose();
            }
            catch
            {

            }
        }

        ~HIDInterface()
        {
            Dispose();
        }

        public bool Connect(HidDevice hidDevice)
        {
            ReusltString result = new ReusltString();

            Hid.HID_RETURN hdrtn = oSp.OpenDevice(hidDevice.vID, hidDevice.pID, hidDevice.serial);

            if (hdrtn == Hid.HID_RETURN.SUCCESS)
            {

                bConnected = true;

                #region 
                result.Result = true;
                result.message = "Connect Success!";
                RaiseEventConnectedState(result.Result);
                #endregion


                return true;
            }

            bConnected = false;

            #region 
            result.Result = false;
            result.message = "Device Connect Error";
            RaiseEventConnectedState(result.Result);

            #endregion
            return false;
        }


        public bool Send(byte[] byData)
        {
            byte[] sendtemp = new byte[byData.Length + 1];
            sendtemp[0] = (byte)byData.Length;
            Array.Copy(byData, 0, sendtemp, 1, byData.Length);

            Hid.HID_RETURN hdrtn = oSp.Write(new report(0, sendtemp));

            if (hdrtn != Hid.HID_RETURN.SUCCESS)
            {
                return false;
            }
            return true;
        }

        public bool Send(string strData)
        {

            byte[] data = Encoding.Unicode.GetBytes(strData);
            return Send(data);
        }



        public void DisConnect()
        {
            bConnected = false;
            //CardDetails.MachineState = false;

            Thread.Sleep(200);
            if (oSp != null)
            {
                oSp.CloseDevice();
            }
        }


        void HidDeviceRemoved(object sender, EventArgs e)
        {
            bConnected = false;
            #region 
            ReusltString result = new ReusltString();
            result.Result = false;
            result.message = "Device Remove";
            RaiseEventConnectedState(result.Result);
            #endregion
            if (oSp != null)
            {
                oSp.CloseDevice();
            }

        }

        public void HidDataReceived(object sender, string e)
        {

            string value = e; // System.Text.ASCIIEncoding.ASCII.GetString(e.reportBuff).Substring(36, 21);
            try
            {
                RaiseEventDataReceived(value);
            }
            catch
            {
                #region 
                ReusltString result = new ReusltString();
                result.Result = false;
                result.message = "Receive Error";
                RaiseEventConnectedState(result.Result);
                #endregion
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder stringHex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                stringHex.AppendFormat("{0:x2}", b);
            }
            return stringHex.ToString();
        }



        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }



        public void Dispose()
        {
            try
            {
                this.DisConnect();
                oSp.DataReceived -= HidDataReceived;
                oSp.DeviceRemoved -= HidDeviceRemoved;
                ReadWriteThread.DoWork -= ReadWriteThread_DoWork;
                ReadWriteThread.CancelAsync();
                ReadWriteThread.Dispose();
            }
            catch
            { }
        }

        Boolean ContinueConnectFlag = true;
        private BackgroundWorker ReadWriteThread = new BackgroundWorker();
        private void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (ContinueConnectFlag)
            {
                try
                {
                    if (!bConnected)
                    {
                        Connect(lowHidDevice);

                    }
                    Thread.Sleep(500);
                }
                catch { }
            }
        }

    }
}
