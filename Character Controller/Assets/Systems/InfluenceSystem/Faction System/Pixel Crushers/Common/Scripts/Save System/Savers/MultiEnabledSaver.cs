// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the enabled/disabled state of a list of components. This component
    /// should not attempt to save the enabled/disabled state of itself.
    /// </summary>
    [AddComponentMenu("")]
    public class MultiEnabledSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool[] active;
        }

        [Tooltip("Components to watch.")]
        [SerializeField]
        private Component[] m_componentsToWatch;

        public Component[] componentsToWatch
        {
            get { return m_componentsToWatch; }
            set { m_componentsToWatch = value; }
        }

        private Data m_data = new Data();

        public override string RecordData()
        {
            if (componentsToWatch == null) return string.Empty;
            if (m_data.active == null || m_data.active.Length != componentsToWatch.Length)
            {
                m_data.active = new bool[componentsToWatch.Length];
            }
            for (int i = 0; i < componentsToWatch.Length; i++)
            {
                m_data.active[i] = (componentsToWatch[i] != null) ? ComponentUtility.IsComponentEnabled(componentsToWatch[i]) : false;
            }
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            if (componentsToWatch == null || string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s, m_data);
            if (data == null || data.active == null) return;
            m_data = data;
            for (int i = 0; i < Mathf.Min(data.active.Length, componentsToWatch.Length); i++)
            {
                if (componentsToWatch[i] == null) continue;
                ComponentUtility.SetComponentEnabled(componentsToWatch[i], data.active[i]);
            }
        }

    }
}
