using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyHandling
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        private class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
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
                        maxEnemySpawnQuantity = authoring.MaxEnemySpawns
                    }
                );
                AddComponent(entity, new FactoryTimer { value = authoring.RandomInitialDuration ? UnityEngine.Random.Range(0f, authoring.Duration) : authoring.Duration });
            }
        }
        public GameObject Prefab;
        public float2 SpawnOffset = new float2(0,0);
        public float Duration = 1f;
        public int SpawnCount = 1;
        public bool RandomInitialDuration = true;
        public float MaxEnemySpawns = 10;
        public static float MaxGlobalEnemySpawns = 100;
    }
}