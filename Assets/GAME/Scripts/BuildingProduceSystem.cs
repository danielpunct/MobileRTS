using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public class BuildingProduceSystem : JobComponentSystem
{

    Unity.Mathematics.Random random = new Unity.Mathematics.Random(0x6E624EB7u);

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);
        var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
        var ghostId = MobileRTSGhostSerializerCollection.FindGhostType<ArrowSnapshotData>();
        var arrowPrefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
        var time = UnityEngine.Time.time;

        Entities.WithoutBurst().ForEach((
            Entity Entity,
            ref Archery archery,
            in PlayerUnit playerUnit,
            in Translation translation) =>
        {
            var produced = time - archery.ProducedAt < 10;

            if (produced)
            {
                return;
            }

            var firstPlayer = playerUnit.PlayerId == 1;
            SpawnArcher(ECB, playerUnit.PlayerId, firstPlayer, translation.Value + new float3(5, 0, 0));

            archery.ProducedAt = time;

        }).Run();
        
        ECB.Playback(EntityManager);
        ECB.Dispose();

        return default;
    }

    void SpawnArcher(EntityCommandBuffer ecb, int playerId, bool firstPlayer, float3 spawnPosition)
    {
        var ghostId = firstPlayer
            ? MobileRTSGhostSerializerCollection.FindGhostType<AArcherSnapshotData>()
            : MobileRTSGhostSerializerCollection.FindGhostType<BArcherSnapshotData>();
        var prefab = GetPrefab(ghostId);

        var unit = ecb.Instantiate(prefab);
        var spawnOffset = new float3(spawnPosition) { y = 0 };

        ecb.SetComponent(unit, new PlayerUnit
        {
            PlayerId = playerId,
            UnitId = 2
        });
        var position = spawnOffset + new float3(random.NextFloat(-10f, 10f), 0, random.NextFloat(-10f, 10f));

        ecb.AddComponent(unit, new MoveTo
        {
            position = position,
            moveSpeed = 6f,
            move = true
        });
        ecb.SetComponent(unit, new Translation
        {
            Value = position
        });
        ecb.AddComponent(unit, new Attack
        {
            AttackRadius = 50,
            AttackedAt = 0
        });
        ecb.SetComponent(unit, new Health { Value = 50 });
    }

    Entity GetPrefab(int ghostId)
    {
        var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
        return EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
    }
}


public struct Archery : IComponentData
{
    public float ProducedAt;
}
