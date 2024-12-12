using GameManagement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace EnemyHandling
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    //[UpdateAfter(typeof(AnimationTimerSystem))]
    public partial struct FactorySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Complete any previous jobs
            state.Dependency.Complete();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Handle production directly without jobs for now
            foreach (var (timer, factoryData) in
                SystemAPI.Query<RefRW<FactoryTimer>, RefRO<FactoryData>>())
            {
                timer.ValueRW.value -= SystemAPI.Time.DeltaTime;

                if (timer.ValueRW.value <= 0)
                {
                    timer.ValueRW.value += factoryData.ValueRO.duration;

                    for (int i = 0; i < factoryData.ValueRO.count; i++)
                    {
                        var instance = ecb.Instantiate(factoryData.ValueRO.prefab);
                        ecb.SetComponent(instance, LocalTransform.FromPosition(
                            factoryData.ValueRO.instantiatePos.ToFloat3()));
                        ecb.AddComponent<FirstFrameTag>(instance);
                    }
                }
            }

            // Playback and cleanup
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }


        public partial struct EnemySpawner : ISystem
        {
            [BurstCompile]
            public void OnCreate(ref SystemState state)
            {
                state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            }

            [BurstCompile]
            public void OnUpdate(ref SystemState state)
            {
                // Complete any previous jobs
                state.Dependency.Complete();

                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                var gameManagerEntity = SystemAPI.GetSingletonEntity<GlobalEnemySpawnerData>();
                var globalEnemySpawnerData = state.EntityManager.GetComponentData<GlobalEnemySpawnerData>(gameManagerEntity);
                // Handle production directly without jobs for now
                foreach (var (timer, enemySpawnerData) in
                    SystemAPI.Query<RefRW<FactoryTimer>, RefRW<EnemySpawnerData>>())
                {
                    timer.ValueRW.value -= SystemAPI.Time.DeltaTime;

                    if (timer.ValueRW.value <= 0 && WeShouldProduceMoreEnemies(enemySpawnerData, globalEnemySpawnerData))
                    {
                        timer.ValueRW.value += enemySpawnerData.ValueRO.duration;

                        for (int i = 0; i < enemySpawnerData.ValueRO.count; i++)
                        {
                            var instance = ecb.Instantiate(enemySpawnerData.ValueRO.prefab);
                            ecb.SetComponent(instance, LocalTransform.FromPosition(
                                enemySpawnerData.ValueRO.instantiatePos.ToFloat3()));
                            ecb.AddComponent<FirstFrameTag>(instance);
                            ecb.AddComponent(instance, new EnemyType {   
                                    ID = enemySpawnerData.ValueRO.enemyTypeID, 
                                });

                            int health = 100;
                            ecb.AddComponent(instance, new HealthComponent
                            {
                                maxHealth = health,
                                currentHealth = health
                            });

                            ecb.AddBuffer<DamageBuffer>(instance);

                            enemySpawnerData.ValueRW.enemySpawnQuantity++;
                            globalEnemySpawnerData.globalEnemySpawns++;
                            globalEnemySpawnerData.globalEnemyPopulation++;
                            ecb.SetComponent(gameManagerEntity, globalEnemySpawnerData);
                        }
                    }
                }
                
                // Playback and cleanup
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }

            private static bool WeShouldProduceMoreEnemies(RefRW<EnemySpawnerData> enemySpawnerData, GlobalEnemySpawnerData globalEnemySpawnerData)
            {
                bool shouldProduceMoreEnemies = true;
                if(enemySpawnerData.ValueRO.enemySpawnQuantity >= enemySpawnerData.ValueRO.maxEnemySpawnQuantity){
                    shouldProduceMoreEnemies = false;}
                else if (globalEnemySpawnerData.globalEnemyPopulation >= globalEnemySpawnerData.maxGlobalEnemyPopulation)
                {
                    shouldProduceMoreEnemies = false;
                }
                return shouldProduceMoreEnemies;
            }
        }
    }
}