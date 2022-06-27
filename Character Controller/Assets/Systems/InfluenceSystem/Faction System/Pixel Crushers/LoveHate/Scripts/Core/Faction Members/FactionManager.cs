// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Coordinates data exchange between factions. If your scene has any faction members,
    /// it must have a faction manager.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class FactionManager : MonoBehaviour
    {

        /// <summary>
        /// The faction database managed by this faction manager.
        /// </summary>
        [Tooltip("The faction database managed by this faction manager.")]
        public FactionDatabase factionDatabase;

        /// <summary>
        /// When a deed is committed, process this many potential witnesses each Update.
        /// </summary>
        [Tooltip("When a deed is committed, process this many potential witnesses each Update.")]
        public int witnessesPerUpdate = 60;

        /// <summary>
        /// Faction members can witness their own deeds.
        /// </summary>
        [Tooltip("Faction members can witness their own deeds.")]
        public bool canWitnessSelf = false;

        /// <summary>
        /// If a faction manager already exists when this one tries to start, destroy this one instead.
        /// </summary>
        [Tooltip("If a faction manager already exists when this one tries to start, destroy this one instead.")]
        public bool allowOnlyOneFactionManager = false;

        /// <summary>
        /// Tick to log activity to the console.
        /// </summary>
        [Tooltip("Log activity to the console.")]
        public bool debug;

        private const int MaxParentSearchDepth = 32;

        private Dictionary<int, Faction> m_factions = new Dictionary<int, Faction>();

        private Dictionary<Faction, List<FactionMember>> m_members = new Dictionary<Faction, List<FactionMember>>();

        private Queue<WitnessQueueItem> m_witnessQueue = new Queue<WitnessQueueItem>();

        public Dictionary<int, Faction> factions { get { return m_factions; } }

        public Dictionary<Faction, List<FactionMember>> members { get { return m_members; } }

        private static FactionManager m_instance = null;

        private bool m_hasInitialized = false;

        public static FactionManager instance
        {
            get { return m_instance; }
            set { m_instance = value; }
        }

        #region Initialization

        protected virtual void Awake()
        {
            if (allowOnlyOneFactionManager && m_instance != null)
            {
                if (debug) Debug.Log("Love/Hate: A Faction Manager already exists. Destroying this new one.");
                Destroy(this.gameObject);
                return;
            }
            m_instance = this;
            Initialize();
        }

        protected virtual void Start()
        { 
            if (factionDatabase == null)
            {
                Debug.LogWarning("Love/Hate: Assign a faction database to FactionManager.", this);
            }
            else if (!m_hasInitialized)
            {
                Initialize();
            }
    
        }
  
    

        private void Initialize()
        {
            if (factionDatabase == null) return;
            factionDatabase = Instantiate(factionDatabase) as FactionDatabase;
            m_factions.Clear();
            for (int f = 0; f < factionDatabase.factions.Length; f++)
            {
                var faction = factionDatabase.factions[f];
                m_factions.Add(faction.id, faction);
            }
            m_members.Clear();
            m_witnessQueue.Clear();
            m_hasInitialized = true;
            if (!Debug.isDebugBuild) debug = false;
            factionDatabase.IsPlaying = true;
        }

    public void OnDestroy()
    {
       factionDatabase.IsPlaying = false;
    }
    #endregion

    #region Faction Registration/Lookup

    /// <summary>
    /// Looks up a faction by its ID.
    /// </summary>
    /// <returns>The faction.</returns>
    /// <param name="factionID">Faction ID.</param>
    public Faction GetFaction(int factionID)
        {
            return GetFaction(factionID, false);
        }

        /// <summary>
        /// Looks up a faction by its ID. Doesn't log any warning messages.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionID">Faction ID.</param>
        public Faction GetFactionSilent(int factionID)
        {
            return GetFaction(factionID, true);
        }

        /// <summary>
        /// Looks up a faction by its ID.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="silent">If set to <c>true</c>, don't log warnings.</param>
        public Faction GetFaction(int factionID, bool silent)
        {
            Faction faction = null;
            if (m_factions.ContainsKey(factionID))
            {
                // Get from cache:
                faction = m_factions[factionID];
            }
            else
            {
                // Look up in database and update cache:
                faction = factionDatabase.GetFaction(factionID);
                if (faction != null)
                {
                    m_factions.Add(factionID, faction);
                }
            }
            if (faction == null && !silent)
            {
                Debug.LogWarning("Love/Hate: Can't find faction with ID " + factionID, this);
            }
            return faction;
        }

        /// <summary>
        /// Looks up a faction by its name.
        /// </summary>
        /// <returns>The faction.</returns>
        /// <param name="factionName">Faction name.</param>
        public Faction GetFaction(string factionName)
        {
            if (factionDatabase == null) return null;
            var faction = factionDatabase.GetFaction(factionName);
            if (faction == null)
            {
                Debug.LogWarning("Love/Hate: Can't find faction named " + factionName, this);
            }
            return faction;
        }

        /// <summary>
        /// Looks up a faction ID by its name.
        /// </summary>
        /// <returns>The faction ID.</returns>
        /// <param name="factionName">Faction name.</param>
        public int GetFactionID(string factionName)
        {
            var id = (factionDatabase == null) ? -1 : factionDatabase.GetFactionID(factionName);
            if (id == -1)
            {
                Debug.LogWarning("Love/Hate: Can't find faction named " + factionName, this);
            }
            return id;
        }

        /// <summary>
        /// Registers a faction member. The faction member calls this in Start.
        /// </summary>
        /// <param name="member">Member.</param>
        public void RegisterFactionMember(FactionMember member)
        {
            if (member == null) return;
            member.factionDatabase = factionDatabase;
            var faction = GetFaction(member.factionID);
            if (faction == null)
            {
                Debug.LogError("Love/Hate: Can't find faction for FactionMember " + member.name + " (faction ID " + member.factionID + ")", member);
                return;
            }
            if (m_members.ContainsKey(faction))
            {
                if (!m_members[faction].Contains(member))
                {
                    m_members[faction].Add(member);
                }
            }
            else
            {
                m_members.Add(faction, new List<FactionMember>() { member });
            }
        }

        /// <summary>
        /// Unregisters a faction member. The faction member calls this in OnDestroy.
        /// </summary>
        /// <param name="member">Member.</param>
        public void UnregisterFactionMember(FactionMember member)
        {
            if (member == null) return;
            var faction = GetFaction(member.factionID);
            if (faction == null) return;
            if (m_members.ContainsKey(faction) && m_members[faction].Contains(member))
            {
                m_members[faction].Remove(member);
            }
        }

        #endregion

        #region Parents

        /// <summary>
        /// Checks if a faction has an ancestor (parent, grandparent, etc.)
        /// </summary>
        /// <returns><c>true</c>, if has ancestor, <c>false</c> otherwise.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="ancestorID">Ancestor ID.</param>
        public bool FactionHasAncestor(int factionID, int ancestorID)
        {
            return (factionDatabase == null) ? false : factionDatabase.FactionHasAncestor(factionID, ancestorID);
        }

        /// <summary>
        /// Checks if a faction has a direct parent.
        /// </summary>
        /// <returns><c>true</c>, if has direct parent, <c>false</c> otherwise.</returns>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        public bool FactionHasDirectParent(int factionID, int parentID)
        {
            return (factionDatabase == null) ? false : factionDatabase.FactionHasDirectParent(factionID, parentID);
        }

        /// <summary>
        /// Adds a parent to a faction.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        public void AddFactionParent(int factionID, int parentID)
        {
            if (factionDatabase == null) return;
            factionDatabase.AddFactionParent(factionID, parentID);
        }

        /// <summary>
        /// Removes a parent from a faction.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="parentID">Parent ID.</param>
        /// <param name="inheritRelationships">If set to <c>true</c> keep the parent's 
        /// relationships if the faction doesn't already have a personal relationship.</param>
        public void RemoveFactionParent(int factionID, int parentID, bool inheritRelationships)
        {
            if (factionDatabase == null) return;
            factionDatabase.RemoveFactionParent(factionID, parentID, inheritRelationships);
        }

        #endregion

        #region Relationships

        /// <summary>
        /// Finds this faction's personal affinity for another faction. Doesn't check parents.
        /// </summary>
        /// <returns>`true` if personal affinity was found, `false` otherwise.</returns>
        /// <param name="subjectFactionID">The other faction ID.</param>
        /// <param name="affinity">Affinity of this faction to the subject.</param>
        public bool FindPersonalAffinity(int judgeFactionID, int subjectFactionID, out float affinity)
        {
            if (factionDatabase == null) return AffinityNotFound(out affinity);
            return factionDatabase.FindPersonalAffinity(judgeFactionID, subjectFactionID, out affinity);
        }

        /// <summary>
        /// Finds the affinity of a judging faction to a subject faction.
        /// </summary>
        /// <returns>`true`, if affinity was found, `false` otherwise.</returns>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinity">Affinity of judge to subject.</param>
        public bool FindAffinity(int judgeFactionID, int subjectFactionID, out float affinity)
        {
            if (factionDatabase == null) return AffinityNotFound(out affinity);
            return factionDatabase.FindAffinity(judgeFactionID, subjectFactionID, out affinity);
        }

        private bool AffinityNotFound(out float affinity)
        {
            affinity = 0;
            return false;
        }

        /// <summary>
        /// Gets the affinity of a judging faction to a subject faction.
        /// </summary>
        /// <returns>The affinity.</returns>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        public float GetAffinity(int judgeFactionID, int subjectFactionID)
        {
            return (factionDatabase == null) ? 0 : factionDatabase.GetAffinity(judgeFactionID, subjectFactionID);
        }

        /// <summary>
        /// Gets a judge's affinity to a subject using their faction names.
        /// </summary>
        /// <returns>The affinity.</returns>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        public float GetAffinity(string judgeFactionName, string subjectFactionName)
        {
            return (factionDatabase == null) ? 0 : factionDatabase.GetAffinity(judgeFactionName, subjectFactionName);
        }

        /// <summary>
        /// Sets this faction's personal affinity to another faction.
        /// </summary>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinity">Affinity to the other faction.</param>
        public void SetPersonalAffinity(int judgeFactionID, int subjectFactionID, float affinity)
        {
            if (factionDatabase == null) return;
            factionDatabase.SetPersonalAffinity(judgeFactionID, subjectFactionID, affinity);
        }

        /// <summary>
        /// Sets a judge's personal affinity to a subject using their faction names.
        /// </summary>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        /// <param name="affinity">Affinity.</param>
        public void SetPersonalAffinity(string judgeFactionName, string subjectFactionName, float affinity)
        {
            if (factionDatabase == null) return;
            factionDatabase.SetPersonalAffinity(judgeFactionName, subjectFactionName, affinity);
        }

        /// <summary>
        /// Modifies (increments or decrements) a judge's personal affinity to a subject 
        /// using their faction IDs.
        /// </summary>
        /// <param name="judgeFactionID">Judge faction ID.</param>
        /// <param name="subjectFactionID">Subject faction ID.</param>
        /// <param name="affinityChange">Affinity change.</param>
        public void ModifyPersonalAffinity(int judgeFactionID, int subjectFactionID, float affinityChange)
        {
            if (factionDatabase == null) return;
            factionDatabase.ModifyPersonalAffinity(judgeFactionID, subjectFactionID, affinityChange);
        }

        /// <summary>
        /// Modifies (increments or decrements) a judge's personal affinity to a subject 
        /// using their faction names.
        /// </summary>
        /// <param name="judgeFactionName">Judge faction name.</param>
        /// <param name="subjectFactionName">Subject faction name.</param>
        /// <param name="affinityChange">Affinity change.</param>
        public void ModifyPersonalAffinity(string judgeFactionName, string subjectFactionName, float affinityChange)
        {
            if (factionDatabase == null) return;
            factionDatabase.ModifyPersonalAffinity(judgeFactionName, subjectFactionName, affinityChange);
        }

        /// <summary>
        /// If the faction has personal affinity for a subject and the other doesn't, this
        /// gives the other an affinity for the subject based on the other's affinity
        /// for this faction.
        /// </summary>
        /// <param name="judgeFactionID">Judging faction ID (gives affinity).</param>
        /// <param name="otherFactionID">Other faction ID (maybe receives affinity).</param>
        /// <param name="subjectFactionID">Subject faction ID (affinity about this faction).</param>
        public void ShareAffinity(int judgeFactionID, int otherFactionID, int subjectFactionID)
        {
            if (factionDatabase == null) return;
            factionDatabase.ShareAffinity(judgeFactionID, otherFactionID, subjectFactionID);
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
        public void CommitDeed(FactionMember actor, Deed deed, bool requiresSight, Dimension dimension, float radius)
        {
            if (actor == null)
            {
                Debug.LogError("Love/Hate: CommitDeed actor is null", this);
                return;
            }
            if (deed == null)
            {
                Debug.LogError("Love/Hate: CommitDeed deed is null", this);
                return;
            }
            if (debug)
            {
                Debug.Log("Love/Hate: CommitDeed(" + deed.tag + ") actor:" + actor.name + " target:" + GetFaction(deed.targetFactionID).name + " impact:" + deed.impact, this);
            }

            var enumerator = m_members.GetEnumerator(); // Enumerates manually to avoid garbage.
            while (enumerator.MoveNext())
            {
                var members = enumerator.Current.Value;
                for (int i = 0; i < members.Count; i++)
                {
                    var member = members[i];
                    if (IsFactionMemberAwake(member, actor) && IsWitnessInRange(member, actor, dimension, radius))
                    {
                        //---Now queued to distribute across frames: member.WitnessDeed(deed, actor, requiresSight, dimension);
                        m_witnessQueue.Enqueue(WitnessQueueItem.GetNew(deed, member, actor, requiresSight, dimension));
                    }
                }
            }
        }

        /// <summary>
        /// Commits the deed. This version has no distance restrictions.
        /// </summary>
        /// <param name="actor">Actor.</param>
        /// <param name="deed">Deed.</param>
        /// <param name="requiresSight">If set to <c>true</c> requires sight.</param>
        public void CommitDeed(FactionMember actor, Deed deed, bool requiresSight)
        {
            CommitDeed(actor, deed, requiresSight, Dimension.Is3D, 0);
        }

        private bool IsFactionMemberAwake(FactionMember member, FactionMember actor)
        {
            return (member != null) &&
                    (canWitnessSelf || (member != actor)) &&
                    member.gameObject.activeInHierarchy &&
                    member.enabled;
        }

        private bool IsWitnessInRange(FactionMember witness, FactionMember actor, Dimension dimension, float radius)
        {
            if (Mathf.Approximately(0, radius)) return true;
            switch (dimension)
            {
                default:
                case Dimension.Is3D:
                    var distance3D = Vector3.Distance(witness.transform.position, actor.transform.position);
                    return distance3D <= radius;
                case Dimension.Is2D:
                    var distance2D = Vector2.Distance(new Vector2(witness.transform.position.x, witness.transform.position.y),
                                                      new Vector2(actor.transform.position.x, actor.transform.position.y));
                    return distance2D <= radius;
            }
        }

        /// <summary>
        /// The Update method processes the queue of deeds waiting to be witnessed.
        /// </summary>
        private void Update()
        {
            if (m_witnessQueue.Count == 0) return;
            var count = Mathf.Min(witnessesPerUpdate, m_witnessQueue.Count);
            for (int i = 0; i < count; i++)
            {
                var item = m_witnessQueue.Dequeue();
                if (item == null) return;
                if (item.witness == null) continue;
                item.witness.WitnessDeed(item.deed, item.actor, item.requiresSight, item.dimension);
                WitnessQueueItem.Release(item);
            }
        }

        #endregion

        #region Serialization

        public string SerializeToString()
        {
            if (factionDatabase == null) return string.Empty;

            var sb = new StringBuilder();

            // Record faction count:
            sb.AppendFormat("{0},", factionDatabase.factions.Length);

            for (int f = 0; f < factionDatabase.factions.Length; f++)
            {
                var faction = factionDatabase.factions[f];

                // Record faction ID and name:
                sb.AppendFormat("{0},{1},", faction.id, SafeConvert.ToSerializedElement(faction.name));

                // Record faction personality trait values:
                for (int p = 0; p < faction.traits.Length; p++)
                {
                    sb.AppendFormat("{0},", faction.traits[p]);
                }

                // Record faction parents:
                sb.AppendFormat("{0},", faction.parents.Length);
                for (int p = 0; p < faction.parents.Length; p++)
                {
                    var parentID = faction.parents[p];
                    sb.AppendFormat("{0},", parentID);
                }

                // Record faction relationships:
                sb.AppendFormat("{0},", faction.relationships.Count);
                for (int r = 0; r < faction.relationships.Count; r++)
                {
                    var relationship = faction.relationships[r];
                    // Record relationship faction ID:
                    sb.AppendFormat("{0},{1},", relationship.factionID, (relationship.inheritable ? 1 : 0));
                    // Record relationship trait values:
                    for (int t = 0; t < factionDatabase.relationshipTraitDefinitions.Length; t++)
                    {
                        sb.AppendFormat("{0},", relationship.traits[t]);
                    }
                }
            }
            return sb.ToString();
        }

        public void DeserializeFromString(string s)
        {
            if (string.IsNullOrEmpty(s) || factionDatabase == null) return;

            var traitCount = factionDatabase.relationshipTraitDefinitions.Length;
            var data = new Queue<string>(s.Split(','));
            if (data.Count < 1) return;

            // Get faction count:
            var factionCount = SafeConvert.ToInt(data.Dequeue());

            for (int f = 0; f < Mathf.Min(factionCount, factionDatabase.factions.Length); f++)
            {
                var faction = factionDatabase.factions[f];

                // Get faction ID and name:
                faction.id = SafeConvert.ToInt(data.Dequeue());
                faction.name = SafeConvert.FromSerializedElement(data.Dequeue());

                // Get faction personality trait values:
                for (int p = 0; p < faction.traits.Length; p++)
                {
                    faction.traits[p] = SafeConvert.ToFloat(data.Dequeue());
                }

                // Get faction parents:
                var parents = new List<int>();
                var parentCount = SafeConvert.ToInt(data.Dequeue());
                for (int p = 0; p < parentCount; p++)
                {
                    parents.Add(SafeConvert.ToInt(data.Dequeue()));
                }
                faction.parents = parents.ToArray();

                // Release faction's old relationships:
                for (int r = 0; r < faction.relationships.Count; r++)
                {
                    var relationship = faction.relationships[r];
                    Relationship.Release(relationship);
                }
                faction.relationships.Clear();

                // Get faction's relationships:
                var relationshipCount = SafeConvert.ToInt(data.Dequeue());
                for (int r = 0; r < relationshipCount; r++)
                {
                    var id = SafeConvert.ToInt(data.Dequeue());
                    var inheritable = (SafeConvert.ToInt(data.Dequeue()) == 1);
                    var traits = new float[traitCount];
                    for (int i = 0; i < traitCount; i++)
                    {
                        traits[i] = SafeConvert.ToFloat(data.Dequeue());
                    }
                    var relationship = Relationship.GetNew(id, inheritable, traits);
                    faction.relationships.Add(relationship);
                }
            }
        }

        #endregion

        #region Zone Controller

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            controllable.Add("witnessesPerUpdate|System.Int32|1|99999|1|When a deed is committed, process this many potential witnesses each Update.");
            controllable.Add("debug|System.Boolean|0|1|1|Log activity to the console.");
            return controllable;
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The methods that Zone Controller can invoke.</returns>
        public static List<string> ZonePluginActivatorMethods()
        {
            List<string> invokable = new List<string>();
            invokable.Add("SetPersonalAffinity(System.Int32 judgeFactionID, System.Int32 subjectFactionID, System.Single affinity");
            invokable.Add("ModifyPersonalAffinity(System.Int32 judgeFactionID, System.Int32 subjectFactionID, System.Single affinityChange");
            return invokable;

        }
        #endregion

        #region Evaluation Version Code

#if EVALUATION_VERSION

        private GameObject watermark = null;

        protected virtual void LateUpdate()
        {
            if (watermark != null) return;
            watermark = new GameObject(System.Guid.NewGuid().ToString());
            watermark.transform.SetParent(transform);
            watermark.hideFlags = HideFlags.HideInHierarchy;
            var canvas = watermark.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 16383;
            Destroy(watermark.GetComponent<UnityEngine.UI.GraphicRaycaster>());
            Destroy(watermark.GetComponent<UnityEngine.UI.CanvasScaler>());
            var text = watermark.AddComponent<UnityEngine.UI.Text>();
            text.text = "Love/Hate\nEvaluation Version";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 24;
            text.fontStyle = FontStyle.Bold;
            text.color = new Color(1, 1, 1, 0.75f);
            text.alignment = (Random.value < 0.5f) ? TextAnchor.UpperCenter : TextAnchor.LowerCenter;
            text.raycastTarget = false;
        }

#endif

        #endregion
    }

}
