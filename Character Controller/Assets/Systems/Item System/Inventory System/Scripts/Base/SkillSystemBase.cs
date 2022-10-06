using System.Collections.Generic;
using SkillMagicSystem;

namespace Dreamers.InventorySystem.Base
{
    [System.Serializable]
    public class SkillSystemBase {
        public List<Magic> MagicInventory;
        public List<Skill> SkillInventory;

        public List<Magic> EquippedMagic;
        public List<Skill> EquippedSkill;

    }
}