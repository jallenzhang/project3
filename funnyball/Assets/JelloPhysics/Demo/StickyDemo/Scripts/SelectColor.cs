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
/// Select color.
/// </summary>
public class SelectColor : MonoBehaviour {

	/// <summary>
	/// The selectable colors.
	/// </summary>
	public Color[] SelectableColors;

	/// <summary>
	/// The background colors.
	/// </summary>
	public Color[] BackgroundColors;

	/// <summary>
	/// The selectable colliders.
	/// </summary>
	public BoxCollider2D[] selectableColliders;

	/// <summary>
	/// The target whos image will be changed to match the newly selected color.
	/// </summary>
	public GameObject target;

	/// <summary>
	/// The sprite mesh link attached to our target body.
	/// </summary>
	public SpriteMeshLink meshLink;

	/// <summary>
	/// The index of the sprites to correspond with our color change.
	/// </summary>
	public int[] spriteIndices;

	/// <summary>
	/// The sprites who's color we will also change.
	/// </summary>
	public SpriteRenderer[] sprites;

	/// <summary>
	/// The particle systems who's color we will also change.
	/// </summary>
	public ParticleSystem[] particleSystems;

	/// <summary>
	/// The selected collider.
	/// </summary>
	private int selectedCollider = -1;

	// Use this for initialization
	void Start () 
	{
		//assign our mesh link.
		if(target != null)
			meshLink = target.GetComponent<SpriteMeshLink>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//upon left mouse click
		if(Input.GetMouseButton(0))
		{
			//exit if we didnt assign everything required in the inspector
			if(selectableColliders == null || target == null)
				return;

			//global mouse position
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			for(int i = 0; i < selectableColliders.Length; i++)
			{
				//skip overlap check if currently selected
				if(i == selectedCollider)
					continue;

				//if our mouse is within a selectable collider, change all of the colors to that index.
				if(selectableColliders[i].OverlapPoint(mousePosition))
					ChangeColor(i);
			}
		}
	}

	/// <summary>
	/// Changes the colors.
	/// </summary>
	/// <param name="selectedIndex">Index of selected color.</param>
	void ChangeColor(int selectedIndex)
	{
		//exit if invalid index
		if(selectedIndex > selectableColliders.Length || selectedIndex < 0f)
			return;

		//update the selected collider index.
		selectedCollider = selectedIndex;

		//change our body sprite.
		meshLink.SelectSprite(spriteIndices[selectedIndex]);

		//change eye colors
		for(int i = 0; i < sprites.Length; i++)
			sprites[i].color = SelectableColors[selectedIndex];
		//change eye particle emitter colors.
		for(int i = 0; i < particleSystems.Length; i++)
			particleSystems[i].startColor = SelectableColors[selectedIndex];
		//change skybox 
		Camera.main.backgroundColor = BackgroundColors[selectedIndex];
	}
}



