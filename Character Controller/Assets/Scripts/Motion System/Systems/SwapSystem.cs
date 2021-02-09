using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using UnityEngine.AI;
using Unity.Jobs;
using DreamersStudio.CameraControlSystem;
using GameMaster;


namespace MotionSystem.System
{
    public class SwapSystem : ComponentSystem
    {

        int index;
        public  GameMasterSystem GMS;
        public ControllerScheme InputSet;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            InputSet = GameMasterSystem.GMS.InputSettings.UserScheme;
        }

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
        bool HasChararacterToSwitchTo => GMS.Party.Count > 1;
        bool NotInTargetMode => Input.GetAxis("Target Trigger") <= 0.15f;
        protected override void OnUpdate()
        {

     


            if (GMS == null) {
                GMS = GameMasterSystem.GMS;
                index = new int();
            }
            else
            {
                
                if (GMS.PlayerIndex != index )
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
                    Entities.With(GetEntityQuery(Player)).ForEach((ref Player_Control PC, NavMeshAgent Agent )=>
                    {
                        if (Agent.enabled)
                        {
                            Agent.enabled = false;
                            //   Camera.main.GetComponentInParent<UnityStandardAssets.Cameras.AutoCam>().Target = Agent.gameObject.transform;
                            // Getupdate virtual camera;
                        
                                CameraControl.Instance.SwapFocus(Agent.gameObject.transform);
                            
                        }
                        Agent.gameObject.tag = "Player";
                    });

                    Entities.With(GetEntityQuery(Party)).ForEach((ref AI_Control AI, NavMeshAgent Agent) =>
                    {
                        if (!Agent.enabled &&AI.IsGrounded )
                            Agent.enabled = true;
                        Agent.gameObject.tag = "Untagged";
                    });

                if (!Input.GetKey(InputSet.ActivateCADMenu) &&  NotInTargetMode && HasChararacterToSwitchTo)
                     {
                   
                    if (Input.GetAxis("Quick Acces Horizontal") > .5f)
                    {
                        GMS.PlayerIndex = 0;
                    }
                    if (Input.GetAxis("Quick Acces Horizontal") == -1) {
                        GMS.PlayerIndex = 1;
                    }
                    if (Input.GetAxis("Quick Acces Vertical") == -1) {
                        GMS.PlayerIndex = 2;
                    } 
                }

            }

        }
    }
    public class LinkAI :SystemBase
    {
        private EntityQuery AIQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            AIQuery = GetEntityQuery(new EntityQueryDesc() { 
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(CharControllerE)),ComponentType.ReadWrite(typeof(AI_Control))}
            });
        }
        protected override void OnUpdate()
        {
            JobHandle systemDeps = Dependency;
            systemDeps = new LinkAIJob()
            {
                ControlChunk = GetComponentTypeHandle<AI_Control>(false),
                ControllerChunk = GetComponentTypeHandle<CharControllerE>(true)
            }.ScheduleParallel(AIQuery, systemDeps);
     Dependency = systemDeps;

        }

        struct LinkAIJob : IJobChunk
        {
            public ComponentTypeHandle<AI_Control> ControlChunk;
            [ReadOnly]public ComponentTypeHandle<CharControllerE> ControllerChunk;
     

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<AI_Control> Controls = chunk.GetNativeArray(ControlChunk);
                NativeArray<CharControllerE> characters = chunk.GetNativeArray(ControllerChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    AI_Control AI = Controls[i];
                    CharControllerE charController = characters[i];
                    AI.IsGrounded = charController.IsGrounded;

                    Controls[i] = AI;
                }
            }
        }

   
    }
}