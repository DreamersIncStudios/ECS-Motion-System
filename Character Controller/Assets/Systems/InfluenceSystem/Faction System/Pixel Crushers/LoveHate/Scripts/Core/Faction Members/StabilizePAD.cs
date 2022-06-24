// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Stabilizes PAD values toward target values. Add this to a faction member to
    /// gradually "cool down" arousal to 0, for example.
    /// </summary>
    [RequireComponent(typeof(FactionMember))]
    [AddComponentMenu("")] // Use wrapper.
    public class StabilizePAD : MonoBehaviour
    {

        [System.Serializable]
        public class StabilizeSettings
        {
            [Tooltip("Stabilize this PAD value.")]
            public bool stabilize = false;

            [Tooltip("Stabilize the PAD value toward this target value.")]
            public float target = 0;

            [Tooltip("Change by this amount per second.")]
            public float changeRate = 0.1f;

            public float Apply(float current)
            {
                return (!stabilize || Mathf.Approximately(current, target)) ? target
                    : (current > target)
                        ? Mathf.Clamp(current - changeRate * GameTime.deltaTime, target, current)
                        : Mathf.Clamp(current + changeRate * GameTime.deltaTime, current, target);
            }
        }

        public StabilizeSettings happinessSettings;
        public StabilizeSettings pleasureSettings;
        public StabilizeSettings arousalSettings;
        public StabilizeSettings dominanceSettings;

        private FactionMember m_member = null;

        protected FactionMember member
        {
            get { return m_member; }
            set { m_member = value; }
        }

        protected virtual void Awake()
        {
            m_member = GetComponent<FactionMember>();
            if (m_member == null) enabled = false;
        }

        protected virtual void Update()
        {
            float happinessOld = m_member.pad.happiness;
            float pleasureOld = m_member.pad.pleasure;
            float arousalOld = m_member.pad.arousal;
            float dominanceOld = m_member.pad.dominance;
            float happinessNew = happinessOld;
            float pleasureNew = pleasureOld;
            float arousalNew = arousalOld;
            float dominanceNew = dominanceOld;
            if (happinessSettings.stabilize)
            {
                happinessNew = happinessSettings.Apply(m_member.pad.happiness);
            }
            if (pleasureSettings.stabilize)
            {
                pleasureNew = pleasureSettings.Apply(m_member.pad.pleasure);
            }
            if (arousalSettings.stabilize)
            {
                arousalNew = arousalSettings.Apply(m_member.pad.arousal);
            }
            if (dominanceSettings.stabilize)
            {
                dominanceNew = dominanceSettings.Apply(m_member.pad.dominance);
            }
            if (happinessNew != happinessOld || pleasureNew != pleasureOld ||
                arousalNew != arousalOld || dominanceNew != dominanceOld)
            {
                m_member.ModifyPAD(happinessNew - happinessOld, pleasureNew - pleasureOld,
                    arousalNew - arousalOld, dominanceNew - dominanceOld);
            }
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            controllable.Add("StabilizeSettings:happinessSettings~stabilize|System.Boolean|0|1|1|Stabilize happiness.|f,target|System.Single|-100|100|1|Value to stabilize toward.|f,changeRate|System.Single|-100|100|1|Change by this amount per second.|f");
            controllable.Add("StabilizeSettings:pleasureSettings~stabilize|System.Boolean|0|1|1|Stabilize pleasure.|f,target|System.Single|-100|100|1|Value to stabilize toward.|f,changeRate|System.Single|-100|100|1|Change by this amount per second.|f");
            controllable.Add("StabilizeSettings:arousalSettings~stabilize|System.Boolean|0|1|1|Stabilize arousal.|f,target|System.Single|-100|100|1|Value to stabilize toward.|f,changeRate|System.Single|-100|100|1|Change by this amount per second.|f");
            controllable.Add("StabilizeSettings:dominanceSettings~stabilize|System.Boolean|0|1|1|Stabilize dominance.|f,target|System.Single|-100|100|1|Value to stabilize toward.|f,changeRate|System.Single|-100|100|1|Change by this amount per second.|f");
            return controllable;
        }

    }

}
