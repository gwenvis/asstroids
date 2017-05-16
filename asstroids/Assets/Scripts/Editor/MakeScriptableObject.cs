using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/My Scriptable Object")]
    public static void CreateMyAsset()
    {
        GameWeapons asset = ScriptableObject.CreateInstance<GameWeapons>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}