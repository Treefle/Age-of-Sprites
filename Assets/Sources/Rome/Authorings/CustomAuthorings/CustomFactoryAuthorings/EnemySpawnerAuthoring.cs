using EnemyHandling;
using Identifiers;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

//Don't put authoring class in a namespace, for some reason, it makes Unity Editor unable to see it as a component
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        private class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                authoring.EnemyTypeID = IDGenerator.GenerateID(authoring.enemyTypeName);
            AddComponent
            (
                entity,
                new EnemySpawnerData
                {
                    prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None),
                    instantiatePos = new float2(authoring.transform.position.x, authoring.transform.position.y) + authoring.SpawnOffset,
                    count = authoring.SpawnCount,
                    duration = authoring.Duration,
                    enemySpawnQuantity = 0,
                    maxEnemySpawnQuantity = authoring.MaxEnemySpawns,
                    enemyTypeID = authoring.EnemyTypeID,
                }
            );
                AddComponent(entity, new FactoryTimer { value = authoring.RandomInitialDuration ? UnityEngine.Random.Range(0f, authoring.Duration) : authoring.Duration });
            }
        }
        public GameObject Prefab;
        public Guid enemyGuid;
        public float2 SpawnOffset = new float2(0,0);
        public float Duration = 1f;
        public int SpawnCount = 1;
        public bool RandomInitialDuration = true;
        public float MaxEnemySpawns = 10;
        public static float MaxGlobalEnemySpawns = 100;
        public string enemyTypeName;
        public int EnemyTypeID;
    }
