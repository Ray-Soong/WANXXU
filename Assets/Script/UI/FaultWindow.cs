using Assets.Script.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FaultWindow : MonoBehaviour
{
    public DataView Panel1;
    public DataView Panel2;
    public Button bt1, bt2;
    public GameObject ErrorLight;
    DeviceManager devm;
    // Start is called before the first frame update
    void Start()
    {
        devm = G.GetSceneScript().DeviceManager;
        Panel1.AutoUpdateAble = false;
        ErrorLight.SetActive(false);
        bt1.onClick.AddListener(prev);
        bt2.onClick.AddListener(next);
    }

    void prev()
    {
        devm.Prev();
    }

    void next()
    {
        devm.Next();
    }

    Device d;
    bool last = true;
    bool first = true;
    float tm = 0;
    // Update is called once per frame
    void Update()
    {
        if (devm == null)
        {
            devm = G.GetSceneScript().DeviceManager;
            return;
        }
        tm += Time.deltaTime;
        if (tm > 0.1f)
        {
            var dd = devm.CurrentErrorDevice;
            if (this.last != devm.IsLast)
            {
                last = devm.IsLast;
                bt2.gameObject.SetActive(last==false);
            }

            if (first != devm.IsFirst)
            {
                first = devm.IsFirst;
                bt1.gameObject.SetActive(first == false);
            }



            if (dd != d)
            {
                if (dd == null)
                {
                    ErrorLight.SetActive(false);
                }
                else
                {
                    ErrorLight.SetActive(true);
                }
                d = dd;
                Panel1.DataContext = d;
                if (Panel1.DataContext != null)
                {
                   // Panel1.gameObject.SetActive(true);
                }
                else
                {
                    Panel1.gameObject.SetActive(false);
                }
            }
        }
    }
}
