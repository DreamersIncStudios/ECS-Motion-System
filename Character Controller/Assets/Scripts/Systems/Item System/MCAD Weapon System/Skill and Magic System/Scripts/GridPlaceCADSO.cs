using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.MagicSkill {
    public  class GridPlaceCADSO : ScriptableObject, IBaseMagicSkill
    {
        public string Name { get { return SkillSpellname; } }
        [SerializeField] string SkillSpellname;
        public int ID { get { return id; } }
        [SerializeField] int id;
        public string Description { get { return description; } }
        [SerializeField] string description;
        public int Level { get { return level; } }
            [SerializeField] int level;
        public AugmentGrid Grid { get { return grid; } }
        [SerializeField] AugmentGrid grid;
        public int Value { get { return value; } }
        [SerializeField] int value;
        public Classification GetClassification { get { return classification; } }
        [SerializeField] Classification classification;
        public Specialty GetSpecialty { get { return specialty; } }
        [SerializeField] Specialty specialty;
        public int width, height;

        public void AugmentItem(CastingDevice Grid, int x, int y) { }


        public  void RemoveAugment(CastingDevice Grid) { }

        public void Create(string name, int width, int height, int Level, int value, Color color = default)
        {
            this.SkillSpellname = name;
            this.level = Level;
            this.value = value;
            this.grid = new AugmentGrid(width, height, name, color);
            grid.grid.GetGridObject(1, 0).SetStatus(GridStatus.Open);
            grid.grid.GetGridObject(1, 2).SetStatus(GridStatus.Open);
        }
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

        public  Vector2Int GetRotationOffset(Dir dir)
        {
            return dir switch
            {
                Dir.Left => new Vector2Int(0, width),
                Dir.Up => new Vector2Int(width, height),
                Dir.Right => new Vector2Int(height, 0),
                _ => new Vector2Int(0, 0),
            };
        }

    }

    public enum Dir { Down, Up, Left, Right}
}