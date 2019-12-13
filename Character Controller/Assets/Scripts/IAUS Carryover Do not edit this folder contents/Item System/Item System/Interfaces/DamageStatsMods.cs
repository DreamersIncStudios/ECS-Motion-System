using UnityEngine;

namespace ItemSystem
{
    public interface DamageStatsMods
    {
        int[] ElementalMod { get; set; }
        int[] AttributeMod { get; set; }

        //    public int ElementalModifier { get { return _ElementalMod; } set { _ElementalMod = value; } }
        //  public int AttributeModifier { get { return _AttributeMod; } set { _AttributeMod = value; } }
    }

    // This interface will hold the variable for raycast melee hits 
    // Start Date 2/4/2019 by MB
    //
    public interface iHitDetection {
        bool debugRays { get; }
        int numOfRays { get; } // This will be the number of raycast along the weapon 
        Vector3 IntialRay { get; } // Location of the first ray 
        Vector3 Spacing { get; } // Intervals of the
        Vector3 dirofRay { get; }
        float lengthOfRay { get; } // long is a ray 
        LayerMask Target { get; set; }
    }

}