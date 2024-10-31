using UnityEngine;
using Unity.Entities;

public  class PlayerAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public float ShootCooldown;
    public float Score;
    public GameObject BulletPrefab;

    public class PlayerBaker : Baker <PlayerAuthoring> {

        public override void Bake(PlayerAuthoring authoring)
        {
            Entity player = GetEntity(TransformUsageFlags.None);
            AddComponent(player, new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                ShootCooldown = authoring.ShootCooldown,
                Score = authoring.Score,
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None)
            });
        }
    }
}

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public float ShootCooldown;
    public float Score;
    public Entity BulletPrefab;
}

