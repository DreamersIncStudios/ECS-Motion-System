// Copyright (c) Pixel Crushers. All rights reserved.

using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Emotion definition for EmotionModel and EmotionalState.
    /// </summary>
    [Serializable]
    public class EmotionDefinition
    {
        public string name;

        public float pleasureMin = -100;
        public float pleasureMax = 100;
        public float arousalMin = -100;
        public float arousalMax = 100;
        public float dominanceMin = -100;
        public float dominanceMax = 100;

    }

}
