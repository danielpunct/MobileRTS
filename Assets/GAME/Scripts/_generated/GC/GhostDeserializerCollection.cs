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
            "ArrowGhostSerializer",
            "AArcherGhostSerializer",
            "ACivilianGhostSerializer",
            "BArcherGhostSerializer",
            "BCivilianGhostSerializer",
            "A_BarracksGhostSerializer",
            "B_BarracksGhostSerializer",
        };
        return arr;
    }

    public int Length => 8;
#endif
    public void Initialize(World world)
    {
        var curPlayerGhostSpawnSystem = world.GetOrCreateSystem<PlayerGhostSpawnSystem>();
        m_PlayerSnapshotDataNewGhostIds = curPlayerGhostSpawnSystem.NewGhostIds;
        m_PlayerSnapshotDataNewGhosts = curPlayerGhostSpawnSystem.NewGhosts;
        curPlayerGhostSpawnSystem.GhostType = 0;
        var curArrowGhostSpawnSystem = world.GetOrCreateSystem<ArrowGhostSpawnSystem>();
        m_ArrowSnapshotDataNewGhostIds = curArrowGhostSpawnSystem.NewGhostIds;
        m_ArrowSnapshotDataNewGhosts = curArrowGhostSpawnSystem.NewGhosts;
        curArrowGhostSpawnSystem.GhostType = 1;
        var curAArcherGhostSpawnSystem = world.GetOrCreateSystem<AArcherGhostSpawnSystem>();
        m_AArcherSnapshotDataNewGhostIds = curAArcherGhostSpawnSystem.NewGhostIds;
        m_AArcherSnapshotDataNewGhosts = curAArcherGhostSpawnSystem.NewGhosts;
        curAArcherGhostSpawnSystem.GhostType = 2;
        var curACivilianGhostSpawnSystem = world.GetOrCreateSystem<ACivilianGhostSpawnSystem>();
        m_ACivilianSnapshotDataNewGhostIds = curACivilianGhostSpawnSystem.NewGhostIds;
        m_ACivilianSnapshotDataNewGhosts = curACivilianGhostSpawnSystem.NewGhosts;
        curACivilianGhostSpawnSystem.GhostType = 3;
        var curBArcherGhostSpawnSystem = world.GetOrCreateSystem<BArcherGhostSpawnSystem>();
        m_BArcherSnapshotDataNewGhostIds = curBArcherGhostSpawnSystem.NewGhostIds;
        m_BArcherSnapshotDataNewGhosts = curBArcherGhostSpawnSystem.NewGhosts;
        curBArcherGhostSpawnSystem.GhostType = 4;
        var curBCivilianGhostSpawnSystem = world.GetOrCreateSystem<BCivilianGhostSpawnSystem>();
        m_BCivilianSnapshotDataNewGhostIds = curBCivilianGhostSpawnSystem.NewGhostIds;
        m_BCivilianSnapshotDataNewGhosts = curBCivilianGhostSpawnSystem.NewGhosts;
        curBCivilianGhostSpawnSystem.GhostType = 5;
        var curA_BarracksGhostSpawnSystem = world.GetOrCreateSystem<A_BarracksGhostSpawnSystem>();
        m_A_BarracksSnapshotDataNewGhostIds = curA_BarracksGhostSpawnSystem.NewGhostIds;
        m_A_BarracksSnapshotDataNewGhosts = curA_BarracksGhostSpawnSystem.NewGhosts;
        curA_BarracksGhostSpawnSystem.GhostType = 6;
        var curB_BarracksGhostSpawnSystem = world.GetOrCreateSystem<B_BarracksGhostSpawnSystem>();
        m_B_BarracksSnapshotDataNewGhostIds = curB_BarracksGhostSpawnSystem.NewGhostIds;
        m_B_BarracksSnapshotDataNewGhosts = curB_BarracksGhostSpawnSystem.NewGhosts;
        curB_BarracksGhostSpawnSystem.GhostType = 7;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_PlayerSnapshotDataFromEntity = system.GetBufferFromEntity<PlayerSnapshotData>();
        m_ArrowSnapshotDataFromEntity = system.GetBufferFromEntity<ArrowSnapshotData>();
        m_AArcherSnapshotDataFromEntity = system.GetBufferFromEntity<AArcherSnapshotData>();
        m_ACivilianSnapshotDataFromEntity = system.GetBufferFromEntity<ACivilianSnapshotData>();
        m_BArcherSnapshotDataFromEntity = system.GetBufferFromEntity<BArcherSnapshotData>();
        m_BCivilianSnapshotDataFromEntity = system.GetBufferFromEntity<BCivilianSnapshotData>();
        m_A_BarracksSnapshotDataFromEntity = system.GetBufferFromEntity<A_BarracksSnapshotData>();
        m_B_BarracksSnapshotDataFromEntity = system.GetBufferFromEntity<B_BarracksSnapshotData>();
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
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_ArrowSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 2:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_AArcherSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 3:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_ACivilianSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 4:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_BArcherSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 5:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_BCivilianSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 6:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_A_BarracksSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 7:
                return GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeDeserialize(m_B_BarracksSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
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
                m_ArrowSnapshotDataNewGhostIds.Add(ghostId);
                m_ArrowSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<ArrowSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 2:
                m_AArcherSnapshotDataNewGhostIds.Add(ghostId);
                m_AArcherSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<AArcherSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 3:
                m_ACivilianSnapshotDataNewGhostIds.Add(ghostId);
                m_ACivilianSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<ACivilianSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 4:
                m_BArcherSnapshotDataNewGhostIds.Add(ghostId);
                m_BArcherSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<BArcherSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 5:
                m_BCivilianSnapshotDataNewGhostIds.Add(ghostId);
                m_BCivilianSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<BCivilianSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 6:
                m_A_BarracksSnapshotDataNewGhostIds.Add(ghostId);
                m_A_BarracksSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<A_BarracksSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 7:
                m_B_BarracksSnapshotDataNewGhostIds.Add(ghostId);
                m_B_BarracksSnapshotDataNewGhosts.Add(GhostReceiveSystem<MobileRTSGhostDeserializerCollection>.InvokeSpawn<B_BarracksSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<PlayerSnapshotData> m_PlayerSnapshotDataFromEntity;
    private NativeList<int> m_PlayerSnapshotDataNewGhostIds;
    private NativeList<PlayerSnapshotData> m_PlayerSnapshotDataNewGhosts;
    private BufferFromEntity<ArrowSnapshotData> m_ArrowSnapshotDataFromEntity;
    private NativeList<int> m_ArrowSnapshotDataNewGhostIds;
    private NativeList<ArrowSnapshotData> m_ArrowSnapshotDataNewGhosts;
    private BufferFromEntity<AArcherSnapshotData> m_AArcherSnapshotDataFromEntity;
    private NativeList<int> m_AArcherSnapshotDataNewGhostIds;
    private NativeList<AArcherSnapshotData> m_AArcherSnapshotDataNewGhosts;
    private BufferFromEntity<ACivilianSnapshotData> m_ACivilianSnapshotDataFromEntity;
    private NativeList<int> m_ACivilianSnapshotDataNewGhostIds;
    private NativeList<ACivilianSnapshotData> m_ACivilianSnapshotDataNewGhosts;
    private BufferFromEntity<BArcherSnapshotData> m_BArcherSnapshotDataFromEntity;
    private NativeList<int> m_BArcherSnapshotDataNewGhostIds;
    private NativeList<BArcherSnapshotData> m_BArcherSnapshotDataNewGhosts;
    private BufferFromEntity<BCivilianSnapshotData> m_BCivilianSnapshotDataFromEntity;
    private NativeList<int> m_BCivilianSnapshotDataNewGhostIds;
    private NativeList<BCivilianSnapshotData> m_BCivilianSnapshotDataNewGhosts;
    private BufferFromEntity<A_BarracksSnapshotData> m_A_BarracksSnapshotDataFromEntity;
    private NativeList<int> m_A_BarracksSnapshotDataNewGhostIds;
    private NativeList<A_BarracksSnapshotData> m_A_BarracksSnapshotDataNewGhosts;
    private BufferFromEntity<B_BarracksSnapshotData> m_B_BarracksSnapshotDataFromEntity;
    private NativeList<int> m_B_BarracksSnapshotDataNewGhostIds;
    private NativeList<B_BarracksSnapshotData> m_B_BarracksSnapshotDataNewGhosts;
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
