using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script
{
    /// <summary>
    /// 点位设置
    /// </summary>
    [ExecuteInEditMode]
    public class WayPoint:MonoBehaviour
    {
        public object CommData;
        public string DeviceType;
        public bool PassData;
        public bool IsCreatePoint;
        public bool IsDeletePoint;
        [NonSerialized]
        public List<NextPointInfo> Outputs = new List<NextPointInfo>();
        public int Way;
        public int WayIndex;
        int way, index;
        public int NameFontSize = 40;
        public int TextFontSize = 40;
        TextMesh namemesh;
        TextMesh textmesh;
        public float UseTime;
        public event IoChangedHandle IoChanged;
        public float SetSpeed;
        public bool Stop;
        public bool IsTable;
        public bool AutoPoint;

       

        public string LastSendPallet;
        public bool LastHasBox;
        //隐藏
        public void hide()
        {
            var p = this.transform.GetChild(0);
           for(int i = 0; i < p.childCount; i++)
            {
                p.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
        }
        public float CurrentSpeed
        {
            get
            {
                return Stop ? 0 : SetSpeed;
            }
        }
        Pallet pp = null;
        public Pallet CurrentPallet
        {
            get {
                if (pp != null)
                {
                    if (pp.current == this)
                        return pp;
                }
                return null;
            } set { pp = value; }

        }

        public float GetUseTime()
        {
            return UseTime;
        }
        public Vector3 Point
        {
            get { return this.transform.position; }
        }

        private void Awake()
        {
            var a = GameObject.Find("Main").GetComponent<WayView>();
            if (a != null)
            {
                a.RegistPoint(this);
            }
        }

        

        private void Start()
        {
            namemesh = G.FindChild(this.transform,"text").GetComponent<TextMesh>();
            textmesh = G.FindChild(this.transform, "text2").GetComponent<TextMesh>();
        }
        void updateText()
        {
            if(namemesh.text!=this.name || namemesh.fontSize != NameFontSize)
            {
                namemesh.fontSize = NameFontSize;
                namemesh.text = this.name;
            }

            if (way != Way || WayIndex != index || textmesh.fontSize != TextFontSize)
            {
                way = Way;
                index = WayIndex;
                textmesh.fontSize = TextFontSize;
                textmesh.text = string.Format("{0}-{1}", way, WayIndex);
            }   
        }
        public void NotifyIo(MonoBehaviour pallet, PointIoStatus status)
        {
            if (this.IoChanged != null)
            {
                this.IoChanged(this,pallet, status);
            }
        }
        public void NotifyUseTime(float time)
        {
           
        }

        public MonoBehaviour ObjectOnPoint;
        private void Update()
        {
            updateText();
        }

        //判断料箱是否能进点位函数回调
        public IoAbleHandle CanInput;
        ////判断料箱是否能出点位函数回调
        public IoAbleHandle CanOutput;
        //选择路线的函数回调
        public SelectWay SelectWayHandler;

        public bool InputAble(MonoBehaviour pallet,WayPoint from)
        {
            if (CanInput == null)
                return true;
            return CanInput(pallet, from, this);
        }

        public bool OutputAble(MonoBehaviour pallet, WayPoint to)
        {
            if (CanOutput == null)
                return true;
            return CanOutput(pallet, this, to);
        }



        public bool CanSelectWay { get { return SelectWayHandler != null; } }

        public WayPoint SelectWay(MonoBehaviour pallet)
        {
            if (SelectWayHandler == null)
                return null;
            else
                return SelectWayHandler(pallet, this);
        }


    }

  


    public delegate void IoChangedHandle(WayPoint point, MonoBehaviour pallet, PointIoStatus status);

    public delegate WayPoint SelectWay(MonoBehaviour pallet, WayPoint From);

    public delegate bool IoAbleHandle(MonoBehaviour pallet, WayPoint from, WayPoint To);

   




    public class NextPointInfo
    {
        public WayPoint Point;
        public string WayName;
        public bool IsWayStart;
        public bool IsWayEnd;
        public WayInfo Way;
    }

}








