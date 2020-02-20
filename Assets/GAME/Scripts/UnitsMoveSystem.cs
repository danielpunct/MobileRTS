using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class UnitsMoveSystem : JobComponentSystem
{
    [BurstCompile]
    struct UnitsMoveSystemJob : IJobForEach<Translation, Rotation, MoveTo>
    {
        public float deltaTime;

        public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation, ref MoveTo moveTo)
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
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new UnitsMoveSystemJob
        {

            deltaTime = UnityEngine.Time.deltaTime
        };


        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }


}

public struct MoveTo : IComponentData
{
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
    public float moveSpeed;
}