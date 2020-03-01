using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

public struct PlayerGhostSerializer : IGhostSerializer<PlayerSnapshotData>
{
    private ComponentType componentTypePlayer;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Player> ghostPlayerType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<PlayerSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayer = ComponentType.ReadWrite<Player>();
        ghostPlayerType = system.GetArchetypeChunkComponentType<Player>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref PlayerSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayer = chunk.GetNativeArray(ghostPlayerType);
        snapshot.SetPlayerPlayerId(chunkDataPlayer[ent].PlayerId, serializerState);
    }
}
