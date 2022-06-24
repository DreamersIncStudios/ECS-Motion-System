// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Provides inspector-selectable methods to control SaveSystem.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystemMethods : MonoBehaviour
    {

        [Tooltip("Scene to load in LoadOrRestart method if no saved game exists yet.")]
        public string defaultStartingSceneName;

        /// <summary>
        /// Saves the current game in the specified slot.
        /// </summary>
        /// <param name="slotNumber">slot to save.</param>
        public void SaveSlot(int slotNumber)
        {
            SaveSystem.SaveToSlot(slotNumber);
        }

        /// <summary>
        /// Loads the game previously-saved in the specified slot.
        /// </summary>
        /// <param name="slotNumber">Slot to load.</param>
        public void LoadFromSlot(int slotNumber)
        {
            SaveSystem.LoadFromSlot(slotNumber);
        }

        /// <summary>
        /// Changes scenes. You can optionally specify a player spawnpoint by 
        /// adding '@' and the spawnpoint GameObject name.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">Scene name followed by an optional at-sign and spawnpoint name.</param>
        public void LoadScene(string sceneNameAndSpawnpoint)
        {
            SaveSystem.LoadScene(sceneNameAndSpawnpoint);
        }

        /// <summary>
        /// Resets all saved game data.
        /// </summary>
        public void ResetGameState()
        {
            SaveSystem.ResetGameState();
        }

        /// <summary>
        /// Resets all saved game data and restarts the game at the specified scene.
        /// </summary>
        /// <param name="startingSceneName">Scene to restart at.</param>
        public void RestartGame(string startingSceneName)
        {
            SaveSystem.RestartGame(startingSceneName);
        }

        /// <summary>
        /// Load the specified slot, or restart the game from the default
        /// starting scene if no save exists yet.
        /// </summary>
        /// <param name="slotNumber">Slot number to load.</param>
        public void LoadOrRestart(int slotNumber)
        {
            if (SaveSystem.HasSavedGameInSlot(slotNumber))
            {
                SaveSystem.LoadFromSlot(slotNumber);
            }
            else
            {
                SaveSystem.RestartGame(defaultStartingSceneName);
            }
        }

        /// <summary>
        /// Deletes the saved game in the specified slot.
        /// </summary>
        public void DeleteSavedGameInSlot(int slotNumber)
        {
            SaveSystem.DeleteSavedGameInSlot(slotNumber);
        }

        /// <summary>
        /// Records the current game state into the Save System.
        /// </summary>
        public void RecordSavedGameData()
        {
            SaveSystem.RecordSavedGameData();
        }

        /// <summary>
        /// Applies the most recently recorded game state.
        /// </summary>
        public void ApplySavedGameData()
        {
            SaveSystem.ApplySavedGameData();
        }

        /// <summary>
        /// Additively loads another scene.
        /// </summary>
        /// <param name="sceneName">Scene to additively load.</param>
        public void LoadAdditiveScene(string sceneName)
        {
            SaveSystem.LoadAdditiveScene(sceneName);
        }

        /// <summary>
        /// Unloads a previously additively-loaded scene.
        /// </summary>
        /// <param name="sceneName">Scene to unload</param>
        public void UnloadAdditiveScene(string sceneName)
        {
            SaveSystem.UnloadAdditiveScene(sceneName);
        }

    }

}
