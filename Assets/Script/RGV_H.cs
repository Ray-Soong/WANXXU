using Assets.Script.Datas;
using Assets.Script.Motion;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script
{



    //孔位型RGV标准脚本 要求主
   public  class RGV_H : MonoBehaviour
    {
        WayPoint table;
        WayPoint[] ps;
        

        public float UseTimePerHole;

        public Clift lift;
        public WayPoint wp;
        SceneScript sc;
        public string cellname;

        public string id;
        Pallet pallet;
        //没个实体轴的移动轴编号
        public AxisName MoveAxis, Fork1Axis, Fork2Axis;
        //在模型中主体名称
        public string BodyName = "Body";
        //模型中的一段货叉名
        public string Fork1Name = "Table";
        //模型中的二段货叉名（没有不填）
        public string Fork2Name = null;
        //点位对象名称
        public string PointName = "Table";

        public float OneHoleUseTime;

        public int xiangdao;

        int floor;
        //库位相关功能
        IStore store;
        Transform car;
        Transform fork1;
        Transform fork2;
     
        AxisController MoveController = new AxisController();
        AxisController ForkController = new AxisController();
        int last_hole;

        public void MoveToHole(int hole)
        {
            

            if (hole > 200)
            {
                this.target_hole = -1;
                this.step = 0;
            }
            else
            {
                this.target_hole = hole;
                this.step = 0;
            }
        }


        //升降梯点位
        DevicePoint LiftPoint;
        private void Start()
        {
            this.sc = G.GetSceneScript();
            car = G.FindChild(this.transform,BodyName);
            fork1 = G.FindChild(this.transform,Fork1Name);
            table = G.FindChild(this.transform, PointName).GetComponent<WayPoint>();
            table.IsTable = true;
            this.store = Store.store;

        }

        int last_floor = -1;
        bool floor_change = false;
        //设置巷道位置

        int athole = 0;
        public void SetXiangdao(int number)
        {
            this.xiangdao = number;
            float xd = store.Getxiangdao(number);
            var t = this.transform.position;
            if (this.MoveAxis == AxisName.X)
            {
                t.z = xd;
            }
            else
            {
                t.x = xd;
            }
            this.transform.position = t;
        }
        //设置楼层位置
        public void SetFloor(int number)
        {
            if (number != last_floor)
            {
                last_floor = number;
                floor_change = true;
            }
        }

        public enum tasktype
        {
            None,
            Get,
            Push
            
        }


      

        

        


     


        //设置所在空位
        public void SetHole(int number)
        {
      
            float xd = store.GetHole(this.xiangdao,this.floor, number);
            var t = this.transform.position;
            if (this.MoveAxis != AxisName.X)
            {
                t.z = xd;
            }
            else
            {
                t.x = xd;
            }
            this.transform.position = t;
        }

        public string pos;


        task_realtime_info lasttask;
        string newtask;




        public void SetData(rgv_realtime_info rgv)
        {

            this.lastdata = rgv;
            this.data_level = int.Parse(rgv.floor);
            this.data_pos = int.Parse(rgv.Position);
            this.data_source = int.Parse(rgv.SourcePosition);
            this.data_target = int.Parse(rgv.TargetPosition);
            
          //  TaskProc();
            this.source_cell = rgv.SourcePosition;
            this.target_cell = rgv.TargetPosition;
            this.pos = rgv.Position;
            this.SetFloor(int.Parse(rgv.floor));
            //this.proc(rgv);
            this.MoveToHole(int.Parse(rgv.Position));
            this.SetXiangdao(int.Parse(rgv.BankID));
          //  action();
            
        }
        public string target_cell;
        public string source_cell;
        int target_hole;
        float target;
        float curr;
        int curr_hole = -11;
        rgv_realtime_info lastdata;
        int step;
        float speed;

        bool inputing;


 

        rgv_status ToRGVStatus(string status)
        {
            return (rgv_status) int.Parse(status);
        }

        Pallet pellet = null;




        void PushPallet(Pallet p,int pos)
        {
            if (pos == 0)
            {
                p.transform.parent = null;
                p.SendPoint(ps[data_level-1]);
                
            }
            else
            {
                p.transform.parent = null;
              
                var z = store.GetHole(this.xiangdao, int.Parse(this.lastdata.floor), pos % 100);
                var y = store.GetFloor(this.xiangdao, int.Parse(this.lastdata.floor));
                var x = store.GetRow(this.xiangdao, int.Parse(this.lastdata.floor), pos / 1000);
                p.MoveCell(pos, new Vector3(x, y, z));
               

            }
        }

        Pallet GetPallet(int pos)
        {
            var p = sc.PalletManager.FindPallet(this.lastdata.TaskID);
            if (p == null)
            {

                p = sc.PalletManager.NewPallet(this.lastdata.TaskID);
                p.SendTask(this.lasttask);
            }

            if (pos == 0)
            {
                
                p.SetPoint(ps[data_level-1]);
                return p;
            }
            else
            {
                // var pp = table.transform.position;
                var z = store.GetHole(this.xiangdao, int.Parse(this.lastdata.floor), pos % 100);
                var y = store.GetFloor(this.xiangdao, int.Parse(this.lastdata.floor));
                var x = store.GetRow(this.xiangdao, int.Parse(this.lastdata.floor), pos / 1000);
                p.transform.position = new Vector3(x, y, z);
                return p;
            }
        }


        int data_pos;
        int data_level;
        int data_source;
        int data_target;
        string taskid;

        int actionstep = 0;
       


        

        public rgv_status last_rgv_status;


        int action_step = 0;
        float action_target;
        float action_speed = 0;
        int action_hole = 0;
        int action_cellnumber = 0;
        Vector3 target_cellpos;
    
        class records
        {
            public string tasktype;
            public string taskid;
            public string pos;
            public string status;
            public string source;
            public string target;
            public float times;
        }

        List<records> recordss = new List<records>();
        records last_record=null;

        class iotask
        {
            public string taskid;
            public int step;
            public string type;
            public int target;
            public int source;
            public int cell;
        }

        iotask tsk;

        void closetask()
        {
            if (this.table.transform.childCount > 1)
            {
                for(int i = 0; i < this.table.transform.childCount;i++)
                {
                    var p = this.table.transform.GetChild(i);
                    if (p.GetComponent<Pallet>() != null)
                    {
                        p.parent = null;
                        G.GetSceneScript().PalletManager.Delete(p.GetComponent<Pallet>());
                    }

                }
            }
            this.pallet = null;
            this.tsk = null;
            if (this.pallet != null)
            {
                if (this.pallet.transform.parent == this.table)
                {
                    this.pallet.transform.parent = null;
                    G.GetSceneScript().PalletManager.Delete(this.pallet);
                }
                tsk = null;
            }
        }

        void newaction()
        {
            /*
            if (this.pallet != null)
            {
                this.pallet.removed = true;
                this.tsk = null;
                return;
            }
            */
            bool atpos = this.curr_hole == target_hole;
            if (this.lastdata != null)
            {
                var t = this.lastdata.TaskID;
                var p = sc.PalletManager.GetTask(t);
                if (p != null)
                {
                    
                    if (tsk != null)
                    {
                        if (tsk.taskid != t)
                        {
                            closetask();
                            tsk = new iotask()
                            {
                                type = p.data.TaskType,
                                taskid = t,
                                step = 0
                            };
                        }
                        this.lasttask = p.data;
                    }
                    else
                    {
                        tsk = new iotask()
                        {
                            type = p.data.TaskType,
                            taskid = t,
                            step = 0
                        };
                        this.lasttask = p.data;
                    }
                }
                if (tsk != null)
                {
                    switch (tsk.step)
                    {
                        case 0:
                            if (tsk.type == "1")
                            {
                             
                                if (this.lastdata.TargetPosition != "0" )
                                {
                                    tsk.target = int.Parse(this.lastdata.TargetPosition);
                                    tsk.step = 100;
                                }
                            }
                            if (tsk.type == "2")
                            {
                                if (this.lastdata.SourcePosition != "0")
                                {
                                    tsk.source = int.Parse(this.lastdata.SourcePosition);
                                    tsk.step = 200;
                                }
                            }
                            if (tsk.type == "3")
                            {
                                if (this.lastdata.TargetPosition != "0" && this.lastdata.SourcePosition != "0")
                                {
                                    tsk.source = int.Parse(this.lastdata.SourcePosition);
                                    tsk.target = int.Parse(this.lastdata.TargetPosition);
                                    tsk.step = 300;
                                }
                            }
                            break;
                        case 100:

                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == 0 && step==0 &&  atpos)
                                {
                                    var pl = GetPallet(tsk.target);
                                    this.pallet = pl;
                                    pl.SendPoint(this.table);
                                    tsk.step = 101;
                                }
                            }
                            break;
                        case 101:
                           
                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == tsk.target%1000 && step==0 && atpos)
                                {
                                    PushPallet(this.pallet, tsk.target);
                                    if (this.pallet.transform.parent == table)
                                    {
                                        this.pallet.transform.parent = null;
                                        this.pallet = null;
                                    }
                                    
                                    tsk.step = 102;
                                }
                            }
                            break;
                        case 200:
                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == tsk.source%1000  && atpos)
                                {
                                    var pl = GetPallet(tsk.source);
                                    this.pallet = pl;
                                    pl.SendPoint(this.table);
                                    tsk.step = 201;
                                }
                            }
                            break;
                        case 201:
                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == 0 && step==0 && atpos)
                                {
                                    var lv = int.Parse(this.lasttask.Layer) - 1;
                                    var pp = ps[lv];
                                    this.pallet.SendPoint(ps[lv]);
                                    if (this.pallet.transform.parent == table)
                                    {
                                        this.pallet.transform.parent = null;
                                        this.pallet = null;
                                    }
                                    //var pl = GetPallet(0);
                                    tsk.step = 202;
                                }
                            }
                            break;
                       case 300:
                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == tsk.source % 1000 && step==0 && atpos)
                                {
                                    var pl = GetPallet(tsk.source);
                                    this.pallet = pl;
                                    pl.SendPoint(this.table);
                                    tsk.step = 301;
                                }
                            }
                            break;
                        case 301:
                            if (this.lastdata.floor == this.lasttask.Layer)
                            {
                                int pos = int.Parse(this.lastdata.Position);
                                if (pos == tsk.target % 1000 && step==0 && atpos)
                                {
                                    PushPallet(this.pallet, tsk.target);
                                    if (this.pallet.transform.parent == table)
                                    {
                                        this.pallet = null;
                                        this.pallet.transform.parent = null;
                                    }
                                  
                                    tsk.step = 302;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        public void save(StringBuilder sb)
        {
            sb.AppendLine(string.Format("RGV_{0}-{1}", this.xiangdao, this.id));
            foreach(var e in recordss)
            {
                sb.AppendLine(string.Format("{5}\t{6}:{0}-p{1}-t{2}-s{3}-{4}  ", e.taskid, e.pos, e.target, e.source, e.status,e.times.ToString("0.00"),e.tasktype));
            }
            sb.AppendLine("");
        }

        void TaskProc()
        {

            if (this.lastdata != null)
            {
                var pp = sc.PalletManager.GetTask(this.lastdata.TaskID);
                if (pp != null)
                    this.lasttask = pp.data;
            }
            else
                return;
            /*
            if (last_record == null)
            {
                var r = new records()
                {
                    tasktype = this.lasttask!=null? this.lasttask.TaskType:"null",
                    pos = this.lastdata.Position,
                    source = this.lastdata.SourcePosition,
                    target = this.lastdata.TargetPosition,
                    taskid = this.lastdata.TaskID,
                    status = this.lasttask == null ? "null" : ToRGVStatus(this.lasttask.TaskRGVStatus).ToString()
                    , times = Time.time
                };
                this.recordss.Add(r);
                this.last_record = r;
            }
            else
            {
                var a1 = this.lasttask != null ? this.lasttask.TaskType : "null";
                var a2 = this.lasttask == null ? "null" : ToRGVStatus(this.lasttask.TaskRGVStatus).ToString();
                if (last_record.pos != this.lastdata.Position || last_record.source != this.lastdata.SourcePosition
                    || last_record.target != this.lastdata.TargetPosition
                    || last_record.taskid != this.lastdata.TaskID
                    || last_record.status != a2

                    || last_record.tasktype != a1
                    )
                {
                    var r = new records()
                    {
                        tasktype = this.lasttask != null ? this.lasttask.TaskType : "null",
                      
                        pos = this.lastdata.Position,
                        source = this.lastdata.SourcePosition,
                        target = this.lastdata.TargetPosition,
                        taskid = this.lastdata.TaskID,
                        status = this.lasttask == null ? "null" : ToRGVStatus(this.lasttask.TaskRGVStatus).ToString()
                     ,times = Time.time
                    };
                    this.recordss.Add(r);
                    this.last_record = r;
                }
            }
            */
            if (this.lasttask!=null)
            {
                newaction();// Action(ToRGVStatus(this.lasttask.TaskRGVStatus));
                /*
                var st = this.last_rgv_status= ToRGVStatus(this.lasttask.TaskRGVStatus);
                switch (st)
                {
                    case rgv_status.None:
                        break;
                    case rgv_status.EmptyBox:
                        break;
                    case rgv_status.FromStore:
                        if (pallet == null && actionstep==0)
                        {
                            var p = GetPallet(int.Parse(this.lastdata.SourcePosition));
                            pallet = p;
                            actionstep = 1;
                        }
                        break;
                    case rgv_status.FromTemp:
                        if (pallet == null && actionstep==0)
                        {
                            var p = GetPallet(int.Parse(this.lastdata.SourcePosition));
                            pallet = p;
                            actionstep = 11;
                        }
                        break;
                    case rgv_status.ToStore:
                        if (pallet != null)
                        {

                            PushPallet(this.pallet, int.Parse(this.lastdata.TargetPosition));
                            this.pallet = null;
                        }


                        break;
                    case rgv_status.ToTemp:
                        if (pallet != null)
                        {

                            PushPallet(this.pallet, 0);
                            this.pallet = null;
                        }


                        break;
                    case rgv_status.ForceComplate:
                        if (this.pallet != null)
                        {
                            //尝试删除
                            this.pallet = null;
                        }
                        break;
                    default:
                        break;
                }
                */
            }



        }


        private void FixedUpdate()
        {

        //   action();
        }

        void AnimiationProc()
        {
            switch (step)
            {
                case 0:

                    if (this.curr_hole != target_hole)
                    {
                        if (target_hole == -1)
                        {
                            this.target = 1.73f;
                            this.speed = Mathf.Abs(target - curr) / G.CarSpeed;
                            this.step = 11;
                        }
                        else
                        {
                            this.target = store.GetHole(this.xiangdao, this.floor, this.target_hole % 100);

                            this.speed = Mathf.Abs(target - curr) / G.CarSpeed;
                            this.step = 1;
                        }
                    }
                    break;
                case 1:
                case 11:
                    this.transform.parent = null;
                    var p = new Vector3(0, 0, this.curr);
                    var t = new Vector3(0, 0, target);
                    var k = Vector3.MoveTowards(p, t, this.speed * Time.deltaTime);
                    this.curr = k.z;
                    var pr = this.transform.position;
                    pr.z = this.curr;
                    this.transform.position = pr;
                    if (k == t)
                    {
                        if (step == 11)
                        {
                            this.transform.parent = this.wp.transform;
                        }
                        this.curr_hole = target_hole;
                        step = 0;
                    }
                    break;




                default:
                    break;
            }
        }
        Clift lft;
        bool hanceng = false;

        Timer tm = new Timer();
        bool nochangefloor;

        private void Update()
        {
            if (this.pallet != null)
            {
                if (this.pallet.removed)
                {
                    this.pallet = null;
                    this.tsk = null;
                }
            }
           
            if (ps == null)
            {
                ps = new WayPoint[8];
                for(int i = 0; i < 8; i++)
                {
                    string ss = string.Format("t-{0}-{1}-2", xiangdao == 1 ? "2" : "1", i+1);
                    var g = GameObject.Find("Main");
                    var w = g.GetComponent<WayView>();

                    ps[i] = w.FindPoint(ss);

                }
            }
            if (tm.Invoke(nochangefloor, 20000))
            {
                nochangefloor = false;
            }

            if (floor_change && nochangefloor==false)
            {
                floor_change = false;
                float xd = store.GetFloor(this.xiangdao, last_floor);
                var t = this.transform.position;
                t.y = xd;
                this.transform.position = t;
            }

            TaskProc();
            switch (step)
            {
                case 0:
                    
                    if (this.curr_hole != target_hole)
                    {
                        if (target_hole == -1)
                        {
                            this.target = 1.73f;
                            this.speed = Mathf.Abs(target - curr) / G.CarSpeed;
                         
                            this.step = 11;
                        }
                        else
                        {
                        
                            this.target = store.GetHole(this.xiangdao, this.floor, this.target_hole % 100);

                            this.speed = Mathf.Abs(target - curr) / G.CarSpeed;
                            this.step = 1;
                        }
                    }
                    break;
                case 1:
                case 11:
                    this.transform.parent = null;
                    var p = new Vector3(0, 0, this.curr);
                    var t = new Vector3(0, 0, target);
                    var k=  Vector3.MoveTowards(p, t, this.speed*Time.deltaTime);
                    this.curr = k.z;
                    var pr = this.transform.position;
                    pr.z = this.curr;
                    this.transform.position = pr;
                    if (k == t)
                    {
                        if (step == 11)
                        {
                            this.curr_hole = target_hole;
                            nochangefloor = true;
                            this.transform.parent = this.wp.transform;
                            this.transform.localPosition = new Vector3(0, 0, 0);
                            this.step = 0;
                        }
                        else
                        {
                            
                            this.curr_hole = target_hole;
                            step = 0;
                        }
                    }
                    break;
                case 21:
                    
                case 20:
                    if (target_hole > 0) 
                    {
                        this.nochangefloor = true;
                        step = 0;
                    }
                    else {
                        if (this.lasttask != null)
                        {
                            if (lift.CurrentLevel.ToString() == this.lasttask.Layer)
                            {
                                this.nochangefloor = true;
                                step = 0;
                            }
                        }
                        break;
                    }
                    break;
       
                   

                default:
                    break;
            }


            if (this.lastdata != null)
            {
                if (this.lastdata.Position == "0")
                {
                    var lv = int.Parse(this.lastdata.floor);
                    WayPoint pp = null;
                    if (lv >= 1 && lv <= 8)
                    {
                        pp = ps[lv - 1];
                      
                    }

                    if (pp.CurrentPallet != null)
                    {
                        if (pp.CurrentPallet.isinput)
                        {
                            pp.CurrentPallet.SendPoint(table);
                            
                        }
                        return;
                    }
                    

                    if (this.table.CurrentPallet != null)
                    {
                        if(this.table.CurrentPallet.TaskType== TaskType.Output)
                        {
                            var p = this.table.CurrentPallet;
                            p.isoutput = true;
                            p.isinput = false;
                            this.table.CurrentPallet.SendPoint(pp);
                        }
                    }
               

                    
                }
                else
                {
                    if (this.lastdata.SourcePosition != "0")
                    {
                        int k = int.Parse(this.lastdata.SourcePosition);
                        if (k != lastgetpos)
                        {
                            if (k % 100 == this.target_hole)
                            {
                             //   var p = G.GetSceneScript().PalletManager.GetPalletByTask(this.xiangdao, int.Parse(this.lastdata.floor), true);
                               
                            }
                        }
                    }
                }
            }

        }

        int lastgetpos;

        public void MoveToTarget(int curr, int target,string boxname)
        {

        }

        class task
        {
            public bool IsInput;
            public bool IsOutput;
            public int target_level;
            public int target_hole;
            public int target_cell;
            public int step;
            public int hole;
            public Pallet p;
            public int cell;
            public int flag = 0;
            
        }
        bool colsed;
        task ztask = null;
        bool moving = false;
      

    }

    [Serializable]
    public class NumberMapping
    {
        public float Position;
        public int Number;
    }

    public enum rgv_status
    {
        None = 0,
        EmptyBox = 1,
        FromStore = 2,
        FromTemp = 3,
        ToStore = 4,
        ToTemp = 5,
        ForceComplate = 6
    }
}
