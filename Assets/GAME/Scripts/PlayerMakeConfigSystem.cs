using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class PlayerMakeConfigSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerConfig>().ForEach((Entity entity, ref PlayerConfig config) =>
        {
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
            var ghostId = MobileRTSGhostSerializerCollection.FindGhostType<CubeSnapshotData>();
            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;

            for (int i = 0; i < config.Units; i++)
            {
                var unit = EntityManager.Instantiate(prefab);
            }

            PostUpdateCommands.RemoveComponent<PlayerConfig>(entity);
        });
    }
}
