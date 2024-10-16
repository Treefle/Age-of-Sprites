using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;

public partial struct BulletSystem : ISystem
{
    private void OnUpdate(ref SystemState state)
    {
        EntityManager manager = state.EntityManager;
        NativeArray<Entity> allEntities = manager.GetAllEntities();

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach(Entity entity in allEntities)
        {
            if(manager.HasComponent<BulletComponent>(entity))
            {
                LocalTransform bulletTransform = manager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = manager.GetComponentData<BulletComponent>(entity);
                bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * bulletTransform.Right();
                manager.SetComponentData(entity, bulletTransform);

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorld.SphereCastAll(bulletTransform.Position, bulletComponent.Size / 2, float3.zero, 1,
                    ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });

                foreach(ColliderCastHit hit in hits)
                {
                    manager.SetEnabled(hit.Entity, false);
                }

                hits.Dispose();
            }
        }
    }
}

public enum CollisionLayer
{
    Default = 1 << 0,
    Enemy = 1 << 6
}
partial struct BulletComponent : IComponentData
{
    public float Speed;
    public float Size;
}