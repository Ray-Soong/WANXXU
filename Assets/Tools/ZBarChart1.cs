using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
//using XCharts.Editor;
using XCharts.Runtime;

namespace Assets.Tools
{
    public class ZBarChart1:MonoBehaviour
    {

        public ZPallete Pallete;
        
        Transform template;
        public string TextBinding;
        public string ValueBinding;
        public double ValueRate = 0.0001;
        public double ValueOffset = 0;
        public string ValueFormat = "0.00";
        public string TextFormat = "{0}   {1}万元";
        object o;

        class item
        {
            public Transform owner;
            public Text text;
            public Image Image;
            public double value;
            public bool show;
            public bool ByColor;
            public double width;
        }


        List<item> datas = new List<item>();

        void clear(int c)
        {
            for(int i=0;i<datas.Count;i++)
            {
                datas[i].owner.parent = null;
                datas[i].owner.gameObject.SetActive(false);
                datas[i].show = false;
            }
        }

        item GetByIndex(int index)
        {
            if (index < 0)
                return null;
            if (index < datas.Count)
            {
                return datas[index];
            }
            if (index == datas.Count)
            {
                var g= GameObject.Instantiate(template.gameObject);
                item itm = new item()
                {
                     owner=g.transform,
                     Image=G.FindChild(g.transform,"Image").GetComponent<Image>(),
                     text=G.FindChild(g.transform,"Text").GetComponent<Text>(),
                      value=0
                };
                if (this.Pallete != null)
                {
                    if (this.Pallete.Items != null)
                    {
                        if (this.Pallete.Items.Length > 0)
                        {
                            var p= this.Pallete.Items[index % this.Pallete.Items.Length];
                            if (p.Sprite != null)
                            {
                                itm.Image.sprite = p.Sprite;
                                itm.Image.type = Image.Type.Filled;
                                itm.Image.fillOrigin = 0;
                                itm.Image.fillMethod = Image.FillMethod.Horizontal;
                                itm.ByColor = false; 
                            }
                            else
                            {
                                itm.Image.color = p.Color;
                                itm.ByColor = true;
                                var d= itm.owner.GetComponent<RectTransform>();
                                itm.width= d.sizeDelta.x;

                                //itm.width = itm.owner.GetComponent<RectTransform>();
                            }
                          
                            
                           
                        }
                    }
                }
                datas.Add(itm);
                return itm;
            }
            return null;
        }


        private void updateView()
        {
            int c = 0;
            if(o is System.Data.DataTable)
            {
                var tb =o as System.Data.DataTable;
                clear(tb.Rows.Count);
                double max = double.MinValue;
                for(int i = 0; i < tb.Rows.Count; i++)
                {
                    string txt = "";
                    double vl = 0;
                    var v1 = tb.Rows[i][this.TextBinding];
                    var v2 = tb.Rows[i][this.ValueBinding];
                    if(v1!=null && v1 != DBNull.Value)
                    {
                        txt = v1.ToString();
                    }
                    if (v2 != null && v2 != DBNull.Value)
                    {
                        vl = (double)System.Convert.ChangeType(v2, typeof(double));
                    }
                    var itm = GetByIndex(i);
                   
                    itm.value = vl * this.ValueRate + this.ValueOffset;
                    if (itm.value > max)
                    {
                        max = itm.value;
                    }
                    itm.text.text = string.Format(this.TextFormat, txt, itm.value.ToString(this.ValueFormat));
                }
                int cc = tb.Rows.Count;
                for(int i = 0; i < cc; i++)
                {
                    var itm = this.datas[i];
                    var v = itm.value;
                    if (v <= 0)
                        v = 0;
                    if (max > 0)
                    {
                        v = v / max;
                    }
                  //  if (itm.show == false)
                    {

                        if (itm.ByColor)
                        {
                            var p = itm.Image.GetComponent<RectTransform>();
                            var sz = p.sizeDelta;
                            var pp = (float)(itm.width * v);
                            sz.x = pp;

                            p.sizeDelta = sz;
                        }
                        else
                        {
                            itm.Image.fillAmount = (float)v;
                        }
                      //  p.sizeDelta = sz;
                       // p.anchorMax = sz;
                       // itm.Image.fillAmount = (float)v;
                        itm.owner.parent = this.transform;
                        itm.owner.gameObject.SetActive(true);
                        itm.show = true;
                    }

                }
            }
        }
        public object DataSource
        {
            get
            {
                return o;
            }
            set
            {
               if( o != value)
                {
                    o = value;
                    this.updateView();
                }
               
            }
        }

        System.Data.DataTable GetRandomTable(double rand_max,params string[] names)
        {
            System.Data.DataTable tab = new System.Data.DataTable();
            tab.Columns.Add(this.TextBinding,typeof(string));
            tab.Columns.Add(this.ValueBinding, typeof(double));
            System.Random random = new System.Random();
            foreach(var e in names)
            {
                var v= random.NextDouble() * rand_max;

                var r = tab.NewRow();
                r[0] = e;
                r[1] = v;
                tab.Rows.Add(r);
            }
            tab.AcceptChanges();
            return tab;

        }
        

        private void Start()
        {
            var p =template= this.gameObject.transform.GetChild(0);
            p.parent = null;
            p.gameObject.SetActive(false);
        }

        double tm = 9;
       
        



    }


    [Serializable]
    public struct ZPad
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
        public bool LeftIsRate;
        public bool TopIsRate;
        public bool RightIsRate;
        public bool BottomIsRate;


    }


    [Serializable]
    public class ZTextStyle
    {
        public Color32 Color;
        public int FontSize;
        public bool IsBold;
        
    }
    [Serializable]
    public class ZItemStyle
    {
        public ZPallete Pallete;
        public ZPalleteItem DefaultColor;
        public ZTextStyle TextStyle;
        public string TextFormat;
        public string NumberFormat;
        public float Size;
        public string TextBinding;
        public string ValueBinding;
        [NonSerialized]
        public Convert TextConvert;
        [NonSerialized]
        public Convert ValueConfert;
    }
    [Serializable]
    public class ZPallete
    {
        public ZPalleteItem[] Items;
    }
    [Serializable]
    public class ZPalleteItem
    {
        public Color32 Color;
        public Sprite Sprite;
    }







}
