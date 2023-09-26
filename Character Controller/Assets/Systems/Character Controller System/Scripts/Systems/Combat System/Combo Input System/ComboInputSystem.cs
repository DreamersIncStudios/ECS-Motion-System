using Unity.Entities;
using System.Collections.Generic;
using Stats.Entities;
using Dreamers.InventorySystem;
using MotionSystem.Components;
using UnityEngine;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static readonly int Property = Animator.StringToHash("Weapon In Hand");
        private static readonly int Block = Animator.StringToHash("Block");

        protected override void OnCreate()
        {
            base.OnCreate();
            Entities.WithoutBurst().ForEach((PlayerComboComponent comboList, Animator anim, Command handler, ref Player_Control pc) =>
            {
                handler.InputQueue = new Queue<AnimationTrigger>();

            }).Run();
        }


        //TODO Decouple this code split into small chu
        protected override void OnUpdate()
        {

            if (!SystemAPI.TryGetSingleton<ControllerInfo>(out var pc))
                return;


            Entities.WithoutBurst().ForEach((Command handler, ref CharControllerE controller, in Player_Control Tag) =>
            {
                if (!pc.InSafeZone)
                    controller.CastingTimer = handler.CanInputAbilities;
            }).Run();
            
            
            Entities.WithoutBurst().WithChangeFilter<CharacterInventory>().ForEach((Command handler, CharacterInventory inventory, ref CharControllerE control) =>
            {
                if (inventory.Equipment.EquippedWeapons.TryGetValue(Dreamers.InventorySystem.Interfaces.WeaponSlot.Primary, out WeaponSO weaponSO))
                {
                    handler.AlwaysDrawnWeapon = weaponSO.AlwaysDrawn;
                    control.EquipOverride = weaponSO.AlwaysDrawn;
                }
            }).Run();

      
            Entities.WithoutBurst().ForEach((Entity entity, PlayerComboComponent comboList, Animator Anim, Command handler, ref Player_Control tag) =>
            {

                handler.InputQueue ??= new Queue<AnimationTrigger>();
                handler.MagicInputQueue ??= new Queue<string>();
                if (pc.InSafeZone || pc.Casting || !comboList.WeaponEquipped)
                {
                    // add logic for play to store weapon

                    return;
                }

                if (!Anim.GetBool(Property) && handler.AlwaysDrawnWeapon)
                    Anim.SetBool(Property, true);
                Anim.SetBool(Block, pc.Blockb);



                if (!pc.OpenCadInput)
                {
                    ComboInputHandling(comboList, handler, pc, Anim);
                }
                else if (handler.CanInputAbilities)
                {

                    if (pc.LightAttackb)
                        handler.MagicInputQueue.Enqueue("X");
                    else if (pc.HeavyAttackb)
                        handler.MagicInputQueue.Enqueue("B");
                    else if (pc.Jumpb)
                        handler.MagicInputQueue.Enqueue("A");
                    else if (pc.DodgeB)
                        handler.MagicInputQueue.Enqueue("Y");
                    handler.InputTimer -= SystemAPI.Time.DeltaTime;
                }
                else {
                    MagicInputHandling(entity, handler, pc);

                }


                if (handler.InputTimer != handler.InputTimeReset)
                {
                    if (!pc.CadButtonPressed)
                        handler.InputTimer += 2.5f * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    pc.Casting = true;
                }
                if (handler.InputTimer > handler.InputTimeReset)
                    handler.InputTimer = handler.InputTimeReset;
                MagicInputHandling(entity, handler, pc);
            }).Run();

            AnimationTriggering();
            if (!pc.OpenCadInput)
            {
                DisableSlowMoMode();
            }
            else {
                EnableSlowMoMode();
            }

        }
      
    }
}