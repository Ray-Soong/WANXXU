using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Tools
{




    public class BackgroundProcess : MonoBehaviour
    {
        bool first = true;
        ProcessStartInfo info;
        Process prcess;
        /// <summary>
        /// Exe文件路径
        /// </summary>
        public string ExePath;
        //进程启动参数
        public string ExeArguments;
        //开机启动
        public bool StartOpen = true;
        //开机启动延迟
        public float StartOpenDelay = 0;
        //进程自动复活
        public bool AutoAlive;

        public float AutoAliveDelay = 10;


        bool open()
        {
            try
            {
                if (System.IO.Path.GetExtension(this.ExePath).ToLower() != ".exe")
                {
                    return false;
                }
                if (!System.IO.File.Exists(this.ExePath))
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
            ProcessStartInfo info = new ProcessStartInfo(ExePath);
            if (ExeArguments != null && ExeArguments != "")
                info.Arguments = ExeArguments;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            this.prcess = Process.Start(info);
            return this.prcess != null;
        }
        int alvstep = 0;
        float alvdelay = 0;
        float time = 0;
        public bool alive = false;

        private void Update()
        {

            if (this.prcess != null)
            {
                if (this.prcess.HasExited)
                {
                    alive = false;
                    this.prcess = null;
                    alvstep = 1;
                    alvdelay = 0;
                }
                else
                {
                    alive = true;
                }
            }
            if (first == true && this.StartOpen)
            {
                if (Time.time > this.StartOpenDelay)
                {
                    open();
                    if (this.prcess != null)
                    {
                        first = false;
                    }
                }
            }
            if (this.AutoAlive && this.prcess == null && this.first == false)
            {

                switch (alvstep)
                {
                    case 1:
                        alvdelay += Time.deltaTime;
                        float f = this.AutoAliveDelay;
                        if (f <= 1)
                            f = 10;
                        if (alvdelay > 10)
                        {
                            if (open())
                            {
                                alvstep = 0;
                            }

                        }
                        break;
                    default:
                        break;
                }
            }


        }


        public void Kill()
        {
            if (this.prcess != null)
            {
                if (this.prcess.HasExited)
                {
                    this.prcess.Kill();
                }
            }
        }

    }
}
