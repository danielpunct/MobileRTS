using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class UnitsSelectionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<UnitSelectionVisual>()
            .WithIncludeAll()
            .ForEach((
                Entity entity,
                ref Parent parent) =>
               {
                   var sel = EntityManager.GetComponentData<UnitSelectionState>(parent.Value).IsSelected;

                   if (!sel)
                   {
                       PostUpdateCommands.AddComponent<Disabled>(entity);
                   }
                   else
                   {
                       PostUpdateCommands.RemoveComponent<Disabled>(entity);
                   }

               });

    }



    //public class UnitsSelectionSystem : JobComponentSystem
    //{
    //    BeginSimulationEntityCommandBufferSystem ECB;

    //    protected override void OnCreate()
    //    {
    //        base.OnCreate();

    //        ECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

    //    }

    //    //[RequireComponentTag(typeof(Disabled))]
    //    //[BurstCompile]
    //    //struct UnitsSelSystemJob : IJobForEachWithEntity<Parent, UnitSelectionVisual>
    //    //{
    //    //    public EntityCommandBuffer.Concurrent CommandBuffer;
    //    //    [ReadOnly] public ComponentDataFromEntity<UnitSelectionState> us;

    //    //    public void Execute(Entity entity, int index,
    //    //        [ReadOnly] ref Parent parent,
    //    //        [ReadOnly] ref UnitSelectionVisual unitSelectionVisual)
    //    //    {

    //    //        var sel = us[parent.Value].IsSelected;
    //    //        if (!sel)
    //    //        {
    //    //            CommandBuffer.AddComponent<Disabled>(index, entity);
    //    //        }
    //    //        else
    //    //        {
    //    //            CommandBuffer.RemoveComponent<Disabled>(index, entity);
    //    //        }
    //    //    }
    //    //}

    //    //protected override JobHandle OnUpdate(JobHandle inputDependencies)
    //    //{
    //    //    var job = new UnitsSelSystemJob
    //    //    {
    //    //        CommandBuffer = ECB.CreateCommandBuffer().ToConcurrent(),
    //    //        us = GetComponentDataFromEntity<UnitSelectionState>(true)
    //    //    }.Schedule(this, inputDependencies);

    //    //    ECB.AddJobHandleForProducer(job);

    //    //    // Now that the job is set up, schedule it to be run. 
    //    //    return job;
    //    //}

    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
    //    {
    //        var ecb = ECB.CreateCommandBuffer().ToConcurrent();
    //           var us = GetComponentDataFromEntity<UnitSelectionState>(true);

    //       return Entities
    //           //.WithoutBurst()
    //           //.WithAny<Disabled>()
    //           .ForEach((
    //        ref Entity entity,
    //        ref int nativeThreadIndex,
    //        in Parent parent,
    //        in UnitSelectionVisual selectionVisual) =>
    //       {
    //           //var sel = EntityManager.HasComponent<UnitSelected>(parent.Value);

    //           var sel = us[parent.Value].IsSelected;

    //           //var ecb = ECB.CreateCommandBuffer().ToConcurrent();

    //          // if (sel)
    //           {
    //               ecb.AddComponent<Disabled>(nativeThreadIndex, entity);
    //           }
    //           //else
    //           //{
    //           //    ecb.RemoveComponent<Disabled>(nativeThreadIndex, entity);
    //           //}


    //       }).Schedule(inputDeps);

    //        return default(JobHandle);
    //    }

}
