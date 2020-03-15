using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Transforms;

public struct QuadGhostSerializer : IGhostSerializer<QuadSnapshotData>
{
    private ComponentType componentTypeUnitSelectionVisual;
    private ComponentType componentTypePerInstanceCullingTag;
    private ComponentType componentTypeRenderBounds;
    private ComponentType componentTypeRenderMesh;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeNonUniformScale;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<QuadSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeUnitSelectionVisual = ComponentType.ReadWrite<UnitSelectionVisual>();
        componentTypePerInstanceCullingTag = ComponentType.ReadWrite<PerInstanceCullingTag>();
        componentTypeRenderBounds = ComponentType.ReadWrite<RenderBounds>();
        componentTypeRenderMesh = ComponentType.ReadWrite<RenderMesh>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeNonUniformScale = ComponentType.ReadWrite<NonUniformScale>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref QuadSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
