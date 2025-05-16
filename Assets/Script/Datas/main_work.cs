using Assets.Script;
using Assets.Script.Datas;
using Assets.Script.Motion;
using Assets.Scripts;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class main_work : MonoBehaviour
{

    #region 路由点，和路由方式
    public string InputOrBackCrossPoint;

    public string Bank1InputCrossPoint;

    public Outline CarOutLine1;
    public Outline CarOutLine2;
    public Outline CarOutLine3;
    public Outline CarOutLine4;
    public Outline CarOutLine5;
    public Outline CarOutLine6;
    public Outline LiftOutLine1;
    public Outline LiftOutLine2;
    public Outline CLiftOutLine1;
    public Outline CLiftOutLine2;


    public Lift1 Lift1;
    public Lift1 Lift2;
    List<temporaryarea> temporaryareas = new List<temporaryarea>();
    WayPoint in_back_p;
    WayPoint bank1_p;
    void create_cross(WayView wv)
    {
        in_back_p = wv.FindPoint(InputOrBackCrossPoint);
        bank1_p = wv.FindPoint(Bank1InputCrossPoint);
        in_back_p.SelectWayHandler = SelectWay1;
        bank1_p.SelectWayHandler = SelectWay2;
    }

    WayPoint SelectWay1(MonoBehaviour pallet, WayPoint From)
    {
        Pallet p = pallet as Pallet;
     //   p.TaskType = TaskType.Input;
        switch (p.TaskType)
        {
            case TaskType.None:
                break;
            case TaskType.Input:
                foreach(var e in From.Outputs)
                {
                    if (e.Point.DeviceType == "input")
                    {
                        return e.Point;
                    }
                }
                break;
            case TaskType.Output:
                break;
            case TaskType.Move:
                break;
            default:
                break;
        }
        return null;
    }

    WayPoint SelectWay2(MonoBehaviour pallet, WayPoint From)
    {
        Pallet p = pallet as Pallet;
        switch (p.TaskType)
        {
            case TaskType.None:
                break;
            case TaskType.Input:
                foreach (var e in From.Outputs)
                {
                    if (e.Point.DeviceType == p.InputBank)
                    {
                        return e.Point;
                    }
                }
                break;
            case TaskType.Output:
                break;
            case TaskType.Move:
                break;
            default:
                break;
        }
        return null;
    }


    #endregion




    public bool History = false;
    // Start is called before the first frame update
    HistoryDataHelper history = new HistoryDataHelper();
    SceneScript sc;
    WayView ways;
    Item[] items;
    public RGV_H[] rgvs;

    public class Item
    {
        public WayPoint point;
        public string Code;
     
        public Pallet pallet;
    }
    bool first = false;
    interfaceline_realtime_info dt = null;
    List<string> ss = new List<string>();
    bool debug = false;
    public bool Save;
    void save()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var e in sc.PalletManager.Pallets)
        {
           e.save(sb);
        }
        System.IO.File.WriteAllText("D:\\上海.txt", sb.ToString());
    }

    class token
    {
        public StringBuilder sb = new StringBuilder();
        public string last_task_view;
        public string lastpoint;
        public string lastinterface1;
        public string lastinterface2;
        public string h1;
        public string h2;
    }

    class saveitem
    {
        public static List<saveitem> items = new List<saveitem>();
        public string pallet;
        public string point;
        public string task;
        public StringBuilder sb = new StringBuilder();

        public bool Push(PointQueue q)
        {
          return false;
            if (q.ContainerCode == pallet)
            {
                if (q.PointCode != point)
                {
                    point = q.PointCode;
                    sb.Append(q.PointCode);
                    sb.Append(" ");
                }
                return true;
            }
            return false;
        }

        public void PushPoint(string s)
        {
           return;
                if (s!= point)
                {
                   point = s;
                    sb.Append(s);
                sb.Append(" ");
            }
        }
        public bool PushTask(task_realtime_info task)
        {
            return false;
            if (task.ContainerCode == pallet)
            {
                string ss = string.Format("t-{0}-{1}-{2}-{3}:{4} ", task.BankID, task.TaskType, task.Layer, task.TaskStatus, task.TaskStep);
                if (ss != this.task)
                {
                    this.task = ss;
                    sb.Append(ss);
                    sb.Append(" ");
                }
                return true;
            }
            return false;
        }

  
        public static void pushPoint(string code, string point)
        {
            return;
            foreach (var e in items)
            {
                if (e.pallet == code)
                {
            //        e.PushPoint(point);
                    return;
                }
            }
            saveitem s = new saveitem();
            s.pallet = code;
            items.Add(s);
         //   s.PushPoint(point);
        }

        public static void push(task_realtime_info task)
        {
            return;
            foreach (var e in items)
            {
            //    if (e.PushTask(task))
              //      return;
            }
            saveitem s = new saveitem();
            s.pallet = task.ContainerCode;
            items.Add(s);
          //  s.PushTask(task);

        }

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var e in items)
            {
                sb.Append("["+e.pallet+"]\r\n");
                sb.AppendLine(e.sb.ToString());
            }
            System.IO.File.WriteAllText(@"D:\上海.txt", sb.ToString());
        }
    }

    public Clift clift1, clift2;

    WayPoint getPoint(task_realtime_info t)
    {
        string ff = string.Format("t-{0}-{1}-2",t.BankID,t.Layer);
        return ways.FindPoint(ff);

    }
    PointManager pm = new PointManager();
    void handle(object o)
    {
        try
        {
            this.sc.DeviceManager.PushData(o);
        }
        catch(System.Exception e)
        {

        }
        if (o is carh_info)
        {
            var data = o as carh_info;
            if (data.BankID == "1")
            {
                clift1.MoveToLevel(int.Parse(data.Targetlayer));
            }
            else
            {
                clift2.MoveToLevel(int.Parse(data.Targetlayer));
            }
            
        }
        if (o is PointQueue)
        {

            var data = o as PointQueue;
            saveitem.pushPoint(data.ContainerCode, data.PointCode);
            var pt = ways.FindPoint(data.PointCode);
            if (pt.PassData)
                return;
            pm.PushData(data.PointCode, data.ContainerCode, ways);
          

        }

        //-1.1   -0.62   -0.22  0.22   0.63  1.03   1.45  1.87


        if (o is rgv_realtime_info)
        {
            var data = o as rgv_realtime_info;
            //data.FaultCode = "88";
            saveitem.pushPoint("rgv" + data.BankID + " " + data.id, string.Format("{0}-{1}-{2}:{3}-box_{4}   ",
                data.BankID, data.floor, data.TargetPosition, data.Position, data.HasBox
                ));
            int id = 1;// int.Parse(data.BankID+" "+data.id);
            //return;
            int bk = int.Parse(data.BankID);
            foreach (var e in rgvs)
            {
                if (e.xiangdao == bk && e.id == data.id)
                {
                    e.SetData(data);
                }
            }

            return;


        }
        if (o is task_realtime_info)
        {
            var data = o as task_realtime_info;
            sc.PalletManager.Push(data);
            saveitem.push(data);
            var p = sc.PalletManager.FindPallet(data.ContainerCode);
            if (p != null)
                p.SendTask(data);

        }


        if (o is interfaceline_realtime_info)
        {
            var dd = o as interfaceline_realtime_info;
            string s1 = dd.FirstPosotion == "2" ? dd.FirstPosotionTask : null;
            string s2 = dd.SecondPosotion == "2" ? dd.SecondPosotionTask : null;
            if (dd.Layer == "2")
            {
                string s = string.Format("i-{0}-2-2", dd.BankID == "1" ? "2" : "1");
                var p = ways.FindPoint(s);
                p.LastHasBox = s2 == "2";
            }
            string ps1 = string.Format("i-{0}-{1}-1", dd.BankID == "1" ? "2" : "1", dd.Layer);
            string ps2 = string.Format("i-{0}-{1}-2", dd.BankID == "1" ? "2" : "1", dd.Layer);
            if (s1 != null)
            {



                string s = string.Format("iii-{0}-{1}-1   ", dd.BankID, dd.Layer);
                saveitem.pushPoint(s1, s);
             
               // pm.PushData(ps1, s1, ways);
            }
            else
            {
                pm.Nothing(ps1);
            }
            if (s2 != null)
            {

                string s = string.Format("iii-{0}-{1}-2   ", dd.BankID, dd.Layer);
                saveitem.pushPoint(s2, s);
          
                //pm.PushData(ps2, s2, ways);
            }
            else
            {
                pm.Nothing(ps2);
            }

        }
        if (o is hoist_realtime_info)
        {
            var dd = o as hoist_realtime_info;

            string s1 = dd.FirstPosotion == "2" ? dd.FirstPosotionTask : null;
            string s2 = dd.SecondPosotion == "2" ? dd.SecondPosotionTask : null;
            if (dd.BankID == "1")
                Lift1.SendData(dd);
            else
                Lift2.SendData(dd);
            int bk = dd.BankID == "1" ? 3 : 2;
            var pname = string.Format("h-{0}-1", bk);
            var pname1 = string.Format("h-{0}-1", bk);
            var pname2 = string.Format("h-{0}-2", bk);
            saveitem.pushPoint("[" + pname + "]", string.Format("{0}-{1}@{3}:{2}@{4}", dd.Level, dd.FirstPosotionTask, dd.SecondPosotionTask, dd.FirstPosotion, dd.SecondPosotion));
            if (s1 != null)
            {
                string s = string.Format("hhh-{0}-{1}-1:{2}  ", dd.BankID, dd.Level, dd.Status);
             
                saveitem.pushPoint(s1, pname1 + "@" + dd.Level);
              //  pm.PushData(pname1, s1, ways);
            }
            else
            {
                pm.Nothing(pname1);
            }
            if (s2 != null)
            {

                string s = string.Format("hhh-{0}-{1}-2:{2}   ", dd.BankID, dd.Level, dd.Status);
      
                saveitem.pushPoint(s2, pname2 + "@" + dd.Level);
                //pm.PushData(pname2, s2, ways);
            }
            else
            {
                pm.Nothing(pname2);
            }
        }



        if (o is temporaryarea_realtime_info)
        {
            var dd = o as temporaryarea_realtime_info;
            foreach (var e in this.temporaryareas)
            {
                if (e.PushData(dd))
                {
                    break;
                }
            }
            string s1 = dd.FirstPosotion == "2" ? dd.FirstPosotionTask : null;
            string s2 = dd.FirstPosotion == "2" ? dd.FirstPosotionTask : null;
            var bk = dd.BankID == "1" ? "2" : "1";
            var pname1 = string.Format("t-{0}-{1}-1", bk, dd.Layer);
            var pp1 = ways.FindPoint(pname1);
            var pname2 = string.Format("t-{0}-{1}-2", bk, dd.Layer);
            var pp2 = ways.FindPoint(pname2);

            if (s1 != null)
            {
                pm.PushData(pname1, s1, ways);
                saveitem.pushPoint(s1, "#" + pname1 + "@" + dd.Direction);
            }
            else
            {
                pm.Nothing(pname1);
            }
            if (s2 != null)
            {
                pm.PushData(pname2, s2, ways);
                saveitem.pushPoint(s2, "#" + pname2 + "@" + dd.Direction);
            }
            else
            {
                pm.Nothing(pname2);
            }




        }
    }
    void _handle(object o)
    {
        this.sc.DeviceManager.PushData(o);
        if(o is carh_info)
        {
            var data = o as carh_info;
            if (data.BankID == "1")
            {
                clift1.MoveToLevel(int.Parse(data.Targetlayer));
            }
            else
            {
                clift2.MoveToLevel(int.Parse(data.Targetlayer));
            }
        }
        if (o is PointQueue)
        {

            var data = o as PointQueue;
            saveitem.pushPoint(data.ContainerCode,data.PointCode);
            var pt = ways.FindPoint(data.PointCode);
            if (pt.PassData)
                return;
            if (pt.LastSendPallet == data.ContainerCode)
            {
                return;
            }
            pt.LastSendPallet = data.ContainerCode;
            var p = sc.PalletManager.FindPallet(data.ContainerCode);
            if (p == null)
            {
                p = sc.PalletManager.NewPallet(data.ContainerCode);
            }
            p.SendPoint(pt);
           
        }

        //-1.1   -0.62   -0.22  0.22   0.63  1.03   1.45  1.87


        if (o is rgv_realtime_info)
        {
            var data = o as rgv_realtime_info;
            //data.FaultCode = "88";
            saveitem.pushPoint("rgv" +data.BankID+" "+ data.id, string.Format("{0}-{1}-{2}:{3}-box_{4}   ",
                data.BankID, data.floor, data.TargetPosition, data.Position, data.HasBox
                ));
            int id = 1;// int.Parse(data.BankID+" "+data.id);
            //return;
            int bk = int.Parse(data.BankID);
            foreach(var e in rgvs)
            {
                 if(e.xiangdao==bk && e.id == data.id)
                {
                    e.SetData(data);
                }
            }
            
            return;
           

        }
        if (o is task_realtime_info)
        {
            var data = o as task_realtime_info;
            sc.PalletManager.Push(data);
            saveitem.push(data);
            var p = sc.PalletManager.FindPallet(data.ContainerCode);
            if (p != null)
                p.SendTask(data);
     
        }


        if (o is interfaceline_realtime_info)
        {
            var dd = o as interfaceline_realtime_info;
            string s1 = dd.FirstPosotion=="2" ? dd.FirstPosotionTask : null;
            string s2 = dd.SecondPosotion=="2"?dd.SecondPosotionTask:null;
            if (dd.Layer == "2")
            {
                string s = string.Format("i-{0}-2-2", dd.BankID == "1" ? "2" : "1");
                var p= ways.FindPoint(s);
                p.LastHasBox =  s2 == "2";
            }
            
            if (s1 != null)
            {

            

                string s = string.Format("iii-{0}-{1}-1   ", dd.BankID, dd.Layer);
                saveitem.pushPoint(s1,s);
                string ps1 = string.Format("i-{0}-2-2", dd.BankID == "1" ? "2" : "1");
                var p = sc.PalletManager.FindPallet(s1);
                if (p != null)
                {
                    var pp = ways.FindPoint(ps1);
                    if (pp != null)
                    {
                        p.SendPoint(pp);
                    }
                }
            }
            if (s2 != null)
            {
           
                string s = string.Format("iii-{0}-{1}-2   ", dd.BankID, dd.Layer);
                saveitem.pushPoint(s2, s);
                string ps1 = string.Format("i-{0}-2-2", dd.BankID == "1" ? "2" : "1");
                var p = sc.PalletManager.FindPallet(s1);
                if (p != null)
                {
                    var pp = ways.FindPoint(ps1);
                    if (pp != null)
                    {
                        p.SendPoint(pp);
                    }
                }
            }
            
        }
        if (o is hoist_realtime_info)
        {
            var dd = o as hoist_realtime_info;

            string s1 =dd.FirstPosotion=="2"?  dd.FirstPosotionTask:null;
            string s2 =dd.SecondPosotion=="2"? dd.SecondPosotionTask:null;
            if (dd.BankID == "1")
                Lift1.SendData(dd);
            else
                Lift2.SendData(dd);
            int bk = dd.BankID == "1" ? 2 : 1;
            var pname = string.Format("h-{0}-1", bk);
            saveitem.pushPoint("[" + pname + "]", string.Format("{0}-{1}@{3}:{2}@{4}", dd.Level, dd.FirstPosotionTask, dd.SecondPosotionTask,dd.FirstPosotion,dd.SecondPosotion));
            if (s1 != null)
            {
                string s = string.Format("hhh-{0}-{1}-1:{2}  ", dd.BankID, dd.Level, dd.Status);
                var pname1 = string.Format("h-{0}-1", bk);
                saveitem.pushPoint(s1, pname1+"@"+dd.Level);
                var p = sc.PalletManager.FindPallet(s1);
                if (p != null)
                {
                    p.SendPoint(dd.BankID == "1" ? Lift1.p1 : Lift2.p1);
                }
               
            }
            if (s2 != null)
            {
              
                string s = string.Format("hhh-{0}-{1}-2:{2}   ", dd.BankID, dd.Level, dd.Status);
                var pname2 = string.Format("h-{0}-2", bk);
                saveitem.pushPoint(s2, pname2 + "@" + dd.Level);
                var p = sc.PalletManager.FindPallet(s2);
                if (p != null)
                {
                    p.SendPoint(dd.BankID == "1" ? Lift1.p1 : Lift2.p1);
                }
            }
        }



        if (o is temporaryarea_realtime_info)
        {
            var dd = o as temporaryarea_realtime_info;
            foreach(var e in this.temporaryareas)
            {
                if (e.PushData(dd))
                {
                    break;
                }
            }
            string s1 =dd.FirstPosotion=="2"? dd.FirstPosotionTask:null;
            string s2 = dd.FirstPosotion == "2" ? dd.FirstPosotionTask : null;
            var bk = dd.BankID == "1" ? "2" : "1";
            var pname1 = string.Format("t-{0}-{1}-1", bk, dd.Layer);
            var pp1 = ways.FindPoint(pname1);
            var pname2 = string.Format("t-{0}-{1}-2", bk, dd.Layer);
            var pp2 = ways.FindPoint(pname2);

            if (s1 != null)
            {
                var pt1 = sc.PalletManager.FindPallet(s1);
                if (pt1 == null)
                {
                  //  pt1 = sc.PalletManager.NewPallet(s1);
                  //  pt1.SetPoint(pp1);
                  
                }
                if (pt1 != null)
                {
                    pt1.SendPoint(pp1);
                }
                saveitem.pushPoint(s1, "#" + pname1+"@"+dd.Direction);
            }
            if (s2 != null)
            {
                var pt2 = sc.PalletManager.FindPallet(s2);
                if (pt2 != null)
                {
                    pt2.SendPoint(pp2);
                    // pt2 = sc.PalletManager.NewPallet(s2);
                    // pt2.SetPoint(pp2);
                }
                saveitem.pushPoint(s2, "#" + pname2 + "@" + dd.Direction);
            }




        }
    }

    Dictionary<string, string> dics = new Dictionary<string, string>();

    bool IsNewPoint(string point,string code)
    {
        if (dics.ContainsKey(point))
        {
            var a = dics[point];
            if (a == code)
                return false;
            dics[point] = code;
            return true;
        }
        else
        {
            dics.Add(point, code);
            return true;
        }
    }




    void initDev()
    {
        string s1 = @"货物提升机由立柱结构和安装辊筒输送机的托架组成，由伺服电机通过皮带传动升、降托架。主要对应立体
仓库存储商品的提升下降，保证商品在立体仓库和输送线间的衔接。";
        string s2 = @"高速换层提升机由立柱结构和小车对接托架组成，由伺服电机通过皮带传动升、降托架。主要对应立体仓库
存储小车的换层需求做出提升下降，保证小车在不同层的高速切换，提升换层效率以及降低立库小车使用数量。";
        string s3 = @"有轨式自动穿梭车 RGV,通过编程实现箱式货物在穿梭式货架中的存取、运输、放置等的搬运小车，可与上位
软件系统进行通讯及调度完成任务。穿梭车在立库中高速运动，搬运货品从起始位到目标位，实现对货品的存取
和搬运功能。";

        sc.DeviceManager.AddDevice(new Device("货物提升机", "货物提升机-1",s1, "1","1",this.LiftOutLine1));
        sc.DeviceManager.AddDevice(new Device("货物提升机", "货物提升机-2", s1, "1","2",this.LiftOutLine2));
        sc.DeviceManager.AddDevice(new Device("换层提升机", "换层提升机-1", s2, "2","1",this.CLiftOutLine1)); 
        sc.DeviceManager.AddDevice(new Device("换层提升机", "换层提升机-2", s2, "2","2",this.CLiftOutLine2));
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 1-1", s3, "3","1-1",this.CarOutLine1));
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 1-2", s3, "3", "1-2", this.CarOutLine2));
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 1-3", s3, "3","1-3", this.CarOutLine3));
  
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 2-1", s3, "3","2-1", this.CarOutLine4));
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 2-2", s3, "3","2-2", this.CarOutLine5));
        sc.DeviceManager.AddDevice(new Device("RGV", "RGV 2-3", s3, "3","2-3", this.CarOutLine6));
      
        sc.DeviceManager.Init();
    }


    void Work(string pallet, string point, float speed)
    {
        if (pallet == null)
            return;
        Item item = null;
        foreach (var e in this.items)
        {
            if (e.Code == point)
            {
                item = e;
                break;
            }
        }
        if (item == null)
            return;
        var pp = sc.PalletManager.FindPallet(pallet);
        if (pp == null  && pallet!="")
        {
            pp = sc.PalletManager.NewPallet(pallet);
            //pp.PushPoint(item.point, 0.01f);
            item.pallet = pp;
            return;
        }
        else
        {
            if (pallet != "")
            {
               // pp.PushPoint(item.point, 1);
                item.pallet = pp;
            }
        }
    }
    bool hist = false;
    Redis.RedisObj redis;
    public string path;
    void Start()
    {
        Time.timeScale = 1.5f;
        this.sc = G.GetSceneScript();
        this.ways = GameObject.Find("Main").GetComponent<WayView>();
        if (History)
        {
            history.Create(handle);
            hist = true;
        }
        else
        {
            redis = new Redis.RedisObj();
            
            redis.Open(@"D:\config\data\");

        }
        initDev();
      
    }
    T[] ToJson<T>(string data)
    {
        var dd=  JArray.Parse(data);
        T[] tt = new T[dd.Count];
        for(int i=0;i<dd.Count;i++)
        {
            tt[i] = ((dd[i]) as JObject).ToObject<T>();
        }
        return tt;
    }
    void Update()
    {
        this.sc.PalletManager.Loop();
        if (first == false)
        {
            first = true;
            this.ways.CreateNet();
            this.create_cross(ways);
            for(int i = 1; i <= 2; i++)
            {
                for(int j = 1; j <= 8; j++)
                {
                    temporaryarea t = new temporaryarea(this.ways, i, j);
                    this.temporaryareas.Add(t);
                }
            }
        }
  
         foreach(var e in this.temporaryareas)
        {
            e.Loop();
        }


        if (Save)
        {
            Save = false;
            saveitem.Save();
            StringBuilder sb = new StringBuilder();
            foreach(var e in this.rgvs)
            {
                e.save(sb);
            }
            System.IO.File.WriteAllText(@"D:\上海1.txt", sb.ToString());
        }
        /*
        if (first == false)
        {
            first = true;
            this.ways.CreateNet();
            List<Item> items = new List<Item>();
            foreach (var e in this.ways.Points)
            {
                items.Add(new Item() { Code = e.name, point = e });
            }
            this.items = items.ToArray();
        }
        */
        if (hist)
        {
            history.loop();
        }
        else
        {
            var k = G.DataBox.GetMessage();
            if (k == null)
                return;
            switch (k.Type)
            {
                case "layerhoist_realtime_info":
                    foreach (var e in ToJson<carh_info>(k.content as string))
                    {
                        try
                        {
                            handle(e);
                        }
                        catch(System.Exception er)
                        {

                        }
                    }
                    return;

                case "PointQueue":
                    foreach(var e in ToJson<PointQueue>(k.content as string))
                    {
                        handle(e);
                    }
                    break;
                case "interfaceline_realtime_info":
                    foreach (var e in ToJson<interfaceline_realtime_info>(k.content as string))
                    {
                        handle(e);
                    }
                    break;
                case "hoist_realtime_info":
                    foreach (var e in ToJson<hoist_realtime_info>(k.content as string))
                    {
                        handle(e);
                    }
                    break;
                case "rgv_realtime_info":
                    foreach (var e in ToJson<rgv_realtime_info>(k.content as string))
                    {
                        handle(e);
                    }
                    break;
                case "temporaryarea_realtime_info":
                 //   handle(JObject.Parse(k.content as string).ToObject<temporaryarea_realtime_info>());
                    break;
                case "task_realtime_info":
                    foreach (var e in ToJson<task_realtime_info>(k.content as string))
                    {
                        handle(e);
                    }
                    break;
                default:
                    break;
            }
           // handle(k.content);
        }
    }



    class PointManager
    {
        List<Point> ps = new List<Point>();

        public void Nothing(string ps)
        {

        }

        PalletManager pm = null;
        public void PushData(string pos, string code, WayView wayv)
        {
            if (pm == null) {
                pm = G.GetSceneScript().PalletManager;
            }
            foreach (var e in ps)
            {
                if (e.Pos == pos)
                {
                    var c = e.Content;
                    if (e.SetPoint(code))
                    {
                        if (code != null && pos[0]=='0')
                        {
                           var pp = pm.FindPallet(code);
                            if (pp == null)
                                pp = pm.NewPallet(code);
                            pp.SendPoint(e.point);
                           // PalletEx.PushPoint(code, e.point);
                        }
                    }
                    return;
                }
            }
            var dd = wayv.FindPoint(pos);
            if (dd != null)
            {
                Point p = new Point(pos, dd);
                this.ps.Add(p);
                var e = p;
                var c = e.Content;
                if (e.SetPoint(code))
                {
                    if (code != null  && code[0] == '0')
                    {
                        var pp = pm.FindPallet(code);
                        if (pp == null)
                            pp = pm.NewPallet(code);
                        pp.SendPoint(e.point);
                        // PalletEx.PushPoint(code, e.point);
                    }
                }
        
            }
        }
    }


    class Point
    {
        public string Pos;
        public string Content;
        public WayPoint point;

        public bool SetPoint(string Content)
        {
            if (Content != this.Content)
            {
                this.Content = Content;
                return true;
            }
            return false;
        }

        public Point(string pos, WayPoint p)
        {
            this.Pos = pos;
            this.point = p;
        }
    }
}
