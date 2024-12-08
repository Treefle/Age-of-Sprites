using Unity.Entities;
using Unity.Mathematics;

namespace EnemyHandling
{
    public struct FactoryData : IComponentData
    {
        //What we are producing
        public Entity prefab;
        //How many to produce each time this factory executes
        public int count;
        //How long between factory executions
        public float duration;
        //Where we put the thing we are producing
        public float2 instantiatePos;
    }
    public struct EnemySpawnerData : IComponentData
    {
        //What we are producing
        public Entity prefab;
        //How many to produce each time this factory executes
        public int count;
        //How long between factory executions
        public float duration;
        //Where we put the thing we are producing
        public float2 instantiatePos;
        //How many THIS factory has produced
        public float enemySpawnQuantity;
        //How many THIS factory should produce
        public float maxEnemySpawnQuantity;

    }
}