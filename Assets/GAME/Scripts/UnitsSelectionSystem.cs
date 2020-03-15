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
        var PostUpdateCommands = new EntityCommandBuffer(Allocator.Temp);

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
                 PostUpdateCommands.AddComponent<Disabled>(entity);
             }
             else
             {
                 PostUpdateCommands.RemoveComponent<Disabled>(entity);
             }
         }).Run();

        PostUpdateCommands.Playback(EntityManager);
        PostUpdateCommands.Dispose();

        return default;
    }
}

