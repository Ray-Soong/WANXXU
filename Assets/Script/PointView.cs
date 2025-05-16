using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**************************************************************************
 *          ���ڵ�λ����;
 *  1.  ͨ��������λ�������豸���ӳ�һ���������ˣ��������豸�����ǿ�λ�����Ǵ�������һ�ж��ǵ�λ����Щ�㹻����������������
 *  2.  �κζ����ű�ʵ���ڵ�λ���ʱ���������λ�����¼���ģ����ʵ�е��豸�������ƣ������������̾ͱ����һ���ջ��������ǵ�λ�����ߣ�������������
 *  3.  ��Ϊ�����磬�Ϳ�����·���滮����ô�Ϳ���������ֽ⣬����ڸ���λ����ֱ���������·���Ͼ����ĵ㣬��Ҫ�õ��豸����ô����ȫ����ģ��WCS��WCS��������,Ȼ��ת��Ϊ�������ݡ�
 *  4.  ��Ϊ�л������ƣ�������ֽ⣬��ô�Ϳ�������豸������������������¼��������Էֽ���������·�ִ�У�ʵ�ַ��湦�ܣ�ͬʱ�·�ָ����·�����ͨѶ���ݵ�ģʽ�·���Ҳͬʱʵ�������������Խӵ��ԣ���
 *  5.  �¼������������иû��ƣ��¼������߾Ϳ������ɵ��������������Ϣ��������Ӧ��ӳ����������߱����������WMS������WCS��������WMS��WCS��ô�������ʵ�֣�
 *  6.  ���������WMS,WCS�����ϣ���һ������MES��֧���µ����ܣ���ô��ֱ�ӱ�������⹤��������ʵ�ַ��棬ͬʱ����������
 *  7.  �¼������������Զ�������¼�����ôֱ�ӾͿ�������������������֮��ĵ�WMS��WCS,MES�Խӡ�

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
    //��ɾ����
    public bool IsDeletePoint;
    //�ǳ�����
    public bool IsCreatePoint;
    //�����ݵ�
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
    //·������
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

//��λ��Ϣ����
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


//�����Ϣ����
[Serializable]
public class EndpointDescription: PointInfoDescription
{
    //������
    public string StoreNumber;
    //���·�߱�ţ���·�������
    public string StoreWayNumber;
    //�������
    public string StorePointNumber;
    //�˵����ͱ��
    public EndpointType PointType;
    //�˿�����
    public string EndpointName;
    //¥���
    public int Floor;
}

public enum EndpointType
{
    //������
    BirthPoint,
    //ɾ����
    DeletePoint,
    //���ڽ����
    StoreInputPoint,
    //���ڳ����
    StoreOutputPoint,
    //��������
    StoreInputExternPoint,
    //��������
    StoreOutputExternPoint
}





//�Ѷ����Ϣ����
[Serializable]
public class StackerDescription: PointInfoDescription
{
   
}

//AGV��Ϣ����
[Serializable]
public class AGVDescription: PointInfoDescription
{
    public string AGVName;
    public string Xiangdao;
    public int Level;
    public bool CanSwitchLevel;
   // public AGV Script;
}



//���Ի�/��������Ϣ/��ת̨�������������豸����
[Serializable]
public class LiftDescription: PointInfoDescription
{
  
}






//�豸��λ
public class DevicePoint
{
    public string Type;
    //�������
    public Transform point;
    //�豸���
    public string DeviceNumber;
    //��λ���
    public string PointCode;
    //��λ��·���ϵ�˳���
    public int PointIndex;
    //���������
    public bool IsPalletCreatePoint;
    //����ɾ����
    public bool IsPalletDeletePoint;
    internal string[] outputs;
    //·�ߺ�
    public int WayNumber;
    public DevicePoint prev;
    public DevicePoint next;
    public DevicePoint[] Outputs;
    //�Զ���������õ�λ�������ݣ����������Զ�����������ȵ������źŲ������
    public bool AutoEnd;
    public StackerDescription StackerDescription;
    public EndpointDescription EndpointDescription;
    public AGVDescription AGVDescription;
    public LiftDescription LiftDescription;
    //��λģ������
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
    //���������λ
    public DevicePoint[] Input;
    //����������λ
    public DevicePoint[] Output;
    //��λ����
    public Vector3 Point
    {
        get
        {
            return point.position;
        }
    }
    //�ƶ�ʱ����õ㣨ר�
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

    //��λֵ�����Ը���λ����ֵ�����ڼ��<=0Ϊ��Чֵ�����, �����λ�������ˣ���ô����ǰһ����ֵ���ں�һ���㣬�����һ������˵�����嵹���ˣ�
    public float PointValue;
    //��λ��һ��̨�棨̨����ƶ��������豸�ϣ�
    public bool IsTablePoint;
    public string TableName;
    //�㱨ʵ��ʹ��ʱ�䣬�����ٶ�����Ӧʹ�ã����ٶ�ͳ�ƣ������֧�ֵ�λ���ٶ��������ӻ���
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





//��λ����״̬
public enum PointIoStatus
{
    BeginEnter,
    AnimationEnter,
    Enter,
    BeginLeave,
    Leave
}



