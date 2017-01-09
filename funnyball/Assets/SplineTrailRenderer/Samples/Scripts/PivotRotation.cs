using UnityEngine;
using System.Collections;

public class PivotRotation : MonoBehaviour 
{
	public Vector3 rotationAxis = Vector3.up;
	public float rotationSpeed = 1f;
	
	void Update () 
	{
		transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
	}
}
