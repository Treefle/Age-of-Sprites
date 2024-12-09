using Identifiers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement
{
    public class GameManagerEnemyTypeIDHandler : MonoBehaviour
    {
        public static Dictionary<string, int> EnemyTypeNameAndTypeID;
        // Start is called before the first frame update
        void Start()
        {
            EnemyTypeNameAndTypeID = new Dictionary<string, int>(IDGenerator.EnemyTypeNameAndTypeID);
            IDGenerator.EnemyTypeNameAndTypeID_WasUpdated += OnEnemyTypeIDsUpdated;
        }

        public void OnEnemyTypeIDsUpdated(object sender, DictionaryUpdateEventArgs e)
        {
            EnemyTypeNameAndTypeID = e.UpdatedDictionary;
        }
    }
}