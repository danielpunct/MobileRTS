using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using Unity.Rendering;

public struct ArrowGhostSerializer : IGhostSerializer<ArrowSnapshotData>
{
    private ComponentType componentTypePlayerBullet;
    private ComponentType componentTypePhysicsCollider;
    private ComponentType componentTypePhysicsDamping;
    private ComponentType componentTypePhysicsMass;
    private ComponentType componentTypePhysicsVelocity;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeLinkedEntityGroup;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerBullet> ghostPlayerBulletType;
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

    public int SnapshotSize => UnsafeUtility.SizeOf<ArrowSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayerBullet = ComponentType.ReadWrite<PlayerBullet>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypePhysicsDamping = ComponentType.ReadWrite<PhysicsDamping>();
        componentTypePhysicsMass = ComponentType.ReadWrite<PhysicsMass>();
        componentTypePhysicsVelocity = ComponentType.ReadWrite<PhysicsVelocity>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeLinkedEntityGroup = ComponentType.ReadWrite<LinkedEntityGroup>();
        ghostPlayerBulletType = system.GetArchetypeChunkComponentType<PlayerBullet>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
        ghostLinkedEntityGroupType = system.GetArchetypeChunkBufferType<LinkedEntityGroup>(true);
        ghostChild0RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild0TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild1RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild1TranslationType = system.GetComponentDataFromEntity<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref ArrowSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerBullet = chunk.GetNativeArray(ghostPlayerBulletType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataLinkedEntityGroup = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
        snapshot.SetPlayerBulletPlayerId(chunkDataPlayerBullet[ent].PlayerId, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
        snapshot.SetChild0RotationValue(ghostChild0RotationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild0TranslationValue(ghostChild0TranslationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild1RotationValue(ghostChild1RotationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
        snapshot.SetChild1TranslationValue(ghostChild1TranslationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
    }
}
