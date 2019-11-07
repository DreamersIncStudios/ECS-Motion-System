using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;
using MotionSystem.Components;
using IAUS.ECS.Component;


namespace MotionSystem.Archetypes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]


    public class CharacterControl : MonoBehaviour,IConvertGameObjectToEntity
    {
        CapsuleCollider Col;
        Rigidbody RB;
        public bool AI_Control;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Col = this.GetComponent<CapsuleCollider>();
            RB = this.GetComponent<Rigidbody>();
            var control = new CharController() { CapsuleRadius = Col.radius, CapsuleCenter = Col.center, CapsuleHeight = Col.height, Mass = RB.mass };
            dstManager.AddComponentData(entity, control);
            if (AI_Control) {
                var move = new Movement() { };
                this.gameObject.AddComponent<NavMeshAgent>();
                dstManager.AddComponentData(entity, move);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}