using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using UnityEngine.AI;

namespace MotionSystem.Archetypes
{
    public class SwapSystem : ComponentSystem
    {
        EntityQueryDesc Party = new EntityQueryDesc() {
            All = new ComponentType[] { typeof(PlayerParty), typeof(NavMeshAgent) },

        };
        EntityQueryDesc Player = new EntityQueryDesc()
        {
            All= new ComponentType[] { typeof(PlayerParty), typeof(Player_Control), typeof(NavMeshAgent)}
        };
        int index;
        public  GameMasterSystem GMS;

        protected override void OnCreate()
        {
            base.OnCreate();
            GMS = GameMasterSystem.GMS;
            index = new int();

        }

        protected override void OnUpdate()
        {
            if (GMS == null) {
                GMS = GameMasterSystem.GMS;
                index = new int();
            }
            else
            {
                if (GMS.PlayerIndex != index)
                {
                    PostUpdateCommands.RemoveComponent<Player_Control>(GMS.Party[index]);
                    PostUpdateCommands.AddComponent<AI_Control>(GMS.Party[index]);

                    index = GMS.PlayerIndex;

                    PostUpdateCommands.AddComponent<Player_Control>(GMS.Party[index]);
                    PostUpdateCommands.RemoveComponent<AI_Control>(GMS.Party[index]);

                    Entities.ForEach((ref Player_Control PC, NavMeshAgent Agent) =>
                    {
                        if (Agent.enabled)
                            Agent.enabled = false;
                    });
                    Entities.ForEach((ref AI_Control AI, NavMeshAgent Agent) =>
                    {
                        if (!Agent.enabled)
                            Agent.enabled = true;
                    });

                }
            }

        }
    }
}