using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Health : IComponentData
{
    [GhostDefaultField(quantizationFactor:100)]
    public float Value;
}
