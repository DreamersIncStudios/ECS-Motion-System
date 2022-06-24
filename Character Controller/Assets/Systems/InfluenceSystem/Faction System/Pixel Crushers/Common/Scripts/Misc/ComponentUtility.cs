// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Utility functions for working with components.
    /// </summary>
    public static class ComponentUtility
    {

        /// <summary>
        /// Returns true if the component is enabled; false otherwise.
        /// </summary>
        public static bool IsComponentEnabled(Component component)
        {
            if (component is Behaviour) return (component as Behaviour).enabled;
            if (component is Renderer) return (component as Renderer).enabled;
            if (component is Collider) return (component as Collider).enabled;
            if (component is Animation) return (component as Animation).enabled;
            if (component is Animator) return (component as Animator).enabled;
            if (component is AudioSource) return (component as AudioSource).enabled;
            return false;
        }

        /// <summary>
        /// Sets a component's enabled state.
        /// </summary>
        public static void SetComponentEnabled(Component component, bool value)
        {
            if (component == null) return;
            if (component is Behaviour) (component as Behaviour).enabled = value;
            if (component is Renderer) (component as Renderer).enabled = value;
            if (component is Collider) (component as Collider).enabled = value;
            if (component is Animation) (component as Animation).enabled = value;
            if (component is Animator) (component as Animator).enabled = value;
            if (component is AudioSource) (component as AudioSource).enabled = value;
        }

    }

}