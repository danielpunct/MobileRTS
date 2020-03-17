using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(UnitAttackSystem))]
public class ArcMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((
            ref Translation translation,
            ref Rotation rotation,
            ref ArcMove arcMove,
            in Entity entity) =>
        {
            arcMove.lerpValue += 0.03f;
            if(arcMove.lerpValue>1)
            {
                ECB.RemoveComponent<ArcMove>(entity);
                return;
            }

            var oldPos = translation.Value;
            var newPos = math.lerp(arcMove.from, arcMove.to, arcMove.lerpValue);

            // add simulated height
            newPos.y += arcMove.maxHeight * math.sin( arcMove.lerpValue * math.PI);
            translation.Value = newPos;
            rotation.Value = Quaternion.LookRotation(math.normalize(newPos - oldPos));

        }).Run();

        ECB.Playback(EntityManager);
        ECB.Dispose();

        return default;
    }
}

public struct ArcMove : IComponentData
{
    public float3 from;
    public float3 to;
    public float maxHeight;
    public float speed;
    public float lerpValue;
}
