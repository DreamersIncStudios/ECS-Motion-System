using System;
using System.Collections;
using System.Collections.Generic;
using DreamersInc.AnimationEventSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;


namespace DreamersInc.VisualEffectSystem
{
    public class DissolveSingle : MonoBehaviour
    {
        [SerializeField] Animator Anim;
        [SerializeField] VisualEffect Effect;
        //TODO Event System needed
            
        public Material DissolveInstance;
        private void Awake()
        {
            DissolveInstance = this.GetComponent<Renderer>().material;
            Effect = GetComponentInChildren<VisualEffect>();
        }
        private void Start()
        {
            Anim = GetComponentInParent<Animator>();
            GetComponentInParent<AnimationEventReceiver>()
                .AssignEvent("OnDissolve", () => Effect.Play());

        }

        private void OnDestroy()
        {
            GetComponentInParent<AnimationEventReceiver>()
                .RemoveEvent("OnDissolve", () => Effect.Play());
        }

        float currentValue => DissolveInstance.GetFloat("Dissolve");
        float animValue => Anim.GetFloat("Dissolve");
        bool change => animValue != currentValue;
        // Update is called once per frame
        void Update()
        {
            if(change)
            DissolveInstance.SetFloat("Dissolve", animValue);
        }
    }
}