using System.Collections.Generic;
using UnityEngine;

namespace EnemyHandling
{
    [CreateAssetMenu(fileName = "EnemyTypeAndIDTable", menuName = "ScriptableObjects/EnemyTypeAndIDTable")]
    public class SO_EnemyTypeAndIDTable : ScriptableObject
    {
        [System.Serializable]
        public struct EnemyTypeAndID
        {
            //public SO_EnemyType enemyType;
            public int enemyID;
        }
        //[ser]
        //public Dictionary<int, SO_EnemyType> enemyTypesAndIDs;
    }
}