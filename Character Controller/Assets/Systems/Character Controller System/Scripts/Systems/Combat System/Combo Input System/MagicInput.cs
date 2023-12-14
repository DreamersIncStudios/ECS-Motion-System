using UnityEngine;
using Unity.Entities;
using Stats.Entities;


namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static void MagicInputHandling(Entity entity, Command handler, ControllerInfo PC)
        {
            if (handler.CanInputAbilities && (PC.OpenCadInput || !handler.HasMagicSpell)) return;
            var output = "";
            while (handler.HasMagicSpell)
            {
                output += handler.MagicInputQueue.Dequeue();
            }
            var skill = handler.EquippedAbilities.GetAbility(output);
            // ReSharper disable once Unity.BurstLoadingManagedType
            if (skill != null)
            {
                skill.Activate(entity);
                handler.InputQueue.Enqueue(new AnimationTrigger()
                {
                    attackType = AttackType.SpecialAttack,
                    triggerAnimIndex = skill.AnimInfo.AnimIndex,
                    TransitionDuration = skill.AnimInfo.TransitionDuration,
                    TransitionOffset = skill.AnimInfo.TransitionOffset,
                    EndofCurrentAnim = skill.AnimInfo.EndofCurrentAnim

                });
            }
            else
                Debug.Log($"{output} not recognize or ability not equipped");
        }


    
    }


        public struct AnimationSpeedMod : IComponentData
        {
            public float SpeedValue;
            public float MaxDuration;
        }
    
}