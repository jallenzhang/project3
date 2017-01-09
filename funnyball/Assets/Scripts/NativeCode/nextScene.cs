using UnityEngine;
using System.Collections;

public class nextScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "scene 1"))
        {
            Application.LoadLevel(0);
        }

        if (GUI.Button(new Rect(10, 70, 100, 30), "scene 2"))
        {
            Application.LoadLevel(2);
        }
    }
}
