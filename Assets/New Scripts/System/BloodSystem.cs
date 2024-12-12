/*using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BloodSystem : ISystem
{
    ComponentLookup<BloodComponent> bloodLookup;

    public void OnCreate(ref SystemState state)
    {
        bloodLookup = state.GetComponentLookup<BloodComponent>(isReadOnly: false);
    }


    private EntityManager manager;
    void ISystem.OnUpdate(ref SystemState state)
    {

        bloodLookup.Update(ref state);

        manager = state.EntityManager;
        EntityCommandBuffer ECB = new(Allocator.Temp);

        foreach (var (blood, entity) in SystemAPI.Query<RefRW<BloodComponent>>().WithEntityAccess())
        {
            if (blood.ValueRO.currentDuration > blood.ValueRO.Lifetime)
            {
                ECB.RemoveComponent<AnimationTimer>(entity);
                //LocalTransform bloodTransform = manager.GetComponentData<LocalTransform>(entity);
                if (blood.ValueRO.currentDuration > blood.ValueRO.Lifetime + 2)
                {
                    ECB.DestroyEntity(entity);
                }
            }

            blood.ValueRW.currentDuration += SystemAPI.Time.DeltaTime;

        }

        ECB.Playback(manager);
    }
}
*/
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using System.Diagnostics;

[BurstCompile]
public partial struct BloodSystem : ISystem
{
    private ComponentLookup<BloodComponent> bloodLookup;
    private ComponentTypeHandle<BloodComponent> bloodTypeHandle;
    private ComponentTypeHandle<LocalTransform> localTransformTypeHandle;
    private EntityTypeHandle entityType;
    private EntityQuery entityQuery;

    public void OnCreate(ref SystemState state)
    {
        bloodLookup = state.GetComponentLookup<BloodComponent>(isReadOnly: false);
        bloodTypeHandle = state.GetComponentTypeHandle<BloodComponent>(isReadOnly: false);
        localTransformTypeHandle = state.GetComponentTypeHandle<LocalTransform>(isReadOnly: false);
        entityType = state.GetEntityTypeHandle();

        state.GetEntityTypeHandle();

        entityQuery = state.GetEntityQuery(
            ComponentType.ReadWrite<BloodComponent>(),
            ComponentType.ReadWrite<LocalTransform>()
            );
    }

    [BurstCompile]
    private struct BloodJob : IJobChunk
    {
        public float deltaTime;
        public ComponentTypeHandle<BloodComponent> bloodTypeHandle;
        public ComponentTypeHandle<LocalTransform> localTransformTypeHandle;
        public EntityTypeHandle entityType;
        public EntityCommandBuffer.ParallelWriter ecb;


        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var bloods = chunk.GetNativeArray(ref bloodTypeHandle);
            var transforms = chunk.GetNativeArray(ref localTransformTypeHandle);
            var entities = chunk.GetNativeArray(entityType);

            for (int i = 0; i < chunk.Count; i++)
            {
                var blood = bloods[i];
                var transform = transforms[i];
                var entity = entities[i];

                if (blood.currentDuration > blood.Lifetime)
                {
                    if (chunk.Has<AnimationTimer>())
                    {
                        ecb.RemoveComponent<AnimationTimer>(unfilteredChunkIndex, entity);
                    }
                          
                    transform.Scale = math.max(transform.Scale - deltaTime * .1f, 0); // Reduce size each frame
                    transforms[i] = transform; // Update the transform

                    if (blood.currentDuration > blood.Lifetime + 10)
                    {
                        ecb.DestroyEntity(unfilteredChunkIndex, entity);
                    }
                }

                blood.currentDuration += deltaTime;
                bloods[i] = blood;
            }
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        bloodLookup.Update(ref state);
        bloodTypeHandle.Update(ref state);
        localTransformTypeHandle.Update(ref state);
        entityType.Update(ref state);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var job = new BloodJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            bloodTypeHandle = bloodTypeHandle,
            localTransformTypeHandle = localTransformTypeHandle,
            entityType = entityType,
            ecb = ecb.AsParallelWriter()
    };

        state.Dependency = job.ScheduleParallel(entityQuery, state.Dependency);

        state.CompleteDependency();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}


