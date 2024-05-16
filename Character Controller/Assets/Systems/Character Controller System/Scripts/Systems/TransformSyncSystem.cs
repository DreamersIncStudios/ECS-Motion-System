using DreamersInc;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace MotionSystem
{
    public partial class TransformSyncSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().WithNone<Player_Control>().ForEach((Transform go, ref LocalTransform local) =>
            {
                go.transform.position = local.Position;
                go.transform.rotation = local.Rotation;
            }).Run();
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Transform go, ref LocalTransform local) =>
            {
                local.Position = go.transform.position;
                local.Rotation = go.transform.rotation;
            }).Run();
        }
    }
}