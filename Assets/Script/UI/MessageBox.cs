using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.UI
{
    class MessageBox:MonoBehaviour
    {


        public event Action<Message> OnMessage;

        public  void SendMessage(object Sender,MessageType type,string text1,string text2,object Content)
        {
            Message mg = new Message()
            {
                MessageType = type,
                text1 = text1,
                text2 = text2,
                Content = Content,
                Sender = Sender
            };
            if (OnMessage != null)
            {
                try
                {
                    OnMessage(mg);
                }
                catch
                {

                }
            }
        }
    }

    public enum MessageType
    {
        System,
        Custom
    }


    class Message
    {
        public object Sender;
        public MessageType MessageType;
        public string text1;
        public string text2;
        public object Content;
    }
}
