using Unity.Entities;

namespace EnemyHandling
{
    //A singleton component to store information about the factories, as pertains to the game state.
    public struct GlobalEnemySpawnerData : IComponentData
    {
        //How many enemies ALL factories have produced
        public int globalEnemySpawns;
        //How many enemies are currently alive
        public int globalEnemyPopulation;
        //How many enemies are allowed to exist simultaneously
        public int maxGlobalEnemyPopulation;
    }
}