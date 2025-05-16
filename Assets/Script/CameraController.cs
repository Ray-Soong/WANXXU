using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Assets.Script.D3
{
    public class CameraController : MonoBehaviour
    {

        Transform cm;

        float ms_dx;
        float ms_dy;
        float ms_dz;
        bool ms_l, ms_r, ms_m;
        public float PinYi = 5;
        public float Rotate = 3;
        Vector3 last;

        // Start is called before the first frame update
        void Start()
        {
            cm = this.transform;

        }

        void ppp()
        {
            if (ms_dz != 0)
            {
                cm.position += (cm.forward.normalized * ms_dz * PinYi);
                return;
            }
            if (ms_m)
            {
                var a = cm.right.normalized;
                a *= ms_dx * PinYi;
                cm.position -= a;
                a = cm.up.normalized;
                a *= ms_dy * PinYi;
                cm.position -= a;
                return;
            }
            if (ms_r)
            {
                rotate();
            }

        }



        bool pingyi()
        {
            Vector3 v = cm.position;
            if (ms_m)
            {

                v.x += ms_dx * PinYi;
                v.y += ms_dy * PinYi;

            }
            else
            {
                if (ms_dz != 0)
                {
                    v.z += ms_dz * PinYi;
                }
                else
                    return false;
            }
            cm.position = v;
            return true;
        }



        void updateInput()
        {
            ms_r = UnityEngine.Input.GetMouseButton(1);
            ms_m = UnityEngine.Input.GetMouseButton(2);
            ms_dx = UnityEngine.Input.GetAxis("Mouse X");
            ms_dy = UnityEngine.Input.GetAxis("Mouse Y");
            ms_dz = UnityEngine.Input.GetAxis("Mouse ScrollWheel");

        }
        float r1 = 0, r2 = 0;
        public Transform LookTo;
        void rotate()
        {

            var xx = ms_dx * Rotate;
            var yy = ms_dy * Rotate;
            if (Mathf.Abs(xx) > Mathf.Abs(yy))
            {
                yy = 0;
            }
            else
                xx = 0;

            if (xx != 0)
            {
                cm.transform.RotateAround(cm.transform.position, new Vector3(0, 1, 0), xx);
            }
            if (yy != 0)
            {
                cm.transform.Rotate(new Vector3(1, 0, 0), -yy);
            }
            transform.RotateAround(LookTo == null ? transform.position : LookTo.position, Vector3.up, ms_dx * 5);
            // transform.RotateAround(LookTo == null ? transform.position : LookTo.position, transform.right, ms_dy * 5);
        }




        // Update is called once per frame
        void Update()
        {

            updateInput();

            ppp();


        }
    }

}




