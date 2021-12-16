using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamersInc.ComboSystem;

namespace DreamersInc.NPCActions {
    public struct ComboAction {
        Queue<AnimationTriggers> InputKeys;
        [Range(0, 1)]
        public float Chance;
    }
}
