using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DreamerInc.PhysicsSpawnSystem
{

    public class SimpleColliderSpawn : MonoBehaviour
    {
        [SerializeField] private Collider col;

        class ColliderAuthorBaker : Baker<SimpleColliderSpawn>
        {
            public override void Bake(SimpleColliderSpawn authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                switch (authoring.col)
                {
                    case BoxCollider box:
                        AddComponent(entity, new BoxColliderData(box));
                        break;
                    case CapsuleCollider capsule :
                        AddComponent(entity, new CapsuleColliderData(capsule));
                        break;
                }

            }
        }
    }
    

    partial struct BasicColliderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            CreateCapsuleCollider(ref state);
            CreateBoxCollider(ref state);
        }

        

        private void CreateBoxCollider(ref SystemState state)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<BoxColliderData>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var boxData = SystemAPI.GetComponent<BoxColliderData>(entity);
                var entityTransform = SystemAPI.GetComponent<LocalToWorld>(entity);
                var spawnedCollider = new GameObject().AddComponent<BoxCollider>();
               
                spawnedCollider.center = boxData.Center ;
                spawnedCollider.size = boxData.Size;

                SetColliderTransform(spawnedCollider.transform, entityTransform);


                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider
                });

            }
        }
        private void CreateCapsuleCollider(ref SystemState state)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<CapsuleColliderData>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var capsuleData = SystemAPI.GetComponent<CapsuleColliderData>(entity);
                var entityTransform = SystemAPI.GetComponent<LocalToWorld>(entity);
                var spawnedCollider = new GameObject().AddComponent<CapsuleCollider>();
               
                spawnedCollider.center = capsuleData.Center ;
                spawnedCollider.height = capsuleData.Height;
                spawnedCollider.radius = capsuleData.Radius;
                SetColliderTransform(spawnedCollider.transform, entityTransform);
                
                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider
                });

            }
        }
        private void SetColliderTransform(Transform colliderTransform, LocalToWorld entityTransform)
        {
            colliderTransform.position = entityTransform.Position;
            colliderTransform.rotation = entityTransform.Rotation;
            colliderTransform.localScale = entityTransform.Value.Scale();
        }
        
    }
}

