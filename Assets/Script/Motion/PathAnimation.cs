using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//预设路径动画,用于传输线
class PathAnimation
{
    float pos;
    float target;
    float speed;
    float useTime;
    public int currentSegID;
    public Vector3 CurrentAngle;
    public Vector3 CurrentPosition;


    public bool forword;
    Vector3 start;
    int step = 0;
    class Segment
    {
        public int ID;
        public Vector3 to;
        public Vector3 from;
        public float Length;
        public Vector3 inc;
        public float pos;
        public Vector3 direction;

        public void forword()
        {


            var x = to.x - from.x;
            var y = to.z - from.z;

            this.direction = Quaternion.LookRotation(new Vector3(x, 0, y), Vector3.up).eulerAngles;

            /*
            var f = 0f;
            if (x == 0)
            {
                if (y > 0)
                {
                    f = 90;
                }
                else
                    f = -90;
            }
            else
            {
                f = Mathf.Atan(y / x) * 180 / Mathf.PI;
                if (x < 0)
                {
                    f = 180 - f;
                }
            }
            this.direction = new Vector3(0, f, 0);
            */

        }

        public void computeLength()
        {
            inc = (to - from).normalized;
            Length = Vector3.Distance(from, to);
        }
    }
    List<Segment> segs = new List<Segment>();

    abstract class segment
    {
        public Vector3 From;
        public Vector3 To;
        public float Start;
        public float Length;
        public Vector3 Current;
        public Quaternion Rotate;
        public abstract void ComputePosition(float pos);
        public abstract void init();



    }



    class ArcSegment : segment
    {
        public Vector3 to;
        public Vector3 from;
        public float angle;

        public override void ComputePosition(float pos)
        {
            // GameObject o;
            //  o.transform.RotateAround()
        }

        public override void init()
        {
            if (angle == 90)
            {
                GameObject o;
                //   Quaternion.rou
            }

        }
    }


    //重新计算目标点,reset代表重新定位，否则继续运动
    void compute(bool reset)
    {

        if (this.segs.Count == 1)
        {
            this.pos = 0;
            this.segs[0].computeLength();
            this.target = this.segs[0].Length;
        }
        else
        {
            var s = this.segs.Last();
            s.pos = this.target;
            s.computeLength();
            this.target += s.Length;
        }
    }


    Vector3 last;

    public bool AddSegment(int id, Vector3 to)
    {

        segs.Add(new Segment()
        {
            from = this.last,
            to = to,
            ID = id
        });
        compute(false);
        this.step = 1;
        if (forword)
        {
            segs.Last().forword();
        }
        this.last = to;
        return true;

    }
    public void Start(int SegmentID, Vector3 from, Vector3 angle)
    {
        this.last = this.start = from;
        this.CurrentPosition = from;
        this.currentSegID = SegmentID;
        this.CurrentAngle = angle;
        this.segs.Clear();
        this.pos = 0;
        this.step = 0;
    }

    public void end()
    {
        if (this.segs.Count > 0)
        {
            this.CurrentPosition = this.segs[this.segs.Count - 1].to;
            this.currentSegID = this.segs[this.segs.Count - 1].ID;
            this.step = 0;
            this.start = this.CurrentPosition;
            if (this.forword)
                this.CurrentAngle = this.segs[this.segs.Count - 1].direction;
            this.segs.Clear();
        }
    }



    public bool update()
    {
        if (step == 0 && this.segs.Count == 0)
            return true;


        if (step == 1)
        {
            var p = G.MoveTo(this.pos, this.target, this.speed);

            this.pos = p;
            if (p == this.target)
            {
                this.end();
                return true;
            }

            int idx = -1;
            for (int i = 0; i < this.segs.Count; i++)
            {
                var sg = segs[i];
                if (p >= sg.pos)
                {
                    idx = i;
                }
            }
            Vector3 cp = CurrentPosition;
            if (idx >= 0)
            {
                var sg = this.segs[idx];
                this.currentSegID = sg.ID;
                float add = p - sg.pos;
                cp = sg.from + sg.inc * add;
                this.CurrentPosition = cp;
                this.CurrentAngle = sg.direction;
                if (idx > 0)
                {
                    for (int i = idx - 1; i >= 0; i--)
                    {
                        this.segs.RemoveAt(0);
                    }
                    float f = this.segs[0].pos;
                    float tmp = 0;
                    for (int i = 0; i < this.segs.Count; i++)
                    {
                        this.segs[i].pos = tmp;
                        tmp += this.segs[i].Length;
                    }
                    this.pos = add;
                    this.target = tmp;
                }
            }
        }
        return false;
    }

    public void setSpeed(float speed)
    {
        this.speed = Mathf.Abs(speed);
    }
    public void setUseTime(float time)
    {
        if (time > 0)
            this.speed = Mathf.Abs(this.target - this.pos) / time;
    }

    public float currentSpeed
    {
        get { return this.speed; }
    }
}




