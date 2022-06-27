// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the active/inactive state of a list of GameObjects. This component should be
    /// on a different GameObject that's guaranteed to be active, or it won't
    /// take effect. When applying data (i.e., setting active/inactive state), if 
    /// it activates an inactive target, it will call ApplyData on the target's
    /// other savers.
    /// </summary>
    [AddComponentMenu("")]
    public class MultiActiveSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool[] active;
        }

        [Tooltip("GameObjects to watch.")]
        [SerializeField]
        private GameObject[] m_gameObjectsToWatch;

        public GameObject[] gameObjectsToWatch
        {
            get { return m_gameObjectsToWatch; }
            set { m_gameObjectsToWatch = value; }
        }

        private Data m_data = new Data();

        public override string RecordData()
        {
            if (gameObjectsToWatch == null) return string.Empty;
            if (m_data.active == null || m_data.active.Length != gameObjectsToWatch.Length)
            {
                m_data.active = new bool[gameObjectsToWatch.Length];
            }
            for (int i = 0; i < gameObjectsToWatch.Length; i++)
            {
                m_data.active[i] = (gameObjectsToWatch[i] != null) ? gameObjectsToWatch[i].activeSelf : false;
            }
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            if (gameObjectsToWatch == null || string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s, m_data);
            if (data == null || data.active == null) return;
            m_data = data;
            // First issue OnBeforeSceneChange/OnLevelWillBeUnloaded in case targets include nested GOs:
            for (int i = 0; i < Mathf.Min(data.active.Length, gameObjectsToWatch.Length); i++)
            {
                if (gameObjectsToWatch[i] == null) continue;
                if (!data.active[i])
                {
                    gameObjectsToWatch[i].BroadcastMessage("OnBeforeSceneChange", SendMessageOptions.DontRequireReceiver);
                    gameObjectsToWatch[i].BroadcastMessage("OnLevelWillBeUnloaded", SendMessageOptions.DontRequireReceiver);
                }
            }
            // Then activate/deactivate:
            for (int i = 0; i < Mathf.Min(data.active.Length, gameObjectsToWatch.Length); i++)
            {
                if (gameObjectsToWatch[i] == null) continue;
                var applyDataToOtherSavers = data.active[i] && !gameObjectsToWatch[i].activeSelf;
                gameObjectsToWatch[i].SetActive(data.active[i]);
                if (applyDataToOtherSavers)
                {
                    var savers = gameObjectsToWatch[i].GetComponentsInChildren<Saver>();
                    for (int j = 0; j < savers.Length; j++)
                    {
                        var saver = savers[j];
                        if (saver == this || !saver.enabled) continue;
                        saver.ApplyData(SaveSystem.currentSavedGameData.GetData(saver.key));
                    }
                }
            }
        }

    }
}
