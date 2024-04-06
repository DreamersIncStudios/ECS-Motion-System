using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Utilities.ReactiveSystem;
using DreamersInc.ComboSystem;
using MotionSystem.Components;
using Stats.Entities;
using Unity.Jobs;

partial class SpeedModSystem : SystemBase
{
    private static readonly int AnimSpeed = Animator.StringToHash("AnimSpeed");

    protected override void OnUpdate()
    {

        Entities.WithoutBurst().ForEach((Animator anim, ref AnimationSpeedMod tag, ref CharControllerE controller) =>
        {
            if (controller.Slowed) return;
            anim.SetFloat(AnimSpeed, .15f);
            controller.Slowed = true;

        }).Run();


        Entities.WithoutBurst()
            .WithNone<AnimationSpeedMod>().ForEach((Animator anim, ref CharControllerE controller) =>
            {
                if (!controller.Slowed) return;
                anim.SetFloat(AnimSpeed, 1.0f);
                controller.Slowed = false;
            }).Run();
    }
}