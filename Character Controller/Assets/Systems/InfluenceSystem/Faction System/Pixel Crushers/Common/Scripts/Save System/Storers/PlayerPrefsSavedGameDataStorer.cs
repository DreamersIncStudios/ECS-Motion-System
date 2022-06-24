// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Implements SavedGameDataStorer using PlayerPrefs.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class PlayerPrefsSavedGameDataStorer : SavedGameDataStorer
    {

        [Tooltip("Save games under this PlayerPrefs key")]
        [SerializeField]
        private string m_playerPrefsKeyBase = "Save";

#if UNITY_EDITOR || UNITY_STANDALONE

        [Tooltip("Encrypt saved game data.")]
        public bool encrypt = false;

        [Tooltip("If encrypting, use this password.")]
        public string encryptionPassword = "My Password";

#else
        private bool encrypt = false;
        private string encryptionPassword = "My Password";
#endif

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug = false;

        public string playerPrefsKeyBase
        {
            get { return m_playerPrefsKeyBase; }
            set { m_playerPrefsKeyBase = value; }
        }

        public bool debug
        {
            get { return m_debug && Debug.isDebugBuild; }
        }

        public string GetPlayerPrefsKey(int slotNumber)
        {
            return m_playerPrefsKeyBase + slotNumber;
        }

        public override bool HasDataInSlot(int slotNumber)
        {
            return PlayerPrefs.HasKey(GetPlayerPrefsKey(slotNumber));
        }

        public override void StoreSavedGameData(int slotNumber, SavedGameData savedGameData)
        {
            var s = SaveSystem.Serialize(savedGameData);
            if (debug) Debug.Log("Save System: Storing in PlayerPrefs key " + GetPlayerPrefsKey(slotNumber) + ": " + s);
            PlayerPrefs.SetString(GetPlayerPrefsKey(slotNumber), encrypt ? EncryptionUtility.Encrypt(s, encryptionPassword) : s);
            PlayerPrefs.Save();
        }

        public override SavedGameData RetrieveSavedGameData(int slotNumber)
        {
            if (debug && HasDataInSlot(slotNumber)) Debug.Log("Save System: Retrieved from PlayerPrefs key " +
                GetPlayerPrefsKey(slotNumber) + ": " + PlayerPrefs.GetString(GetPlayerPrefsKey(slotNumber)));
            var s = PlayerPrefs.GetString(GetPlayerPrefsKey(slotNumber));
            if (encrypt)
            {
                string plainText;
                s = EncryptionUtility.TryDecrypt(s, encryptionPassword, out plainText) ? plainText : string.Empty;
            }
            return HasDataInSlot(slotNumber) ? SaveSystem.Deserialize<SavedGameData>(s) : new SavedGameData();
        }

        public override void DeleteSavedGameData(int slotNumber)
        {
            PlayerPrefs.DeleteKey(GetPlayerPrefsKey(slotNumber));
            PlayerPrefs.Save();
        }

    }

}
