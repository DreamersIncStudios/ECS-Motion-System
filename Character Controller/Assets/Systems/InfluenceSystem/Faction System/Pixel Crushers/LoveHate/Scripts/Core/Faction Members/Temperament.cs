// Copyright (c) Pixel Crushers. All rights reserved.

using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Temperament specifies an emotional state. It's a function of 
    /// Pleasure (P), Arousal (A), and Dominance (D).
    /// </summary>
    [Flags]
    public enum Temperament
    {
        Exuberant = 1,   // P+ A+ D+
        Bored = 2,       // P- A- D-
        Dependent = 4,   // P+ A+ D-
        Disdainful = 8,  // P- A- D+
        Relaxed = 16,    // P+ A- D+
        Anxious = 32,    // P- A+ D-
        Docile = 64,     // P+ A- D-
        Hostile = 128,   // P- A+ D+
        Neutral = 256    // All zero
    }

}