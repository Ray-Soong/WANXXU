using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCharts.Runtime;

namespace Assets.Tools
{
    public class TChart
    {
        XCharts.Runtime.BaseChart baseChart;
        binding[] bindings;
        binding axis;
        object data;

        public bool AxisIsY;

        public object Tag;

        class binding
        {
            public string field;
            public Convert convers;
            public object param;
            public bool enable;

            public void UpdateEnable(DataTable tb)
            {
                foreach(DataColumn e in tb.Columns)
                {
                    if (e.ColumnName == field)
                    {
                        enable = true;
                        return;
                    }
                }
                enable = false;
            }

            public double GetValue(DataRow row)
            {
                var dd= row[field];
                if(dd== DBNull.Value || dd == null)
                {
                    return 0;
                }
                if (convers != null)
                    dd = convers(dd, null);
                return  (double)System.Convert.ChangeType(dd, typeof(double));
            }

            public string GetString(DataRow row)
            { 
                var dd = row[field];
                if (dd == DBNull.Value || dd == null)
                {
                    return null;
                }
                if (convers != null)
                    dd = convers(dd, null);
                return dd == null ? "" : dd.ToString();
            }
        }

        bool UpdateAxis(string[] vs)
        {
            if (this.AxisIsY)
            {
                var a = baseChart.GetChartComponent<YAxis>();
                if (a.data != null)
                {
                    if (a.data.Count == vs.Length)
                    {
                        for (int i = 0; i < vs.Length; i++)
                        {
                            a.data[i] = vs[i];
                        }
                        a.SetAllDirty();
                        return true;
                    }
                }
                a.data = new List<string>(vs);
                a.SetAllDirty();
                return true;

            }
            else
            {
                var a = baseChart.GetChartComponent<XAxis>();
                if (a.data != null)
                {
                    if (a.data.Count == vs.Length)
                    {
                        for (int i = 0; i < vs.Length; i++)
                        {
                            a.data[i] = vs[i];
                        }
                        a.SetAllDirty();
                        return true;
                    }
                }
                a.data = new List<string>(vs);
                a.SetAllDirty();
                return true;
            }
        }
        void UpdateSeries(int index,double[] dv)
        {
            var s = baseChart.series[index];
            if (s.data != null)
            {
                if (s.data.Count == dv.Length)
                {
                    for(int i = 0; i < dv.Length; i++)
                    {
                        s.data[i].data[1] = dv[i];
                    }
                   // s.SetAllDirty();
                    s.label.SetAllDirty();
                    return;
                }
            }
            s.data.Clear();
            for(int i = 0; i < dv.Length; i++)
            {
                SerieData d = new SerieData();
                d.data = new List<double>();
                d.data.Add(i);
                d.data.Add(dv[i]);
                s.data.Add(d);
            }
            s.SetAllDirty();
            s.label.SetAllDirty();
          //  s.data = sd;
           
        }
        public TChart(XCharts.Runtime.BaseChart chart)
        {
            this.baseChart = chart;
            bindings = new binding[chart.series.Count];
            for(int i = 0; i < bindings.Length; i++)
            {
                bindings[i] = new binding() { enable = false };
            }
        }

        public void Update()
        {
            bool nothing = true;
            if (data is System.Data.DataTable)
            {
                var t = data as System.Data.DataTable;
                if (t.Rows.Count > 0)
                    nothing = false;
            }
            if (nothing == false)
            {




                var tb = data as System.Data.DataTable;
                //List<double[]> da = new List<double[]>();

                foreach (var e in bindings)
                {
                    e.UpdateEnable(tb);
                }
                axis.UpdateEnable(tb);
                if (axis.enable == false)
                    return;


                object[] dt = new object[bindings.Length + 1];
                dt[0] = new string[tb.Rows.Count];
                var strs = dt[0] as string[];
                for (int i = 1; i < dt.Length; i++)
                {
                    dt[i] = new double[tb.Rows.Count];
                }
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    DataRow r = tb.Rows[i];
                    for (int j = 0; j < bindings.Length; j++)
                    {
                        if (bindings[j].enable)
                        {
                            var dvs = dt[j + 1] as double[];
                            dvs[i] = bindings[j].GetValue(r);
                        }
                    }
                    strs[i] = axis.GetString(r);
                }

                if (this.UpdateAxis(strs))
                {
                    for (int i = 0; i < bindings.Length; i++)
                    {
                        baseChart.series[i].data.Clear();
                    }
                }
                for (int i = 0; i < bindings.Length; i++)
                {
                    if (bindings[i].enable)
                    {
                        this.UpdateSeries(i, dt[i + 1] as double[]);
                    }
                    else
                    {
                        baseChart.series[i].data.Clear();
                        baseChart.series[i].SetAllDirty();
                    }
                }
            }
            else
            {
                foreach (var e in baseChart.series)
                {
                    e.data.Clear();
                }
                foreach (var e in baseChart.series)
                {
                    e.labelDirty = true;
                    e.label.SetAllDirty();
                    e.label.ClearDirty();
                }
                if (AxisIsY)
                {
                    if (baseChart.GetChartComponent<YAxis>().data != null)
                    {
                        baseChart.GetChartComponent<YAxis>().data.Clear();
                    }
                    baseChart.GetChartComponent<YAxis>().SetAllDirty();
                }
                else
                {
                    if (baseChart.GetChartComponent<XAxis>().data != null)
                    {
                        baseChart.GetChartComponent<XAxis>().data.Clear();
                    }
                    baseChart.GetChartComponent<XAxis>().SetAllDirty();
                }
            }

        }

        public System.Data.DataTable RandomDataSource(DateTime from, TimeSpan add, string DateFromat, int count, double min, double max)
        {
            System.Data.DataTable tab = new DataTable();
            tab.Columns.Add(axis.field, typeof(string));
            Random rdm = new Random();
            foreach (var e in this.bindings)
            {
                tab.Columns.Add(e.field, typeof(double));
            }
            for (int i = 0; i < count; i++)
            {
                var r = tab.NewRow();
                var t = (from + add * i).ToString(DateFromat);
                r[0] = t;
                for (int j = 0; j < bindings.Length; j++)
                {
                    r[j+1] = rdm.NextDouble() * (max - min) + min;
                }
                tab.Rows.Add(r);
            }
            return tab;

        }


        public object DataSource { get { return this.data; } }

        public void SetDataSource(object obj)
        {
            this.data = obj;
        }

        public void SetSeriesBinding(int SeriesIndex,string BindingName, Convert Convert,object Param)
        {
            bindings[SeriesIndex] = new binding()
            {
                field = BindingName,
                convers = Convert,
                param=Param
            };
        }

        public void SetVisible(int SeriesIndex,bool visible)
        {
            baseChart.series[SeriesIndex].gameObject.SetActive(visible);
        }


        public void SetXAxisBinding(string BindingName,Convert Convert=null,object Param=null)
        {
            axis = new binding()
            {
                field = BindingName,
                convers=Convert,
                param=Param
            };
        }



    }

    public delegate object Convert(object Source, object param);

   
}
