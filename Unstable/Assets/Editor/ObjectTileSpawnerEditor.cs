using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ObjectTileSpawner))]
public class ObjectTileSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectTileSpawner tileSpawner = (ObjectTileSpawner) target;

        if (GUILayout.Button("Spawn Tiles"))
            tileSpawner.SpawnTiles();
        if (GUILayout.Button("Remove Tiles"))
            tileSpawner.RemoveTiles();
    }
}
