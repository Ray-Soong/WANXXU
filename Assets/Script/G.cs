using Assets.Scripts;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public static class G
{
    public static float PalletUseTime = 0.5f;
    public static double RecordBeginTime;

    public static float PalletSpeed = 1f;   //料箱移动速度
    public static float PalletAutoSpeed = 0.9f; //这是出库料箱无数据自动速度
    public static float CarSpeed = 1f;      //小车速度
    public static bool ShowPoint = false;
    public static float CLiftSpeed = 5;   //换层提升机速度
    public static float LiftSpeed = 5; //普通提升机速度
    public static float ChartUpdateTime = 10;
    /// <summary>
    /// 查找字对象
    /// </summary>
    public static Transform FindChild(Transform trans, string name)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var t = trans.GetChild(i);
            if (t.name == name)
                return t;
        }
        for (int i = 0; i < trans.childCount; i++)
        {
            var tt = FindChild(trans.GetChild(i), name);
            if (tt != null)
                return tt;
        }
        return null;
    }
    static System.Random rand = new System.Random();

    public static float MoveTo(float current, float target, float speed)
    {
        if (speed == 0)
            return current;
        if (current == target)
            return current;
        if (current > target)
            speed = -speed;
        current += Time.deltaTime * speed;



        if (speed > 0)
        {
            if (current > target)
                current = target;

        }
        else
        {
            if (current < target)
                current = target;
        }
        return current;
    }

    public static int Random(int Ceiling)
    {

        return rand.Next(Ceiling);
    }
    internal static SceneScript GetSceneScript()
    {
        var m = GameObject.Find("Main");
        if (m == null)
            return null;
        return m.GetComponent<SceneScript>();
    }

    public static DataMessageBox DataBox = new DataMessageBox();


    public static void AutoSize(GameObject UI, bool autoHeight, bool autoWidth, Vector2 offset)
    {
        if (autoHeight == false && autoWidth == false)
            return;
        float w = 0, h = 0;
        var rt = UI.GetComponent<RectTransform>();
        if (rt == null)
            return;
        for (int i = 0; i < UI.transform.childCount; i++)
        {
            var tran = UI.transform.GetChild(i);
            if (tran.gameObject.activeSelf)
            {

                var rect = tran.GetComponent<RectTransform>();
                if (rect != null)
                {
                    var hh = rect.anchoredPosition.y - rect.sizeDelta.y / 2;
                    var ww = rect.anchoredPosition.x + rect.sizeDelta.x / 2;
                    if (hh < h)
                    {
                        h = hh;
                    }
                    if (ww > w)
                    {
                        w = ww;
                    }
                }
            }
        }
        Vector2 size = rt.sizeDelta;
        if (autoWidth)
        {
            size.x = w + offset.x;
        }
        if (autoHeight)
        {
            size.y = -h + offset.y;
        }
        rt.sizeDelta = size;
    }


    public static System.Data.DataTable Query(string sql)
    {
        return Assets.Script.Datas.db.proc(sql);
    }

    public static T HttpJsonGet<T>(string url) where T : class
    {
        System.Net.HttpWebResponse g = null;
        System.Net.HttpWebRequest r = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
        try
        {
            r.Method = "GET";
            g = r.GetResponse() as System.Net.HttpWebResponse;
            if (g.StatusCode != System.Net.HttpStatusCode.OK)
                return null;
            byte[] bt = new byte[g.ContentLength];
            int pos = 0;
            while (pos < bt.Length)
            {
                var c = g.GetResponseStream().Read(bt, pos, bt.Length - pos);
                pos += c;
                if (c == 0)
                    break;
            }
            var str = System.Text.Encoding.UTF8.GetString(bt, 0, pos);
            return JObject.Parse(str).ToObject<T>();
        }
        catch (System.Exception e)
        {

        }
        finally
        {
            if (g != null)
                g.Close();
        }
        return null;

    }


    public static class Chart
    {
        public static void SetChartXAxisLabel(BaseChart chart, IEnumerable<string> labels)
        {
            var axis = chart.GetChartComponent<XAxis>();
            axis.data = new List<string>(labels);
        }

        public static void SetSeriesData(XCharts.Runtime.Serie serie, SerieType type, IEnumerable<double> values)
        {
            switch (type)
            {
                case SerieType.Line:
                case SerieType.Bar:
                    serie.data.Clear();
                    // List<SerieData> datas = new List<SerieData>();
                    int i = 0;
                    foreach (var e in values)
                    {
                        SerieData d = new SerieData();
                        d.data = new List<double>(new double[] { i, e });
                        serie.data.Add(d);
                        i++;
                    }
                    break;
                case SerieType.Pie:
                    break;
                default:
                    break;
            }
        }
    }
}


public enum SerieType
{
    Line,
    Bar,
    Pie
}



public class DataMessage
{
    public string Type;
    public double time;
    public object content;
}


public class DataMessageBox
{
    Queue<DataMessage> dms = new Queue<DataMessage>();
    object Lock = new object();
    public void Push(string type, string content)
    {
        lock (Lock)
        {
            dms.Enqueue(new DataMessage() { Type = type, content = content });
        }

    }

    public event System.Action<DataMessage> PopMessage;

    public DataMessage GetMessage()
    {
        DataMessage msg = null;
        if (dms.Count > 0)
        {
            lock (Lock)
            {
                if (dms.Count > 0)
                    msg = dms.Dequeue();
            }
        }
        if (PopMessage != null && msg != null)
        {
            PopMessage(msg);
        }
        return msg;

    }
}





//秒表
public sealed class StopWatch
{
    double t = 0;
    bool stop = true;
    float tt = 0;
    public void Start()
    {
        stop = false;
        tt = 0;
        t = Time.timeAsDouble;
    }

    public float time
    {
        get
        {
            if (!stop)
            {
                return (float)(Time.timeAsDouble - t);
            }
            else
            {
                return tt;
            }
        }
    }

    public float Stop()
    {
        if (!stop)
        {
            tt = (float)(Time.timeAsDouble - t);
            stop = true;
        }
        return tt;
    }
}


