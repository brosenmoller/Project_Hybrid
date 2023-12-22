#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Set Room Index"))
        {
            gameManager.SetRoomIndex();
        }

        if (GUILayout.Button("Delete All Saves"))
        {
            gameManager.DeleteAllSaves();
        }
    }
}

#endif

