using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct PlayerSnapshotData : ISnapshotData<PlayerSnapshotData>
{
    public uint tick;
    private int PlayerPlayerId;
    uint changeMask0;

    public uint Tick => tick;
    public int GetPlayerPlayerId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerPlayerId;
    }
    public int GetPlayerPlayerId()
    {
        return (int)PlayerPlayerId;
    }
    public void SetPlayerPlayerId(int val, GhostSerializerState serializerState)
    {
        PlayerPlayerId = (int)val;
    }
    public void SetPlayerPlayerId(int val)
    {
        PlayerPlayerId = (int)val;
    }

    public void PredictDelta(uint tick, ref PlayerSnapshotData baseline1, ref PlayerSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        PlayerPlayerId = predictor.PredictInt(PlayerPlayerId, baseline1.PlayerPlayerId, baseline2.PlayerPlayerId);
    }

    public void Serialize(int networkId, ref PlayerSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (PlayerPlayerId != baseline.PlayerPlayerId) ? 1u : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(PlayerPlayerId, baseline.PlayerPlayerId, compressionModel);
    }

    public void Deserialize(uint tick, ref PlayerSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            PlayerPlayerId = reader.ReadPackedIntDelta(baseline.PlayerPlayerId, compressionModel);
        else
            PlayerPlayerId = baseline.PlayerPlayerId;
    }
    public void Interpolate(ref PlayerSnapshotData target, float factor)
    {
    }
}
