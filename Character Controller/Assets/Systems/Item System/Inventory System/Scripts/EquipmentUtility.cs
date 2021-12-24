 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Interfaces;
using Stats;

namespace Dreamers.InventorySystem{ 
public static  class EquipmentUtility 
{
    public static void ModCharacterStats(BaseCharacter character, List<StatModifier> Modifiers, bool Add) {
        int MP = 1;
        if (!Add)
        {
            MP = -1;
        }
        foreach (StatModifier mod in Modifiers)
        {

            switch (mod.Stat)
            {
                case AttributeName.Level:
                    Debug.LogWarning("Level Modding is not allowed at this time. Please contact Programming is needed");
                    break;
                case AttributeName.Strength:
                    character.GetPrimaryAttribute((int)AttributeName.Strength).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Vitality:
                    character.GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Awareness:
                    character.GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Speed:
                    character.GetPrimaryAttribute((int)AttributeName.Speed).BuffValue += mod.BuffValue * MP ;
                    break;
                case AttributeName.Skill:
                    character.GetPrimaryAttribute((int)AttributeName.Skill).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Resistance:
                    character.GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Concentration:
                    character.GetPrimaryAttribute((int)AttributeName.Concentration).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.WillPower:
                    character.GetPrimaryAttribute((int)AttributeName.WillPower).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Charisma:
                    character.GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue += mod.BuffValue * MP;
                    break;
                case AttributeName.Luck:
                    character.GetPrimaryAttribute((int)AttributeName.Luck).BuffValue += mod.BuffValue * MP;
                    break;
            }
        }
        character.StatUpdate();

    }

}
}
