using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

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
           in PlayerUnit playerUnit,
           in Attack attack,
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
                       // create prefab
                       ECB.Instantiate(arrowPrefab);

                       // update components
                       var attack_updated = attack;
                       attack_updated.AttackedAt = time;
                       ECB.SetComponent(entity, attack_updated);
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



    //    void TryAttack(bool attacked, EntityCommandBuffer ECB, Attack attack, float time, float3 position, Entity entity)
    //    {
    //        Entities.ForEach((
    //              ref Entity otherEntity,
    //              in Translation otherTranslation) =>
    //              {
    //                  if (attacked)
    //                  {
    //                      return;
    //                  }

    //                  if (otherEntity != entity)
    //                  {
    //                      var distance = math.distance(position, otherTranslation.Value);
    //                      if (distance < attack.AttackRadius)
    //                      {
    //                          var attack_updated = attack;
    //                          attack_updated.AttackedAt = time;
    //                          ECB.SetComponent(entity, attack_updated);
    //                          attacked = true;
    //                      }
    //                  }
    //              }).Run();

    //    }
}

public struct Attack : IComponentData
{
    public float AttackRadius;
    public float AttackedAt;
}
