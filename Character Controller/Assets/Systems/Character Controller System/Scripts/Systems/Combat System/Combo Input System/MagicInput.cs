using UnityEngine;
using Unity.Entities;
using Stats.Entities;
using UnityEngine.InputSystem;


namespace DreamersInc.ComboSystem
{
    public struct AnimationSpeedMod : IComponentData
        {
            public float SpeedValue;
            public float MaxDuration;
        }
    
}