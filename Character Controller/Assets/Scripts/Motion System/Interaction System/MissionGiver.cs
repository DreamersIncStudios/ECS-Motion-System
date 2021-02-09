using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionSystem.Interaction {
    public class MissionGiver : MonoBehaviour,IInteractable
    {

        public void Interact()
        {
            Debug.Log("Within Interaction range");
        }
        public void OnTriggerStay(Collider other)
        {
            Interact();
        }
    }
}