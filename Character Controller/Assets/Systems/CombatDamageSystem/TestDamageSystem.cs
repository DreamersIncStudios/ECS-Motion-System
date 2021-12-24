using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamersInc.DamageSystem.Interfaces;
using Stats;

namespace DreamersInc.DamageSystem
{
    public class TestDamageSystem : MonoBehaviour
    {
    public EnemyCharacter DamageThis;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y)) {
                DamageThis.TakeDamage(-100, TypeOfDamage.Melee, Element.None);
            }
        }
    }
}