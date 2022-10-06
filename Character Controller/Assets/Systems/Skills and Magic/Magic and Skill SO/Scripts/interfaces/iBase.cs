using SkillMagicSystem.AbilityEffects;
using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillMagicSystem
{
    public interface iBase
    {
        string Name { get; }
        string Description { get; }
        public Level Level { get; }
        public int ReqdLevel { get; }
        public int ManaRqd { get; }
        public Targets AbilityTarget { get; }
        public bool CanAdd(int level); //Todo Add more check besides level 
        public bool CanCast(BaseCharacter User);
        public void Activate(BaseCharacter User, BaseCharacter targetCharacter);
        public void Deactivate(BaseCharacter User, BaseCharacter targetCharacter);
        public void Equip(BaseCharacter targetCharacter);
        public void Unequip(BaseCharacter targetCharacter);


    }
    public interface extra
    {
        //public void WriteToTextFile();
        //public PassiveAbility AddPassiveAbility();
        //public ActiveAbillity AddActiveAbility();

    }
    public enum TriggerTypes { OnCommand, OnHit, OnGetHit, OnKill, OnPlayerDeath, OnTimer, OnEquip, OnCommandTimer, OnCommandOnHit }

    public enum Targets { Self, TeamMember, Enemy, Anyone, AOE, Projectile }

    public class BaseAbility : ScriptableObject, iBase
    {

        public string Name { get { return _name; } private set { _name = value; } }
        [SerializeField] string _name;
        public string Description { get { return description; } private set { description = value; } }
        [SerializeField] string description;
        public Level Level { get { return level; } private set { level = value; } }
        [SerializeField] Level level;
        public int ReqdLevel { get { return reqdLevel; } private set { reqdLevel = value; } }
        [SerializeField] int reqdLevel;
        public bool Magic;
        public TriggerTypes Trigger;
        public List<BaseEffect> Effects;
        public Targets AbilityTarget { get { return abilityTarget; } private set { abilityTarget = value; } }
        [SerializeField] Targets abilityTarget;
        public bool CanAdd(int characterlevel)
        {
            return ReqdLevel <= characterlevel;
        }
        public int ManaRqd { get { return manaRqd; } }
        [SerializeField] int manaRqd;
        public bool CanCast(BaseCharacter User)
        {
            return User.CurMana > ManaRqd;

        }


        public virtual void Activate(BaseCharacter User, BaseCharacter targetCharacter = null)
        {
        }
        public virtual void Deactivate(BaseCharacter User, BaseCharacter targetCharacter = null) { }
        public virtual void Equip(BaseCharacter targetCharacter) { }
        public virtual void Unequip(BaseCharacter targetCharacter) { }
    }


    public enum Level { Rookie, Intermed, Master, }
}
