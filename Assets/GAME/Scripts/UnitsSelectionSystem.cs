
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class UnitsSelectionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;

        // for each  input from each client
        Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer, ref PredictedGhostComponent prediction, ref Player player) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            inputBuffer.GetDataAtTick(tick, out PlayerInput input);

            var playerId = player.PlayerId;
            var selectionInput = false;

            float minX = math.min(input.selectionX1, input.selectionX2);
            float maxX = math.max(input.selectionX1, input.selectionX2);
            float minZ = math.min(input.selectionZ1, input.selectionZ2);
            float maxZ = math.max(input.selectionZ1, input.selectionZ2);

            if (minX != 0 || maxX != 0 || minZ != 0 || maxZ != 0)
            {
                selectionInput = true;
            }

            if (selectionInput)
            {
                // deselect all units // not at all optimal
                Entities.WithAll<UnitSelected>().ForEach((Entity entity) =>
                {
                    PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
                });

                // for each unit of client
                Entities.ForEach((Entity entity, ref Translation unitTrans, ref PlayerUnit playerUnit) =>
                {
                    if (playerUnit.PlayerId == playerId)
                    {
                        if (minX <= unitTrans.Value.x &&
                            maxX >= unitTrans.Value.y &&
                            minZ <= unitTrans.Value.z &&
                            maxZ >= unitTrans.Value.z)
                        {
                            PostUpdateCommands.AddComponent(entity, new UnitSelected());
                        }
                    }
                });
            }
            // if destination input
            else if (input.destinationX != 0 || input.destinationZ != 0)
            {
                Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveTo moveTo) =>
                {
                    moveTo.position = new float3(input.destinationX, 0, input.destinationZ);
                    moveTo.move = true;
                });
            }
        });
    }
}