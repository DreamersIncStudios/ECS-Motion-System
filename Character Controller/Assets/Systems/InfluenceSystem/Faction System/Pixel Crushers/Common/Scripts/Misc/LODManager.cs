// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// Implements a Level of Detail (LOD) system according to distance from the player.
    /// Add this component to any GameObject with script(s) that implement a method 
    /// named `OnLOD(int)`. It's the script's responsibility to handle the message
    /// accordingly. For example, an AI script could reduce the frequency of perception
    /// checks as the LOD number increases.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class LODManager : MonoBehaviour
    {

        [Serializable]
        public class LOD
        {

            [Tooltip("The minimum distance for this LOD.")]
            [SerializeField]
            private float m_minDistance = 0;

            [Tooltip("The max distance for this LOD.")]
            [SerializeField]
            private float m_maxDistance = Mathf.Infinity;

            /// <summary>
            /// The minimum distance for this LOD.
            /// </summary>
            public float minDistance
            {
                get { return m_minDistance; }
                set { m_minDistance = value; }
            }

            /// <summary>
            /// The max distance for this LOD.
            /// </summary>
            public float maxDistance
            {
                get { return m_maxDistance; }
                set { m_maxDistance = value; }
            }

            public bool Contains(float distance)
            {
                return (minDistance <= distance && distance <= maxDistance);
            }

        }

        [Tooltip("The LODs (levels of detail).")]
        [SerializeField]
        private LOD[] m_levels;

        [Tooltip("The frequency at which to check distance from the player and update the current LOD if necessary.")]
        [SerializeField]
        private float m_monitorFrequency = 5f;

        /// <summary>
        /// The LODs.
        /// </summary>
        public LOD[] levels
        {
            get { return m_levels; }
            set { m_levels = value; }
        }

        /// <summary>
        /// The frequency at which to check distance from the player and update
        /// the current LOD if necessary.
        /// </summary>
        public float monitorFrequency
        {
            get { return m_monitorFrequency; }
            set { m_monitorFrequency = value; }
        }

        /// <summary>
        /// Gets or sets the player's transform. The Start method assigns the
        /// GameObject tagged "Player" to this property.
        /// </summary>
        /// <value>The player.</value>
        public Transform player { get; set; }

        private int m_currentLevel = 0;
        private WaitForSeconds m_currentWaitForSeconds = new WaitForSeconds(60); // Cache to reduce garbage collection.
        private float m_currentWaitForSecondsValue = 0;

        private void Start()
        {
            FindPlayer();
            StartCoroutine(MonitorLOD());
        }

        /// <summary>
        /// Assigns the GameObject tagged "Player" to the player property.
        /// </summary>
        public void FindPlayer()
        {
            var go = GameObject.FindWithTag("Player");
            player = (go != null) ? go.transform : null;
        }

        private IEnumerator MonitorLOD()
        {
            yield return new WaitForSeconds(UnityEngine.Random.value); // Stagger GameObjects.
            m_currentWaitForSecondsValue = -1;
            while (true)
            {
                CheckLOD();
                if (monitorFrequency != m_currentWaitForSecondsValue)
                {
                    m_currentWaitForSecondsValue = monitorFrequency;
                    m_currentWaitForSeconds = new WaitForSeconds(monitorFrequency);
                }
                yield return m_currentWaitForSeconds;
            }
        }

        /// <summary>
        /// Updates the current level of detail based on distance from the player and 
        /// the component's LOD ranges.
        /// </summary>
		public void CheckLOD()
        {
            if (player == null || levels == null || levels.Length == 0) return;
            float distance = Vector3.Distance(transform.position, player.position);
            if (levels[m_currentLevel].Contains(distance)) return;
            for (int level = 0; level < levels.Length; level++)
            {
                if (levels[level].Contains(distance))
                {
                    m_currentLevel = level;
                    BroadcastMessage("OnLOD", level, SendMessageOptions.DontRequireReceiver);
                    return;
                }
            }
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            controllable.Add("monitorFrequency|System.Single|0|99999|1|The frequency at which to check distance from the player and update the current LOD if necessary.");
            return controllable;
        }

    }

}
