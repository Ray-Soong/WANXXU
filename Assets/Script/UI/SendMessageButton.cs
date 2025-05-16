using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Script.UI
{
    class SendMessageButton:MonoBehaviour
    {

        private void Start()
        {
            Button bt = this.GetComponent<Button>();
            bt.onClick.AddListener(clk);
        }
        void clk()
        {
            if (Target != null)
            {
                Target.SendMessage(this, Type, Text1, Text2, null);
            }
        }

         
        public MessageBox Target;
        public MessageType Type;
        public string Text1;
        public string Text2;
     

    }

    
}
