using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Archetypes;

namespace MotionSystem.Components
{

    public class SafeZone : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            Entity EntityEnter = Entity.Null;
            if (other.gameObject.CompareTag("Player"))
            {
                EntityEnter = other.GetComponent<CharacterControl>().ObjectEntity;
            }
            if (EntityEnter != Entity.Null)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<InSafeZoneTag>(EntityEnter, new InSafeZoneTag() { InZone = true });
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Entity EntityExit = Entity.Null;
            if (other.gameObject.CompareTag("Player"))
            {
                EntityExit = other.GetComponent<CharacterControl>().ObjectEntity;
            }
            if (EntityExit != Entity.Null)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<InSafeZoneTag>(EntityExit, new InSafeZoneTag() { InZone = false });
            }
        }
    }
        public struct InSafeZoneTag : IComponentData
        {
            public bool InZone;

        }
    
}