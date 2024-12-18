using EnemyHandling;
using ProjectDawn.Navigation;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateBefore(typeof(AgentSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct BulletSystem : ISystem
{
    private EntityQuery bulletQuery;
    ComponentLookup<LocalTransform> transformLookup;
    private ComponentLookup<BulletComponent> bulletLookup;

    public void OnCreate(ref SystemState state)
    {
        transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: false);
        bulletLookup = state.GetComponentLookup<BulletComponent>(true);

        bulletQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<BulletComponent>(),
            ComponentType.ReadWrite<LocalTransform>()
        );

    }

    [BurstCompile]
    void ISystem.OnUpdate(ref SystemState state)
    {
        // Add this at the start of OnUpdate to ensure previous jobs are complete
        state.Dependency.Complete();

        // Update lookups
        transformLookup.Update(ref state);
        bulletLookup.Update(ref state);

        var spatial = GetSingleton<AgentSpatialPartitioningSystem.Singleton>();
        var ecb = GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        var uniqueEntities = new UnsafeParallelHashSet<Entity>(256, Allocator.TempJob);

        // Perform spatial queries outside the job
        var bulletEntities = bulletQuery.ToEntityArray(Allocator.TempJob);
        var bulletPositions = new NativeArray<float3>(bulletEntities.Length, Allocator.TempJob);

        Entity player = GetSingletonEntity<PlayerComponent>();
        var playerComponent = GetComponentRW<PlayerComponent>(player);

        Entity gameManagerEntity = GetSingletonEntity<GlobalEnemySpawnerData>();
        var globalEnemySpawnerData = state.EntityManager.GetComponentData<GlobalEnemySpawnerData>(gameManagerEntity);

        for (int i = 0; i < bulletEntities.Length; i++)
        {
            var entity = bulletEntities[i];
            
            // Check if entity still exists before accessing components
            if (!state.EntityManager.Exists(entity))
                continue;

            var bulletComponent = bulletLookup[entity];

            if(bulletComponent.currentDuration > bulletComponent.Lifetime)
            {
                ecb.DestroyEntity(entity);
                continue; // Skip further processing for this entity
            }

            // Check if entity still has required components
            if (!transformLookup.HasComponent(entity))
                continue;

            var transform = transformLookup[entity];
            bulletPositions[i] = transform.Position;

            bulletComponent.currentDuration += Time.DeltaTime;

            SetComponent(entity, bulletComponent);

            // HIT STUFF
            var action = new CollisionAction { } ;
            spatial.QueryCircle(transform.Position, bulletComponent.Size, spatial.QueryCapacity, ref action, NavigationLayers.Default);

            if (action.Target != Entity.Null && action.Target != player)
            {
                if (uniqueEntities.Add(action.Target))
                {
                    if(state.EntityManager.Exists(action.Target) && transformLookup.HasComponent(action.Target))
                    {
                        // valid target 

                        var enemyTransform = transformLookup[action.Target];

                        DynamicBuffer<DamageBuffer> damageBuffer = state.EntityManager.GetBuffer<DamageBuffer>(action.Target);

                        damageBuffer.Add(new DamageBuffer { Value = bulletComponent.Damage });

                        playerComponent.ValueRW.Score = playerComponent.ValueRO.Score + 1;
                        globalEnemySpawnerData.globalEnemyPopulation--;
                        ecb.SetComponent(gameManagerEntity, globalEnemySpawnerData);

                        Entity splatterEntity = ecb.Instantiate(bulletComponent.OnHitPrefab);
                        ecb.SetComponent(splatterEntity, enemyTransform);
                        ecb.AddComponent(splatterEntity, new AnimationTimer { value = Time.ElapsedTime });
                        ecb.AddComponent<SpawnedThisFrameTag>(splatterEntity);
                    }
                }
            }

        }
        SetComponent(player, playerComponent.ValueRO);


        // Schedule the job to handle bullet updates
        var bulletJob = new BulletJob
        {
            deltaTime = Time.DeltaTime,
            ecb = ecb.AsParallelWriter(),
            positions = bulletPositions,
            bulletEntities = bulletEntities,
            bulletLookup = bulletLookup,
            transformLookup = transformLookup,
            uniqueEntities = uniqueEntities,
        };
        state.Dependency = bulletJob.Schedule(bulletEntities.Length, 32, state.Dependency);
        state.Dependency.Complete();

        // Write back positions to transforms
        for (int i = 0; i < bulletEntities.Length; i++)
        {
            var entity = bulletEntities[i];
            var transform = transformLookup[entity];
            transform.Position = bulletPositions[i];
            transformLookup[entity] = transform;
        }



        bulletEntities.Dispose();
        bulletPositions.Dispose();
        uniqueEntities.Dispose();
    }

    [BurstCompile]
    partial struct BulletJob : IJobParallelFor
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;
        public NativeArray<float3> positions;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public UnsafeParallelHashSet<Entity> uniqueEntities;
        [ReadOnly] public NativeArray<Entity> bulletEntities;
        [ReadOnly] public ComponentLookup<BulletComponent> bulletLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> transformLookup;

        public void Execute(int index)
        {
            var entity = bulletEntities[index];
            
            // Skip if entity no longer exists or doesn't have required components
            if (!transformLookup.HasComponent(entity) || !bulletLookup.HasComponent(entity))
                return;

            var bullet = bulletLookup[entity];
            var transform = transformLookup[entity];

            // Move forward based on the local right vector in 2D
            positions[index] += transform.Right() * bullet.Speed * deltaTime;

            // Consider moving entity destruction to main thread or using ECB properly
            foreach (var ue in uniqueEntities)
            {
                if (ue != entity) // Avoid destroying self
                {
                   // ecb.DestroyEntity(index, ue);
                }
            }
        }
    }
}

public struct CollisionAction : ISpatialQueryEntity
{
    public Entity Target;
    public float3 Position;
    public void Execute(Entity entity, AgentBody body, AgentShape shape, LocalTransform transform)
    {
        if (entity == Target)
            return;

        Target = entity;
        Position = transform.Position;
    }
}




/*public enum CollisionLayer
    {
        Default = 1 << 0,
        Enemy = 1 << 6
    }*/