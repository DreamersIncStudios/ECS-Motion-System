using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public interface IArmor
    {
        int SpeedMod {get;}
        int ResistanceMod { get; }
        int VitalityMod { get; }
        int CharismaMod { get; }
        int Awareness { get; }
        ArmorType armor { get; }
        
    }
    public interface IRendered
    {
        GameObject GO { get; }
        HumanBodyBones SpawnBone { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; } 

    }
    public interface IEquipment
    {
        // considering the possiblity of equipment leveling up with character
        uint Level { get; set; }
        uint Exp { get; set; }


    }
    public enum ArmorType {
        Helmet,
        Chest,
        Gloves,
        Boots,
        Shield
    }
}
