using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveSingle : MonoBehaviour
{
    public Animator Anim;
    public Material DissolveInstance;
    private void Awake()
    {
        DissolveInstance = this.GetComponent<Renderer>().material;
        Anim = this.GetComponentInParent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        DissolveInstance.SetFloat("Dissolve", Anim.GetFloat("Dissolve"));
    }
}
