using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BulletSystem))]
public partial struct PlayerSystem : ISystem
{
    private Entity playerEntity;
    private Entity inputEntity;
    private EntityManager manager;
    private PlayerComponent playerComponent;
    private InputComponent inputComponent;

    public void OnUpdate(ref SystemState state)
    {
        manager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        playerComponent = manager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponent = manager.GetComponentData<InputComponent>(inputEntity);

        Move(ref state);
        Shoot(ref state);
    }

    private void Move(ref SystemState state) {
        if (!manager.HasComponent<LocalTransform>(playerEntity)){ manager.AddComponent<LocalTransform>(playerEntity); }
        LocalTransform playerTransform = manager.GetComponentData<LocalTransform>(playerEntity);

        playerTransform.Position += new float3(inputComponent.movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0);

        Vector2 lookDirection = (Vector2)inputComponent.mouse - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(lookDirection.y, lookDirection.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        manager.SetComponentData(playerEntity, playerTransform);
    }
    private float nextShootTime;

    [BurstCompile]
    private void Shoot(ref SystemState state)
    {
        if(inputComponent.shoot && nextShootTime < SystemAPI.Time.ElapsedTime)
        {
            var ecb = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                             .CreateCommandBuffer(state.WorldUnmanaged);

            LocalTransform playerTransform = manager.GetComponentData<LocalTransform>(playerEntity);

         /*    var bullet = state.EntityManager.CreateEntity(stackalloc ComponentType[]
            {
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadWrite<AnimationTimer>(),
                ComponentType.ReadOnly<FirstFrameTag>(),
            });

            var transform = state.EntityManager.GetComponentData<LocalTransform>(bullet);

            SystemAPI.SetComponent(bullet, LocalTransform.FromPositionRotation(
                playerTransform.Position + playerTransform.Right() + playerTransform.Up() * -.5f,
                playerTransform.Rotation
            ));

            var animation = state.EntityManager.GetComponentData<AnimationTimer>(bullet);
            animation.value = SystemAPI.Time.ElapsedTime;

            SystemAPI.SetComponent(bullet, animation);
            state.EntityManager.Instantiate(bullet);
 */
            Entity bulletEntity = ecb.Instantiate(playerComponent.BulletPrefab);
            
            
            var bulletTransform = LocalTransform.FromPositionRotation(
                playerTransform.Position + playerTransform.Right() + playerTransform.Up() * -.5f, 
                playerTransform.Rotation
            );
            
            ecb.SetComponent(bulletEntity, bulletTransform);
            ecb.AddComponent(bulletEntity, new AnimationTimer { value = SystemAPI.Time.ElapsedTime });
            ecb.AddComponent<SpawnedThisFrameTag>(bulletEntity);

            nextShootTime = (float)SystemAPI.Time.ElapsedTime + playerComponent.ShootCooldown;
        }
    }
}

