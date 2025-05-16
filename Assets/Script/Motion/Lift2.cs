using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.Motion
{
    public class Lift2 : MonoBehaviour
    {
        public float ceng1;
        public float ceng2;
        int step;
        float curr;
        float target;
        public int level;
        int targetlevel;
        public SingleAxisTable axis;
        public string PointName;
        WayPoint pp;

        float[] ff = new float[8];
        public void Start()
        {
            create();
            float k = 2.952f - 2.516f;
            for(int i = 7; i >= 0; i--)
            {
                ff[i] = 2.952f - (7 - i) * k;
            }
        }

         
        void create()
        {
           pp = G.FindChild(this.transform,PointName).GetComponent<WayPoint>();
            pp.CanInput = input;
            pp.CanOutput = output;
          
        }
        bool input(MonoBehaviour pallet, WayPoint from, WayPoint To)
        {
            if (level == 2 && step == 1)
            {
                return false;
            }
            if (level !=1)
            {
                this.SetLevel(1);
                return false;
            }
            return level == 1;
        }
        bool output(MonoBehaviour pallet, WayPoint from, WayPoint To)
        {
            if (level == 1 && step == 1)
            {
                return false;
            }
            if (level == 1)
            {
                this.SetLevel(2);
                return false;
            }
            return level == 2;
        }
        public void SetLevel(int level)
        {

            this.target = level==1 ? ceng1 : ceng2;
            this.targetlevel = level;
            step = 1;

        }
        private void Update()
        {
            switch (step)
            {
                case 0:

                case 1:
                    Vector3 v = new Vector3(0, 0, curr);
                    Vector3 t = new Vector3(0, 0, target);
                    var k  = Vector3.MoveTowards(v, t, 5 * Time.deltaTime);
                    axis.Value = curr = k.z;
                    if (k == t)
                    {
                        step = 2;
                        this.level = targetlevel;
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
