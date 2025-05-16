using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.Motion
{
    [System.Serializable]
    public class LinearTransformation
    {
        float k = 1, b1 = 1, b2 = 1;
        public float transform(float a)
        {
            return (a - b1) * k + b2;
        }

        public void compute()
        {
            k = (LocalValue1 - LocalValue2) / (SourceValue1 - SourceValue2);
            b1 = SourceValue2;
            b2 = LocalValue2;
        }


        public float LocalValue1 = 0;
        public float LocalValue2 = 1;
        public float SourceValue1 = 0;
        public float SourceValue2 = 1;
    }


    public enum AxisName
    {
        X,
        Y,
        Z,
        A,
        B,
        C
    }

    //轴控制器
    public class AxisController
    {
        float dt = 0;
        bool isrealtime;
        float curr;
        float target;
        float speed;
        bool moving;
        bool force;
        public bool Moving { get { return moving; } }
        public void ToTarget()
        {
            this.curr = this.target;
        }


        //直接设置位置
        public void SetPosition(float v, bool clearTarget = true)
        {
            this.force = true;
            this.curr = v;
            if (clearTarget)
                this.target = this.curr;

        }

        public float move(float curr)
        {
            if (!force)
            {
                this.curr = curr;
            }
            if (this.speed <= 0)
                return this.curr;
            if (this.curr == this.target)
                return this.curr;
            float p = 0;
            if (this.target < this.curr)
            {
                p = this.curr - this.speed * Time.deltaTime;
                if (p <= this.target)
                {
                    this.curr = this.target;
                }
            }
            else
            {
                if (this.target > this.curr)
                {
                    p = this.curr + this.speed * Time.deltaTime;
                    if (p >= this.target)
                    {
                        p = this.target;
                    }
                }
            }
            this.curr = p;
            return p;
        }

        public float CurrentValue { get { return curr; } }

        //以指定速度移动到某位置
        public void MoveToBySpeed(float target, float speed)
        {
            this.target = target;
            this.speed = Mathf.Abs(speed);
        }
        //以指定时间移动到某位置
        public void MoveToByTime(float target, float time)
        {
            this.target = target;
            if (this.curr != this.target && time > 0)
            {
                this.speed = Mathf.Abs((this.target - this.curr) / time);
            }
            else
            {
                this.SetPosition(target);
            }
        }

        public void RealTime(float position, float maxinterval = 2)
        {
            this.target = position;
            if (this.target != curr)
            {
                if (dt <= 0 || dt >= maxinterval)
                {
                    this.SetPosition(target);
                }
                else
                {
                    this.speed = Mathf.Abs((this.target - curr) / dt);
                }
                dt = 0;
            }
            else
            {
                dt = 0;
                return;
            }
        }
    }


  
}
