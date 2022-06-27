// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Provides more drawing routines for gizmos.
    /// </summary>
    public static class MoreGizmos
    {

        /// <summary>
        /// Draws an arrow, which is a ray with an arrowhead at the end.
        /// </summary>
        /// <param name='from'>
        /// Start of the ray.
        /// </param>
        /// <param name='direction'>
        /// Direction and length the ray extends.
        /// </param>
        /// <param name='arrowheadLength'>
        /// Length of the arrowhead that extends from the end the ray.
        /// </param>
        /// <param name='arrowheadAngle'>
        /// Angle of the arrowhead in degrees.
        /// </param>
        public static void DrawArrow(Vector3 from, Vector3 direction, float arrowheadLength = 0.2f, float arrowheadAngle = 30f)
        {
            if (Mathf.Approximately(direction.magnitude, 0)) return;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f + arrowheadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f - arrowheadAngle, 0) * Vector3.forward;
            Gizmos.DrawRay(from, direction);
            Gizmos.DrawRay(from + direction, right * arrowheadLength);
            Gizmos.DrawRay(from + direction, left * arrowheadLength);
        }

    }

}
