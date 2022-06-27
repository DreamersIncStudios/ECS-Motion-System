// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This optional component adds a more customizable emotion model based on PAD values than the
    /// PAD's built-in Temperament value.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [RequireComponent(typeof(FactionMember))]
    public class EmotionalState : MonoBehaviour, IModifyPadDeedEventHandler
    {

        /// <summary>
        /// The template to use when defining emotions.
        /// </summary>
        public EmotionModel emotionModelTemplate;

        /// <summary>
        /// The emotion definitions for this faction member.
        /// </summary>
        public EmotionDefinition[] emotionDefinitions = new EmotionDefinition[0];

        public enum EmotionMatchMode { BestFit, Sequentially }

        /// <summary>
        /// Specifies how to match emotion to PAD values.
        /// </summary>
        [Tooltip("- Best Fit: Match the emotion whose average values are closest to member's PAD.\n- Sequentially: Match first emotion in list whose ranges encompass member's PAD.")]
        public EmotionMatchMode emotionMatchMode = EmotionMatchMode.BestFit;

        /// <summary>
        /// Index into emotionDefinitions of the current emotion based on the faction member's PAD values.
        /// </summary>
        [Tooltip("Index into emotionDefinitions of the current emotion based on the faction member's PAD values.")]
        public int currentEmotion = -1;

        /// <summary>
        /// The emotion name associated with the current emotion.
        /// </summary>
        [Tooltip("The emotion name associated with the current emotion.")]
        public string currentEmotionName = string.Empty;

        private FactionMember m_member = null;

        private const float MaxEmotionDistance = 600;

        public void Awake()
        {
            m_member = GetComponent<FactionMember>();
        }

        /// <summary>
        /// Updates the current emotion based on the faction member's PAD values.
        /// </summary>
        public void UpdateEmotionalState()
        {
            GetCurrentEmotionName();
        }

        /// <summary>
        /// Returns the name of the current emotion determined by the faction member's PAD values.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentEmotionName()
        {
            var currentEmotion = GetCurrentEmotion();
            currentEmotionName = (0 <= currentEmotion && currentEmotion < emotionDefinitions.Length)
                ? emotionDefinitions[currentEmotion].name : string.Empty;
            return currentEmotionName;
        }

        /// <summary>
        /// Returns the current emotion determined by the faction member's PAD values.
        /// Identifies the emotion that most closely matches the PAD values.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentEmotion()
        {
            int closestEmotionIndex = -1;
            float closestDistance = MaxEmotionDistance;
            for (int i = 0; i < emotionDefinitions.Length; i++)
            {
                float distance = GetEmotionDistance(emotionDefinitions[i]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEmotionIndex = i;
                }
            }
            return closestEmotionIndex;
        }

        /// <summary>
        /// Returns `true` if the current PAD values fall within the ranges specified by
        /// an emotionDefinition.
        /// </summary>
        /// <param name="emotionDefinition">Emotion definition to check</param>
        /// <returns>True if within the range</returns>
        public bool IsWithinEmotionRange(EmotionDefinition emotionDefinition)
        {
            return (emotionDefinition.pleasureMin <= m_member.pad.pleasure && m_member.pad.pleasure <= emotionDefinition.pleasureMax) &&
                (emotionDefinition.arousalMin <= m_member.pad.arousal && m_member.pad.arousal <= emotionDefinition.arousalMax) &&
                (emotionDefinition.dominanceMin <= m_member.pad.dominance && m_member.pad.dominance <= emotionDefinition.dominanceMax);
        }

        /// <summary>
        /// Returns distance between PAD values and average of emotion's range.
        /// </summary>
        /// <param name="emotionDefinition">Emotion definition to check</param>
        /// <returns>Distance, or MaxEmotionDistance if PAD is not in emotion's range.</returns>
        public float GetEmotionDistance(EmotionDefinition emotionDefinition)
        {
            if (!IsWithinEmotionRange(emotionDefinition)) return MaxEmotionDistance;
            float avgPleasure = (emotionDefinition.pleasureMin + emotionDefinition.pleasureMax) / 2;
            float avgArousal = (emotionDefinition.arousalMin + emotionDefinition.arousalMax) / 2;
            float avgDominance = (emotionDefinition.dominanceMin + emotionDefinition.dominanceMax) / 2;
            return
                Mathf.Abs(m_member.pad.pleasure - avgPleasure) +
                Mathf.Abs(m_member.pad.arousal - avgArousal) +
                Mathf.Abs(m_member.pad.dominance - avgDominance);
        }

        /// <summary>
        /// Updates the current emotion whenever the faction member's PAD values change.
        /// </summary>
        public void OnModifyPad(float happinessChange, float pleasureChange, float arousalChange, float dominanceChange)
        {
            UpdateEmotionalState();
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            return controllable;
        }

    }

}
