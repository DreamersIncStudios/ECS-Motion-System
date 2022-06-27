// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This static wrapper class provides easy access to FactionManager methods.
    /// </summary>
    public static class LoveHate
    {

        #region Properties

        /// <summary>
        /// The current FactionManager instance.
        /// </summary>
        public static FactionManager factionManager { get { return FactionManager.instance; } }

        /// <summary>
        /// The faction database managed by FactionManager instance.
        /// </summary>
        public static FactionDatabase factionDatabase { get { return factionManager ? factionManager.factionDatabase : null; } }

        /// <summary>
        /// When a deed is committed, process this many potential witnesses each Update.
        /// </summary>
        public static int witnessesPerUpdate
        {
            get { return factionManager ? factionManager.witnessesPerUpdate : 1; }
            set { if (factionManager) factionManager.witnessesPerUpdate = value; }
        }

        /// <summary>
        /// Faction members can witness their own deeds.
        /// </summary>
        public static bool canWitnessSelf
        {
            get { return factionManager ? factionManager.canWitnessSelf : false; }
            set { if (factionManager) factionManager.canWitnessSelf = value; }
        }

        /// <summary>
        /// Log activity to the console.
        /// </summary>
        public static bool debug
        {
            get { return factionManager ? factionManager.debug : false; }
            set { if (factionManager) factionManager.debug = value; }
        }

        #endregion

        #region Faction Lookup

        /// <summary>
        /// Looks up a faction by its ID.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionID">Faction ID.</param>
        public static Faction GetFaction(int factionID)
        {
            return factionManager ? factionManager.GetFaction(factionID, false) : null;
        }

        /// <summary>
        /// Looks up a faction by its ID. Doesn't log any warning messages.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionID">Faction ID.</param>
        public static Faction GetFactionSilent(int factionID)
        {
            return factionManager ? factionManager.GetFactionSilent(factionID) : null;
        }

        /// <summary>
        /// Looks up a faction by its ID.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="silent">If set to <c>true</c>, don't log warnings.</param>
        public static Faction GetFaction(int factionID, bool silent)
        {
            return factionManager ? factionManager.GetFaction(factionID, silent) : null;
        }

        /// <summary>
        /// Looks up a faction by its name.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionName">Faction name.</param>
        public static Faction GetFaction(string factionName)
        {
            return factionManager ? factionManager.GetFaction(factionName) : null;
        }

        /// <summary>
        /// Looks up a faction ID by its name.
        /// </summary>
        /// <returns>The faction ID.</returns>
        /// <param name="factionName">Faction name.</param>
        public static int GetFactionID(string factionName)
        {
            return factionManager ? factionManager.GetFactionID(factionName) : -1;
        }

        #endregion

        #region Parents

        /// <summary>
        /// Checks if a faction has an ancestor (parent, grandparent, etc.)
        /// </summary>
        /// <returns><c>true</c>, if has ancestor, <c>false</c> otherwise.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="ancestorID">Ancestor ID.</param>
        public static bool FactionHasAncestor(int factionID, int ancestorID)
        {
            return factionManager ? factionManager.FactionHasAncestor(factionID, ancestorID) : false;
        }

        /// <summary>
        /// Checks if a faction has a direct parent.
        /// </summary>
        /// <returns><c>true</c>, if has direct parent, <c>false</c> otherwise.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        public static bool FactionHasDirectParent(int factionID, int parentID)
        {
            return factionManager ? factionManager.FactionHasDirectParent(factionID, parentID) : false;
        }

        /// <summary>
        /// Adds a parent to a faction.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        public static void AddFactionParent(int factionID, int parentID)
        {
            if (factionManager) factionManager.AddFactionParent(factionID, parentID);
        }

        /// <summary>
        /// Removes a parent from a faction.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        /// <param name="inheritRelationships">If set to <c>true</c> keep the parent's 
        /// relationships if the faction doesn't already have a personal relationship.</param>
        public static void RemoveFactionParent(int factionID, int parentID, bool inheritRelationships)
        {
            if (factionManager) factionManager.RemoveFactionParent(factionID, parentID, inheritRelationships);
        }

        #endregion

        #region Relationships

        /// <summary>
        /// Finds this faction's personal affinity for another faction. Doesn't check parents.
        /// </summary>
        /// <returns>`true` if personal affinity was found, `false` otherwise.</returns>
        /// <param name="subjectFactionID">The other faction ID.</param>
        /// <param name="affinity">Affinity of this faction to the subject.</param>
        public static bool FindPersonalAffinity(int judgeFactionID, int subjectFactionID, out float affinity)
        {
            affinity = 0;
            return factionManager ? factionManager.FindPersonalAffinity(judgeFactionID, subjectFactionID, out affinity) : false;
        }

        /// <summary>
        /// Finds the affinity of a judging faction to a subject faction.
        /// </summary>
        /// <returns>`true`, if affinity was found, `false` otherwise.</returns>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinity">Affinity of judge to subject.</param>
        public static bool FindAffinity(int judgeFactionID, int subjectFactionID, out float affinity)
        {
            affinity = 0;
            return factionManager ? factionManager.FindPersonalAffinity(judgeFactionID, subjectFactionID, out affinity) : false;
        }

        /// <summary>
        /// Gets the affinity of a judging faction to a subject faction.
        /// </summary>
        /// <returns>The affinity.</returns>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        public static float GetAffinity(int judgeFactionID, int subjectFactionID)
        {
            return factionManager ? factionManager.GetAffinity(judgeFactionID, subjectFactionID) : 0;
        }

        /// <summary>
        /// Gets a judge's affinity to a subject using their faction names.
        /// </summary>
        /// <returns>The affinity.</returns>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        public static float GetAffinity(string judgeFactionName, string subjectFactionName)
        {
            return factionManager ? factionManager.GetAffinity(judgeFactionName, subjectFactionName) : 0;
        }

        /// <summary>
        /// Sets this faction's personal affinity to another faction.
        /// </summary>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinity">Affinity to the other faction.</param>
        public static void SetPersonalAffinity(int judgeFactionID, int subjectFactionID, float affinity)
        {
            if (factionManager) factionManager.SetPersonalAffinity(judgeFactionID, subjectFactionID, affinity);
        }

        /// <summary>
        /// Sets a judge's personal affinity to a subject using their faction names.
        /// </summary>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        /// <param name="affinity">Affinity.</param>
        public static void SetPersonalAffinity(string judgeFactionName, string subjectFactionName, float affinity)
        {
            if (factionManager) factionManager.SetPersonalAffinity(judgeFactionName, subjectFactionName, affinity);
        }

        /// <summary>
        /// Modifies (increments or decrements) a judge's personal affinity to a subject 
        /// using their faction IDs.
        /// </summary>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinityChange">Affinity change.</param>
        public static void ModifyPersonalAffinity(int judgeFactionID, int subjectFactionID, float affinityChange)
        {
            if (factionManager) factionManager.ModifyPersonalAffinity(judgeFactionID, subjectFactionID, affinityChange);
        }

        /// <summary>
        /// Modifies (increments or decrements) a judge's personal affinity to a subject 
        /// using their faction names.
        /// </summary>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        /// <param name="affinityChange">Affinity change.</param>
        public static void ModifyPersonalAffinity(string judgeFactionName, string subjectFactionName, float affinityChange)
        {
            if (factionManager) factionManager.ModifyPersonalAffinity(judgeFactionName, subjectFactionName, affinityChange);
        }

        /// <summary>
        /// If the faction has personal affinity for a subject and the other doesn't, this
        /// gives the other an affinity for the subject based on the other's affinity
        /// for this faction.
        /// </summary>
        /// <param name="judgeFactionID">Judging faction ID (gives affinity).</param>
        /// <param name="otherFactionID">Other faction ID (maybe receives affinity).</param>
        /// <param name="subjectFactionID">Subject faction ID (affinity about this faction).</param>
        public static void ShareAffinity(int judgeFactionID, int otherFactionID, int subjectFactionID)
        {
            if (factionManager) factionManager.ShareAffinity(judgeFactionID, otherFactionID, subjectFactionID);
        }

        #endregion

        #region Commit Deed

        /// <summary>
        /// Informs factions that a deed was committed.
        /// </summary>
        /// <param name="actor">FactionMember who committed the deed.</param>
        /// <param name="deed">Deed.</param>
        /// <param name="requiresSight">If `true`, the deed must be seen to be witnessed.</param>
        /// <param name="dimension">The world space dimension to use for radius checks.</param>
        /// <param name="radius">Max distance from position that factions can witness the deed (0=anywhere).</param>
        public static void CommitDeed(FactionMember actor, Deed deed, bool requiresSight, Dimension dimension, float radius)
        {
            if (factionManager) factionManager.CommitDeed(actor, deed, requiresSight, dimension, radius);
        }

        /// <summary>
        /// Commits the deed. This version has no distance restrictions.
        /// </summary>
        /// <param name="actor">Actor.</param>
        /// <param name="deed">Deed.</param>
        /// <param name="requiresSight">If set to <c>true</c> requires sight.</param>
        public static void CommitDeed(FactionMember actor, Deed deed, bool requiresSight)
        {
            CommitDeed(actor, deed, requiresSight, Dimension.Is3D, 0);
        }

        #endregion

    }
}
