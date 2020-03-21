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
        var pu = GetComponentDataFromEntity<PlayerUnit>(true);

        var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
        var ECB = new EntityCommandBuffer(Allocator.Temp);


        PlayerUnit? selection = null;
        Entities
             .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
             .ForEach((
          ref Entity entity,
          in Parent parent,
          in UnitSelectionVisual selectionVisual) =>
         {
             var playerUnit = pu[parent.Value];

             if (localPlayerId != playerUnit.PlayerId)
             {
                 return;
             }

             var sel = us[parent.Value].IsSelected;

             if (!sel)
             {
                 ECB.AddComponent<Disabled>(entity);
             }
             else
             {
                 ECB.RemoveComponent<Disabled>(entity);
                 selection = playerUnit;
             }
         }).Run();

        ECB.Playback(EntityManager);
        ECB.Dispose();

        if (selection != null)
        {
            GameReferences.Instance.ShowInfo(selection.Value.UnitId);
        }
        else
        {
            GameReferences.Instance.Clear();
        }
        return default;
    }
}

