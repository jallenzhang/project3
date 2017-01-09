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
using System;

/// <summary>
/// The MeshLink class and its derivative classes (SpriteMeshLink, TextureMeshLink, etc...) serve as a go-between 
/// for JelloBody objects and allow for JelloBody driven Mesh deformation with a number of Mesh options available in Unity.
/// Extend this class to implement your own mesh link.
/// Add your derived class name to the MeshLinkType enum in this class.
/// </summary>

[ExecuteInEditMode][RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class MeshLink : MonoBehaviour
{	
	//add your meshLink derived class to this enum

	/// <summary>
	/// The available MeshLink options for JelloBody driven Mesh deformation.
	/// Add your own class name for to this enum for custom implementations of the MeshLink class.
	/// </summary>
	public enum MeshLinkType { None, SpriteMeshLink, TextureMeshLink, RageSplineMeshLink, tk2dMeshLink };
	
	/// <summary>
	/// Whether to calculate Mesh.normals for MeshLink.LinkedMeshFilter.sharedMesh.
	/// </summary>
	public bool CalculateNormals = false;
	
	/// <summary>
	/// Whether to calculate Mesh.tangents for MeshLink.LinkedMeshFilter.sharedMesh.
	/// </summary>
	public bool CalculateTangents = false;
	
	/// <summary>
	/// The type of the MeshLink this is.
	/// </summary>
	public MeshLinkType meshLinkType = MeshLinkType.None;
	
	/// <summary>
	/// Whether the MeshLink will allow the pivot point to be modified.
	/// </summary>
	public bool canModifyPivotPoint = true;
	
	/// <summary>
	/// The JelloBody driving the MeshLink.LinkedMeshFilter.sharedMesh deformation.
	/// </summary>
	public JelloBody body;
	
	/// <summary>
	/// The pivot offset.
	/// Allows for off-centered JelloBody objects to correctly drive MeshLink.LinkedMeshFilter.sharedMesh deformation.
	/// </summary>
	public Vector2 pivotOffset = Vector2.zero;
	
	/// <summary>
	/// The offset applied to the MeshLink.LinkedMeshFilter.sharedMesh.uv coordinates.
	/// </summary>
	public Vector2 offset = Vector2.zero;
	
	/// <summary>
	/// The scale applied to the MeshLink.LinkedMeshFilter.sharedMesh.uv coordinates.
	/// </summary>
	public Vector2 scale = Vector2.one;
	
	/// <summary>
	/// The angle applied to the MeshLink.LinkedMeshFilter.sharedMesh.uv coordinates.
	/// </summary>
	public float angle =  0f;
	

	/// <summary>
	/// Array of vertices used frequently for MeshLink.LinkedMeshFilter.sharedMesh initialization / deformation.
	/// </summary>
	[SerializeField]
	protected Vector3[] vertices = new Vector3[0];
	
	/// <summary>
	/// The MeshFilter that owns the MeshLink.LinkedMeshFilter.sharedMesh to be deformed.
	/// </summary>
	[SerializeField]
	protected MeshFilter mMeshFilter;

	/// <summary>
	/// The MeshRenderer that owns the MeshLink.LinkedMeshRenderer.sharedMaterial.
	/// </summary>
	[SerializeField]
	protected MeshRenderer mMeshRenderer;

	[SerializeField]
	protected int mSortingOrder = 0;

	[SerializeField]
	protected int mSortingLayer = 0;

	protected Vector2[] debugVertices = new Vector2[0];



	/// <summary>
	/// Get or set the MeshFilter that owns the MeshLink.LinkedMeshFilter.sharedMesh to be deformed.
	/// </summary>
	/// <value>The linked mesh filter.</value>
	public MeshFilter LinkedMeshFilter
	{
		get
		{
			if (mMeshFilter == null) 
			{
				mMeshFilter = GetComponent<MeshFilter>();
				
				if (mMeshFilter == null) 
					mMeshFilter = gameObject.AddComponent<MeshFilter>();
			}
			
			return mMeshFilter;
		}
		set { mMeshFilter = value; }
	}

	/// <summary>
	/// Get or set the MeshFilter that owns the MeshLink.LinkedMeshFilter.sharedMesh to be deformed.
	/// </summary>
	/// <value>The linked mesh filter.</value>
	public MeshRenderer LinkedMeshRenderer
	{
		get
		{
			if (mMeshRenderer == null) 
			{
				mMeshRenderer = GetComponent<MeshRenderer>();
				
				if (mMeshRenderer == null) 
					mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
			}
			
			return mMeshRenderer;
		}
		set { mMeshRenderer = value; }
	}

	void Awake()
	{
		LinkedMeshFilter.sharedMesh = new Mesh();;

		//TODO do what this says!!!
		//make sure to handle the case of LinkedMeshRenderer.sharedMaterial being null derived classes. (in the initilize method)
		if(LinkedMeshRenderer.sharedMaterial != null)
			LinkedMeshRenderer.sharedMaterial = new Material(LinkedMeshRenderer.sharedMaterial);

		mSortingOrder = GetComponent<Renderer>().sortingOrder;

		Initialize(true);
	}



	
	/// <summary>
	/// Initialize the MeshLink.
	/// </summary>
	/// <param name="forceUpdate">Whether to force an update to the MeshLink and MeshLink.LinkedMeshFilter.sharedMesh.</param>
	public virtual void Initialize(bool forceUpdate = false)
	{
		body = GetComponent<JelloBody>();
		
		if (LinkedMeshFilter.sharedMesh == null) LinkedMeshFilter.sharedMesh = new Mesh();
	}
	
	/// <summary>
	/// Update the MeshLink.mesh.vertices.
	/// This is called each Update() and drives the continuous MeshLink.LinkedMeshFilter.sharedMesh deformation.
	/// </summary>
	/// <param name="points">The basis of the new MeshLink.LinkedMeshFilter.sharedMesh.vertices.</param>
	public virtual void UpdateMesh(Vector2[] points){}
	
	/// <summary>
	/// Update the pivot point.
	/// </summary>
	/// <param name="change">The amount by which to change the pivot point.</param>
	/// <param name="monoBehavior">The MonoBehavior that may have been affected by change in pivot point. This is used mainly for setting it dirty in the Editor.</param>
	/// <returns>Whether the pivot point was updated.</returns>
	public virtual bool UpdatePivotPoint (Vector2 change, out MonoBehaviour monoBehavior){ monoBehavior = null;  return false;}
	
	
	void OnDestroy()
	{
		#if UNITY_EDITOR
		DestroyImmediate(LinkedMeshRenderer.sharedMaterial);
		DestroyImmediate(LinkedMeshFilter.sharedMesh);
		#else
		Destroy(LinkedMeshRenderer.sharedMaterial);
		Destroy(LinkedMeshFilter.sharedMesh);
		#endif
	}
	
	/// <summary>
	/// Apply a new offset to MeshLink.LinkedMeshFilter.sharedMesh.uv positions.
	/// </summary>
	/// <param name="newOffset">The offset to be applied.</param>
	public virtual void ApplyNewOffset(Vector2 newOffset)
	{
		Vector2 oldOffset = offset;
		offset = newOffset;
		
		if(LinkedMeshFilter.sharedMesh != null)
		{
			Vector2[] uvPts = LinkedMeshFilter.sharedMesh.uv;
			
			for(int i= 0; i < uvPts.Length; i++)
				uvPts[i] += oldOffset - offset;
			
			LinkedMeshFilter.sharedMesh.uv = uvPts;
		}
	}
	
	/// <summary>
	/// Calculate the MeshLink.LinkedMeshFilter.sharedMesh.tangents.
	/// </summary>
	protected void calculateMeshTangents()//Mesh mesh)
	{
		//speed up math by copying the mesh arrays
		int[] triangles = LinkedMeshFilter.sharedMesh.triangles;
		Vector3[] vertices = LinkedMeshFilter.sharedMesh.vertices;
		Vector2[] uv = LinkedMeshFilter.sharedMesh.uv;
		Vector3[] normals = LinkedMeshFilter.sharedMesh.normals;
		
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		
		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		LinkedMeshFilter.sharedMesh.tangents = tangents;
	}
}
