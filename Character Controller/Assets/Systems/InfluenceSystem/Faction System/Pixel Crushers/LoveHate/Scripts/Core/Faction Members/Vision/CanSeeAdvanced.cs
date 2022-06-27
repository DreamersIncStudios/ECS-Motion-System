// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Add to a GameObject that has a FactionMember. Provides a more advanced 
    /// implementation for FactionMember.CanSee that offers multiple fields of 
    /// view and visual debug inforomation in the Scene view.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [RequireComponent(typeof(FactionMember))]
    public class CanSeeAdvanced : MonoBehaviour
    {

        /// <summary>
        /// The sensor's fields of view (FOVs).
        /// </summary>
        [Tooltip("The sensor's fields of view (FOVs).")]
        public FieldOfView[] fieldsOfView = new FieldOfView[0];

        /// <summary>
        /// Extra heights at which to run raycast checks in addition to the target's
        /// local origin. When trying to see a humanoid, you might want to run raycasts to
        /// its feet, abdomen, and head. The feet and abdomen might be blocked by a low wall, but
        /// the head could be visible above it.
        /// </summary>
        [Tooltip("Extra heights at which to run raycast checks in addition to the target's local origin. When trying to see a humanoid, you might want to run raycasts to its feet, abdomen, and head. The feet and abdomen might be blocked by a low wall, but the head could be visible above it.")]
        public float[] extraRaycastHeights = new float[0];

        public static readonly FieldOfView CentralFOV = new FieldOfView(45f, 120f, 20f, new Color(1, 1, 1, 0.3f));
        public static readonly FieldOfView WideFOV = new FieldOfView(120f, 120f, 15f, new Color(1, 1, 1, 0.2f));
        public static readonly FieldOfView PeripheralFOV = new FieldOfView(180f, 120f, 5f, new Color(1, 1, 1, 0.1f));

        [HideInInspector]
        public FactionMember factionMember;
        [HideInInspector]
        public Transform raySource;
        [HideInInspector]
        public Dimension dimension = Dimension.Is3D;
        [HideInInspector]
        public bool hasCheckedEver = false;
        [HideInInspector]
        public Vector3 lastPointChecked;
        [HideInInspector]
        public string lastPointGizmoName = string.Empty;

        protected virtual void Reset()
        {
            fieldsOfView = new FieldOfView[3]
             {
                new FieldOfView(CentralFOV),
                new FieldOfView(WideFOV),
                new FieldOfView(PeripheralFOV)
             };
        }

        protected virtual void Awake()
        {
            factionMember = GetComponent<FactionMember>();
        }

        protected virtual void Start()
        {
            if (factionMember == null) return;
            factionMember.CanSee = CanSee;
            raySource = (factionMember.eyes != null) ? factionMember.eyes.transform : factionMember.transform;
        }

        public virtual bool CanSee(FactionMember actor, Dimension dimension)
        {
            if (actor == null || factionMember == null) return false;
            this.dimension = dimension;
            hasCheckedEver = true;
            lastPointChecked = actor.transform.position;
            lastPointGizmoName = string.Empty;
            var seen = IsInFOVs(actor.transform, dimension) && IsInLineOfSight(actor.transform, dimension);
            if (seen && factionMember.factionDatabase != null)
            {
                var faction = factionMember.factionDatabase.GetFaction(actor.factionID);
                if (faction != null && faction.color < Faction.GizmoIconNames.Length)
                {
                    lastPointGizmoName = Faction.GizmoIconNames[faction.color];
                }
            }
            return seen;
        }

        protected bool IsInFOVs(Transform target, Dimension dimension)
        {
            if (target == null || factionMember == null) return false;
            for (int i = 0; i < fieldsOfView.Length; i++)
            {
                var fov = fieldsOfView[i];
                if (fov != null && fov.Contains(raySource, target, dimension)) return true;
            }
            return false;
        }

        protected bool IsInLineOfSight(Transform target, Dimension dimension)
        {
            if (target == null || factionMember == null) return false;
            if (RaycastHitTarget(target, raySource.position, target.position, dimension)) return true;
            for (int i = 0; i < extraRaycastHeights.Length; i++)
            {
                if (RaycastHitTarget(target, raySource.position, target.position + Vector3.up * extraRaycastHeights[i], dimension)) return true;
            }
            return false;
        }

        protected bool RaycastHitTarget(Transform target, Vector3 rayStart, Vector3 rayEnd, Dimension dimension)
        {
            var layerMask = factionMember.sightLayerMask;
            if (dimension == Dimension.Is2D)
            {
#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER
                RaycastHit2D[] hits = Physics2D.LinecastAll(rayStart, rayEnd, layerMask);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform == transform) continue;
                    return (hits[i].transform == target);
                }
#endif
                return false;
            }
            else
            {
                RaycastHit hitInfo;
                bool hit = Physics.Linecast(rayStart, rayEnd, out hitInfo, layerMask);
                return (hit && hitInfo.transform == target);
            }
        }


    }
}
