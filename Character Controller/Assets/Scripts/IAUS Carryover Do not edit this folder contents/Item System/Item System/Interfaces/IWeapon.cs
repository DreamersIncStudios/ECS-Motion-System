using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public interface IWeapon
    {
        WeaponStyle WType { get; }
        Vector3 HandPosition { get; }
        Vector3 HandRotation { get; }

        int StrengthMod { get; }
        int VitalityMod { get; }
        int SpeedMod { get; }
        int SkillMod { get; }
        int ConcentrationMod { get; }

    }


    public enum WeaponStyle { Sword = 0, Two_Handed_Sword = 1, Gloves = 2, Pole = 3, Pistol = 4, Revolver = 5 }

}