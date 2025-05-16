using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script
{
    /// <summary>
    /// 路线设置
    /// </summary>
    [ExecuteInEditMode]
    class WayView : MonoBehaviour
    {
       string[] ss = new string[] {
            "#","h-3-2","t-2-8-1","t-2-8-2",
"#","h-3-2","t-2-7-1","t-2-7-2",
"#","h-3-2","t-2-6-1","t-2-6-2",
"#","h-3-2","t-2-5-1","t-2-5-2",
"#","h-3-2","t-2-4-1","t-2-4-2",
"#","h-3-2","t-2-3-1","t-2-3-2",
"#","h-3-2","t-2-2-1","t-2-2-2",
"#","h-3-2","t-2-1-1","t-2-1-2",

"#","h-2-2","t-1-8-1","t-1-8-2",
"#","h-2-2","t-1-7-1","t-1-7-2",
"#","h-2-2","t-1-6-1","t-1-6-2",
"#","h-2-2","t-1-5-1","t-1-5-2",
"#","h-2-2","t-1-4-1","t-1-4-2",
"#","h-2-2","t-1-3-1","t-1-3-2",
"#","h-2-2","t-1-2-1","t-1-2-2",
"#","h-2-2","t-1-1-1","t-1-1-2",

"","i-2-2-1","i-2-2-2","h-3-1",
"#","h-3-1","h-3-2",

"","h-3-1","i-2-1-2",

"","i-1-2-1","i-1-2-2","h-2-1",
"#","h-2-1","h-2-2",
"","h-2-1","i-1-1-2","i-1-1-1",
"","01001","h-1-1","01032"

        };

        bool bb = false;

        bool created;
        public bool Created { get { return created; } }

        List<WayPoint> allpoints = new List<WayPoint>();
        List<WayPoint> points = new List<WayPoint>();
        public WayInfo[] Ways;
        public bool EditUpdateFlag = false;
        MeshFilter f;
        MeshRenderer rd;
        public float width;
        private void Start()
        {
            EditUpdateFlag = true;
            f = this.gameObject.GetComponent<MeshFilter>();
            rd = this.gameObject.GetComponent<MeshRenderer>();
            if (rd == null)
                rd = this.gameObject.AddComponent<MeshRenderer>();
            if (f == null)
                f = this.gameObject.AddComponent<MeshFilter>();
        }

        public void RegistPoint(WayPoint p)
        {
            if (this.allpoints.Contains(p) == false)
            {
                this.allpoints.Add(p);
            }
        }
        public void CreateNet()
        {
            List<WayPoint> pts = new List<WayPoint>();
            this.points = pts;
            // this.allpoints = this.points;
            foreach (var e in this.Ways)
            {
                foreach (var k in e.Points)
                {
                    pts.Add(k);
                }

            }
            foreach (var e in pts)
            {
                e.Outputs.Clear();
            }
            foreach (var e in this.Ways)
            {
                WayPoint first = null;
                for (int i = 0; i < e.Points.Length; i++)
                {
                    var pp = e.Points[i];
                    pp.Way = e.Number;
                    pp.WayIndex = i + 1;
                    if (first == null)
                    {
                        first = pp;
                    }
                    else
                    {
                        bool nnn = false;
                        foreach(var er in first.Outputs)
                        {
                            if (er.Point == pp)
                            {
                                nnn = true;
                                first = pp;
                                break;
                            }
                        }
                        if (nnn)
                            continue;

                        NextPointInfo ppp = new NextPointInfo()
                        {
                            IsWayEnd = i == e.Points.Length - 1,
                            IsWayStart = i == 0,
                            WayName = e.Name,
                            Way = e,
                            Point=pp
                        };
                        
                        first.Outputs.Add(ppp);
                        first = pp;
                    }
                }
            }
            if (G.ShowPoint == false)
            {
                foreach(var e in this.allpoints)
                {
                    e.hide();
                }
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            created = true;


        }
        class MeshData
        {
            public List<Vector3> Point = new List<Vector3>();
            public List<int> ints = new List<int>();
            public List<Vector3> normals = new List<Vector3>();
            public List<Vector2> uv = new List<Vector2>();
            public int index;

            public void PushSegment(Vector3[] points)
            {
                Point.AddRange(points);
                int k = index;
                ints.AddRange(new int[]{
                   k+0,
                   k+1,
                   k+2,
                   k+3,
                   k+2,
                   k+1
                }
                    );
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(0, 1));
                index += 4;
            }

            Vector3 rotate(Vector3 v, float angle)
            {
                var l = v.magnitude;
                var k = Mathf.Cos(angle * Mathf.PI / 180);
                v.y += Mathf.Sin(angle * Mathf.PI / 180) * l;
                v.x *= k;
                v.z *= k;
                return v;
            }

            public void AddLine(Vector3 p1, Vector3 p2, float width, bool End = false)
            {
                //    var p2 = p1;
                //   var a = angle * Mathf.PI / 180;
                float caplen = width * 2;
                float length = (p2 - p1).magnitude;

                Vector2 v2 = new Vector2(p2.x, p2.z);
                Vector2 v1 = new Vector2(p1.x, p1.z);
                var tmp = (v2 - v1);
                var tmp1 = new Vector2(tmp.y, -tmp.x).normalized * width;
                float xx = -tmp1.x / 2;
                float yy = -tmp1.y / 2;
                Vector3 pp2 = p2;
                if (End)
                {
                    Vector3 per = (p2 - p1).normalized;
                    length = length - width;
                    pp2 = p1 + per * length;
                }
                var point = new Vector3[4];
                point[1] = point[0] = p1;
                point[0].x -= xx;
                point[0].z -= yy;
                point[1].x += xx;
                point[1].z += yy;

                point[3] = point[2] = pp2;
                point[2].x -= xx;
                point[2].z -= yy;
                point[3].x += xx;
                point[3].z += yy;

                PushSegment(point);
                if (End == false)
                    return;
                Vector3[] ap = new Vector3[3];
                ap[0] = ap[1] = pp2;
                ap[0].x -= xx * 2;
                ap[0].z -= yy * 2;
                ap[1].x += xx * 2;
                ap[1].z += yy * 2;
                ap[2] = p2;
                this.Point.AddRange(ap);
                int k = index;
                ints.AddRange(new int[]{
                   k+0,
                   k+1,
                   k+2
                }
                   );
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(1, 1));
                index += 3;

            }

            public void End()
            {

            }
        }
        void reload()
        {
            List<WayInfo> infos = new List<WayInfo>();
            infos.AddRange(this.Ways);
            infos.RemoveAll((a) => { return a.AddFlag == true; });
            List<WayPoint> wp = new List<WayPoint>();
            bool both = false;
            foreach(var e in ss)
            {
                if (e == "" || e=="#")
                {
                    if (wp.Count>0)
                    {
                        WayInfo  ifo = new WayInfo()
                        {
                             AddFlag=true,
                              BothWay=both,
                               Points=wp.ToArray()
                            
                        };
                        infos.Add(ifo);
                        wp.Clear();
                    }
                    both = e == "#";
                }
                else
                {
                    var pp = this.FindPoint(e);
                    if (pp == null)
                        throw new Exception("错误名称");
                    wp.Add(pp);
                }
            }
            if (wp.Count > 0)
            {
                WayInfo ifo = new WayInfo()
                {
                    AddFlag = true,
                    BothWay = both,
                    Points = wp.ToArray()

                };
                infos.Add(ifo);
                wp.Clear();
            }
            this.Ways = infos.ToArray();
        }

        public bool reloadflag;


        public void Update()
        {
            if (reloadflag == true)
            {
                reloadflag = false;
                reload();
                return;
            }
            if (EditUpdateFlag)
            {
               // reload();
                EditUpdateFlag = false;
                MeshData md = new MeshData();
                foreach(var e in Ways)
                {
                    WayPoint first = null;
                    if (e.Points != null)
                    {
                        for (int i = 0; i < e.Points.Length ; i++)
                        {
                            var p = e.Points[i];
                            if (p == null)
                            {
                                first = null;
                                continue;
                            }
                            p.Way = e.Number;
                            p.WayIndex = i + 1;
                            if (first == null)
                            {
                                first = p;
                            }
                            else
                            {
                                var p1 = first.transform.position;
                                var p2 = p.transform.position;
                                p1.y += 0.01f;
                                p2.y += 0.01f;
                                md.AddLine(p1,p2, width, true);
                                first = p;
                            }
                        }
                    }
                }
                var mesh = new Mesh();
                mesh.vertices = md.Point.ToArray();
                mesh.uv = md.uv.ToArray();
                mesh.normals = md.normals.ToArray();
                mesh.triangles = md.ints.ToArray();
                f.mesh = null;
                f.mesh = mesh;
            }
        }

        public IEnumerable<WayPoint> Points { get { return this.allpoints; } }
        public WayPoint FindPoint(string code)
        {
            foreach(var e in allpoints)
            {
                if (e.name.Trim() == code)
                    return e;
            }
            return null;
        }
        static bool find(WayPoint curr, WayPoint to, List<WayPoint> ps, int maxdepth)
        {
            if (curr == to)
            {
                ps.Add(curr);
                return true;
            }
            int c = ps.Count;
            if (c < maxdepth)
            {
                ps.Add(curr);

                if (curr.Outputs != null)
                {
                    foreach (var e in curr.Outputs)
                    {
                        var b = find(e.Point, to, ps, maxdepth);
                        if (b)
                            return true;
                    }
                }
            }
            while (ps.Count > c)
                ps.RemoveAt(ps.Count - 1);
            return false;
        }

        public static List<WayPoint> FindWay(WayPoint from, WayPoint to, int maxdepth)
        {
            List<WayPoint> ps = new List<WayPoint>();
            if (find(from, to, ps, maxdepth))
            {
                return ps;
            }
            return null;
        }


    }


    [Serializable]
    public class WayInfo
    {
        public WayPoint[] Points;
        public int Number;
        public string Name;
        //双向路
        public bool BothWay;
        //双向路的反向名称
        public string BackName;
        public string[] StringNames;
        public bool AddFlag;
    }




     





    
}
