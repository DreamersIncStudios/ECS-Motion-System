using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VisualEffect
{
<<<<<<< HEAD
    public class DissolveSingle : MonoBehaviour
    {
        public Animator Anim;
        public Material DissolveInstance;
        private void Awake()
        {
            DissolveInstance = this.GetComponent<Renderer>().material;
            Anim = this.GetComponentInParent<Animator>();

=======
    public Animator Anim;
    public Material DissolveInstance;
    private void Awake()
    {
        DissolveInstance = this.GetComponent<Renderer>().material;
        Anim = this.GetComponentInParent<Animator>();


    }
>>>>>>> 8ff764df9e264f29935341465e14496604252a57

        }

        // Update is called once per frame
        void Update()
        {
            DissolveInstance.SetFloat("Dissolve", Anim.GetFloat("Dissolve"));
        }
    }
}