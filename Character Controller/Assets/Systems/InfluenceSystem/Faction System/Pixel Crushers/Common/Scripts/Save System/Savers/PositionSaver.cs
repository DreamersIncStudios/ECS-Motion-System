// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

namespace PixelCrushers
{

    /// <summary>
    /// Saves a GameObject's position.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class PositionSaver : Saver
    {

        [Tooltip("When changing scenes, if a player spawnpoint is specified, move this GameObject to the spawnpoint.")]
        [SerializeField]
        private bool m_usePlayerSpawnpoint = false;

        [Tooltip("Record positions in every scene. If unticked, only records position in most recent scene.")]
        [SerializeField]
        private bool m_multiscene = false;

        [Serializable]
        public class PositionData
        {
            public int scene = -1;
            public Vector3 position;
            public Quaternion rotation;
        }

        [Serializable]
        public class ScenePositionData
        {
            public int scene;
            public Vector3 position;
            public Quaternion rotation;
            public ScenePositionData(int _scene, Vector3 _position, Quaternion _rotation)
            {
                scene = _scene;
                position = _position;
                rotation = _rotation;
            }
        }

        [Serializable]
        public class MultiscenePositionData
        {
            public List<ScenePositionData> positions = new List<ScenePositionData>();
        }

        protected PositionData m_data;
        protected MultiscenePositionData m_multisceneData;
        protected NavMeshAgent m_navMeshAgent;

        public bool usePlayerSpawnpoint
        {
            get { return m_usePlayerSpawnpoint; }
            set { m_usePlayerSpawnpoint = value; }
        }

        protected bool multiscene { get { return m_multiscene; } }

        public override void Awake()
        {
            base.Awake();
            if (m_multiscene) m_multisceneData = new MultiscenePositionData();
            else m_data = new PositionData();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override string RecordData()
        {
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            if (multiscene)
            {
                var found = false;
                for (int i = 0; i < m_multisceneData.positions.Count; i++)
                {
                    if (m_multisceneData.positions[i].scene == currentScene)
                    {
                        found = true;
                        m_multisceneData.positions[i].position = transform.position;
                        m_multisceneData.positions[i].rotation = transform.rotation;
                        break;
                    }
                }
                if (!found)
                {
                    m_multisceneData.positions.Add(new ScenePositionData(currentScene, transform.position, transform.rotation));
                }
                return SaveSystem.Serialize(m_multisceneData);
            }
            else
            {
                m_data.scene = currentScene;
                m_data.position = transform.position;
                m_data.rotation = transform.rotation;
                return SaveSystem.Serialize(m_data);
            }
        }

        public override void ApplyData(string s)
        {
            if (usePlayerSpawnpoint && SaveSystem.playerSpawnpoint != null)
            {
                SetPosition(SaveSystem.playerSpawnpoint.transform.position, SaveSystem.playerSpawnpoint.transform.rotation);
            }
            else if (!string.IsNullOrEmpty(s))
            {
                var currentScene = SceneManager.GetActiveScene().buildIndex;
                if (multiscene)
                {
                    var multisceneData = SaveSystem.Deserialize<MultiscenePositionData>(s, m_multisceneData);
                    if (multisceneData == null) return;
                    m_multisceneData = multisceneData;
                    for (int i = 0; i < m_multisceneData.positions.Count; i++)
                    {
                        if (m_multisceneData.positions[i].scene == currentScene)
                        {
                            SetPosition(m_multisceneData.positions[i].position, m_multisceneData.positions[i].rotation);
                            break;
                        }
                    }
                }
                else
                {
                    var data = SaveSystem.Deserialize<PositionData>(s, m_data);
                    if (data == null) return;
                    m_data = data;
                    if (data.scene == currentScene || data.scene == -1)
                    {
                        SetPosition(data.position, data.rotation);
                    }
                }
            }
        }

        protected virtual void SetPosition(Vector3 position, Quaternion rotation)
        {
            if (m_navMeshAgent != null)
            {
                m_navMeshAgent.Warp(position);
            }
            else
            {
                transform.position = position;
            }
            transform.rotation = rotation;
        }

    }
}
