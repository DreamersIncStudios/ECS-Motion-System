// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Saves the FactionManager's FactionDatabase to the SaveSystem.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(FactionManager))]
    public class FactionManagerSaver : Saver
    {

        private FactionManager m_manager = null;

        public override void Awake()
        {
            base.Awake();
            m_manager = GetComponent<FactionManager>();
        }

        public override string RecordData()
        {
            return m_manager.SerializeToString();
        }

        public override void ApplyData(string s)
        {
            m_manager.DeserializeFromString(s);
        }

    }
}
