using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ParticleDamage : MonoBehaviour
{
    int damage;
    [SerializeField] float moveDistance;
    [SerializeField] float growScale=1;
    [SerializeField] float moveTime;
    [SerializeField] bool delay;
    [SerializeField] float delayTime;
    public Element element;
    private void Start()
    {
        startDamage = false;
        if (delay)
            Invoke(nameof(MoveDamageCollider), delayTime);
        else
            MoveDamageCollider();
    }

    private void MoveDamageCollider()
    {
        startDamage = true;
        transform.DOMove(transform.position + transform.root.forward * moveDistance, moveTime, false);
        transform.DOScaleX(growScale, moveTime);
    }

    public void SetDamage(int damage) { 
    
    }

    bool startDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (startDamage)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null && other.tag != "Player")
            {
                damageable.TakeDamage(1000, TypeOfDamage.MagicAoE, element);
                Debug.Log($"hit {other.name}");
            }
        }
    }
}