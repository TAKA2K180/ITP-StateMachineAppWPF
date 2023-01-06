using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITP_StateMachine.Helpers;

namespace ITP_StateMachine.Classes
{
    public class DeviceWatcher
    {
        public void ApplicationWatcher(string readername, string applicationname, string arguments, DataReceivedEventHandler handlername)
        {
            applicationname = "iTellerPlus.IDTechReader";
            var process = Process.GetProcesses().Where(u => u.ProcessName == "iTellerPlus.IDTechReader").ToList();
            if (process.Count <= 0)
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
                    //CardDetails.MachineState = true;
                }
            }
            else
            {
                foreach (var item in process)
                {
                    if (item.ProcessName == "iTellerPlus.IDTechReader")
                    {
                        foreach (var processes in Process.GetProcessesByName("iTellerPlus.IDTechReader"))
                        {
                            processes.Kill();
                        }

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
                            //CardDetails.CardNumber = "Device online";
                            //CardDetails.MachineState = true;
                        }
                    }
                }
            }
        }
       

        public void StartDevice()
        {

        }
    }
}
