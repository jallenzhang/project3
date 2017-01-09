using UnityEngine;
using System.Collections;

public enum DialogStyle
{
	NormalDialog,
	NotificationDialog,
}

public class DialogBase : MonoBehaviour {
	public DialogStyle style = DialogStyle.NormalDialog;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(GameObject obj, string param)
	{
		if (obj != null) {
			InitWithParam(obj);		
		}

		if (param != null)
		{
			InitWithParam(param);
		}
		initUI ();
	}

	protected virtual void InitWithParam(GameObject obj)
	{

	}

	protected virtual void InitWithParam(string param)
	{
	}

	protected virtual void initUI()
	{

	}

	public virtual bool IsFullScreen() {
		return true;
	}

	/// <summary>
	/// called when dialog covered by other dialog
	/// </summary>
	public virtual void OnPause() {
	}

	/// <summary>
	/// called when dialog become visiable
	/// </summary>
	public virtual void OnResume() {
	}


}

public class NotFullDialog : DialogBase {

	public override bool IsFullScreen() {
		return false;
	}

}
