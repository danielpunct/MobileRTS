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

            var firstPlayer = player.PlayerId == 1;
            var spawnPoint = firstPlayer ? ServerReferences.Instance.spawnA : ServerReferences.Instance.spawnB;

            for (int i = 0; i < config.Civilians; i++)
            {
                SpawnCivilian(player.PlayerId, firstPlayer, spawnPoint);
            }
            for (int i = 0; i < config.Archers; i++)
            {
                SpawnArcher(player.PlayerId, firstPlayer, spawnPoint);
            }

            PostUpdateCommands.RemoveComponent<PlayerConfig>(entity);
        });
    }

    void SpawnCivilian(int playerId, bool firstPlayer, Transform spawnPoint)
    {
        var ghostId = firstPlayer
            ? MobileRTSGhostSerializerCollection.FindGhostType<ACivilianSnapshotData>()
            : MobileRTSGhostSerializerCollection.FindGhostType<BCivilianSnapshotData>();
        var prefab = GetPrefab(ghostId);

        var unit = EntityManager.Instantiate(prefab);
        var spawnOffset = new float3(spawnPoint.transform.position) { y = 0 };

        PostUpdateCommands.SetComponent(unit, new PlayerUnit { PlayerId = playerId });
        PostUpdateCommands.AddComponent(unit, new MoveTo
        {
            position = spawnOffset + new float3(random.NextFloat(-10f, 10f), 0, random.NextFloat(-10f, 10f)),
            moveSpeed = 5f,
            move = true
        });
    }

    void SpawnArcher(int playerId, bool firstPlayer, Transform spawnPoint)
    {
        var ghostId = firstPlayer
            ? MobileRTSGhostSerializerCollection.FindGhostType<AArcherSnapshotData>()
            : MobileRTSGhostSerializerCollection.FindGhostType<BArcherSnapshotData>();
        var prefab = GetPrefab(ghostId);

        var unit = EntityManager.Instantiate(prefab);
        var spawnOffset = new float3(spawnPoint.transform.position) { y = 0 };

        PostUpdateCommands.SetComponent(unit, new PlayerUnit { PlayerId = playerId });
        PostUpdateCommands.AddComponent(unit, new MoveTo
        {
            position = spawnOffset + new float3(random.NextFloat(-10f, 10f), 0, random.NextFloat(-10f, 10f)),
            moveSpeed = 6f,
            move = true
        });
        PostUpdateCommands.AddComponent(unit, new Attack
        {
            AttackRadius = 50,
            AttackedAt = 0
        });

    } 


    Entity GetPrefab(int ghostId)
    {
        var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
        return EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
    }
}

public struct PlayerConfig : IComponentData
{
    public int Civilians;
    public int Archers;
}

