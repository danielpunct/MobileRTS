using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GenerateAuthoringComponent]
public struct UnitSelectionState : IComponentData
{
    [GhostDefaultField]
    public bool IsSelected;
}
