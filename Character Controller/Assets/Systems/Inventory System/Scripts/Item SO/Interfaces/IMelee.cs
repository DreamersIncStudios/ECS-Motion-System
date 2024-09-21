namespace Dreamers.InventorySystem
{

    public interface IMeleeWeapon
    {
        
        float MaxDurability { get; }
        float CurrentDurability { get; set; }
        bool Breakable { get; }
    }
}