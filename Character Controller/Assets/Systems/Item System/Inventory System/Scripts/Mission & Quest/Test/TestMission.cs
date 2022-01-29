using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.MissionSystem;
//TODO Delete Later
namespace Dreamers.InventorySystem.Testing
{
    public class TestMission : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                MissionHub hub = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterInventory>().QuestLog;
                hub.OnKill(NPC.test1);

            }
        }
    }
}