using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct MobileRTSGhostSerializerCollection : IGhostSerializerCollection
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
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(PlayerSnapshotData))
            return 0;
        if (typeof(T) == typeof(ArrowSnapshotData))
            return 1;
        if (typeof(T) == typeof(AArcherSnapshotData))
            return 2;
        if (typeof(T) == typeof(ACivilianSnapshotData))
            return 3;
        if (typeof(T) == typeof(BArcherSnapshotData))
            return 4;
        if (typeof(T) == typeof(BCivilianSnapshotData))
            return 5;
        if (typeof(T) == typeof(A_BarracksSnapshotData))
            return 6;
        if (typeof(T) == typeof(B_BarracksSnapshotData))
            return 7;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_PlayerGhostSerializer.BeginSerialize(system);
        m_ArrowGhostSerializer.BeginSerialize(system);
        m_AArcherGhostSerializer.BeginSerialize(system);
        m_ACivilianGhostSerializer.BeginSerialize(system);
        m_BArcherGhostSerializer.BeginSerialize(system);
        m_BCivilianGhostSerializer.BeginSerialize(system);
        m_A_BarracksGhostSerializer.BeginSerialize(system);
        m_B_BarracksGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_PlayerGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_ArrowGhostSerializer.CalculateImportance(chunk);
            case 2:
                return m_AArcherGhostSerializer.CalculateImportance(chunk);
            case 3:
                return m_ACivilianGhostSerializer.CalculateImportance(chunk);
            case 4:
                return m_BArcherGhostSerializer.CalculateImportance(chunk);
            case 5:
                return m_BCivilianGhostSerializer.CalculateImportance(chunk);
            case 6:
                return m_A_BarracksGhostSerializer.CalculateImportance(chunk);
            case 7:
                return m_B_BarracksGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_PlayerGhostSerializer.SnapshotSize;
            case 1:
                return m_ArrowGhostSerializer.SnapshotSize;
            case 2:
                return m_AArcherGhostSerializer.SnapshotSize;
            case 3:
                return m_ACivilianGhostSerializer.SnapshotSize;
            case 4:
                return m_BArcherGhostSerializer.SnapshotSize;
            case 5:
                return m_BCivilianGhostSerializer.SnapshotSize;
            case 6:
                return m_A_BarracksGhostSerializer.SnapshotSize;
            case 7:
                return m_B_BarracksGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<PlayerGhostSerializer, PlayerSnapshotData>(m_PlayerGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<ArrowGhostSerializer, ArrowSnapshotData>(m_ArrowGhostSerializer, ref dataStream, data);
            }
            case 2:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<AArcherGhostSerializer, AArcherSnapshotData>(m_AArcherGhostSerializer, ref dataStream, data);
            }
            case 3:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<ACivilianGhostSerializer, ACivilianSnapshotData>(m_ACivilianGhostSerializer, ref dataStream, data);
            }
            case 4:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<BArcherGhostSerializer, BArcherSnapshotData>(m_BArcherGhostSerializer, ref dataStream, data);
            }
            case 5:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<BCivilianGhostSerializer, BCivilianSnapshotData>(m_BCivilianGhostSerializer, ref dataStream, data);
            }
            case 6:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<A_BarracksGhostSerializer, A_BarracksSnapshotData>(m_A_BarracksGhostSerializer, ref dataStream, data);
            }
            case 7:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<B_BarracksGhostSerializer, B_BarracksSnapshotData>(m_B_BarracksGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private PlayerGhostSerializer m_PlayerGhostSerializer;
    private ArrowGhostSerializer m_ArrowGhostSerializer;
    private AArcherGhostSerializer m_AArcherGhostSerializer;
    private ACivilianGhostSerializer m_ACivilianGhostSerializer;
    private BArcherGhostSerializer m_BArcherGhostSerializer;
    private BCivilianGhostSerializer m_BCivilianGhostSerializer;
    private A_BarracksGhostSerializer m_A_BarracksGhostSerializer;
    private B_BarracksGhostSerializer m_B_BarracksGhostSerializer;
}

public struct EnableMobileRTSGhostSendSystemComponent : IComponentData
{}
public class MobileRTSGhostSendSystem : GhostSendSystem<MobileRTSGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableMobileRTSGhostSendSystemComponent>();
    }
}
