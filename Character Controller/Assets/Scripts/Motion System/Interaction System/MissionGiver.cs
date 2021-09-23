using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionSystem.Interaction {
    public class MissionGiver :  StoreFront
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