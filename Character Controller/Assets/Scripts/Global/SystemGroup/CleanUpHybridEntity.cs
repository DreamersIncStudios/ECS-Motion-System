using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

namespace Utilities.ECS
{

    public class CleanUpHybridEntity : ComponentSystem
    {
        protected override void OnUpdate()
        {

            Entities.ForEach((Entity entity, ref DestroyTag tag) =>
            {
                PostUpdateCommands.DestroyEntity(entity);
            });
        }

    }

}