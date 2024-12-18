using System.Numerics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        UnityEngine.Vector3 cameraForward = UnityEngine.Vector3.zero;
        if(Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }
        foreach ((
            RefRW<LocalTransform>localTransform,
            RefRO<HealthBar> healthBar)
            in SystemAPI.Query <
                RefRW<LocalTransform>, 
                RefRO<HealthBar>>())
        {

            HealthComponent health = SystemAPI.GetComponent<HealthComponent>(healthBar.ValueRO.enemyEntity);
            float healthNormalized = (float)health.currentHealth / health.maxHealth;

           // if (healthNormalized == 1f){
           //     localTransform.ValueRW.Scale = 0f;
           // }
           // else{ localTransform.ValueRW.Scale = 1f;}
            localTransform.ValueRW.Scale = 1f;

            RefRW<PostTransformMatrix>barVisualPostTransformMatrix = 
                SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}
