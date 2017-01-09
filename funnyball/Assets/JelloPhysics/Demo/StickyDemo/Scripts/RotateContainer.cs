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
/// Rotate container.
/// Rotates the gameobject. 
/// Allows for randomized rotation and directly controlled rotation.
/// </summary>
public class RotateContainer : MonoBehaviour 
{
	/// <summary>
	/// Whether or not a GUI element will be displayed to control the Rotation.
	/// </summary>
	public bool GUIControllable = false;

	/// <summary>
	/// The rotate speed.
	/// </summary>
	public float RotateSpeed = 10f;

	/// <summary>
	/// Whether or not to randomize the rotation.
	/// </summary>
	public bool randomize;

	/// <summary>
	/// The max rotation speed.
	/// </summary>
	public float MaxRotationSpeed = 20;

	/// <summary>
	/// The minimum rotation speed.
	/// </summary>
	public float MinRotationSpeed = 0;

	/// <summary>
	/// The max duration of the rotation.
	/// </summary>
	public float MaxRotationDuration = 10;

	/// <summary>
	/// The minimum duration of the rotation.
	/// </summary>
	public float MinRotationDuration = 1;

	/// <summary>
	/// The transform being rotated
	/// </summary>
	Transform xform;

	// Use this for initialization
	void Start () 
	{
		//assign xform reference.
		xform = transform;

		if(randomize)
			StartCoroutine(RotationSpeedRandomizer());
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		//apply new rotation.
		xform.localEulerAngles = new Vector3(xform.localEulerAngles.x, xform.localEulerAngles.y, xform.localEulerAngles.z + RotateSpeed * Time.fixedDeltaTime);
	}

	void OnGUI()
	{	
		if(GUIControllable)
		{
			GUILayout.Space(80f);

			bool oldValue = randomize;
			randomize = GUILayout.Toggle(randomize, "Randomize Ring Rotation");
			if(randomize && oldValue != randomize)
			{
				//if randomize value is changed from false to true, start the randomize coroutine
				StartCoroutine(RotationSpeedRandomizer());
			}

			Rect r = GUILayoutUtility.GetRect(40, 20);
			if(!randomize)
			{
				//Horizontal scrollbar to directly affect the rotation speed.
				RotateSpeed = GUI.HorizontalSlider(r, RotateSpeed, -20, 20);
			}
		}
	}

	/// <summary>
	/// Rotation speed randomizer.
	/// </summary>
	/// <returns>IEnumerator</returns>
	IEnumerator RotationSpeedRandomizer()
	{
		while(randomize)
		{
			//change rotation speeds smoothly
			StartCoroutine(SmoothChangeRotationSpeed());

			//wait for a random duration then change velocity again.
			yield return new WaitForSeconds(Random.Range(MinRotationDuration, MaxRotationDuration));
		}
	}

	/// <summary>
	/// Smoothly changes the rotation speed over one second.
	/// </summary>
	/// <returns>IEnumerator.</returns>
	IEnumerator SmoothChangeRotationSpeed()
	{
		//store the old rotation speed
		float oldRotateSpeed = RotateSpeed;
		//generate new random rotation speed
		float newRotateSpeed = Random.Range(MinRotationSpeed, MaxRotationSpeed);

		//over the duration of one second.
		float transitionDuration = 1f;
		while (transitionDuration > 0)
		{
			transitionDuration -= Time.deltaTime;

			//transition from the old duration to the new random duration.
			RotateSpeed = newRotateSpeed + transitionDuration * (oldRotateSpeed - newRotateSpeed);

			yield return null;
		}
	}
}
