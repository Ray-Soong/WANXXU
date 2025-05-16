using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clift : MonoBehaviour
{
    float[] pos = new float[] { -1.1f, - 0.62f, - 0.22f,  0.22f,   0.63f,  1.03f,   1.45f,  1.87f };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int showlevel;
    public Assets.Script.Motion.SingleAxisTable axis;
    public float speed;
    float target;
    float curr;
    int level;
    public int CurrentLevel;
    int targetLevel;

    // Update is called once per frame
    void Update()
    {
        if (targetLevel != CurrentLevel)
        {
            Vector3 v = new Vector3(0, 0, target);
            Vector3 V1 = new Vector3(0, 0, curr);
            var p = Vector3.MoveTowards(V1, v, G.CLiftSpeed * Time.deltaTime);
            this.curr = p.z;
            axis.Value = p.z;
            if(this.curr==target)
            {
                this.CurrentLevel = targetLevel;
            }
            

        }
    }


    public void MoveToLevel(int level)
    {
        this.showlevel = level;
        if (level == 0)
            return;
        this.target = this.pos[level - 1];
        this.targetLevel = level;
    }
}
