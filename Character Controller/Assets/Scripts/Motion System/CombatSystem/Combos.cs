using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

[CreateAssetMenu(fileName = "Combo", menuName ="ComboSystem")]
public class Combos : ScriptableObject, ICombos
{
    public List<AnimationCombo> _comboList;
    public List<AnimationCombo> ComboList { get { return _comboList; } }

    public void Setup(Entity entity)
    {

    }
    public void UnlockCombo(ComboNames Name) { 
    
    }
}
