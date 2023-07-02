using Unity.Entities;
using System.Collections.Generic;
using Stats.Entities;
using Dreamers.InventorySystem;
using MotionSystem.Components;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Entities.WithoutBurst().ForEach((PlayerComboComponent ComboList, AnimatorComponent anim, Command handler, ref Player_Control PC) =>
            {
                handler.InputQueue = new Queue<AnimationTrigger>();

            }).Run();
        }


        //TODO Decouple this code split into small chu
        protected override void OnUpdate()
        {

            if (!SystemAPI.TryGetSingleton<ControllerInfo>(out var PC))
                return;


            Entities.WithoutBurst().ForEach((Command handler, ref CharControllerE controller, in Player_Control Tag) =>
            {
                if (!PC.InSafeZone)
                    controller.CastingTimer = handler.CanInputAbilities;
            }).Run();
            
            
            Entities.WithoutBurst().WithChangeFilter<CharacterInventory>().ForEach((Command Handler, CharacterInventory inventory, ref CharControllerE control) =>
            {
                if (inventory.Equipment.EquippedWeapons.TryGetValue(Dreamers.InventorySystem.Interfaces.WeaponSlot.Primary, out WeaponSO weaponSO))
                {
                    Handler.AlwaysDrawnWeapon = weaponSO.AlwaysDrawn;
                    control.EquipOverride = weaponSO.AlwaysDrawn;
                }
            }).Run();

      
            Entities.WithoutBurst().ForEach((Entity entity, PlayerComboComponent ComboList, AnimatorComponent animC, Command handler, ref Player_Control tag) =>
            {

                handler.InputQueue ??= new Queue<AnimationTrigger>();
                handler.MagicInputQueue ??= new Queue<string>();
                if (PC.InSafeZone || PC.Casting || !ComboList.WeaponEquipped)
                {
                    // add logic for play to store weapon

                    return;
                }

                var anim = animC.anim;
                if (!anim.GetBool("Weapon In Hand") && handler.AlwaysDrawnWeapon)
                    anim.SetBool("Weapon In Hand", true);
                animC.anim.SetBool("Block", PC.Blockb);



                if (!PC.OpenCadInput)
                {
                    ComboInputHandling(ComboList, handler, PC, anim);
                }
                else if (handler.CanInputAbilities)
                {

                    if (PC.LightAttackb)
                        handler.MagicInputQueue.Enqueue("X");
                    else if (PC.HeavyAttackb)
                        handler.MagicInputQueue.Enqueue("B");
                    else if (PC.Jumpb)
                        handler.MagicInputQueue.Enqueue("A");
                    else if (PC.Projectileb)
                        handler.MagicInputQueue.Enqueue("Y");
                    handler.InputTimer -= SystemAPI.Time.DeltaTime;
                }
                else {
                    MagicInputHandling(entity, handler, PC);

                }


                if (handler.InputTimer != handler.InputTimeReset)
                {
                    if (!PC.CadButtonPressed)
                        handler.InputTimer += 2.5f * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    PC.Casting = true;
                }
                if (handler.InputTimer > handler.InputTimeReset)
                    handler.InputTimer = handler.InputTimeReset;
                MagicInputHandling(entity, handler, PC);
            }).Run();

            AnimationTriggering();
            if (!PC.OpenCadInput)
            {
                DisableSlowMoMode();
            }
            else {
                EnableSlowMoMode();
            }

        }
      
    }
}