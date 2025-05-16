using Assets.Script;
using Assets.Script.Datas;
using Assets.Script.Motion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Pallet : MonoBehaviour
{

    int step = 0;
    public WayPoint current;
    Vector3 target;
    public int TaskLevel;
    public TaskType TaskType;
    public string InputBank;
    public string point;
    PathAnimation anim = new PathAnimation();
    bool frompos = false;
    public bool output_liu;

    public string CellPosition;
    

  

    Vector3 celltarget;
    public int cellnumber = 0;
    public void MoveCell(int cellnumber, Vector3 p)
    {
        this.transform.parent = null;
        var curr = this.current;
        if (this.current != null)
        {
            if (this.current.CurrentPallet == this)
            {
                this.current.CurrentPallet = null;
            }
            this.current = null;
        }
        this.current = null;
        this.cellnumber = cellnumber;
        this.celltarget = p;
        this.transform.parent = null;
        if (curr!= null)
        {
            if (curr.name != "Table")
            {
                this.frompos = true;
                this.transform.position = p;
                step = 11;
                return;
            }
        }
        if (curr == null)
        {
            this.frompos = true;
            this.transform.position = p;
            step = 11;
            return; 
        }



        this.anim.Start(1, this.transform.position, new Vector3(0, 0, 0));
        this.anim.AddSegment(1, p);
        this.anim.setUseTime(2);
        step = 10;
    }
    public string LastPoint;
    List<WayPoint> wps = new List<WayPoint>();
    WayPoint lastp = null;

    bool pushPoint(WayPoint p)
    {

        this.LastPoint = p.name;
        foreach(var e in wps)
        {
            if (p == e)
            {
                wps.Clear();
            }
        }
        wps.Add(p);
        if (wps.Count > 5)
        {
            wps.RemoveAt(0);
        }



        if (lastp == null)
        {
            anim.Start(1, p.Point, new Vector3(0, 0, 0));
            lastp = p;
            return true;
        }
        var pp = lastp.Point;
        if (this.frompos)
        {
            frompos = false;
            anim.Start(1, this.transform.position, new Vector3(0, 0, 0));
            pp = this.transform.position;
        }
        var st = false;
        if (Math.Abs(pp.y - p.Point.y) > 0.2)
        {
            st = true;
        }

        //这里是一个数据纠错，解决掉头运动问题
        if (p.name[0] == '0' && lastp.name[0] == '0')
        {

            if (p.Outputs.Count > 0)
            {
                foreach (var e in p.Outputs)
                {
                    if (e.Point == lastp)
                        return false;
                }
            }
        }


        if (p.name[0] != '0' && lastp.name[0] != '0')
        {
            if (st != true)
            {
                anim.AddSegment(1, p.Point);
                anim.setUseTime(1);
            }
            else
            {
                anim.Start(1,p.Point, new Vector3(0, 0, 0));
;            }
            lastp = p;

            return true;
        }
        else
        {
            if (st == true)
            {
                anim.Start(1, p.Point, new Vector3(0, 0, 0));
                lastp = p;
                return true;
            }
            List<WayPoint> ps = WayView.FindWay(lastp, p, 5);
            if (ps != null)
            {
                if (ps.Count <= 5)
                {
                    bool b = false;
                    foreach (var e in ps)
                    {
                        if (b == false)
                        {
                            b = true;
                            continue;
                        }
                        anim.AddSegment(1, e.Point);
                    }
        
                    anim.setUseTime(1);
                    lastp = p;
                    return true;
                }
                else
                {
                    anim.Start(1, p.Point, new Vector3(0, 0, 0));
                    lastp = p;
                    return true;
                }

            }
            else
            {
                anim.Start(1, p.Point, new Vector3(0, 0, 0));
                lastp = p;
                return true;
            }
        }
        
    }


    public void BeforeDelete()
    {
        if (this.current != null)
        {
            this.current.CurrentPallet = null;
            this.current = null;
        }
    }

    WayPoint targetpos = null;
    void InnerMoveTo(WayPoint p)
    {
        points.Add(p);
        if (points.Count > 5)
        {
            points.RemoveAt(0);
        }
        if (p.name == "i-1-1-1" || p.name=="i-1-1-2" || p.name=="i-2-1-2" || p.name == "i-2-1-1")
        {
            this.output_liu = true;
        }

        if (p.name[0] == 'i')
        {
            if (this.isinput && Posstate == PosState.InputLine || Posstate == PosState.None)
            {
                Posstate = PosState.InputInterface;
            }
        }
        if (this.current != null && this.current != p)
        {
            if (this.current.CurrentPallet == this)
            {
                this.current.CurrentPallet = null;

            }
        }
        // this.targetpos = p;   
        this.current = p;
        p.CurrentPallet = this;
        step = 0;
    }

    BankPointHelper bank1;
    BankPointHelper bank2;
    /*
    bool MoveEndEx()
    {
        var bk = bank1;
        if(this.current == bk.InputP1)
        {
            if (bk.InputP2.CurrentPallet == null)
            {
                this.InnerMoveTo(bk.InputP2);
            }
            return true;
        }
        if(this.current== bk.InputP2)
        {
            if (bk.Lift.level == -2)
            {
                this.InnerMoveTo(bk.Lift.p1); 
            }
            return true;
        }
        if (this.current == bk.Lift.p1)
        {
            if (this.isinput)
            {
                if (bk.Lift.p2.CurrentPallet == null)
                    this.InnerMoveTo(bk.Lift.p2);
            }
            if (this.isoutput)
            {
                if (bk.Lift.level == -1)
                {
                    this.InnerMoveTo(bk.OutputP1);
                }
            }
            return true;
        }
        if (this.current == bk.Lift.p2)
        {
            if (this.isinput)
            {
                if (bk.Lift.p2.CurrentPallet == null)
                {
                    if (bk.Lift.level > 0 && bk.Lift.level<=8)
                    {
                        var p = bk.TempP1s[bk.Lift.level-1];
                        temporaryarea_realtime_info info = p.CommData as temporaryarea_realtime_info;
                        if (info != null)
                        {
                            if (bk.Lift.KeepLevelTimes > 1 || info.Direction == "1")
                            {
                                this.InnerMoveTo(p);
                            }
                        }
                    }
                }
            }
            if (this.isoutput)
            {
                if (bk.Lift.p1.CurrentPallet == null)
                    this.InnerMoveTo(bk.Lift.p1);
            }
            else
            {

            }
            return true;
        }
        if (this.current == bk.OutputP1)
        {

        }

    } 
    */

    void MoveEnd()
    {
        if (output_liu)
        {
            WayPoint next = null;
            if (current.Outputs.Count == 1)
            {
                next = current.Outputs[0].Point;

            }
            if (current.name == "01015")
            {
                foreach (var e in current.Outputs)
                {
                    if (e.Point.name == "01016")
                    {
                        next = e.Point;
                        break;
                    }
                }
            }
            if (current.name == "01020")
            {
                foreach (var e in current.Outputs)
                {
                    if (e.Point.name == "01021")
                    {
                        next = e.Point;
                        break;
                    }
                }
            }
            if (next != null)
            {
                if ((next.CurrentPallet == this || next.CurrentPallet == null))
                {
                    this.InnerMoveTo(next);
                }
            }
            if (current.name == "01003")
            {
                this.output_liu = false;
            }
        }



        if (current.CanSelectWay)
        {

            var p = current.SelectWay(this);
            if (p == null)
                return;
            if (current.OutputAble(this, p) && p.InputAble(this, current))
            {
                this.InnerMoveTo(p);
            }
        }
        else
        {



            if (current.Outputs.Count == 1)
            {

                var p = current.Outputs[0].Point;
                if (current.AutoPoint)
                {
                    if (current.OutputAble(this, p) && p.InputAble(this, current))
                    {
                        this.InnerMoveTo(p);
                    }
                }
            }
            else
            {
                return;
                if (current.Outputs.Count > 1)
                {
                    var p = current.Outputs[G.Random(current.Outputs.Count)].Point;
                    if (current.OutputAble(this, p) && p.InputAble(this, current))
                    {
                        this.InnerMoveTo(p);
                    }
                }
                else
                {
                    this.SetPoint(ffff);
                }
            }

        }


    }

    public void Delete()
    {
        G.GetSceneScript().PalletManager.DelayDelete(this);
    }


    float speed;

    WayPoint last;
    float tm = -1;
    void maybeerror()
    {
        if (last != this.current)
        {
            if (this.current != null)
            {
                if(this.current.name[0] == '0')
                {
                    tm = 0;
                }
                else
                {
                    tm = -1;
                }
            }
            this.last = current;
           
        }
        else
        {
            if(this.current!=null)
          //  if (this.current.name[0] == '0')
            {

                tm += Time.deltaTime;
                if (tm > 10)
                {
                    if (this.last != null)
                    {
                        
                        if (this.last.name != "01024"
                            && this.last.name!="Table"
                            && this.last.name[0] != 't'
                            && this.last.name != "01025"
                              && this.last.name != "01026"
                                && this.last.name != "01027"

                                  && this.last.name != "01028"
                                    && this.last.name != "01029"
                                      && this.last.name != "01030"
                                        && this.last.name != "01031"
                            )
                        {
                            this.Delete();
                        }
                        
                    }

                }
            }
        }
    }

    bool end = false;

    private void Update()
    {
       
        if (targetpos != null)
        {
            if (true)
            {
                if (this.current != null && this.current != targetpos)
                {
                    if (this.current.CurrentPallet == this)
                    {
                        this.current.CurrentPallet = null;
                    }
                }
                this.current = targetpos;
                targetpos = null;
                this.current.CurrentPallet = this;
                step = 0;
            }
        }
        if (current != null || (current == null && step == 10))
        {
            switch (step)
            {
                case 0:
                    target = current.Point;
                    this.transform.parent = null;
                    if (this.pushPoint(current))
                    {
                        step = 1;
                    }
                    break;
                case 1:
                    var p = this.transform.position;
                    if (anim.update())
                    {
                        if (end == false)
                        {
                            end = true;
                            this.transform.position = anim.CurrentPosition;
                        }
                        step = 2;
                        if (this.current.IsTable)
                        {
                            this.transform.parent = this.current.transform;
                            this.frompos = true;
                        }
                        this.MoveEnd();
                    }
                    else
                    {
                        end = false;
                        this.transform.position = anim.CurrentPosition;
                    }
                    break;
                case 2:
                    this.MoveEnd();
                    break;
                case 10:
                    var pa = this.transform.position;
                    var pp = Vector3.MoveTowards(pa, this.celltarget, G.PalletSpeed * Time.deltaTime);
                    if (pp == this.celltarget)
                    {
                        step = 11;
                    }
                    this.transform.position = pp;
                    break;
                default:
                    break;
            }
        }
        maybeerror();
    }
    public object token;
    internal task_realtime_info task;
    //类型名称
    public string TypeName;

    WayPoint ffff;
    public void SetPoint(WayPoint p)
    {
        ffff = p;
        if (this.current != null && this.current != p)
        {
            if (this.current.CurrentPallet == this)
            {
                this.current.CurrentPallet = null;
            }
        }
        this.current = p;
        p.CurrentPallet = this;
        this.transform.position = p.Point;
        if (p.IsTable)
        {
            this.transform.parent = p.transform;
        }
        this.step = 2;
    }

    List<WayPoint> points = new List<WayPoint>();

    bool isnew = true;
    public bool removed;
    public bool isinput = false;
    public bool isoutput = false;
    public bool outputing = false;

    public void SendPoint(WayPoint p, bool force = false)
    {
       
        

        
        if (p == null || removed)
            return;
        if (this.current != null)
        {
            if(p.name[0]=='0' && current.name[0] == '0')
            {
                var pp = WayView.FindWay(p, current, 5);
                if (pp != null)
                    return;
            }



            if (current.name[0] == 't' && p.name[0] == 't')
            {
                var pp = current.Point.z - p.Point.z;
                if (this.TaskType == TaskType.Input && pp < 0)
                {
                    return;
                }
                if (this.TaskType == TaskType.Output && pp > 0)
                {
                    return;
                }

            }
        }


        if (p.name == "01035")
        {
            if (current.name != "01032" && current.name != "01033" && current.name != "01034")
                return;
        }

        if (this.output_liu)
            return;
        if (p.name == "01038" || p.name == "01039")
        {
            this.isinput = true;
            this.Posstate = PosState.InputLine;
        }
        this.point = p.name;

        foreach (var e in points)
        {
            if (e == p)
                return;
        }
        /*
       var pp = p.Point;
       var kk = this.transform.position;
       var t = (pp - kk);
       t.Normalize();
       var x = Mathf.Abs(t.x);
       var z = Mathf.Abs(t.z);
       var y = Mathf.Abs(t.y);
       bool b = false;
       if (!(x > 0.9 || z > 0.9) && y<)
       {
           b = true;
       }
        */
        bool b = false;
       
        if (this.current != null && p != null)
        {
            if (this.current.name == "table" || p.name == "table")
            {

            }
            else
            {
                b = true;
                if (this.current.Outputs != null)
                {
                    foreach (var e in this.current.Outputs)
                    {
                        if (e.Point == p)
                        {
                            b = false;
                            break;
                        }
                    }
                }
                if (b == true)
                {
                    if (p.Outputs != null)
                    {
                        foreach (var e in p.Outputs)
                        {
                            if (e.Point == this.current)
                            {
                                b = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (b == false)
        {
            var pp = p.Point;
            var kk = this.transform.position;
            var t = (pp - kk);
            t.Normalize();
            var x = Mathf.Abs(t.x);
            var z = Mathf.Abs(t.z);
            var y = Mathf.Abs(t.y);
            if (y>0.05f)
            {
                b = true;
            }
        }

        if (isnew)
        {
            isnew = false;
            SetPoint(p);
        }
        else
        {
            InnerMoveTo(p);
        }
    }

    internal void SendTask(task_realtime_info task)
    {
        this.InputBank = task.BankID;
        switch (task.TaskType)
        {
            case "1":
                this.TaskType = TaskType.Input;
                break;
            case "2":
                this.TaskType = TaskType.Output;
                break;
            case "3":
                this.TaskType = TaskType.Move;
                break;

            default:
                this.TaskType = TaskType.None;
                break;
        }
        if (task.Layer != null)
        {
            this.TaskLevel = int.Parse(task.Layer);
        }
    }



    void sendtask(task_realtime_info task)
    {
        string ss = string.Format("t-{0}-{1}-{2}-{3}:{4} ", task.BankID, task.TaskType, task.Layer, task.TaskStatus, task.TaskStep);
        if (ss != lasttask)
        {
            lasttask = ss;
            sb.Append(ss);
            sb.Append(" ");
        }
    }

    StringBuilder sb = new StringBuilder();

    public void save(StringBuilder sb)
    {
        sb.AppendFormat("[{0}]\r\n", this.name);
        sb.AppendLine(this.sb.ToString());
    }
    float ttm = 0;
    public void Error()
    {
        bool ok = false;
        if (this.current != null)
        {
            if (this.current.name == "i-2-2-2" || this.current.name == "i-1-2-2")
            {
                if (this.current.LastHasBox == false)
                {
                    ok = true;
                    ttm += Time.deltaTime;
                    if (ttm > 20)
                    {
                        G.GetSceneScript().PalletManager.Delete(this);
                    }
                }
                else
                {
                    this.current.LastHasBox = true;
                }
            }
        }
        if (!ok)
        {
            ttm = 0;
        }
    }


    string lastpoint;
    string lasttask;

    public PosState Posstate;

    public enum PosState
    {
        None,
        InputLine,
        InputInterface,
        InputLift,
        InputTem,
        OutputTem,
        OutputInterface,
        OutputLift,
        OutputLine

    }


}
public enum TaskType
{
    None,
    Input,
    Output,
    Move
}


public class BankPointHelper
{
    public int Bank;
    public WayPoint InputP1;
    public WayPoint InputP2;
    public WayPoint OutputP1;
    public WayPoint OutputP2;
    public WayPoint[] TempP1s;
    public WayPoint[] TempP2s;
    public Lift1 Lift;
}







