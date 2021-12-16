
using UnityEngine;
using UnityEngine.UI;
using DreamersInc.MagicSkill;
using System;
using DreamersInc.Utils;

namespace Dreamers.InventorySystem.UISystem
{
    public partial class DisplayMenu
    {

       public  class CADPanel : Panel
        {
            Sprite gridSquares;
            CastingDevice CAD;
            public CADPanel( Vector2 Size, Vector2 Position, CastingDevice CAD)
            {
                Setup(Size, Position);
                gridSquares = Resources.Load<Sprite>("Sprites/Grid_Square");
                this.CAD = CAD;
            }

            public override GameObject CreatePanel(Transform Parent)
            {
                if (Top)
                    UnityEngine.Object.Destroy(Top);
                Top = Manager.GetPanel(Parent, Size, Position);
                Top.name = "CAD Menu";
                VerticalLayoutGroup verticalLayoutGroup = Top.AddComponent<VerticalLayoutGroup>();
              
                GameObject CADGRID = Manager.GetPanel(Top.transform, new Vector2(0, 300), new Vector2(0, 150));
                    GridLayoutGroup gridLayoutGroup = CADGRID.AddComponent<GridLayoutGroup>();
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
                gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                gridLayoutGroup.constraintCount = CAD.GetDimensions.x;
                gridLayoutGroup.cellSize = new Vector2(50, 50);
                gridSquares = Resources.Load<Sprite>("Sprites/Grid_Square");

                for (int y = 0; y < CAD.GetDimensions.y; y++)
                {
                    for (int x = 0; x < CAD.GetDimensions.x; x++)
                    {
                        int i = x; int j = y;
                        //Todo Check to see if slot is taken 

                        Button gridUI= Manager.UIButton(CADGRID.transform,"");
                        gridUI.gameObject.AddComponent<GridSquareDrop>().Create(new Vector2Int(i,j), CAD);
                        CAD.grid.GetGridObject(i, j).SetButton(gridUI);
                        gridUI.navigation = new Navigation() { mode = Navigation.Mode.None };


                        gridUI.onClick.AddListener(() => {

                            CAD.grid.GetGridObject(i, j).RemoveMapToGrid(new Vector2Int(i,j));
                            
                        });
                    }
                }
                GameObject Inventory = Manager.GetPanel(Top.transform, new Vector2(0, 150), new Vector2(0, 150));
                Inventory.AddComponent<HorizontalLayoutGroup>();
                foreach (GridPlaceCADSO so in CAD.test) {
                    Image item = Manager.GetImage(Inventory.transform, gridSquares);
                    item.gameObject.AddComponent<DragDropGridMap>().GridSO = so;
                    item.gameObject.AddComponent<CanvasGroup>();

                }

                return Top;
            }

            public override void DestoryPanel()
            {
                UnityEngine.Object.Destroy(Top);

            }
            public override void Refresh()
            {
                throw new System.NotImplementedException();
            }
        }

        public CADPanel GetCADPanel;

        
    }

}