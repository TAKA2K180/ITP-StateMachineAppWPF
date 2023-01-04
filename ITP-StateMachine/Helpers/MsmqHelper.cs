using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using MSMQ.Messaging;

namespace ITP_StateMachine.Helpers
{
    partial class MsmqHelper
    {
        public struct MessageBody
        {
            public string Body;
        }
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
            msg.Body = $"[{DateTime.Now}] " + body;
            MessageQueue messageQueue = new MessageQueue(".\\Private$\\StateMachineHardwareQueue");
            messageQueue.Label = "State Machine Hardware Queue";
            messageQueue.Send(msg);
        }

        public void SendTimerQueue(string body)
        {
            Message msg = new Message();
            msg.Body = $"[{DateTime.Now}] " + body;
            MessageQueue messageQueue = new MessageQueue(".\\Private$\\StateMachineTimerQueue");
            messageQueue.Label = "State Machine Timer Queue";
            messageQueue.Send(msg);
        }
        public string ReceiveCommandQueue()
        {
            string messagebody = default;
            //note, you cannot use method exists on remote queues

            if (MessageQueue.Exists(".\\Private$\\StateMachineCommandQueue"))
            {

                var queue = new MessageQueue(".\\Private$\\StateMachineCommandQueue")
                {
                    MessageReadPropertyFilter = new MessagePropertyFilter
                    {
                        ArrivedTime = true,
                        Body = true
                    }
                };


                var messages = queue.GetAllMessages().FirstOrDefault();
                var m = messages;
                m.Formatter = new XmlMessageFormatter(new String[] { });

                StreamReader sr = new StreamReader(m.BodyStream);

                string ms = "";
                string line;

                while (sr.Peek() >= 0)
                {
                    ms += sr.ReadLine();
                }

                //ms now contains the message
                messagebody = ms;
            }

            return messagebody;
        }
        public string ReceiveHardwareQueue()
        {
            string messagebody = default;
            //note, you cannot use method exists on remote queues

            if (MessageQueue.Exists(".\\Private$\\statemachinehardwarequeue"))
            {

                var queue = new MessageQueue(".\\Private$\\statemachinehardwarequeue")
                {
                    MessageReadPropertyFilter = new MessagePropertyFilter
                    {
                        ArrivedTime = true,
                        Body = true
                    }
                };


                var messages = queue.GetAllMessages().FirstOrDefault();
                var m = messages;
                m.Formatter = new XmlMessageFormatter(new String[] { });

                StreamReader sr = new StreamReader(m.BodyStream);

                string ms = "";
                string line;

                while (sr.Peek() >= 0)
                {
                    ms += sr.ReadLine();
                }

                //ms now contains the message
                messagebody = ms;
            }


            return messagebody;
        }
        public string ReceiveTimerQueue()
        {
            string messagebody = default;
            //note, you cannot use method exists on remote queues

            if (MessageQueue.Exists(".\\Private$\\statemachinetimerqueue"))
            {

                var queue = new MessageQueue(".\\Private$\\statemachinetimerqueue")
                {
                    MessageReadPropertyFilter = new MessagePropertyFilter
                    {
                        ArrivedTime = true,
                        Body = true
                    }
                };


                var messages = queue.GetAllMessages().FirstOrDefault();
                var m = messages;
                m.Formatter = new XmlMessageFormatter(new String[] { });

                StreamReader sr = new StreamReader(m.BodyStream);

                string ms = "";
                string line;

                while (sr.Peek() >= 0)
                {
                    ms += sr.ReadLine();
                }

                //ms now contains the message
                messagebody = ms;
            }


            return messagebody;
        }
    }
}
