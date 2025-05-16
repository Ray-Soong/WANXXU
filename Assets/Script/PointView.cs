using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**************************************************************************
 *          关于点位的用途
 *  1.  通过描述点位将所有设备连接成一个网络拓扑，无论是设备，还是库位，还是传感器点一切都是点位，这些点够成了整个物流网络
 *  2.  任何动画脚本实体在点位变更时，都会向点位发送事件（模拟现实中的设备反馈机制，这样仿真流程就变成了一个闭环），凡是点位侦听者，都能侦听到。
 *  3.  因为有网络，就可以做路径规划，那么就可以做任务分解，给入口给库位，就直接算出整个路径上经过的点，和要用的设备，那么就完全可以模拟WCS和WCS发送任务,然后转化为孪生数据。
 *  4.  因为有回馈机制，有任务分解，那么就可以针对设备做虚拟调度器，根据事件反馈，对分解任务逐个下发执行，实现仿真功能（同时下发指令，以下发孪生通讯数据的模式下发，也同时实现了离线孪生对接调试）。
 *  5.  事件机制驱动，有该机制，事件侦听者就可以轻松的侦听动画变更信息，做出对应反映，如果真听者本身就是虚拟WMS，虚拟WCS，甚至在WMS和WCS那么仿真就能实现，
 *  6.  如果在虚拟WMS,WCS基础上，加一层虚拟MES，支持下单功能，那么就直接变成了虚拟工厂，真正实现仿真，同时又是孪生。
 *  7.  事件机制如果作成远程网络事件，那么直接就可以用真正独立于孪生之外的的WMS，WCS,MES对接。

 ************************************************************************/



[ExecuteInEditMode]
public class PointView : MonoBehaviour
{

    public string text = "PointView";
    TextMesh ms;
    TextMesh ms1;
    Transform line;
    Transform port;
    public float Length = 1;
    public float Offset = 0.5f;
    public string DeviceNumber;
    float size = 1;
    float offset;
    public string[] Outputs;
    public bool IsTablePoint;
    public float MoveTime;
    //是删除点
    public bool IsDeletePoint;
    //是出生点
    public bool IsCreatePoint;
    //无数据点
    public bool NoDataPoint;



    public void NotifyUseTime(float time)
    {

    }


    public PointView[] NextPoints;

  
  



    private void Awake()
    {
        if (G.ShowPoint == false)
            G.GetSceneScript().PointManager.AddPoint(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        line = G.FindChild(this.gameObject.transform,"Line");
        port = G.FindChild(this.gameObject.transform, "Cube");
        ms = G.FindChild(this.gameObject.transform,"text").GetComponent<TextMesh>();
        ms1 = G.FindChild(this.gameObject.transform, "text2").GetComponent<TextMesh>();

    }
    // Update is called once per frame
    void Update()
    {
        if (ms != null)
        {
            if (ms.text != name)
            {
                ms.text = name;
            }
        }
        if (ms1 != null)
        {
            if (this.text != ms1.text)
            {
                ms1.text = this.text;
            }
        }
        if (size != Length || offset != Offset)
        {
            this.size = Length;
            this.offset = Offset;
            var pr = line.transform.localScale;// = size;
            pr.x = Length;
            line.transform.localScale = pr;
            var pp = port.transform.localPosition;
            pp.x = (Offset - 0.5f) * Length;
            port.transform.localPosition = pp;
        }

    }
    public IEnumerable<DevicePoint> GetPoints()
    {
        string code = null;
        var tt = name.Trim();
        if (tt.Length > 0)
        {
            if (tt[0] != '#')
            {
                code = tt;
            }
        }
        string[] ss = text.Split('-');
        DevicePoint p = new DevicePoint()
        {
            point = G.FindChild(this.transform, "Cube"),
            DeviceNumber = DeviceNumber.Trim(),
            Input = null,
            Output = null,
            IsTablePoint = IsTablePoint,
            PointCode = code,
            WayNumber = int.Parse(ss[0]),
            PointIndex = int.Parse(ss[ss.Length - 1]),
            manultime = this.MoveTime,
            outputs=this.Outputs
        };
        return new DevicePoint[] { p };
    }
    public CellDescription CellDescription;
    public StackerDescription StackerDescription;
    public EndpointDescription EndpointDescription;
    public AGVDescription AGVDescription;
    public LiftDescription LiftDescription;
}







public class Way
{
    DevicePoint[] points;

    public int Number;
    public DevicePoint EndPoint { get { return points[points.Length - 1]; } }
    public DevicePoint BeginPoint { get { return points[0]; } }
    public Way(int number,DevicePoint[] points)
    {
        this.Number = number;
        this.points = points;
        foreach(var e in points)
        {
          
            e.Way = this;
        }
    }
}


public enum PointType
{
    Nomral,
    Begin,
    End
}



public class PointManager
{
    List<PointView> views = new List<PointView>();
    List<DevicePoint> points = new List<DevicePoint>();
    List<Way> way = new List<Way>();
    public int Count { get { return points.Count; } }
    public IEnumerable<DevicePoint> Points { get { return points; } }
    public void AddPoint(PointView p)
    {
        views.Add(p);
    }
    public DevicePoint FindByPointCode(string code)
    {
        foreach(var e in points)
        {
            if (e.PointCode == code)
            {
                return e;
            }
        }
        return null;
    }
    public DevicePoint[] FindPointsByDeviceId(string id)
    {
        return (from a in points
                where a.DeviceNumber == id
                select a).ToArray();
    }
    public void Create()
    {
        List<DevicePoint> ps = new List<DevicePoint>();
        this.points = ps;
        List<Way> ways = new List<Way>();
        foreach(var e in views)
        {
            ps.AddRange(e.GetPoints());
        }
        var k = (from a in ps
                 group a by a.WayNumber into g
                 select g).ToArray();
        foreach(var e in k)
        {
            var kk = (from a in e
                      orderby a.PointIndex ascending
                      select a).ToArray();
            for(int i = 0; i < kk.Length; i++)
            {
                DevicePoint p1 = i >0 ? kk[i - 1] : null;
                DevicePoint p2 = i < kk.Length - 1 ? kk[i + 1]:null;
                kk[i].prev = p1;
                kk[i].next = p2;
            }
            Way w = new Way(e.Key,kk);
           
            ways.Add(w);
        }
        this.way = ways;
        List<DevicePoint> outs = new List<DevicePoint>();
        foreach(var e in ps)
        {
            e.Outputs = null;
            outs.Clear();
            if (e.outputs != null)
            {
                if (e.outputs.Length > 0)
                {
                    
                    foreach (var ss in e.outputs)
                    {
                        foreach(var kr in this.way)
                        {
                            if (kr.Number==int.Parse(ss))
                            {
                                outs.Add(kr.BeginPoint);
                                break;
                            }
                        }
                    }
                    e.Outputs = outs.ToArray();
                }
            }
        }
        this.way = ways;
    }
    public DevicePoint GetWayBeginPoint(int number)
    {
        foreach(var e in way)
        {
            if (e.Number == number)
            {
                return e.BeginPoint;
            }
        }
        return null;
    }
    //路径搜索
    bool FindPath(DevicePoint curr, DevicePoint target, List<DevicePoint> ps)
    {
        int c = ps.Count;
        if (curr.next != null)
        {
            ps.Add(curr.next);
            if (curr == target)
            {
                return true;
            }
            var ret = FindPath(curr.next, target, ps);
            if (ret)
                return true;
        }
        while (ps.Count > c)
        {
            ps.RemoveAt(ps.Count - 1);
        }
        if (curr.Outputs != null)
        {
            foreach(var e in curr.Outputs)
            {
                ps.Add(e);
                if (e == target)
                    return true;
                var ret = FindPath(curr.next, target, ps);
                if (ret)
                    return true;
                while (ps.Count > c)
                {
                    ps.RemoveAt(ps.Count - 1);
                }
            }
        }
        return false;
    }


}



class Cell
{
    public string Xiangdao;
    public int Kongwei;
    public Vector3 Point;
    public string Number;
}

//库位信息描述
[Serializable]
public class CellDescription
{
    public bool able;
    public float UpdateInterval;
}


[Serializable]
public class PointInfoDescription
{
    public bool able;
    public float UpdateInterval;
    public string TypeName;
}


//入口信息描述
[Serializable]
public class EndpointDescription: PointInfoDescription
{
    //巷道编号
    public string StoreNumber;
    //巷道路线编号（多路线巷道）
    public string StoreWayNumber;
    //巷道点编号
    public string StorePointNumber;
    //端点类型编号
    public EndpointType PointType;
    //端口名称
    public string EndpointName;
    //楼层号
    public int Floor;
}

public enum EndpointType
{
    //出生点
    BirthPoint,
    //删除点
    DeletePoint,
    //库内进库点
    StoreInputPoint,
    //库内出库点
    StoreOutputPoint,
    //库外进库点
    StoreInputExternPoint,
    //库外出库点
    StoreOutputExternPoint
}





//堆垛机信息描述
[Serializable]
public class StackerDescription: PointInfoDescription
{
   
}

//AGV信息描述
[Serializable]
public class AGVDescription: PointInfoDescription
{
    public string AGVName;
    public string Xiangdao;
    public int Level;
    public bool CanSwitchLevel;
   // public AGV Script;
}



//移栽机/提升机信息/旋转台描述，即单轴设备描述
[Serializable]
public class LiftDescription: PointInfoDescription
{
  
}






//设备点位
public class DevicePoint
{
    public string Type;
    //坐标对象
    public Transform point;
    //设备编号
    public string DeviceNumber;
    //点位编号
    public string PointCode;
    //点位在路线上的顺序号
    public int PointIndex;
    //货物出生点
    public bool IsPalletCreatePoint;
    //货物删除点
    public bool IsPalletDeletePoint;
    internal string[] outputs;
    //路线号
    public int WayNumber;
    public DevicePoint prev;
    public DevicePoint next;
    public DevicePoint[] Outputs;
    //自动结束代表该点位是有数据，无数据则自动结束，否则等到结束信号才算结束
    public bool AutoEnd;
    public StackerDescription StackerDescription;
    public EndpointDescription EndpointDescription;
    public AGVDescription AGVDescription;
    public LiftDescription LiftDescription;
    //点位模拟数据
    internal object SimulationToken;

    List<float> usetimes = new List<float>();
    float self_adaption_time = 0;
    public float MoveTime
    {
        get
        {
            if (self_adaption_time > 0)
                return self_adaption_time;
            else
                return manultime;
        }
    }
    public float manultime;
    public Way Way;
    //入口相连点位
    public DevicePoint[] Input;
    //出口相连点位
    public DevicePoint[] Output;
    //点位坐标
    public Vector3 Point
    {
        get
        {
            return point.position;
        }
    }
    //移动时看向该点（专项）
    public bool LookAt;
    public event Action<DevicePoint,Pallet, PointIoStatus> IoChanged;
    public event CanEnterPoint CanEnterPoint;

    public bool SimCanEnterPoint(Pallet p)
    {
        if (CanEnterPoint != null)
        {
            bool rt = false;
            CanEnterPoint(this, p, ref rt);
            return rt;
        }
        return true;
    }

    //点位值，可以给点位设置值，用于检错（<=0为无效值不检测, 假设点位不允许倒退，那么发现前一个的值大于后一个点，则代表一个错误说明物体倒退了）
    public float PointValue;
    //点位是一个台面（台面会移动并且在设备上）
    public bool IsTablePoint;
    public string TableName;
    //汇报实际使用时间，用于速度自适应使用，和速度统计（后面会支持点位的速度质量可视化）
    public void NotifyUseTime(float time)
    {
        this.self_adaption_time = time;
    }
    public void NotifyIo(Pallet pallet, PointIoStatus status)
    {

        if (IoChanged != null)
        {
            IoChanged(this,pallet, status);
        }
    }
}

public delegate void CanEnterPoint(DevicePoint point, Pallet pallet,ref bool result);





//点位进出状态
public enum PointIoStatus
{
    BeginEnter,
    AnimationEnter,
    Enter,
    BeginLeave,
    Leave
}



