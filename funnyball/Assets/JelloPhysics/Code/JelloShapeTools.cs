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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility class for working with polygonal shapes
/// </summary>
public static class JelloShapeTools 
{
	
	/// <summary>
	/// Combine overlaping shapes and return the results.
	/// </summary>
	/// <param name='"shapesArray">The array of shapes to be comibned. Format is an array of Vector2 arrays.</param>
	/// <returns>An array of combined shapes.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //combine terrain shape, tree shape, and bush shape into as few shapes as possible and draw a glow around the resulting shapes
	/// Vector2[] terrainShape, treeShape, bushShape;
	/// Vector2[][] shapes = new Vector2[3][];
	/// 
	/// shapesIn[0] = treeShape;
	/// shapesIn[1] = terrainshape;
	/// shapesIn[2] = bushShape;
	/// 
	/// shapes = JelloShapeTools.MergeShapes(shapes);
	/// 
	/// for(int i = 0; i < shapes.Length; i++)
	/// {
	/// 	DrawShapeGlow(shapes[i]);
	/// }
	/// ~~~
	public static Vector2[][] MergeShapes(Vector2[][] shapesArray)
	{
		List<Vector2[]> shapes = new List<Vector2[]>();
		
		for(int i = 0; i < shapesArray.Length; i++)
			shapes.Add(shapesArray[i]);
		
		bool merge = true;
		
		while(merge == true) 
		{
			merge = false;
			
			for(int i = 0; i < shapes.Count; i++)
			{
				for(int a = i; a < shapes.Count; a++)
				{
					if( Intersect(shapes[i], shapes[a]) )
					{
						//merge shapes here
						Vector2[] newShape = Merge(shapes[i], shapes[a]);
						
						shapes.Remove(shapes[i]);
						shapes.Remove(shapes[a]);
						
						shapes.Add(newShape);
						
						merge = true;
					}
					
					if(merge)
						break;
				}
				
				if(merge)
					break;
			}
		}
		
		Vector2[][] outshape = new Vector2[shapes.Count][];
		
		for(int i = 0; i < shapes.Count; i++)
			outshape[i] = shapes[i];
			
		return outshape;
	}
	
	/// <summary>
	/// Check if two polygonal shapes intersect.
	/// </summary>
	/// <param name="shape1">The first shape.</param>
	/// <param name="shape2">The second shape.</param>
	/// <returns>Whether the shapes intersect at any point.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep moving a shape to the right until it no longer overlaps another one
	/// Vector2[] shapeOne, shapeTwo;
	/// 
	/// while(JelloShapeTools.Intersect(shapeOne, shapeTwo)))
	/// {
	/// 	foreach(Vector2 v in shapeOne)
	/// 	{
	/// 		v += Vector2.Right;
	/// 	}
	/// }
	/// ~~~
	public static bool Intersect(Vector2[] shape1, Vector2[] shape2)
	{
		bool intersect = false;
		
		for(int i = 0; i < shape2.Length; i++)
		{
			if(Contains(shape1, shape2[i]))
				intersect = true;
			
			if(intersect)
				break;
		}
		
		if(!intersect)
		{	
			for(int i = 0; i < shape1.Length; i++)
			{
				if(Contains(shape2, shape1[i]))
					intersect = true;
				
				if(intersect)
					break;
			}
		}	
			
		return intersect;
	}
	
	/// <summary>
    /// Detect if a point is inside a shape.
    /// </summary>
    /// <param name="shape">The shape in global space.</param>
    /// <param name="point">The point in global space.</param>
    /// <returns>Whether the given point is within the given shape.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep moving a point to the right until it is no longer inside a given shape
	/// Vector2[] shape;
	/// Vector2 point;
	/// 
	/// while(JelloShapeTools.Contains(shape, point)))
	/// {
	/// 	point += Vector2.right;
	/// }
	/// ~~~
    public static bool Contains(Vector2[] shape, Vector2 point)
    {
        // basic idea: draw a line from the point to a point known to be outside the body.  count the number of
        // lines in the polygon it intersects.  if that number is odd, we are inside.  if it's even, we are outside.
        // in this implementation we will always use a line that moves off in the positive X direction from the point
        // to simplify things.

        bool inside = false;
		int h = shape.Length - 1;
		
        for (int i = 0; i < shape.Length; h = i++)
        {
			if
			(
				(shape[h].y >= point.y != shape[i].y >= point.y) 
				&&
				(point.x <= (shape[h].x - shape[i].x) * (point.y - shape[i].y) / (shape[h].y - shape[i].y) + shape[i].x )
			)
			inside= !inside;
        }
		
        return inside;
    }
	
	/// <summary>
	/// Merge two given shapes.
	/// Does not support holes. Only call this if you already know the to shapes overlap.
	/// </summary>
	/// <param name="shapeA">The first shape.</param>
	/// <param name="shapeB">The second shape.</param>
	/// <returns>The silhouette of the overlapping shapes.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //combine terrain shape and tree shape into a single shape and draw a glow around the resulting shape
	/// Vector2[] terrainShape, treeShape;
	/// 
	/// if(JelloShapeTools.Intersect(terrainShape, treeShape))
	/// {	
	/// 	//only do this if you already know that the shapes overlap
	/// 	Vector2[] shape = ShapeTools.Merge(terrainShape, treeShape);
	/// 	
	/// 	DrawShapeGlow(shapes);
	/// }
	/// ~~~
	public static Vector2[] Merge(Vector2[] shapeA, Vector2[] shapeB)
	{
		//find the start point (bottom most point. if multiple bottom most, then left most)
		Vector2 startPoint = shapeA[0];
		int index = 0; //index of the shape that the startPoint represents
		
		//start at 1 becasue we already assigned it to 0
		for(int i = 1; i < shapeA.Length; i++)
		{
			if(shapeA[i].y < startPoint.y)
			{
				startPoint = shapeA[i];
				index = i; 
			}
			else if(shapeA[i].y == startPoint.y && shapeA[i].x < startPoint.x)
			{
				startPoint = shapeA[i];
				index = i; 
			}
		}
		
		bool startWithShapeB = false;
		for(int i = 0; i < shapeB.Length; i++)
		{
			if(shapeB[i].y < startPoint.y)
			{
				startPoint = shapeB[i];
				startWithShapeB = true;
			}
			else if(shapeB[i].y == startPoint.y && shapeB[i].x < startPoint.x)
			{
				startPoint = shapeB[i];
				startWithShapeB = true;
			}
		}
					
		Vector2[][] shapes = new Vector2[2][];
		if(!startWithShapeB)
		{
			shapes[0] = shapeA;
			shapes[1] = shapeB;
		}
		else
		{
			shapes[0] = shapeB;
			shapes[1] = shapeA;
		}
		
		List<Vector2> newShape = new List<Vector2>();
		
		int iterations = 0;
		bool startWithHit = false;
		Vector2 hitPointToAdd = new Vector2();
		
		while(iterations < 1000)
		{
			float dist = Mathf.Infinity;
			bool found = false;
			int edgenum = 0;
			int numHits = 0;

			for(int b = index; b < shapes[0].Length + index; b++)
			{
				int i = b < shapes[0].Length ? b : b - shapes[0].Length;
					
				Vector2 thisPos1 = startWithHit ? hitPointToAdd : shapes[0][i]; 
				Vector2 nextPos1 = shapes[0][i + 1 < shapes[0].Length ? i + 1 : 0];
				
				if(newShape.Count > 0 && thisPos1 == newShape[0])
					return newShape.ToArray();
				
				if(!startWithHit)
					newShape.Add (thisPos1);
				
				startWithHit = false;
				hitPointToAdd = Vector2.zero;
				
				for(int a = 0; a < shapes[1].Length; a++)
				{
					Vector2 thisPos2 = shapes[1][a];
					Vector2 nextPos2 = shapes[1][a + 1 < shapes[1].Length ? a + 1 : 0];
					
					Vector2 hitPt = new Vector2();
					float distToA = 0f;
					float distToB = 0f;
					
					//if(VectorTools.lineIntersect(thisPos1, nextPos1, thisPos2, nextPos2, out hitPt, out distToA, out distToB))
					if(JelloVectorTools.LineSegmentsIntersect(thisPos1, nextPos1, thisPos2, nextPos2, out hitPt, out distToA, out distToB))
					{
						bool disregard = false;
						
						if(newShape.Count > 0 && hitPt == newShape[newShape.Count - 1])
							disregard = true;
						
						if(!disregard)
						{
							found = true;
							numHits ++;
							
							if(distToA < dist)
							{
								dist = distToA;
								hitPointToAdd = hitPt;
								edgenum = a;
							}	
						}
					}
				}
				
				if(found)
				{
					startWithHit = true;
					
					newShape.Add(hitPointToAdd);	//reverse arays and break, set index to new edgenum
					
					index = edgenum;
					
					Vector2[] temp1 = shapes[0];
					Vector2[] temp2 = shapes[1];
					
					shapes[0] = temp2;
					shapes[1] = temp1;

					break;
				}	
				
			}
			
			//this could only occur if the other shape is wholy inside this one.
			if(numHits == 0)
				return shapes[0];
			
			iterations ++;
		}
		
		return newShape.ToArray();
	}
	
	
	/// <summary>
	/// Find the center of a shape (average vertex position).
	/// </summary>
	/// <param name="shape">The shape.</param>
	/// <returns>The center of the shape.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //spawn exhaust particles at the center of the shape
	/// Vector2[] shape;
	/// 
	/// Vector2 center = JelloShapeTools.FindCenter(shape);
	/// 
	/// SpawnParticlesAtPoint(center);
	/// ~~~
	public static Vector2 FindCenter(Vector2[] shape)
	{	
		Vector2 center = new Vector2();

		if(shape.Length < 1)
			return center;
		
		for(int i = 0; i < shape.Length; i++)
			center += shape[i];
		
		center /= shape.Length;
		
		return center;
	}
	
	/// <summary>
	/// Checks if two convex shapes overlap.
	/// Requires ClockWise winding. 
	/// Uses separating axis theorem.
	/// </summary>
	/// <param name="shapeA">The first convex shape.</param>
	/// <param name="shapeB">The second convex shape.</param>
	/// <returns>Whether the two convex shapes overlap.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep moving a triangle to the right until it no longer overlaps another one
	/// Vector2[] triangleOne, triangleTwo;
	/// 
	/// while(JelloShapeTools.Intersect_SAT(triangleOne, triangleTwo)))
	/// {
	/// 	foreach(Vector2 v in triangleOne)
	/// 	{
	/// 		v += Vector2.Right;
	/// 	}
	/// }
	/// ~~~
	public static bool IntersectSAT(Vector2[] shapeA, Vector2[] shapeB)
	{		
		float maxA = 0f,
				minA = 0f,
				maxB = 0f,
				minB = 0f,
				p = 0f;
		
		Vector2 axis = Vector2.zero;
		
		//project shapes onto each axis and compare max/min
		for(int i = 0; i < shapeA.Length + shapeB.Length; i++)
		{
			if(i < shapeA.Length)
				axis = JelloVectorTools.getPerpendicular(shapeA[i + 1 < shapeA.Length ? i + 1 : 0] - shapeA[i]);
			else
				axis = JelloVectorTools.getPerpendicular(shapeB[i + 1 - shapeA.Length < shapeB.Length ? i + 1 - shapeA.Length : 0] - shapeB[i - shapeA.Length]);
			
			maxA = Vector2.Dot (shapeA[0], axis);
			minA = maxA;
			for(int a = 1; a < shapeA.Length; a++)
			{
				p = Vector2.Dot (shapeA[a], axis);
				
				if(p  > maxA)
					maxA = p;
				if(p < minA)
					minA = p;
			}
									
			maxB = Vector2.Dot(shapeB[0], axis);
			minB = maxB;
			for(int a = 1; a < shapeB.Length; a++)
			{
				p = Vector2.Dot (shapeB[a], axis);

				if(p  > maxB)
					maxB = p;
				if(p < minB)
					minB = p;
			}						
			
			if(maxA < minB || minA > maxB)
				return false;
		}
		
		return true;
	}

	/// <summary>
	/// Find the closest vertex on shape to a given point.
	/// </summary>
	/// <param name="point">The point to find the closest vertex to.</param>
	/// <param name="shape">The shape to test against.</param>
	/// <returns>The closest vertex on the shape as an index of the passed in shape array.</returns>
	public static int FindClosestVertexOnShape(Vector2 point, Vector2[] shape)
	{
		float distance = Mathf.Infinity;
		int index = -1;
		float dist;
		
		for (int i = 0; i < shape.Length; i++)
		{
			dist = (point - shape[i]).sqrMagnitude;
			if (dist < distance)
			{
				distance = dist;
				index = i;
			}
		}

		return index;
	}

	/// <summary>
	/// Find the closest edge on shape to a given point.
	/// </summary>
	/// <param name="point">The point to find the closest edge to.</param>
	/// <param name="shape">The shape to test against.</param>
	/// <returns>The closest edge on shape as indices of the shape.</returns>
	public static int[] FindClosestEdgeOnShape(Vector2 point, Vector2[] shape)
	{
		float distance = Mathf.Infinity;
		int[] indices = new int[2];

		for(int i = 0; i < shape.Length; i++)
		{
			int next = i + 1 < shape.Length ? i + 1 : 0;

			Vector2 hit;
			float dist = JelloVectorTools.getClosestPointOnSegmentSquared(point, shape[i], shape[next], out hit);
			if(dist < distance)
			{
				distance = dist;
				indices[0] = i;
				indices[1] = next;
			}
		}
		return indices;
	}

	/// <summary>
	/// Find the closest edge on shape to a given point.
	/// </summary>
	/// <param name="point">The point to find the closest edge to.</param>
	/// <param name="shape">The shape to test against.</param>
	/// <param name="hit">The point on the edge that was closest to the given point</param>
	/// <returns>The closest edge on shape as indices of the shape.</returns>
	public static int[] FindClosestEdgeOnShape(Vector2 point, Vector2[] shape, out Vector2 hit)
	{
		float distance = Mathf.Infinity;
		int[] indices = new int[2];
		hit = Vector2.zero;
		
		for(int i = 0; i < shape.Length; i++)
		{
			int next = i + 1 < shape.Length ? i + 1 : 0;
			
			Vector2 h;
			float dist = JelloVectorTools.getClosestPointOnSegmentSquared(point, shape[i], shape[next], out h);
			if(dist < distance)
			{
				distance = dist;
				indices[0] = i;
				indices[1] = next;
				hit = h;
			}
		}
		return indices;
	}

	/// <summary>
	/// Determine if the given shape has clockwise winding.
	/// </summary>
	/// <param name="shape">The shape to test.</param>
	/// <returns>Whether the given shape has clockwise winding.</returns>
	public static bool HasClockwiseWinding(Vector2[] shape)
	{
		float sum = 0f;
		for(int i = 0; i < shape.Length; i++)
			sum += (shape[i + 1 < shape.Length ? i + 1 : 0].x - shape[i].x) * (shape[i + 1 < shape.Length ? i + 1 : 0].y + shape[i].y);

		return sum >= 0f;
	} 

	/// <summary>
	/// Remove any duplicate points from the given shape.
	/// </summary>
	/// <param name="shape">The shape to remove points from.</param>
	/// <returns>The shape with any duplicate points removed.</returns>
	public static Vector2[] RemoveDuplicatePoints(Vector2[] shape)
	{
		List<Vector2> newList = new List<Vector2>();
		int num = 0;
		for(int i = 0; i < shape.Length; i++)
		{
			if(!newList.Contains(shape[i]))
				newList.Add (shape[i]);
			else
				num++;
		}
		
		if(num != 0)
			return newList.ToArray();
		else
			return shape;
	}

	/// <summary>
	/// Removes the points on perimeter.
	/// </summary>
	/// <returns>The points on perimeter.</returns>
	/// <param name="shape">Shape.</param>
	/// <param name="points">Points.</param>
	public static Vector2[] RemovePointsOnPerimeter(Vector2[] shape, Vector2[] points)
	{
		int num = 0;
		for(int i = 0; i < points.Length; i++)
		{
			if(points[i].x == Mathf.Infinity)
			{
				num++;
				continue;
			}

			for(int a = 0; a < shape.Length; a++)
			{
				int b = a + 1 < shape.Length ? a + 1 : 0;

				if(JelloVectorTools.CrossProduct(points[i] - shape[a], shape[b] - shape[a]) == 0f)
				{
					if(
						points[i].x > Mathf.Max (shape[b].x, shape[a].x) ||
						points[i].x < Mathf.Min (shape[b].x, shape[a].x) || 
						points[i].y > Mathf.Max (shape[b].y, shape[a].y) ||
						points[i].y < Mathf.Min (shape[b].y, shape[a].y)
						)
					{
						continue;
					}

					points[i] = Vector2.one * Mathf.Infinity;
					num++;
					break;
				}
			}
		}

		Vector2[] returnPoints = new Vector2[points.Length - num];
		num = 0;
		for(int i = 0; i < points.Length; i++)
		{
			if(points[i].x != Mathf.Infinity)
			{
				returnPoints[num] = points[i];
				num++;
			}
		}

		return returnPoints;
	}

	public static bool PointOnPerimeter(Vector2[] shape, Vector2 point)
	{
		bool hit = false;
			for(int a = 0; a < shape.Length; a++)
			{
				int b = a + 1 < shape.Length ? a + 1 : 0;
				
				if(JelloVectorTools.CrossProduct(point - shape[a], shape[b] - shape[a]) == 0f)
				{
					if(
						point.x > Mathf.Max (shape[b].x, shape[a].x) ||
						point.x < Mathf.Min (shape[b].x, shape[a].x) || 
						point.y > Mathf.Max (shape[b].y, shape[a].y) ||
						point.y < Mathf.Min (shape[b].y, shape[a].y)
						)
					{
						continue;
					}

					hit = true;
				}
			}

		
		return hit;
	}

	/// <summary>
	/// Get the barycentric coordinates of a point relative to a triangle.
	/// </summary>
	/// <param name="point">The Point to find the barycentric coordinates of.</param>
	/// <param name="triangle">Triangle to test against.</param>
	/// <returns>The barycentric coords of the given point relative to the given triangle.</returns>
	public static float[] GetBarycentricCoords(Vector2 point, Vector2[] triangle)
	{
		float[] barycentricCoords = new float[3];

		barycentricCoords[0] = (triangle[1].y - triangle[2].y) * (point.x - triangle[2].x) + (triangle[2].x - triangle[1].x) * (point.y - triangle[2].y);
		barycentricCoords[0] /= (triangle[1].y - triangle[2].y) * (triangle[0].x - triangle[2].x) + (triangle[2].x - triangle[1].x) * (triangle[0].y - triangle[2].y);
		
		barycentricCoords[1] = (triangle[2].y - triangle[0].y) * (point.x - triangle[2].x) + (triangle[0].x - triangle[2].x) * (point.y - triangle[2].y);
		barycentricCoords[1] /= (triangle[1].y - triangle[2].y) * (triangle[0].x - triangle[2].x) + (triangle[2].x - triangle[1].x) * (triangle[0].y - triangle[2].y);
		
		barycentricCoords[2] = 1 - barycentricCoords[0] - barycentricCoords[1];

		return barycentricCoords;
	}

	/// <summary>
	/// Given a shape and its triangles, find the triangle that contains a point.
	/// If the shape does not contain the point, the 3 closest points are returned.
	/// </summary>
	/// <param name="point">The point to find containment of.</param>
	/// <param name="shape">The shape that the triangles derive from (includes any internal points).</param>
	/// <param name="shapePerimeter">The perimeter of the shape (does not include any internal points).</param>
	/// <param name="triangles">The triangles representing the triangulation of the given shape.</param>
	/// <param name="barycentricCoords">The barycentric coordinates of the given point relative to its containint triangle.</param>
	/// <returns>The containing triangle as a indices of the given shape.</returns>
	public static int[] FindContainingTriangle(Vector2 point, Vector2[] shape, Vector2[] shapePerimeter, int[] triangles, out float[] barycentricCoords)
	{
		barycentricCoords = new float[3];
		Vector2[] tri = new Vector2[3];
		int[] triangleIndices = new int[3];

		if(Contains( shapePerimeter, point))
		{
			for(int i = 0; i < triangles.Length; i+=3)
			{
				tri[0] = shape[triangles[i]];
				tri[1] = shape[triangles[i + 1]];
				tri[2] = shape[triangles[i + 2]];
				
				barycentricCoords[0] = (tri[1].y - tri[2].y) * (point.x - tri[2].x) + (tri[2].x - tri[1].x) * (point.y - tri[2].y);
				barycentricCoords[0] /= (tri[1].y - tri[2].y) * (tri[0].x - tri[2].x) + (tri[2].x - tri[1].x) * (tri[0].y - tri[2].y);
				if(barycentricCoords[0] < 0) //also check if greater than 1?
					continue;
				barycentricCoords[1] = (tri[2].y - tri[0].y) * (point.x - tri[2].x) + (tri[0].x - tri[2].x) * (point.y - tri[2].y);
				barycentricCoords[1] /= (tri[1].y - tri[2].y) * (tri[0].x - tri[2].x) + (tri[2].x - tri[1].x) * (tri[0].y - tri[2].y);
				if(barycentricCoords[1] < 0) //also check if greater than 1?
					continue;
				barycentricCoords[2] = 1 - barycentricCoords[0] - barycentricCoords[1];
				if(barycentricCoords[2] < 0) //also check if greater than 1?
					continue;

				triangleIndices[0] = triangles[i];
				triangleIndices[1] = triangles[i + 1];
				triangleIndices[2] = triangles[i + 2];
				
				break;
			}
		}
		else
		{
			triangles = GetClosestIndices(point, shape, 3);
			
			tri[0] = shape[triangles[0]];
			tri[1] = shape[triangles[1]];
			tri[2] = shape[triangles[2]];
			
			barycentricCoords[0] = (tri[1].y - tri[2].y) * (point.x - tri[2].x) + (tri[2].x - tri[1].x) * (point.y - tri[2].y);
			barycentricCoords[0] /= (tri[1].y - tri[2].y) * (tri[0].x - tri[2].x) + (tri[2].x - tri[1].x) * (tri[0].y - tri[2].y);
			barycentricCoords[1] = (tri[2].y - tri[0].y) * (point.x - tri[2].x) + (tri[0].x - tri[2].x) * (point.y - tri[2].y);
			barycentricCoords[1] /= (tri[1].y - tri[2].y) * (tri[0].x - tri[2].x) + (tri[2].x - tri[1].x) * (tri[0].y - tri[2].y);
			barycentricCoords[2] = 1 - barycentricCoords[0] - barycentricCoords[1];

			triangleIndices[0] = triangles[0];
			triangleIndices[1] = triangles[1];
			triangleIndices[2] = triangles[2];
		}

		return triangleIndices;
	}


	//triangulate
	/// <summary>
	/// Triangulate this shape.
	/// Make sure there are no duplicate points in JelloClosedShape.EdgeVertices or JelloClosedShape.InternalVertices or else the triangulator will fail.
	/// </summary>
	public static int[] Triangulate(Vector2[] shape)
	{	
		JelloPoly2Tri.JelloPolygonPoint[] points = new JelloPoly2Tri.JelloPolygonPoint[shape.Length]; 
		
		for(int i = 0; i < shape.Length; i++)
		{
			points[i] = new JelloPoly2Tri.JelloPolygonPoint((double)shape[i].x, (double)shape[i].y);
			points[i].polygonIndex = i;
		}
		
		JelloPoly2Tri.Polygon poly = new JelloPoly2Tri.Polygon(points);
		
//		points = new JelloPoly2Tri.JelloPolygonPoint[mInternalVertices.Length];
//		for(int i = 0; i < mInternalVertices.Length; i++)
//		{
//			points[i] = new JelloPoly2Tri.JelloPolygonPoint((double)InternalVertices[i].x, (double)InternalVertices[i].y);
//			points[i].polygonIndex = i + shape.Length;
//			poly.AddSteinerPoint(points[i]);
//		}
		
		JelloPoly2Tri.P2T.Triangulate(poly);

		int[] triangles = new int[poly.Triangles.Count * 3];
		
		JelloPoly2Tri.JelloPolygonPoint jpp;
		for(int i = 0; i < poly.Triangles.Count; i++) //reverse triangle winding as well //TODO make sure this works for CCW and CW polygons...
		{
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[2];
			triangles[3 * i + 0] = jpp.polygonIndex;
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[1];
			triangles[3 * i + 1] = jpp.polygonIndex;
			jpp = (JelloPoly2Tri.JelloPolygonPoint)poly.Triangles[i].Points[0];
			triangles[3 * i + 2] =  jpp.polygonIndex;
		}

		return triangles;
	}




	/// <summary>
	/// Get the specefied number of indices on a shape closest to a given point.
	/// </summary>
	/// <param name="point">The point.</param>
	/// <param name="shape">The shape to test against.</param>
	/// <param name="returnNum">The number of closest indices to find.</param>
	/// <returns>The closest indices.</returns>
	public static int[] GetClosestIndices(Vector2 point, Vector2[] shape, int returnNum)
	{
		if(returnNum > shape.Length)
			returnNum = shape.Length;
		int[] indices = new int[returnNum];
		float[] distances = new float[returnNum];

		for(int i = 0; i < returnNum; i++)
			distances[i] = Mathf.Infinity;

		float dist;
		
		for (int i = 0; i < shape.Length; i++)
		{
			dist = (point - shape[i]).sqrMagnitude;
			for(int a = 0; a < returnNum; a++)
			{
				if(dist < distances[a])
				{
					for(int c = returnNum - 1; c > a; c--)
					{
						distances[c] = distances[c - 1];
						indices[c] = indices[c - 1];
					}
					
					distances[a] = dist;
					indices[a] = i;
					break;
				}
			}
		}
		
		return indices;
	}
}

