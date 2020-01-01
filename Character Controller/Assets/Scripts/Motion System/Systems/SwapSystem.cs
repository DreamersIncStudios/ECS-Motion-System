using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using UnityEngine.AI;
using Unity.Jobs;
using Unity.Burst;
<<<<<<< HEAD
using GameMaster;

=======
>>>>>>> 8ff764df9e264f29935341465e14496604252a57

namespace MotionSystem.System
{
    public class SwapSystem : ComponentSystem
    {

        int index;
        public  GameMasterSystem GMS;

        EntityQueryDesc Party = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(PlayerParty), typeof(NavMeshAgent), typeof(CharControllerE)},
            None = new ComponentType[]  { typeof(Player_Control)}

        };

        EntityQueryDesc Player = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(PlayerParty), typeof(Player_Control), typeof(NavMeshAgent) }
        };
        protected override void OnCreate()
        {
            base.OnCreate();
            GMS = GameMasterSystem.GMS;
            index = 0;

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
                    Control[GMS.Party[index]] = test;

                    index = GMS.PlayerIndex;

                    PostUpdateCommands.AddComponent<Player_Control>(GMS.Party[index]);
                    PostUpdateCommands.RemoveComponent<AI_Control>(GMS.Party[index]);
                    test = Control[GMS.Party[index]];
                    test.AI = false;
                    Control[GMS.Party[index]] = test;
                }
                    Entities.With(GetEntityQuery(Player)).ForEach((ref Player_Control PC, NavMeshAgent Agent) =>
                    {
                        if (Agent.enabled)
                        {
                            Agent.enabled = false;
                            Camera.main.GetComponentInParent<UnityStandardAssets.Cameras.AutoCam>().Target = Agent.gameObject.transform;

                        }
                        Agent.gameObject.tag = "Player";
                    });

                    Entities.With(GetEntityQuery(Party)).ForEach((ref AI_Control AI, NavMeshAgent Agent) =>
                    {
                        if (!Agent.enabled &&AI.IsGrounded )
                            Agent.enabled = true;
                        Agent.gameObject.tag = "Untagged";
                    });

                if (Input.GetAxis("Quick Acces Horizontal") >.5f)
                {
                    GMS.PlayerIndex = 0;
                    Debug.Log("Pressed");
                }
                if (Input.GetAxis("Quick Acces Horizontal") ==-1) {
                    GMS.PlayerIndex = 1;
                }
                if (Input.GetAxis("Quick Acces Vertical") == -1) {
                    GMS.PlayerIndex = 2;
                }

            }

        }
    }
    public class LinkAI : JobComponentSystem
    {
    
        struct LinkAIJob : IJobForEach<AI_Control, CharControllerE>
        {
            public void Execute(ref AI_Control c0, ref CharControllerE c1)
            {
                c0.IsGrounded = c1.IsGrounded;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new LinkAIJob();
            return job.Schedule(this, inputDeps);
        }
    }
}