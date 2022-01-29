using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamersInc.Utils;
using Utilities;
using Unity.Mathematics;
namespace DreamersInc.MagicSkill
{
    public class CastingDevice :MonoBehaviour
    {
        private int width, height;
        private float cellsize;
        public GridGeneric<MagicSkillGridObject> grid;
        public List<string> SpellNames;
        public Vector2Int GetDimensions { get { return new Vector2Int(width, height); } }
        public List<GridPlaceCADSO> test;
        public void Setup ( int width = 15, int height =10, float cellsize = 5f) {
            grid = new GridGeneric<MagicSkillGridObject>(width, height, cellsize, (GridGeneric<MagicSkillGridObject> g, int x, int y) => new MagicSkillGridObject(g, x, y)
            );
            this.width = width;
            this.height = height;
            this.cellsize = cellsize;
            
        }
        private void Start()
        {
            test = new List<GridPlaceCADSO>();
            for (int i = 0; i < 20; i++)
            { 
               GridPlaceCADSO temp = (GridPlaceCADSO)ScriptableObject.CreateInstance(typeof(GridPlaceCADSO));
                temp.Create("FireBall", 2, 3, 1, 100, Color.black);
                test.Add(temp);
            }
            Setup();

        }

        public void Update()
        {
            
        }




    }


    [System.Serializable]
    public class AugmentGrid {
        public int Width { get; private set; }
        public int Height{ get; private set; }
        public Dir dir;
        public Color MapColor { get; private set; }
        
        
        public GridGeneric<MagicSkillGridObject> grid;

        public AugmentGrid(int width, int height, string name, Color color) {
            grid = new GridGeneric<MagicSkillGridObject>(width, height, 5.0f,new Vector3(-20,0,20),(GridGeneric<MagicSkillGridObject> g, int x, int y) => new MagicSkillGridObject(g, x, y)
            
            );
            this.Width = width;
            this.Height = height;
            MapColor = color;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    grid.GetGridObject(x, y).SetStatus(GridStatus.Occupied);
                    grid.GetGridObject(x, y).SetName(name);
                }
            }

        }



    }
}