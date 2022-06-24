// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// A rumor is a faction member's subjective memory of a deed.
    /// </summary>
    [Serializable]
    public class Rumor : IComparable<Rumor>
    {

        /// <summary>
        /// A static pool of objects, to prevent garbage collection stutter.
        /// </summary>
        public static Pool<Rumor> pool = new Pool<Rumor>();

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            pool = new Pool<Rumor>();
        }
#endif

        /// <summary>
        /// The GUID of the deed this rumor is about. We remember the GUID in case 
        /// another character shares the same rumor; in this case, we already know
        /// about the deed so we don't need to evaluate it again.
        /// </summary>
        public Guid deedGuid;

        /// <summary>
        /// The type of deed (e.g., "attack", "compliment").
        /// </summary>
        public string tag;

        /// <summary>
        /// The actor faction that committed the deed.
        /// </summary>
        public int actorFactionID;

        /// <summary>
        /// The target faction that the deed was done to.
        /// </summary>
        public int targetFactionID;

        /// <summary>
        /// The impact of the deed, where -100 is the worst and +100 is the best.
        /// For example, killing a target in an awful way approaches -100, while
        /// saving a target's family approaches +100.
        /// </summary>
        [Range(-100, 100)]
        public float impact;

        /// <summary>
        /// How aggressive or submissive the deed is, where -100 is the most 
        /// submissive and +100 is the most aggressive.
        /// </summary>
        [Range(-100, 100)]
        public float aggression;

        /// <summary>
        /// The actor's power level.
        /// </summary>
        public float actorPowerLevel;

        /// <summary>
        /// The traits associated with the deed.
        /// </summary>
        public float[] traits;

        /// <summary>
        /// The number of times this deed was done to the target. For example, if an 
        /// actor repeatedly attacks a target, count increases rather than creating
        /// separate rumors for each sword swing.
        /// </summary>
        public int count = 1;

        /// <summary>
        /// The confidence the faction has in the source of the rumor. When the faction
        /// has higher affinity for the source, the rumor's confidence will be higher.
        /// </summary>
        [Range(0, 100)]
        public float confidence = 0;

        /// <summary>
        /// The pleasure that this rumor caused the faction.
        /// </summary>
        public float pleasure = 0;

        /// <summary>
        /// The arousal that this rumor caused the faction.
        /// </summary>
        public float arousal = 0;

        /// <summary>
        /// The dominance change that this rumor caused in the faction.
        /// </summary>
        public float dominance = 0;

        /// <summary>
        /// Who is allowed to evaluate the deed when witnessed or shared as gossip.
        /// </summary>
        public PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone;

        /// <summary>
        /// Is the memory important enough to remember?
        /// </summary>
        public bool memorable = false;

        /// <summary>
        /// The game time at which the rumor expires from short term memory.
        /// </summary>
        public float shortTermExpiration = 0;

        /// <summary>
        /// The game time at which the rumor expires from long term memory.
        /// </summary>
        public float longTermExpiration = 0;

        public bool isExpiredFromShortTerm { get { return GameTime.time > shortTermExpiration; } }

        public bool isExpiredFromLongTerm { get { return GameTime.time > longTermExpiration; } }

        public object customData = null;

        /// <summary>
        /// Expires the rumor so it will be removed from memory.
        /// </summary>
        public void Expire()
        {
            shortTermExpiration = 0;
            longTermExpiration = 0;
        }

        public void Clear()
        {
            deedGuid = Guid.Empty;
            tag = string.Empty;
            actorFactionID = 0;
            targetFactionID = 0;
            impact = 0;
            aggression = 0;
            actorPowerLevel = 0;
            confidence = 0;
            pleasure = 0;
            arousal = 0;
            dominance = 0;
            permittedEvaluators = PermittedEvaluators.Everyone;
            shortTermExpiration = 0;
            longTermExpiration = 0;
            customData = null;
        }

        public void AssignDeed(Deed deed)
        {
            Clear();
            deedGuid = deed.guid;
            tag = deed.tag;
            actorFactionID = deed.actorFactionID;
            targetFactionID = deed.targetFactionID;
            impact = deed.impact;
            aggression = deed.aggression;
            actorPowerLevel = deed.actorPowerLevel;
            Traits.Copy(deed.traits, ref traits);
            permittedEvaluators = deed.permittedEvaluators;
        }

        public void AssignRumor(Rumor rumor)
        {
            Clear();
            deedGuid = rumor.deedGuid;
            tag = rumor.tag;
            actorFactionID = rumor.actorFactionID;
            targetFactionID = rumor.targetFactionID;
            impact = rumor.impact;
            aggression = rumor.aggression;
            actorPowerLevel = rumor.actorPowerLevel;
            confidence = rumor.confidence;
            pleasure = rumor.pleasure;
            arousal = rumor.arousal;
            dominance = rumor.dominance;
            Traits.Copy(rumor.traits, ref traits);
            permittedEvaluators = rumor.permittedEvaluators;
        }

        public int CompareTo(Rumor other)
        {
            return (other == null) ? 1 : arousal.CompareTo(other.arousal);
        }

        public static Rumor GetNew()
        {
            var rumor = pool.Get();
            rumor.Clear();
            return rumor;
        }

        public static Rumor GetNew(Deed deed)
        {
            var rumor = GetNew();
            rumor.AssignDeed(deed);
            return rumor;
        }

        public static Rumor GetNew(Rumor sourceRumor)
        {
            var rumor = GetNew();
            rumor.AssignRumor(sourceRumor);
            return rumor;
        }

        public static void Release(Rumor rumor)
        {
            pool.Release(rumor);
        }

    }

}
