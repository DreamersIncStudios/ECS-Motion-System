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
    [UpdateBefore(typeof(AdjustVitalsSystem))]
    public partial class DamagePopupSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((UIPrefab_Entities UI) =>
            {
                Entities.ForEach(  (ref AdjustHealth mod, Transform transform) =>
                {
                    var spawnedUI = GameObject.Instantiate(UI.uiPrefab, transform);
                    spawnedUI.GetComponent<TextMeshPro>().text = Mathf.Abs(mod.Value).ToString(); 
                    spawnedUI.transform.localPosition += Vector3.up;
                    spawnedUI.transform.DOShakePosition(3.5f,.25f,5);
                    Object.Destroy(spawnedUI.gameObject, 3.5f);
                    transform.DOShakeScale(1, .25f, 2);
                });
            });

            Entities.ForEach((TextMeshPro textMesh, Transform transform) => {
                transform.rotation = Camera.main.transform.rotation;
            });
        }
    }
}