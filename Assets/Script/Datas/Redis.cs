using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis
{

    public class Config
    {
        public string Host;
        public int Port;
        public Point[] Points;
    }

    public class Point
    {

        public string Key;
        public string Name;
        public int Interval;
    }

    class RedisObj
    {
        ConnectionMultiplexer redis;
        Config config;
        bool connected;

        public static bool Record;

        public RedisObj()
        {
            config = JObject.Parse(System.IO.File.ReadAllText(@"D:\config\config.txt")).ToObject<Config>();
       
            System.Threading.ThreadPool.QueueUserWorkItem(loop);
        }


     





        List<PointView> pvs = new List<PointView>();
        class PointView 
        {
            bool first = false;
            public void ReadAll(ConnectionMultiplexer redis)
            {
                StringBuilder sb = new StringBuilder();
                var db = redis.GetDatabase(0);
                for (int i = 0; i < 1000; i++)
                {
                    var pp = db.ListGetByIndex(this.p.Key, i);
                    if (pp.IsNullOrEmpty)
                        break;
                    sb.AppendLine("#######");
                    sb.AppendLine(pp.ToString());
                }
                if (System.IO.File.Exists(this.path) == false)
                {
                    System.IO.File.Create(this.path).Close();
                }
                System.IO.File.WriteAllText(this.path, sb.ToString());
            }

            object Lock = new object();
            StringBuilder strs = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            string last = null;
            long index = 0;
            
            //bool first = false;
            public string Read(ConnectionMultiplexer redis)
            {



                var db = redis.GetDatabase(0);
                var c = db.ListLength(this.p.Key);
                if (c > 0)
                {

                    var pp = db.ListGetByIndex(this.p.Key, c-1);
                    if (pp.IsNullOrEmpty)
                    {
                        return null;
                    }
                    var ss = pp.ToString();
                   
                    var len = db.ListRemove(this.p.Key, c);
                    if (first == false)
                    {
                        first = true;
                        return null;
                    }
                    return ss;
                    if (last == ss) 
                        return null;
                    
                    lock (Lock)
                    {
                        last = ss;
                        strs.AppendLine("#######");
                        strs.AppendLine(ss);
                    }
                    return ss;
                }
                return null;
            }


            public string[] Readdata(ConnectionMultiplexer redis)
            {



                var db = redis.GetDatabase(0);
                var c = db.ListLength(this.p.Key);
                if (first == false)
                {
                    db.ListTrim(this.p.Key, 1, 0);

                    c = db.ListLength(this.p.Key);
                    first = true;
                    return null;
                }
                RedisValue latestValue = new RedisValue();
                List<string> sss = new List<string>();
                do
                {
                    latestValue = db.ListLeftPop(this.p.Key);
                    if (!latestValue.IsNullOrEmpty)
                    {
                        sss.Add(latestValue.ToString());
                    }
                    else
                        break;
                } while (latestValue.IsNullOrEmpty == false);
                c = db.ListLength(this.p.Key);
                if (sss.Count > 0)
                {
                    return sss.ToArray();
                }
                return null;
                /*
                if (db.ListLength(this.p.Key) > 0)
                {
                    db.ListTrim(this.p.Key, 1, 0);
                }

                if (!latestValue.IsNullOrEmpty)
                {
                    return new string[] { latestValue.ToString() };
                }
                return null;
                */
            }



            public string[] _Readdata(ConnectionMultiplexer redis)
            {

                long lenn = 0;

                var db = redis.GetDatabase(0);
                var c = db.ListLength(this.p.Key);
                if (first == false)
                {
                    try
                    {
                        lenn = db.ListRemove(this.p.Key, c);
                    }
                    catch(System.Exception e)
                    {
                        var ss = e;
                    }
                    first = true;
                    return null;
                }

                if (c > 0)
                {
                    List<string> sss = new List<string>();
                    for (int i = 0; i < c; i++)
                    {

                        var pp = db.ListGetByIndex(this.p.Key, i);
                        if (!pp.IsNullOrEmpty)
                        {
                            sss.Add(pp.ToString());
                        }

                    }
                    var len = db.ListRemove(this.p.Key, c);
                    return sss.ToArray();
                }
                return null;
            }
            public void Save()
            {
                string str = null;
                lock (Lock)
                {
                    if (strs.Length > 0)
                    {
                        str = strs.ToString();
                        strs.Clear();
                    }
                    else
                        return;
                }
                if (System.IO.File.Exists(this.path) == false)
                {
                    System.IO.File.Create(this.path).Close();
                }
                System.IO.File.AppendAllText(this.path, str);
            }


   
            DateTime time;
            Point p;
            ConnectionMultiplexer redis;
            IDatabase db;
            public Point Point { get { return p; } set { p = value; } }

            public string Name { get { return p.Name; } }

            public string Key { get { return p.Key; } }
            string data;
            DateTime save;
            public string path;
            public DateTime Time { get; set; }
            public string Data { get { return data; } }

            int _tick;
         

            public int tick;

            public string View
            {
                get { return data; }
            }

            void enuma(IDatabase db)
            {
                RedisKey[] keys = null;
                long cursor = 0;
                var allKeys = new List<RedisKey>();
                var kk = db.HashGetAll("*");

            }


        

            public void loop(ConnectionMultiplexer redis)
            {
                DateTime t = DateTime.Now;


                if ((t - time).TotalMilliseconds > p.Interval)
                {
                    time = t;
                    if (redis != this.redis)
                    {
                        db = redis.GetDatabase(0);
                        // db.HashKeys(null);
                    }

                  
                    var dd = db.ListGetByIndex("bank_realtime_info", 0);

                    //   db.HashValues("bank_realtime_info");

                    //  var kk = db.HashGetAll("*");
                    this.Time = t;
                    tick++;
                    if (RedisObj.Record)
                    {
                        sb.AppendLine("#########################################################");
                        sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.nnn"));
                        sb.AppendLine(this.data);

                    }
                }

                if ((t - save).TotalSeconds > 10)
                {
                    if (System.IO.File.Exists(path) == false)
                    {
                        System.IO.File.Create(path).Close();
                    }
                    if (sb.Length > 0)
                    {
                        System.IO.File.AppendAllText(path, sb.ToString());
                        save = t;
                        sb.Clear();
                    }
                }
            }


        }

        int step;

        public void Close()
        {
            this.step = 30;
        }

  


        public void Open(string path)
        {
            if (this.step == 0)
            {

                this.step = 10;
                pvs.Clear();
                foreach (var e in this.config.Points)
                {
                    PointView v = new PointView()
                    {
                        path = path + e.Name + ".txt",
                        Point = e
                    };
                    pvs.Add(v);
                }
            }
        }

        public bool Connected { get { return step == 20; } }
        int k = 0;


        //redis 数据轮询
        void loop(object o)
        {
            while (true)
            {
                switch (step)
                {
                    case 30:
                        if (this.redis != null)
                        {
                            this.redis.Close();
                            this.step = 0;
                            this.redis = null;
                        }
                        break;
                    case 10:
                        try
                        {

                            ConfigurationOptions configurationOptions = new ConfigurationOptions
                            {
                                EndPoints = { { config.Host, config.Port } }, // 更改为你的Redis服务器地址和端口
                                AbortOnConnectFail = false,
                              //  User = config.Host,
                                Password = ""
                            };
                            // 创建连接
                            this.redis = ConnectionMultiplexer.Connect(configurationOptions);
                            if (this.redis != null)
                            {
                                if (this.redis.IsConnected)
                                {
                                    this.step = 20;
                                }

                            }
                            if (this.step == 10)
                                System.Threading.Thread.Sleep(1000);
                        }
                        catch
                        {

                        }
                        break;
                    case 20:

                        foreach (var e in pvs)
                        {
                            try
                            {
                                // 读一次数据（pvs）
                                var s = e.Readdata(this.redis);
                                if (s != null)
                                {
                                    foreach (var r in s)
                                    {
                                        G.DataBox.Push(e.Key, r);
                                    }
                                }
                                if (k > 5000)
                                {
                                    
                                   // e.Save();
                                }
                            }
                            catch (Exception er)
                            {
                                try
                                {
                                    this.redis.Close();
                                    
                                }
                                catch
                                {

                                }
                                this.step = 10;
                                break;
                            }

                        }
                        if (k > 5000)
                        {
                            k = 0;
                        }
                        
                        
                        System.Threading.Thread.Sleep(50);
                        k += 50;
                    
                        
                           break;
                }
            }

        }
   

    }
   
   
    /*
    class dd
    {
        public static System.Data.DataTable[] proc()
        {
            string connectionString = "User Id=wcs_db;Password=wcs_db;Data Source=172.30.121.57:1521/orcl;";

            // 创建Oracle连接
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // 打开连接
                connection.Open();

                List<System.Data.DataTable> tb = new List<System.Data.DataTable>();
                foreach (var e in new string[] { "SELECT * FROM view_order_num", "SELECT * FROM view_box_num", "SELECT * FROM view_tag_num" })
                {
                    // 创建命令
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        // 设置SQL命令
                        command.CommandText = e;


                        // 执行命令，得到一个DataReader
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            System.Data.DataTable tab = new System.Data.DataTable();
                            tb.Add(tab);
                            int c = reader.FieldCount;
                            for (int i = 0; i < c; i++)
                            {
                                tab.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                            }
                            // 遍历结果集
                            while (reader.Read())
                            {

                                var r = tab.NewRow();
                                for (int i = 0; i < c; i++)
                                {
                                    r[i] = reader.GetValue(i);
                                }
                                tab.Rows.Add(r);
                                // 输出每一行的数据
                                //  Console.WriteLine(reader.GetString(0)); // 假设第一列是字符串类型
                            }

                        }
                    }
                }

                return tb.ToArray();
            }
        }
    }
    */


}