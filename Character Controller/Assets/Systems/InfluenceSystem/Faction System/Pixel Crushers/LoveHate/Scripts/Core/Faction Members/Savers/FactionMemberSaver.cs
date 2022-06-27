// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Saves a FactionMember's data to the SaveSystem.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(FactionMember))]
    public class FactionMemberSaver : Saver
    {

        private FactionMember m_member = null;

        public override void Awake()
        {
            base.Awake();
            m_member = GetComponent<FactionMember>();
        }

        public override string RecordData()
        {
            return m_member.SerializeToString();
        }

        public override void ApplyData(string s)
        {
            m_member.DeserializeFromString(s);
        }

    }
}
