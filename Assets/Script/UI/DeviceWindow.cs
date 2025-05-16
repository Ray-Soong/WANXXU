using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeviceWindow : MonoBehaviour
{

    public Button bt1;
    public Button bt2;
    public Button bt3;
    public DataGrid grid;
    public DataView DataView;
    // Start is called before the first frame update
    void Start()
    {
        bt1.onClick.AddListener(c1);
        bt2.onClick.AddListener(c2);
        bt3.onClick.AddListener(c3);
        grid.OnItemClick += Grid_OnItemClick;
    }

    private void Grid_OnItemClick(DataGridItem obj)
    {
        var d = obj.DataContext;
        DataView.DataContext = d;
        DataView.gameObject.SetActive(true);
 
    }

    void show(string type)
    {
        var dd = G.GetSceneScript().DeviceManager.SelectByType(type);
        grid.DataSource = dd;

    }

    void c1()
    {
        show("RGV");
    }

    void c2()
    {
        show("换层提升机");
    }

    void c3()
    {
        show("货物提升机");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
