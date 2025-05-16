using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Script
{
    public class SwitchPositionScript:MonoBehaviour
    {
        public ObjectPosition[] Positions;

        public void SetPosition(int index)
        {
            if (Positions != null)
            {
                if(index>=0 && index < Positions.Length)
                {
                    this.gameObject.transform.position = Positions[index].Position;
                    this.gameObject.transform.eulerAngles = Positions[index].Angle;
                }
            }
        }
    }

    [System.Serializable]
    public class ObjectPosition
    {
        public Vector3 Angle;
        public Vector3 Position;
        public string Name;
    }
}
