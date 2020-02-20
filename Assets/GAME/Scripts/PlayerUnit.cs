using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct PlayerUnit : IComponentData
{
    [GhostDefaultField]
    public int PlayerId;
}
