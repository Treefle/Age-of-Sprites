using GameManagement;
using Identifiers;
using UnityEditor;
using UnityEngine;

namespace CustomEditorStuff
{
    [CustomEditor(typeof(GameManagerEnemyTypeIDHandler))]
    public class GabeCustomEditor_GameManagerEnemyTypeIDTablePopulator : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameManagerEnemyTypeIDHandler myScript = (GameManagerEnemyTypeIDHandler)target;

            if (GUILayout.Button("Clear EnemyTypeID_Dictionary"))
            {
                IDGenerator.ClearEnemyTypeNameAndTypeID();
            }
            if (GUILayout.Button("Print EnemyTypeID_Dictionary"))
            {
                IDGenerator.NotifyDictionaryUpdated(IDGenerator.EnemyTypeNameAndTypeID);
            }
        }
    }
}