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
/// Eye blink.
/// Changes sprites of the eyes in different patterns to create a blinking effect.
/// </summary>
public class EyeBlink : MonoBehaviour 
{
	/// <summary>
	/// The minimum blink frequency.
	/// </summary>
	public float minBlinkFrequency = 2f;

	/// <summary>
	/// The max blink frequency.
	/// </summary>
	public float maxBlinkFrequency = 8f;

	/// <summary>
	/// The renderers.
	/// one for each eye.
	/// </summary>
	public SpriteRenderer[] renderers;

	/// <summary>
	/// The sprites used for the blink animation.
	/// </summary>
	public Sprite[] sprites;

	/// <summary>
	/// The frame delay.
	/// Deleay between switching images for the animation.
	/// </summary>
	public float frameDelay = 0.05f;

	/// <summary>
	/// Whether the eyes are in the middle of a blink pattern coroutine.
	/// </summary>
	private bool blinking;
	
	// Use this for initialization 
	void Start () 
	{
		//Do nothing if sprite and renderers havnet been assigned in the editor
		if(renderers == null || sprites == null)
			return;

		//start blinking!
		StartCoroutine(BlinkTimer());
	}

	/// <summary>
	/// Starts a new blink pattern at random intervals.
	/// </summary>
	/// <returns>IEnumerator.</returns>
	IEnumerator BlinkTimer()
	{
		while(true)
		{
			//select a blink pattern
			StartCoroutine(BlinkSelector());

			//wait a random time before being allowed to blink again.
			yield return new WaitForSeconds(Random.Range(minBlinkFrequency, maxBlinkFrequency));
		}
	}

	/// <summary>
	/// Selects a blink pattern
	/// </summary>
	/// <returns>IEnumerator.</returns>
	IEnumerator BlinkSelector()
	{
		//dont allow multiple blink patterns to be executed at once.
		if(blinking)
			yield break;

		//we are blinking!
		blinking = true;

		//choose one of three blink patterns at random and wait for the coroutine to complete
		int selector = Random.Range(1,3);
		switch(selector)
		{
		case 1: yield return StartCoroutine(BlinkPatternOne()); break;
		case 2: yield return StartCoroutine(BlinkPatternTwo()); break;
		case 3: yield return StartCoroutine(BlinkPatternThree()); break;
		}

		//blink pattern complete
		blinking = false;
	}

	/// <summary>
	/// Blink pattern one.
	/// close then open both eyes
	/// </summary>
	/// <returns>The pattern one.</returns>
	IEnumerator BlinkPatternOne()
	{
		//close eyes
		for(int i = 1; i < sprites.Length; i++)
		{
			for(int r = 0; r < renderers.Length; r++)
				renderers[r].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}
		
		//then open again
		for(int i = sprites.Length - 2; i >= 0; i--)
		{
			for(int r = 0; r < renderers.Length; r++)
				renderers[r].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}
	}

	/// <summary>
	/// Blink pattern two.
	/// close both eyes, pause, then open again
	/// </summary>
	/// <returns>The pattern two.</returns>
	IEnumerator BlinkPatternTwo()
	{
		//close eyes partially
		for(int i = 1; i < sprites.Length - 1; i++)
		{
			for(int r = 0; r < renderers.Length; r++)
				renderers[r].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}

		//pause
		yield return new WaitForSeconds(2f);

		//then open again
		for(int i = sprites.Length - 3; i >= 0; i--)
		{
			for(int r = 0; r < renderers.Length; r++)
				renderers[r].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}
	}

	/// <summary>
	/// Blink pattern three.
	/// close both eyes, pause, open one eye, pause, then open the other
	/// </summary>
	/// <returns>The pattern three.</returns>
	IEnumerator BlinkPatternThree()
	{
		//close eyes
		for(int i = 1; i < sprites.Length - 1; i++)
		{
			for(int r = 0; r < renderers.Length; r++)
				renderers[r].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}

		//pause
		yield return new WaitForSeconds(2f);

		//open one eye
		for(int i = sprites.Length - 3; i >= 0; i--)
		{
			renderers[1].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}

		//pause
		yield return new WaitForSeconds(2f);

		//then open the other
		for(int i = sprites.Length - 3; i >= 0; i--)
		{
			renderers[0].sprite = sprites[i];
			yield return new WaitForSeconds(frameDelay);
		}
	}
}
