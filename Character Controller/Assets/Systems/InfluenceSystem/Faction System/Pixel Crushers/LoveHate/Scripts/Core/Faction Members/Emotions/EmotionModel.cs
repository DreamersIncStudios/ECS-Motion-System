// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This scriptable object asset defines an emotion model, which is an EmotionDefinition list.
    /// It serves as the template for faction members' EmotionalState components.
    /// </summary>
    public class EmotionModel : ScriptableObject
    {

        public EmotionDefinition[] emotionDefinitions;

    }

}
