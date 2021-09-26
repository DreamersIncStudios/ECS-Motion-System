using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Interfaces;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

namespace Dreamers.InventorySystem {
    [System.Serializable]
    public class RecoveryItemSO : ItemBaseSO, IRecoverItems,IGeneral
    {
        
        [SerializeField] private uint _recoverAmount;
        public uint RecoverAmount { get { return _recoverAmount; } }
        [SerializeField] private uint _iterations;
        public uint Iterations { get { return _iterations; } }
        [SerializeField] private float _freq;
        public float Frequency { get { return _freq; } }
        [SerializeField] RecoverType _recoverWhat;
        public RecoverType RecoverWhat { get { return _recoverWhat; } }
        [SerializeField] private StatusEffect _removeStatus;
        public StatusEffect RemoveStatus { get { return _removeStatus; } }
        [SerializeField] private TypeOfGeneralItem _GeneralType;
        public TypeOfGeneralItem GeneralItemType { get { return _GeneralType; } }


        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            /// Rewrite this system to be entity based 
            Use(characterInventory);
            Material CharacterMaterial = player.GetComponent<Renderer>().material;
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
            PlayerStatComponent pc   =  entityManager.GetComponentData<PlayerStatComponent>(player.selfEntityRef);
            if (Iterations == 0)
            {
                switch (RecoverWhat)
                {
                    case RecoverType.Health:
                        pc.AdjustHealth((int)RecoverAmount);
                        break;
                    case RecoverType.Mana:
                        pc.AdjustMana((int)RecoverAmount);

                        break;
                    case RecoverType.HealthMana:
                        pc.AdjustHealth((int)RecoverAmount);

                        pc.AdjustMana((int)RecoverAmount);

                        break;
                    case RecoverType.Status:
                        //add logic later
                        break;
                    case RecoverType.StatusPlusHealth:
                        //add status logic
                        pc.AdjustHealth((int)RecoverAmount);
                        break;
                    case RecoverType.StatusPlusMana:
                        //add status logic
                        pc.AdjustMana((int)RecoverAmount);
                        break;
                    case RecoverType.All:
                        //add status logic
                        pc.AdjustHealth((int)RecoverAmount);
                        pc.AdjustMana((int)RecoverAmount);
                        break;
                }
            }
            else {
                pc.AdjustHealth((int)RecoverAmount);
                entityManager.AddComponentData(pc.selfEntityRef, new TimedHeal() { Duruation = Frequency, Frequency = Frequency, Iterations = Iterations, RecoverAmount = (int)RecoverAmount });
            }
        }

        public override void Convert(Entity entity, EntityManager dstManager)
        { 
        
        }
        void Activate() { 
        
        
        }
        public bool Equals(ItemBaseSO obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj.Type != Type)
                return false;

            // TODO: write your implementation of Equals() here

           RecoveryItemSO Recover = (RecoveryItemSO)obj;

            return ItemID == Recover.ItemID && ItemName == Recover.ItemName && Value == Recover.Value && RecoverAmount == Recover.RecoverAmount &&
                RecoverWhat == Recover.RecoverWhat && GeneralItemType == Recover.GeneralItemType && Iterations == Recover.Iterations;
        }


    }

    public interface IRecoverItems {
        uint RecoverAmount { get; }
        uint Iterations { get; }
        float Frequency { get; }
        RecoverType RecoverWhat { get; }
        StatusEffect RemoveStatus { get; }
    }


    public enum RecoverType{
        Health,Mana, HealthMana, Status, StatusPlusHealth, StatusPlusMana, All
    }
    public struct TimedHeal : IComponentData {
        public float Duruation;
        public int RecoverAmount;
        public uint Iterations;
        public float Frequency;
        public bool RemoveComponent => Iterations <= 0;
    }

    public class TimedHealSystem : SystemBase {

        EntityQuery TimedHealEntities;
        EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            TimedHealEntities = GetEntityQuery(new EntityQueryDesc() { 
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(PlayerStatComponent)), ComponentType.ReadWrite(typeof(TimedHeal))}
            });

            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        }
        protected override void OnUpdate()
        {
            JobHandle systemDeps = Dependency;
            systemDeps = new TimeHealJob() {
                PlayerChunk = GetComponentTypeHandle<PlayerStatComponent>(false),
                HealChunk = GetComponentTypeHandle<TimedHeal>(false),
                EntitiesChunk = GetEntityTypeHandle(),
                CMDBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                DT = Time.DeltaTime
            }.ScheduleParallel(TimedHealEntities, systemDeps);
            _entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

            Dependency = systemDeps;
        }

        public struct TimeHealJob : IJobChunk
        {
            public ComponentTypeHandle<PlayerStatComponent> PlayerChunk;
            public ComponentTypeHandle<TimedHeal> HealChunk;
           [ReadOnly] public EntityTypeHandle EntitiesChunk;
            public EntityCommandBuffer.ParallelWriter CMDBuffer;
            public float DT;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<PlayerStatComponent> Players = chunk.GetNativeArray(PlayerChunk);
                NativeArray<TimedHeal> Heals = chunk.GetNativeArray(HealChunk);
                NativeArray<Entity> Entities = chunk.GetNativeArray(EntitiesChunk);

                for (int i = 0; i < chunk.Count; i++)
                {
                    TimedHeal heal = Heals[i];
                    if (heal.RemoveComponent)
                    {
                        CMDBuffer.RemoveComponent<TimedHeal>(chunkIndex,Entities[i]);
                    }
                    else {
                        if (heal.Duruation <= 0.0f)
                        {
                            Players[i].AdjustHealth(heal.RecoverAmount);
                            heal.Duruation = heal.Frequency;
                        }
                        else {
                            heal.Duruation -= DT;
                        }
                    }
                    Heals[i] = heal;
                }
            }
        }
    }
}