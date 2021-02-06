using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace AISenses.VisionSystems
{
    [UpdateAfter(typeof(VisionSystem))]
    public class TargetingSystem : SystemBase
    {
        EntityQuery Player;
        EntityQuery PlayersParty;

        protected override void OnCreate()
        {
            base.OnCreate();
            Player = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)),ComponentType.ReadOnly(typeof(Player_Control)) },

            });
           PlayersParty = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)), ComponentType.ReadOnly(typeof(PlayerParty))},
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(Player_Control)) }

            });

        }
        int index = 0;
        protected override void OnUpdate()
        {
            if ()
            {


            }
            else {
                index = 0;
            }
        }
    }
}