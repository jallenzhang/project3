using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonNextTrail : MonoBehaviour 
{
	public List<GameObject> trailPrefabs;
	public SplineTrailRenderer trailRef;

	int currentTrailIndex = 0;
	SplineTrailRenderer currentTrail = null;

	void Start () 
	{
		SwitchTrail();
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width / 2  - 40, 35, 80, 25), "Next Trail"))
		{
			SwitchTrail();
		}
	}

	void SwitchTrail()
	{
		if(trailPrefabs.Count > 0 && ++currentTrailIndex >= trailPrefabs.Count)
		{
			currentTrailIndex = 0;
		}

		if(currentTrailIndex < trailPrefabs.Count)
		{
			if(currentTrail != null)
			{
				Destroy(currentTrail.gameObject);
			}

			currentTrail = (Instantiate(trailPrefabs[currentTrailIndex]) as GameObject).GetComponent<SplineTrailRenderer>();
			trailRef.ImitateTrail(currentTrail);
		}
	}
}
