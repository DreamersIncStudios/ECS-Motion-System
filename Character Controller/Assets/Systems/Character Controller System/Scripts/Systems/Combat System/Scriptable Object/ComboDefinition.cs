using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DreamersInc.ComboSystem
{
    [System.Serializable]
    public class ComboDefinition
    {
        [FormerlySerializedAs("name")] public string Name;
        public ComboNames ComboEnumName;
        public bool Unlocked { get; set; }
        [NonReorderable] public Queue<AttackType> Test;
    }
}