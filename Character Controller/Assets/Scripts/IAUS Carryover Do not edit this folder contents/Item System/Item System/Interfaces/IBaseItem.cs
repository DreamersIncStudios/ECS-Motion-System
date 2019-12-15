using UnityEngine;
namespace ItemSystem
{
    public interface IBaseItem
    {
        string Name { get; set; }
        int ItemID { get; }
        Sprite Icon { get; set; }
        uint Value { get; set; }
        string Description { get; set; }
        ItemType itemType { get; set; }
        bool Consumable { get; }
        bool Stackable { get; }
        bool canBeStored { get; } // as in deep storage system
        uint size { get; set; }
        Quality Quality { get; }

    }

    public interface ISpell
    {
        uint Level { get; set; }
        uint Exp { get; set; }
        ParticleSystem PS { get; set; }
        uint ManaConsumed { get; set; }
      //  Elements Element { get; set; }
        bool Active { get; set; }
        Effect Effector { get; }
    }

  


    //public interface ICad {
    //    OpStatus OpertionalStatus { get; set; }
    //    List<Weapon> Weapons { get; set; }
    //    List<ScriptableObject> Inventory { get; set; }
    //    uint Core { get; set; }
    //    uint SSDSize { get; set; }
    //    uint MDDR4 { get; set; }


    //}
    //public enum Elements {
    //Fire,
    //Water,
    //Air,
    //Earth,
    //Holy,
    //Darkness
    //}

    public enum OpStatus
    {
        Normal,
        Overclocked,
        Lagged,
        Infected,
    }
    public enum ArmorCategories
    {
        head, chest, arms, legs
    }
    public enum Effect
    {
        Null,
        Speed_Up,
        Defense_Up

    }
    public enum Quality {
        Mythic,
        Exotic,
        Legendary,
        Rare,
        Uncommon,
        Common
    }
    public enum ItemType { Armor,Weapon,Quest,Curative}
    
}