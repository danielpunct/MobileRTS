using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class PlayerMakeConfigSystem : ComponentSystem
{
    Unity.Mathematics.Random random = new Unity.Mathematics.Random(0x6E624EB7u);

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerConfig>().ForEach((Entity entity, ref PlayerConfig config, ref Player player) =>
        {
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
            var ghostId = MobileRTSGhostSerializerCollection.FindGhostType<CubeSnapshotData>();
            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;

            for (int i = 0; i < config.Units; i++)
            {
                var unit = EntityManager.Instantiate(prefab);
                PostUpdateCommands.SetComponent(unit, new PlayerUnit { PlayerId = player.PlayerId });
                PostUpdateCommands.AddComponent(unit, new MoveTo
                {
                    position = new float3(random.NextFloat(-10f, 10f), 0, random.NextFloat(-10f, 10f)),
                    moveSpeed = 5f,
                    move = true
                });
            }

            PostUpdateCommands.RemoveComponent<PlayerConfig>(entity);
        });
    }
}

public struct PlayerConfig : IComponentData
{
    public int Units;
}

