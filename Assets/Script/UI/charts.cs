using Assets.Script;
using Assets.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class charts : MonoBehaviour
{
    // Start is called before the first frame update
    public LineChart line1;
    public LineChart line2;
    public BarChart line3;
    public LineChart bar;

    public LineChart _line1;
    public LineChart _line2;
   // public LineChart line3;
    public LineChart _bar;




    public ZBarChart1 barchart;
    public PieChart pie;
    public Button CloseButton;
    public Text clock;
    public Text date;
    Transform t1, t2, t3, t4, t5;
    public Button DevButton;
    public Button ChartButton;
    public Button ErrorButton;
    public GameObject DevPanel1;
    public GameObject DevPanel2;
    public GameObject ErrorPanel;
    public DataView CurrentError;

    public Button YB1, YB2, YB3;
    public Button MB1, MB2, MB3;
    public Button DB1, DB2, DB3;

    public Button _YB1, _YB2, _YB3;
    public Button _MB1, _MB2, _MB3;
    public Button _DB1, _DB2, _DB3;
    

    public SwitchPositionScript Camera;
    public Button cam1, cam2, cam3;

    Button[] btg1, btg2, btg3;
    void InitButton()
    {
        YB1.onClick.AddListener(yb1);
        YB2.onClick.AddListener(yb2);
        YB3.onClick.AddListener(yb3);
        MB1.onClick.AddListener(mb1);
        MB2.onClick.AddListener(mb2);
        MB3.onClick.AddListener(mb3);
        DB1.onClick.AddListener(db1);
        DB2.onClick.AddListener(db2);
        DB3.onClick.AddListener(db3);

        _YB1.onClick.AddListener(yb1);
        _YB2.onClick.AddListener(yb2);
        _YB3.onClick.AddListener(yb3);
        _MB1.onClick.AddListener(mb1);
        _MB2.onClick.AddListener(mb2);
        _MB3.onClick.AddListener(mb3);
        _DB1.onClick.AddListener(db1);
        _DB2.onClick.AddListener(db2);
        _DB3.onClick.AddListener(db3);



        btg1 = new Button[] { YB1, MB1, DB1, _YB1, _MB1, _DB1 };
        btg2 = new Button[] { YB2, MB2, DB2, _YB2, _MB2, _DB2 };
        btg3 = new Button[] { YB3, MB3, DB3, _YB3, _MB3, _DB3 };
        cam1.onClick.AddListener(Cm1);
        cam2.onClick.AddListener(Cm2);
        cam3.onClick.AddListener(Cm3);
    }

    



    TChart tc1_y, tc1_m, tc1_d;
    TChart tc2_y, tc2_m, tc2_d;
    TChart tc3_y, tc3_m, tc3_d;

    TChart _tc1_y, _tc1_m, _tc1_d;
    TChart _tc2_y, _tc2_m, _tc2_d;
    TChart _tc3_y, _tc3_m, _tc3_d;
    TChart zchart;




    class charttab
    {
        public string sql;
        public string tbname;
        public System.Data.DataTable table;
        public bool dirty;
        public string[] HistoryField;
        public string HistoryFile;
        public int KeepCount;
        public int tick;
        public int lasttck=-1;
        public bool updateok;
        bool loaded = false;
        public TChart BigChart;

        public void Load()
        {
            try
            {
                System.Data.DataTable tab = null;
                if (this.table == null)
                {
                    tab = new System.Data.DataTable();
                    foreach (var e in HistoryField)
                        tab.Columns.Add(e, typeof(string));
                    this.table = tab;
                }
                else
                    tab = this.table;

                if (System.IO.File.Exists(this.HistoryFile))
                {
                    var f = System.IO.File.ReadAllText(HistoryFile);
                    var ff = f.Split('вс');
                    foreach (var e in ff)
                    {
                        var ss = e.Split('в█');
                        var r = tab.NewRow();
                        for (int i = 0; i < tab.Columns.Count; i++)
                        {

                            r[i] = ss[i];
                        }
                        tab.Rows.Add(r);
                    }
                    tab.AcceptChanges();
                    //            return tab;
                }
            }
            catch
            {

            }
    
        }

        public void Save(System.Data.DataTable tab)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(DataRow e in tab.Rows)
            {

                if (first == false)
                {
                    sb.Append('вс');
                }
                else
                    first = false;
                bool ff = true;
                foreach(var k in this.HistoryField)
                {
                    if (ff == false)
                    {
                        sb.Append('в█'); 
                    }
                    else
                        ff = false;
                    var v = e[k];
                    sb.Append((v == DBNull.Value || v == null) ? "" : v.ToString());
                }
            }
            try
            {
                System.IO.File.WriteAllText(this.HistoryFile, sb.ToString());
            }
            catch(Exception e)
            {

            }
        }
        public charttab(string name)
        {
            this.tbname = name;
        }



        bool SameDay(DataRow r,DataTable tb)
        {
            if (tb.Rows.Count == 0)
                return false;
            var rr = tb.Rows[tb.Rows.Count - 1];
            if (rr[this.HistoryField[0]].Equals(r[this.HistoryField[0]]))
            {
                return true;
            }
            return false;
        }

        public void update(TChart chart, DateTime time, int add, string format, int count)
        {
            var tab = chart.RandomDataSource(time, TimeSpan.FromDays(add), format, count, 0, 100);
            this.table = tab;
            dirty = true;
        }

        public void update()
        {
            var sqll = string.Format("SELECT * FROM {0}", tbname);
            if (sql != null)
            {
                sqll = this.sql;
            }
            var tab= G.Query(sqll);
            if (HistoryFile == null)
            {
                this.table = tab;
                dirty = true;
            }
            else
            {
                bool save = false;
                if (tab.Rows.Count > 0)
                {
                    {
                        var r = tab.Rows[0];
                        if (this.SameDay(r,this.table))
                        {
                            var row = this.table.Rows[this.table.Rows.Count - 1];
                            bool same = true;
                            foreach(var e in this.HistoryField)
                            {
                                var a= row[e];
                                var b = r[e];
                                if (a.Equals(b) == false)
                                {
                                    same = false;
                                    row[e] = b;
                                }
                            }
                            if (same == false)
                            {
                                save = true;
                            }
                        }
                        else
                        {
                            var row = this.table.NewRow();
                            foreach (var e in this.HistoryField)
                            {
                                row[e] = r[e];
                            }
                            this.table.Rows.Add(row);
                            
                            save= true;
                        }
                    }
                    if (this.KeepCount <= 0)
                        this.KeepCount = 7;
                    if (this.table.Rows.Count > this.KeepCount)
                    {
                        this.table.Rows.RemoveAt(0);
                        save = true;
                    }
                    this.table.AcceptChanges();
                    if (save)
                    {
                        this.table.AcceptChanges();
                        this.Save(this.table);
                    }
                }
            }
            this.tick++;
        }

        public void updateView(TChart chart)
        {
            chart.SetDataSource(this.table);
            chart.Update();
        }
    }

    object JEcov(object a,object v)
    {
        var p= (double)System.Convert.ChangeType(a,typeof(double));
        p= p / 10000;
        return p;
    }


    void initchart()
    {
        tc1_y = new TChart(bar);
        tc1_y.SetXAxisBinding("THISYEAR");
        tc1_y.SetSeriesBinding(0, "INSNUM",cv,null);
        tc1_y.SetSeriesBinding(1, "OUTNUM",cv, null);
       
        _tc1_y = new TChart(_bar);
        _tc1_y.SetXAxisBinding("THISYEAR");
        _tc1_y.SetSeriesBinding(0, "INSNUM", cv, null);
        _tc1_y.SetSeriesBinding(1, "OUTNUM", cv, null);
        tc1_y.Tag = new charttab("VIEW_ORDER_NUM_YEARS")
        {
            BigChart = _tc1_y
        };
        var date_cv = new Assets.Tools.Convert((aa, bb) =>
        {
            var s = aa as string;
            if (s == null)
                return null;
            var pp = s.Split('-');
            if (pp.Length > 1)
                return pp[1];
            return pp[0];
        }
        );

        tc1_m = new TChart(bar);
        tc1_m.SetXAxisBinding("THISDATE",date_cv);
        tc1_m.SetSeriesBinding(0, "INSNUM",cv, null);
        tc1_m.SetSeriesBinding(1, "OUTNUM",cv, null);
      
        _tc1_m = new TChart(_bar);
        _tc1_m.SetXAxisBinding("THISDATE",date_cv);
        _tc1_m.SetSeriesBinding(0, "INSNUM", cv, null);
        _tc1_m.SetSeriesBinding(1, "OUTNUM", cv, null);
        tc1_m.Tag = new charttab("VIEW_ORDER_NUM_MONTHS")
        {
            BigChart = _tc1_m
        };

        tc1_d = new TChart(bar);
        tc1_d.SetXAxisBinding("INSDATE");
        tc1_d.SetSeriesBinding(0, "INSNUM", cv, null);
        tc1_d.SetSeriesBinding(1, "OUTNUM", cv, null);
        _tc1_d = new TChart(_bar);
        _tc1_d.SetXAxisBinding("INSDATE");
        _tc1_d.SetSeriesBinding(0, "INSNUM", cv, null);
        _tc1_d.SetSeriesBinding(1, "OUTNUM", cv, null);
        var a  = new charttab("view_order_num")
        {
            HistoryFile = @".\order_num1.txt",
            HistoryField = new string[] { "INSDATE", "INSNUM", "OUTNUM" },
            BigChart=_tc1_d
        };
        a.Load();
        tc1_d.Tag = a;


        tc2_y = new TChart(line1);
        tc2_y.SetXAxisBinding("THISYEAR");
        tc2_y.SetSeriesBinding(0, "INSJE", this.JEcov, null);
        tc2_y.SetSeriesBinding(1, "OUTJE", this.JEcov, null);
        _tc2_y = new TChart(_line1);
        _tc2_y.SetXAxisBinding("THISYEAR");
        _tc2_y.SetSeriesBinding(0, "INSJE",this.JEcov, null);
        _tc2_y.SetSeriesBinding(1, "OUTJE", this.JEcov, null);

        tc2_y.Tag = new charttab("VIEW_STOCK_MONEY_YEARS") { 
        BigChart=_tc2_y
        };

        tc2_m = new TChart(line1);
        tc2_m.SetXAxisBinding("THISMONTH",date_cv);
        tc2_m.SetSeriesBinding(0, "INSJE", JEcov, null);
        tc2_m.SetSeriesBinding(1, "OUTJE", JEcov, null);
        _tc2_m = new TChart(_line1);
        _tc2_m.SetXAxisBinding("THISMONTH",date_cv);
        _tc2_m.SetSeriesBinding(0, "INSJE", JEcov, null);
        _tc2_m.SetSeriesBinding(1, "OUTJE", JEcov, null);
        tc2_m.Tag = new charttab("VIEW_STOCK_MONEY_MONTHS")
        {
            BigChart=_tc2_m
        };



        tc2_d = new TChart(line1);
        tc2_d.SetXAxisBinding("ISDATE");
        tc2_d.SetSeriesBinding(0, "INSJE", JEcov, null);
        tc2_d.SetSeriesBinding(1, "OUTJE", JEcov, null);
        _tc2_d = new TChart(_line1);
        _tc2_d.SetXAxisBinding("ISDATE");
        _tc2_d.SetSeriesBinding(0, "INSJE", JEcov, null);
        _tc2_d.SetSeriesBinding(1, "OUTJE", JEcov, null);
        a = new charttab("VIEW_STOCK_MONEY")
        {
            HistoryFile = @".\STOCK_MONEY1.txt",
            HistoryField = new string[] { "ISDATE", "INSJE", "OUTJE", "KCRATE" , "BYKCJE" },
            BigChart=_tc2_d
        };
        a.Load();
        tc2_d.Tag = a;

        tc3_y = new TChart(line2);
        tc3_y.SetXAxisBinding("THISYEAR");
        tc3_y.SetSeriesBinding(0, "INSQTY", cv, null);
        tc3_y.SetSeriesBinding(1, "OUTQTY", cv, null);
        _tc3_y = new TChart(_line2);
        _tc3_y.SetXAxisBinding("THISYEAR");
        _tc3_y.SetSeriesBinding(0, "INSQTY", cv, null);
        _tc3_y.SetSeriesBinding(1, "OUTQTY", cv, null);
        tc3_y.Tag = new charttab("VIEW_TAG_NUM_YEARS")
        {
            BigChart=_tc3_y
        };

        tc3_m = new TChart(line2);
        tc3_m.SetXAxisBinding("THISMONTH",date_cv);
        tc3_m.SetSeriesBinding(0, "INSQTY", cv, null);
        tc3_m.SetSeriesBinding(1, "OUTQTY", cv, null);
        _tc3_m = new TChart(_line2);
        _tc3_m.SetXAxisBinding("THISMONTH",date_cv);
        _tc3_m.SetSeriesBinding(0, "INSQTY", cv, null);
        _tc3_m.SetSeriesBinding(1, "OUTQTY", cv, null);
        tc3_m.Tag = new charttab("VIEW_TAG_NUM_MONTHS")
        {
            BigChart=_tc3_m
        };
        

        tc3_d =new TChart(line2);
        tc3_d.SetXAxisBinding("INSDATE");
        tc3_d.SetSeriesBinding(0, "INSQTY", cv, null);
        tc3_d.SetSeriesBinding(1, "OUTQTY", cv, null);
       
        _tc3_d = new TChart(_line2);
        _tc3_d.SetXAxisBinding("INSDATE");
        _tc3_d.SetSeriesBinding(0, "INSQTY", cv, null);
        _tc3_d.SetSeriesBinding(1, "OUTQTY", cv, null);
        tc3_d.Tag = new charttab("")
        {
            BigChart = _tc3_d,
            sql = "SELECT * FROM view_tag_num WHERE INSDATE >= to_char(SYSDATE - 7,'yyyy-mm-dd')"
        };


        zchart = new TChart(line3);
        zchart.AxisIsY = true;
        zchart.SetXAxisBinding("INDEPTNAME");
        zchart.SetSeriesBinding(0, "JE", this.JEcov, null);
        //zchart.SetDataSource((tc2_d.Tag as charttab).table);
        zchart.Tag = new charttab("view_dept_money");
        mb1();
        mb2();
        mb3();

    }

    object cv(object v,object p)
    {
        try
        {
            if (v is double)
                return v;
            if (v == null || v == DBNull.Value)
                return (double)0;
            if (v is string)
            {
                var s = v as string;
                if (s.Length > 0)
                {
                    if (s[s.Length - 1] == '%')
                    {
                        return double.Parse(s.Substring(0, s.Length - 1));
                    }
                }
                else
                    return (double)0;
            }
            return v.ToString();
        }
        catch(Exception e)
        {
            return (double)0;
        }
             
    }


    void Cm1()
    {
        Camera.SetPosition(0);
    }

    void Cm2()
    {
        Camera.SetPosition(1);
    }

    void Cm3()
    {
        Camera.SetPosition(2);
    }

    void FocusButton(Button[] group,Button focus,Button focus2)
    {
        foreach(var e in group)
        {
            e.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
        focus2.transform.GetChild(0).GetComponent<Text>().color=
        focus.transform.GetChild(0).GetComponent<Text>().color = Color.red;
    }

    void yb1()
    {
        FocusButton(btg1, YB1,_YB1);
        chart1 = tc1_y;
        updatechart(chart1);
    }

    void updatechart(TChart chart)
    {
        try
        {
            if (chart == null)
                return;
            charttab tav = chart.Tag as charttab;
            if (tav != null)
            {
                tav.updateView(chart);
                var d = tav.BigChart;
                d.SetDataSource(chart.DataSource);
                d.Update();
            }
             
        }
        catch
        {

        }
    }

    void yb2()
    {
        FocusButton(btg2, YB2,_YB2);
        chart2 = tc2_y;

        updatechart(chart2);
    }
    void yb3()
    {
        FocusButton(btg3, YB3,_YB3);
        chart3 = tc3_y;
        updatechart(chart3);
    }

    void mb1()
    {
        FocusButton(btg1, MB1,_MB1);
        chart1 = tc1_m;
        updatechart(chart1);
    }

    void mb2()
    {
        FocusButton(btg2, MB2,_MB2);
        chart2 = tc2_m;
        updatechart(chart2);
    }
    void mb3()
    {
        FocusButton(btg3, MB3,_MB3);
        chart3 = tc3_m;
        updatechart(chart3);
    }


    void db1()
    {
        FocusButton(btg1, DB1,_DB1);
        chart1 = tc1_d;
    
        updatechart(chart1);
    }
    void db2()
    {
        FocusButton(btg2, DB2,_DB2);
        chart2 = tc2_d;
        updatechart(chart2);
    }
    void db3()
    {
         FocusButton(btg3, DB3,_DB3);
        chart3 = tc3_d;
        updatechart(chart3);
    }
    void Start()
    {
        CloseButton.onClick.AddListener(close);
        t1 = line1.transform.parent;
        t2 = line2.transform.parent;
        t3 = line3.transform.parent;
        t4 = bar.transform.parent;
        t5 = pie.transform;
        ChartButton.onClick.AddListener(showchart);
        DevButton.onClick.AddListener(showdev);
        ErrorButton.onClick.AddListener(errshow);
        load();
        InitButton();
        initchart();
        bar.gameObject.AddComponent<Button>().onClick.AddListener(big1);
        line1.gameObject.AddComponent<Button>().onClick.AddListener(big2);
        line2.gameObject.AddComponent<Button>().onClick.AddListener(big3);
        _bar.gameObject.AddComponent<Button>().onClick.AddListener(_big1);
        _line1.gameObject.AddComponent<Button>().onClick.AddListener(_big2);
        _line2.gameObject.AddComponent<Button>().onClick.AddListener(_big3);
    }

    void big1()
    {
        showbig(1);
    }

    void big2()
    {
        showbig(2);
    }

    void big3()
    {
        showbig(3);
    }

    void _big1()
    {
        showbig(1,true);
    }

    void _big2()
    {
        showbig(2,true);
    }

    void _big3()
    {
        showbig(3,true);
    }

    void showbig(int num,bool little=false)
    {
        if (!little)
        {
            switch (num)
            {
                case 1:
                    bar.transform.parent.gameObject.SetActive(false);
                    _bar.transform.parent.gameObject.SetActive(true);
                    line1.transform.parent.gameObject.SetActive(true);
                    _line1.transform.parent.gameObject.SetActive(false);
                    line2.transform.parent.gameObject.SetActive(true);
                    _line2.transform.parent.gameObject.SetActive(false);
                    break;
                case 2:
                    bar.transform.parent.gameObject.SetActive(true);
                    _bar.transform.parent.gameObject.SetActive(false);
                    line1.transform.parent.gameObject.SetActive(false);
                    _line1.transform.parent.gameObject.SetActive(true);
                    line2.transform.parent.gameObject.SetActive(true);
                    _line2.transform.parent.gameObject.SetActive(false);
                    break;
                case 3:
                    bar.transform.parent.gameObject.SetActive(true);
                    _bar.transform.parent.gameObject.SetActive(false);
                    line1.transform.parent.gameObject.SetActive(true);
                    _line1.transform.parent.gameObject.SetActive(false);
                    line2.transform.parent.gameObject.SetActive(false);
                    _line2.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            bar.transform.parent.gameObject.SetActive(true);
            _bar.transform.parent.gameObject.SetActive(false);
            line1.transform.parent.gameObject.SetActive(true);
            _line1.transform.parent.gameObject.SetActive(false);
            line2.transform.parent.gameObject.SetActive(true);
            _line2.transform.parent.gameObject.SetActive(false);
        }
    }

   
    






    void errshow()
    {
        ShowChart(false);
        ShowDev(false);
        ShowError(true,true);
        var g= ErrorPanel.GetComponent<DataGrid>();
        g.DataSource = G.GetSceneScript().DeviceManager.history;
        g.UpdateView();

    }
    void showchart()
    {
        ShowChart(true,true);
        ShowDev(false);
        ShowError(false);
        
    }

    void showdev()
    {
        ShowChart(false);
        ShowError(false);
        ShowDev(true,true);
    }
    void ShowError(bool show, bool swt = false)
    {
        if (swt) show = ErrorPanel.activeSelf == false &&
                        CurrentError.gameObject.activeSelf == false;
        if (show)
        {
            if (CurrentError.DataContext != null)
            {
                CurrentError.gameObject.SetActive(true);
            }
            else
            ErrorPanel.SetActive(true);
        }
        else
        {
            CurrentError.gameObject.SetActive(false);
            ErrorPanel.SetActive(false);
        }

    }


    void ShowDev(bool show,bool swt=false)
    {
        if(swt)show = DevPanel1.activeSelf == false;
        if (show)
        {
            DevPanel1.SetActive(true);
        }
        else
        {
            DevPanel1.SetActive(false);
            DevPanel2.SetActive(false);
        }
    }



    System.Data.DataTable tb1;
    System.Data.DataTable tb2;
    System.Data.DataTable tb3;
    System.Data.DataTable tb4;

    bool busy = false;

    string[] ss = new string[] { "SELECT * FROM view_order_num", "SELECT * FROM view_box_num", "SELECT * FROM view_tag_num", "SELECT * FROM view_stock_money"

    };

    bool _select()
    {
        (tc1_y.Tag as charttab).update(tc1_y, DateTime.Now.AddDays(-365 * 5), 365, "yyyy", 6);
        (tc2_y.Tag as charttab).update(tc2_y, DateTime.Now.AddDays(-365 * 5), 365, "yyyy", 6);
        (tc3_y.Tag as charttab).update(tc3_y, DateTime.Now.AddDays(-365 * 5), 365, "yyyy", 6);
        (tc1_m.Tag as charttab).update(tc1_m, DateTime.Now.AddDays(-31 * 5), 31, "MM", 6);
        (tc2_m.Tag as charttab).update(tc2_m, DateTime.Now.AddDays(-31 * 5), 31, "MM", 6);
        (tc3_m.Tag as charttab).update(tc3_m, DateTime.Now.AddDays(-31 * 5), 31, "MM", 6);
        (tc1_d.Tag as charttab).update(tc1_d, DateTime.Now.AddDays(-7), 1, "MM-dd", 8);
        (tc2_d.Tag as charttab).update(tc2_d, DateTime.Now.AddDays(-7), 1, "MM-dd", 8);

        //var t2 = G.Query(ss[1]);
        tb2 = null;// t2;
        (tc3_d.Tag as charttab).update(tc3_d, DateTime.Now.AddDays(-7), 1, "MM-dd", 8);
        return true;
        //  (tc1_y.Tag as charttab).update();
    }



    bool select()
    {
        (tc1_y.Tag as charttab).update();
        (tc2_y.Tag as charttab).update();
        (tc3_y.Tag as charttab).update();
        (tc1_m.Tag as charttab).update();
        (tc2_m.Tag as charttab).update();
        (tc3_m.Tag as charttab).update();
        (tc1_d.Tag as charttab).update();
        (tc2_d.Tag as charttab).update();
        (zchart.Tag as charttab).update();
        var t2 = G.Query(ss[1]);
        tb2 = t2;
       (tc3_d.Tag as charttab).update();
        return true;
      //  (tc1_y.Tag as charttab).update();
    }

  
    void uuuu()
    {
        var ct1 = chart1;
        var ct2 = chart2;
        var ct3 = chart3;
        var ct1_h = tc1_d;
        var ct2_h = tc2_d;
        TChart[] tcs = new TChart[] { ct1, ct2, ct3, ct1_h, ct2_h };
    }

    bool _select(TChart[] tcs)
    {
        foreach(var e in tcs)
        {
            try
            {
                if (e != null)
                {
                    
                    var tb = e.Tag as charttab;
                    tb.updateok = false;
                    if (tb != null)
                        tb.update();
                }
            }
            catch(Exception er)
            {

            }
        }
        var t2 = G.Query(ss[1]);
     

       // tb1 = t1;
        tb2 = t2;
       // tb3 = t3;
       // tb4 = t4;
        return true;
    }

    float asfloat(object o)
    {
        if(o==null || o == DBNull.Value)
        {
            return 0;
        }
        try
        {
           // return (float)Convert.ChangeType(o, typeof(float));
        }
        catch
        {

        }
        return 0;
    }

    int lastl1c = 0;


    int cc = 1;
    int kk = 0;
    
    class datas
    {
        public string time;
        public double d1;
        public double d2;
        public double d3;
        public double d4;
    }


    class hdatas
    {
        public datas[] datas;
    }

    List<datas> dts = new List<datas>();


    hdatas hd = new hdatas();



    void save()
    {
        System.IO.File.WriteAllText(@".\sv.txt", JObject.FromObject(hd).ToString());

    }

    void load()
    {
        if (System.IO.File.Exists(@"sv.txt"))
        {
            var t = System.IO.File.ReadAllText(@".\sv.txt");
            hd = JObject.Parse(t).ToObject<hdatas>();


        }
        else
        {
            hd = new hdatas()
            {
                 datas=new datas[0]
            };
        }
        if (hd == null)
        {
            hd = new hdatas()
            {
                datas = new datas[0]
            };
        }
       this.dts = new List<datas>(hd.datas);
    }

    TChart tchart1_y, tchart1_m,tchart1_d;
    TChart tchart2_y, tchart2_m, tchart2_d;
    TChart tchart3_y, tchart3_m, tchart3_d;

   


    void showdataEx()
    {

    }

    TChart chart1,_chart1;
    TChart chart2,_chart2;
    TChart chart3,_chart3;

    void showdata()
    {
       
        try
        {
            if (chart1 != null)
            {
                var tb= chart1.Tag as charttab;
                tb.updateView(chart1);
            }
        }
        catch (Exception e)
        {

        }
        try
        {
            if (chart2 != null)
            {
                var tb = chart2.Tag as charttab;
                tb.updateView(chart2);
            }
        }
        catch (Exception e)
        {

        }

        try
        {
            if (chart3 != null)
            {
                var tb = chart3.Tag as charttab;
                tb.updateView(chart3);
            }
        }
        catch (Exception e)
        {

        }

        try
        {
            if(zchart != null)
            {
                var tb = zchart.Tag as charttab;
                barchart.DataSource = tb.table;
               // tb.updateView(zchart);
            }
        }
        catch (Exception e)
        {

        }


        if (tb2 != null)
        {
            string xx = "--";
            var  aa= tc2_d.Tag as charttab;
            if (aa != null)
            {
                if (aa.table.Rows.Count > 0)
                {
                    var p = aa.table.Rows[aa.table.Rows.Count - 1];

                    xx = p["BYKCJE"].ToString();

                }
            }
            pie.UpdateData(tb2, xx);
        }

    }


    int ccc = 0;
    void __showdata()
    {
    
        if (tb1 != null)
        {
          
            if (tb1.Rows.Count > 0)
            {
                var r = tb1.Rows[0];
                bar.series[0].data[0].data[1] = asfloat(r["INSNUM"]);
                bar.series[0].data[1].data[1] = asfloat(r["INSYCL"]);
                bar.series[0].data[2].data[1] = asfloat(r["INSWCL"]);
                bar.series[1].data[0].data[1] = asfloat(r["OUTNUM"]);
                bar.series[1].data[1].data[1] = asfloat(r["OUTYCL"]);
                bar.series[1].data[2].data[1] = asfloat(r["OUTWCL"]);
            }
        }
        double xx = 0;
        if (tb4 != null)
        {
            var c = tb4.Rows.Count;
            float[] f1 = new float[c];
            float[] f2 = new float[c];
            float[] f3 = new float[c];
            float[] f4 = new float[c];
            string[] ss = new string[c];
            if(c>0)
            {
                var r = tb4.Rows[0];
                f1[0] = asfloat(r["BYKCJE"]);
                f2[0] = asfloat(r["OUTJE"]);
                f3[0] = asfloat(r["INSJE"]);
                var ff = r["KCRATE"].ToString();
                if (ff.Length > 1)
                {
                    ff = ff.Substring(0, ff.Length - 1);
                    f4[0] = asfloat(ff);
                }
                else
                {
                    f4[0] = 0;
                }
                xx = f1[0];
                ss[0] = r["ISDATE"].ToString();
                bool change = false;
                if (dts.Count > 0)
                {
                    if (dts[dts.Count - 1].time != ss[0])
                    {
                        dts.Add(new datas()
                        {
                            time = ss[0],
                            d1 = f1[0],
                            d2 = f2[0],
                            d3 = f3[0],
                            d4 = f4[0]
                        }
                            );
                        change = true;
                    }
                    else
                    {
                        var l = dts[dts.Count - 1];
                        if(l.d1!=f1[0] || l.d2!=f2[0] || l.d3!=f3[0] || l.d4 != f4[0])
                        {
                            change = true;
                        }
                        l.d1 = f1[0];
                        l.d2 = f1[1];
                        l.d3 = f1[2];
                        l.d4 = f1[3];
                    }
                }
                if (change)
                {
                    if (dts.Count > 7)
                        dts.RemoveAt(0);
                    hd.datas = dts.ToArray();
                    save();
                }
            }
            var xAxis = line1.GetChartComponent<XAxis>();
            List<string> sss = new List<string>();
            line1.series[0].data.Clear();
            line1.series[1].data.Clear();
            line1.series[2].data.Clear();
            for (int i = 0; i < dts.Count; i++)
            {
                sss.Add(dts[i].time);
                var d=new  SerieData();
                d.data[0] = i;
                d.data[1] = dts[i].d2;
                line1.series[0].data.Add(d);
                d = new SerieData();
                d.data[0] = i;
                d.data[1] = dts[i].d3;
                line1.series[1].data.Add(d);
             
            }
            xAxis.data = sss;

            xAxis = line3.GetChartComponent<XAxis>();
            sss = new List<string>();

            line3.series[0].data.Clear();
            for (int i = 0; i < dts.Count; i++)
            {
                sss.Add(dts[i].time);
                var d = new SerieData();
                d.data[1] = dts[i].d4;
                line3.series[0].data.Add(d);
            }
            xAxis.data = sss;
        }


        if (tb3 != null)
        {
            var c = tb3.Rows.Count;
            float[] f1 = new float[c];
            float[] f2 = new float[c];
            string[] ss = new string[c];
            for (int i = 0; i < c; i++)
            {
                var r = tb3.Rows[i];
                f1[i] = asfloat(r["INSQTY"]);
                f2[i] = asfloat(r["OUTQTY"]);
        
                ss[i] = r["INSDATE"].ToString();
            }
            var xAxis = line2.GetChartComponent<XAxis>();
            List<string> sss = new List<string>();
            line2.series[0].data.Clear();
            line2.series[1].data.Clear();
            for (int i = 0; i < ss.Length; i++)
            {
                sss.Add(ss[i]);
                var d = new SerieData();
                d.data[1] = f1[i];
                line2.series[0].data.Add(d);
                d = new SerieData();
                d.data[1] = f2[i];
                line2.series[1].data.Add(d);        
            }
            xAxis.data = sss;

        }
        if (tb2 != null)
        {
            pie.UpdateData(tb2,xx.ToString());
        }
    }
     async void getdata()
    {
        if (busy)
            return;
        busy = true;
        try
        {
            var ret = await Task<bool>.Run(() => { return select(); });
            if (ret)
                showdata();
        }
        catch(Exception e)
        {
         //   showdata();
        }
        finally
        {
            busy = false;
        }
        
    }


    public void ShowChart(bool show,bool swt=false)
    {
        if (swt)
        {
            bool b = t1.gameObject.activeSelf || t2.gameObject.activeSelf
                || t3.gameObject.activeSelf || t4.gameObject.activeSelf
                || t5.gameObject.activeSelf;
            show = !b;
        }

        if (show)
        {
            t1.gameObject.SetActive(true);
            t2.gameObject.SetActive(true);
            t3.gameObject.SetActive(true);
            t4.gameObject.SetActive(true);
            t5.gameObject.SetActive(true);
            _bar.transform.parent.gameObject.SetActive(false);
            _line1.transform.parent.gameObject.SetActive(false);
            _line2.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            t1.gameObject.SetActive(false);
            t2.gameObject.SetActive(false);
            t3.gameObject.SetActive(false);
            t4.gameObject.SetActive(false);
            t5.gameObject.SetActive(false);
            _bar.transform.parent.gameObject.SetActive(false);
            _line1.transform.parent.gameObject.SetActive(false);
            _line2.transform.parent.gameObject.SetActive(false);
        }
    }

    void close()
    {
        Application.Quit();
    }

 
    bool first = false;
    bool wait = false;
    double time = -1;
    double time2 = 0;
    // Update is called once per frame
    void Update()
    {
        if (time == -1)
        {
            time = 0;
            //   showdata1();
            getdata();
        }
        time += Time.deltaTime;
        if (time > G.ChartUpdateTime)
        {
            // showdata1();
            time = 0;
            getdata();
        }
        time2 += Time.deltaTime;
        if (time2 > 1)
        {
            time2 = 0;
            clock.text = DateTime.Now.ToString("yyyy.MM.dd  HH:mm");
           // throw new Exception();
            date.text = DateTime.Now.ToString("yyyy.MM.dd");
        }
    }
}
