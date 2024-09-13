using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem.Interfaces {
    public interface IArmor {

        ArmorType ArmorType { get; }


        float MaxDurability { get; }
        float CurrentDurability { get; set; }
        bool Breakable { get; }
        bool Upgradeable { get; }
        int SkillPoints { get; set; }
        int Experience { get; set; }

    }

    public enum ArmorType { 
        Shield,Helmet,Chest,Arms,Legs, Signature
    }

}
