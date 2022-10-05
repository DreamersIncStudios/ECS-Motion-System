using AISenses.VisionSystems;
using AISenses.VisionSystems.Combat;
using MotionSystem.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace MotionSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(TargetingSystem))]
    public partial class TargettingSync : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref CharControllerE controller, in AttackTarget target) =>
            {
                controller.Targetting = target.isTargeting;
                if (target.isTargeting) {
                    controller.TimerForEquipReset = 5;
                }
              
            }).ScheduleParallel();
            Entities.ForEach((ref Player_Control player, in CharControllerE controller) =>
            {
                player.Casting = controller.Casting;
            

            }).ScheduleParallel();
        }
    }
}
