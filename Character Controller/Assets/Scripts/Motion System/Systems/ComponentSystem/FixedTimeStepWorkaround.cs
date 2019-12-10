using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace MotionSystem.System {
    public class FixedTimeStepWorkaround : MonoBehaviour
    {
        private AnimatorUpdate AnimUpdate;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void FixedUpdate()
        {
            if (AnimUpdate == null)
            {
                AnimUpdate = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimatorUpdate>();
            }
                AnimUpdate.Update(); 
        }
    }

}
