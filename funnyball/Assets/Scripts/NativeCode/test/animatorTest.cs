using UnityEngine;
using System.Collections;

public class animatorTest : MonoBehaviour {
    public Animator button;
    public Animator setting;

	// Use this for initialization
	void Start () {
        button.enabled = false;
        setting.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onSetting()
    {
        button.enabled = true;
        setting.enabled = true;
        button.SetBool("isHidden", true);
        setting.SetBool("isHidden", true);
    }
}
