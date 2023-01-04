using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITP_StateMachine.Common.Helpers;
using ITP_StateMachine.Helpers;

namespace ITP_StateMachine.Classes
{
    public class DeviceWatcher
    {
        public void ApplicationWatcher(string readername, string applicationname, string arguments, DataReceivedEventHandler handlername)
        {
            var process = Process.GetProcesses().FirstOrDefault(item => item.ProcessName == applicationname);

            if (process == null)
            {
                using (var watcher = new Process())
                {
                    watcher.StartInfo.FileName = System.IO.Path.Combine("D:\\Sources\\iTellerPlus.IDTechReader\\iTellerPlus.IDTechReader\\obj\\Debug", applicationname);
                    watcher.StartInfo.UseShellExecute = false;
                    watcher.StartInfo.CreateNoWindow = true;
                    watcher.StartInfo.RedirectStandardInput = true;
                    watcher.StartInfo.RedirectStandardOutput = true;
                    watcher.StartInfo.Arguments = arguments;
                    watcher.StartInfo.Verb = "runas";
                    watcher.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    watcher.OutputDataReceived += new DataReceivedEventHandler(handlername);
                    watcher.Start();
                    watcher.BeginOutputReadLine();
                }
            }


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

        public void ApplicationExit(string readername, string applicationname, string arguments, DataReceivedEventHandler handlername)
        {
            var process = Process.GetProcesses().FirstOrDefault(item => item.ProcessName == applicationname);

            if (process == null)
            {
                using (var watcher = new Process())
                {
                    watcher.StartInfo.FileName = System.IO.Path.Combine("D:\\Sources\\iTellerPlus.IDTechReader\\iTellerPlus.IDTechReader\\obj\\Debug", applicationname);
                    watcher.StartInfo.UseShellExecute = false;
                    watcher.StartInfo.CreateNoWindow = true;
                    watcher.StartInfo.RedirectStandardInput = true;
                    watcher.StartInfo.RedirectStandardOutput = true;
                    watcher.StartInfo.Arguments = arguments;
                    watcher.StartInfo.Verb = "runas";
                    watcher.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    watcher.OutputDataReceived += new DataReceivedEventHandler(handlername);
                    watcher.Close();
                }
            }
        }
    }
}
