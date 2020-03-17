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

            var firstPlayer = player.PlayerId == 1;
            var ghostId = firstPlayer ? MobileRTSGhostSerializerCollection.FindGhostType<AArcherSnapshotData>() : MobileRTSGhostSerializerCollection.FindGhostType<BArcherSnapshotData>();
            var spawnPoint = firstPlayer ? ServerReferences.Instance.spawnA : ServerReferences.Instance.spawnB;


            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;

            for (int i = 0; i < config.Units; i++)
            {
                var unit = EntityManager.Instantiate(prefab);
                var spawnOffset = new float3(spawnPoint.transform.position);
                spawnOffset.y = 0;

                PostUpdateCommands.SetComponent(unit, new PlayerUnit { PlayerId = player.PlayerId });
                PostUpdateCommands.AddComponent(unit, new MoveTo
                {
                    position = spawnOffset + new float3(random.NextFloat(-10f, 10f), 0, random.NextFloat(-10f, 10f)),
                    moveSpeed = 5f,
                    move = true
                });
                PostUpdateCommands.AddComponent(unit, new Attack
                {
                    AttackRadius = 50,
                    AttackedAt = 0
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

