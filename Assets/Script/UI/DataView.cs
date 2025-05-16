using Assets.Script.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataView : MonoBehaviour
{
    int? tick = null;
    public Binding[] Bindings;
    public Button CloseButton;
    bool inited = false;
    // Start is called before the first frame update
    void Start()
    {
     
        if (Bindings == null)
            agt = null;
        agt = new BindingAgent[Bindings.Length];
        for (int i = 0; i < agt.Length; i++)
        {
            agt[i] = new BindingAgent(Bindings[i], this.gameObject);
        }
        if (this.CloseButton != null)
        {
            this.CloseButton.onClick.AddListener(close);
        }
        inited = true;
    }

    void close()
    {
        this.gameObject.SetActive(false);
    }

    BindingAgent[] agt;

    public void TryUpdateView()
    {
        if(this.DataContext==null && tick != null)
        {
            tick = null;
            UpdateView();
        }
        if (this.DataContext != null)
        {

            if(this.DataContext is IUpdateTickAble)
            {
                var dd = this.DataContext as IUpdateTickAble;
                var tk = dd.Tick;
                if (tk != tick)
                {
                    UpdateView();
                    tick = tk;
                }
            }
            else
            {
                TryUpdateView();
                tick = 1;
            }
        }
    }

     void UpdateView()
    {
        if (agt == null)
            return;
        foreach (var e in agt)
        {
            e.UpdateValue(this.DataContext);
        }
    }
    object dc;

    public object DataContext
    {
        get { return dc; }
        set
        {
            if (inited == false)
                Start();
            if (dc != value)
            {
                tick = null;
                dc = value;
                TryUpdateView();
            }
        }

    }
    public bool AutoUpdateAble;
    public float AutoUpdateInterval;
    float tm = 0;
    // Update is called once per frame
    void Update()
    {
        if (AutoUpdateAble)
        {
            tm += Time.deltaTime;
            if (tm > (AutoUpdateInterval<=0.05f?0.05f:AutoUpdateInterval))
            {
                tm = 0;
                TryUpdateView();
            }
        }
    }
}
