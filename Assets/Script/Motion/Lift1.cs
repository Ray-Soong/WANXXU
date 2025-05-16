using Assets.Script.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.Motion
{
    [ExecuteInEditMode]
    public class Lift1 : MonoBehaviour
    {
        //提升机上点位名称,点位必须按进库方向填写
        public string[] PointNames;
        public float FloorOffset = -0.86f + 0.583f;

        public float interface1Floor;
        public float interface2Floor;

        List<floor> floors = new List<floor>();
        float[] ssa = new float[] { -0.05f,  0.39f,   0.81f,   1.23f,   1.66f,  2.08f,   2.51f,  0.93f };
        int getlevel(int level)
        {
            if (level > 100)
                return level - 100;
            else
                return -level;
        }
        public int level;
        public void SendData(hoist_realtime_info info)
        {
       
            this.showlevel = info.Level;
            var f = info.Level;
            if (f =="0")
                return;
            var p = this.transform.position;
            int v = this.getlevel(int.Parse(info.Level));
            this.setLevel(v);
            // p.y = this.getfloor(v);
           // this.transform.position = p;

        }


        class floor
        {
            public WayPoint fpoint;
            public string id;
            public Vector3 Point;
            public WayPoint waypoint;
            public WayPoint wayPoint2;
            public float offset;
            public int level;
            public float ffff;

            public float GetValue()
            {
                return ffff;
                
            }

            public floor(WayPoint point,float offset)
            {
                ffff = point.transform.localPosition.y;//offset;
               var ss= point.name.Split('-');
                this.id = ss[2].Trim();
                this.Point = point.Point;
                
                this.wayPoint2 = point.Outputs[0].Point;
                level = int.Parse(this.id);
                this.offset = offset;
                this.waypoint = point;
                this.wayPoint2.SelectWayHandler = SelectWay2;
                this.waypoint.SelectWayHandler = SelectWay1;
                this.waypoint.AutoPoint = true;
                this.wayPoint2.AutoPoint = true;
            }

            WayPoint SelectWay1(MonoBehaviour pallet, WayPoint From)
            {
                var pp = pallet as Pallet;
                switch (pp.TaskType)
                {
                    case TaskType.None:
                        break;
                    case TaskType.Input:
                        return this.wayPoint2;
                    case TaskType.Output:
                        return fpoint;
                    case TaskType.Move:
                        break;
                    default:
                        break;
                }
                return null;
            }

            WayPoint SelectWay2(MonoBehaviour pallet, WayPoint From)
            {
                var pp = pallet as Pallet;
                switch (pp.TaskType)
                {
                    case TaskType.None:
                        break;
                    case TaskType.Input:
                     //  pp.TaskType = TaskType.Output;
                       break;
                    case TaskType.Output:
                        return this.waypoint;
                    case TaskType.Move:
                        break;
                    default:
                        break;
                }
                return null;
            }


        }

        //IStore store;
        public int Bank;
       public  WayPoint p2;
       public  WayPoint p1;
        int targetlevel;
        float target;
        float curr;

        public float KeepLevelTimes;

        int iostep;
        internal void TempMessage(temporaryarea temp, bool InputLib,WayPoint pp1,WayPoint pp2)
        {
           
            if (this.level == temp.Level)
            {
                if (iostep == 0)
                {
                    if (InputLib)
                    {
                        int c = 0;
                        if (p1.CurrentPallet != null)
                        {
                            c++;
                        }
                        if (p2.CurrentPallet != null)
                        {
                            c++;
                        }
                        if (c == 1)
                        {
                            if (p1.CurrentPallet != null)
                                p1.CurrentPallet.SendPoint(pp1, true);
                            if (p2.CurrentPallet != null)
                                p2.CurrentPallet.SendPoint(pp1, true);
                            iostep = 1;
                        }
                        if (c == 2)
                        {
                            if (p1.CurrentPallet.TaskLevel == level)
                            {
                                p2.CurrentPallet.SendPoint(pp2, true);
                                p1.CurrentPallet.SendPoint(pp1, true);
                            }
                            else
                            {
                                p2.CurrentPallet.SendPoint(pp1, true);
                            }
                            if(c>0)
                            iostep = 1;

                        }
                    }
                    else
                    {
                        var a1 = pp1.CurrentPallet;
                        var a2 = pp2.CurrentPallet;
                        int c = 0;
                        if (a1 != null)
                        {
                            a1.isoutput = true;
                            a1.isinput = false;
                            a1.outputing = true;
                            a1.SendPoint(this.p2, true);
                            c = 1;
                        }
                        if (a2 != null)
                        {
                            a2.isoutput = true;
                            a2.isinput = false;

                            a2.SendPoint(c == 1 ? p1 : this.p2, true);
                        }
                        if(c>0)
                        this.iostep = 1;
                    }
                }
            }
            else
            {
                iostep = 0;
            }
        }




        public SingleAxisTable axis;
        bool input1(MonoBehaviour p, WayPoint from, WayPoint to)
        {
            Pallet pp = p as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:
                    if (level != -2)
                    {
                        this.SetLevel(-2);
                    }
                    return level == -2;
                case TaskType.Output:
                    return true;
                case TaskType.Move:
                    break;
                default:
                    break;
            }
            return false;
        }

        bool input2(MonoBehaviour p, WayPoint from, WayPoint to)
        {
            Pallet pp = p as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:
                    return true;
                case TaskType.Output:
                    if (level != pp.TaskLevel)
                    {
                        this.SetLevel(pp.TaskLevel);
                    }
                    return level == pp.TaskLevel;
                case TaskType.Move:
                    break;
                default:
                    break;
            }
            return false;
        }

        bool output2(MonoBehaviour p, WayPoint from, WayPoint to)
        {
                Pallet pp = p as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:
                    if (level != pp.TaskLevel)
                    {
                        this.SetLevel(pp.TaskLevel);
                    }
                    return level == pp.TaskLevel;
                case TaskType.Output:
                    return true;
                case TaskType.Move:
                    break;
                default:
                    break;
            }
            return false;
        }

       

        bool output1(MonoBehaviour p, WayPoint from, WayPoint to)
        {
            Pallet pp = p as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:
                    return true;
                case TaskType.Output:
                    if (level != -1)
                    {
                        this.SetLevel(-1);
                    }
                    return -1 == this.level;
                case TaskType.Move:
                    break;
                default:
                    break;
            }
            return false;
        }

        WayPoint SelectWay1(MonoBehaviour pallet, WayPoint From)
        {
            Pallet pp = pallet as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:
                    foreach(var e in From.Outputs)
                    {
                        if (e.Point == p2)
                            return e.Point;
                    }
                    break;
                case TaskType.Output:
                    foreach(var e in From.Outputs)
                    {
                        if (e.Point.DeviceType == "output")
                        {
                            return e.Point;
                        }
                    }
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
            Pallet pp = pallet as Pallet;
            switch (pp.TaskType)
            {
                case TaskType.None:
                    break;
                case TaskType.Input:

                    foreach(var e in this.floors)
                    {
                        if (e.level == pp.TaskLevel)
                        {
                            foreach (var r in From.Outputs)
                            {
                                if (r.Point == e.waypoint)
                                    return r.Point;
                            }
                        }
                    }
                    break;
                case TaskType.Output:
                    return p1;
                  
                case TaskType.Move:
                    break;
                default:
                    break;
            }
            return null;
        }

        bool created;
        bool create()
        {
            if (created)
                return true;
            WayView v = G.GetSceneScript().transform.GetComponent<WayView>();
            if (v != null) {
                if (v.Created)
                {
                   
                    p1 = G.GetSceneScript().transform.GetComponent<WayView>().FindPoint(PointNames[0]);
                    p2 = G.GetSceneScript().transform.GetComponent<WayView>().FindPoint(PointNames[1]);
                    p1.AutoPoint = true;
                    p2.AutoPoint = true;
                    p1.IsTable = true;
                    p2.IsTable = true;
                    p1.CanInput = input1;
                    p1.SelectWayHandler = SelectWay1;
                    p2.SelectWayHandler = SelectWay2;
                    p2.CanInput = input2;
                    p1.CanOutput = output1;
                    p2.CanOutput = output2;
                    float offset = 0;

                    int i = 0;
                    foreach (var e in p2.Outputs)
                    {
                        var p = e.Point;
                        if (p.name[0] == 't')
                        {
                            var ss = p.name.Split('-');
                            var id = ss[2].Trim();
                            int lv = int.Parse(id);
                            floors.Add(new floor(e.Point,ssa[lv-1]));
                            i++;
                        }
                    }
                   
                    foreach(var e in floors)
                    {
                        e.offset = offset;
                        e.fpoint = this.p2;
                    }
                    created = true;
                }
            }
            return created;
     
        }

        int step = 1;

        void clearp()
        {
            for(int i = 0; i < p1.transform.childCount; i++)
            {
                var dd = p1.transform.GetChild(i);
                var p = dd.GetComponent<Pallet>();
                if (p != null)
                {
                    p.transform.parent = null;
                    G.GetSceneScript().PalletManager.Delete(p);
                    break;
                }
            }
            for (int i = 0; i < p2.transform.childCount; i++)
            {
                var dd = p2.transform.GetChild(i);
                var p = dd.GetComponent<Pallet>();
                if (p != null)
                {
                    G.GetSceneScript().PalletManager.Delete(p);
                    break;
                }
            }


        }
        private void Update()
        {
            if (create() == false)
                return;
            if (this.targetlevel != this.level)
            {
                Vector3 v = new Vector3(0, 0, curr);
                Vector3 t = new Vector3(0, 0, target);
                var k = Vector3.MoveTowards(v, t, G.LiftSpeed * Time.deltaTime);
                axis.Value = curr = k.z;
                if (k == t)
                {
                   // step = 2;
                    this.level = targetlevel;
                    if (this.level == -2)
                    {
                        clearp();
                        return;
                    }
                }
           
            }

        }

        public int xiangdao;

        public string showlevel;

        float getfloor(int level)
        {
            
            if (level == -1)
                return interface1Floor;
            if (level == -2)
                return interface2Floor;
            foreach(var e in floors)
            {
                if (e.level == level)
                    return e.GetValue();
            }
            return 0;
        }
        public void setLevel(int level)
        {
         
            var f = getfloor(level);
            this.target = f;
            this.targetlevel = level;
            /*
            if (this.target != curr)
            {
                step = 1;
            }
            */
        }
        public void SetLevel(int level)
        {
            return;
            if (xiangdao == 2)
            {
                return;
            }
          //  return;
            if (step == 1)
            {
                return;
            }

            var f = getfloor(level);
            this.target = f;
            this.targetlevel = level;
            if (this.target != curr)
            {
                step = 1;
            }

        }

    }



 
}

