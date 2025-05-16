using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataGridItem : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        var bt= this.GetComponent<Button>();
        if (bt != null)
            bt.onClick.AddListener(c1);
    }

    public DataGrid DataGird { get { return g; } }

    BindingAgent[] agt;

    DataGrid g;

    void c1()
    {
        if (g != null)
        {
            g.ItemClick(this);
        }
    }
    


    public void Init(DataGrid g,Binding[] bindings)
    {
        this.g = g;
        if (bindings == null)
            agt = null;
        agt = new BindingAgent[bindings.Length];
        for(int i = 0; i < agt.Length; i++)
        {
            agt[i] = new BindingAgent(bindings[i], this.gameObject);
        }
    }

    public void UpdateView()
    {
        if (agt == null)
            return;
        foreach(var e in agt)
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
            if (dc !=value)
            {
                dc = value;
                UpdateView();
            }
        }

    }
}


public class BindingAgent
{
    public Binding Binding;
    GameObject obj;
    object ctrl;
    public BindingAgent(Binding binding, GameObject Target)
    {
        this.Binding = binding;
        switch (binding.ControlType)
        {
            case ControlType.Text:
                obj = G.FindChild(Target.transform, Binding.ControlName).gameObject;
                if (obj != null)
                {
                    ctrl = obj.GetComponent<Text>();
                }
                break;
            case ControlType.Image:
                obj = G.FindChild(Target.transform, Binding.ControlName).gameObject;
                if (obj != null)
                {
                    ctrl= obj.GetComponent<Image>();
                }
                break;
            case ControlType.DataGrid:
                obj = G.FindChild(Target.transform, Binding.ControlName).gameObject;
                if (obj != null)
                {
                    ctrl = obj.GetComponent<DataGrid>();
                }
                break;
            case ControlType.DataView:
                obj = G.FindChild(Target.transform, Binding.ControlName).gameObject;
                if (obj != null)
                {
                    ctrl = obj.GetComponent<DataView>();
                }
                break;
           
            default:
                break;
        }
       

    }

    public void UpdateValue(object data)
    {
        try
        {
            if (Binding.ControlType == ControlType.Text)
            {
                var text = ctrl as Text;
                if (text == null)
                    return;
                if (data == null)
                    text.text = Binding.NullValue;
                var p = data.GetType().GetProperty(Binding.PropertyName);
                if (p == null)
                {
                    text.text = Binding.NullValue;
                    return;
                }
                var o = p.GetValue(data);
                if (o == null)
                {
                    text.text = Binding.NullValue;
                    return;
                }
                text.text = o.ToString();
            }

            if (Binding.ControlType == ControlType.Image)
            {
                Image img = ctrl as Image;
                if (img == null)
                    return;
                if (data == null)
                    return;
                var p = data.GetType().GetProperty(Binding.PropertyName);
                if (p.PropertyType != typeof(Sprite))
                {
                    return;
                }
                var dd = p.GetValue(data);
                if (dd == null)
                    return;
                img.sprite = dd as Sprite;
            }
        }
        catch
        {

        }


    }
}


[System.Serializable]
public class Binding
{
    public string ControlName;
    public ControlType ControlType = ControlType.Text;
    public string PropertyName;
    public string Format;
    public string NullValue;
}

public enum ControlType
{
    Text,
    Image,
    DataGrid,
    DataView
}