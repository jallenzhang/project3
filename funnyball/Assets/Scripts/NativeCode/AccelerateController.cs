using UnityEngine;
using System.Collections;

public class AccelerateController : MonoBehaviour {
    private float lowPassFilterFactor = 15f / 360f;

    private readonly Quaternion baseIdentity = Quaternion.Euler(0, 0, 0);

    private readonly Vector3 landscapeHorization = new Vector3(0, 3, 0);
    private readonly Vector3 landscapeVertical = new Vector3(2, 0, 0);

    private Quaternion referanceRotationX = Quaternion.identity;
    private Quaternion referanceRotationY = Quaternion.identity;

    private bool accelerateEnable = false;
	// Use this for initialization
	void Start () {
        EnableAccelerate();
	}
	
	// Update is called once per frame
	void Update () {
	    if (accelerateEnable)
        {
            referanceRotationX = Quaternion.Euler(landscapeHorization * Input.acceleration.x);
            referanceRotationY = Quaternion.Euler(landscapeVertical * Input.acceleration.y);
            transform.rotation = Quaternion.Slerp(transform.rotation, referanceRotationX, lowPassFilterFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, referanceRotationY, lowPassFilterFactor);
        }
	}

    private void EnableAccelerate()
    {
        accelerateEnable = true;
    }
}
