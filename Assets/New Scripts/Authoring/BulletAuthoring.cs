
using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float Speed;
    public float Size;
    public float Lifetime;
    public float currentDuration;
    public int Damage;
    public GameObject OnHitPrefab;

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity bullet = GetEntity(TransformUsageFlags.None);
            AddComponent(bullet, new BulletComponent
            {
                Speed = authoring.Speed,
                Size = authoring.Size,
                Lifetime = authoring.Lifetime,
                currentDuration = authoring.currentDuration,
                Damage = authoring.Damage,
                OnHitPrefab = GetEntity(authoring.OnHitPrefab, TransformUsageFlags.None)
            });
        }
    }

}
public partial struct BulletComponent : IComponentData
{
    public float Speed;
    public float Size;
    public float Lifetime;
    public float currentDuration;
    public int Damage;
    public Entity OnHitPrefab;
}