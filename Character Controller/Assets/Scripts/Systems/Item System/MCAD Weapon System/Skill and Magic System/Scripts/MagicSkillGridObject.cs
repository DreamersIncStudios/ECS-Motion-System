using System.Collections.Generic;
using DreamersInc.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DreamersInc.MagicSkill {

    [System.Serializable]
    public class MagicSkillGridObject 
    {
        private GridStatus Status;
        private string SkillSpellName;
        private GridGeneric<MagicSkillGridObject> grid;
        private AugmentGrid refernceToSkill;
        private Color gridColor = Color.white;
        private PlacedAugmentedGrid placedAugmentedGrid;
        
        private Button button;

        private int2 StartPos;
        private int x, y;
        public MagicSkillGridObject( GridGeneric<MagicSkillGridObject> grid, int x, int y, string name = default) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            SkillSpellName = name;
        }

        public void SetStatus(GridStatus status) 
        {
            Status = status;
            grid.TriggerGridObjectChanged(x,y);
        }
        public void SetButton(Button button) {
            this.button = button;
            if (!CanPlace()) {
                SetButtonColor(gridColor);
            }
        }
        public void SetButtonColor(Color color) {
            gridColor = button.gameObject.GetComponent<Image>().color = color;

        }
        public void SetPlacedAugmentedGrid(PlacedAugmentedGrid grid) {
            this.placedAugmentedGrid = grid;
            SetStatus(GridStatus.Occupied);
            SetButtonColor(grid.GetPlaceGrid.MapColor);
            
        }

        public void SetName(string name) {
            SkillSpellName = name;
            grid.TriggerGridObjectChanged(x, y);
        }
        public void SetFirstCell(int2 location) {
            StartPos = location;
            grid.TriggerGridObjectChanged(x, y);
        }
        public void SetGridRef(AugmentGrid grid) {
            Status = GridStatus.Occupied;
            refernceToSkill = grid;
            this.grid.TriggerGridObjectChanged(x, y);
        }
        public void SetGridColor(Color color) {
            this.gridColor = color;
            grid.TriggerGridObjectChanged(x, y);

        }
        public void ClearGridColor() {
            this.gridColor = Color.white;
            grid.TriggerGridObjectChanged(x, y);

        }

        public void Reset() {
            placedAugmentedGrid = null;
            SetButtonColor(gridColor = Color.white);
            refernceToSkill = null;
            SetStatus(GridStatus.Open);
        }

        public bool CanPlace() {
            return Status == GridStatus.Open;
        }
     
        public GridStatus GetStatus
        {
            get
            {
                return Status;
            }
        }
        public string GetName
        {
            get { return SkillSpellName;}
        }
        public int2 GetFirstCell { get { return StartPos; } }
        public AugmentGrid GetSkillMap { get { return refernceToSkill; } }
        public override string ToString()
        {
            return x+", " + y+ "\n" + Status+ "\n" + SkillSpellName;
        }

        public bool AddMapToGrid( AugmentGrid addGrid)
        {
            Vector2Int input = new Vector2Int(x, y);
            List<Vector2Int> gridPositionList = GetGridPositionList(input, addGrid);
            bool canPlace = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!grid.GetGridObject(gridPosition).CanPlace())
                {
                    canPlace = false;
                }
            }
            if (canPlace)
            {
                //TODO Add Visualization Implementation 
                PlacedAugmentedGrid placed = PlacedAugmentedGrid.Create(input, addGrid);
                foreach (var gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition).SetPlacedAugmentedGrid(placed);
                }
            }

            return canPlace;
        }

        public void RemoveMapToGrid(Vector2Int input)
        {
            if (grid.GetGridObject(x, y).placedAugmentedGrid != null)
            {
                List<Vector2Int> gridPositionList = grid.GetGridObject(x, y).GetGridPositionList(input,placedAugmentedGrid.GetPlaceGrid);
                foreach (Vector2Int vector in gridPositionList)
                {
                    grid.GetGridObject(vector).Reset();
                }

            }
        }

        public List<Vector2Int> GetGridPositionList(Vector2Int offset, AugmentGrid addGrid)
        {
            int width = addGrid.Width;
            int height = addGrid.Height;
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (addGrid.dir)
            {
                default:
                case Dir.Down:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (!addGrid.grid.GetGridObject(i, j).CanPlace())
                                gridPositionList.Add(offset - new Vector2Int(i, j - height + 1));
                        }
                    }
                    break;
                case Dir.Up:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (!addGrid.grid.GetGridObject(i, j).CanPlace())
                                gridPositionList.Add(offset + new Vector2Int(i, j));
                        }
                    }
                    break;
                case Dir.Left:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (!addGrid.grid.GetGridObject(i, j).CanPlace())
                                gridPositionList.Add(offset + new Vector2Int(j, i));
                        }
                    }
                    break;
                case Dir.Right:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (!addGrid.grid.GetGridObject(x, y).CanPlace())
                                gridPositionList.Add(offset - new Vector2Int(y - width, x));
                        }
                    }
                    break;
            }

            return gridPositionList;
        }

    }

    public enum GridStatus { 
       Open, Blocked, Occupied,
    }
}