using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DreamerInc.PhysicsSpawnSystem
{

    public class ColliderSpawn : MonoBehaviour
    {
        [SerializeField] private Collider col;

        class ColliderAuthorBaker : Baker<ColliderSpawn>
        {
            public override void Bake(ColliderSpawn authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                switch (authoring.col)
                {
                    case MeshCollider:
                        AddComponent<MeshColliderTag>(entity);
                        break;
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

    public interface IAddMonoBehaviourToEntityOnAnimatorInstantiation
    {
    }

    class ColliderInstantiationData : IComponentData
    {
        public GameObject ColliderGameObject;
    }

    partial struct ColliderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            CreateMeshCollider(state);
            CreateCapsuleCollider(state);
            CreateBoxCollider(state);
        }

        private void CreateMeshCollider(SystemState state)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<Child>().WithAll<MeshColliderTag>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var childEntity = SystemAPI.GetBuffer<Child>(entity)[1].Value;
                var materialMeshInfo = SystemAPI.GetComponent<MaterialMeshInfo>(childEntity);
                var render = World.DefaultGameObjectInjectionWorld.EntityManager
                    .GetSharedComponentManaged<RenderMeshArray>(childEntity);

                var entityTransform = SystemAPI.GetComponent<LocalToWorld>(entity);
                var spawnedCollider = new GameObject().AddComponent<MeshCollider>();
                spawnedCollider.sharedMesh = render.GetMesh(materialMeshInfo);
                SetColliderTransform(spawnedCollider.transform, entityTransform);
                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider.GetComponent<Collider>()
                });
            }
        }

        private void CreateBoxCollider(SystemState state)
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
        private void CreateCapsuleCollider(SystemState state)
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

