using UnityEngine;
using Unity.Entities;
using Stats.Entities;


namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static void MagicInputHandling(Entity entity, Command handler, ControllerInfo PC)
        {
            if (!handler.CanInputAbilities || !PC.OpenCadInput && handler.HasMagicSpell)
            {
                string output = "";
                while (handler.HasMagicSpell)
                {
                    output += handler.MagicInputQueue.Dequeue();
                }
                var skill = handler.EquippedAbilities.GetAbility(output);
                if (skill)
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


        public void EnableSlowMoMode()
        {
            Entities.WithoutBurst().WithStructuralChanges().WithNone<AnimationSpeedMod>().ForEach((Entity entity, AnimatorComponent animC ) => {
                //Todo add range limit;
                EntityManager.AddComponentData(entity, new AnimationSpeedMod() {
                        SpeedValue = .15f
                });


            }).Run();

        }

        public void DisableSlowMoMode()
        {
            Entities.WithoutBurst().WithStructuralChanges().WithAll<AnimationSpeedMod>().ForEach((Entity entity, AnimatorComponent animC) => {
                EntityManager.RemoveComponent<AnimationSpeedMod>(entity);

            }).Run();

        }
    }


        public struct AnimationSpeedMod : IComponentData
        {
            public float SpeedValue;
            public float MaxDuration;
        }
    
}