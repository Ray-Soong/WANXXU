using Assets.Script.Datas;
using Assets.Script.Motion;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script
{

    class OutTask
    {
        public task_realtime_info info;
        public string pallet;
        public int status;
        public int level;
        public int bank;
    }
    class PalletManager
    {
        List<Cell> Cells = new List<Cell>();
        public string Url = "http://127.0.0.1:8987/";
        Cells update()
        {
            return G.HttpJsonGet<Cells>(this.Url);

        }
        int updatecount = 0;
        float updatecelltime = -1;
        void updateCellLoop()
        {
            if (updatecelltime < 0)
            {
                updatecelltime = 0;
                this.updateCell();
            }
            updatecelltime += Time.deltaTime;
            if (updatecount == 0)
            {
                if (updatecelltime > 60)
                {
                    updatecelltime = 0;
                    this.updateCell();
                }
            }
            else
            {
                if (updatecelltime > 60 * 30)
                {
                    updatecelltime = 0;
                    this.updateCell();
                }
            }
        }
        async void updateCell()
        {
            if (Url == null)
                return;
            try
            {
                var a = await Task<Cells>.Run(update);
                if (a != null)
                {
                    if (a.Content != null)
                    {
                        this.cellcontents = a.Content.ToArray();
                        this.loadstep = 1;
                        this.index = 0;
                        if (a.Content.Length > 0)
                        {
                            this.updatecount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
        }
        public Cell GetCell(string name)
        {
            foreach (var e in Cells)
            {
                if (e.name == name)
                {
                    return e;
                }
            }
            Vector3 p;
            if (GetPosition(name, out p))
            {
                var pp = new Cell() { name = name, pos = p };
                Cells.Add(pp);
                return pp;
            }
            return null;
        }
        bool GetPosition(string pos, out Vector3 p)
        {
            p = new Vector3();
            if (pos == null || pos == "")
                return false;

            var ss = pos.Trim().Split('-');
            if (ss.Length != 3)
            {
                return false;
            }
            int f = 0, h = 0, r = 0;
            int xd = 1;
            try
            {
                xd = int.Parse(ss[0]);
                f = int.Parse(ss[1]);
                int k = int.Parse(ss[2]);
                r = k / 1000;
                h = k % 100;
            }
            catch (Exception e)
            {
                return false;
            }
            var y = Store.store.GetFloor(xd, f);
            var z = Store.store.GetHole(xd, f, h);
            var x = Store.store.GetRow(xd, f, r);
            p = new Vector3(x, y, z);

            return true;
        }

        List<Pallet> delays = new List<Pallet>();
        public void DelayDelete(Pallet p)
        {
            if (delays.Contains(p) == false)
            {
                delays.Add(p);
            }
        }
        public void ClearDelay()
        {
            if (delays.Count > 0)
            {

                foreach (var e in this.delays)
                {
                    this.Delete(e);
                }
                delays.Clear();
            }
        }
        List<Pallet> cellpls = new List<Pallet>();
        CellContent[] cellcontents;
        int loadstep = 0;
        int index = 0;
        public void InitPallet(CellContent[] contents)
        {
            loadstep = 1;
        }


        public void PopCell(Pallet p)
        {
            this.cellpls.Remove(p);
            if (!this.pallets.Contains(p))
            {
                this.pallets.Add(p);
            }
        }

        public void SetCellPosition(string cell, string pallet, bool New = false)
        {
            var a = this.FindPallet(pallet);
            var c = this.GetCell(cell);
            if (c == null)
                return;
            if (a == null && New == false)
                return;
            if (a == null)
                a = NewPallet(pallet);

            if (a.current == null)
            {
                a.CellPosition = cell;
                this.pallets.Remove(a);
                if (!cellpls.Contains(a))
                {
                    cellpls.Add(a);
                }
                a.transform.position = c.pos;
            }

        }


        double tm = 0;
        void UpdateCell()
        {
            switch (loadstep)
            {
                case 1:
                    tm += Time.deltaTime;
                    if (tm < 0.2)
                        return;
                    tm = 0;
                    int end = index + 400;
                    if (end > this.cellcontents.Length)
                    {
                        end = this.cellcontents.Length;
                    }
                    for (int i = index; i < end; i++)
                    {
                        var p = this.cellcontents[i];
                        this.SetCellPosition(p.name, p.Nothing ? null : p.pallet, true);
                    }
                    index = end;
                    if (index >= this.cellcontents.Length)
                        loadstep = 0;
                    break;
                default:
                    break;
            }
        }

        bool loadfirst = true;
        public void Loop()
        {
            if (loadfirst == false)
            {
                loadfirst = false;
                var ss = JObject.Parse(System.IO.File.ReadAllText(@"D:\test\abc.txt"));
                var d = ss.ToObject<Cells>();
                this.loadstep = 1;
                this.cellcontents = d.Content.ToArray();
            }
            try
            {
                foreach (var e in pallets)
                {
                    e.Error();
                }
                this.ClearDelay();
            }
            catch
            {

            }
            updateCellLoop();
            UpdateCell();
        }
        //各种类型料箱的模版
        Dictionary<string, GameObject> orgins = new Dictionary<string, GameObject>();

        List<Pallet> pallets = new List<Pallet>();

        public string DefaultType = "default";

        List<task> tasks = new List<task>();

        public class task
        {
            public task_realtime_info data;
            public string taskid;

            public void update()
            {

            }
        }




        public task GetTask(string id)
        {
            foreach (var e in tasks)
            {
                if (e.taskid == id)
                    return e;
            }
            return null;
        }

        public void Push(task_realtime_info info)
        {
            foreach (var e in this.tasks)
            {
                if (e.taskid == info.ContainerCode)
                {
                    e.data = info;
                    e.update();
                    return;
                }
            }
            task t = new task() { data = info, taskid = info.ContainerCode };
            this.tasks.Add(t);
            t.update();
        }


        public void Delete(Pallet p)
        {
            pallets.Remove(p);
            cellpls.Remove(p);
            p.BeforeDelete();
            p.removed = true;
            GameObject.Destroy(p.gameObject);
        }

        //添加料箱模版
        public void Add(GameObject orgin)
        {
            var p = orgin.GetComponent<Pallet>();
            var type = p.TypeName;
            orgins.Add(type, orgin);
        }
        public Pallet NewPallet(string code, string type = null)
        {
            type = type == null ? DefaultType : type;
            var gm = orgins[type];
            var rt = GameObject.Instantiate(gm);
            rt.name = code;
            var p = rt.GetComponent<Pallet>();
            p.name = code;
            pallets.Add(p);
            return p;

        }
        public Pallet FindPallet(string code)
        {
            foreach (var e in pallets)
            {
                if (e.name == code)
                    return e;
            }
            foreach (var e in this.cellpls)
            {
                if (e.name == code)
                    return e;
            }
            return null;
        }


        public IEnumerable<Pallet> Pallets { get { return pallets; } }

    }


    public class CellContent
    {
        public string name;
        public string pallet;
        public bool Nothing;
    }


    class Cells
    {
        public CellContent[] Content;
        public string Message;
    }
    class Cell
    {
        public string name;
        public Vector3 pos;
        public Pallet pallet;
    }
}
