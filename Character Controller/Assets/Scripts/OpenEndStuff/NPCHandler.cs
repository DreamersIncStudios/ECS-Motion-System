using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem;
using Dreamers.InventorySystem;
using Unity.Entities;
using Dreamers.Global;

public class NPCHandler : MonoBehaviour,IOnDeathHandler,IConvertGameObjectToEntity
{
    public NPC GetNPCType { get { return getNPCType; } }
    [SerializeField] NPC getNPCType;
    Entity reference;
    public void OnDeath()
    {
        MissionHub hub = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterInventory>().QuestLog;
        hub.OnKill(GetNPCType);
    }
    bool dont = false;
    public void OnApplicationQuit()
    {
        dont = true;
    }

    public void OnDestroy()
    {
        if (!dont)
        {
            OnDeath();
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DestroyTag>(reference);
        }
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        reference = entity;
    }
}
