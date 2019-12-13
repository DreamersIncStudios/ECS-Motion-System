using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItemSystem
{
    public interface ICuratives 
    {
        uint HealthRestored { get; }
        uint ManaRestored { get; }
    }
    public interface IStatBooseter {
        float Duration { get; }
        int[] AttributeMod { get; set; }
    }
    public enum CurativeType {
        Healing,
        StatusChange, // add to late need to figure out status like burning, stone, frozen, mute 
    }

}
