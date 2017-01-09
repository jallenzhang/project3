/*
Copyright (c) 2014 David Stier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


******Jello Physics was born out of Walabers JellyPhysics. As such, the JellyPhysics license has been include.
******The original JellyPhysics library may be downloaded at http://walaber.com/wordpress/?p=85.


Copyright (c) 2007 Walaber

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Sticky demo camera.
/// </summary>
public class StickyDemoCamera : MonoBehaviour {

	/// <summary>
	/// The target body to follow when zoomed in.
	/// </summary>
	public Transform target;

	/// <summary>
	/// The orthographic camera size when zoomed in.
	/// </summary>
	public float zoomedInSize;
	
	/// <summary>
	/// The orthographic camera size when zoomed out.
	/// </summary>
	public float zoomedOutSize;

	/// <summary>
	/// The state of the cam.
	/// </summary>
	private CameraState camState = CameraState.zoomedOut;
	private enum CameraState { zoomedIn, zoomedOut };

	/// <summary>
	/// Are we transitioning between camera states.
	/// </summary>
	private bool transitioning;

	/// <summary>
	/// The zoom button text.
	/// </summary>
	private string zoomText = "Zoom Out";

	// Use this for initialization
	void Start () 
	{
		//set appropriate zoom when starting.
		if(camState == CameraState.zoomedIn)
		{
			zoomText = "Zoom Out";
			StartCoroutine(SmoothChangeSize(zoomedInSize));
		}
		else
		{
			zoomText = "Zoom In";
			StartCoroutine(SmoothChangeSize(zoomedOutSize));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//exit if no target is assigned
		if(target == null)
			return;

		//follow the targets x and y positions.
		transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
	}

	void OnGUI()
	{
		//zoom button
		if(GUILayout.Button(zoomText) && !transitioning)
		{
			if(camState == CameraState.zoomedIn)
			{
				zoomText = "Zoom In";
				camState = CameraState.zoomedOut;
				StartCoroutine(SmoothChangeSize(zoomedOutSize));
			}
			else
			{
				zoomText = "Zoom Out";
				StartCoroutine(SmoothChangeSize(zoomedInSize));
				camState = CameraState.zoomedIn;
			}
		}
	}
	
	/// <summary>
	/// Zoom in or out smoothly
	/// </summary>
	/// <returns>IEnumerator.</returns>
	/// <param name="newSize">New size.</param>
	IEnumerator SmoothChangeSize(float newSize)
	{
		if(transitioning)//don't process if already processing.
			yield break;

		transitioning = true;//procesing.
		float duration = 1f;//the amount of time it takes to transition from the current zoom to the new zoom.
		float oldSize = Camera.main.orthographicSize;
		while(duration > 0f)
		{
			duration -= Time.deltaTime;

			//apply portion of zoom
			Camera.main.orthographicSize = newSize + duration * (oldSize - newSize);
			yield return null;
		}

		transitioning = false;//process complete.
	}
}
