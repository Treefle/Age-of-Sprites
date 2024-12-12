

using Unity.Entities;
using UnityEngine;

public class BloodAuthoring : MonoBehaviour
{
    public float Lifetime;
    public float currentDuration;

    public class BloodBaker : Baker<BloodAuthoring>
    {
        public override void Bake(BloodAuthoring authoring)
        {
            Entity blood = GetEntity(TransformUsageFlags.None);
            AddComponent(blood, new BloodComponent()
            {
                currentDuration = authoring.currentDuration,
                Lifetime = authoring.Lifetime
            });
        }
    }
}

partial struct BloodComponent : IComponentData
{
    public float currentDuration;
    public float Lifetime;
}