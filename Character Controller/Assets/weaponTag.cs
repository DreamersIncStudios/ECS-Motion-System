using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
public class weaponTag : MonoBehaviour
{
 
}

public class WeaponDamage : MonoBehaviour {
    // This needs to be redesign to work with all equipped weapons

    public Material DissolveInstance;
    public bool WeaponVisible { get {
            if (DissolveInstance)
                DissolveInstance = this.GetComponentInChildren<Renderer>().material;
            return DissolveInstance.GetFloat("Dissolve") == 0.0f;
        }
    }
    private void Awake()
    {
            DissolveInstance = this.GetComponentInChildren<Renderer>().material;

    }
    private void OnTriggerEnter(Collider Hit)
    {
        EnemyCharacter EC = Hit.gameObject.GetComponent<EnemyCharacter>();
        if (EC && WeaponVisible) {
            Debug.Log("Hit the enemy", this);
        }
    }
}
