using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
using Unity.Entities;
using Unity.Mathematics;
using ProjectDawn.Navigation;
using Unity.Transforms;

public class AgentSetDestination : MonoBehaviour
{
    public GameObject Target;
    void Start()
    {
        GetComponent<AgentAuthoring>().SetDestination(Target.transform.position);
    }
}

// ECS component
public struct SetDestination : IComponentData
{
    public float3 Value;
}

// Bakes mono component into ecs component
class AgentSetDestinationBaker : Baker<AgentSetDestination>
{
    public override void Bake(AgentSetDestination authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),
            new SetDestination { Value = authoring.Target.transform.position });
    }
}

// Sets agents destination
partial struct AgentSetDestinationSystem : ISystem
{
    public void OnUpdate(ref SystemState systemState)
    {
        Vector3 target = new Vector3();
        foreach (var (transform, body, player) in SystemAPI.Query< RefRO<LocalTransform>, RefRO<AgentBody>, RefRO<PlayerComponent> >())
        {
            target = transform.ValueRO.Position;
        }
        foreach (var (destination, body) in SystemAPI.Query< RefRW<SetDestination>, RefRW<AgentBody> >())
        {
            body.ValueRW.SetDestination(target);
        }
    }
}