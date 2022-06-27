// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Defines a field of view, which is an area with a horizontal arc and vertical arc.
    /// </summary>
    [Serializable]
    public class FieldOfView
    {

        /// <summary>
        /// The horizontal viewing angle in degrees.
        /// </summary>
        [Tooltip("The horizontal viewing angle in degrees.")]
        public float horizontalFOV = 120f;

        /// <summary>
        /// The vertical viewing angle in degrees.
        /// </summary>
        [Tooltip("The vertical viewing angle in degrees.")]
        public float verticalFOV = 120f;

        /// <summary>
        /// The max distance from the sensor that the field of view extends.
        /// </summary>
        [Tooltip("The max distance from the sensor that the field of view extends.")]
        public float maxDistance = 10f;

        /// <summary>
        /// The color to use when drawing this field of view in the Scene view.
        /// </summary>
		[Tooltip("The color to use when drawing this field of view in the Scene view.")]
        public Color gizmoColor = new Color(1, 1, 1, 0.25f);

        public FieldOfView() { }

        /// <summary>
        /// Creates a new field of view.
        /// </summary>
        /// <param name="horizontalFOV">Horizontal viewing angle in degrees.</param>
        /// <param name="verticalFOV">Vertical viewing angle in degrees.</param>
        /// <param name="maxDistance">Max distance from the sensor.</param>
        /// <param name="gizmoColor">Color to use when drawing the Scene view gizmo.</param>
		public FieldOfView(float horizontalFOV, float verticalFOV, float maxDistance, Color gizmoColor)
        {
            this.horizontalFOV = horizontalFOV;
            this.verticalFOV = verticalFOV;
            this.maxDistance = maxDistance;
            this.gizmoColor = gizmoColor;
        }

        /// <summary>
        /// Copies a field of view.
        /// </summary>
        /// <param name="fov">Source field of view.</param>
        public FieldOfView(FieldOfView fov)
        {
            if (fov == null) return;
            horizontalFOV = fov.horizontalFOV;
            verticalFOV = fov.verticalFOV;
            maxDistance = fov.maxDistance;
            gizmoColor = fov.gizmoColor;
        }

        /// <summary>
        /// Determines whether a target is within the field of view that extends from
        /// the sensor's origin point.
        /// </summary>
        /// <param name="origin">Sensor's origin point.</param>
        /// <param name="target">Target to check.</param>
        /// <param name="dimension">Specifies whether to check in 2D or 3D.</param>
        /// <returns>`true` if the target is within the field of view; `false` otherwise.</returns>
		public bool Contains(Transform origin, Transform target, Dimension dimension)
        {
            float distance = Vector3.Distance(origin.position, target.position);
            if (distance > maxDistance) return false;
            if (dimension == Dimension.Is2D)
            {
                float verticalAngle = Vector3.Angle(origin.right, Vector3.Normalize(target.position - origin.position));
                return (verticalAngle < (0.5f * verticalFOV));
            }
            else
            {
                Vector3 targetHorizontalPosition = new Vector3(target.position.x, origin.position.y, target.position.z);
                float horizontalAngle = Vector3.Angle(origin.forward, Vector3.Normalize(targetHorizontalPosition - origin.position));
                if (horizontalAngle > (0.5f * horizontalFOV)) return false;
                float verticalAngle = Vector3.Angle(origin.forward, Vector3.Normalize(target.position - origin.position));
                return (verticalAngle < (0.5f * verticalFOV));
            }
        }
    }

}
