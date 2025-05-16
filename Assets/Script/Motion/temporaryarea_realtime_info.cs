using Assets.Script.Datas;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Motion
{
    class temporaryarea
    {

        public WayPoint P1, P2;
        public Lift1 Lift;
        public RGV_H[] cars;
        public int Level;
        int direction;
        temporaryarea_realtime_info last;
        string sbank;
        string slevel;


        public void Loop()
        {
          //  lift_from_trigger.Invoke();
            if ( direction == 1)
            {
                Lift.TempMessage(this, true, P1, P2); 
            }
            if (direction == 2)
            {
                Lift.TempMessage(this, false, P1, P2);
            }
        }


        public temporaryarea(WayView wv,int bank,int level)
        {
            sc = G.GetSceneScript();
            var name1 = string.Format("t-{0}-{1}-1", bank == 1 ? 2 : 1, level);
            var name2 = string.Format("t-{0}-{1}-2", bank == 1 ? 2 : 1, level);
            Lift = GameObject.Find(bank == 1 ? "Lift1" : "Lift2").GetComponent<Lift1>();
            P1 = wv.FindPoint(name1);
            P2 = wv.FindPoint(name2);
            this.sbank = bank.ToString();
            this.Level = level;
            this.slevel = level.ToString();

        }

        string p1, p2;
        string time = null;

        SceneScript sc;

        public bool PushData(temporaryarea_realtime_info d)
        {

            if (d.BankID == sbank && d.Layer == slevel)
            {
                this.direction = int.Parse(d.Direction);
                return true;

            }
            return false;
        }
    }
}
