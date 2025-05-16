using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Datas
{
    interface IRealTime
    {
        TimeSpan GetTime();
    }


    class PointQueue : IRealTime
    {
        public string areaID;
        public string PointCode;
        public string ContainerCode;
        public string TargetPoint;
        public string speed;
        public string TimeSpan;
        public string SeqNo;
        public string Status;

        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }

    class task_realtime_info : IRealTime
    {
        public string BankID;
        public string Layer;
        public string ContainerCode;
        public string TaskStatus;
        public string TaskStep;
        public string TimeSpan;
        public string TaskType;
        public string TaskRGVStatus;

        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }


    public class hoist_realtime_info : IRealTime
    {
        public string BankID;
        public string Way;
        public string Status;
        public string FirstPosotion;
        public string FirstPosotionTask;
        public string SecondPosotion;
        public string SecondPosotionTask;
        public string Level;
        public string FaultCode;
        public string TimeSpan;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }


    public class rgv_realtime_info : IRealTime
    {
        public string BankID;
        public string id;
        public string FaultCode;
        public string floor;
        public string enable;
        public string HasBox;
        public string Position;
        public string SourcePosition;
        public string TargetPosition;
        public string TaskID;
        public string Speed;
        public string TimeSpan;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }

    public class interfaceline_realtime_info : IRealTime
    {
        public string BankID;
        public string Layer;
        public string Way;
        public string Status;
        public string FirstPosotion;
        public string FirstPosotionTask;
        public string SecondPosotion;
        public string SecondPosotionTask;
        public string TimeSpan;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }

    }
    //"BankID":"1","Status":"1","HasCar":"1","FaultCode":"0","CarID":"0","Targetlayer":"0","TimeSpan":"1721786299"
    //"BankID":"1","Way":"1","Status":"1","FirstPosotion":"1","FirstPosotionTask":null,"SecondPosotion":"1","SecondPosotionTask":null,"Level":"0","FaultCode":"2","TimeSpan":"1721786311"
    public class carh_info : IRealTime
    {
        public string BankID;
        public string Status;
        public string HasCar;
        public string FaultCode;
        public string CarID;
        public string Targetlayer;
        public string TimeSpan;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }
    public class temporaryarea_realtime_info : IRealTime
    {
        public string BankID;
        public string Layer;
        public string Way;
        public string Status;
        public string FirstPosotion;
        public string FirstPosotionTask;
        public string SecondPosotion;
        public string SecondPosotionTask;
        public string TimeSpan;
        public string Direction;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }

    public class _temporaryarea_realtime_info : IRealTime
    {
        public string BankID;
        public string Status;
        public string HasCar;
        public string FaultCode;
        public string CarID;
        public string Targetlayer;
        public string TimeSpan;
        public TimeSpan GetTime()
        {
            return System.TimeSpan.FromSeconds(double.Parse(this.TimeSpan));
        }
    }


    //数据调度器，所有数据读取程序只用将数据读取后解码，通过调度器，可以发送到目标位置。
    class DataDispatcher
    {
        Dictionary<Type, List<ISendDataTarget>> targets = new Dictionary<Type, List<ISendDataTarget>>();
        public void RegistTarget(ISendDataTarget target)
        {
            var t = target.DataType;
            if (targets.ContainsKey(t))
            {
                var kk = targets[t];
                if (kk.Contains(target)==false)
                {
                    kk.Add(target);
                }
            }
            else
            {
                targets.Add(t, new List<ISendDataTarget>(new ISendDataTarget[] { target }));
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <typeparam name="T">数据泛型</typeparam>
        /// <param name="data">数据内容</param>
        /// <param name="time">发送时间</param>
        /// <param name="IsBroadcast">是广播，广播会发给所有目标，否则有接受者就结束</param>
        public void PushData<T>(T data,DateTime? time,bool IsBroadcast)
        {
            List<ISendDataTarget> tg = null;
            if (targets.TryGetValue(typeof(T), out tg))
            {
                foreach(var e in tg)
                {
                    if (IsBroadcast)
                    {
                        e.TrySend(data, time);
                    }
                    else
                    {
                        if (e.TrySend(data, time))
                        {
                            return;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <typeparam name="T">数据泛型</typeparam>
        /// <param name="data">数据内容</param>
        /// <param name="time">发送时间</param>
        /// <param name="IsBroadcast">是广播，广播会发给所有目标，否则有接受者就结束</param>
        public void PushData<T>(T[] data, DateTime? time, bool IsBroadcast)
        {
            List<ISendDataTarget> tg = null;
            if (targets.TryGetValue(typeof(T), out tg))
            {
                foreach (var k in data)
                {
                    foreach (var e in tg)
                    {
                        if (IsBroadcast)
                        {
                            e.TrySend(k, time);
                        }
                        else
                        {
                            if (e.TrySend(k, time))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    //数据发送目标对象，可以注册在调度器上，凡是
    interface ISendDataTarget
    {
        bool TrySend(object data,DateTime? time=null);
        Type DataType { get; }
    }

 

}
