using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSMQ.Messaging;
using Message = MSMQ.Messaging.Message;

namespace ITP_StateMachine.IDTechReader
{
    public class MsmqIDtech
    {
        public void SendCommandQueue(string body)
        {
            Message msg = new Message();
            msg.Body = $"[{DateTime.Now}] " + body;
            MessageQueue messageQueue = new MessageQueue(".\\Private$\\StateMachineCommandQueue");
            messageQueue.Label = "State Machine Command Queue";
            messageQueue.Send(msg);
        }
        public void SendHardwareQueue(string body)
        {
            Message msg = new Message();
            msg.Body = body;
            MessageQueue messageQueue = new MessageQueue(".\\Private$\\StateMachineCommandQueue");
            messageQueue.Label = "State Machine Command Queue";
            messageQueue.Send(msg);
        }
    }
}
