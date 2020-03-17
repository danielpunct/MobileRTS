using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(UnitAttackSystem))]
public class ArcMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((
            ref Translation translation,
            ref ArcMove arcMove,
            in Entity entity) =>
        {
            arcMove.lerpValue += 0.1f;
            if(arcMove.lerpValue>1)
            {
                ECB.RemoveComponent<ArcMove>(entity);
                return;
            }

            translation.Value = math.lerp(arcMove.from, arcMove.to, arcMove.lerpValue);

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
