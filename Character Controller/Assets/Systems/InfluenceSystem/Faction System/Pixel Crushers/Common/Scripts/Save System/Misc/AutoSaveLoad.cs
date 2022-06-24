// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Auto-saves when the game closes and auto-loads when the game opens.
    /// Useful for mobile games.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class AutoSaveLoad : MonoBehaviour
    {

        [Tooltip("Save to this slot.")]
        public int saveSlotNumber = 1;

        [Tooltip("Don't auto-save in these scene indices.")]
        public int[] dontSaveInScenes = new int[0];

        [Tooltip("Load the saved game when this component starts.")]
        public bool loadOnStart = true;

        [Tooltip("Save when the player quits the app.")]
        public bool saveOnQuit = true;

        [Tooltip("Save when the player pauses or minimizes the app; tick this for mobile builds.")]
        public bool saveOnPause = true;

        [Tooltip("Save when the app loses focus.")]
        public bool saveOnLoseFocus = false;

        /// <summary>
        /// When starting, load the game.
        /// </summary>
        private void Start()
        {
            if (loadOnStart && SaveSystem.HasSavedGameInSlot(saveSlotNumber))
            {
                SaveSystem.LoadFromSlot(saveSlotNumber);
            }
        }

#if UNITY_2018_1_OR_NEWER
        private void OnEnable()
        {
            Application.wantsToQuit -= OnWantsToQuit;
            Application.wantsToQuit += OnWantsToQuit;
        }

        private void OnDisable()
        {
            Application.wantsToQuit -= OnWantsToQuit;
        }

        private bool OnWantsToQuit()
        {
            CheckSaveOnQuit();
            return true;
        }
#else
        /// <summary>
        /// When quitting, save the game.
        /// </summary>
        private void OnApplicationQuit()
        {
            CheckSaveOnQuit();
        }
#endif

        private void CheckSaveOnQuit()
        {
            if (enabled && saveOnQuit && CanSaveInThisScene())
            {
                SaveSystem.SaveToSlotImmediate(saveSlotNumber);
            }
        }

        /// <summary>
        /// When app is paused (e.g., minimized) and saveOnPause is true, save game.
        /// </summary>
        /// <param name="paused">True indicates game is being paused.</param>
        private void OnApplicationPause(bool paused)
        {
            if (enabled && paused && saveOnPause && CanSaveInThisScene())
            {
                SaveSystem.SaveToSlotImmediate(saveSlotNumber);
            }
        }

        /// <summary>
        /// When app loses focus and saveOnLoseFocus is true, save the game.
        /// </summary>
        /// <param name="focusStatus">False indicates game is losing focus.</param>
        void OnApplicationFocus(bool focusStatus)
        {
            if (enabled && saveOnLoseFocus && focusStatus == false && CanSaveInThisScene())
            {
                SaveSystem.SaveToSlotImmediate(saveSlotNumber);
            }
        }

        private bool CanSaveInThisScene()
        {
            var sceneIndex = SaveSystem.GetCurrentSceneIndex();
            for (int i = 0; i < dontSaveInScenes.Length; i++)
            {
                if (sceneIndex == dontSaveInScenes[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Clears the saved game data and restarts the game at a specified scene.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public void Restart(string startingSceneName)
        {
            SaveSystem.DeleteSavedGameInSlot(saveSlotNumber);
            SaveSystem.RestartGame(startingSceneName);
        }

    }

}
