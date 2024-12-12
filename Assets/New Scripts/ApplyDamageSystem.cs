using GameManagement;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace EnemyHandling
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ApplyDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.RequireForUpdate<LateSimulationSystemGroup>();
            state.RequireForUpdate<DamageBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Complete any previous jobs
            //state.Dependency.Complete();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (health, entity) in
               SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
            {
                DynamicBuffer<DamageBuffer> damageBuffer = state.EntityManager.GetBuffer<DamageBuffer>(entity);
                int totalDamageThisFrame = 0;
                for(int i = 0; i < damageBuffer.Length; i++)
                {
                    var damage = damageBuffer[i];
                    totalDamageThisFrame += damage.Value;
                }

                if(health.ValueRO.currentHealth < totalDamageThisFrame)
                {
                    health.ValueRW.currentHealth = 0;
                }
                else
                {
                    health.ValueRW.currentHealth -= totalDamageThisFrame;
                }

                // trigger death
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}