using UnityEngine;
namespace DreamersInc.BestiarySystem
{
    public sealed partial class BestiaryDB : MonoBehaviour
    {
        public static void SpawnPlayer(uint ID, Vector3 position) //Todo Add Save Load
        {
            var info = GetPlayer(ID);
            new CharacterBuilder(info.Name)
                .WithModel(info.Prefab, position, "Player")
                .WithStats(info.stats,info.ID)
                .WithEntityPhysics(info.PhysicsInfo)
                .WithInventorySystem(info.Inventory, info.Equipment)
                .WithCombat(info.Combo)
                .WithPlayerControl()
                .WithCharacterDetection()
                .WithAnimation()
                .WithVFX()
                .WithMovement(info.Move,CreatureType.biped)
                .WithFactionInfluence(info.factionID, 3, 4, 1, true)
                .Build();
        }
    }

}