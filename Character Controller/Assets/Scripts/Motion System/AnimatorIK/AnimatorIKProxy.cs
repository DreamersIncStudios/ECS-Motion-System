using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorIKProxy : MonoBehaviour
{
   public  static AnimatorIKProxy Proxy;
    private void Awake()
    {
        if (Proxy == null)
            Proxy = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
