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
/// The SpriteMeshLink links Unity's built in Texture2D objects to JelloBody objects for Mesh deformation.
/// </summary>
public class TextureMeshLink : MeshLink 
{
	/// <summary>
	/// The Texture2D to link.
	/// </summary>
	public Texture2D texture;

	/// <summary>
	/// Initialize the MeshLink.
	/// </summary>
	/// <param name="forceUpdate">Whether to force an update to the MeshLink and MeshLink.LinkedMeshFilter.sharedMesh.</param>
	public override void Initialize(bool forceUpdate = false)
	{
		base.Initialize();

		meshLinkType = MeshLinkType.TextureMeshLink;

		if(texture == null)
		{
			Debug.LogWarning("No texture found. exiting operation.");
			return;
		}

		if(LinkedMeshRenderer.sharedMaterial == null)
			LinkedMeshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
		
		if(LinkedMeshRenderer.sharedMaterial.mainTexture == null)
			LinkedMeshRenderer.sharedMaterial.mainTexture = texture;

		if(forceUpdate || LinkedMeshFilter.sharedMesh.vertexCount != body.Shape.VertexCount)
		{
			
			if(LinkedMeshFilter.sharedMesh.vertexCount != body.Shape.VertexCount)
				LinkedMeshFilter.sharedMesh.Clear();
			
			vertices = new Vector3[body.Shape.VertexCount];
			for(int i = 0; i < vertices.Length; i++)
				vertices[i] = (Vector3)body.Shape.getVertex(i);

			Vector2[] uvPts = new Vector2[body.Shape.VertexCount];

			for(int i= 0; i < uvPts.Length; i++)
			{
				uvPts[i] = body.Shape.getVertex(i) - pivotOffset;
				uvPts[i] = JelloVectorTools.rotateVector(uvPts[i], angle);
				uvPts[i] = new Vector2(uvPts[i].x / scale.x, uvPts[i].y / scale.y);
				uvPts[i] -= offset;
			}
		
			LinkedMeshFilter.sharedMesh.vertices = vertices;
			LinkedMeshFilter.sharedMesh.uv = uvPts;
			LinkedMeshFilter.sharedMesh.triangles = body.Shape.Triangles;

			if(CalculateNormals)
				LinkedMeshFilter.sharedMesh.RecalculateNormals();
			if(CalculateTangents)
				calculateMeshTangents();
			
			LinkedMeshFilter.sharedMesh.RecalculateBounds();
			LinkedMeshFilter.sharedMesh.Optimize();
			LinkedMeshFilter.sharedMesh.MarkDynamic();
		}
	}


	/// <summary>
	/// Update the MeshLink.LinkedMeshFilter.sharedMesh.vertices.
	/// This is called each Update() and drives the continuous MeshLink.LinkedMeshFilter.sharedMesh deformation.
	/// </summary>
	/// <param name="points">The basis of the new MeshLink.LinkedMeshFilter.sharedMesh.vertices.</param>
	public override void UpdateMesh (Vector2[] points)
	{
		if(vertices.Length != points.Length)
			vertices = new Vector3[points.Length];
		
		for(int i = 0; i < vertices.Length; i++)
			vertices[i] = (Vector3)points[i];
		
		LinkedMeshFilter.sharedMesh.vertices = vertices;
		LinkedMeshFilter.sharedMesh.RecalculateBounds();
	}

	/// <summary>
	/// Update the pivot point.
	/// </summary>
	/// <param name="change">The amount by which to change the pivot point.</param>
	/// <param name="monoBehavior">The MonoBehavior that may have been affected by change in pivot point. This is used mainly for setting it dirty in the Editor.</param>
	/// <returns>Whether the pivot point was updated.</returns>
	public override bool UpdatePivotPoint (Vector2 change, out MonoBehaviour monoBehavior)
	{
		if(LinkedMeshFilter.sharedMesh == null)
			Initialize(true);
	
		pivotOffset -= change;
			
		vertices = LinkedMeshFilter.sharedMesh.vertices;
		for(int i = 0; i < LinkedMeshFilter.sharedMesh.vertices.Length; i++)
			vertices[i] -= (Vector3)change;
		
		LinkedMeshFilter.sharedMesh.vertices = vertices;

		return base.UpdatePivotPoint(change, out monoBehavior);
	}
}
