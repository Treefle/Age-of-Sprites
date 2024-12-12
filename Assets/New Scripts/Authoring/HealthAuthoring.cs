
using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    private class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            authoring.currentHealth = authoring.maxHealth;

            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new HealthComponent {
                maxHealth = authoring.maxHealth,
                currentHealth = authoring.currentHealth
            });
        }
    }

    public int maxHealth;
    int currentHealth;
}
