using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaskController : MonoBehaviour {
    public GameObject m_ball;

    private float m_width = 568f;
    private float m_height = 320f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float offsetx = -(m_ball.transform.localPosition.x) / (m_width * 2f); //offset / 0.5  = x / (1136 / 2)
        float offsety = -(m_ball.transform.localPosition.y) / (m_height * 2f);

        GetComponent<Image>().material.SetTextureOffset("_Mask", new Vector2(offsetx, offsety));
	}
}
