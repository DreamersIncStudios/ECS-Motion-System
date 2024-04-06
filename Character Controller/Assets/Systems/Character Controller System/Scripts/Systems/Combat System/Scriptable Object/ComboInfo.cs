using UnityEngine.Serialization;

namespace DreamersInc.ComboSystem
{
    [System.Serializable]
    public struct ComboInfo
    {
        [FormerlySerializedAs("name")] public ComboNames Name;
        public bool Unlocked;
    }
}