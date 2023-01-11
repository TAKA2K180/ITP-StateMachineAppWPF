using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSMQ.Messaging;

namespace ITP_StateMachine.Helpers
{
    public class SingleQueueHelper
    {
        public static string Address = ".\\Private$\\StateMachineEventQueue";

        public void SendToQueue(string body)
        {
            Message msg = new Message();
            msg.Body = $"[{DateTime.Now}] " + body;
            MessageQueue messageQueue = new MessageQueue(Address);
            messageQueue.Label = "State Machine Event Queue";
            messageQueue.Send(msg);
        }

        public string ReceiveQueue()
        {
            string messagebody = default;

            if (MessageQueue.Exists(Address))
            {

                var queue = new MessageQueue(Address)
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
        public void DeleteAllMessages()
        {
            MessageQueue queue = new MessageQueue(Address);
            queue.Purge();

        }
    }
}
