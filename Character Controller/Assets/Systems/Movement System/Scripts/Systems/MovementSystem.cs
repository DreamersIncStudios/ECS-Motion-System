
using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using Components.MovementSystem;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace IAUS.ECS.Systems
{
   //[UpdateAfter(typeof(IAUS_UpdateState))]

    public partial  
    public partial class MovementSystem : SystemBase
    {
        private EntityQuery Mover;


        protected override void OnCreate()
        {
            base.OnCreate();
            Mover = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Movement)), ComponentType.ReadOnly(typeof(LocalToWorld))}

            });

        }
        protected override void OnUpdate()
        {
            JobHandle systemDeps = Dependency;

            systemDeps = new UpdateDistanceRemaining();

            Dependency = systemDeps;

            Entities.WithoutBurst().ForEach((NavMeshAgent Agent, ref Movement move) =>
            {
                if (move.CanMove)
                {
                    //rewrite with a set position bool;
                    if (move.SetTargetLocation)
                    {
                        Agent.SetDestination(move.TargetLocation);
                        Agent.isStopped = false;
                        move.SetTargetLocation = false;
                    }



                    if (Agent.hasPath)
                    {
                        if (move.WithinRangeOfTargetLocation)
                        {
                            move.CanMove = false;
                        }
                    }
                }
                else
                {
                    Agent.isStopped = true;

                }


            }).Run();


        }

        public struct UpdateDistanceRemaining : IJobEntity
        {
            public ComponentTypeHandle<Movement> MovementChunk;
            [ReadOnly] public ComponentTypeHandle<LocalToWorld> CurrentPointChunk;

            public void Execute(ref WorldTransform CurPos, ref Movement movement)
            {

                movement.DistanceRemaining = Vector3.Distance(movement.TargetLocation, CurPos.Position);


            }
        }
    }

}