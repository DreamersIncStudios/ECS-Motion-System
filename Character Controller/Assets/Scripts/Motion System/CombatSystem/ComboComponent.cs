using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class ComboComponent : IComponentData
{
    public Combos combo;
    public Animator animator;
}