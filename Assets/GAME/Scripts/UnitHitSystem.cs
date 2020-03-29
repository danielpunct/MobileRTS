using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(UnitsMoveSystem))]
public class UnitHitSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<PlayerBullet>().WithoutBurst().ForEach((
            Entity entity,
            in Translation translation) =>
        {
            var query = GetEntityQuery(ComponentType.ReadOnly<Health>(), ComponentType.ReadOnly<Translation>());
            var positions = query.ToComponentDataArray<Translation>(Allocator.TempJob);
            var unitsHealth = query.ToComponentDataArray<Health>((Allocator.TempJob));
            var entityes_native = query.ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < positions.Length; i++)
            {
                var distance = math.distance(positions[i].Value, translation.Value);
                if (distance < 2)
                {
                    // move the removal in another system?
                    var new_health =  unitsHealth[i].Value- 10;
                    UnityEngine.Debug.Log("take");
                    if (new_health > 0)
                    {
                        ECB.SetComponent(entityes_native[i], new Health { Value = new_health });
                    }
                    else
                    {
                        ECB.DestroyEntity(entityes_native[i]);
                    }

                    ECB.DestroyEntity(entity);
                }
            }

            positions.Dispose();
            unitsHealth.Dispose();
            entityes_native.Dispose();

        }).Run();

        ECB.Playback(EntityManager);
        ECB.Dispose();

        return default;
    }
}
