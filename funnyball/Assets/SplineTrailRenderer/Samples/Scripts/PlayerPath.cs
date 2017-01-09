using UnityEngine;
using System.Collections;

public class PlayerPath : MonoBehaviour 
{
	public SplineTrailRenderer trailReference;
	public string groundLayerName = "Water";
	public string playerLayerName = "Default";
	public Vector3 trailOffset = new Vector3(0, 0.02f, 0);

	private bool playerSelected = false;
	
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

			if(Physics.Raycast(ray, out hit, float.MaxValue, LayerNameToIntMask(playerLayerName)))
			{
				playerSelected = true;
				MoveOnFloor();
				trailReference.Clear();
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			playerSelected = false;
		}

		if(Input.GetMouseButton(0) && playerSelected)
		{
			MoveOnFloor();
		}
	}

	void MoveOnFloor()
	{
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, 
			Input.mousePosition.y, 0)), out hit, float.MaxValue, LayerNameToIntMask(groundLayerName)))
		{
			trailReference.transform.position = hit.point + trailOffset;
		}
	}

	static int LayerNameToIntMask(string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);

		if(layer == 0)
			return int.MaxValue;

		return 1 << layer;
	}
}
