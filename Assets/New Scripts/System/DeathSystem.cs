using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace EnemyHandling
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    public partial struct DeathSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Complete any previous jobs
            state.Dependency.Complete();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (deathTag, entity) in
               SystemAPI.Query<RefRO<DiesThisFrameTag>>().WithEntityAccess())
            {
                ecb.RemoveComponent<DiesThisFrameTag>(entity);
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}