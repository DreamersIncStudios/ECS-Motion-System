using DreamersInc.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillMagicSystem
{
    public class GridPlace 
    {
        public Shape shape;
        public bool Rotatable { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dir dir;
        public GridGeneric<MagicGridObject> grid;
        public Color MapColor { get; private set; }

        public GridPlace(int width, int height, string name, Color color, Shape shape, bool Rotatable = true , Dir dir = Dir.Right) {
            this.Width = width;
            this.Height = height;
            MapColor = color;
            this.shape = shape;
            this.Rotatable = Rotatable;
            this.dir = dir;

            int cnt = new int();
            switch (shape) {
                case Shape.Square:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            grid.GetGridObject(x, y).SetStatus(GridStatus.Occupied);
                            grid.GetGridObject(x, y).SetName(name);
                        }
                    }
                    break;
                case Shape.T:
                    for (int x = 0; x < Width; x++)
                    {
                        grid.GetGridObject(x, 0).SetStatus(GridStatus.Occupied);
                        grid.GetGridObject(x, 0).SetName(name);
                    }
                    cnt = Mathf.RoundToInt( width / 2.0f);

                    for (int y = 0; y < Height; y++)
                    {
                        grid.GetGridObject(cnt, y).SetStatus(GridStatus.Occupied);
                        grid.GetGridObject(cnt, y).SetName(name);
                    }
                    break;
                case Shape.L:
                    for (int x = 0; x < Width; x++)
                    {
                        grid.GetGridObject(x, height-1).SetStatus(GridStatus.Occupied);
                        grid.GetGridObject(x, height - 1).SetName(name);
                    }
                   cnt = Mathf.RoundToInt(width / 2.0f);

                    for (int y = 0; y < Height; y++)
                    {
                        grid.GetGridObject(0, y).SetStatus(GridStatus.Occupied);
                        grid.GetGridObject(0, y).SetName(name);
                    }
                    break;
            }
        }
    }

    public enum Shape {  Square, T, t, L, l, Cross, }
}
