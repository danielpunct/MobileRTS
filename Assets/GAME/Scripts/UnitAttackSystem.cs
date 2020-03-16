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
        //var ECB = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach((
           Entity entity,
           in Translation translation,
           in Attack attack) =>
           {
               var position = translation.Value;
               var attackRadius = attack.AttackRadius;

               Entities.ForEach((
                   ref Entity otherEntity,
                   in Translation otherTranslation) =>
                   {
                       if (otherEntity != entity)
                       {
                           if (math.distance(position, otherTranslation.Value) < attackRadius)
                           {
                               Debug.Log("da");
                           }
                       }
                   }).Run();

           }).Run();

        //ECB.Playback(EntityManager);
        //ECB.Dispose();

        return default;
    }
}

public struct Attack : IComponentData
{
    public float AttackRadius;
}
