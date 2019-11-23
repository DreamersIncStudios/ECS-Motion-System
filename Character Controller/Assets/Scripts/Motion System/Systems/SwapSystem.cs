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
        int index = 0;
        bool StartUp;
        protected override void OnUpdate()
        {

            NativeArray<Entity> PartyArray = GetEntityQuery(Party).ToEntityArray(Allocator.Persistent);
            NavMeshAgent[] Agents = GetEntityQuery(Party).ToComponentArray<NavMeshAgent>();
            NativeArray<Entity> player = GetEntityQuery(Player).ToEntityArray(Allocator.Persistent);
            NavMeshAgent[] AgentsPlayer = GetEntityQuery(Player).ToComponentArray<NavMeshAgent>();


            if (Input.GetKeyUp(KeyCode.P)) {

                if (index >= PartyArray.Length-1)
                {
                    index = 0;
                }
                else
                    index++;
                PostUpdateCommands.RemoveComponent<AI_Control>(PartyArray[index]);
                PostUpdateCommands.AddComponent<Player_Control>(PartyArray[index]);
                Agents[index].enabled = false;
                Agents[index].tag = "Player";
                PostUpdateCommands.RemoveComponent<Player_Control>(player[0]);
                PostUpdateCommands.AddComponent<AI_Control>(player[0]);
                AgentsPlayer[0].enabled = true;
                AgentsPlayer[0].tag = "Untagged";


            }

            PartyArray.Dispose();
            player.Dispose();
        }
    }
}