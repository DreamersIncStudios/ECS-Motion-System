using Unity.Entities;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using MotionSystem.Components;
using UnityEngine;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static readonly int WeaponHand = Animator.StringToHash("Weapon In Hand");
        private static readonly int Block = Animator.StringToHash("Block");
        
        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
            RequireForUpdate<ControllerInfo>();

            
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

                if (!Anim.GetBool(WeaponHand) && handler.AlwaysDrawnWeapon)
                    Anim.SetBool(WeaponHand, true);
                

                if (handler.CanInputAbilities)
                {
                    handler.InputTimer -= SystemAPI.Time.DeltaTime;
                }
                else {
                    MagicInputHandling(entity, handler, pc);

                }

     
            }).Run();

            AnimationTriggering();
    
        }
      
    }
}