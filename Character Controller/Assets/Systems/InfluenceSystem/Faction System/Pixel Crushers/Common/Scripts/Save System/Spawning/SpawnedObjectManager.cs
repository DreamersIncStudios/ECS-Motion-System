// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// Manages spawned objects for a scene.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SpawnedObjectManager : Saver
    {

        [Serializable]
        public class SpawnedObjectData
        {
            public string prefabName;
            public Vector3 position;
            public Quaternion rotation;

            public SpawnedObjectData() { }
            public SpawnedObjectData(string prefabName, Vector3 position, Quaternion rotation)
            {
                this.prefabName = prefabName;
                this.position = position;
                this.rotation = rotation;
            }
        }

        [Serializable]
        public class SpawnedObjectDataList
        {
            public List<SpawnedObjectData> list = new List<SpawnedObjectData>();
        }

        [Tooltip("Prefabs for all spawnable objects. If your spawnable object isn't in this list, Spawned Object Manager won't be able to respawn it when restoring a scene.")]
        [SerializeField]
        private List<SpawnedObject> m_spawnedObjectPrefabs = new List<SpawnedObject>();

        [SerializeField]
        private List<SpawnedObject> m_spawnedObjects = new List<SpawnedObject>();

        [Tooltip("When restoring this Spawned Object Manager, tell respawned objects to restore their saved data also.")]
        private bool m_applySaveDataToSpawnedObjectsOnRestore = false;

        private static SpawnedObjectManager m_instance;

        public List<SpawnedObject> spawnedObjectPrefabs
        {
            get { return m_spawnedObjectPrefabs; }
            set { m_spawnedObjectPrefabs = value; }
        }

        public List<SpawnedObject> spawnedObjects
        {
            get { return m_spawnedObjects; }
            set { m_spawnedObjects = value; }
        }

        public bool applySaveDataToSpawnedObjectsOnRestore
        {
            get { return m_applySaveDataToSpawnedObjectsOnRestore; }
            set { m_applySaveDataToSpawnedObjectsOnRestore = value; }
        }

        public override string key
        {
            get // Help ensure unique keys by adding scene index if left blank in inspector.
            {
                var baseKey = base.key;
                return string.Equals(baseKey, name) ? name + " Scene " + SaveSystem.currentSceneIndex : baseKey;
            }
            set { base.key = value; }
        }

        public override void Reset()
        {
            base.Reset();
            saveAcrossSceneChanges = true;
        }

        public override void Awake()
        {
            base.Awake();
            m_instance = this;
        }

        public override string RecordData()
        {
            var spawnedObjectDataList = new SpawnedObjectDataList();
            for (int i = 0; i < m_spawnedObjects.Count; i++)
            {
                var spawnedObject = m_spawnedObjects[i];
                if (spawnedObject == null) continue;
                spawnedObjectDataList.list.Add(new SpawnedObjectData(spawnedObject.name.Replace("(Clone)", string.Empty), spawnedObject.transform.position, spawnedObject.transform.rotation));
            }
            return SaveSystem.Serialize(spawnedObjectDataList);
        }

        public override void ApplyData(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            var spawnedObjectDataList = SaveSystem.Deserialize<SpawnedObjectDataList>(data);
            if (spawnedObjectDataList == null || spawnedObjectDataList.list == null) return;
            m_spawnedObjects.Clear();
            for (int i = 0; i < spawnedObjectDataList.list.Count; i++)
            {
                var spawnedObjectData = spawnedObjectDataList.list[i];
                if (spawnedObjectData == null) continue;
                var prefab = GetSpawnedObjectPrefab(spawnedObjectData.prefabName);
                if (prefab == null) continue;
                Instantiate(prefab, spawnedObjectData.position, spawnedObjectData.rotation);
            }
            if (m_applySaveDataToSpawnedObjectsOnRestore)
            {
                if (SaveSystem.framesToWaitBeforeApplyData == 0)
                {
                    ApplyDataToRespawnedObjects();
                }
                else
                {
                    StartCoroutine(ApplyDataToRespawnedObjectsAfterFrames(SaveSystem.framesToWaitBeforeApplyData));
                }
            }
        }

        protected void ApplyDataToRespawnedObjects()
        {
            for (int i = 0; i < m_spawnedObjects.Count; i++)
            {
                foreach (var saver in m_spawnedObjects[i].GetComponentsInChildren<Saver>())
                {
                    saver.ApplyData(SaveSystem.currentSavedGameData.GetData(saver.key));
                }
            }
        }

        protected IEnumerator ApplyDataToRespawnedObjectsAfterFrames(int numFrames)
        {
            for (int i = 0; i < numFrames; i++)
            {
                yield return null;
            }
            ApplyDataToRespawnedObjects();
        }

        protected SpawnedObject GetSpawnedObjectPrefab(string prefabName)
        {
            return m_spawnedObjectPrefabs.Find(x => x != null && string.Equals(x.name, prefabName));
        }

        public static void AddSpawnedObjectData(SpawnedObject spawnedObject)
        {
            if (m_instance == null || spawnedObject == null) return;
            m_instance.m_spawnedObjects.Add(spawnedObject);
        }

        public static void RemoveSpawnedObjectData(SpawnedObject spawnedObject)
        {
            if (m_instance == null || spawnedObject == null) return;
            m_instance.m_spawnedObjects.Remove(spawnedObject);
        }

    }

}
