// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves when a GameObject has been destroyed or disabled. The next time the game
    /// or scene is loaded, if the GameObject has previously been destroyed/disabled, this
    /// script will destroy/deactivate it again. It will also spawn a replacement destroyed
    /// version if a prefab is assigned.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DestructibleSaver : Saver
    {

        [Serializable]
        public class DestructibleData
        {
            public bool destroyed = false;
            public Vector3 position;
        }

        public enum Mode { OnDisable, OnDestroy }

        public enum DestroyMode { Destroy, Deactivate }

        [Tooltip("Event to watch for.")]
        [SerializeField]
        private Mode m_mode = Mode.OnDestroy;

        [Tooltip("How to re-destroy object.")]
        [SerializeField]
        private DestroyMode m_destroyMode = DestroyMode.Destroy;

        [Tooltip("Instantiate this if already destroyed when loading game or scene.")]
        [SerializeField]
        private GameObject m_destroyedVersionPrefab;

        private DestructibleData m_data = new DestructibleData();
        private bool m_ignoreOnDestroy = false;

        public Mode mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        public DestroyMode destroyMode
        {
            get { return m_destroyMode; }
            set { m_destroyMode = value; }
        }

        public GameObject destroyedVersionPrefab
        {
            get { return m_destroyedVersionPrefab; }
            set { m_destroyedVersionPrefab = value; }
        }

        public override void OnBeforeSceneChange()
        {
            base.OnBeforeSceneChange();
            m_ignoreOnDestroy = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (m_mode != Mode.OnDisable) return;
            RecordDestruction();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_mode != Mode.OnDestroy) return;
            RecordDestruction();
        }

        public virtual void RecordDestruction()
        {
            if (!m_ignoreOnDestroy && SaveSystem.instance != null)
            {
                m_data.destroyed = true;
                m_data.position = transform.position;
                SaveSystem.UpdateSaveData(this, SaveSystem.Serialize(m_data));
            }
            m_ignoreOnDestroy = false;
        }

        public override string RecordData()
        {
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            var data = SaveSystem.Deserialize<DestructibleData>(s, m_data);
            if (data == null) return;
            m_data = data;
            if (data.destroyed)
            {
                if (destroyedVersionPrefab != null)
                {
                    Instantiate(destroyedVersionPrefab, data.position, transform.rotation);
                }
                switch (destroyMode)
                {
                    case DestroyMode.Destroy:
                        Destroy(gameObject);
                        break;
                    case DestroyMode.Deactivate:
                        gameObject.SetActive(false);
                        break;
                }
            }

        }

    }
}
