using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HealthBarAuthoring : MonoBehaviour
{
    public GameObject barVisualGameObject;
    public GameObject enemyGameObject;
    private class HealthBarBaker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring){
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar
            {
                barVisualEntity = GetEntity(authoring.barVisualGameObject, TransformUsageFlags.NonUniformScale),
                enemyEntity = GetEntity(authoring.enemyGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct HealthBar: IComponentData{
    public Entity barVisualEntity;
    public Entity enemyEntity;
}