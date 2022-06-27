using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Mathematics;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.SO;
using Dreamers.InventorySystem.Base;
using Unity.Transforms;
using System.Collections;
using DreamersInc.CombatSystem.Animation;

namespace DreamersInc.ComboSystem
{
    public class ReactToHitSystem : ComponentSystem
    {

        EntityCommandBuffer commandBuffer;

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref ReactToContact contact, Animator anim) => {
                //Todo Add check to see if we can interrupt 
                if (contact.HitIntensity < 5)
                {
                    switch (contact.HitDirection())
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
                    switch (contact.HitDirection())
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
                EntityManager.RemoveComponent<ReactToContact>(entity);
            });
        }

  
       
    }
}