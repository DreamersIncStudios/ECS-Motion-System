using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct DestroyAfterSeconds : IComponentData
{
    public float Delay;
}
