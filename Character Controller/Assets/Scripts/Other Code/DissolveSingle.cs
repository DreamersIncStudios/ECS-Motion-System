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
            Anim = this.GetComponentInParent<Animator>();
            WeaponTrail = this.GetComponentInChildren<TrailRenderer>();

        }

        // Update is called once per frame
        void Update()
        {
            DissolveInstance.SetFloat("Dissolve", Anim.GetFloat("Dissolve"));
            WeaponTrail.emitting = Anim.GetFloat("Emit Trail") == 1 ? true : false;
        }
    }
}