using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{
    public DataGrid DataGird;
    // Start is called before the first frame update
    void Start()
    {
        List<ABC> abc = new List<ABC>();
        for(int i = 0; i < 3; i++)
        {
            abc.Add(new ABC() { A = (i+1).ToString(), B = (i*2).ToString(), C = (i/3.0f).ToString() });
        }
        this.DataGird.DataSource = abc;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}


public class ABC
{
    public string A { get;  set; }
    public string B { get; set; }
    public string C { get; set; }
}
