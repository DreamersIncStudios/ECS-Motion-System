using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VisualEffect
{
    public class DissolveSingle : MonoBehaviour
    {
        public Animator Anim;
        TrailRenderer WeaponTrail;

        public Material DissolveInstance;
        private void Awake()
        {
            DissolveInstance = this.GetComponent<Renderer>().material;
            WeaponTrail = this.GetComponentInChildren<TrailRenderer>();

        }
        private void Start()
        {
            Anim = GetComponentInParent<Animator>();

        }
        // Update is called once per frame
        void Update()
        {
            DissolveInstance.SetFloat("Dissolve", Anim.GetFloat("Dissolve"));
            WeaponTrail.emitting = Anim.GetFloat("Emit Trail") == 1;
        }
    }
}