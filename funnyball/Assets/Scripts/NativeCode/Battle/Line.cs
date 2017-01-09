using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour {
    LineRenderer lineRender;
	// Use this for initialization
	void Start () {
        lineRender = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateStartPoint(Vector3 pos)
    {
        if (lineRender == null)
            lineRender = GetComponent<LineRenderer>();

        pos.z = lineRender.transform.position.z;// +.1f;
        lineRender.SetPosition(0, pos);
    }

    public void UpdateEndPoint(Vector3 pos)
    {
        if (lineRender == null)
            lineRender = GetComponent<LineRenderer>();

        pos.z = lineRender.transform.position.z;// +.1f;
        lineRender.SetPosition(1, pos);
    }
}
