using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DreamersInc.MagicSkill
{
    public class PlacedAugmentedGrid 
    {
        public static PlacedAugmentedGrid Create(Vector2Int origin, AugmentGrid gridPlaced)
        {
            PlacedAugmentedGrid placed = new PlacedAugmentedGrid( origin, gridPlaced);

            return placed;
        }
        public PlacedAugmentedGrid(Vector2Int origin,  AugmentGrid grid) {
            this.origin = origin;
         //   this.direction = grid.dir;
            this.placedObject = grid;
        }

        public readonly Vector2Int origin;
       // private readonly Dir direction;
        private readonly AugmentGrid placedObject;
        public AugmentGrid GetPlaceGrid
        {
            get { return placedObject; }
        }
    }
}