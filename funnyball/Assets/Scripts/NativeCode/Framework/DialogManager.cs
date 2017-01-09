using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NullParam : DialogParam
{

}

public class DialogManager : MonoBehaviour {

	private const int BASE_DEPTH = 20;

	private int currentMaxDepth = BASE_DEPTH;

	private Stack<GameObject> dialogQueue = new Stack<GameObject>();

	private GameObject maskPanel;

	public GameObject MaskPanel {
		get {
			return maskPanel;
		}
	}

	private GameObject messagePrefab;

	private static DialogManager _instance;
	public static DialogManager Instance {
		get {
			if (_instance == null) {
				GameObject go = GameObject.FindGameObjectWithTag("DialogManager");
				if (go != null) _instance = go.GetComponent<DialogManager>();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		_instance = this;
	}

	public static DialogManager GetInstance() {
		return Instance;
	}

    public GameObject PopupDialog(string prefab, bool instant = true)
    {
        return PopupDialog<NullParam>(prefab, null, instant);
    }

	public GameObject PopupDialog<T>(string prefab, T param, bool instant = true) where T : DialogParam {
		return PopupDialog(CommonAsset.Load(prefab) as GameObject, param, instant);
	}

	public GameObject PopupDialog<T>(GameObject prefab, T param, bool instant = true) where T : DialogParam {
		GameObject go = UGUITools.AddChild(gameObject, prefab);
		if (go.GetComponent<DialogBase>() != null) go.GetComponent<DialogBase>().style = DialogStyle.NormalDialog;
		PushDialog(go, param, instant);

		return go;
	}

	private void AdjustPanelDepth(GameObject go, bool increase = true)
	{
        //UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
        //if (panels == null || panels.Length == 0) return;

        //int minDepth = int.MaxValue;
        //int maxDepth = int.MinValue;
        //foreach (UIPanel p in panels)
        //{
        //    if (p.depth < minDepth) minDepth = p.depth;
        //    if (p.depth > maxDepth) maxDepth = p.depth;
        //}

        //if (increase)
        //{
        //    foreach (UIPanel p in panels)
        //    {
        //        p.depth = p.depth - minDepth + currentMaxDepth;
        //    }

        //    currentMaxDepth = currentMaxDepth + maxDepth - minDepth + 10;
        //}
        //else
        //{
        //    currentMaxDepth = minDepth;
        //}
	}

	private void PushDialog<T>(GameObject go, T param, bool instant) where T : DialogParam
	{
		try {
			var newDialog = go.GetComponent<BaseDialog<T>>();
			if (newDialog != null)
			{
				newDialog.Init(param);
			}
		} catch (Exception e) {
			Debug.LogError(e);
		}
		
		dialogQueue.Push (go);
	}

	public GameObject GetTopDialog(){
		return dialogQueue.Peek ();
	}

	public int GetDialogCountInScreen()
	{
		return dialogQueue.Count;
	}

	public void CloseDialog()
	{
		if (dialogQueue.Count <= 0) {
			return;
		}

		GameObject go = dialogQueue.Pop ();
        Destroy(go);
	}

	public void ClearAll()
	{
		while (dialogQueue.Count > 0) 
		{
			CloseDialog();
		}
		currentMaxDepth = BASE_DEPTH;
	}
}
