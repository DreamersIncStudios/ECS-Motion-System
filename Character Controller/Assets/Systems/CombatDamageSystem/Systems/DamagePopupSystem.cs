using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;
using Unity.Transforms;
using Unity.Mathematics;
using DG.Tweening;
using TMPro;

namespace DreamerInc.CombatSystem

{
   // [UpdateBefore(typeof(AdjustVitalsSystem))]
    public partial class DamagePopupSystem : SystemBase //TODO rewrite
    {
        protected override void OnUpdate()
        {
            foreach (var UI in SystemAPI.Query<UIPrefab_Entities>()) //TODO: UIPrefab_Entities should be a single entity
            {
                var temp = UI;
                Entities.WithoutBurst().ForEach((Transform transform, ref AdjustHealth mod) =>
                {
                    var spawnedUI = GameObject.Instantiate(temp.uiPrefab, transform);
                    spawnedUI.GetComponent<TextMeshPro>().text = Mathf.Abs(mod.Value).ToString();
                    spawnedUI.transform.localPosition += Vector3.up;
                    spawnedUI.transform.DOShakePosition(3.5f, .25f, 5);
                    Object.Destroy(spawnedUI.gameObject, 3.6f);
                }).Run();
            }

            Entities.WithoutBurst().ForEach((TextMeshPro textMesh, Transform transform) => {
                transform.rotation = Camera.main.transform.rotation;
            }).Run();
        }
    }
}