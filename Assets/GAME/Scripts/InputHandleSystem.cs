
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class InputHandleSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;

        // for each  input from each client
        Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer, ref PredictedGhostComponent prediction, ref Player player) =>
        {

            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
            {
                Debug.Log("discarted");
                return;
            }
            inputBuffer.GetDataAtTick(tick, out PlayerInput input);

            var playerId = player.PlayerId;
            var selectionInput = false;

            float minX = math.min(input.selectionX1, input.selectionX2);
            float maxX = math.max(input.selectionX1, input.selectionX2);
            float minZ = math.min(input.selectionZ1, input.selectionZ2);
            float maxZ = math.max(input.selectionZ1, input.selectionZ2);

            if (minX != 0 || maxX != 0 || minZ != 0 || maxZ != 0)
            {
                selectionInput = true;
            }

            if (selectionInput)
            {
                // deselect all units // not at all optimal
                Entities.ForEach((Entity entity, ref UnitSelectionState selectionState) =>
                {
                    PostUpdateCommands.SetComponent(entity, new UnitSelectionState());
                });

                // for each unit of client
                Entities.ForEach((Entity entity, ref Translation unitTrans, ref PlayerUnit playerUnit) =>
                {
                   // if (playerUnit.PlayerId == playerId)
                    {
                        if (minX <= unitTrans.Value.x &&
                            maxX >= unitTrans.Value.x &&
                            minZ <= unitTrans.Value.z &&
                            maxZ >= unitTrans.Value.z)
                        {
                            PostUpdateCommands.SetComponent(entity, new UnitSelectionState() { IsSelected = true }) ;
                        }
                    }
                });
            }
            // if destination input
            else if (input.destinationX != 0 || input.destinationZ != 0)
            {
                var destination = new float3(input.destinationX, 0, input.destinationZ);
                var positions = GetPositionListAround(destination);
                int positionIndex = 0;
                Entities.ForEach((Entity entity, ref MoveTo moveTo, ref UnitSelectionState selectionState) =>
                {
                    if (selectionState.IsSelected)
                    {
                        moveTo.position = positions[positionIndex++];
                        moveTo.move = true;
                    }
                });
            }
        });
    }

    List<float3> GetPositionListAround(float3 startPosition)
    {
        float[] ringDistance = new float[] { 10, 20, 30, 40, 50 };
        int[] ringPositionCount = new int[] { 5, 10, 20, 40, 80 };
        var positionList = new List<float3>();

        positionList.Add(startPosition);
        for (int ring = 0; ring < ringPositionCount.Length; ring++)
        {
            var ringPositionList = GetPositionListAround(startPosition, ringDistance[ring]/2, ringPositionCount[ring]);
            positionList.AddRange(ringPositionList);
        }

        return positionList;
    }

    List<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
    {
        var positionList = new List<float3>();

        for(int i = 0; i < positionCount; i++)
        {
            var angle = i * (360 / positionCount);
            var dir = ApplyRotationToVector(new float3(1, 0, 0), angle);
            var position = startPosition + dir * distance;
            positionList.Add(position);
        }

        return positionList;
        
    }

    private float3 ApplyRotationToVector(float3 vec, int angle)
    {
        return Quaternion.Euler(0, angle, 0) * vec;
    }
}

