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

        int index;
        public  GameMasterSystem GMS;

        protected override void OnCreate()
        {
            base.OnCreate();
            GMS = GameMasterSystem.GMS;
            index = new int();

        }
        public ComponentDataFromEntity<CharControllerE> Control;
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


                    Control = GetComponentDataFromEntity<CharControllerE>(false);
                    var test = Control[GMS.Party[index]];
                    test.AI = true;
                    Control[GMS.Party[index]]= test;

                    index = GMS.PlayerIndex;

                    PostUpdateCommands.AddComponent<Player_Control>(GMS.Party[index]);
                    PostUpdateCommands.RemoveComponent<AI_Control>(GMS.Party[index]);
                    test = Control[GMS.Party[index]];
                    test.AI = false;
                    Control[GMS.Party[index]] = test;

                    Entities.ForEach((ref Player_Control PC, NavMeshAgent Agent) =>
                    {
                        if (Agent.enabled)
                            Agent.enabled = false;
                        Agent.tag = "Player";
                    });
                    Entities.ForEach((ref AI_Control AI, NavMeshAgent Agent) =>
                    {
                        if (!Agent.enabled)
                            Agent.enabled = true;
                        Agent.tag = "Untagged";
                    });

                }
            }

        }
    }
}