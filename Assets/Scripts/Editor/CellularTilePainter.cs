using UnityEditor;
using UnityEngine;

public class CellularTilePainter : ScriptableObject
{
    [MenuItem("Tools/Cellular/TilePainter")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
}