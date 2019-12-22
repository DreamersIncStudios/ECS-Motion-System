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
     
    }
    // Start is called before the first frame update
    void Start()
    {
        Anim = this.GetComponentInParent<Animator>();
        Anim.SetFloat("Dissolve", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        DissolveInstance.SetFloat("Dissolve", Anim.GetFloat("Dissolve"));
    }
}
