using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Text text1;
    public Text text2;
    public Text text3;
    string sql;
    bool ok;
    bool busy = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(image1!=null && image2!=null && text1!=null && text2 != null)
        {
            ok = true;
        }
    }



    public  void UpdateData(System.Data.DataTable ret,string jine)
    {
        try
        {
            if (ret.Rows.Count > 0)
            {
                var tt = ret.Rows[0];
                var o = tt["USERATE"];
                if(o!=null && o != DBNull.Value)
                {
                   var s= o.ToString();
                    if (s.Length > 2)
                    {
                        if (s[s.Length - 1] == '%')
                        {
                            float a = float.Parse(s.Substring(0, s.Length - 1)) / 100;
                            image1.fillAmount = a;
                            image2.fillAmount =(1.01f-a);
                        }
                    }
                    text2.text = s;
                }
                o  = tt["USENUM"];
                if (o != null && o != DBNull.Value)
                {
                    text1.text = "货位实际使用数： " + o.ToString()+"/750";
                }
                text3.text = "库存总金额："+jine;
            }
        }
        catch(System.Exception e)
        {

        }
    }

    
}
