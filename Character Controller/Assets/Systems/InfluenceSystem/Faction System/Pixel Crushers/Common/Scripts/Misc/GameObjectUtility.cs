// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Utility functions for working with GameObjects.
    /// </summary>
    public static class GameObjectUtility
    {

        /// <summary>
        /// Determines if a GameObject reference is a non-instantiated prefab or a scene object.
        /// If `go` is `null`, active in the scene, or its parent is active in the scene, it's
        /// considered a scene object. Otherwise this method searches all scene objects for
        /// matches. If it doesn't find any matches, this is a prefab.
        /// </summary>
        /// <returns><c>true</c> if a prefab; otherwise, <c>false</c>.</returns>
        /// <param name="go">GameObject.</param>
        public static bool IsPrefab(GameObject go)
        {
            if (go == null) return false;
            if (go.activeInHierarchy) return false;
            if ((go.transform.parent != null) && go.transform.parent.gameObject.activeSelf) return false;
            var list = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == go) return false;
            }
            return true;
        }

        /// <summary>
        /// Finds an in-scene GameObject by name even if it's inactive.
        /// </summary>
        /// <param name="goName">Name of the GameObject.</param>
        /// <param name="checkAllScenes">If true, check all open scenes; otherwise only check active scene.</param>
        /// <returns>The GameObject.</returns>
        public static GameObject GameObjectHardFind(string goName, bool checkAllScenes = true)
        {
            if (string.IsNullOrEmpty(goName)) return null;

            // Try the normal method to find an active GameObject first:
            GameObject result = GameObject.Find(goName);
            if (result != null) return result;

            // Otherwise check all GameObjects, active and inactive:
            if (checkAllScenes)
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                    result = GameObjectHardFindRootObjects(goName, string.Empty, rootGameObjects);
                    if (result != null) return result;
                }
                return null;
            }
            else
            {
                var activeSceneRootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                return GameObjectHardFindRootObjects(goName, string.Empty, activeSceneRootGameObjects);
            }
        }

        /// <summary>
        /// Finds an in-scene GameObject by name and tag even if it's inactive.
        /// </summary>
        /// <param name="goName">GameObject name to search for.</param>
        /// <param name="tag">Tag to search for.</param>
        /// <param name="checkAllScenes">If true, check all open scenes; otherwise only check active scene.</param>
        /// <returns></returns>
        public static GameObject GameObjectHardFind(string goName, string tag, bool checkAllScenes = true)
        {
            // Try the normal method to find active GameObjects with tag first:
            GameObject result = null;
            var gameObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in gameObjects)
            {
                if (string.Equals(go.name, goName)) return go;
            }

            // Otherwise check all GameObjects, active and inactive:
            if (checkAllScenes)
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                    result = GameObjectHardFindRootObjects(goName, tag, rootGameObjects);
                    if (result != null) return result;
                }
                return null;
            }
            else
            {
                var activeSceneRootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                return GameObjectHardFindRootObjects(goName, tag, activeSceneRootGameObjects);
            }
        }

        private static GameObject GameObjectHardFindRootObjects(string goName, string tag, GameObject[] rootGameObjects)
        {
            if (rootGameObjects == null) return null;
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                var result = GameObjectSearchHierarchy(rootGameObjects[i].transform, goName, tag);
                if (result != null) return result;
            }
            return null;
        }

        private static GameObject GameObjectSearchHierarchy(Transform t, string goName, string tag)
        {
            if (t == null) return null;
            if (string.Equals(t.name, goName) && (string.IsNullOrEmpty(tag) || string.Equals(t.tag, tag))) return t.gameObject;
            foreach (Transform child in t)
            {
                var result = GameObjectSearchHierarchy(child, goName, tag);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Finds all objects of a type, including on inactive GameObjects.
        /// <param name="checkAllScenes">If true, check all open scenes; otherwise only check active scene.</param>
        /// </summary>
        public static T[] FindObjectsOfTypeAlsoInactive<T>(bool checkAllScenes = true) where T : Component
        {
            var list = new System.Collections.Generic.List<T>();


            // Otherwise check all GameObjects, active and inactive:
            if (checkAllScenes)
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                    FindObjectsSearchRootObjects<T>(rootGameObjects, list);
                }
            }
            else
            {
                var activeSceneRootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                FindObjectsSearchRootObjects<T>(activeSceneRootGameObjects, list);
            }

            return list.ToArray();
        }

        private static void FindObjectsSearchRootObjects<T>(GameObject[] rootGameObjects, System.Collections.Generic.List<T> list) where T : Component
        {
            if (rootGameObjects == null) return;
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                FindObjectsSearchHierarchy<T>(rootGameObjects[i].transform, list);
            }
        }

        private static void FindObjectsSearchHierarchy<T>(Transform t, System.Collections.Generic.List<T> list) where T : Component
        {
            if (t == null) return;
            var components = t.GetComponents<T>();
            if (components.Length > 0) list.AddRange(components);
            foreach (Transform child in t)
            {
                FindObjectsSearchHierarchy<T>(child, list);
            }
        }

        /// <summary>
        /// Like GetComponentInChildren(), but also searches parents.
        /// </summary>
        /// <returns>The component, or <c>null</c> if not found.</returns>
        /// <param name="gameObject">Game object to search.</param>
        /// <typeparam name="T">The component type.</typeparam>
        public static T GetComponentAnywhere<T>(GameObject gameObject) where T : Component
        {
            if (!gameObject) return null;
            T component = gameObject.GetComponentInChildren<T>();
            if (component) return component;
            Transform ancestor = gameObject.transform.parent;
            int safeguard = 0;
            while (!component && ancestor && safeguard < 256)
            {
                component = ancestor.GetComponentInChildren<T>();
                ancestor = ancestor.parent;
            }
            return component;
        }

        /// <summary>
        /// Gets the height of the game object based on its collider. This only works if the
        /// game object has a CharacterController, CapsuleCollider, BoxCollider, or SphereCollider.
        /// </summary>
        /// <returns>The game object height if it has a recognized type of collider; otherwise <c>0</c>.</returns>
        /// <param name="gameObject">Game object.</param>
        public static float GetGameObjectHeight(GameObject gameObject)
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                return controller.height;
            }
            else
            {
                CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
                if (capsuleCollider != null)
                {
                    return capsuleCollider.height;
                }
                else
                {
                    BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        return boxCollider.center.y + boxCollider.size.y;
                    }
                    else
                    {
                        SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
                        if (sphereCollider != null)
                        {
                            return sphereCollider.center.y + sphereCollider.radius;
                        }
                    }
                }
            }
            return 0;
        }

    }

}