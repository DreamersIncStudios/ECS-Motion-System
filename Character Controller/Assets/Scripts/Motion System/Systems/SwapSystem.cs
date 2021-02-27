using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using UnityEngine.AI;
using Unity.Jobs;
using DreamersStudio.CameraControlSystem;
using ControllerSwap;
using Core;


namespace MotionSystem.System
{
    public class SwapSystem : ComponentSystem
    {

        int index;
        public ControllerScheme InputSet => Core.GameMaster.Instance.InputSettings.UserScheme;
        PartySwapSystem Swap => PartySwapSystem.GMS;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        EntityQueryDesc PartyEntity = new EntityQueryDesc()
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
            index = 0;

        }
        public ComponentDataFromEntity<CharControllerE> Control;
        bool HasChararacterToSwitchTo => Swap.Party.Count > 1;
        bool NotInTargetMode => Input.GetAxis("Target Trigger") <= 0.15f;
        protected override void OnUpdate()
        {
 
                if (Swap.PlayerIndex != index )
                {
                    PostUpdateCommands.RemoveComponent<Player_Control>(Swap.Party[index]);
                    PostUpdateCommands.AddComponent<AI_Control>(Swap.Party[index]);


                    Control = GetComponentDataFromEntity<CharControllerE>(false);

                    var test = Control[Swap.Party[index]];
                    test.AI = true;
                    Control[Swap.Party[index]] = test;

                    index = Swap.PlayerIndex;

                    PostUpdateCommands.AddComponent<Player_Control>(Swap.Party[index]);
                    PostUpdateCommands.RemoveComponent<AI_Control>(Swap.Party[index]);
                    test = Control[Swap.Party[index]];
                    test.AI = false;
                    Control[Swap.Party[index]] = test;
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

                    Entities.With(GetEntityQuery(PartyEntity)).ForEach((ref AI_Control AI, NavMeshAgent Agent) =>
                    {
                        if (!Agent.enabled &&AI.IsGrounded )
                            Agent.enabled = true;
                        Agent.gameObject.tag = "Untagged";
                    });

                if (!Input.GetKey(InputSet.ActivateCADMenu) &&  NotInTargetMode && HasChararacterToSwitchTo)
                     {
                   
                    if (Input.GetAxis("Quick Acces Horizontal") > .5f)
                    {
                    Swap.PlayerIndex = 0;
                    }
                    if (Input.GetAxis("Quick Acces Horizontal") == -1) {
                    Swap.PlayerIndex = 1;
                    }
                    if (Input.GetAxis("Quick Acces Vertical") == -1) {
                    Swap.PlayerIndex = 2;
                    } 
                }

            }

        }
    }
public class LinkAI : SystemBase
{
    private EntityQuery AIQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        AIQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] { ComponentType.ReadOnly(typeof(CharControllerE)), ComponentType.ReadWrite(typeof(AI_Control)) }
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
        [ReadOnly] public ComponentTypeHandle<CharControllerE> ControllerChunk;


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
