using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public struct PlayerInput : ICommandData<PlayerInput>
{
    public uint tick;
    public float destinationX;
    public float destinationZ;

    public float selectionX1;
    public float selectionZ1;
    public float selectionX2;
    public float selectionZ2;

    public uint Tick => tick;


    public void Deserialize(uint tick, ref DataStreamReader reader, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, ref reader);
    }

    public void Deserialize(uint tick, ref DataStreamReader reader)
    {
        this.tick = tick;

        destinationX = reader.ReadFloat();
        destinationZ = reader.ReadFloat();

        selectionX1 = reader.ReadFloat();
        selectionZ1 = reader.ReadFloat();
        selectionX2 = reader.ReadFloat();
        selectionZ2 = reader.ReadFloat();
    }

    public void Serialize(ref DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(ref writer);
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteFloat(destinationX);
        writer.WriteFloat(destinationZ);
        writer.WriteFloat(selectionX1);
        writer.WriteFloat(selectionZ1);
        writer.WriteFloat(selectionX2);
        writer.WriteFloat(selectionZ2);
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

    bool hasSelection;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableMobileRTSGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;

        if (localInput == Entity.Null)
        {
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref Player player) =>
            {
                if (player.PlayerId == localPlayerId)
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

                if (SelectionIsValid())
                {
                    SetSelection(ref input);
                }
                else
                {
                    if (hasSelection)
                    {
                        SetDestination(ref input);
                        GameReferences.Instance.ConsumeBuildEvent();
                    }
                }
            }
        }

        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.AddCommandData(input);
    }

    public void SetDestination(ref PlayerInput input)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
        {
            var d = ray.GetPoint(dist);

            input.destinationX = d.x;
            input.destinationZ = d.z;
        }
    }

    public bool SetSelection(ref PlayerInput input)
    {
        input.selectionX1 = startPoint.x;
        input.selectionZ1 = startPoint.z;
        input.selectionX2 = endPoint.x;
        input.selectionZ2 = endPoint.z;

        hasSelection = true;
        return true;
    }

    bool SelectionIsValid()
    {
        return Vector3.SqrMagnitude(endPoint - startPoint) > 0.1f;
    }
}
