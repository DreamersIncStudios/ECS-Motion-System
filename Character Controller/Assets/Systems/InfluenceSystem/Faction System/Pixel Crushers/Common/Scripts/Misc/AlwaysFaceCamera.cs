// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Always keeps the GameObject facing the main camera.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class AlwaysFaceCamera : MonoBehaviour
    {

        [Tooltip("Leave Y rotation untouched.")]
        [SerializeField]
        private bool m_yAxisOnly = false;

        [Tooltip("Flip 180 degrees.")]
        [SerializeField]
        private bool m_rotate180 = false;

        /// <summary>
        /// Set `true` to leave Y rotation untouched.
        /// </summary>
        public bool yAxisOnly
        {
            get { return m_yAxisOnly; }
            set { m_yAxisOnly = value; }
        }

        public bool rotate180
        {
            get { return m_rotate180; }
            set { m_rotate180 = value; }
        }

        private Camera m_mainCamera = null;

        private void Update()
        {
            if (m_mainCamera == null || !m_mainCamera.enabled || !m_mainCamera.gameObject.activeInHierarchy) m_mainCamera = Camera.main;
            if (m_mainCamera == null || !m_mainCamera.enabled || !m_mainCamera.gameObject.activeInHierarchy) return;
            if (rotate180)
            {
                if (yAxisOnly)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (m_mainCamera.transform.rotation.eulerAngles + 180f * Vector3.up).y, transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(-m_mainCamera.transform.forward, m_mainCamera.transform.up);
                }
            }
            else
            {
                if (yAxisOnly)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_mainCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = m_mainCamera.transform.rotation;
                }
            }
        }

    }

}
