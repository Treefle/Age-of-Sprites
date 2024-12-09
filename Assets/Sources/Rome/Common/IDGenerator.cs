using EnemyHandling;
using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Identifiers
{
    public static class IDGenerator 
    {
        public static event EventHandler<DictionaryUpdateEventArgs> EnemyTypeNameAndTypeID_WasUpdated;
        public static Dictionary<string, int> EnemyTypeNameAndTypeID = new Dictionary<string, int> { };
        static int id = 0;
        public static int GenerateID(string EnemyTypeName)
        {
            if (EnemyTypeNameAndTypeID.ContainsKey(EnemyTypeName)){
                id = EnemyTypeNameAndTypeID[EnemyTypeName];
                return id; }
            else
            {
                id = EnemyTypeNameAndTypeID?.Count ?? 0; // If list is null, count will be 0
                EnemyTypeNameAndTypeID.Add(EnemyTypeName, id);
                NotifyDictionaryUpdated(EnemyTypeNameAndTypeID);
            }       
            return id;
        }
        public static void NotifyDictionaryUpdated(Dictionary<string,int> updatedDictionary)
        {
            DictionaryUpdateEventArgs args = new DictionaryUpdateEventArgs(updatedDictionary);
            EnemyTypeNameAndTypeID_WasUpdated?.Invoke(null, args);
            foreach (var kvp in EnemyTypeNameAndTypeID)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        public static void ClearEnemyTypeNameAndTypeID()
        {
            EnemyTypeNameAndTypeID.Clear();
            NotifyDictionaryUpdated(EnemyTypeNameAndTypeID);
        }
    }
    public class DictionaryUpdateEventArgs : EventArgs
    {
        public Dictionary<string, int> UpdatedDictionary { get; }

        public DictionaryUpdateEventArgs(Dictionary<string, int> updatedDictionary)
        {
            UpdatedDictionary = updatedDictionary;
        }
    }

}