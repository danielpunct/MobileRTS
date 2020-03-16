using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class UnitsSelectionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var us = GetComponentDataFromEntity<UnitSelectionState>(true);
        var ECB = new EntityCommandBuffer(Allocator.Temp);

        Entities
             .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
             .ForEach((
          ref Entity entity,
          in Parent parent,
          in UnitSelectionVisual selectionVisual) =>
         {
             var sel = us[parent.Value].IsSelected;

             if (!sel)
             {
                 ECB.AddComponent<Disabled>(entity);
             }
             else
             {
                 ECB.RemoveComponent<Disabled>(entity);
             }
         }).Run();

        ECB.Playback(EntityManager);
        ECB.Dispose();

        return default;
    }
}

