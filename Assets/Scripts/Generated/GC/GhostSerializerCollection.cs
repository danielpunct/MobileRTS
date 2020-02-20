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
            "CubeGhostSerializer",
            "PlayerGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(CubeSnapshotData))
            return 0;
        if (typeof(T) == typeof(PlayerSnapshotData))
            return 1;
        return -1;
    }
    public int FindSerializer(EntityArchetype arch)
    {
        if (m_CubeGhostSerializer.CanSerialize(arch))
            return 0;
        if (m_PlayerGhostSerializer.CanSerialize(arch))
            return 1;
        throw new ArgumentException("Invalid serializer type");
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_CubeGhostSerializer.BeginSerialize(system);
        m_PlayerGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_CubeGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_PlayerGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public bool WantsPredictionDelta(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_CubeGhostSerializer.WantsPredictionDelta;
            case 1:
                return m_PlayerGhostSerializer.WantsPredictionDelta;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_CubeGhostSerializer.SnapshotSize;
            case 1:
                return m_PlayerGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<CubeGhostSerializer, CubeSnapshotData>(m_CubeGhostSerializer, data);
            }
            case 1:
            {
                return GhostSendSystem<MobileRTSGhostSerializerCollection>.InvokeSerialize<PlayerGhostSerializer, PlayerSnapshotData>(m_PlayerGhostSerializer, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private CubeGhostSerializer m_CubeGhostSerializer;
    private PlayerGhostSerializer m_PlayerGhostSerializer;
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
