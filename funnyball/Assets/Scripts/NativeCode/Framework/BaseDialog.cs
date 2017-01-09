using UnityEngine;
using System.Collections;

// TODO: this class should inherit from MonoBehaviour where all dialog modify complete
public abstract class BaseDialog<T> : DialogBase where T : DialogParam {

	T param;

	// param should be readonly
	public T Param {
		get {
			return param;
		}
	}

	public void Init(T param) {
		this.param = param;
	}

}

public abstract class DialogParam {
}

public class DialogParam<T> : DialogParam {
	public T param;
}

public class DialogParam<T1, T2> : DialogParam {
	public T1 param1;
	public T2 param2;
}

public class DialogParam<T1, T2, T3> : DialogParam {
	public T1 param1;
	public T2 param2;
	public T3 param3;
}