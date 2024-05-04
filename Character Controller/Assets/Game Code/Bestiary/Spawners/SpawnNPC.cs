using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace DreamersInc.BestiarySystem
{
    public sealed partial class BestiaryDB : MonoBehaviour
    {
    

        public static bool SpawnNPC(uint ID, Vector3 Position, out GameObject GO, out Entity entity, bool limitHealth = false, uint healthlimit = 0) {
            
            var info = GetCreature(ID);
            
            if (!info) throw new AssertionException(nameof(ID), $"ID {ID} not valid entry in Database");
           
            if (info.hasAttack)
            {
                new CharacterBuilder(info.Name, out entity)
                    .WithModel(info.Prefab, Position, "Enemy NPC", out GO)
                    .WithStats(info.stats,ID, info.ExpGiven,limitHealth, healthlimit)
                    .WithEntityPhysics(info.PhysicsInfo)
                    .WithInventorySystem(info.Inventory, info.Equipment)
                    .WithAIControl()
                   .WithCharacterDetection()
                    .WithAnimation()
                 //   .WithNPCAttack(info.AttackSequence)
                    .WithMovement(info.Move, info.creatureType,true)
                    .WithFactionInfluence(info.FactionID, 3, 4, 1, true)
                   // .WithAI(info.GetNPCLevel,info.AIStatesToAdd,info.CapableOfMelee,info.CapableOfMagic, info.CapableOfRange)
                   .Build();
                return true;
            }
            
            new  CharacterBuilder(info.Name, out entity)
                .WithModel(info.Prefab, Position, "Enemy NPC", out GO)
                .WithStats(info.stats,ID, info.ExpGiven,limitHealth, healthlimit)
                .WithEntityPhysics(info.PhysicsInfo)
                .WithInventorySystem(info.Inventory, info.Equipment)
                .WithAIControl()
                .WithCharacterDetection()
                .WithAnimation()
                .WithMovement(info.Move, info.creatureType,true)
                .WithFactionInfluence(info.FactionID, 3, 4, 1, true)
               // .WithAI(info.GetNPCLevel,info.AIStatesToAdd,info.CapableOfMelee,info.CapableOfMagic, info.CapableOfRange)
                .Build();
            return true;

           
        }
        public static bool SpawnNPC(uint ID, Vector3 Position, out GameObject GO)
        {
            return SpawnNPC(ID, Position, out GO, out _);
        } 
        
        public static bool SpawnNPC(uint ID, Vector3 Position) {
            return SpawnNPC(ID, Position, out _, out _);
        }
        public static bool SpawnNPC(uint ID, Vector3 Position, uint HealthLimit) {
            return SpawnNPC(ID, Position, out _, out _, true, HealthLimit);
        }
    }
}