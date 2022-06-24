// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    public enum PermittedEvaluators
    {
        /// <summary>
        /// Any faction member is allowed to evaluate the deed.
        /// </summary>
        Everyone,

        /// <summary>
        /// Only the target of the deed is allowed to evaluate it.
        /// </summary>
        OnlyTarget,

        /// <summary>
        /// Any faction member except the target is allowed to evaluate it.
        /// </summary>
        EveryoneExceptTarget
    }

    /// <summary>
    /// A deed is an objective record of an act committed by an actor to a target.
    /// </summary>
    [Serializable]
    public class Deed
    {

        /// <summary>
        /// A static pool of objects, to prevent garbage collection stutter.
        /// </summary>
        public static Pool<Deed> pool = new Pool<Deed>();

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            pool = new Pool<Deed>();
        }
#endif

        /// <summary>
        /// The GUID for this deed, used to determine if a faction has already
        /// heard about the deed when sharing rumors.
        /// </summary>
        public Guid guid;

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
        /// Who is allowed to evaluate the deed when witnessed or shared as gossip.
        /// </summary>
        public PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone;

        public int refCount;

        /// <summary>
        /// Assigns content to a deed object.
        /// </summary>
        /// <param name="tag">The deed tag (e.g., "attack", "compliment").</param>
        /// <param name="actorFactionID">Faction ID of the actor that committed the deed.</param>
        /// <param name="targetFactionID">Faction ID of the target that the action was done to.</param>
        /// <param name="impact">Impact to the target, where -100 is worst and +100 is best.</param>
        /// <param name="aggression">Aggressiveness of the deed [-100,+100].</param>
        /// <param name="actorPowerLevel">The actor's power level.</param>
        /// <param name="traits">Traits associated with the action.</param>
        /// <param name="permittedEvaluators">Who is allowed to evaluate the deed.</param>
        public void Assign(string tag, int actorFactionID, int targetFactionID, float impact,
                           float aggression, float actorPowerLevel, float[] traits, PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone)
        {
            this.guid = Guid.NewGuid();
            this.tag = tag;
            this.actorFactionID = actorFactionID;
            this.targetFactionID = targetFactionID;
            this.impact = impact;
            this.aggression = aggression;
            this.actorPowerLevel = actorPowerLevel;
            this.permittedEvaluators = permittedEvaluators;
            this.refCount = 1;
            Traits.Copy(traits, ref this.traits);
        }

        /// <summary>
        /// Gets a new deed object from the pool.
        /// </summary>
        /// <returns>The new.</returns>
        /// <param name="tag">Tag.</param>
        /// <param name="actorFactionID">Actor faction ID.</param>
        /// <param name="targetFactionID">Target faction ID.</param>
        /// <param name="impact">Impact.</param>
        /// <param name="traits">Traits.</param>
        /// <param name="permittedEvaluators">Who is allowed to evaluate the deed.</param>
        public static Deed GetNew(string tag, int actorFactionID, int targetFactionID, float impact,
                                  float aggression, float actorPowerLevel, float[] traits, PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone)
        {
            var deed = pool.Get();
            deed.Assign(tag, actorFactionID, targetFactionID, impact, aggression, actorPowerLevel, traits, permittedEvaluators);
            return deed;
        }

        /// <summary>
        /// Releases a deed object back to the pool.
        /// </summary>
        /// <param name="deed">Deed.</param>
        public static void Release(Deed deed)
        {
            if (deed == null) return;
            deed.refCount--;
            if (deed.refCount <= 0)
            {
                pool.Release(deed);
            }
        }

    }

}
