using System;

namespace UnityEngine.Rendering.HighDefinition
{
    [VolumeComponentMenu("Sky/Procedural Sky")]
    [SkyUniqueID((int)SkyType.Procedural)]
    public class ProceduralSky : SkySettings
    {
        [Tooltip("Sets the size modifier of the sun disk.")]
        public ClampedFloatParameter sunSize = new ClampedFloatParameter(0.04f, 0.0f, 1.0f);
        [Tooltip("Sets the size convergence of the sun, smaller values make the sun appear larger.")]
        public ClampedFloatParameter sunSizeConvergence = new ClampedFloatParameter(5.0f, 1.0f, 10.0f);
        [Tooltip("Sets the density of the atmosphere.")]
        public ClampedFloatParameter atmosphereThickness = new ClampedFloatParameter(1.0f, 0.0f, 5.0f);
        [Tooltip("Sets the color of the sky.")]
        public ColorParameter skyTint = new ColorParameter(new Color(0.5f, 0.5f, 0.5f, 1.0f));
        [Tooltip("Sets the color of the ground, the area below the horizon.")]
        public ColorParameter groundColor = new ColorParameter(new Color(0.369f, 0.349f, 0.341f, 1.0f));
        [Tooltip("When enabled, HDRP displays the sun disk.")]
        public BoolParameter enableSunDisk = new BoolParameter(true);

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();

            unchecked
            {
                hash = hash * 23 + sunSize.GetHashCode();
                hash = hash * 23 + sunSizeConvergence.GetHashCode();
                hash = hash * 23 + atmosphereThickness.GetHashCode();
                hash = hash * 23 + skyTint.GetHashCode();
                hash = hash * 23 + groundColor.GetHashCode();
                hash = hash * 23 + multiplier.GetHashCode();
                hash = hash * 23 + enableSunDisk.GetHashCode();
            }

            return hash;
        }

        public override System.Type GetSkyRendererType() { return typeof(ProceduralSkyRenderer); }
    }
}
