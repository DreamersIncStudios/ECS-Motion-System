using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamersInc.Utils;
using Unity.Mathematics;

namespace SkillMagicSystem
{
    public class MagicGridObject 
    {
        private GridStatus Status;
        private Color gridColor = Color.white;
        private string SkillSpellName;
        // private AugmentGrid refernceToSkill;
        private int2 StartPos;
        private int x, y;

        private GridGeneric<MagicGridObject> grid;

        public MagicGridObject(GridGeneric<MagicGridObject> grid, int x, int y, string name = default)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            SkillSpellName = name;
        }
        public GridStatus GetGridStatus()
        {
            return Status;
        }
        public void SetStatus(GridStatus status)
        {
            Status = status;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void AddToMagicList() { }
        public void RemoveFromMagicList() { }

        public int width, height;
        public static Dir GetNextDir(Dir dir)
        {
            return dir switch
            {
                Dir.Left => Dir.Up,
                Dir.Up => Dir.Right,
                Dir.Right => Dir.Down,
                _ => Dir.Left,
            };
        }

        public static int GetRotationAngle(Dir dir)
        {
            return dir switch
            {
                Dir.Left => 90,
                Dir.Up => 180,
                Dir.Right => 270,
                _ => 0,
            };
        }

        public Vector2Int GetRotationOffset(Dir dir)
        {
            return dir switch
            {
                Dir.Left => new Vector2Int(0, width),
                Dir.Up => new Vector2Int(width, height),
                Dir.Right => new Vector2Int(height, 0),
                _ => new Vector2Int(0, 0),
            };
        }

        public void SetName(string name)
        {
            SkillSpellName = name;
            grid.TriggerGridObjectChanged(x, y);
        }

    }

    public enum Dir { Down, Up, Left, Right }
    public enum GridStatus
    {
        Open, Blocked, Occupied,
    }
}

