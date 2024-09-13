﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dreamers.InventorySystem.Interfaces
{
    public interface IWeapon
    {
        HumanBodyBones HeldBone { get; }
        WeaponType WeaponType { get; }
        WeaponSlot Slot { get; }
        float MaxDurability { get; }
        float CurrentDurability { get; set; }
        bool Breakable { get; }
        bool Upgradeable { get; }
        int SkillPoints { get; set; }
        int Experience { get; set; }

        Vector3 SheathedPos { get; }
        Vector3 HeldPos { get; }

        Vector3 SheathedRot { get; }
        Vector3 HeldRot { get; }

        Vector3 StyleHeldPost { get; }
        Vector3 StyleHeldRot { get; }
        //List skills SOs
    }
    public enum WeaponType { 
        Sword, H2BoardSword, Katana, Bo_Staff, Mage_Staff, Club, Pistol, Bow, Axe, Gloves, SpellBook, SpellBlade, Claws
    }
    public enum WeaponSlot { Primary, Secondary, Projectile}
}