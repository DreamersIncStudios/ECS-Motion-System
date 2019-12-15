using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public class ConjuredWeapon : Weapon, IConjuredWeapon
    {
        [SerializeField] Material _shader;
        [SerializeField] float _revealRate;
        [SerializeField] float _dissolveDelay;

        public Material Shader { get { return _shader; } }
        public float RevealRate { get { return _revealRate; } }
        public float DissolveDelay { get { return _dissolveDelay; } }

    }
}
