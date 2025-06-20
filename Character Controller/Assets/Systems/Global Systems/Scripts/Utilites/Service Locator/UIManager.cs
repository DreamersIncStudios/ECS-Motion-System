using System;
using System.Collections;
using System.Collections.Generic;
using DreamersInc.ServiceLocatorSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DreamersInc.Utils
{


    public class UIManager : MonoBehaviour
    {
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
    }
}
