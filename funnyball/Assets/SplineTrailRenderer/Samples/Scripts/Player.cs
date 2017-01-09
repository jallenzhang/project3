using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float speed;
	public SplineTrailRenderer trailReference;
	public Animation anim;

	private float distance = 0;
	private Vector3 lastPosition;

	void Start()
	{
		lastPosition = transform.position;
	}
	
	void Update () 
	{
		float length = trailReference.spline.Length(); 

		distance = Mathf.Clamp(distance + speed * Time.deltaTime, 0, length-0.1f);
		trailReference.maxLength = Mathf.Max(length - distance, 0);

		Vector3 forward = trailReference.spline.FindTangentFromDistance(distance);
		Vector3 position = trailReference.spline.FindPositionFromDistance(distance);

		if(forward != Vector3.zero)
		{
			if(anim != null)
			{
				if(lastPosition == position)
				{
					anim.CrossFade("Idle");
				}
				else
				{
					anim.CrossFade("Walk");
				}
			}

			transform.forward = forward;
			transform.position = lastPosition = position;
		}
	}
}
