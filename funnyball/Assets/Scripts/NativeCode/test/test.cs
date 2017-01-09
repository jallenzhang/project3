using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class test : MonoBehaviour {
    public GameObject target;

    private Vector3 targetPos;
    private Vector3 revertPos;
    public float speed = 1;
	// Use this for initialization
	void Start () {
        revertPos = transform.localPosition;
        targetPos = transform.localPosition + Vector3.up * 100;

        ItemData data = ItemData.GetItemDataByID(1);
        Debug.Log(data.dRemarks);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "revert"))
        {
            targetPos = revertPos;
        }
    }



}
