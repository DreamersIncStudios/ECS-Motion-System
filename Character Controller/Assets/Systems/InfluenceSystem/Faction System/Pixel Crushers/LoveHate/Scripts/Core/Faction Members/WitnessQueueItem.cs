// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Witness queue items are deeds waiting to be evaluated by witnesses. 
    /// The faction manager uses a queue to distribute evaluation across frames.
    /// </summary>
    [Serializable]
    public class WitnessQueueItem
    {

        /// <summary>
        /// A static pool of objects, to prevent garbage collection stutter.
        /// </summary>
        public static Pool<WitnessQueueItem> pool = new Pool<WitnessQueueItem>();

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            pool = new Pool<WitnessQueueItem>();
        }
#endif

        public Deed deed;

        public FactionMember witness;

        public FactionMember actor;

        public bool requiresSight;

        public Dimension dimension;

        public static WitnessQueueItem GetNew(Deed deed, FactionMember witness, FactionMember actor, bool requiresSight, Dimension dimension)
        {
            var item = pool.Get();
            if (deed != null) deed.refCount++;
            item.deed = deed;
            item.witness = witness;
            item.actor = actor;
            item.requiresSight = requiresSight;
            item.dimension = dimension;
            return item;
        }

        public static void Release(WitnessQueueItem item)
        {
            if (item == null) return;
            Deed.Release(item.deed);
            pool.Release(item);
        }

    }

}
