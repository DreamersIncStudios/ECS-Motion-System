// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Changes scenes, either by calling UsePortal() or OnTriggerEnter.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class ScenePortal : MonoBehaviour
    {

        [Tooltip("Only objects with this tag can use the portal.")]
        [SerializeField]
        private string m_requiredTag = "Player";

        [Tooltip("Go to this scene.")]
        [SerializeField]
        private string m_destinationSceneName;

        [Tooltip("If not blank, move the player to the GameObject with this name.")]
        [SerializeField]
        private string m_spawnpointNameInDestinationScene;

        [SerializeField]
        private UnityEngine.Events.UnityEvent m_onUsePortal = new UnityEngine.Events.UnityEvent();

        private bool m_isLoadingScene = false;

        public string requiredTag
        {
            get { return m_requiredTag; }
            set { m_requiredTag = value; }
        }

        public string destinationSceneName
        {
            get { return m_destinationSceneName; }
            set { m_destinationSceneName = value; }
        }

        public string spawnpointNameInDestinationScene
        {
            get { return m_spawnpointNameInDestinationScene; }
            set { m_spawnpointNameInDestinationScene = value; }
        }

        public bool isLoadingScene
        {
            get { return m_isLoadingScene; }
            set { m_isLoadingScene = value; }
        }

        public UnityEngine.Events.UnityEvent onUsePortal { get { return m_onUsePortal; } }

        public virtual void UsePortal()
        {
            if (isLoadingScene) return;
            isLoadingScene = true;
            onUsePortal.Invoke();
            LoadScene();
        }

        protected void LoadScene()
        {
            SaveSystem.LoadScene(string.IsNullOrEmpty(spawnpointNameInDestinationScene) ? destinationSceneName : destinationSceneName + "@" + spawnpointNameInDestinationScene);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(requiredTag)) return;
            UsePortal();
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(requiredTag)) return;
            UsePortal();
        }

#endif

    }

}
