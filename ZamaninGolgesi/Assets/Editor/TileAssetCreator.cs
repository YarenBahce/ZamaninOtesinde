#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileAssetCreator
{
    [MenuItem("Assets/Create/Custom/Basic Tile")]
    public static void CreateBasicTile()
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        AssetDatabase.CreateAsset(tile, "Assets/Tiles/GroundTile.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;
    }
}
#endif