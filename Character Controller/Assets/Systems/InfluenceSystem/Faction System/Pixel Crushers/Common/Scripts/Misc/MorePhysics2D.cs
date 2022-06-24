// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

    /// <summary>
    /// Provides more routines for Physics2D.
    /// </summary>
    public static class MorePhysics2D
    {

        /// <summary>
        /// Wrapper for Physics2D.queriesStartInColliders.
        /// </summary>
        public static bool queriesStartInColliders
        {
            get
            {
                return Physics2D.queriesStartInColliders;
            }
            set
            {
                Physics2D.queriesStartInColliders = value;
            }
        }

        /// <summary>
        /// Size of the preallocated array for nonallocating raycasts.
        /// </summary>
        public static int maxRaycastResults
        {
            get
            {
                return raycastResults.Length;
            }
            set
            {
                if (value != raycastResults.Length)
                {
                    raycastResults = new RaycastHit2D[value];
                }
            }
        }

        private static RaycastHit2D[] raycastResults = new RaycastHit2D[20];

        /// <summary>
        /// Runs a nonallocating linecast, ignoring the source.
        /// </summary>
        public static GameObject Raycast2DWithoutSelf(Transform source, Transform destination, LayerMask layerMask)
        {
            var start2D = new Vector2(source.position.x, source.position.y);
            var end2D = new Vector2(destination.position.x, destination.position.y);
            var originalRaycastsStartInColliders = MorePhysics2D.queriesStartInColliders;
            MorePhysics2D.queriesStartInColliders = false;
            var numResults = Physics2D.LinecastNonAlloc(start2D, end2D, raycastResults, layerMask);
            MorePhysics2D.queriesStartInColliders = originalRaycastsStartInColliders;
            for (int i = 0; i < numResults; i++)
            {
                var result = raycastResults[i];
                if (result.transform == source) continue; // Skip source.
                return result.collider.gameObject; // Array is in distance order, so return first non-source.
            }
            return null;
        }

    }

#else

    /// <summary>
    /// Provides more routines for Physics2D.
    /// </summary>
    public static class MorePhysics2D
    {


        /// <summary>
        /// Stub wrapper for Physics2D.queriesStartInColliders.
        /// </summary>
        public static bool queriesStartInColliders
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Stub for size of the preallocated array for nonallocating raycasts.
        /// </summary>
        public static int maxRaycastResults
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Stub for running a nonallocating linecast, ignoring the source.
        /// </summary>
        public static GameObject Raycast2DWithoutSelf(Transform source, Transform destination, LayerMask layerMask)
        {
            LogUsePhysics2DWarning();
            return null;
        }

        public static void LogUsePhysics2DWarning()
        {
            if (Debug.isDebugBuild) Debug.LogWarning("To enable Physics2D support for a Pixel Crushers asset, add the Scripting Define Symbol 'USE_PHYSICS2D'.");
        }

    }

#endif


}
