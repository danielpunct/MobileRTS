using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using Unity.Rendering;

public struct AArcherGhostSerializer : IGhostSerializer<AArcherSnapshotData>
{
    private ComponentType componentTypePlayerUnit;
    private ComponentType componentTypeUnitSelectionState;
    private ComponentType componentTypePhysicsCollider;
    private ComponentType componentTypeCompositeScale;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeLinkedEntityGroup;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerUnit> ghostPlayerUnitType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<UnitSelectionState> ghostUnitSelectionStateType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkBufferType<LinkedEntityGroup> ghostLinkedEntityGroupType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild0RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild0TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild1RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild1TranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<AArcherSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayerUnit = ComponentType.ReadWrite<PlayerUnit>();
        componentTypeUnitSelectionState = ComponentType.ReadWrite<UnitSelectionState>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypeCompositeScale = ComponentType.ReadWrite<CompositeScale>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeLinkedEntityGroup = ComponentType.ReadWrite<LinkedEntityGroup>();
        ghostPlayerUnitType = system.GetArchetypeChunkComponentType<PlayerUnit>(true);
        ghostUnitSelectionStateType = system.GetArchetypeChunkComponentType<UnitSelectionState>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
        ghostLinkedEntityGroupType = system.GetArchetypeChunkBufferType<LinkedEntityGroup>(true);
        ghostChild0RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild0TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild1RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild1TranslationType = system.GetComponentDataFromEntity<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref AArcherSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerUnit = chunk.GetNativeArray(ghostPlayerUnitType);
        var chunkDataUnitSelectionState = chunk.GetNativeArray(ghostUnitSelectionStateType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataLinkedEntityGroup = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
        snapshot.SetPlayerUnitPlayerId(chunkDataPlayerUnit[ent].PlayerId, serializerState);
        snapshot.SetUnitSelectionStateIsSelected(chunkDataUnitSelectionState[ent].IsSelected, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
        snapshot.SetChild0RotationValue(ghostChild0RotationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild0TranslationValue(ghostChild0TranslationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild1RotationValue(ghostChild1RotationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
        snapshot.SetChild1TranslationValue(ghostChild1TranslationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
    }
}