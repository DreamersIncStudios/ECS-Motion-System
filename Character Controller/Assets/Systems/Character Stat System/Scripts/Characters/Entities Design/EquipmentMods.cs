using System;
using Stats;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace Stats.Entities
{
    public partial class BaseCharacterComponent : IComponentData
    {
        public void ModCharacterStats(List<StatModifier> modifiers, bool add = true)
        {
            var mp = add ? 1 : -1;
            foreach (var statMod in modifiers)
            {
                switch (statMod.Stat)
                {
                    case StatName.MeleeOffence:
                        GetStat((int)StatName.MeleeDefense).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.MeleeDefense:
                        GetStat((int)StatName.MeleeDefense).BuffValue += statMod.BuffValue * mp;
                        
                        break;
                    case StatName.RangedOffence:
                        GetStat((int)StatName.RangedOffence).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.RangedDefence:
                        GetStat((int)StatName.RangedDefence).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.MagicOffence:
                        GetStat((int)StatName.MagicOffence).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.MagicDefense:
                        GetStat((int)StatName.MagicDefense).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.RangeTarget:
                        GetStat((int)StatName.RangeTarget).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.RangeMotion:
                        GetStat((int)StatName.RangeMotion).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.StatusChange:
                        GetStat((int)StatName.StatusChange).BuffValue += statMod.BuffValue * mp;
                        break;
                    case StatName.ManaRecover:
                        GetStat((int)StatName.ManaRecover).BuffValue += statMod.BuffValue * mp;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }

        public void ModCharacterAttributes(List<AttributeModifier> Modifiers, bool Add = true)
        {
            var MP = Add ? 1 : -1;

            foreach (AttributeModifier mod in Modifiers)
            {

                switch (mod.Attribute)
                {
                    case AttributeName.Level:
                        Debug.LogWarning(
                            "Level Modding is not allowed at this time. Please contact Programming is needed");
                        break;
                    case AttributeName.Strength:
                        GetPrimaryAttribute((int)AttributeName.Strength).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Vitality:
                        GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Awareness:
                        GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Speed:
                        GetPrimaryAttribute((int)AttributeName.Speed).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Skill:
                        GetPrimaryAttribute((int)AttributeName.Skill).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Resistance:
                        GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Concentration:
                        GetPrimaryAttribute((int)AttributeName.Concentration).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.WillPower:
                        GetPrimaryAttribute((int)AttributeName.WillPower).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Charisma:
                        GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue += mod.BuffValue * MP;
                        break;
                    case AttributeName.Luck:
                        GetPrimaryAttribute((int)AttributeName.Luck).BuffValue += mod.BuffValue * MP;
                        break;
                }
            }

            StatUpdate();

        }
    }
}