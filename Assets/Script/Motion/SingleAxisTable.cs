using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.Motion
{
    [ExecuteInEditMode]
    public class SingleAxisTable : MonoBehaviour
    {
        public bool EditUpdateFlag;
        public float Value;
        public AxisName Axis;

        // Start is called before the first frame update
        Transform trans;
        public LinearTransformation Trans = new LinearTransformation();
        AxisController controler = new AxisController();
        public AxisController GetController()
        {
            return controler;
        }

        void Start()
        {
            trans = G.FindChild(this.gameObject.transform, "Axis");

            Trans.compute();
        }

        // Update is called once per frame
        void Update()
        {
            if (EditUpdateFlag)
            {
                if (trans == null)
                    Start();
                Trans.compute();
            }
            this.Value = controler.move(this.Value);
            float Value = Trans.transform(this.Value);
            switch (Axis)
            {
                case AxisName.X:
                    var p = trans.localPosition;
                    p.x = Value;
                    trans.localPosition = p;
                    break;
                case AxisName.Y:
                    p = trans.localPosition;
                    p.y = Value;
                    trans.localPosition = p;
                    break;
                case AxisName.Z:
                    p = trans.localPosition;
                    p.z = Value;
                    trans.localPosition = p;
                    break;
                case AxisName.A:
                    p = trans.localEulerAngles;
                    p.x = Value;
                    trans.localEulerAngles = p;
                    break;
                case AxisName.B:
                    p = trans.localEulerAngles;
                    p.y = Value;
                    trans.localEulerAngles = p;
                    break;
                case AxisName.C:
                    p = trans.localEulerAngles;
                    p.z = Value;
                    trans.localEulerAngles = p;
                    break;
                default:
                    break;
            }
            this.EditUpdateFlag = false;
        }
    }

}