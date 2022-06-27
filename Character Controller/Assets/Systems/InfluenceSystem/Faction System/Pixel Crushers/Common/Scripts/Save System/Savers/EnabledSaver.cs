// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the enabled/disabled state of a component. This component should be
    /// on a GameObject that's guaranteed to be active, or it won't take effect.
    /// </summary>
    [AddComponentMenu("")]
    public class EnabledSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool enabled;
        }

        [Tooltip("Component to watch.")]
        [SerializeField]
        private Component m_componentToWatch;

        public Component componentToWatch
        {
            get { return m_componentToWatch; }
            set { m_componentToWatch = value; }
        }

        private Data m_data = new Data();

        public override string RecordData()
        {
            var value = (componentToWatch != null) ? ComponentUtility.IsComponentEnabled(componentToWatch) : false;
            m_data.enabled = value;
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            if (componentToWatch == null || string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s, m_data);
            if (data == null) return;
            m_data = data;
            ComponentUtility.SetComponentEnabled(componentToWatch, data.enabled);
        }

    }
}
