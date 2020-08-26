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
    Animator Anim;
    PlayerCharacter PC;
    StandardScreenUI UIManager;
    public bool WeaponVisible { get {
            if (DissolveInstance)
                DissolveInstance = this.GetComponentInChildren<Renderer>().material;
            return DissolveInstance.GetFloat("Dissolve") == 0.0f;
        }
    }
    public bool CanDamage { get { if (Anim)
                Anim = this.GetComponentInParent<Animator>();
            return Anim.GetFloat("DamageWindow") == 1.2f;
        } }
    private void Awake()
    {
        if(!DissolveInstance)
            DissolveInstance = this.GetComponentInChildren<Renderer>().material;
        if(!Anim)
        Anim = this.GetComponentInParent<Animator>();
        if (!PC)
            PC = this.GetComponentInParent<PlayerCharacter>();
        UIManager = StandardScreenUI.Manager;
    }
    private void OnTriggerEnter(Collider Hit)
    {
        EnemyCharacter EC = Hit.gameObject.GetComponent<EnemyCharacter>();
        if (!EC)
            return;
        DamageStep(EC);

        CheckECStatus(EC);
    }

    void DamageStep( EnemyCharacter EC)
    {
        if (EC.Alive && CanDamage)
        {
            EC.AdjustHealth(-(int)(EC.MeleeDef * PC.MeleeAttack));
#if UNITY_EDITOR
            Debug.Log("Hit the enemy for " + (int)(EC.MeleeDef * PC.MeleeAttack) + " HP pts ", this);
#endif  
            UIManager.HitTimer = UIManager.HitTimerReset;
            UIManager.HitCount++;
        }

    }
    void CheckECStatus(EnemyCharacter EC) {
        if (EC.Alive)
            return;
        EC.OnDeath(10);
        PC.FreeExp += EC.EXPgained;

    }
}
