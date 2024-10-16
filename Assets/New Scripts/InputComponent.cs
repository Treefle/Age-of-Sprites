using Unity.Mathematics;
using Unity.Entities;

public struct InputComponent : IComponentData
{
    public float2 movement;
    public float2 mouse;
    public bool shoot;
}
