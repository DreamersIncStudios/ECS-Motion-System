using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Transforms;
using Unity.Physics;

public class StaticObjectController : MonoBehaviour
{
    List<WeaponController> attachedWeapons;
    public void SetupControllerEntityData(Entity Data){
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (GetAttachedWeapons()) {
            DynamicBuffer<Child> children = em.AddBuffer<Child>(Data);
                foreach (var weapon in attachedWeapons) {
                    weapon.SpawnEntityData(Data,attachedWeapons.IndexOf(weapon));
                } 
        }

 
    }
    public bool GetAttachedWeapons() { 
    attachedWeapons = new List<WeaponController>();
        WeaponController[] weaponControllers = GetComponentsInChildren<WeaponController>();
        foreach (var item in weaponControllers)
        {
            attachedWeapons.Add(item);
        }
        return attachedWeapons.Count > 0;
    }
}
