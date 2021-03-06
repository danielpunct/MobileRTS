﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class UnitsMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var deltaTime = UnityEngine.Time.deltaTime;

        var job = Entities.ForEach((ref Translation translation, ref MoveTo moveTo, in Rotation rotation) =>
        {
            if (moveTo.move) {
                float reachedPositionDistance = 1f;
                if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance) {
                    // Far from target position, Move to position
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    moveTo.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveTo.moveSpeed * deltaTime;
                } else {
                    // Already there
                    moveTo.move = false;
                }
            }

        }).Schedule(inputDependencies);

        job.Complete();

        return job;
    }
}

public struct MoveTo : IComponentData
{
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
    public float moveSpeed;
}