/* /*
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

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single polygonal closed shape (can be concave)
/// Used a base shape for JelloBody objects.
/// </summary>
[Serializable]
public class JelloClosedShape
{
    #region PRIVATE VARIABLES

	/// <summary>
	/// Vertices that make up this collision geometry.  
	/// Shape connects vertices in CounterClockwise or Clockwise order, closing the last vertex to the first.
	/// </summary>
	[SerializeField]
	private Vector2[] mEdgeVertices;

	/// <summary>
	/// Vertices internal to the collision geometry. 
	/// </summary>
	[SerializeField]
	private Vector2[] mInternalVertices;

	/// <summary>
	/// The center of the vertices. calculated when JelloClosedShape.finish() is called
	/// </summary>
	[SerializeField]
	private Vector2 mCenter;

	/// <summary>
	/// The triangles indeces representing the triangulation of this JelloClosedShape.
	/// </summary>
	[SerializeField]
	private int[] mTriangles;
	
    #endregion

	#region PUBLIC VARIABLES

	/// <summary>
	/// Shape winding (CW or CCW) enum.
	/// </summary>
	public enum Winding { Clockwise, CounterClockwise, Unassigned };

	/// <summary>
	/// shape winding (CW or CCW).
	/// </summary>
	public Winding winding = Winding.Clockwise;

	#endregion

    #region CONSTRUCTORS

	/// <summary>
	/// Initializes a new JelloClosedShape.
	/// </summary>
    public JelloClosedShape()
    {
        mEdgeVertices = new Vector2[0];
		mInternalVertices = new Vector2[0];
    }
	
	/// <summary>
	/// Initializes a new JelloClosedShape.
	/// Constructed from an existing list of vertices.
	/// </summary>
	/// <param name="edgeVertices">The edge vertices to construct this JelloClosedShape from.</param>
	/// <param name="internalVertices">The internal vertices to construct this JelloClosedShape from.</param>
	/// <param name="recenter">Whether to convert the vertices into local space.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a closed shape square
	/// Vector2[] square = new Vector2[4];
	/// square[0] = new Vector2(-1,1);	//top left
	/// square[1] = new Vector2(1,1);	//top right
	/// square[2] = new Vector2(1,-1);	//bottom right
	/// square[3] = new Vector2(-1,-1);	//bottom left
	/// 
	/// JelloClosedShape shape = new JelloClosedShape(square);
	/// ~~~
    public JelloClosedShape(Vector2[] edgeVertices, Vector2[] internalVertices = null, bool recenter = true)
    {
		mEdgeVertices = new Vector2[edgeVertices.Length];
		
        for(int i = 0; i < mEdgeVertices.Length; i++)
			mEdgeVertices[i] = edgeVertices[i];

		if(internalVertices != null)
		{
			bool[] validity = new bool[internalVertices.Length];
			int num = 0;
			for(int i = 0;  i < internalVertices.Length; i++)
			{
				if(JelloShapeTools.Contains(mEdgeVertices, internalVertices[i]))
				{
					validity[i] = true;
					num++;
				}
				else
				{
					validity[i] = false;
				}
			}

			mInternalVertices = new Vector2[num];
			num = 0;
			for(int i = 0; i < internalVertices.Length; i++)
			{
				if(validity[i])
				{
					mInternalVertices[num] = internalVertices[i];
					num++;
				}
			}
		}
		else
		{
			mInternalVertices = new Vector2[0];
		}

        finish(recenter);
    }
	
    #endregion

    #region SETUP - ADDING VERTS
    
	/// <summary>
	/// Get this JelloClosedShape ready to start adding vertices.
	/// Will clear JelloClosedShape.EdgeVertices, JelloClosedShape.InternalVertices and JelloClosedShape.Traingles.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a closed shape square
	/// JelloClosedShape shape = new JelloClosedShape();
	/// 
	/// shape.begin();
	/// 
	/// shape.addPoint(new Vector2(-1,1));	//top left
	/// shape.addPoint(new Vector2(-1,1));	//top right
	/// shape.addPoint(new Vector2(-1,1));	//bottom right
	/// shape.addPoint(new Vector2(-1,1));	//bottom left
	/// 
	/// shape.finish();
	/// ~~~
    public void begin()
    {
		mEdgeVertices = new Vector2[0];
		mInternalVertices = new Vector2[0];
		mTriangles = new int[0];
    }
	
	/// <summary>
	/// Add the specified point to the perimeter of the JelloClosedShape. 
	/// Be sure to call JelloClosedShape.begin() before adding vertices and JelloClosedShape.finish() when done adding any vertices..
	/// </summary>
	/// <param name="vertex">The vertex to add to the JelloClosedShape.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a closed shape square
	/// JelloClosedShape shape = new JelloClosedShape();
	/// 
	/// shape.begin();
	/// 
	/// shape.addEdgeVertex(new Vector2(-1,1));	//top left
	/// shape.addEdgeVertex(new Vector2(-1,1));	//top right
	/// shape.addEdgeVertex(new Vector2(-1,1));	//bottom right
	/// shape.addEdgeVertex(new Vector2(-1,1));	//bottom left
	/// 
	/// shape.finish();
	/// ~~~
	public void addEdgeVertex(Vector2 vertex)
	{
		Vector2[] temp = new Vector2[mEdgeVertices.Length + 1];
		
		for(int i = 0; i < mEdgeVertices.Length; i++)
			temp[i] = mEdgeVertices[i];
		
		temp[temp.Length - 1] = vertex;
		
		mEdgeVertices = temp;
	}

	/// <summary>
	/// Add the specified point to the interior of the JelloClosedShape.
	/// Be sure to call JelloClosedShape.begin() before adding vertices and JelloClosedShape.finish() when done adding any vertices..
	/// </summary>
	/// <param name="vertex">The vertex to add to the JelloClosedShape.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a closed shape square with an internal point in the center
	/// JelloClosedShape shape = new JelloClosedShape();
	/// 
	/// shape.begin();
	/// 
	/// shape.addEdgeVertex(new Vector2(-1,1));	//top left
	/// shape.addEdgeVertex(new Vector2(-1,1));	//top right
	/// shape.addEdgeVertex(new Vector2(-1,1));	//bottom right
	/// shape.addEdgeVertex(new Vector2(-1,1));	//bottom left
	/// shape.addInternalVertex(Vector2.zero);	//internal point at center
	/// 
	/// shape.finish();
	/// ~~~
	public bool addInternalVertex(Vector2 vertex)
	{
//		//dont allow duplicate points.
//		for(int i = 0; i < mInternalVertices.Length; i++)
//			if(vertex == mInternalVertices[i])
//				return false;
//
//		//dont allow points outside the perimeter of the body.
//		if(!JelloShapeTools.Contains(mEdgeVertices, vertex))
//			return false;
//
//
//		//dont allow points along the perimiter of the body. 
//		for(int i = 0; i < mEdgeVertices.Length; i++)
//		{
//			int b = i + 1 < mEdgeVertices.Length ? i + 1 : 0;
//			if(JelloVectorTools.CrossProduct(vertex - mEdgeVertices[i], mEdgeVertices[b] - mEdgeVertices[i]) == 0f)
//			{
//				if(
//					vertex.x <= Mathf.Max (mEdgeVertices[i].x, mEdgeVertices[b].x) ||
//					vertex.x >= Mathf.Min (mEdgeVertices[i].x, mEdgeVertices[b].x) || 
//					vertex.y <= Mathf.Max (mEdgeVertices[i].y, mEdgeVertices[b].y) ||
//					vertex.y >= Mathf.Min (mEdgeVertices[i].y, mEdgeVertices[b].y)
//					)
//				{
//					return false;
//				}
//			}
//		}

		Vector2[] temp = new Vector2[mInternalVertices.Length + 1];
		
		for(int i = 0; i < mInternalVertices.Length; i++)
			temp[i] = mInternalVertices[i];
		
		temp[temp.Length - 1] = vertex;
		
		mInternalVertices = temp;
		return true;
	}

	/// <summary>
	/// Removes the internal point at the given index. Be sure to call JelloClosedShape.finish() after this.
	/// </summary>
	/// <param name="index">The index of the point to remove.</param>
	public void removeInternalVertex(int index)
	{
		if(index < 0 || index >= mInternalVertices.Length)
			return;

		List<Vector2> tempVertices = new List<Vector2>();
		for(int i = 0; i < mInternalVertices.Length; i++)
			if(i != index)
				tempVertices.Add (mInternalVertices[i]);
		
		mInternalVertices = tempVertices.ToArray();
	}

	/// <summary>
	/// Clears all internal points. Be sure to call JelloClosedShape.finish() after this.
	/// </summary>
	public void clearInternalVertices()
	{
		mInternalVertices = new Vector2[0];
	}

	/// <summary>
	/// Finish adding vertices to this JeloClosedShape, and choose whether to convert into local space.
	/// JelloClosedShape.winding and JelloClosedShape.Triangles will be set.
	/// Make sure there are no duplicate points before calling this. use JelloShapeTools.RemoveDuplicatePoints().
	/// </summary>
	/// <param name="recenter">whether to convert the positions of the JelloClosedShape into local space.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a closed shape square
	/// JelloClosedShape shape = new JelloClosedShape();
	/// 
	/// shape.begin();
	/// 
	/// shape.addPoint(new Vector2(-1,1));	//top left
	/// shape.addPoint(new Vector2(-1,1));	//top right
	/// shape.addPoint(new Vector2(-1,1));	//bottom right
	/// shape.addPoint(new Vector2(-1,1));	//bottom left
	/// 	
	/// shape.finish();
	/// ~~~
    public void finish(bool recenter = true)
    {

		if(mInternalVertices != null && mInternalVertices.Length > 0)
		{
			//dont allow duplicate points
			mInternalVertices = JelloShapeTools.RemoveDuplicatePoints(mInternalVertices);

			//dont allow points outside of the perimiter
			for(int i = 0;  i < mInternalVertices.Length; i++)
			{
				if(!JelloShapeTools.Contains(mEdgeVertices, mInternalVertices[i]))
				{	
					mInternalVertices[i] = Vector2.one * Mathf.Infinity;
				}
			}
			//dont allow points on the perimiter. (this will also remove any null points)
			mInternalVertices = JelloShapeTools.RemovePointsOnPerimeter(mEdgeVertices, mInternalVertices);
		}

		mCenter = JelloShapeTools.FindCenter(mEdgeVertices);
        
		if (recenter)
        {	
            // now subtract this from each element, to get proper "local" coordinates.
            for (int i = 0; i < mEdgeVertices.Length; i++)
                mEdgeVertices[i] -= mCenter;
			if(mInternalVertices != null)
				for (int i = 0; i < mInternalVertices.Length; i++)
					mInternalVertices[i] -= mCenter;
        }

		if(JelloShapeTools.HasClockwiseWinding(mEdgeVertices))
			winding = Winding.Clockwise;
		else
			winding = Winding.CounterClockwise;



		Triangulate();
    }

    #endregion

    #region PUBLIC PROPERTIES

	/// <summary>
	/// Get the edge vertices.
	/// </summary>
	/// <value>The edge vertices.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //get the position of every other edge vertex in the shape
	/// JelloClosedShape shape;
	/// 
	/// Vector2[] verts = new Vector2[shape.VertexCount * 0.5f]
	/// for(int i = 0; i < shape.EdgeVertexCount; i+=2)
	/// {
	/// 	verts[i] = shape.EdgeVertices[i];
	/// }
	/// ~~~
    public Vector2[] EdgeVertices
    {
        get { return mEdgeVertices; }
    }

	/// <summary>
	/// Get the internal vertices.
	/// </summary>
	/// <value>The internal vertices.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //get the position of every other internal vertex in the shape
	/// JelloClosedShape shape;
	/// 
	/// Vector2[] verts = new Vector2[shape.VertexCount * 0.5f]
	/// for(int i = 0; i < shape.InternalVertexCount; i+=2)
	/// {
	/// 	verts[i] = shape.InternalVertices[i];
	/// }
	/// ~~~
	public Vector2[] InternalVertices
	{
		get { return mInternalVertices; }
	}

	/// <summary>
	/// Get the number of vertices (internal and external).
	/// </summary>
	/// <value>The vertex count.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //get the position of every other point in the shape
	/// JelloClosedShape shape;
	/// 
	/// Vector2[] verts = new Vector2[shape.VertexCount * 0.5f]
	/// for(int i = 0; i < shape.VertexCount; i+=2)
	/// {
	/// 	verts[i] = shape.getVertex(i);
	/// }
	/// ~~~
	public int VertexCount
	{
		get{ return mEdgeVertices.Length + mInternalVertices.Length; }
	}

	/// <summary>
	/// Get the edge vertex count.
	/// </summary>
	/// <value>The edge vertex count.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //get the position of every other edge vertex in the shape
	/// JelloClosedShape shape;
	/// 
	/// Vector2[] verts = new Vector2[shape.EdgeVertexCount * 0.5f]
	/// for(int i = 0; i < shape.EdgeVertexCount; i+=2)
	/// {
	/// 	verts[i] = shape.EdgeVertices[i];
	/// }
	/// ~~~
	public int EdgeVertexCount
	{
		get{ return mEdgeVertices.Length; }
	}

	/// <summary>
	/// Gets the internal vertex count.
	/// </summary>
	/// <value>The internal vertex count.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //get the position of every other point in the shape
	/// JelloClosedShape shape;
	/// 
	/// Vector2[] verts = new Vector2[shape.InternalVertexCount * 0.5f]
	/// for(int i = 0; i < shape.InternalVertexCount; i+=2)
	/// {
	/// 	verts[i] = shape.InternalVertices[i];
	/// }
	/// ~~~
	public int InternalVertexCount
	{
		get{ return mInternalVertices.Length; }
	}

	/// <summary>
	/// Gets the center of the shape.
	/// JelloClosedShape.InternalVertices are not used calculating this value.
	/// </summary>
	/// <value>The center of the JelloClosedShape.</value>
	public Vector2 Center
	{
		get { return mCenter; }
	}
	
	/// <summary>
	/// Get the triangles indices representing the triangulation of this JelloClosedShape.
	/// </summary>
	/// <value>The triangle indices.</value>
	public int[] Triangles
	{
		get { return mTriangles; }
	}
	
    #endregion

    #region HELPER FUNCTIONS
	
    /// <summary>
    /// Get an array of vertices by transformin the local vertices by the given position, angle, and scale.
    /// Transformation is applied in the following order:  scale -> rotation -> position.
    /// </summary>
    /// <param name="position">The position transform vertices to.</param>
    /// <param name="angle">The angle (in degrees) to rotate the vertices to.</param>
    /// <param name="scale">The scale to transform the vertices by.</param>
	/// <param name="includeInternal">Whether to include JelloClosedShape.InternalVertices.</param>
    /// <returns>A new array of transformed vertices.</returns>
    public Vector2[] transformVertices(Vector2 position, float angle, Vector2 scale, bool includeInternal = false)     
	{
		Vector2[] ret;

		if(includeInternal)
		{
			ret = new Vector2[mEdgeVertices.Length + mInternalVertices.Length];
			
			for (int i = 0; i < mEdgeVertices.Length; i++)
			{
				// rotate the point, and then translate.
				ret[i].x = mEdgeVertices[i].x * scale.x;
				ret[i].y = mEdgeVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref ret[i], angle);
				
				ret[i] += position;
			}
			for(int i = 0; i < mInternalVertices.Length; i++)
			{
				ret[mEdgeVertices.Length + i].x = mInternalVertices[i].x * scale.x;
				ret[mEdgeVertices.Length + i].y = mInternalVertices[i].y * scale.y;

				JelloVectorTools.rotateVector(ref ret[mEdgeVertices.Length + i], angle);

				ret[mEdgeVertices.Length + i] += position;
			}
		}
		else
		{
			ret = new Vector2[mEdgeVertices.Length];
			
			for (int i = 0; i < mEdgeVertices.Length; i++)
			{
				// rotate the point, and then translate.
				ret[i].x = mEdgeVertices[i].x * scale.x;
				ret[i].y = mEdgeVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref ret[i], angle);
				
				ret[i] += position;
			}
		}

        return ret;
    }
	
	/// <summary>
	/// Get an array of vertices by transformin the local vertices by the given position, angle, and scale.
	/// Transformation is applied in the following order:  scale -> rotation -> position.
    /// </summary>
	/// <param name="position">The position transform vertices to.</param>
	/// <param name="angle">The angle (in degrees) to rotate the vertices to.</param>
	/// <param name="scale">The scale to transform the vertices by.</param>
	/// <param name="vertices">A new array of transformed vertices.</param>
	/// <param name="includeInternal">Whether to include JelloClosedShape.InternalVertices.</param>
	public void transformVertices(Vector2 position, float angle, Vector2 scale, ref Vector2[] vertices, bool includeInternal = false)
    {
		if(includeInternal)
		{
			if(vertices.Length != mEdgeVertices.Length + mInternalVertices.Length)
				vertices = new Vector2[mEdgeVertices.Length + mInternalVertices.Length];
			
			for (int i = 0; i < mEdgeVertices.Length; i++)
			{
				// rotate the point, and then translate.
				vertices[i].x = mEdgeVertices[i].x * scale.x;
				vertices[i].y = mEdgeVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref vertices[i], angle);
				
				vertices[i] += position;
			}
			for (int i = 0; i < mInternalVertices.Length; i++)
			{
				// rotate the point, and then translate.
				vertices[mEdgeVertices.Length + i].x = mInternalVertices[i].x * scale.x;
				vertices[mEdgeVertices.Length + i].y = mInternalVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref vertices[mEdgeVertices.Length + i], angle);
				
				vertices[mEdgeVertices.Length + i] += position;
			}
		}
		else
		{
			if(vertices.Length != mEdgeVertices.Length)
				vertices = new Vector2[mEdgeVertices.Length];
			
			for (int i = 0; i < mEdgeVertices.Length; i++)
			{
				// rotate the point, and then translate.
				vertices[i].x = mEdgeVertices[i].x * scale.x;
				vertices[i].y = mEdgeVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref vertices[i], angle);
				
				vertices[i] += position;
			}
		}
    }

	/// <summary>
	/// Get an array of vertices by transformin the local vertices by the given position, angle, and scale.
	/// Transformation is applied in the following order:  scale -> rotation -> position.
	/// </summary>
	/// <param name="position">The position transform vertices to.</param>
	/// <param name="angle">The angle (in degrees) to rotate the vertices to.</param>
	/// <param name="scale">The scale to transform the vertices by.</param>
	/// <param name="vertices">A new array of transformed vertices.</param>
	/// <param name="includeInternal">Whether to include JelloClosedShape.InternalVertices.</param>
	public void transformVertices(ref Vector2 position, ref float angle, ref Vector2 scale, ref Vector2[] vertices, bool includeInternal = false)
	{
		if(includeInternal)
		{
			if(vertices.Length != mEdgeVertices.Length + mInternalVertices.Length)
				vertices = new Vector2[mEdgeVertices.Length + mInternalVertices.Length];
			
			for (int i = 0; i < mEdgeVertices.Length; i++)
			{
				// rotate the point, and then translate.
				vertices[i].x = mEdgeVertices[i].x * scale.x;
				vertices[i].y = mEdgeVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref vertices[i], angle);
				
				vertices[i] += position;
			}
			for (int i = 0; i < mInternalVertices.Length; i++)
			{
				// rotate the point, and then translate.
				vertices[mEdgeVertices.Length + i].x = mInternalVertices[i].x * scale.x;
				vertices[mEdgeVertices.Length + i].y = mInternalVertices[i].y * scale.y;
				
				JelloVectorTools.rotateVector(ref vertices[mEdgeVertices.Length + i], angle);
				
				vertices[mEdgeVertices.Length + i] += position;
			}
		}
		else
		{
			if(vertices.Length != mEdgeVertices.Length)
				vertices = new Vector2[mEdgeVertices.Length];
			
	        for (int i = 0; i < mEdgeVertices.Length; i++)
	        {
	            // rotate the point, and then translate.
	            vertices[i].x = mEdgeVertices[i].x * scale.x;
	            vertices[i].y = mEdgeVertices[i].y * scale.y;
				
	            JelloVectorTools.rotateVector(ref vertices[i], angle);
	            
				vertices[i] += position;
			}
		}
    }
	
	/// <summary>
	/// Flip the JelloClosedShape horizontaly.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //turn a body around when it hits a wall
	/// 
	/// JelloBody body;
	/// void handleCollisionEnter(JelloCollisionManifold manifold)
	/// {
	/// 	if(manifold.GetOtherBody(body).gameObject.tag == "wall")
	/// 	{
	/// 		body.flipX();
	/// 		//also reverse movement direction
	/// 	}
	/// }
	/// ~~~
	public void flipX() //TODO check into this --> will i need to retriangulate? because mesh triangles may need to always be wound in a certain direction.
	{
		Vector2[] tempVertices = new Vector2[mEdgeVertices.Length];
		
		for(int i = 0; i < mEdgeVertices.Length; i++)
		{																						
			mEdgeVertices[i] = new Vector2(mEdgeVertices[i].x + 2 * (Center.x - mEdgeVertices[i].x), mEdgeVertices[i].y);
			tempVertices[mEdgeVertices.Length - i - 1] = mEdgeVertices[i];
		}
		
		mEdgeVertices = tempVertices;

		tempVertices = new Vector2[mInternalVertices.Length];
		
		for(int i = 0; i < mInternalVertices.Length; i++)
		{																						
			mInternalVertices[i] = new Vector2(mInternalVertices[i].x + 2 * (Center.x - mInternalVertices[i].x), mInternalVertices[i].y);
			tempVertices[mInternalVertices.Length - i - 1] = mInternalVertices[i];
		}
		
		mInternalVertices = tempVertices;

		if(winding == Winding.Clockwise)
		{
			winding = Winding.CounterClockwise;
		}
		else if(winding == Winding.CounterClockwise)
		{
			winding = Winding.Clockwise;
		}
		else
		{
			if(JelloShapeTools.HasClockwiseWinding(mEdgeVertices))
				winding = Winding.Clockwise;					
			else
				winding = Winding.CounterClockwise;
		}

		//finish();
	}
	
	/// <summary>
	/// Flip the JelloClosedShape verticaly.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //turn a body around when it hits a ceiling
	/// 
	/// JelloBody body;
	/// void handleCollisionEnter(JelloCollisionManifold manifold)
	/// {
	/// 	if(manifold.GetOtherBody(body).gameObject.tag == "ceiling")
	/// 	{
	/// 		body.flipY();
	/// 		//also reverse movement direction
	/// 	}
	/// }
	/// ~~~
	public void flipY() //TODO check into this --> will i need to retriangulate? because mesh triangles may need to always be wound in a certain direction.
	{
		Vector2[] tempVertices = new  Vector2[mEdgeVertices.Length];
		
		for(int i = 0; i < mEdgeVertices.Length; i++)
		{																						
			mEdgeVertices[i] = new Vector2(mEdgeVertices[i].x, mEdgeVertices[i].y + 2 * (Center.x - mEdgeVertices[i].x));
			tempVertices[mEdgeVertices.Length - i - 1] = mEdgeVertices[i];
		}
		
		mEdgeVertices = tempVertices;


		tempVertices = new  Vector2[mInternalVertices.Length];
		
		for(int i = 0; i < mInternalVertices.Length; i++)
		{																						
			mInternalVertices[i] = new Vector2(mInternalVertices[i].x, mInternalVertices[i].y + 2 * (Center.x - mInternalVertices[i].x));
			tempVertices[mInternalVertices.Length - i - 1] = mInternalVertices[i];
		}
		
		mInternalVertices = tempVertices;

		if(winding == Winding.Clockwise)
		{
			winding = Winding.CounterClockwise;
		}
		else if(winding == Winding.CounterClockwise)
		{
			winding = Winding.Clockwise;
		}
		else
		{
			if(JelloShapeTools.HasClockwiseWinding(mEdgeVertices))
				winding = Winding.Clockwise;					
			else
				winding = Winding.CounterClockwise;
		}

		//finish ();
	}
	
	/// <summary>
	/// Change the position of vertex at the given index.
	/// </summary>
	/// <param name="index">The index of vertext to modify.</param>
	/// <param name="position">The position to set vertex to.</param>
	/// <param name="recenter">Whether to recenter the shape.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //make the first edge vertex of the shape have a position increase vertical by one
	/// JelloBody body;
	/// 
	/// body.Shape.changeEdgeVertexPosition(0, body.Shape.EdgeVertices[0] + Vector2.up, true);
	/// ~~~
	public void changeEdgeVertexPosition(int index, Vector2 position, bool recenter)//TODO keep/remove recentering form this?
	{
		if(index < 0 || index >= mEdgeVertices.Length)
			return;

		mEdgeVertices[index] = position;
		
		finish (recenter);
	}

	/// <summary>
	/// Change the position of internal vertex at the index. be sure to call JelloClosedShape.finish() after this.
	/// </summary>
	/// <param name="index">index of vertext to modify.</param>
	/// <param name="position">position to set vertex to.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //make the first internal vertex of the shape have a position increase vertically by one
	/// JelloBody body;
	/// 
	/// body.Shape.changeInternalVertexPosition(0, body.Shape.InternalVertices[0] + Vector2.up, true);
	/// ~~~
	public void changeInternalVertexPosition(int index, Vector2 position)
	{
		if(index < 0 || index >= mInternalVertices.Length)
			return;

		mInternalVertices[index] = position;
	}
	
	/// <summary>
	/// Change the positions of the vertices of the JelloClosedShape. Be sure to call JelloClosedShape.finish() after this.
	/// Will fail if number of vertices do not match.
	/// </summary>
	/// <param name="edgeVertices">The new edge vertex positions.</param>
	/// <param name="internalVertices">The new internal vertex positions.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //create a closed shape in the form of a square and then change it into a rectangle
	/// Vector2[] square;
	/// Vector2[] rectangle;
	/// 
	/// JelloClosedShape shape = new JelloClosedShape(square);
	/// 
	/// shape.changeVertices(rectangle, new Vector2[0], true);
	/// ~~~
	public void changeVertices(Vector2[] edgeVertices, Vector2[] internalVertices)
	{
		if(edgeVertices.Length == mEdgeVertices.Length)
		{
			for(int i = 0 ; i < edgeVertices.Length; i++)
				mEdgeVertices[i] = edgeVertices[i];

			bool[] valid = new bool[internalVertices.Length];
			int num = 0;

			for(int i = 0 ; i < internalVertices.Length; i++)
			{
				if(JelloShapeTools.Contains(mEdgeVertices, internalVertices[i]))
				{
					valid[i] = true;
					num++;
				}
				else
				{
					valid[i] = false;
				}
			}

			mInternalVertices = new Vector2[num];

			num = 0;
			for(int i = 0 ; i < internalVertices.Length; i++)
			{
				if(valid[i])
				{
					mInternalVertices[num] = internalVertices[i];
					num++;
				}
			}
		}
		else
		{
			Debug.LogWarning("new vertices count less than current vertices count");
		}
	}

	/// <summary>
	/// Gets the vertex. Includes JelloClosedShape.EdgeVertices and JelloClosedShape.InternalVertices.
	/// 0 will return the first point in JelloClosedShape.EdgeVertices and JelloClosedShape.EdgeVertexCount will return the first point in JelloClosedShape.InternalVertices (if any exist). 
	/// </summary>
	/// <param name="index">The index of the vertex.</param>
	/// <returns>The vertex.</returns>
	public Vector2 getVertex(int index)
	{
		if(index >= 0)
		{
			if(index < mEdgeVertices.Length)
				return mEdgeVertices[index];
			else if(index < mEdgeVertices.Length + mInternalVertices.Length)
				return mInternalVertices[index - mEdgeVertices.Length];
			else
				return Vector2.zero;
		}
		else
		{
			return Vector2.zero;
		}
	}

	/// <summary>
	/// Set the vertex at the given index. Includes JelloClosedShape.EdgeVertices and JelloClosedShape.InternalVertices.
	/// 0 will set the first point in JelloClosedShape.EdgeVertices and JelloClosedShape.EdgeVertexCount will set the first point in JelloClosedShape.InternalVertices (if any exist).
 	/// Be sure to call JelloClosedShape.finish() after this. 
	/// </summary>
	/// <param name="index">The index of the vertex to set.</param>
	/// <param name="position">The new vertex position.</param>
	public void setVertex(int index, Vector2 position)
	{
		if(index >= 0)
		{
			if(index < mEdgeVertices.Length)
				mEdgeVertices[index] = position;
			else if(index < mEdgeVertices.Length + mInternalVertices.Length)
				mInternalVertices[index - mEdgeVertices.Length] = position;
		}
	}

	/// <summary>
	/// Triangulate this JelloClosedShape.
	/// Make sure there are no duplicate points in JelloClosedShape.EdgeVertices or JelloClosedShape.InternalVertices or else the triangulator will fail.
	/// </summary>
	public void Triangulate()
	{	
		JelloPoly2Tri.JelloPolygonPoint[] points = new JelloPoly2Tri.JelloPolygonPoint[mEdgeVertices.Length]; 
		
		for(int i = 0; i < mEdgeVertices.Length; i++)
		{
			points[i] = new JelloPoly2Tri.JelloPolygonPoint((double)mEdgeVertices[i].x, (double)mEdgeVertices[i].y);
			points[i].polygonIndex = i;
		}
		
		JelloPoly2Tri.Polygon poly = new JelloPoly2Tri.Polygon(points);
		
		points = new JelloPoly2Tri.JelloPolygonPoint[mInternalVertices.Length];
		for(int i = 0; i < mInternalVertices.Length; i++)
		{
			points[i] = new JelloPoly2Tri.JelloPolygonPoint((double)InternalVertices[i].x, (double)InternalVertices[i].y);
			points[i].polygonIndex = i + mEdgeVertices.Length;
			poly.AddSteinerPoint(points[i]);
		}
		
		JelloPoly2Tri.P2T.Triangulate(poly);

		if(mTriangles == null || mTriangles.Length != poly.Triangles.Count * 3)
			mTriangles = new int[poly.Triangles.Count * 3];

		JelloPoly2Tri.JelloPolygonPoint jpp;
		for(int i = 0; i < poly.Triangles.Count; i++) //reverse triangle winding as well //TODO make sure this works for CCW and CW polygons...
		{
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[2];
			mTriangles[3 * i + 0] = jpp.polygonIndex;
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[1];
			mTriangles[3 * i + 1] = jpp.polygonIndex;
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[0];
			mTriangles[3 * i + 2] =  jpp.polygonIndex;
		}
	}
    #endregion
}
