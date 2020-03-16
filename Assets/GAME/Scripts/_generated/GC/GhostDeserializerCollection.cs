using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct MobileRTSGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "PlayerGhostSerializer",
            "AArcherGhostSerializer",
            "BArcherGhostSerializer",
            "ArrowGhostSerializer",
        };
        return arr;
    }

    public int Length => 4;
#endif
    public void Initialize(World world)
    {
        var curPlayerGhostSpawnSystem = world.GetOrCreateSystem<PlayerGhostSpawnSystem>();
        m_PlayerSnapshotDataNewGhostIds = curPlayerGhostSpawnSystem.NewGhostIds;
        m_PlayerSnapshotDataNewGhosts = curPlayerGhostSpawnSystem.NewGhosts;
        curPlayerGhostSpawnSystem.GhostType = 0;
        var curAArcherGhostSpawnSystem = world.GetOrCreateSystem<AArcherGhostSpawnSystem>();
        m_AArcherSnapshotDataNewGhostIds = curAArcherGhostSpawnSystem.NewGhostIds;
        m_AArcherSnapshotDataNewGhosts = curAArcherGhostSpawnSystem.NewGhosts;
        curAArcherGhostSpawnSystem.GhostType = 1;
        var curBArcherGhostSpawnSystem = world.GetOrCreateSystem<BArcherGhostSpawnSystem>();
        m_BArcherSnapshotDataNewGhostIds = curBArcherGhostSpawnSystem.NewGhostIds;
        m_BArcherSnapshotDataNewGhosts = curBArcherGhostSpawnSystem.NewGhosts;
        curBArcherGhostSpawnSystem.GhostType = 2;
        var curArrowGhostSpawnSystem = world.GetOrCreateSystem<ArrowGhostSpawnSystem>();
        m_ArrowSnapshotDataNewGhostIds = curArrowGhostSpawnSystem.NewGhostIds;
        m_ArrowSnapshotDataNewGhosts = curArrowGhostSpawnSystem.NewGhosts;
        curArrowGhostSpawnSystem.GhostType = 3;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_PlayerSnapshotDataFromEntity = system.GetBufferFromEntity<PlayerSnapshotData>();
        m_AArcherSnapshotDataFromEntity = system.GetBufferFromEntity<AArcherSnapshotData>();
        m_BArcherSnapshotDataFromEntity = system.GetBufferFromEntity<BArcherSnapshotData>();
        m_ArrowSnapshotDataFromEntity = system.GetBufferFromEntity<ArrowSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_PlayerSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_AArcherSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 2:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_BArcherSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 3:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_ArrowSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_PlayerSnapshotDataNewGhostIds.Add(ghostId);
                m_PlayerSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<PlayerSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_AArcherSnapshotDataNewGhostIds.Add(ghostId);
                m_AArcherSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<AArcherSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 2:
                m_BArcherSnapshotDataNewGhostIds.Add(ghostId);
                m_BArcherSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<BArcherSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 3:
                m_ArrowSnapshotDataNewGhostIds.Add(ghostId);
                m_ArrowSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<ArrowSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<PlayerSnapshotData> m_PlayerSnapshotDataFromEntity;
    private NativeList<int> m_PlayerSnapshotDataNewGhostIds;
    private NativeList<PlayerSnapshotData> m_PlayerSnapshotDataNewGhosts;
    private BufferFromEntity<AArcherSnapshotData> m_AArcherSnapshotDataFromEntity;
    private NativeList<int> m_AArcherSnapshotDataNewGhostIds;
    private NativeList<AArcherSnapshotData> m_AArcherSnapshotDataNewGhosts;
    private BufferFromEntity<BArcherSnapshotData> m_BArcherSnapshotDataFromEntity;
    private NativeList<int> m_BArcherSnapshotDataNewGhostIds;
    private NativeList<BArcherSnapshotData> m_BArcherSnapshotDataNewGhosts;
    private BufferFromEntity<ArrowSnapshotData> m_ArrowSnapshotDataFromEntity;
    private NativeList<int> m_ArrowSnapshotDataNewGhostIds;
    private NativeList<ArrowSnapshotData> m_ArrowSnapshotDataNewGhosts;
}
public struct EnableMobileRTSGhostReceiveSystemComponent : IComponentData
{}
public class MobileRTSGhostReceiveSystem : GhostReceiveSystem<MobileRTSGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableMobileRTSGhostReceiveSystemComponent>();
    }
}
