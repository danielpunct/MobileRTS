using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(UnitsMoveSystem))]
public class UnitAttackSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);
        var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
        var ghostId = MobileRTSGhostSerializerCollection.FindGhostType<ArrowSnapshotData>();
        var arrowPrefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;

        var time = UnityEngine.Time.time;

        Entities.WithoutBurst().ForEach((
           Entity entity,
           ref Attack attack,
           in PlayerUnit playerUnit,
           in Translation translation) =>
           {
               var attacked = time - attack.AttackedAt < 2;
               if (attacked)
               {
                   return;
               }

               var query = GetEntityQuery(ComponentType.ReadOnly<PlayerUnit>(), ComponentType.ReadOnly<Translation>());
               var positions = query.ToComponentDataArray<Translation>(Allocator.TempJob);
               var playerUnits = query.ToComponentDataArray<PlayerUnit>((Allocator.TempJob));

               for (int i = 0; i < playerUnits.Length; i++)
               {
                   if (playerUnits[i].PlayerId == playerUnit.PlayerId)
                   {
                       continue;
                   }

                   var distance = math.distance(positions[i].Value, translation.Value);
                   if (distance < attack.AttackRadius)
                   {
                       Shoot(ECB, arrowPrefab, translation.Value, positions[i].Value);

                       // update components
                       attack.AttackedAt = time;
                       attacked = true;
                       break;
                   }
               }

               positions.Dispose();
               playerUnits.Dispose();

           }).Run();

        ECB.Playback(EntityManager);
        ECB.Dispose();

        return default;
    }

    void Shoot(EntityCommandBuffer ECB, Entity arrowPrefab, float3 from, float3 to)
    {
        // create prefab
        var bullet = ECB.Instantiate(arrowPrefab);
        ECB.AddComponent(bullet, new ArcMove
        {
            from = from,
            to = to,
            lerpValue = 0,
            maxHeight = 20,
            speed = 30
        });
    }
}

public struct Attack : IComponentData
{
    public float AttackRadius;
    public float AttackedAt;
}
