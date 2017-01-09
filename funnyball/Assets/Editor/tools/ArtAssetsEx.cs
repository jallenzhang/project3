using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
public class ArtAssetsEx : EditorWindow
{
    #region Init
    static List<string> _filePrefabPath = new List<string>();
    static List<string> _fileAssetsPath = new List<string>();
    static List<PrefabDependencies> _prefabs = new List<PrefabDependencies>();
    static List<string> _readyToFind;
    static bool _path;
    static public AssetUsedData _AssetUsedData;
    [MenuItem("Tools/FindAssetsUsed")]
    public static void Init()
    {
        EditorWindow.GetWindow<ArtAssetsEx>();
        _filePrefabPath.Add(Application.dataPath);
        Find();
    }
    //        [MenuItem("Assets/设为Find Art Prefab Path")]
    //        public static void AddFilePrefabPath(){
    //                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //                path = path.Replace ("Assets", "");
    //                _filePrefabPath.Add(Application.dataPath + path);
    //        }
    //        [MenuItem("Assets/设为Find Art Assets Path")]
    //        public static void AddFileAssetsPath(){
    //                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //                path = path.Replace ("Assets", "");
    //                _fileAssetsPath.Add(Application.dataPath + path);
    //        }
    [MenuItem("Assets/设为Find Use Prefab")]
    public static void FindUsePrefab()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        _AssetUsedData = new AssetUsedData(path);
        _AssetUsedData.FindUsed(_prefabs);
    }
    void OnGUI()
    {
        _path = EditorGUILayout.Foldout(_path, "路径");
        if (_path)
        {
            GUILayout.Label("预制路径");
            for (int i = 0; i < _filePrefabPath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    _filePrefabPath.RemoveAt(i);
                    i++;
                }
                else
                {
                    GUILayout.Label(_filePrefabPath[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Label("资源路径");
            for (int i = 0; i < _fileAssetsPath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    _fileAssetsPath.RemoveAt(i);
                    i++;
                }
                else
                {
                    GUILayout.Label(_fileAssetsPath[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        if (_prefabs != null)
        {
            EditorGUILayout.LabelField("当前目录下的预制数目:" + _prefabs.Count);
        }
        //              if (GUILayout.Button ("Find"))
        //                    Find ();
        ShowTest();

        Repaint();
    }
    Vector2 pos;
    void ShowTest()
    {
        pos = EditorGUILayout.BeginScrollView(pos);
        //                if (_readyToFind != null) {
        //                        for(int i = 0;i < _readyToFind.Count;i++){
        //                                EditorGUILayout.LabelField(_readyToFind[i]);
        //                        }
        //                }
        //
        //                if (_prefabs != null) {
        //                        for(int i = 0;i < _prefabs.Count;i++){
        //
        //                                EditorGUILayout.LabelField(_prefabs[i]._prefabPath);
        //                                _prefabs[i]._showDependencies = EditorGUILayout.Toggle(_prefabs[i]._showDependencies);
        //                                if(_prefabs[i]._showDependencies && _prefabs[i]._dependencies!=null){
        //                                        for(int x = 0;x<_prefabs[i]._dependencies.Count;x++){
        //                                                EditorGUILayout.LabelField(_prefabs[i]._dependencies[x]);
        //                                        }
        //                                }
        //                        }
        //                }
        if (_AssetUsedData != null)
            _AssetUsedData.OnGUI();
        EditorGUILayout.EndScrollView();
    }
    #endregion
    #region Find
    static void Find()
    {
        if (_filePrefabPath == null || _filePrefabPath.Count == 0)
            _filePrefabPath.Add(Application.dataPath);
        _readyToFind = new List<string>();
        _prefabs = new List<PrefabDependencies>();
        for (int i = 0; i < _filePrefabPath.Count; i++)
        {
            FindAllPath(_filePrefabPath[i]);
        }

        CreatePrefabData();
        RestPrefabDependencie();
    }
    static void CreatePrefabData()
    {
        for (int i = 0; i < _readyToFind.Count; i++)
        {
            string expandname = Path.GetExtension(_readyToFind[i]);
            if (expandname == ".prefab" || expandname == ".unity")
            {
                PrefabDependencies pd = new PrefabDependencies(_readyToFind[i]);
                _prefabs.Add(pd);
            }
        }
    }
    static void RestPrefabDependencie()
    {
        for (int i = 0; i < _prefabs.Count; i++)
        {
            _prefabs[i].GetDependencies();
            if (EditorUtility.DisplayCancelableProgressBar("获取索引", "GetDependencie:" + i, (float)i / _prefabs.Count))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
        }
        EditorUtility.ClearProgressBar();
    }
    static void FindAllPath(string path)
    {
        string[] Directorys = new string[0];
        try
        {
            Directorys = Directory.GetFiles(path);
        }
        catch { }
        if (Directorys != null)
        {
            for (var i = 0; i < Directorys.Length; i++)
            {
                if (!_readyToFind.Contains(Directorys[i]))
                    _readyToFind.Add(Directorys[i]);
            }
        }
        Directorys = Directory.GetDirectories(path);
        for (int i = 0; i < Directorys.Length; i++)
        {
            string newpath;
            newpath = Path.GetDirectoryName(Directorys[i]) + "/" + Path.GetFileName(Directorys[i]);
            FindAllPath(newpath);
        }
    }
    #endregion
}
public class AssetUsedData
{
    public AssetUsedData(string path)
    {
        _path = path;
    }
    public string _path;
    public List<Object> _usedPrefab = new List<Object>();
    public void FindUsed(List<PrefabDependencies> prefabs)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i]._dependencies.Contains(_path))
            {
                _usedPrefab.Add(AssetDatabase.LoadAssetAtPath<Object>(prefabs[i]._prefabPath));
            }
        }
    }
    public void OnGUI()
    {
        for (int i = 0; i < _usedPrefab.Count; i++)
        {
            EditorGUILayout.ObjectField(_usedPrefab[i], typeof(GameObject));
        }
    }
}
public class PrefabDependencies
{
    public PrefabDependencies(string path)
    {
        _prefabPath = "Assets" + path.Replace(Application.dataPath, "");
        _prefabPath = _prefabPath.Replace("\\", "/");

    }
    //.prefab or unity
    public string _prefabPath;
    public List<string> _dependencies;
    public bool _showDependencies;
    public void GetDependencies()
    {
        string[] path = new string[1];
        path[0] = _prefabPath;
        string[] paths = AssetDatabase.GetDependencies(path);
        _dependencies = new List<string>();
        for (int i = 0; i < paths.Length; i++)
        {
            _dependencies.Add(paths[i]);
        }
    }
}