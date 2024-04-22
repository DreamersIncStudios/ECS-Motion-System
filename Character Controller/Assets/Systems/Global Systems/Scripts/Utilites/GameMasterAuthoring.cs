using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc
{
    public class GameMasterAuthoring : MonoBehaviour
    {

        public class Baking : Baker<GameMasterAuthoring>
        {
            public override void Bake(GameMasterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            }
        }
    }
}
