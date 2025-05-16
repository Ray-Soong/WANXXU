using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.Motion
{

    class Param
    {
        float Hole1_1 = -0.408f;
        float Hole1_2 = -3.19f;
        float Hole1_3 = -5.928f;
        float holeadd = -0.54225f;

        float Hole_x1 = -6.381f;
        float Hole_x2 = -7.11f;

        float Hole_y1 = -4.88f;
        float Hole_y2 = -4.151f;
        float Lift_Param = -0.867f;

        float[] xiangdao_values = new float[] { -5.631f, -178f };

        float[] xiang1cols_offset = new float[] { -0.751f, -1.48f, 0.851f, 1.48f };
        public xiangdao xiangdao1;
        public xiangdao xiangdao2;
        public void create()
        {
            List<float> holes = new List<float>();
            var st = Hole1_1;
            for (int i = 0; i < 5; i++)
            {
                holes.Add(st + i * holeadd);
            }
            st = Hole1_2;
            for (int i = 0; i < 5; i++)
            {
                holes.Add(st + i * holeadd);
            }

            st = Hole1_3;
            for (int i = 0; i < 5; i++)
            {
                holes.Add(st + i * holeadd);
            }



            xiangdao1 = new xiangdao()
            {
                Position = -1.78f,
                Holes = holes.ToArray()
            };
            xiangdao2 = new xiangdao()
            {
                Position = -5.631f,
                Holes = holes.ToArray()
            };
        }

    }

    class xiangdao
    {
        public float Position;
        public float[] ColumnOffset = new float[] { -1.48f ,- 0.751f, 0.751f, 1.48f };
        public float[] Holes;
        public float[] LibFloor = new float[] { 0.546f, 0.97f, 1.4f, 1.827f, 2.253f, 2.684f, 3.115f, 3.544f };
        public float[] InterfaceFloor = new float[] { 0.583f, 1.313f };

        
        public float GetRow(int column)
        {
            return Position + ColumnOffset[column-1];
        }


    }


    class Store : IStore
    {
        Param pm = new Param();
        public static IStore store = new Store();
           
        public Store()
        {
            pm.create();
        }
        public float GetFloor(int xiangdao, int floor)
        {
            var xd = xiangdao == 1 ? pm.xiangdao1 : pm.xiangdao2;
            return xd.LibFloor[floor - 1];
        }

        public float GetHole(int xiangdao, int floor, int holenumber)
        {
            var xd = xiangdao == 1 ? pm.xiangdao1 : pm.xiangdao2;
            if (holenumber == 0)
            {
                return 0.3f;
            }
            return xd.Holes[xd.Holes.Length-holenumber];
        }



        public float GetRow(int xiangdao, int floor, int column)
        {
            var xd = xiangdao == 1 ? pm.xiangdao1 : pm.xiangdao2;
            return xd.GetRow(column);
        }

        public float Getxiangdao(int xiangdao)
        {
            var xd = xiangdao == 1 ? pm.xiangdao1 : pm.xiangdao2;
            return xd.Position;
        }
    }


    public interface IStore
    {
        float GetFloor(int xiangdao, int floor);
        float GetHole(int xiangdao, int floor, int holenumber);
        float GetRow(int xiangdao,int floor,int column);
        float Getxiangdao(int xiangdao);
    }
}
