using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGrid : MonoBehaviour
{
    public Binding[] Columns;
    public GameObject Template;
    Transform panel;
    // Start is called before the first frame update
    void Start()
    {
        if (Template != null)
        {
            panel = Template.transform.parent;
            var itm = Template.GetComponent<DataGridItem>();
            if (itm == null)
                Template.AddComponent<DataGridItem>();
            Template.transform.parent = null;
            Template.SetActive(false);
        }
    }

    internal void ItemClick(DataGridItem item)
    {
        if (OnItemClick != null)
        {
            OnItemClick(item);
        }
    }

    public event Action<DataGridItem> OnItemClick;


    Queue<Transform> frees = new Queue<Transform>();
    public void UpdateView()
    {
        if (panel == null)
        {
            return;
        }
        while (panel.childCount > 0)
        {

            
            var d = panel.GetChild(0);
            d.parent = null;

            frees.Enqueue(d);
            //GameObject.Destroy(d);
        }

        foreach(var e in db)
        {
            GameObject p = null;
            if (frees.Count > 0)
            {
                p = frees.Dequeue().gameObject;
            }
            else
            {
                p = GameObject.Instantiate(Template);
            }
            p.SetActive(true);
            var dgi = p.GetComponent<DataGridItem>();
            dgi.Init(this,this.Columns);
            dgi.transform.parent = panel.transform;
         
            dgi.DataContext = e;
            dgi.UpdateView();
        }
    }

    IEnumerable db;
 
    public IEnumerable DataSource
    {
        get { return db; }
        set
        {
            if (db != value)
            {
                db = value;
                UpdateView();
            }
        }
    }

    
}

