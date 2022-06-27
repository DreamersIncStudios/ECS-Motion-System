// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Service to notify subscribers when a scene is being unloaded.
    /// </summary>
    public static class SceneNotifier
    {

        public delegate void UnloadSceneDelegate(int sceneIndex);

        /// <summary>
        /// Invoked by NotifyWillUnloadScene(sceneIndex), which should be called
        /// before unloading a scene.
        /// </summary>
        public static event UnloadSceneDelegate willUnloadScene = delegate { };

        /// <summary>
        /// Notifies all subscribers that the scene with the specified index will be unloaded.
        /// </summary>
        /// <param name="sceneIndex">Scene index in build settings.</param>
        public static void NotifyWillUnloadScene(int sceneIndex)
        {
            willUnloadScene(sceneIndex);
        }

    }

}