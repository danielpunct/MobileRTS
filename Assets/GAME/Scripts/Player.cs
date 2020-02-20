using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Player : IComponentData
{
    [GhostDefaultField]
    public int PlayerId;
}
