using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItemSystem.projectiles
{
    public interface iBase {
        HumanBodyBones StartPos { get; }

    }
    public interface Forged
    {
        int purchaseQty { get; }
    }

    public interface Manifested
    {
        int manaCost { get; }
    }
    public interface Lob {
        float Force { get; }
        Vector3 Angle { get; }
    }

    public interface Straight {
        float force { get; }

    }

    public interface Effects
    {
        float Delay { get; }

    }

}