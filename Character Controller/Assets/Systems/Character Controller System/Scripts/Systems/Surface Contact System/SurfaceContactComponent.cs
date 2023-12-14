using System.Collections;
using System.Collections.Generic;
using MotionSystem;
using MotionSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace DreamersInc.CharacterControllerSys.SurfaceContact
{

    public struct SurfaceContactComponent : IComponentData
    {
        public int stepsSinceLastGrounded,stepsSinceLastJump,steepContactCount,groundContactCount;
        public float3 Velocity;
        public float speed;
        public float SnapSpeed => 20;
        public bool SnapToGround () {
            if (stepsSinceLastGrounded > 1) {
                return false;
            }
            
            if (OverSnapSpeed)
                return false;
            if (groundNormal.y < minGroundDotProduct) 
                return false;
            groundContactCount = 1;
                ContactNormal = groundNormal;
                var dot = Vector3.Dot(Velocity, groundNormal);
                if (!(dot > 0f)) return false;
                var temp =( (Vector3)(Velocity - groundNormal* dot)).normalized * speed;
                Velocity = temp;
                return true;
        }

        public bool CheckSteepContacts()
        {
            if (steepContactCount <= 1) return false;
            Vector3 steepNormal = SteepNormal;
            steepNormal.Normalize();
            if ((steepNormal.y < minGroundDotProduct)) return false;
            steepContactCount = 0;
            groundContactCount = 1;
            SteepNormal = ContactNormal = steepNormal;
            return true;
            return true;
        }

        public float3 adjustedVelocity;
        public float3 NormalVector;
        public float3 ContactNormal,SteepNormal;
        public float3 groundNormal;
        public bool OverSnapSpeed=> speed >SnapSpeed;
        public float maxGroundAngle => 20;
        public float  maxStairsAngle => 45;
        public float minGroundDotProduct=>  Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        private float minStairsDotProduct => Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);

        public Vector3 ProjectOnContactPlane (float3 vector) {
            return vector - ContactNormal * Vector3.Dot(vector, groundNormal);
        }
    }
[UpdateAfter(typeof(GroundCheckSystem))]
    public partial class TestingSystem : SystemBase 
    {
    

        protected override void OnUpdate()
       
        {
            Entities.WithoutBurst().ForEach((Rigidbody RB, ref SurfaceContactComponent surface) =>
            {
                surface.Velocity = RB.velocity;
                surface.speed = RB.velocity.magnitude;

            }).Run();


            new GroundCheckJob().ScheduleParallel();
            new AdjustVelocity() { DT = SystemAPI.Time.DeltaTime }.ScheduleParallel();
            Entities.WithoutBurst().ForEach((Rigidbody RB, ref SurfaceContactComponent surface) =>
            {
                RB.velocity = surface.adjustedVelocity;

            }).Run();
        }

        partial struct GroundCheckJob: IJobEntity
        {
            
            void Execute(ref SurfaceContactComponent surface, in CharControllerE control)
            {
            
                if (control.IsGrounded || surface.SnapToGround()|| surface.CheckSteepContacts())
                {
                    surface.stepsSinceLastGrounded=0;
                    surface.stepsSinceLastJump++;
                    if (surface.groundContactCount > 1)
                    {
                        Vector3 temp = surface.ContactNormal;
                        temp.Normalize();
                        surface.ContactNormal = temp;
                    }
                }
                else
                {
                    surface.stepsSinceLastGrounded ++;
                    surface.stepsSinceLastJump =0;

                    surface.ContactNormal = Vector3.up;
                }
        

            }
        }

        partial struct AdjustVelocity : IJobEntity
        {
            public float DT;
            void Execute(ref SurfaceContactComponent surface,in LocalTransform transform, in CharControllerE control)
            {
                Vector3 xAxis = surface.ProjectOnContactPlane(Vector3.right).normalized;
                Vector3 zAxis = surface.ProjectOnContactPlane(Vector3.forward).normalized;
                
                float currentX = Vector3.Dot(surface.Velocity, xAxis);
                float currentZ = Vector3.Dot(surface.Velocity, zAxis);
                float acceleration = control.IsGrounded? 300 : 400; //	float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
                float maxSpeedChange = acceleration * DT;
                float newX =
                    Mathf.MoveTowards(currentX, surface.Velocity.x, maxSpeedChange);
                float newZ =
                    Mathf.MoveTowards(currentZ, surface.Velocity.z, maxSpeedChange);

                surface.adjustedVelocity= surface.Velocity + (float3)(xAxis * (newX - currentX) + zAxis * (newZ - currentZ));
                
            }
        }


    }
}
