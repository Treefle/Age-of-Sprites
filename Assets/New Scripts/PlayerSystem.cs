using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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

        playerTransform.Position += new Unity.Mathematics.float3(inputComponent.movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0);

        Vector2 lookDirection = (Vector2)inputComponent.mouse - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(lookDirection.y, lookDirection.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        manager.SetComponentData(playerEntity, playerTransform);
    }
    private float nextShootTime;
    private void Shoot(ref SystemState state)
    {
        if(inputComponent.shoot && nextShootTime < SystemAPI.Time.ElapsedTime)
        {
            EntityCommandBuffer ECB = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            Entity bulletEntity = manager.Instantiate(playerComponent.BulletPrefab);

            ECB.AddComponent(bulletEntity, new BulletComponent { Speed = 5, Size = 0.25f });

            LocalTransform bulletTransform = manager.GetComponentData<LocalTransform>(bulletEntity);
            bulletTransform.Rotation = manager.GetComponentData<LocalTransform>(playerEntity).Rotation;
            LocalTransform playerTransform = manager.GetComponentData<LocalTransform>(playerEntity);
            bulletTransform.Position = playerTransform.Position + playerTransform.Right() + playerTransform.Up() * -.025f; // 
            ECB.SetComponent(bulletEntity, bulletTransform);

            ECB.Playback(manager);

            nextShootTime = (float)SystemAPI.Time.ElapsedTime + playerComponent.ShootCooldown;
        }

    }
}

