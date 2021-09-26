using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardScreenUI : MonoBehaviour
{
   public static StandardScreenUI Manager;
    [SerializeField]public int HitCount;
    [HideInInspector]public float HitTimerReset= 6.0f;
    [HideInInspector]public float HitTimer;

    private void Awake()
    {
        if (!Manager) { Manager = this; }
        else { Destroy(this.gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HitCounter();
    }
    void HitCounter() {
        if (HitCount == 0)
            return;

        if (HitTimer > 0.0f)
        {
            HitTimer -= Time.deltaTime;
        }
        else { HitCount = 0; }

    }
}
