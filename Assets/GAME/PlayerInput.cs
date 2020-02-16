using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public struct PlayerInput : ICommandData<PlayerInput>
{
    public uint tick;
    public float selectionX1;
    public float selectionY1;
    public float selectionX2;
    public float selectionY2;

    public uint Tick => tick; 

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        this.tick = tick;

        selectionX1 = reader.ReadFloat(ref ctx);
        selectionY1 = reader.ReadFloat(ref ctx);
        selectionX2 = reader.ReadFloat(ref ctx);
        selectionY2 = reader.ReadFloat(ref ctx);
    }

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, reader, ref ctx);
    }

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write(selectionX1);
        writer.Write(selectionY1);
        writer.Write(selectionX2);
        writer.Write(selectionY2);
    }

    public void Serialize(DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(writer);
    }

    public class NetCubeSendCommandSystem : CommandSendSystem<PlayerInput>
    {
    }


    public class NetCubeReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
    {
    }
}

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class SamplePlayerInput : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableMobileRTSGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;

        if(localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref MovableCubeComponent cube) =>
            {
                if(cube.PlayerId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent { targetEntity = ent });
                }
            });
            return;
        }

        var input = default(PlayerInput);
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;


        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
            {
                var point = ray.GetPoint(dist);
                Debug.Log(point);
            }
        }


        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.AddCommandData(input);
    }
}
