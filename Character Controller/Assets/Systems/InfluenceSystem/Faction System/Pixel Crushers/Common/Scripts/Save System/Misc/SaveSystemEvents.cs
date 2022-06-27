// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Provides Save System UnityEvents.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystemEvents : MonoBehaviour
    {

        public UnityEvent onSaveStart = new UnityEvent();
        public UnityEvent onSaveEnd = new UnityEvent();
        public UnityEvent onLoadStart = new UnityEvent();
        public UnityEvent onLoadEnd = new UnityEvent();
        public UnityEvent onSaveDataApplied = new UnityEvent();
        public UnityEvent onSceneLoad = new UnityEvent();

        private void OnEnable()
        {
            UnregisterEvents();
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        { 
            SaveSystem.saveStarted += OnSaveStarted;
            SaveSystem.saveEnded += OnSaveEnded;
            SaveSystem.loadStarted += OnLoadStarted;
            SaveSystem.loadEnded += OnLoadEnded;
            SaveSystem.saveDataApplied += OnSaveDataApplied;
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        private void UnregisterEvents()
        {
            SaveSystem.saveStarted -= OnSaveStarted;
            SaveSystem.saveEnded -= OnSaveEnded;
            SaveSystem.loadStarted -= OnLoadStarted;
            SaveSystem.loadEnded -= OnLoadEnded;
            SaveSystem.saveDataApplied -= OnSaveDataApplied;
            SaveSystem.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSaveStarted()
        {
            onSaveStart.Invoke();
        }

        private void OnSaveEnded()
        {
            onSaveEnd.Invoke();
        }

        private void OnLoadStarted()
        {
            onLoadStart.Invoke();
        }

        private void OnLoadEnded()
        {
            onLoadEnd.Invoke();
        }

        private void OnSaveDataApplied()
        {
            onSaveDataApplied.Invoke();
        }

        private void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            onSceneLoad.Invoke();
        }
    }

}
