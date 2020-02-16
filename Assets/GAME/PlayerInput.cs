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
    public float selectionZ1;
    public float selectionX2;
    public float selectionZ2;

    public uint Tick => tick; 

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        this.tick = tick;

        selectionX1 = reader.ReadFloat(ref ctx);
        selectionZ1 = reader.ReadFloat(ref ctx);
        selectionX2 = reader.ReadFloat(ref ctx);
        selectionZ2 = reader.ReadFloat(ref ctx);
    }

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, reader, ref ctx);
    }

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write(selectionX1);
        writer.Write(selectionZ1);
        writer.Write(selectionX2);
        writer.Write(selectionZ2);
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
    Vector3 startPoint = Vector3.zero;
    Vector3 endPoint = Vector3.zero;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableMobileRTSGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;

        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref MovableCubeComponent cube) =>
            {
                if (cube.PlayerId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent { targetEntity = ent });
                }
            });
            return;
        }

        var input = default(PlayerInput);
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;


        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
            {
                startPoint = ray.GetPoint(dist);
                GameReferences.Instance.selection.gameObject.SetActive(true);
                GameReferences.Instance.selection.position = startPoint;
            }
        }

        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
            {
                var point = ray.GetPoint(dist);

                var selectionSize = point - startPoint;
                selectionSize.y = 1;
                GameReferences.Instance.selection.localScale = selectionSize;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            GameReferences.Instance.selection.gameObject.SetActive(false);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
            {
                endPoint = ray.GetPoint(dist);

                input.selectionX1 = startPoint.x;
                input.selectionZ1 = startPoint.z;
                input.selectionX2 = endPoint.x;
                input.selectionZ2 = endPoint.z;
            }
        }

        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.AddCommandData(input);
    }
}
