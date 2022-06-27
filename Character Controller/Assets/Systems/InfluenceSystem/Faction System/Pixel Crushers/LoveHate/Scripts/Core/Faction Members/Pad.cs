// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// An emotional state, based on the PAD model: https://en.wikipedia.org/wiki/PAD_emotional_state_model
    /// </summary>
    [Serializable]
    public class Pad
    {

        /// <summary>
        /// The sum of all pleasure values over the whole lifetime.
        /// </summary>
        [Tooltip("The sum of all pleasure values over the whole lifetime.")]
        [Range(-100, 100)]
        public float happiness = 0;

        /// <summary>
        /// How happy or sad the actor currently is.
        /// </summary>
        [Tooltip("How happy or sad the actor currently is.")]
        [Range(-100, 100)]
        public float pleasure = 0;

        /// <summary>
        /// How worked up/excited the actor currently is.
        /// </summary>
        [Tooltip("How worked up/excited the actor currently is.")]
        [Range(-100, 100)]
        public float arousal = 0;

        /// <summary>
        /// How dominant or submissive the actor feels in the current situation.
        /// </summary>
        [Tooltip("How dominant or submissive the actor feels in the current situation.")]
        [Range(-100, 100)]
        public float dominance = 0;

        /// <summary>
        /// The threshold above which the PAD values above affect temperament.
        /// </summary>
        [Tooltip("The threshold above which the PAD values above affect temperament.")]
        [Range(0, 100)]
        public float excitabilityThreshold = 20f;

        /// <summary>
        /// Modifies the PAD values.
        /// </summary>
        /// <param name="happinessChange">Happiness change.</param>
        /// <param name="pleasureChange">Pleasure change.</param>
        /// <param name="arousalChange">Arousal change.</param>
        /// <param name="dominanceChange">Dominance change.</param>
        public void Modify(float happinessChange, float pleasureChange, float arousalChange, float dominanceChange)
        {
            happiness = Mathf.Clamp(happiness + happinessChange, -100, 100);
            pleasure = Mathf.Clamp(pleasure + pleasureChange, -100, 100);
            arousal = Mathf.Clamp(arousal + arousalChange, -100, 100);
            dominance = Mathf.Clamp(dominance + dominanceChange, -100, 100);
        }

        /// <summary>
        /// Gets the actor's temperament based on current PAD values.
        /// </summary>
        /// <returns>The temperament.</returns>
        public Temperament GetTemperament()
        {
            var happy = (pleasure > excitabilityThreshold);
            var unhappy = (pleasure < -excitabilityThreshold);
            var aroused = (arousal > excitabilityThreshold);
            var lulled = (arousal < -excitabilityThreshold);
            var dominant = (dominance > excitabilityThreshold);
            var submissive = (dominance < -excitabilityThreshold);
            //---Unused: var stronger = (dominance > 0);
            if (happy && aroused && !submissive) { return Temperament.Exuberant; }
            else if (happy && aroused && submissive) { return Temperament.Dependent; }
            else if (happy && lulled && submissive) { return Temperament.Docile; }
            else if (happy && lulled && !submissive) { return Temperament.Relaxed; }
            else if (unhappy && aroused && dominant) { return Temperament.Hostile; }
            else if (unhappy && aroused) { return Temperament.Anxious; }
            else if (unhappy && lulled && dominant) { return Temperament.Disdainful; }
            else if (unhappy && lulled && !dominant) { return Temperament.Bored; }
            else { return Temperament.Neutral; }
        }

    }

}
