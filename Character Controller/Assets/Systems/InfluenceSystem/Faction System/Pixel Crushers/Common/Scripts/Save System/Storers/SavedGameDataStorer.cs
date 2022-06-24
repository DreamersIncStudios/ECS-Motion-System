// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Abstract base class for "storage providers" that store saved game 
    /// data somewhere, such as PlayerPrefs or a disk file. To save asynchronously,
    /// override StoreSavedGameDataAsync.
    /// </summary>
    public abstract class SavedGameDataStorer : MonoBehaviour
    {
        /// <summary>
        /// Return the current progress (0-1) of the current async save operation.
        /// </summary>
        public virtual float progress { get; protected set; }

        /// <summary>
        /// Return true if the specified slot contains a saved game.
        /// </summary>
        public abstract bool HasDataInSlot(int slotNumber);

        /// <summary>
        /// Store saved game data in the specified slot.
        /// </summary>
        public abstract void StoreSavedGameData(int slotNumber, SavedGameData savedGameData);

        /// <summary>
        /// Retrieve saved game data from the specified slot, or null if no saved game in the slot.
        /// </summary>
        public abstract SavedGameData RetrieveSavedGameData(int slotNumber);

        /// <summary>
        /// Delete the saved game from the specified slot if present.
        /// </summary>
        public abstract void DeleteSavedGameData(int slotNumber);

        /// <summary>
        /// Asynchronously store the saved game data in the specified slot. The base version of
        /// this method just calls the synchronous version, StoreSavedGameData(). If you override
        /// it, keep the progress property updated so any watchers will know how far along it is.
        /// </summary>
        public virtual IEnumerator StoreSavedGameDataAsync(int slotNumber, SavedGameData savedGameData)
        {
            StoreSavedGameData(slotNumber, savedGameData);
            progress = 1;
            yield break;
        }

    }

}
