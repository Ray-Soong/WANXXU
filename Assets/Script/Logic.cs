using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script
{
    public class R_TRIG
    {
        bool st = false;
        bool q = false;
        public R_TRIG(bool initvalue = false)
        {
            st = initvalue;
        }
        public bool Invoke(bool clk)
        {
            var rt = false;
            if (clk != st)
            {
                st = clk;
                if (st)
                    rt = true;
            }
            q = rt;
            return q;
        }
        public bool Q { get { return q; } }
        public bool ST { get { return st; } }
    }

    /// <summary>
    /// 下降沿
    /// </summary>
    public class F_TRIG
    {
        bool st = false;
        bool q = false;
        public F_TRIG(bool initvalue = false)
        {
            st = initvalue;
        }
        public bool Invoke(bool clk)
        {
            var rt = false;
            if (clk != st)
            {
                st = clk;
                if (!st)
                    rt = true;
            }
            q = rt;
            return q;
        }
        public bool Q { get { return q; } }
        public bool ST { get { return st; } }
    }




    /// <summary>
    /// 定时器
    /// </summary>
    public class Timer
    {
        int step;

        bool q;
        DateTime last;
        double time;
        double timing;

        public double ET
        {
            get { return time; }
        }

        public bool Invoke(bool clk, int ms)
        {
            if (clk == false)
            {
                step = 0;
                q = false;
                return false;
            }

            switch (step)
            {
                case 0:
                    step = 1;
                    time = 0;
                    last = DateTime.Now;
                    timing = ms / 1000.0;
                    break;
                case 1:
                    DateTime now = DateTime.Now;
                    double dt = (now - last).TotalSeconds;
                    last = now;
                    if (dt > 0)
                    {
                        time += dt;
                    }
                    if (time >= timing)
                    {
                        step = 2;
                    }
                    break;
                default:
                    break;
            }
            q = step == 2;
            return q;
        }
        public bool Q { get { return q; } }


        public void ResetClock()
        {
            this.time = 0;
        }
    }


    /// <summary>
    /// 周期循环定时器
    /// </summary>
    public class LoopTimer
    {
        double timing;
        DateTime last;
        double time;
        public LoopTimer(int ms)
        {
            timing = ms / 1000.0;
            last = DateTime.Now;
            time = 0;
        }
        public bool Invoke()
        {

            DateTime now = DateTime.Now;
            double dt = (now - last).TotalSeconds;
            last = now;
            if (dt > 0)
                time += dt;
            if (time > timing)
            {

                time = 0;
                return true;
            }
            return false;
        }


    }
}
