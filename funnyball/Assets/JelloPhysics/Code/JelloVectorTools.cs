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

/// <summary>
/// Some helpful vector tools.
/// </summary>
public static class JelloVectorTools
{
	#region VECTOR FUNCITONS 

	/// <summary>
	/// Transforms the vector by a position, scale and angle.
	/// </summary>
	/// <param name="point">The point in local space.</param>
	/// <param name="position">The position in global space to transform the position to.</param>
	/// <param name="scale">The scale to be applied to the point.</param>
	/// <param name="angle">The angle by which to rotate the point.</param>
	/// <returns>The point in global space.</returns>
	public static Vector2 transformVector(Vector2 point, Vector2 position, Vector2 scale, float angle)
	{
		point.x = point.x * scale.x;
		point.y = point.y * scale.y;

		rotateVector(ref point, angle);
		
		return point + position;
	}


    /// <summary>
    ///  Rotate a vector by a given angle (in degrees).
    /// </summary>
    /// <param name="vector">The vector to be rotated.</param>
    /// <param name="angle">The angle in degrees to rotate the vector by.</param>
    /// <returns>The rotated vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //fire a cone of bullets
	/// public void GetBulletDirections(Vector2 direction)
	/// {
	/// 	Vector2 directionLeft = JelloVectorTools.rotateVector(direction, -15f);
	/// 	Vector2 directionRight = JelloVectorTools.rotateVector(direction, 15f);
	/// 	
	/// 	FireBullet(direction);
	/// 	FireBullet(directionLeft);
	/// 	FireBullet(directionRight);
	/// }
	/// ~~~
    public static Vector2 rotateVector(Vector2 vector, float angle)
    {
		angle *= Mathf.Deg2Rad;
       
		float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
		
		return new Vector2((c * vector.x) - (s * vector.y), (c * vector.y) + (s * vector.x));
    }

    /// <summary>
	/// Rotate a vector by a given angle (in degrees).
    /// </summary>
	/// <param name="vectorIn">The vector to be rotated.</param>
	/// <param name="angle">The angle in degrees to rotate the vector by.</param>
	/// <param name="vectorOut">The rotated vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //fire a cone of bullets
	/// public void GetBulletDirections(Vector2 direction)
	/// {
	/// 	Vector2 directionLeft = new Vector2();
	/// 	JelloVectorTools.rotateVector(ref direction, -15f, ref directionLeft);
	/// 	
	/// 	Vector2 directionRight = new Vector2();
	/// 	JelloVectorTools.rotateVector(ref direction, 15f, ref directionRight);
	/// 	
	/// 	FireBullet(direction);
	/// 	FireBullet(directionLeft);
	/// 	FireBullet(directionRight);
	/// }
	/// ~~~
    public static void rotateVector(ref Vector2 vectorIn, float angle, ref Vector2 vectorOut)
    {
		angle *= Mathf.Deg2Rad;
       
		float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
       
		vectorOut.x = (c * vectorIn.x) - (s * vectorIn.y);
        vectorOut.y = (c * vectorIn.y) + (s * vectorIn.x);
    }

    /// <summary>
	/// Rotate a vector by a given angle (in degrees).
    /// </summary>
	/// <param name="vectorInOut">The vector to be rotated / The rotated Vector.</param>
	/// <param name="angle">The angle in degrees to rotate the vector by.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //fire a cone of bullets
	/// public void GetBulletDirections(Vector2 direction)
	/// {
	/// 	FireBullet(direction);
	/// 	
	/// 	JelloVectorTools.rotateVector(ref direction, -15f);
	/// 	FireBullet(direction);
	/// 	
	/// 	JelloVectorTools.rotateVector(ref direction, 30f);
	/// 	FireBullet(direction);
	/// }
	/// ~~~
    public static void rotateVector(ref Vector2 vectorInOut, float angle)
    {
		angle *= Mathf.Deg2Rad;
       
		float originalX = vectorInOut.x;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
		
		vectorInOut.x = (c * originalX) - (s * vectorInOut.y);
		vectorInOut.y = (c * vectorInOut.y) + (s * originalX);
	}

    /// <summary>
    ///  Reflect a vector about a normal.  Normal must be a unit vector (have a magnitude of one).
    /// </summary>
    /// <param name="vector">The vector to reflect.</param>
    /// <param name="normal">The normal to reflect the vectory by.</param>
    /// <returns>The reflected vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Bounce a laser off of a mirror
	/// public void BounceLaser(Vector2 laserStart, Vector2 laserMirrorHit, vector2 mirrorStart, Vector2 mirrorEnd)
	/// {
	/// 	Vector2 laserDirection = laserMirrorHit - laserStart;
	/// 	Vector2 mirrorNorm = (mirrorEnd - mirroStart).normalized;
	/// 	
	/// 	Vector2 newLaserDirection = JelloVectorTools.reflectVector(ref laserDirection, ref mirrorNorm);
	/// 	
	/// 	FireLaser(newLaserDirection);
	/// }
	/// ~~~
    public static Vector2 reflectVector(ref Vector2 vector, ref Vector2 normal)
    {
        return vector - (normal * (2f * Vector2.Dot(vector, normal)));
    }

    /// <summary>
	/// Reflect a vector about a normal.  Normal must be a unit vector (have a magnitude of one).
    /// </summary>
	/// <param name="vector">The vector to reflect.</param>
	/// <param name="normal">The normal to reflect the vectory by.</param>
	/// <param name="reflectedVector">The reflected vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Bounce a laser off of a mirror
	/// public void BounceLaser(Vector2 laserStart, Vector2 laserMirrorHit, vector2 mirrorStart, Vector2 mirrorEnd)
	/// {
	/// 	Vector2 laserDirection = laserMirrorHit - laserStart;
	/// 	Vector2 mirrorNorm = (mirrorEnd - mirroStart).normalized;
	/// 	
	/// 	Vector2 newLaserDirection = new Vector2();
	/// 	JelloVectorTools.reflectVector(ref laserDirection, ref mirrorNorm, ref newLaserDirection);
	/// 	
	/// 	FireLaser(newLaserDirection);
	/// }
	/// ~~~
    public static void reflectVector(ref Vector2 vector, ref Vector2 normal, ref Vector2 reflectedVector)
    {
        reflectedVector = vector - (normal * (2f * Vector2.Dot (vector, normal)));
    }

    /// <summary>
    /// Get The vector perpendicular to a given vector.
    /// </summary>
    /// <param name="vector">The source vector.</param>
    /// <returns>The vector perpendicular to the given vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep shield facing to the left of the player
	/// public void GetShieldDirection(Vector2 playerDirection)
	/// {
	/// 	SetShieldDirection(JelloVectorTools.getPerpendicular(playerDirection));
	/// }
	/// ~~~
    public static Vector2 getPerpendicular(Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }
	
	/// <summary>
	/// Get The vector perpendicular to a given vector.
    /// </summary>
	/// <param name="vector">The source vector.</param>
	/// <returns>The vector perpendicular to the given vector.</returns>
    public static Vector2 getPerpendicular(ref Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }
	
    /// <summary>
	/// Get The vector perpendicular to a given vector.
    /// </summary>
	/// <param name="vectorIn">The source vector.</param>
	/// <param name="vectorOut">The vector perpendicular to the given vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep shield facing to the left of the player
	/// public void GetShieldDirection(Vector2 playerDirection)
	/// {
	/// 	Vector2 shieldDirection = new Vector2();
	/// 	JelloVectorTools.getPerpendicular(ref playerDirection, ref shieldDirection);
	///	
	/// 	SetShieldDirection(shieldDirection);
	/// }
	/// ~~~
    public static void getPerpendicular(ref Vector2 vectorIn, ref Vector2 vectorOut)
    {
        vectorOut.x = -vectorIn.y;
        vectorOut.y = vectorIn.x;
    }

    /// <summary>
    /// Make the given vector perpendicular to itself.
    /// </summary>
    /// <param name="vectorInOut">The source vector / The vector perpendicular to the source vector.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep shield facing to the left of the player
	/// public void GetShieldDirection(Vector2 playerDirection)
	/// {
	/// 	JelloVectorTools.makePerpendicular(ref playerDirection);
	/// 	
	/// 	SetShieldDirection(playerDirection);
	/// }
	/// ~~~
    public static void makePerpendicular(ref Vector2 vectorInOut)
    {
        float tempX = vectorInOut.x;
        vectorInOut.x = -vectorInOut.y;
        vectorInOut.y = tempX;
    }

    /// <summary>
    /// Check if rotating from one vector to another is Counter-clockwise.
    /// </summary>
    /// <param name="vectorA">The first vector.</param>
    /// <param name="vectorB">The second vector.</param>
    /// <returns>Whether rotating from one vector to another is counter clockwise.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //move to first point in array that is counter clockwise to current position/direction
	/// public void MoveToFirstCCWNode(Vector2 playerPosition, Vector2 playerDirection, Vector2[] nodes)
	/// {
	/// 	for(int i = 0; i < nodes.Length; i++)
	/// 	{
	/// 		Vector2 nodeDirection = nodes[i] - playerPosition;
	/// 		
	/// 		if(JelloVectorTools.isCCW(playerDirection, nodeDirection))
	/// 		{
	/// 			SetPlayerPosition(playerPosition);
	/// 			return;
	/// 		}
	/// 	}
	/// }
	/// ~~~
    public static bool isCCW(Vector2 vectorA, Vector2 vectorB)
    {
        return (Vector2.Dot (vectorB, getPerpendicular(vectorA)) >= 0f);
    }

    /// <summary>
	/// Check if rotating from one vector to another is Counter-clockwise.
    /// </summary>
    /// <param name="vectorA">The first vector.</param>
    /// <param name="vectorB">The second vector.</param>
	/// <returns>Whether rotating from one vector to another is counter clockwise.</returns>
    public static bool isCCW(ref Vector2 vectorA, ref Vector2 vectorB)
    {
        
        return (Vector2.Dot (vectorB, getPerpendicular(ref vectorA)) >= 0.0f);
    }
	
	/// <summary>
	/// Calculate the cross product of two vectors.
	/// </summary>
	/// <param name="vectorA">The first vector.</param>
	/// <param name="vectorB">The secodn vector.</param>
	/// <returns>The cross product of two vectors.</returns>
	public static float CrossProduct(Vector2 vectorA, Vector2 vectorB)
	{
		return vectorA.x * vectorB.y - vectorA.y * vectorB.x;
	}
	
	/// <summary>
	/// Calculate the dot product of two vectors.
	/// </summary>
	/// <param name="vectorA">The first vector.</param>
	/// <param name="vectorB">The second vector.</param>
	/// <returns>The dot product.</returns>
	public static float DotProduct(Vector2 vectorA, Vector2 vectorB)
	{
		return vectorA.x * vectorB.x + vectorA.y * vectorB.y;
	}

    /// <summary>
    /// Get a Vector3 from a Vector2 (sets z to zero).
    /// </summary>
    /// <param name="vector">The source Vector2.</param>
    /// <returns>The resulting Vector3.</returns>
    public static Vector3 Vec3FromVec2(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    /// <summary>
	/// Get a Vector3 from a Vector2 (sets z to zero).
    /// </summary>
	/// <param name="vector">The source Vector2.</param>
	/// <returns>The resulting Vector3.</returns>
    public static Vector3 vec3FromVec2(ref Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    /// <summary>
	/// Get a Vector3 from a Vector2, setting z to a given float value.
    /// </summary>
	/// <param name="vector">The source Vector2.</param>
    /// <param name="z">The value of the resulting Vector3.z.</param>
	/// <returns>The resulting Vector3.</returns>
    public static Vector3 vec3FromVec2(Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    /// <summary>
	/// Get a Vector3 from a Vector2, setting z to a given float value.
    /// </summary>
	/// <param name="vector">The source Vector2.</param>
	/// <param name="z">The value of the resulting Vector3.z.</param>
	/// <returns>The resulting Vector3.</returns>
    public static Vector3 vec3FromVec2(ref Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
	
	#endregion
	
	#region VECTOR INTERSECTIONS
	
	/// <summary>
    /// Check if two line segments cross each other. (segment AB intesects with segment CD).
    /// </summary>
    /// <param name="pointA">The first point on segment AB.</param>
    /// <param name="pointB">The second point on segment AB.</param>
    /// <param name="pointC">The first point on segment CD.</param>
    /// <param name="pointD">The second point on segment CD.</param>
    /// <returns>Whether the line segments intersect.</returns>
	public static bool LineSegmentsIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
	{
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(pointB - pointA, pointD - pointC);
		float scalarAB = CrossProduct(pointC - pointA, pointD - pointC) * denomRecipricol; 
		float scalarCD = CrossProduct(pointC - pointA, pointB - pointA) * denomRecipricol;
		
		if(0f <= scalarAB && scalarAB <= 1f && 0f <= scalarCD && scalarCD <= 1f)
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Check if two line segments cross each other. (segment AB intesects with segment CD).
	/// </summary>
	/// <param name="pointA">The first point on segment AB.</param>
	/// <param name="pointB">The second point on segment AB.</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
    /// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
	/// <returns>Whether the line segments intersect.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //if two swords are touching play a spark animation at the point of contact
	/// public void CheckSwordSparkAnimation(Vector2 swordOneBase, Vector2 SwordOneTip, Vector2 swordTwoBase, Vector2 SwordTwoTip)
	/// {
	/// 	Vector2 hitPoint = new Vector2();
	/// 	
	/// 	if(JelloVectorTools.LineSegmentsIntersect(swordOneBase, swordOneTip, swordTwoBase, swordTwoTip, out hitPoint))
	/// 	{
	/// 		PlaySparkAnimation(hitPoint);
	/// 	}
	/// }
	/// ~~~
	public static bool LineSegmentsIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint)
	{
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(pointB - pointA, pointD - pointC);
		float scalarAB = CrossProduct(pointC - pointA, pointD - pointC) * denomRecipricol; //pta ptb scalar
		float scalarCD = CrossProduct(pointC - pointA, pointB - pointA) * denomRecipricol; //ptc ptd scalar
		
		if(0f <= scalarAB && scalarAB <= 1f && 0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = pointA + (pointB - pointA) * scalarAB;
			
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if two line segments cross each other. (segment AB intesects with segment CD).
	/// </summary>
	/// <param name="pointA">The first point on segment AB.</param>
	/// <param name="pointB">The second point on segment AB.</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
	/// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
    /// <param name="scalarAB">The normalized distance along segment AB to the point of intersection [0,1].</param>
	/// <param name="scalarCD">The normalized distance along segment CD to the point of intersection [0,1].</param>
	/// <returns>Whether the line segments intersect.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //if two swords are touching play a spark animation at the point of contact
	//and make the spark animation bigger the closer it is to the base of swordOne
	/// public void CheckSwordSparkAnimation(Vector2 swordOneBase, Vector2 SwordOneTip, Vector2 swordTwoBase, Vector2 SwordTwoTip)
	/// {
	/// 	Vector2 hitPoint = new Vector2();
	/// 	Vector2 scalarAB = 0f;
	/// 	Vector2 scalarCD = 0f;
	/// 	
	/// 	if(JelloVectorTools.LineSegmentsIntersect(swordOneBase, swordOneTip, swordTwoBase, swordTwoTip, out hitPoint, out scalarAB, out scalarCD))
	/// 	{
	/// 		float animationSize = 1 - scalarAB;
	/// 		
	/// 		PlaySparkAnimation(hitPoint, animationSize);
	/// 	}
	/// }
	/// ~~~
	public static bool LineSegmentsIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
	{
		hitPoint = Vector2.zero;
		scalarAB = 0f;
		scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(pointB - pointA, pointD - pointC);
		scalarAB = CrossProduct(pointC - pointA, pointD - pointC) * denomRecipricol; //pta ptb scalar
		scalarCD = CrossProduct(pointC - pointA, pointB - pointA) * denomRecipricol; //ptc ptd scalar
		
		if(0f <= scalarAB && scalarAB <= 1f && 0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = pointA + (pointB - pointA) * scalarAB;
			
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line segment. (ray beggining at an orgin towards a direction intersects line segment CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray/</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
	/// <returns>Whether the ray intersects the line segment.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //if a bullet hits a wall, destroy the wall
	/// public void CheckBulletAgainstWall(Vector2 gunBarrelTip, Vector2 gunBarrelDirection, Wall wall)
	/// {
	/// 	
	/// 	if(JelloVectorTools.RayIntersectsSegment(gunBarrelTip, gunBarrelDirection, wall.Top, wall.Bottom))
	/// 	{
	/// 		Destroy(wall);
	/// 	}
	/// }
	/// ~~~
	public static bool RayIntersectsSegment(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD)
	{	
		//parallel lines
		if(CrossProduct(direction, pointD - pointC) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(direction, pointD - pointC);
		float scalarAB = CrossProduct(pointC - origin, pointD - pointC) * denomRecipricol;
		float scalarCD = CrossProduct(pointC - origin, direction) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD && scalarCD <= 1f)
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line segment. (ray beggining at an orgin towards a direction intersects line segment CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray/</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
    /// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
	/// <returns>Whether the ray intersects the line segment.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //if a bullet hits a wall, reduce the size of the wall to the contact point
	/// public void CheckSwordSparkAnimation(Vector2 gunBarrelTip, Vector2 gunBarrelDirection, Wall wall)
	/// {
	/// 	Vector2 hitPoint = new Vector2();
	/// 	
	/// 	if(JelloVectorTools.RayIntersectsSegment(gunBarrelTip, gunBarrelDirection, wall.Top, wall.Bottom, out hitPoint))
	/// 	{
	/// 		wall.top = hitPoint;
	/// 	}
	/// }
	/// ~~~
	public static bool RayIntersectsSegment(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint)
	{	
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(direction, pointD - pointC) == 0f)
			return false;
		
		float denomRecipricol = 1 / CrossProduct(direction, pointD - pointC);
		float scalarAB = CrossProduct(pointC - origin, pointD - pointC) * denomRecipricol;
		float scalarCD = CrossProduct(pointC - origin, direction) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = origin + direction * scalarAB;
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line segment. (ray beggining at an orgin towards a direction intersects line segment CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray/</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
	/// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
	/// <param name="scalarAB">The normalized distance along segment AB to the point of intersection [0,1].</param>
	/// <param name="scalarCD">The normalized distance along segment CD to the point of intersection [0,1].</param>
	/// <returns>Whether the ray intersects the line segment.</returns>
	public static bool RayIntersectsSegment(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
	{	
		hitPoint = Vector2.zero;
		scalarAB = scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(direction, pointD - pointC) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(direction, pointD - pointC);
		scalarAB = CrossProduct(pointC - origin, pointD - pointC) * denomRecipricol;
		scalarCD = CrossProduct(pointC - origin, direction) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = origin + direction * scalarAB;
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if two rays crosses over eachother. (rays beggining at their origin towards their directions intersect each other).
	/// </summary>
	/// <param name="originA">The point at which the first ray begins.</param>
	/// <param name="directionA">The direction of the first ray.</param>
	/// <param name="originC">The point at which the second ray begins.</param>
	/// <param name="directionC">The direction of the second ray.</param>
	/// <returns>Whether the rays intersect.</returns>
	public static bool RaysIntersect(Vector2 originA, Vector2 directionA, Vector2 originC, Vector2 directionC)
	{	
		//parallel lines
		if(CrossProduct(directionA, directionC) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(directionA, directionC);
		float scalarAB = CrossProduct(originC - originA, directionC) * denomRecipricol;
		float scalarCD = CrossProduct(originC - originA, directionA) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD)
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Check if two rays crosses over eachother. (rays beggining at their origin towards their directions intersect each other).
	/// </summary>
	/// <param name="originA">The point at which the first ray begins.</param>
	/// <param name="directionA">The direction of the first ray.</param>
	/// <param name="originC">The point at which the second ray begins.</param>
	/// <param name="directionC">The direction of the second ray.</param>
	/// <param name="hitPoint">The point at which the two rays intersect.</param>
	/// <returns>Whether the rays intersect.</returns>
	public static bool RaysIntersect(Vector2 originA, Vector2 directionA, Vector2 originC, Vector2 directionC, out Vector2 hitPoint)
	{	
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(directionA, directionC) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(directionA, directionC);
		float scalarAB = CrossProduct(originC - originA, directionC) * denomRecipricol;
		float scalarCD = CrossProduct(originC - originA, directionA) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD)
		{
			hitPoint = originA + directionA * scalarAB;
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if two rays crosses over eachother. (rays beggining at their origin towards their directions intersect each other).
	/// </summary>
	/// <param name="originA">The point at which the first ray begins.</param>
	/// <param name="directionA">The direction of the first ray.</param>
	/// <param name="originC">The point at which the second ray begins.</param>
	/// <param name="directionC">The direction of the second ray.</param>
	/// <param name="hitPoint">The point at which the two rays intersect.</param>
    /// <param name="scalarAB">The distance along the first ray to the point of intersection [0, infinity].</param>
    /// <param name="scalarCD">The distance along the second ray to the point of intersection [0, infinity].</param>
	/// <returns>Whether the rays intersect.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //check if two beams of light touch and play a glow animation 
	/// //from the point of contact to the origin closest to the point of contact
	/// public void CheckLightBeamsOverlap(Vector2 lightOneOrigin, Vector2 lightOneDirection, Vector2 lightTwoOrigin, Vector2 lightTwoDirection)
	/// {
	/// 	Vector2 hitPoint = new Vector2();
	/// 	Vector2 scalarAB = 0f;
	/// 	Vector2 scalarCD = 0f;
	/// 	
	/// 	if(JelloVectorTools.RaysIntersect(lightOneOrigin, lightOneDirection, lightTwoOrigin, lightTwoDirection, out hitPoint, out scalarAB, out scalarCD))
	/// 	{
	/// 		Vector2 closestOrigin = scalarAB < scalarCD ? lightOneOrigin: lightTwoOrigin;
	/// 		PlayGlowAnimation(hitPoint, closestOrigin);
	/// 	}
	/// }
	/// ~~~
	public static bool RaysIntersect(Vector2 originA, Vector2 directionA, Vector2 originC, Vector2 directionC, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
	{	
		hitPoint = Vector2.zero;
		scalarAB = scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(directionA, directionC) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(directionA, directionC);
		scalarAB = CrossProduct(originC - originA, directionC) * denomRecipricol;
		scalarCD = CrossProduct(originC - originA, directionA) * denomRecipricol;
		
		if(0f <= scalarAB && 0f <= scalarCD)
		{
			hitPoint = originA + directionA * scalarAB;
			return true;
		}
		
		return false;
	}
			
	/// <summary>
    /// Check if a line crosses over a line segment. (line AB intesects with segment CD).
    /// </summary>
    /// <param name="pointA">The first point on line AB.</param>
    /// <param name="pointB">The second point on line AB.</param>
    /// <param name="pointC">The first point on segment CD.</param>
    /// <param name="pointD">The second point on segment CD.</param>
    /// <returns>Whether the Line intersects the segment.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //keep moving the javelin until it hits the ground
	///
	/// Vector2 groundLineA, groundLineB;
	/// Vector2 javelinTipA, javelinTipB;
	/// bool hit = false;
	///
	/// while(!hit)
	/// {
	/// 	MoveJavelin();
	/// 	
	/// 	if(JelloVectorTools.LineIntersectsSegment(groundLineA, groundLineB, javelinTipA, javelinTipB))
	/// 	{
	/// 		hit = true;
	/// 	}
	/// }
	/// ~~~
    public static bool LineIntersectsSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float scalarCD = CrossProduct(pointC - pointA, pointB - pointA) / CrossProduct(pointB - pointA, pointD - pointC); //ptc ptd scalar
		
		if(0f <= scalarCD && scalarCD <= 1f)
			return true;
		
		return false;
    }

	/// <summary>
	/// Check if a line crosses over a line segment. (line AB intesects with segment CD).
	/// </summary>
	/// <param name="pointA">The first point on line AB.</param>
	/// <param name="pointB">The second point on line AB.</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
    /// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
	/// <returns>Whether the Line intersects the segment.</returns>
	public static bool LineIntersectsSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint)
    {
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float scalarCD = CrossProduct(pointC - pointA, pointB - pointA) / CrossProduct(pointB - pointA, pointD - pointC); //ptc ptd scalar
		
		if(0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = pointC + (pointD - pointC) * scalarCD;
			return true;
		}
		
		return false;
		
    }
	
	/// <summary>
	/// Check if a line crosses over a line segment. (line AB intesects with segment CD).
	/// </summary>
	/// <param name="pointA">The first point on line AB.</param>
	/// <param name="pointB">The second point on line AB.</param>
	/// <param name="pointC">The first point on segment CD.</param>
	/// <param name="pointD">The second point on segment CD.</param>
	/// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
    /// <param name="scalarAB">The distance along AB to the point of intersection [-infinity, infinity].</param>
    /// <param name="scalarCD">The normalized distance along CD to the point of intersection [0,1].</param>
	/// <returns>Whether the Line intersects the segment.</returns>
    public static bool LineIntersectSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
    {
		hitPoint = Vector2.zero;
		scalarAB = scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(pointB- pointA, pointD - pointC);
		scalarAB = CrossProduct(pointC - pointA, pointD - pointC) * denomRecipricol;
		scalarCD = CrossProduct(pointC - pointA, pointB- pointA) * denomRecipricol;
		
		if(0f <= scalarCD && scalarCD <= 1f)
		{
			hitPoint = pointA + (pointB - pointA) * scalarAB;
			return true;
		}

		
		return false;
    }
	
	/// <summary>
    /// Check if two lines cross each other. (line AB intesects with line CD).
    /// </summary>
    /// <param name="pointA">The first point on line AB.</param>
    /// <param name="pointB">The second point on line AB.</param>
    /// <param name="pointC">The first point on line CD.</param>
    /// <param name="pointD">The second point on line CD.</param>
    /// <returns>Whether the lines intersect eachother.</returns>
	public static bool LinesIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
	{
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;
		else
			return true;
	}
	
	/// <summary>
	/// Check if two lines cross each other. (line AB intesects with line CD).
	/// </summary>
	/// <param name="pointA">The first point on line AB.</param>
	/// <param name="pointB">The second point on line AB.</param>
	/// <param name="pointC">The first point on line CD.</param>
	/// <param name="pointD">The second point on line CD.</param>
    /// <param name="hitPoint">The point at which the intersection, if any, occurs</param>
	/// <returns>Whether the lines intersect eachother.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //find where two boundry lines intersect and place a turret at that point
	/// public void PlaceTurretAtBoundryCorner(Vector2 boundryLineA, Vector2 boundryLineB, Vector2 boundryLineC, Vector2 boundryLineC)
	/// {
	/// 	Vector2 hitPoint = new Vector();
	/// 	
	/// 	if(JelloVectorTools.LinesIntersect(boundryLineA, boundryLineB, boundryLineC, boundryLineC, out hitPoint))
	/// 	{
	/// 		PlaceTurretAtPoint(hitPoint);
	/// 	}
	/// }
	/// ~~~
	public static bool LinesIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint)
	{
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;
		
		hitPoint = pointA + (pointB - pointA) * CrossProduct(pointC - pointA, pointD - pointC) / CrossProduct(pointB - pointA, pointD - pointC);
		
		return true;
	}
	
	/// <summary>
	/// Check if two lines cross each other. (line AB intesects with line CD).
	/// </summary>
	/// <param name="pointA">The first point on line AB.</param>
	/// <param name="pointB">The second point on line AB.</param>
	/// <param name="pointC">The first point on line CD.</param>
	/// <param name="pointD">The second point on line CD.</param>
	/// <param name="hitPoint">The point at which the intersection, if any, occurs</param>
    /// <param name="scalarAB">The distance along AB to the point of intersection [-infinity, infinity].</param>
    /// <param name="scalarCD">The distance along CD to the point of intersection [-infinity, infinity].</param>
	/// <returns>Whether the lines intersect eachother.</returns>
	public static bool LinesIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
	{
		hitPoint = Vector2.zero;
		scalarAB = 0f;
		scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(pointB - pointA, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(pointB - pointA, pointD - pointC);
		scalarAB = CrossProduct(pointC - pointA, pointD - pointC) * denomRecipricol; //pta ptb scalar
		scalarCD = CrossProduct(pointC - pointA, pointB - pointA) * denomRecipricol; //ptc ptd scalar
		
		hitPoint = pointA + (pointB - pointA) * scalarAB;
		
		return true;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line. (ray beggining at an orgin towards a direction intersects line CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray.</param>
	/// <param name="pointC">The first point on line CD.</param>
    /// <param name="pointD">The second point on line CD.</param>
	/// <returns>Whether the ray intersects the line.</returns>
	public static bool RayIntersectsLine(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD)
	{
		if(CrossProduct(direction, pointD - pointC) == 0f)
			return false;

		float scalarAB = CrossProduct(pointC - origin, pointD - pointC) / CrossProduct(direction, pointD - pointC);
		
		if(0f <= scalarAB)
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line. (ray beggining at an orgin towards a direction intersects line CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray.</param>
	/// <param name="pointC">The first point on line CD.</param>
	/// <param name="pointD">The second point on line CD.</param>
    /// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
	/// <returns>Whether the ray intersects the line.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //spawn an explosion on the ground where a laser from a sattelite hits
	/// public void checkSpawnExplosion(Vector2 laserOrigin, Vector2 laserDirection, Vector2 groundPointA, Vector2 groundPointB)
	/// {
	/// 	Vector2 hitPoint = new Vector2();
	/// 	
	/// 	if(JelloVectorTools.RayIntersectsLine(laserOrigin, laserDirection, groindPointA, groundPointB, out hitPoint))
	/// 	{
	/// 		spawnExplosion(hitPoint);
	/// 	}
	/// }
	/// ~~~
	public static bool RayIntersectsLine(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint)
	{
		hitPoint = Vector2.zero;
		
		//parallel lines
		if(CrossProduct(direction, (pointD - pointC)) == 0f)
			return false;

		float scalarAB = CrossProduct(pointC - origin, pointD - pointC) / CrossProduct(direction, pointD - pointC); //pta ptb scalar
		
		if(0f <= scalarAB)
		{
			hitPoint = origin + (direction) * scalarAB;
			
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Check if a ray crosses over a line. (ray beggining at an orgin towards a direction intersects line CD).
	/// </summary>
	/// <param name="origin">The point at which the ray begins.</param>
	/// <param name="direction">The direction of the the ray.</param>
	/// <param name="pointC">The first point on line CD.</param>
	/// <param name="pointD">The second point on line CD.</param>
	/// <param name="hitPoint">The point at which the intersection, if any, occurs.</param>
    /// <param name="scalarAB">The distance along the ray to the point of intersection [0, infinity].</param>
    /// <param name="scalarCD">The distance along line CD to the point of intersection [-infinity, infinity].</param>
	/// <returns>Whether the ray intersects the line.</returns>
	public static bool RayIntersectsLine(Vector2 origin, Vector2 direction, Vector2 pointC, Vector2 pointD, out Vector2 hitPoint, out float scalarAB, out float scalarCD)
	{
		hitPoint = Vector2.zero;
		scalarAB = 0f;
		scalarCD = 0f;
		
		//parallel lines
		if(CrossProduct(direction, (pointD - pointC)) == 0f)
			return false;

		float denomRecipricol = 1 / CrossProduct(direction, pointD - pointC);
		scalarAB = CrossProduct(pointC - origin, pointD - pointC) * denomRecipricol; //pta ptb scalar
		scalarCD = CrossProduct(pointC - origin, direction) * denomRecipricol; //ptc ptd scalar
		
		if(0f <= scalarAB)
		{
			hitPoint = origin + (direction) * scalarAB;
			
			return true;
		}
		
		return false;
	}
	
	#endregion
	
	#region SPRING SIMULATIONS


    /// <summary>
    /// Calculate the force of a spring.
    /// </summary>
    /// <param name="positionA">The position of the first end of the spring.</param>
    /// <param name="velocityA">The velocity of the first end of the spring.</param>
    /// <param name="positionB">The position of the second end of the spring.</param>
    /// <param name="velocityB">The velocity of the second end of the spring.</param>
    /// <param name="distance">The rest distance of the spring.</param>
    /// <param name="stiffness">The spring stiffness.</param>
    /// <param name="damping">The spring damping.</param>
    /// <returns>The resulting spring force vector.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	///		//attach two Bodies via spring
	///		
	///		JelloBody bodyA, bodyB;
	///		
	/// 	void FixedUpdate()
	///		{
	///			Vector2 force = JelloVectorTools.CalculateSpringForce(bodyA.Position, bodyA.Velocity, bodyB.Position, bodyB.Velocity, 0f, 100f, 10f);
	///	
	///			bodyA.AddForce(force);
	///			bodyB.AddForce(-force);
	/// 	}
	/// ~~~
    public static Vector2 calculateSpringForce(Vector2 positionA, Vector2 velocityA, Vector2 positionB, Vector2 velocityB, float distance, float stiffness, float damping)
    {
		if(positionA == positionB)
			return Vector2.zero;

		float dist = Vector2.Distance(positionA, positionB);
		Vector2 BtoA = (positionA - positionB) / dist;
        
        dist = distance - dist;

        return BtoA * ((dist * stiffness) - (Vector2.Dot(velocityA - velocityB, BtoA) * damping));  
    }

	/// <summary>
	/// Calculate the force of a spring.
	/// </summary>
	/// <param name="positionA">The position of the first end of the spring.</param>
	/// <param name="velocityA">The velocity of the first end of the spring.</param>
	/// <param name="positionB">The position of the second end of the spring.</param>
	/// <param name="velocityB">The velocity of the second end of the spring.</param>
	/// <param name="distance">The rest distance of the spring.</param>
	/// <param name="stiffness">The spring stiffness.</param>
	/// <param name="damping">The spring damping.</param>
	/// <param name="force">The resulting spring force vector.</returns>
    public static void calculateSpringForce(ref Vector2 positionA, ref Vector2 velocityA, ref Vector2 positionB, ref Vector2 velocityB, float distance, float stiffness, float damping, ref Vector2 force)
    {
        if(positionA == positionB)
		{
            force = Vector2.zero;
            return;
        }

		float dist = Vector2.Distance(positionA, positionB);
		Vector2 BtoA = (positionA - positionB) / dist;

        dist = distance - dist;

        force = BtoA * ((dist * stiffness) - (Vector2.Dot(velocityA - velocityB, BtoA) * damping));
    }
	
	#endregion
	
	#region CLOSEST POINT ON VECTOR
	
	/// <summary>
    /// Find the squared distance from a point to the closest point on a given segment.
    /// </summary>
    /// <param name="pointA">The point.</param>
    /// <param name="pointB">The first point of the segment to test against.</param>
    /// <param name="pointC">The second point of the segment to test against.</param>
    /// <param name="hitPoint">The point on the given segment closest to the given point.</param>
    /// <param name="normal">The normal of the given edge.</param>
    /// <param name="scalarBC">The normalized distance along the given segment to the closest point [0,1].</param>
    /// <returns>The squared distance from the point to the closest distance on the segment.</returns>
    public static float getClosestPointOnSegmentSquared(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint, out Vector2 normal, out float scalarBC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;
		
		// normal
		normal = JelloVectorTools.getPerpendicular(E);

        // calculate the distance!
		scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
            scalarBC = 0f;
			return (pointA - pointB).sqrMagnitude;
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
            scalarBC = 1f;
			return (pointA - pointC).sqrMagnitude;
        }
        else
        {
            // point lies somewhere on the line segment.
			float dist = JelloVectorTools.CrossProduct(pointA - pointB, E);
			dist *= dist; //if were negative, it would be multiplied out here, so no need for mathf.abs
			hitPoint = pointB + E * scalarBC;
            scalarBC /= edgeLength;
			return dist;
        }
    }
	
	/// <summary>
	/// Find the squared distance from a point to the closest point on a given segment.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="hitPoint">The point on the given segment closest to the given point.</param>
	/// <param name="scalarBC">The normalized distance along the given segment to the closest point [0,1].</param>
	/// <returns>The squared distance from the point to the closest distance on the segment.</returns>
    public static float getClosestPointOnSegmentSquared(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint, out float scalarBC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
            scalarBC = 0f;
			return (pointA - pointB).sqrMagnitude;
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
            scalarBC = 1f;
			return (pointA - pointC).sqrMagnitude;
        }
        else
        {
            // point lies somewhere on the line segment.
			float dist = JelloVectorTools.CrossProduct(pointA - pointB, E);
			dist *= dist; //if were negative, it would be multiplied out here, so no need for mathf.abs
			hitPoint = pointB + E * scalarBC;
            scalarBC /= edgeLength;
			return dist;
        }
    }
			
	/// <summary>
	/// Find the squared distance from a point to the closest point on a given segment.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="hitPoint">The point on the given segment closest to the given point.</param>
	/// <returns>The squared distance from the point to the closest distance on the segment.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //make a missile home towards the closest point on a shield
	/// public void MoveMissileToShield(Vector2 missilePosition, Vector2 shieldStartPosition, Vector2 shieldEndPosition, float deltaTime)
	/// {
	/// 	Vector2 hitPoint;
	///	
	/// 	JelloVectorTools.getClosestPointOnSegmentSquared(missilePosition, shieldStartPosition, shieldEndPosition, out hitPoint);
	///	
	/// 	Vector2 directionToShield = (hitPoint - missilePosition).normalized;
	///	
	/// 	missilePosition += directionToShield * deltaTime;
	/// }
	/// ~~~
    public static float getClosestPointOnSegmentSquared(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		float scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
			return (pointA - pointB).sqrMagnitude;
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
			return (pointA - pointC).sqrMagnitude;
        }
        else
        {
            // point lies somewhere on the line segment.
			hitPoint = pointB + E * scalarBC;
			float dist = JelloVectorTools.CrossProduct(pointA - pointB, E);
			dist *= dist; //if were negative, it would be multiplied out here, so no need for mathf.abs
			return dist;
        }
    }
	
	/// <summary>
	/// Find the distance from a point to the closest point on a given segment.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="hitPoint">The point on the given segment closest to the given point.</param>
	/// <param name="normal">The normal on of the given edge.</param>
	/// <param name="scalarBC">The normalized distance along the given segment to the closest point [0,1].</param>
	/// <returns>The distance from the point to the closest distance on the segment.</returns>
    public static float getClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint, out Vector2 normal, out float scalarBC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;
		
		// normal
		normal = JelloVectorTools.getPerpendicular(E);

        // calculate the distance!
		scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
            scalarBC = 0f;
			return Vector2.Distance(pointA, pointB);
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
            scalarBC = 1f;
			return Vector2.Distance(pointA, pointC);
        }
        else
        {
            // point lies somewhere on the line segment.
			hitPoint = pointB + E * scalarBC;
            scalarBC /= edgeLength;
			return Mathf.Abs (JelloVectorTools.CrossProduct(pointA - pointB, E));
        }
    }
	
	/// <summary>
	/// Find the distance from a point to the closest point on a given segment.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="hitPoint">The point on the given segment closest to the given point.</param>
	/// <param name="scalarBC">The normalized distance along the given segment to the closest point [0,1].</param>
	/// <returns>The distance from the point to the closest distance on the segment.</returns>
    public static float getClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint, out float scalarBC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
            scalarBC = 0f;
			return Vector2.Distance(pointA, pointB);
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
            scalarBC = 1f;
			return Vector2.Distance(pointA, pointC);
        }
        else
        {
            // point lies somewhere on the line segment.
			hitPoint = pointB + E * scalarBC;
            scalarBC /= edgeLength;
			return Mathf.Abs (JelloVectorTools.CrossProduct(pointA - pointB, E));
        }
    }
	
	/// <summary>
	/// Find the distance from a point to the closest point on a given segment.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="hitPoint">The point on the given segment closest to the given point.</param>
	/// <returns>The distance from the point to the closest distance on the segment.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //make a missile home towards the closest point on a shield
	/// public void MoveMissileToShield(Vector2 missilePosition, Vector2 shieldStartPosition, Vector2 shieldEndPosition, float deltaTime)
	/// {
	/// 	Vector2 hitPoint;
	///	
	/// 	JelloVectorTools.getClosestPointOnSegment(missilePosition, shieldStartPosition, shieldEndPosition, out hitPoint);
	/// 	
	/// 	Vector2 directionToShield = (hitPoint - missilePosition).normalized;
	///	
	/// 	missilePosition += directionToShield * deltaTime;
	/// }
	/// ~~~
    public static float getClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		float scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            hitPoint = pointB;
			return Vector2.Distance(pointA, pointB);
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            hitPoint = pointC;
			return Vector2.Distance(pointA, pointC);
        }
        else
        {
            // point lies somewhere on the line segment.
			hitPoint = pointB + E * scalarBC;
			return Mathf.Abs (JelloVectorTools.CrossProduct(pointA - pointB, E));
        }
    }
	
	/// <summary>
	/// Find the closest point on the given line segment to the given point.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <param name="scalarBC">The normalized distance along the given segment to the closest point [0,1].</param>
	/// <returns>The point on the given segment closest to the given point.</returns>
    public static Vector2 getClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC, out float scalarBC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            scalarBC = 0f;
			return pointB;
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            scalarBC = 1f;
			return pointC;
        }
        else
        {
            // point lies somewhere on the line segment.
            scalarBC /= edgeLength;
			return pointB + E * scalarBC;
        }
    }
	
	/// <summary>
	/// Find the closest point on the given line segment to the given point.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the segment to test against.</param>
	/// <param name="pointC">The second point of the segment to test against.</param>
	/// <returns>The point on the given segment closest to the given point.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //make a missile home towards the closest point on a shield
	/// public void MoveMissileToShield(Vector2 missilePosition, Vector2 shieldStartPosition, Vector2 shieldEndPosition, float deltaTime)
	/// {
	/// 	Vector2 directionToShield = (JelloVectorTools.getClosestPointOnSegment(missilePosition, shieldStartPosition, shieldEndPosition) - missilePosition).normalized;
	///	
	/// 	missilePosition += directionToShield * deltaTime;
	/// }
	/// ~~~
    public static Vector2 getClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;

        // calculate the distance!
		float scalarBC = Vector2.Dot(pointA - pointB, E);
        if (scalarBC <= 0.0f)
        {
            // x is outside the line segment, distance is from pt to ptA.
            return pointB;
        }
        else if (scalarBC >= edgeLength)
        {
            // x is outside of the line segment, distance is from pt to ptB.
            return pointC;
        }
        else
        {
            // point lies somewhere on the line segment.
			return pointB + E * scalarBC;
        }
    }
	
	/// <summary>
	/// Find the closest point on the given line to the given point.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the line to test against.</param>
	/// <param name="pointC">The second point of the line to test against.</param>
	/// <returns>The point on the given line closest to the given point.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	///	//display a shadow animation on the ground immediately below the plane
	/// public void DisplayShadowAnimation(Vector2 planePosition, Vector2 groundLineA, Vector2 groundLineB)
	/// {
	/// 	PlayShadowAnimationAtPoint(JelloVectorTools.getClosestPointOnLine(planePosition, groundLineA, groundLineB));
	/// }
	///~~~
	public static Vector2 getClosestPointOnLine(Vector2 pointA, Vector2 pointB, Vector2 pointC)
	{	
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;
		
		return pointB + E * Vector2.Dot (pointA- pointB, E);
	}

	/// <summary>
	/// Find the distance to the closest point on the given line to the given point.
	/// </summary>
	/// <param name="pointA">The point.</param>
	/// <param name="pointB">The first point of the line to test against.</param>
	/// <param name="pointC">The second point of the line to test against.</param>
	/// <param name="hitPoint">The point on the given line closest to the given point.</param>
	/// <returns>The distance to the point on the line.</returns>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	///	//display a shadow animation on the ground immediately below the plane
	///	public void DisplayShadowAnimation(Vector2 planePosition, Vector2 groundLineA, Vector2 groundLineB)
	///	{
	///		Vector2 hitPoint;
	///		
	///		JelloVectorTools.getClosestPointOnLine(planePosition, groundLineA, groundLineB, out hitPoint);
	///		
	///		PlayShadowAnimationAtPoint(hitPoint);
	///	}
	///~~~
	public static float getClosestPointOnLine(Vector2 pointA, Vector2 pointB, Vector2 pointC, out Vector2 hitPoint)
	{	
		Vector2 E = pointC - pointB;
		
		// get the length of the edge, and use that to normalize the vector.
		float edgeLength = Vector2.Distance(pointC, pointB);
		if (edgeLength > 0f)
			E /= edgeLength;
		
		hitPoint = pointB + E * Vector2.Dot (pointA- pointB, E);
		
		return Mathf.Abs (JelloVectorTools.CrossProduct(pointA - pointB, E));
	}

	#endregion
}