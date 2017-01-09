using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorTools : MonoBehaviour
{

    [MenuItem("Tools/Save Mesh")]
    public static void SaveMesh()
    {
        Mesh m = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(m, "Assets/cmbMesh.asset");
        AssetDatabase.SaveAssets();
    }
}