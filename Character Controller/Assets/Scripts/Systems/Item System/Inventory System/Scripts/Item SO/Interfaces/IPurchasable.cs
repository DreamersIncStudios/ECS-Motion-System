using UnityEngine;
using Dreamers.InventorySystem;

namespace Dreamers.InventorySystem.Interfaces
{
    public interface IPurchasable
    {
        uint Value { get; }
        uint MaxStackCount { get; }
        bool Stackable { get; }
    }
}