using EnemyHandling;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameManagement
{
    public class GameManagerAuthoring : MonoBehaviour
    {
        private class GameManagerBaker : Baker<GameManagerAuthoring>
        {
            public override void Bake(GameManagerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent
                (
                    entity,
                    new GlobalEnemySpawnerData
                    {
                        globalEnemySpawns = 0,
                        globalEnemyPopulation = 0,
                        maxGlobalEnemyPopulation = authoring.MaxGlobalEnemyPopulation
                    }
                );
            }
        }
        public int MaxGlobalEnemyPopulation;
    }
}