using UnityEngine;

namespace Prototype.ItemSystem {
    public interface Base {
        string Name { get; }
        string Description { get; }
        int Value { get; }
        Sprite Icon { get; }
        Quality quality { get; }

    }

    public interface Equipable {
        uint BaseLevel { get; }

    }



    public enum Quality
    {
        Mythic,
        Exotic,
        Legendary,
        Rare,
        Uncommon,
        Common
    }

    public enum ItemType
    {
      

    }
}