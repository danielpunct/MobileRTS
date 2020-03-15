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
            "AArcherGhostSerializer",
            "BArcherGhostSerializer",
        };
        return arr;
    }

    public int Length => 3;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(PlayerSnapshotData))
            return 0;
        if (typeof(T) == typeof(AArcherSnapshotData))
            return 1;
        if (typeof(T) == typeof(BArcherSnapshotData))
            return 2;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_PlayerGhostSerializer.BeginSerialize(system);
        m_AArcherGhostSerializer.BeginSerialize(system);
        m_BArcherGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_PlayerGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_AArcherGhostSerializer.CalculateImportance(chunk);
            case 2:
                return m_BArcherGhostSerializer.CalculateImportance(chunk);
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
                return m_AArcherGhostSerializer.SnapshotSize;
            case 2:
                return m_BArcherGhostSerializer.SnapshotSize;
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
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<AArcherGhostSerializer, AArcherSnapshotData>(m_AArcherGhostSerializer, ref dataStream, data);
            }
            case 2:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<BArcherGhostSerializer, BArcherSnapshotData>(m_BArcherGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private PlayerGhostSerializer m_PlayerGhostSerializer;
    private AArcherGhostSerializer m_AArcherGhostSerializer;
    private BArcherGhostSerializer m_BArcherGhostSerializer;
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
