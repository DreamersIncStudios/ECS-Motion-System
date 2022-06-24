// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Instantiates prefabs on Awake.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class InstantiatePrefabs : MonoBehaviour
    {

        [Tooltip("Make instances children of this parent. If unassigned, use this GameObject.")]
        [SerializeField]
        private Transform m_parent;

        [Tooltip("Prefabs to instantiate.")]
        [SerializeField]
        private GameObject[] m_prefabs = new GameObject[0];

        public enum Position { ScreenSpaceUI, OriginalPosition, ParentPosition }

        [Tooltip("Untick for screen-space GameObjects such as UI elements; tick for world-space GameObjects.")]
        [SerializeField]
        private Position m_position = Position.ScreenSpaceUI;

        private void OnEnable()
        {
            if (m_parent == null) m_parent = this.transform;
            for (int i = 0; i < m_prefabs.Length; i++)
            {
                var prefab = m_prefabs[i];
                if (prefab != null)
                {
                    var instance = (m_position == Position.ParentPosition)
                        ? Instantiate(prefab, m_parent.position, m_parent.rotation) as GameObject
                        : Instantiate(prefab) as GameObject;
                    if (instance == null)
                    {
                        Debug.LogWarning("Instantiate Prefabs was unable to instantiate " + prefab, this);
                    }
                    else
                    {
                        instance.transform.SetParent(m_parent, (m_position != Position.ScreenSpaceUI));
                        instance.name = prefab.name;
                    }
                }
            }
            Destroy(this);
        }

    }
}