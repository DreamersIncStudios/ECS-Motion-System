// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// This is the main Save System class. It runs as a singleton MonoBehaviour
    /// and provides static methods to save and load games.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystem : MonoBehaviour
    {

        public const int NoSceneIndex = -1;

        /// <summary>
        /// Stores an int indicating the slot number of the most recently saved game.
        /// </summary>
        public const string LastSavedGameSlotPlayerPrefsKey = "savedgame_lastSlotNum";

        [Tooltip("Optional saved game version number of your choosing. Version number is included in saved game files.")]
        [SerializeField]
        private int m_version = 0;

        [Tooltip("When loading a game, load the scene that the game was saved in.")]
        [SerializeField]
        private bool m_saveCurrentScene = true;

        [Tooltip("When loading a game/scene, wait this many frames before applying saved data to allow other scripts to initialize first.")]
        [SerializeField]
        private int m_framesToWaitBeforeApplyData = 1;

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug = false;

        private bool m_isLoadingAdditiveScene = false;

        private static SaveSystem m_instance = null;

        private static List<Saver> m_savers = new List<Saver>();

        private static SavedGameData m_savedGameData = new SavedGameData();

        private static DataSerializer m_serializer = null;

        private static SavedGameDataStorer m_storer = null;

        private static SceneTransitionManager m_sceneTransitionManager = null;

        private static GameObject m_playerSpawnpoint = null;

        private static int m_currentSceneIndex = NoSceneIndex;

        private static List<string> m_addedScenes = new List<string>();

        private static bool m_autoUnloadAdditiveScenes = false;

        private static AsyncOperation m_currentAsyncOperation = null;

        private static bool m_isQuitting = false;

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            m_instance = null;
            m_savers = new List<Saver>();
            m_savedGameData = new SavedGameData();
            m_serializer = null;
            m_storer = null;
            m_sceneTransitionManager = null;
            m_playerSpawnpoint = null;
            m_currentSceneIndex = NoSceneIndex;
            m_addedScenes = new List<string>();
            m_currentAsyncOperation = null;
            m_isQuitting = false;
        }
#endif

        /// <summary>
        /// Optional saved game version number of your choosing. Version number is included in saved game files.
        /// </summary>
        public static int version
        {
            get
            {
                return (m_instance != null) ? m_instance.m_version : 0;
            }
            set
            {
                if (m_instance != null) m_instance.m_version = value;
            }
        }

        /// <summary>
        /// When loading a game, load the scene that the game was saved in.
        /// </summary>
        public static bool saveCurrentScene
        {
            get
            {
                return (m_instance != null) ? m_instance.m_saveCurrentScene : true;
            }
            set
            {
                if (m_instance != null) m_instance.m_saveCurrentScene = value;
            }
        }

        /// <summary>
        /// When loading a game/scene, wait this many frames before applying saved data to allow other scripts to initialize first.
        /// </summary>
        public static int framesToWaitBeforeApplyData
        {
            get
            {
                return (m_instance != null) ? m_instance.m_framesToWaitBeforeApplyData : 1;
            }
            set
            {
                if (m_instance != null) m_instance.m_framesToWaitBeforeApplyData = value;
            }
        }

        public static bool debug
        {
            get
            {
                return (m_instance != null) ? m_instance.m_debug && Debug.isDebugBuild : false;
            }
            set
            {
                if (m_instance != null) m_instance.m_debug = value;
            }
        }

        public static SaveSystem instance
        {
            get
            {
                if (m_instance == null && !m_isQuitting)
                {
                    m_instance = FindObjectOfType<SaveSystem>();
                    if (m_instance == null)
                    {
                        m_instance = new GameObject("Save System", typeof(SaveSystem)).GetComponent<SaveSystem>();
                    }
                }
                return m_instance;
            }
        }

        public static DataSerializer serializer
        {
            get
            {
                if (m_serializer == null)
                {
                    m_serializer = instance.GetComponent<DataSerializer>();
                    if (m_serializer == null && !m_isQuitting)
                    {
                        Debug.Log("Save System: No DataSerializer found on " + instance.name + ". Adding JsonDataSerializer.", instance);
                        m_serializer = instance.gameObject.AddComponent<JsonDataSerializer>();
                    }
                }
                return m_serializer;
            }
        }

        public static SavedGameDataStorer storer
        {
            get
            {
                if (m_storer == null)
                {
                    m_storer = instance.GetComponent<SavedGameDataStorer>();
                    if (m_storer == null && !m_isQuitting)
                    {
                        Debug.Log("Save System: No SavedGameDataStorer found on " + instance.name + ". Adding PlayerPrefsSavedGameDataStorer.", instance);
                        m_storer = instance.gameObject.AddComponent<PlayerPrefsSavedGameDataStorer>();
                    }
                }
                return m_storer;
            }
        }

        public static SceneTransitionManager sceneTransitionManager
        {
            get
            {
                if (m_sceneTransitionManager == null)
                {
                    m_sceneTransitionManager = instance.GetComponentInChildren<SceneTransitionManager>();
                }
                return m_sceneTransitionManager;
            }
        }

        /// <summary>
        /// Scenes that have been loaded additively.
        /// </summary>
        public static List<string> addedScenes { get { return m_addedScenes; } }

        /// <summary>
        /// When changing scenes, automatically unload all additively-loaded scenes.
        /// </summary>
        public static bool autoUnloadAdditiveScenes
        {
            get { return m_autoUnloadAdditiveScenes; }
            set { m_autoUnloadAdditiveScenes = value; }
        }

        /// <summary>
        /// Current asynchronous scene load operation, or null if none. Loading scenes can use this
        /// value to update a progress bar.
        /// </summary>
        public static AsyncOperation currentAsyncOperation
        {
            get { return m_currentAsyncOperation; }
            set { m_currentAsyncOperation = value; }
        }

        /// <summary>
        /// The saved game data recorded by the last call to SaveToSlot,
        /// LoadScene, or RecordSavedGameData.
        /// </summary>
        public static SavedGameData currentSavedGameData
        {
            get { return m_savedGameData; }
            set { m_savedGameData = value; }
        }

        /// <summary>
        /// Where the player should spawn in the current scene.
        /// </summary>
        public static GameObject playerSpawnpoint
        {
            get { return m_playerSpawnpoint; }
            set { m_playerSpawnpoint = value; }
        }

        /// <summary>
        /// Build index of the current scene.
        /// </summary>
        public static int currentSceneIndex
        {
            get
            {
                if (m_currentSceneIndex == NoSceneIndex) m_currentSceneIndex = GetCurrentSceneIndex();
                return m_currentSceneIndex;
            }
        }

        public delegate void SceneLoadedDelegate(string sceneName, int sceneIndex);

        /// <summary>
        /// Invoked after a scene has been loaded.
        /// </summary>
        public static event SceneLoadedDelegate sceneLoaded = delegate { };

        /// <summary>
        /// Invoked when starting to save a game. If assigned, waits one frame before
        /// starting the save to allow UIs to update.
        /// </summary>
        public static event System.Action saveStarted = delegate { };

        /// <summary>
        /// Invoked when finished saving a game.
        /// </summary>
        public static event System.Action saveEnded = delegate { };

        /// <summary>
        /// Invoked when starting to load a game. If assigned, waits one frame before
        /// starting the load to allow UIs to update.
        /// </summary>
        public static event System.Action loadStarted = delegate { };

        /// <summary>
        /// Invoked when finished loading a game.
        /// </summary>
        public static event System.Action loadEnded = delegate { };

        /// <summary>
        /// Invoked after ApplyData() has been called on all savers.
        /// </summary>
        public static event System.Action saveDataApplied = delegate { };

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                if (transform.parent != null) transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            m_isQuitting = true;
            BeforeSceneChange();
        }

#if UNITY_5_4_OR_NEWER
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            FinishedLoadingScene(scene.name, scene.buildIndex);
        }

#else
        public void OnLevelWasLoaded(int level)
        {
            FinishedLoadingScene(GetCurrentSceneName(), level);
        }
#endif

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        public static string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public static int GetCurrentSceneIndex()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            if (sceneTransitionManager == null)
            {
                if (sceneName.StartsWith("index:"))
                {
                    var index = SafeConvert.ToInt(sceneName.Substring("index:".Length));
                    UnityEngine.SceneManagement.SceneManager.LoadScene(index);
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                }
                yield break;
            }
            else
            {
                yield return instance.StartCoroutine(LoadSceneInternalTransitionCoroutine(sceneName));
            }
        }

        private static IEnumerator LoadSceneInternalTransitionCoroutine(string sceneName)
        {
            yield return instance.StartCoroutine(sceneTransitionManager.LeaveScene());
            if (sceneName.StartsWith("index:"))
            {
                var index = SafeConvert.ToInt(sceneName.Substring("index:".Length));
                m_currentAsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
            }
            else
            {
                m_currentAsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            }
            while (m_currentAsyncOperation != null && !m_currentAsyncOperation.isDone)
            {
                yield return null;
            }
            m_currentAsyncOperation = null;
            instance.StartCoroutine(sceneTransitionManager.EnterScene());
        }

        public static IEnumerator LoadAdditiveSceneInternal(string sceneName)
        {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) yield break;
            var rootGOs = scene.GetRootGameObjects();
            for (int i = 0; i < rootGOs.Length; i++)
            {
                RecursivelyApplySavers(rootGOs[i].transform);
            }
        }

        public static void UnloadAdditiveSceneInternal(string sceneName)
        {
#if UNITY_5_3 || UNITY_5_4
            UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName);
#else
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid())
            {
                var rootGOs = scene.GetRootGameObjects();
                for (int i = 0; i < rootGOs.Length; i++)
                {
                    RecursivelyInformBeforeSceneChange(rootGOs[i].transform);
                }
            }
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
#endif
        }

        /// <summary>
        /// Tells all saver components on the transform and its children to retrieve their states from the current saved game data.
        /// </summary>
        /// <param name="t"></param>
        public static void RecursivelyApplySavers(Transform t)
        {
            if (t == null) return;
            var saver = t.GetComponent<Saver>();
            if (saver != null) saver.ApplyData(currentSavedGameData.GetData(saver.key));
            foreach (Transform child in t)
            {
                RecursivelyApplySavers(child);
            }
        }

        /// <summary>
        /// Calls BeforeSceneChange on all saver components on the transform and its children.
        /// Used when unloading an additive scene.
        /// </summary>
        /// <param name="t"></param>
        public static void RecursivelyInformBeforeSceneChange(Transform t)
        {
            if (t == null) return;
            var saver = t.GetComponent<Saver>();
            if (saver != null) saver.OnBeforeSceneChange();
            foreach (Transform child in t)
            {
                RecursivelyInformBeforeSceneChange(child);
            }
        }

#else

        public static string GetCurrentSceneName()
        {
            return Application.loadedLevelName;
        }

        public static int GetCurrentSceneIndex()
        {
            return Application.loadedLevel;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            Application.LoadLevel(sceneName);
            yield break;
        }

        public static IEnumerator LoadAdditiveSceneInternal(string sceneName)
        {
            yield return Application.LoadLevelAdditiveAsync(sceneName);
        }

        public static void UnloadAdditiveSceneInternal(string sceneName)
        {
            Application.UnloadLevel(sceneName);
        }
#endif

        /// <summary>
        /// Saves a game into a slot using the storage provider on the 
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber">Slot in which to store saved game data.</param>
        public void SaveGameToSlot(int slotNumber)
        {
            SaveToSlot(slotNumber);
        }

        /// <summary>
        /// Loads a game from a slot using the storage provider on the
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void LoadGameFromSlot(int slotNumber)
        {
            LoadFromSlot(slotNumber);
        }

        /// <summary>
        /// Loads a scene, optionally positioning the player at a
        /// specified spawnpoint.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">
        /// A string containing the name of the scene to load, optionally
        /// followed by "@spawnpoint" where "spawnpoint" is the name of
        /// a GameObject in that scene. The player will be spawned at that
        /// GameObject's position.
        /// </param>
        public void LoadSceneAtSpawnpoint(string sceneNameAndSpawnpoint)
        {
            LoadScene(sceneNameAndSpawnpoint);
        }

        /// <summary>
        /// Returns true if there is a saved game in the specified slot.
        /// </summary>
        public static bool HasSavedGameInSlot(int slotNumber)
        {
            return storer.HasDataInSlot(slotNumber);
        }

        /// <summary>
        /// Deletes the saved game in the specified slot.
        /// </summary>
        public static void DeleteSavedGameInSlot(int slotNumber)
        {
            storer.DeleteSavedGameData(slotNumber);
        }

        /// <summary>
        /// Saves the current game to a slot.
        /// </summary>
        public static void SaveToSlot(int slotNumber)
        {
            instance.StartCoroutine(SaveToSlotCoroutine(slotNumber));
        }

        private static IEnumerator SaveToSlotCoroutine(int slotNumber)
        {
            saveStarted();
            yield return null;
            PlayerPrefs.SetInt(LastSavedGameSlotPlayerPrefsKey, slotNumber);
            yield return storer.StoreSavedGameDataAsync(slotNumber, RecordSavedGameData());
            saveEnded();
        }

        /// <summary>
        /// Saves the current game to a slot synchronously and immediately.
        /// </summary>
        public static void SaveToSlotImmediate(int slotNumber)
        {
            saveStarted();
            PlayerPrefs.SetInt(LastSavedGameSlotPlayerPrefsKey, slotNumber);
            storer.StoreSavedGameData(slotNumber, RecordSavedGameData());
            saveEnded();
        }

        /// <summary>
        /// Loads a game from a slot.
        /// </summary>
        public static void LoadFromSlot(int slotNumber)
        {
            if (!HasSavedGameInSlot(slotNumber))
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Save System: LoadFromSlot(" + slotNumber + ") but there is no saved game in this slot.");
                return;
            }
            if (loadStarted.GetInvocationList().Length > 1)
            {
                instance.StartCoroutine(LoadFromSlotCoroutine(slotNumber));
            }
            else
            {
                LoadFromSlotNow(slotNumber);
            }
        }

        private static IEnumerator LoadFromSlotCoroutine(int slotNumber)
        {
            loadStarted();
            yield return null;
            LoadFromSlotNow(slotNumber);
            //--- Always notify, in case loadEnded listeners are added via code:
            //--- if (loadEnded.GetInvocationList().Length > 1)
            sceneLoaded += NotifyLoadEndedWhenSceneLoaded;
        }

        private static void NotifyLoadEndedWhenSceneLoaded(string sceneName, int sceneIndex)
        {
            sceneLoaded -= NotifyLoadEndedWhenSceneLoaded;
            loadEnded();
        }

        private static void LoadFromSlotNow(int slotNumber)
        {
            LoadGame(storer.RetrieveSavedGameData(slotNumber));
        }

        public static void RegisterSaver(Saver saver)
        {
            if (saver == null || m_savers.Contains(saver)) return;
            m_savers.Add(saver);
        }

        public static void UnregisterSaver(Saver saver)
        {
            m_savers.Remove(saver);
        }

        /// <summary>
        /// Clears the SaveSystem's internal saved game data cache.
        /// </summary>
        public static void ClearSavedGameData()
        {
            m_savedGameData = new SavedGameData();
        }

        /// <summary>
        /// Records the current scene's savers' data into the SaveSystem's
        /// internal saved game data cache.
        /// </summary>
        /// <returns></returns>
        public static SavedGameData RecordSavedGameData()
        {
            m_savedGameData.version = version;
            m_savedGameData.sceneName = GetCurrentSceneName();
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), saver.RecordData());
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return m_savedGameData;
        }

        private static int GetSaverSceneIndex(Saver saver)
        {
            return (saver == null || !saver.saveAcrossSceneChanges) ? currentSceneIndex : NoSceneIndex;
        }

        /// <summary>
        /// Updates the SaveSystem's internal saved game data cache with data for a 
        /// specific saver.
        /// </summary>
        /// <param name="saver"></param>
        /// <param name="data"></param>
        public static void UpdateSaveData(Saver saver, string data)
        {
            m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), data);
        }

        /// <summary>
        /// Applies the saved game data to the savers in the current scene.
        /// </summary>
        /// <param name="savedGameData">Saved game data.</param>
        public static void ApplySavedGameData(SavedGameData savedGameData)
        {
            if (savedGameData == null) return;
            m_savedGameData = savedGameData;
            if (m_savers.Count <= 0) return;
            for (int i = m_savers.Count - 1; i >= 0; i--) // A saver may remove itself from list during apply.
            {
                try
                {
                    if (0 <= i && i < m_savers.Count)
                    {
                        var saver = m_savers[i];
                        if (saver != null) saver.ApplyData(savedGameData.GetData(saver.key));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            saveDataApplied();
        }

        /// <summary>
        /// Applies the most recently recorded saved game data.
        /// </summary>
        public static void ApplySavedGameData()
        {
            ApplySavedGameData(m_savedGameData);
        }

        /// <summary>
        /// If changing scenes manually, calls before changing scenes to inform components
        /// that listen for OnDestroy messages that they're being destroyed because of the
        /// scene change.
        /// </summary>
        public static void BeforeSceneChange()
        {
            // Notify savers:
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    saver.OnBeforeSceneChange();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            // Notify SceneNotifier:
            try
            {
                SceneNotifier.NotifyWillUnloadScene(m_currentSceneIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Loads the scene recorded in the saved game data (if saveCurrentScene is true) and 
        /// applies the saved game data to it.
        /// </summary>
        /// <param name="savedGameData"></param>
        public static void LoadGame(SavedGameData savedGameData)
        {
            if (savedGameData == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("SaveSystem.LoadGame received null saved game data. Not loading.");
            }
            else if (saveCurrentScene)
            {
                instance.StartCoroutine(LoadSceneCoroutine(savedGameData, null));
            }
            else
            {
                ApplySavedGameData(savedGameData);
            }
        }

        /// <summary>
        /// Loads a scene, optionally moving the player to a specified spawnpoint.
        /// If the scene name starts with "index:" followed by an index number, this
        /// method loads the scene by build index number.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">Scene name, followed by an optional spawnpoint separated by '@'.</param>
        public static void LoadScene(string sceneNameAndSpawnpoint)
        {
            if (string.IsNullOrEmpty(sceneNameAndSpawnpoint)) return;
            string sceneName = sceneNameAndSpawnpoint;
            string spawnpointName = string.Empty;
            if (sceneNameAndSpawnpoint.Contains("@"))
            {
                var strings = sceneNameAndSpawnpoint.Split('@');
                sceneName = strings[0];
                spawnpointName = (strings.Length > 1) ? strings[1] : null;
            }
            var savedGameData = RecordSavedGameData();
            savedGameData.sceneName = sceneName;
            instance.StartCoroutine(LoadSceneCoroutine(savedGameData, spawnpointName));
        }

        private static IEnumerator LoadSceneCoroutine(SavedGameData savedGameData, string spawnpointName)
        {
            if (savedGameData == null) yield break;
            if (debug) Debug.Log("Save System: Loading scene " + savedGameData.sceneName +
                (string.IsNullOrEmpty(spawnpointName) ? string.Empty : " [spawn at " + spawnpointName + "]"));
            m_savedGameData = savedGameData;
            BeforeSceneChange();
            if (autoUnloadAdditiveScenes) UnloadAllAdditiveScenes();
            yield return LoadSceneInternal(savedGameData.sceneName);
            ApplyDataImmediate();
            // Allow other scripts to spin up scene first:
            for (int i = 0; i < framesToWaitBeforeApplyData; i++)
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            m_playerSpawnpoint = !string.IsNullOrEmpty(spawnpointName) ? GameObject.Find(spawnpointName) : null;
            if (!string.IsNullOrEmpty(spawnpointName) && m_playerSpawnpoint == null) Debug.LogWarning("Save System: Can't find spawnpoint '" + spawnpointName + "'. Is spelling and capitalization correct?");
            ApplySavedGameData(savedGameData);
        }

        // Calls ApplyDataImmediate on all savers.
        private static void ApplyDataImmediate()
        {
            if (m_savers.Count <= 0) return;
            try
            {
                for (int i = m_savers.Count - 1; i >= 0; i--) // A saver may remove itself from list during apply.
                {
                    if (0 <= i && i < m_savers.Count)
                    {
                        var saver = m_savers[i];
                        if (saver != null) saver.ApplyDataImmediate();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void FinishedLoadingScene(string sceneName, int sceneIndex)
        {
            m_currentSceneIndex = sceneIndex;
            if (!m_isLoadingAdditiveScene)
            { // Don't delete other non-cross-scene data if loading additive scene:
                m_savedGameData.DeleteObsoleteSaveData(sceneIndex);
            }
            m_isLoadingAdditiveScene = false;
            sceneLoaded(sceneName, sceneIndex);
        }

        /// <summary>
        /// Additively loads another scene.
        /// </summary>
        /// <param name="sceneName">Scene to additively load.</param>
        public static void LoadAdditiveScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || m_addedScenes.Contains(sceneName)) return;
            m_addedScenes.Add(sceneName);
            instance.m_isLoadingAdditiveScene = true;
            instance.StartCoroutine(LoadAdditiveSceneInternal(sceneName));
        }

        /// <summary>
        /// Unloads a previously additively-loaded scene.
        /// </summary>
        /// <param name="sceneName">Scene to unload</param>
        public static void UnloadAdditiveScene(string sceneName)
        {
            if (!m_addedScenes.Contains(sceneName)) return;
            m_addedScenes.Remove(sceneName);
            UnloadAdditiveSceneInternal(sceneName);
        }

        /// <summary>
        /// Unloads all previously additively-loaded scenes.
        /// </summary>
        public static void UnloadAllAdditiveScenes()
        {
            for (int i = m_addedScenes.Count - 1; i >= 0; i--)
            {
                UnloadAdditiveScene(m_addedScenes[i]);
            }
        }

        /// <summary>
        /// Clears the SaveSystem's saved game data cache and loads a
        /// starting scene. Same as ResetGameState except loads a starting scene.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public static void RestartGame(string startingSceneName)
        {
            ResetGameState();
            instance.StartCoroutine(LoadSceneInternal(startingSceneName));
        }

        /// <summary>
        /// Clears the SaveSystem's saved game data cache. Same as
        /// RestartGame except it doesn't load a scene after resetting.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public static void ResetGameState()
        {
            ClearSavedGameData();
            BeforeSceneChange();
            SaversRestartGame();
        }

        /// <summary>
        /// Calls OnRestartGame on all savers.
        /// </summary>
        public static void SaversRestartGame()
        {
            if (m_savers.Count <= 0) return;
            for (int i = m_savers.Count - 1; i >= 0; i--) // A saver may remove itself from list during restart.
            {
                try
                {
                    if (0 <= i && i < m_savers.Count)
                    {
                        var saver = m_savers[i];
                        if (saver != null) saver.OnRestartGame();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Returns a serialized version of an object using whatever serializer is
        /// assigned to the SaveSystem (JSON by default).
        /// </summary>
        public static string Serialize(object data)
        {
            return serializer.Serialize(data);
        }

        /// <summary>
        /// Deserializes a previously-serialized string representation of an object
        /// back into an object. Uses whatever serializer is assigned to the 
        /// SaveSystem (JSON by default).
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="s">The object's serialized data.</param>
        /// <param name="data">Optional preallocated object to serialize data into.</param>
        /// <returns>The deserialized object, or null if it couldn't be deserialized.</returns>
        public static T Deserialize<T>(string s, T data = default(T))
        {
            return serializer.Deserialize<T>(s, data);
        }

    }
}