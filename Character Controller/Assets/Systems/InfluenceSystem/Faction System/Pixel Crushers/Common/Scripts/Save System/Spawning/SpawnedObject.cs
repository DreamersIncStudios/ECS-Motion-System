// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// A spawned object or spawnable prefab.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SpawnedObject : Saver
    {

        public enum Mode { OnDisable, OnDestroy }

        [Tooltip("Event to watch for to record that object was despawned.")]
        [SerializeField]
        private Mode m_mode = Mode.OnDestroy;

        private bool m_ignoreOnDestroy = false;

        public Mode mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        public override void Start()
        {
            base.Start();
            SpawnedObjectManager.AddSpawnedObjectData(this);
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

        protected virtual void RecordDestruction()
        {
            if (!m_ignoreOnDestroy)
            {
                SpawnedObjectManager.RemoveSpawnedObjectData(this);
            }
            m_ignoreOnDestroy = false;
        }

        public override string RecordData()
        {
            return string.Empty;
        }

        public override void ApplyData(string data)
        {
        }
    }

}
