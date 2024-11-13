using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

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
}