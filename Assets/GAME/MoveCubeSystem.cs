
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;

        // for each client input
        Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer, ref Translation trans, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            PlayerInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            float minX = math.min(input.selectionX1, input.selectionX2);
            float maxX = math.max(input.selectionX1, input.selectionX2);
            float minZ = math.min(input.selectionZ1, input.selectionZ2);
            float maxZ = math.max(input.selectionZ1, input.selectionZ2);

            if (minX == 0 && maxX == 0 && minZ == 0 && maxZ == 0)
            {
                return;
            }


            // deselect all units // not at all optimal
            Entities.WithAll<UnitSelected>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
            });

            // for each unit of client
            Entities.ForEach((Entity entity, ref Translation unitTrans) =>
            {
                if (minX <= unitTrans.Value.x &&
                   maxX >= unitTrans.Value.y &&
                   minZ <= unitTrans.Value.z &&
                   maxZ >= unitTrans.Value.z)
                {
                    PostUpdateCommands.AddComponent(entity, new UnitSelected());
                }

            });


        });
    }
}