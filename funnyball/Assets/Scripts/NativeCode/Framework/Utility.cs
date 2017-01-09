using System.Collections;
using UnityEngine;

internal class CoroutineManagerMonoBehaviour : MonoBehaviour
{
}

public class CoroutineManager
{
    private static CoroutineManagerMonoBehaviour _CoroutineManagerMonoBehaviour;

    static CoroutineManager()
    {
        Init();
    }

    public static void DoCoroutine(IEnumerator routine)
    {
        _CoroutineManagerMonoBehaviour.StartCoroutine(routine);
    }

    private static void Init()
    {
        var go = new GameObject();
        go.name = "CoroutineManager";
        _CoroutineManagerMonoBehaviour = go.AddComponent<CoroutineManagerMonoBehaviour>();
        GameObject.DontDestroyOnLoad(go);
    }
}