using Unity.Entities;

partial struct HealthComponent : IComponentData
{
    public int maxHealth;
    public int currentHealth;
    
}

/// <summary>
/// Stores the combined damaged from ALL sources this frame
/// </summary>
public struct DamageBuffer : IBufferElementData
{
    public int Value;
}