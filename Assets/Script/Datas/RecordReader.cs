using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.Datas
{
    
    class HistoryDataHelper
    {
        RecordReader<task_realtime_info> tasks;
        RecordReader<hoist_realtime_info> hoist;
        RecordReader<PointQueue> points;
        RecordReader<rgv_realtime_info> rgvs;
        RecordReader<interfaceline_realtime_info> inters;
        RecordReader<temporaryarea_realtime_info> tmps;
        RecordReader<carh_info> hlift;
        Action<object> sendhandle;
        bool first;

        void SetData<T>(RecordReader<T> rc, bool first) where T:IRealTime
        {
            if (first)
            {
                if(rc.MinTime<G.RecordBeginTime || G.RecordBeginTime == 0)
                {
                    G.RecordBeginTime = rc.MinTime;
                }
                return;
            }
            var dt= rc.getData();
            if (dt != null)
            {
                foreach(var e in dt)
                {
                    sendhandle(e);
                }
            }
        }

        public void loop()
        {
            SetData(tasks, false);
            SetData(hoist, false);
            SetData(points, false);
            SetData(rgvs, false);
            SetData(inters, false);
            SetData(tmps, false);
            SetData(hlift, false);
        }

        public void Create(Action<object> sendhandle)
        {
            this.sendhandle = sendhandle;
            string path = @"D:\config\abcc\8-24\data\";
            double d = double.MaxValue;
            tasks = new RecordReader<task_realtime_info>(path+"任务.txt");
            hoist = new RecordReader<hoist_realtime_info>(path + "货物提升机.txt");
            points= new RecordReader<PointQueue>(path + "输送线.txt");
            rgvs = new RecordReader<rgv_realtime_info>(path + "RGV.txt");
            inters = new RecordReader<interfaceline_realtime_info>(path + "接口线体.txt");
            tmps = new RecordReader<temporaryarea_realtime_info>(path + "暂存台.txt");
            hlift = new RecordReader<carh_info>(path + "换层提升机.txt");
            SetData(tasks,true);
            SetData(hoist, true);
            SetData(points, true);
            SetData(rgvs, true);
            SetData(inters, true);
            SetData(tmps, true);
            SetData(hlift, true);

            G.RecordBeginTime += 50;

        }

    }




    class RecordReader<T> where T : IRealTime
    {
       
        List<data<T>> datas = new List<data<T>>();
        int index = 0;

        public double MinTime;
        class data<T> where T : IRealTime
        {
            public T[] datas;
        
            public double timing;
            public data(string data, DateTime time)
            {
                var tt = JArray.Parse(data);
                if (tt.Count > 0)
                {
                    T[] t = new T[tt.Count];
                    for (int i = 0; i < t.Length; i++)
                    {
                        t[i] = (tt[i] as JObject).ToObject<T>();
                    }
                    this.datas = t;
                    this.timing = t[0].GetTime().TotalSeconds;
                }
            }
        }

        public T[] getData()
        {
        
            if (index < datas.Count)
            {
                var t = datas[0];
                var dd = t.timing - G.RecordBeginTime;
               if (dd <= Time.time)
                {
             

                    datas.RemoveAt(0);
                    return t.datas;
                }
            }
            return null;
        }

        public RecordReader(string file)
        {

            List<data<T>> tt = this.datas;
            int step = 0;
            MinTime = double.MaxValue;
            using (var st = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var sr = new System.IO.StreamReader(st))
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime tm = new DateTime();
                    while (!sr.EndOfStream)
                    {
                        var s = sr.ReadLine();
                        if (s == null)
                            continue;
                        s = s.Trim();
                        if (s == "" || s.Length <= 0)
                            continue;
                        if (s[0] == '#' && s[1] == '#' && s[2] == '#')
                        {
                            if (sb.Length != 0)
                            {
                                data<T> d = new RecordReader<T>.data<T>(sb.ToString(), tm);
                                if (d.timing < MinTime && d.timing != 0)
                                    MinTime = d.timing;
                                tt.Add(d);
                                sb.Clear();
                            }
                        }
                        else
                        {
                            sb.Append(s);
                        }

                    }
                    if (sb.Length != 0)
                    {
                        data<T> d = new RecordReader<T>.data<T>(sb.ToString(), tm);
                        if (d.timing < MinTime && d.timing != 0)
                            MinTime = d.timing;
                        tt.Add(d);
                    }

                }
            }
        }
    }
}
