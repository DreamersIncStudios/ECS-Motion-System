using System;
using System.Collections;
using System.Collections.Generic;
using DreamersInc.ServiceLocatorSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class SpawnService : MonoBehaviour
{
    [SerializeField]public SpawnType SpawnType;
    [Title("Registered Services")] 
   [SerializeField] private List<Object> services;
    private void Awake()
    {
        ServiceLocator sl = ServiceLocator.For(this);
        foreach (var service in services)
        {
            sl.Register(service.GetType(), service);
        }
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

public enum SpawnType
{
    Player,NPC,Enemy, Item, Chest
}
