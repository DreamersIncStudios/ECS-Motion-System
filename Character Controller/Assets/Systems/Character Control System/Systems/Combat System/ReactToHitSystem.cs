using UnityEngine;
using Unity.Entities;
using DreamersInc.CombatSystem.Animation;

namespace DreamersInc.ComboSystem
{
    public class ReactToHitSystem : ComponentSystem
    {

        EntityCommandBuffer commandBuffer;

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref ReactToContact contact, Animator anim, Rigidbody rb) => {
                //Todo Add check to see if we can interrupt 
                Direction dir = contact.HitDirection(out Vector3 dirToTarget);
                if (contact.HitIntensity < 5)
                {
                    switch (dir)
                    {
                        case Direction.Left:
                            anim.Play("HitLeft", 0);
                            break;
                        case Direction.Right:
                            anim.Play("HitRight", 0);
                            break;
                        case Direction.Front:
                            anim.Play("HitFront", 0);
                            break;
                        case Direction.Back:
                            anim.Play("HitBack", 0);
                            break;
                    }
                }
                else
                {
                    switch (dir)
                    {
                        case Direction.Left:
                            anim.Play("HitLeftStrong", 0);
                            break;
                        case Direction.Right:
                            anim.Play("HitRightStrong", 0);
                            break;
                        case Direction.Front:
                            anim.Play("HitFrontStrong", 0);
                            break;
                        case Direction.Back:
                            anim.Play("HitBackStrong", 0);
                            break;
                    }
                }
                rb.AddForce(dirToTarget * contact.HitIntensity, ForceMode.Impulse);
                EntityManager.RemoveComponent<ReactToContact>(entity);
            });
        }

  
       
    }
}