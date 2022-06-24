// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Enables a component when the scene starts.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class EnableOnStart : MonoBehaviour
    {
        [Tooltip("Enable this component when on start.")]
        [SerializeField]
        private Component m_component;

        public Component component
        {
            get { return m_component; }
            set { m_component = value; }
        }

        private void Start()
        {
            ComponentUtility.SetComponentEnabled(m_component, true);
        }
    }
}
