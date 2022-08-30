using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace LODRenderSystem
{
    public class LODAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<LODTag>(entity);
            dstManager.AddComponent<RenderTag>(entity);
            dstManager.AddComponent<AnimateTag>(entity);

        }
    }

    public class RenderSet : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref AnimateTag tag, Animator anim) => {
                if(!anim.enabled)
                    anim.enabled = true;
            
            });
            Entities.ForEach((ref RenderTag tag, Renderer anim) => {
                if (!anim.enabled)
                    anim.enabled = true;

            });


            Entities.WithNone<AnimateTag>().ForEach(( Animator anim) => {
                if (anim.enabled)
                    anim.enabled = false;

            });
            Entities.WithNone<RenderTag>().ForEach(( Renderer anim) => {
                if (anim.enabled)
                    anim.enabled = false;

            });
        }
    }
}
