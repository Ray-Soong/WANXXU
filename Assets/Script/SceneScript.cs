using Assets.Script;
using Assets.Script.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    class SceneScript:MonoBehaviour
    {
        //已放弃使用，被WayView代替
        PointManager points = new PointManager();
        //已放弃
        DataDispatcher mDataDispatchers = new DataDispatcher();

        //托盘管理器
        PalletManager pllets = new PalletManager();

        public PalletManager PalletManager
        {
            get { return pllets; }
        }

        DeviceManager dv = new DeviceManager();

        public DeviceManager DeviceManager { get { return dv; } }

        //点位管理器（没用）
        public PointManager PointManager { get { return points; } }
        //数据调度器
        public DataDispatcher DataDispatcher { get { return mDataDispatchers; } }


        public GameObject orgin;
        int step = 0;
        private void Start()
        {
            points.Create();
            pllets.Add(GameObject.Find("Pallet"));

        }

    
    }


   
}
