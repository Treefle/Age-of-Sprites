using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]
public partial struct FixedStepAnimationTimerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                         .CreateCommandBuffer(state.WorldUnmanaged);

        var currentTime = SystemAPI.Time.ElapsedTime;

        foreach (var (timer, entity) in SystemAPI.Query<RefRW<AnimationTimer>>()
            .WithEntityAccess()
            .WithAll<SpawnedThisFrameTag>())
        {
            timer.ValueRW.value = currentTime;
            ecb.RemoveComponent<SpawnedThisFrameTag>(entity);
        }
    }
} 