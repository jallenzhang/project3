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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The JelloWorld class represents the physics world. It keeps track of all JelloBody objects, coordinating integration, collision, etc.
/// </summary>
[Serializable]
public class JelloWorld : MonoBehaviour
{
#region PUBLIC VARIABLES

	/// <summary>
	/// The number of iterations per FixedUpdate() for solving forces, velocities and positions.
	/// </summary>
	public int Iterations = 5;

	/// <summary>
	/// Whether to show JelloPointMass.velocity in the debug visualization.
	/// </summary>
	public bool showVelocities;

	/// <summary>
	/// Whether to show JelloPointMass.Position by drawing an outline of the JelloBody in the debug visualization.
	/// </summary>
	public bool showPointMassPositions;

	/// <summary>
	/// Whether to show the unscaled and unrotated JelloBody.Shape by drawing its outline in the debug visualization.
	/// </summary>
	public bool showBaseShape;

	/// <summary>
	/// Whether to show the scaled and rotated JelloBody.Shape by drawing its outline in the debug visualization.
	/// </summary>
	public bool showXformedBaseShape;

	/// <summary>
	/// Whether to show each JelloSpring in the debug visualization..
	/// </summary>
	public bool showSprings;

	/// <summary>
	/// Whether to show pressure normals (Direction of pressure force on JelloPointMass) in the debug visualization.
	/// </summary>
	public bool showPressureNormals;

	/// <summary>
	/// Whether to show shape matching springs in the debug visualization.
	/// </summary>
	public bool showShapeMatching;

	/// <summary>
	/// The default PhysicsMaterial2D used in resolving collisions.
	/// </summary>
	public PhysicsMaterial2D defaultPhysicsMaterial;

	/// <summary>
	/// The maximum penetration before a JelloPointMass is considered "too deep" and the potential JelloContact is ignored.
	/// </summary>
	public float PenetrationThreshold = 0.5f;

	/// <summary>
	/// The maximum tangent penetration before a JelloContact will ignore its exact JelloContact.hitPoint and use the closest point on line method for calculating JelloContact.hitPoint.
	/// </summary>
	public float TangentPenetrationThreshold = 0.05f;

	/// <summary>
	/// The velocity at which a JelloBody qualifiies to be put to sleep.
	/// </summary>
	public float sleepVelocity = 1f;//TODO consider making sure each point mass is less than the velocity...
	
	/// <summary>
	/// The angular velocity at which a JelloBody qualifiies to be put to sleep.
	/// </summary>
	public float sleepAngularVelocity = 0.1f;

	/// <summary>
	/// The velocity required by a colliding body to wake up a sleeping body
	/// </summary>
	public float wakeVelocity = 0.75f;

	/// <summary>
	/// The angular velocity required by a colliding body to wake up a sleeping body
	/// </summary>
	public float wakeAngularVelocity = 0.1f;

	/// <summary>
	/// The number of iterations required for a JelloBody to be at/below JelloWorld.sleepVelocity and JelloWorld.sleepAngularVelocity in order to fall asleep.
	/// </summary>
	public int sleepCntRequired = 200; 

	/// <summary>
	/// The minimum translation vector modifier for all collisions.
	/// When collisions occur, the JelloPointMass objects invloved are moved away from each until no longer penetrating before the collision impulse is applied.
	/// This modifier will increase or decrease the distance that the JelloPointMass objects are pushed (a negative value will leave some penetration.). 
	/// </summary>
	public float mtvModifierGlobal = 0.001f;

	#endregion

	#region PUBLIC PROPERTIES

	/// <summary>
	/// Get the staic JelloWorld.
	/// If unset, will first search the scene for a GameObject named "Jello World" with a JelloWorld Component attached to it.
	/// If the GameObject named "Jello World" does not exist, one will be created and the JelloWorld Component will be attached to it.
	/// </summary>
	/// <value>The static JelloWorld object.</value>
	public static JelloWorld World
	{
		get
		{
			if(world == null)
			{
				if(GameObject.Find ("Jello World") != null)
				{
					if(GameObject.Find ("Jello World").GetComponent<JelloWorld>() != null)
					{
						world = GameObject.Find ("Jello World").GetComponent<JelloWorld>();
					}
					else
					{
						GameObject.Find ("Jello World").AddComponent<JelloWorld>();
						
						world = GameObject.Find ("Jello World").GetComponent<JelloWorld>();
					}
				}
				else
				{
					GameObject jelloWorld = new GameObject();
					jelloWorld.name = "Jello World";
					jelloWorld.AddComponent<JelloWorld>();
					world = jelloWorld.GetComponent<JelloWorld>();
				}
			}
			
			return world;
		}
	}

	#endregion

	#region PRIVATE VARIABLES

	/// <summary>
	/// The JelloWorld that all bodies belong to.
	/// </summary>
	protected static JelloWorld world;

	/// <summary>
	/// The JelloBody objects that this JelloWorld is simulating.
	/// </summary>
	private List<JelloBody> mBodies = new List<JelloBody>();

	/// <summary>
	/// The JelloJoint objects that this JelloWorld is simulating.
	/// </summary>
	private List<JelloJoint> mJoints = new List<JelloJoint>();

	#endregion


	#region COLLIDER TRACKING
	//TODO work on this! Try to never remove some of them?
	
	/// <summary>
	/// How often to check for null/invalid SupplementaryColliderInfo objects. (so it can be removed.)
	/// </summary>
	public float colliderTrackingCleanupFrequency = 10f;
	
	/// <summary>
	/// The tracked Collider2D objects.
	/// Colliders are added to this list when they collide with a JelloBody and are removed after a period of not colliding with a JelloBody.
	/// </summary>
	private List<Collider2D> trackedColliders = new List<Collider2D>();
	
	
	//TODO why not just create a separate class? Or jut add the collider2d to the supplementaryColliderInfo class? because searching if something already exists would be a pain maybe???
	/// <summary>
	/// Dictionary correlating the Collider2D objects in the JelloWorld.trackedColliders list to the SupplementaryColliderInfo stored about each of them them.
	/// </summary>
	public Dictionary<int, SupplementaryColliderInfo> TrackedColliderDictionary = new Dictionary<int, SupplementaryColliderInfo>();
	
	/// <summary>
	/// Start tracking the Collider2D.
	/// Adds it to the JelloWorld.trackedColliders list and JelloWorld.TrackedColliderDictionary.
	/// </summary>
	/// <param name="collider">The Collider2D to start tracking.</param>
	/// <param name="jelloBody">The JelloBody, if any, the Collider2D belongs to.</param>
	public void StartTrackingCollider(Collider2D collider, JelloBody jelloBody = null)
	{
		if(trackedColliders.Contains(collider))
			return;
		
		trackedColliders.Add (collider);
		
		TrackedColliderDictionary.Add (collider.GetInstanceID(), new SupplementaryColliderInfo(collider, jelloBody));
	}

	/// <summary>
	/// Start tracking the Collider2D.
	/// Adds it to the JelloWorld.trackedColliders list and JelloWorld.TrackedColliderDictionary.
	/// </summary>
	/// <param name="collider">The Collider2D to start tracking.</param>
	/// <param name="colliderInfo">The SupplementaryColliderInfo that describes the now tracked Collider2D.</param>
	/// <param name="jelloBody">The JelloBody, if any, the Collider2D belongs to.</param>
	/// <returns>Whether the collider is already being tracked. True if already being tracked, false if not.</returns>
	public bool StartTrackingCollider(Collider2D collider, out SupplementaryColliderInfo colliderInfo, JelloBody jelloBody = null)
	{
		if(trackedColliders.Contains(collider))
		{
			colliderInfo = World.TrackedColliderDictionary[collider.GetInstanceID()];
			return true;
		}
		
		trackedColliders.Add (collider);
		colliderInfo = new SupplementaryColliderInfo(collider, jelloBody);
		TrackedColliderDictionary.Add (collider.GetInstanceID(), colliderInfo);
		return false;
		
	}
	
	/// <summary>
	/// Stop tracking the Collider2D.
	/// </summary>
	/// <param name="collider">The Collider2D to stop tracking.</param>
	public void StopTrackingCollider(Collider2D collider)
	{
		if(!trackedColliders.Contains(collider))
			return;
		
		trackedColliders.Remove(collider);
		TrackedColliderDictionary.Remove(collider.GetInstanceID());
	}
	
	/// <summary>
	/// Clean up JelloWorld.trackedColliders and JelloWorld.TrackedColliderDictionary of any null entries.
	/// </summary>
	/// <returns>IEnumerator.</returns>
	IEnumerator CleanUpColliderTracker()
	{
		while(true)
		{
			yield return new WaitForSeconds(colliderTrackingCleanupFrequency);
			
			int numNull = 0;
			
			for(int i = trackedColliders.Count - 1; i > -1; i--)
			{
				if(trackedColliders[i] == null)
				{
					trackedColliders.RemoveAt(i);
					numNull++;
				}
			}
			
			if(numNull == 0)
				continue;
			
			List<int> keysToRemove = new List<int>();
			
			foreach(var key in TrackedColliderDictionary.Keys)
				if(TrackedColliderDictionary[key].collider == null)
					keysToRemove.Add (key);
			
			
			for(int i = 0; i < keysToRemove.Count; i++)
				TrackedColliderDictionary.Remove(keysToRemove[i]);
		}
	}
	
	#endregion

#region ADDING / REMOVING BODIES
	
	/// <summary>
	/// Add a JelloBody to the JelloWorld.
	/// </summary>
	/// <param name="body">The JelloBody to be added.</param>
	public void addBody(JelloBody body)
	{
		//exit if this body is already here
	    if (mBodies.Contains(body))
				return;
			
			//if the body shares a gameobject with one of the bodies already in this world, remove the body that is already there
			for(int i = 0; i < mBodies.Count; i++)
				if(mBodies[i].gameObject == body.gameObject)
					removeBody(mBodies[i]);
			
			if(!body.overrideWorldGravity)
				body.gravity = Physics2D.gravity;
			
			mBodies.Add(body);
	}

	/// <summary>
	/// Remove a JelloBody from the JelloWorld.
	/// </summary>
	/// <param name="body">The JelloBody to remove.</param>
	public void removeBody(JelloBody body)
	{
	    if (mBodies.Contains(body))
	        mBodies.Remove(body);
	}

	/// <summary>
	/// Get a JelloBody at a specific index.
	/// </summary>
	/// <param name="index">The index of the JelloBody.</param>
	/// <returns>The JelloBody at the index.</returns>
	public JelloBody getBody(int index)
	{
	    if ((index >= 0) && (index < mBodies.Count))
	        return mBodies[index];
	    return null;
	}
		
	#endregion

	#region ADDING / REMOVING BODIES

	/// <summary>
	/// Add a JelloJoint to the JelloWorld.
	/// </summary>
	/// <param name="joint">The JelloJoint to be added.</param>
	public void addJoint(JelloJoint joint)
	{
		//exit if this body is already here
		if (mJoints.Contains(joint))
			return;

		mJoints.Add(joint);
	}

	/// <summary>
	/// Remove the JelloJoint from the JelloWorld.
	/// </summary>
	/// <param name="joint">The JelloJoint to remove.</param>
	public void removeJoint(JelloJoint joint)
	{
		if (mJoints.Contains(joint))
		{
			mJoints.Remove(joint);
			if(joint.bodyA != null)
				joint.bodyA.RemoveJoint(joint);
			if(joint.bodyB != null)
				joint.bodyB.RemoveJoint(joint);
		}
	}

	/// <summary>
	/// Get the JelloJoint at the given index.
	/// </summary>
	/// <param name="index">The index of the JelloJoint.</param>
	/// <returns>The JelloJoint at the given index.</returns>
	public JelloJoint getJoint(int index)
	{
		if ((index >= 0) && (index < mJoints.Count))
			return mJoints[index];
		return null;
	}

	#endregion

	#region BODY HELPERS
		
	/// <summary>
	/// Find the closest JelloPointMass in the JelloWorld to a given point.
	/// </summary>
	/// <param name="point">The global point to find closest to.</param>
	/// <param name="bodyIndex">The index of the JelloBody that contains the JelloPointMass.</param>
	/// <param name="pointMassIndex">The index of the JelloPointMass.</param>
	public void getClosestPointMass(Vector2 point, out int bodyIndex, out int pointMassIndex)
	{
	    bodyIndex = -1;
	    pointMassIndex = -1;
		float closestD = Mathf.Infinity;
	    
		for (int i = 0; i < mBodies.Count; i++)
	    {
	        float dist = 0f;
	        
			int pm = mBodies[i].getClosestPointMass(point, out dist);
	        
			if (dist < closestD)
	        {
	            closestD = dist;
	            bodyIndex = i;
	            pointMassIndex = pm;
	        }
	    }
	}
		
	/// <summary>
	/// Find the closest JelloPointMass in the JelloWorld to a given point.
	/// </summary>
	/// <param name="point">The global point to find closest to.</param>
	/// <returns>The closest JelloPointMass to the given global point.</returns>
	public JelloPointMass getClosestPointMass(Vector2 point)
	{
	    int bodyIndex = -1;
	    int pointMassIndex = -1;
			float closestD = Mathf.Infinity;
	    
			for (int i = 0; i < mBodies.Count; i++)
	    {
	        float dist = 0f;
	        
				int pm = mBodies[i].getClosestPointMass(point, out dist);
	        
				if (dist < closestD)
	        {
	            closestD = dist;
	            bodyIndex = i;
	            pointMassIndex = pm;
	        }
	    }
			
			return mBodies[bodyIndex].getEdgePointMass(pointMassIndex);
	}

	/// <summary>
	/// Given a global point, get the first JelloBody (if any) that contains this point.
	/// Useful for picking objects with a cursor, etc.
	/// </summary>
	/// <param name="point">The global point to check for containment of.</param>
	/// <returns>The first JelloBody found to contain the given globabl point. Null if no JelloBody contains the given point.</returns>
	public JelloBody getBodyContaining(Vector2 point)
	{
	    for (int i = 0; i < mBodies.Count; i++)
	        	if (mBodies[i].contains(ref point))
	            	return mBodies[i];
			
	    return null;
	}
		
	/// <summary>
	/// Given a global point, get the JelloBody objects (if any) that contain this point.
	/// Useful for picking objects with a cursor, etc.
	/// </summary>
	/// <param name="point">The global point to check for containment of.</param>
	/// <returns>An array of JelloBody objects that contain the given global point. Null if no JelloBody contains the given point.</returns>
	public JelloBody[] getBodiesContaining(Vector2 point)
	{
			List<JelloBody> bodies = new List<JelloBody>();
	    for (int i = 0; i < mBodies.Count; i++)
	        	if (mBodies[i].contains(point))
	            	bodies.Add(mBodies[i]);
			
	    if(bodies.Count > 0)
				return bodies.ToArray();
			else
				return null;
	}
	
#endregion

#region UPDATE
	
	void Awake()
	{
		//set a new default physics material if not is assigned in the editor
		if (defaultPhysicsMaterial == null)
			defaultPhysicsMaterial = new PhysicsMaterial2D();

		//Clear all lists for the simulation
		mBodies.Clear();
		trackedColliders.Clear();
		TrackedColliderDictionary.Clear();
		mJoints.Clear();

		//start the collider tracker cleanup coroutine (will run as long as the simulation is running)
		//removes any null values from the collider tracker.
		StartCoroutine(CleanUpColliderTracker());
	}

	#if UNITY_EDITOR
	void Update()
	{

		//these values can be set in the editor.
		if 
		(
			showVelocities ||
			showPointMassPositions ||
		    showBaseShape ||				
		    showXformedBaseShape ||
		    showSprings ||							
		    showPressureNormals ||		
		    showShapeMatching			
		)
			debugDrawAllBodies();

	}
	#endif

	void FixedUpdate()
	{
		//advance the simulation over the fixed delta time.
		update (Time.fixedDeltaTime, Iterations);
	}
	
	/// <summary>
	/// Update the JelloWorld a number of times within a specific amount of time.
	/// </summary>
	/// <param name="deltaTime">The amount of time to advance the simulation by (in seconds).</param>
	/// <param name="iterations">The number of times to subdivide deltaTime by for force accumulation and integration.</param>
	private void update(float deltaTime, int iterations)
    {
		//solve all joints
		//have to do this once per fixed update since rigidbodies positions are updated only once per frame and will not play nicely with the joints otherwise
		for(int i = 0; i < mJoints.Count; i++)
			mJoints[i].Solve(deltaTime);

		//split up the time into smaller chunks for more stable/accurate simulation
		deltaTime /= iterations;

		for(int n = mBodies.Count - 1; n >= 0; n--)
		{
			if(mBodies[n] == null)
				mBodies.RemoveAt(n);


			//check if any bodies have fallen asleep
			if(!mBodies[n].IsKinematic)
			{
				if(mBodies[n].velocity.sqrMagnitude < sleepVelocity * sleepVelocity && Mathf.Abs(mBodies[n].angularVelocity) < sleepAngularVelocity)
					mBodies[n].sleepCount += 1;
				else
					mBodies[n].sleepCount = 0;
				
				if(mBodies[n].sleepCount >= sleepCntRequired)
				{
					mBodies[n].IsAwake = false;//update collider here and store collisions into prevois collisions and remove velocity from points???? maybe...
					continue;
				}
			}

			if(mBodies[n].disabled || mBodies[n].IsStatic || !mBodies[n].IsAwake)
				continue;

			mBodies[n].accumulateExternalForces();							//gravity force
		}

		for(int t = 0; t < iterations; t++)
		{
			for(int n = 0; n < mBodies.Count; n++)
			{
				if(mBodies[n].disabled || mBodies[n].IsStatic || !mBodies[n].IsAwake)
					continue;

				mBodies[n].accumulateInternalForces(deltaTime);				//spring and pressure forces.
				mBodies[n].integrate(deltaTime);										//actually convert the force into change in velocity and velocity into change in position
				mBodies[n].ClearForces();													//clear forces to start fresh for the next iteration
				mBodies[n].derivePositionAndAngle(deltaTime);				//calculate the new position and angle by comparing the base shape to the current point mass positions.
			}
		}

		//only call this once per fixed update
		for(int n = 0; n < mBodies.Count; n++)
		{
		
			if(!mBodies[n].IsAwake)
				continue;

			mBodies[n].previousCollisions = mBodies[n].collisions.ToArray(); //store the collisions to assist with the sleeping system
			mBodies[n].collisions.Clear();															//clear collisions to start fresh in the next fixedUpdate

			if(mBodies[n].disabled || mBodies[n].IsStatic)
				continue;

			mBodies[n].ClearExternalForces();
			mBodies[n].UpdateCollider();															//set the collider vertices to the current point mass positions so that box2d has the correct shape when it comes time to process collisions

			//point masses that have forceInternal enabled are forced back within the perimeter of their body
			for(int p = 0; p < mBodies[n].InternalPointMassCount; p++) //TODO optimise this!
			{
				if(mBodies[n].getInternalPointMass(p).forceInternal && !JelloShapeTools.Contains(mBodies[n].polyCollider.points, mBodies[n].getInternalPointMass(p).LocalPosition))
				{
					Vector2 hitPoint;
					Vector2 norm;
					int a;
					int b;
					float scalar;
					mBodies[n].getClosestEdgePoint(mBodies[n].getInternalPointMass(p).Position,out hitPoint, out norm, out a, out b, out scalar);

					mBodies[n].getInternalPointMass(p).Position = hitPoint;
				}
			}

			//update attach points.
			mBodies[n].UpdateAttachPoints();
		}

		//update duration in the collider tracker and remove any colliders + info that have gone too long since their last collision
		for(int i = trackedColliders.Count - 1; i > -1; i--)
		{
			int id = trackedColliders[i].GetInstanceID();

			if(!TrackedColliderDictionary.ContainsKey(id))//will be cleaned up in the cleanup coroutine.
				continue;

			TrackedColliderDictionary[id].TimeSinceLastUpdate += Time.fixedDeltaTime;

			if(TrackedColliderDictionary[id].TimeSinceLastUpdate > 60f)
			{
				TrackedColliderDictionary.Remove(id);
				trackedColliders.Remove(trackedColliders[i]);
			}
		}
	}

	
#endregion

#region COLLISION CHECKS / RESPONSE
	
	/// <summary>
	/// Generates the fine details between two known-colliding Collider2D objects.
	/// One collider will be a CircleCollider2D and the other will be a PolygonCollider2D that belongs to a JelloBody.
	/// </summary>
	/// <param name="collA">The CircleCollider2D involved in the collision.</param>
	/// <param name="info">The SupplementartyColliderInfo representing the PolygonCollider2D that belongs to a JelloBody involved in the collision.</param>
	/// <param name="contacts">The JelloContact list to add any generated JelloContact to.</param>
	public void BodyCollide(CircleCollider2D collA, SupplementaryColliderInfo info, ref List<JelloContact> contacts) //TODO make multi-path colliders work......
	{
		JelloContact contact;
		Vector2 pt;
		Vector2 hitPt;
		Vector2 norm;
		float scalarAB;
		float dist;
		float radius;
		
		//contact = new JelloContact();
		radius = collA.radius * Mathf.Max(collA.transform.localScale.x, collA.transform.localScale.y);
		
		pt = (Vector2)collA.transform.TransformPoint(collA.offset);
		
		//for each pointmass in body B
		for (int j = 0; j < info.ColliderVertices.Length - 2; j++)
		{	
			//get collision info
			dist = JelloVectorTools.getClosestPointOnSegmentSquared(pt, info.ColliderVertices[j + 1], info.ColliderVertices[j + 2], out hitPt, out norm, out scalarAB); //TODO make this work with fully penetrated circles.
			//fill out collisioninfo
			if(dist < radius * radius)
			{
				contact = new JelloContact();
				
				contact.bodyA = null;
				contact.colliderA = collA;
				contact.rigidbodyA = collA.GetComponent<Rigidbody2D>();
				contact.transformA = collA.transform;
				contact.bodyB = info.body;
				contact.colliderB = info.collider;
				contact.rigidbodyB = info.collider.GetComponent<Rigidbody2D>();
				contact.transformB = info.collider.transform;
				contact.bodyBpmA = j;
				contact.bodyBpmB = j + 1 > info.ColliderVertices.Length - 3 ? 0 : j + 1;
				contact.scalarAB = scalarAB;
				contact.hitPoint = hitPt;
				contact.normal = (hitPt - pt).normalized * (JelloShapeTools.Contains(info.ColliderVertices, pt) ? 1f : -1f);
				contact.mtv = contact.normal;
				contact.penetration = radius - Mathf.Sqrt(dist);
				contact.R = contact.hitPoint - (Vector2)collA.transform.position;
				contact.R2 = contact.hitPoint - (Vector2)info.collider.transform.position;
				
				contacts.Add(contact);
			}
		}
	}

	//public void BodyCollide(PolygonCollider2D collA, EdgeCollider2D collB, ref List<JelloContact> contacts){}
	
	//public void BodyCollide(EdgeCollider2D collA, PolygonCollider2D collB, ref List<JelloContact> contacts){}

	
	public void BodyCollide(SupplementaryColliderInfo infoA, SupplementaryColliderInfo infoB, ref List<JelloContact> contacts)
	{
		List<JelloContact> closestLineContacts = new List<JelloContact>();

		//find each point of the colliders that are penetrating each other.
		FindPenetratingPoints(infoA, infoB, ref contacts);

		//find each contact that satisfies the parametric contact method
		FindParametricContactDetails(infoA, infoB, ref contacts);

		//decide the detection method for each contact, parametric, closest line, or hybrid 
		FinalizeContactTypes(infoA, infoB, ref contacts, ref closestLineContacts);

		//get details (using the closest line method) for each contact that does not satisfy the parametric or hybrid detection methods.
		FindClosestLineContactDetails(infoA, infoB, ref contacts, ref closestLineContacts);
	}

	private void ClosestLineContactTest(Vector2 A, Vector2 B, Vector2 C, int edgeIndex, Vector2 edgeNormal, Vector2 pointNormal, float inverseSquaredEdgeLength, JelloContact contactSame, JelloContact contactAway)
	{
		float distToA = Vector2.SqrMagnitude(A - C);
		float distToB = Vector2.SqrMagnitude(B - C);
		
		// quick test of distance to each point on the edge, if both are greater than current mins, we can skip!
		if ((distToA > contactSame.penetration) && (distToA > contactAway.penetration) && (distToB > contactSame.penetration) && (distToB > contactAway.penetration) && 
		    (C.x < Mathf.Min (A.x, B.x) || C.x > Mathf.Max (A.x, B.x)) && (C.y < Mathf.Min (A.y, B.y) || C.y > Mathf.Max (A.y, B.y))) //and not within mini-aabb?
			return;
		
		Vector2 hitPt;
		float dist = Mathf.Infinity;
		Vector2 E = B - A;
		// calculate the distance!
		float scalarAB = Vector2.Dot(C - A, E) * inverseSquaredEdgeLength;
		if (scalarAB <= 0f)
		{
			// x is outside the line segment, distance is from pt to ptA.
			hitPt = A;
			scalarAB = 0f;
			dist = distToA;
		}
		else if (scalarAB >= 1f)
		{
			// x is outside of the line segment, distance is from pt to ptB.
			hitPt = B;
			scalarAB = 1f;
			dist = distToB;
		}
		else
		{
			// point lies somewhere on the line segment.
			hitPt = A + scalarAB * (E);
			dist = (hitPt - C).sqrMagnitude;
		}
		
		//TODO skip if dist to long?
		//if(dist >= contacts[a].penetration && dist >= contacts[a + 1].penetration)
		//	continue;
		
		if (Vector2.Dot (pointNormal, edgeNormal) <= 0f)
		{
			if (dist < contactSame.penetration)
			{
				//closest same contact
				contactSame.penetration = dist;
				contactSame.bodyBpmA = edgeIndex;
				contactSame.bodyBpmB = edgeIndex + 1;
				contactSame.scalarAB = scalarAB;
				contactSame.hitPoint = hitPt;
				contactSame.normal =  edgeNormal;
			}
		}
		else
		{
			if(dist < contactAway.penetration)
			{
				//closest away contact
				contactAway.penetration = dist; 
				contactAway.bodyBpmA = edgeIndex;
				contactAway.bodyBpmB = edgeIndex + 1;
				contactAway.scalarAB = scalarAB;
				contactAway.hitPoint = hitPt;
				contactAway.normal = edgeNormal;
			}
		}
	}

	private void FindClosestLineContactDetails(SupplementaryColliderInfo infoA, SupplementaryColliderInfo infoB, ref List<JelloContact> contacts, ref List<JelloContact> closestLineContacts)
	{
		Vector2 A, B, edgeNormal;

		for (int i = 0; i < Mathf.Max (infoA.ColliderVertices.Length - 2, infoB.ColliderVertices.Length - 2); i++)
		{	
			if(i < infoA.ColliderVertices.Length - 2)
			{
				A = infoA.ColliderVertices[i + 1];
				B = infoA.ColliderVertices[i + 2];
				
				//edge normal
				edgeNormal = infoA.EdgeNormals[i];


				for(int a = 0; a < closestLineContacts.Count; a+=2)
				{
					if(closestLineContacts[a].colliderA == infoA.collider)
						continue;

					ClosestLineContactTest
						(
							A,
							B, 
							infoB.ColliderVertices[closestLineContacts[a].bodyApm + 1], 
							i, 
							edgeNormal, 
							infoB.PointNormals[closestLineContacts[a].bodyApm], 
							infoA.InverseSquaredEdgeLengths[i], 
							closestLineContacts[a], 
							closestLineContacts[a + 1]
						);
				}
			}
			
			if(i < infoB.ColliderVertices.Length - 2)
			{
				A = infoB.ColliderVertices[i + 1];
				B = infoB.ColliderVertices[i + 2];

				// normal
				edgeNormal = infoB.EdgeNormals[i];
				
				for(int a = 0; a < closestLineContacts.Count; a+=2) 
				{
					if(closestLineContacts[a].colliderA == infoB.collider)
						continue;
					
					ClosestLineContactTest
						(
							A,
							B,
							infoA.ColliderVertices[closestLineContacts[a].bodyApm + 1],
							i,
							edgeNormal, 
							infoA.PointNormals[closestLineContacts[a].bodyApm], 
							infoB.InverseSquaredEdgeLengths[i], 
							closestLineContacts[a], 
							closestLineContacts[a + 1]
						);
				}
			}
		}

		//TODO place the following code into another method?
		for(int i = 0; i < closestLineContacts.Count; i++)
		{
			if (closestLineContacts[i].penetration > PenetrationThreshold && closestLineContacts[i + 1].penetration < closestLineContacts[i].penetration && closestLineContacts[i + 1].penetration != Mathf.Infinity) 
			{
				closestLineContacts[i + 1].penetration = Mathf.Sqrt(closestLineContacts[i + 1].penetration);
				
				float eLength;
				if(closestLineContacts[i + 1].colliderA == infoA.collider)
				{
					eLength = Vector2.Distance(infoB.ColliderVertices[closestLineContacts[i + 1].bodyBpmA + 1], infoB.ColliderVertices[closestLineContacts[i + 1].bodyBpmB + 1]);
					
					if(closestLineContacts[i + 1].bodyBpmB == infoB.ColliderVertices.Length - 2)
						closestLineContacts[i + 1].bodyBpmB = 0;
				}
				else
				{
					eLength = Vector2.Distance(infoA.ColliderVertices[closestLineContacts[i + 1].bodyBpmA + 1], infoA.ColliderVertices[closestLineContacts[i + 1].bodyBpmB + 1]);
					
					if(closestLineContacts[i + 1].bodyBpmB == infoA.ColliderVertices.Length - 2)
						closestLineContacts[i + 1].bodyBpmB = 0;
				}
				
				if(eLength != 0)
					eLength = 1 / eLength;
				closestLineContacts[i + 1].normal *= eLength;
				closestLineContacts[i + 1].mtv = closestLineContacts[i + 1].normal;
				
				closestLineContacts[i + 1].R = closestLineContacts[i + 1].hitPoint - (Vector2)closestLineContacts[i + 1].transformA.position;
				closestLineContacts[i + 1].R2 = closestLineContacts[i + 1].hitPoint - (Vector2)closestLineContacts[i + 1].transformB.position;

				closestLineContacts.Remove(closestLineContacts[i]);
			}
			else if(closestLineContacts[i].penetration != Mathf.Infinity)
			{
				closestLineContacts[i].penetration = Mathf.Sqrt(closestLineContacts[i].penetration);
				
				float eLength;
				if(closestLineContacts[i].colliderA == infoA.collider)
				{
					eLength = Vector2.Distance(infoB.ColliderVertices[closestLineContacts[i].bodyBpmA + 1], infoB.ColliderVertices[closestLineContacts[i].bodyBpmB + 1]);
					
					if(closestLineContacts[i].bodyBpmB >= infoB.ColliderVertices.Length - 2)
						closestLineContacts[i].bodyBpmB = 0;
				}
				else
				{
					eLength = Vector2.Distance(infoA.ColliderVertices[closestLineContacts[i].bodyBpmA + 1], infoA.ColliderVertices[closestLineContacts[i].bodyBpmB + 1]);
					
					if(closestLineContacts[i].bodyBpmB >= infoA.ColliderVertices.Length - 2)
						closestLineContacts[i].bodyBpmB = 0;
				}
				
				if(eLength != 0)
					eLength = 1 / eLength;
				closestLineContacts[i].normal *= eLength;
				closestLineContacts[i].mtv = closestLineContacts[i].normal;
				
				closestLineContacts[i].R = closestLineContacts[i].hitPoint - (Vector2)closestLineContacts[i].transformA.position;
				closestLineContacts[i].R2 = closestLineContacts[i].hitPoint - (Vector2)closestLineContacts[i].transformB.position;

				closestLineContacts.Remove(closestLineContacts[i + 1]);
			}
			else
			{
				closestLineContacts.Remove(closestLineContacts[i]);
				closestLineContacts.Remove(closestLineContacts[i]);
				i-=1;
			}
		}

		//now place the closet line contacs back into the original contacts list.
		for(int i = 0; i < closestLineContacts.Count; i++)
		{
			contacts.Add(closestLineContacts[i]);
		}
	}

	//private void HybridContactTest(Vector2 A, Vector2 B, Vector2 C, float inverseSquaredEdgeLength, JelloContact contact)
	private void HybridContactTest(Vector2 A, Vector2 B, Vector2 C, float inverseSquaredEdgeLength, JelloContact contact)
	{

		float edgeLength;

		edgeLength = Vector2.Distance(A, B);
		if(edgeLength != 0)
			contact.normal /= edgeLength;

		 
		//project the mtv onto contact tangent, if too high, use a hybrid method for finding hit point.
		//it is hybrid because it uses the edge found via the parametric test, but uses the closest point on that edge as the hitpoint.
		if(Mathf.Abs(JelloVectorTools.DotProduct (contact.mtv, contact.tangent)) > TangentPenetrationThreshold)//TODO account for winding...?
		{
			Vector2 E, hitPt;
			E = B - A;

			// calculate the hit point!
			float scalarAB = Vector2.Dot(C - A, E) * inverseSquaredEdgeLength;
			if (scalarAB <= 0f)
			{
				// x is outside the line segment, hitpoint is A.
				hitPt = A;
				scalarAB = 0f;
			}
			else if (scalarAB >= 1f)
			{
				// x is outside of the line segment, hitpoint is B.
				hitPt = B;
				scalarAB = 1f;
			}
			else
			{
				// point lies somewhere on the line segment.
				hitPt = A + scalarAB * E;
			}

			contact.hitPoint = hitPt;
			contact.mtv = hitPt - C;
		}

		//now that our contact details are final, get the penetration and normalize the mtv vector.
		contact.penetration = Vector2.Distance(contact.hitPoint, C);

		if(contact.penetration != 0f)
			contact.mtv /= contact.penetration;
	}

	private void FinalizeContactTypes(SupplementaryColliderInfo infoA, SupplementaryColliderInfo infoB, ref List<JelloContact> contacts, ref List<JelloContact> closestLineContacts)
	{
		//loop through each contact and fill out the rest of the details.
		for(int i = contacts.Count - 1; i >= 0; i--)
		{
			//if the time of impact is infinity then this contact point fialed the parametric contact tests and the closest line method will need to be used instead.
			if(contacts[i].toi == Mathf.Infinity)
			{
				//add two copies of this conact to the closest line contact list.

				//representative of closestSame
				closestLineContacts.Add (contacts[i]);
				closestLineContacts[closestLineContacts.Count - 1].penetration = Mathf.Infinity;

				//representative of closestAway
				closestLineContacts.Add(new JelloContact());
				closestLineContacts[closestLineContacts.Count - 1].penetration = Mathf.Infinity;
				closestLineContacts[closestLineContacts.Count - 1].bodyA = contacts[i].bodyA;
				closestLineContacts[closestLineContacts.Count - 1].bodyB = contacts[i].bodyB;
				closestLineContacts[closestLineContacts.Count - 1].bodyApm = contacts[i].bodyApm;
				closestLineContacts[closestLineContacts.Count - 1].colliderA = contacts[i].colliderA;
				closestLineContacts[closestLineContacts.Count - 1].colliderB = contacts[i].colliderB;
				closestLineContacts[closestLineContacts.Count - 1].rigidbodyA = contacts[i].rigidbodyA;
				closestLineContacts[closestLineContacts.Count - 1].rigidbodyB = contacts[i].rigidbodyB;
				closestLineContacts[closestLineContacts.Count - 1].transformA = contacts[i].transformA;
				closestLineContacts[closestLineContacts.Count - 1].transformB = contacts[i].transformB;
			
				//remove this contact from the contacts list.
				//one of the two above will be returned to this list later at the end of this.
				contacts.RemoveAt(i);
				continue;
			}

			if(contacts[i].colliderA == infoA.collider)
			{
				
				//assign the normal
				contacts[i].normal = infoB.EdgeNormals[contacts[i].bodyBpmA];

				//test hybrid
				HybridContactTest
					(
						infoB.ColliderVertices[contacts[i].bodyBpmA + 1],
						infoB.ColliderVertices[contacts[i].bodyBpmB + 1], 
						infoA.ColliderVertices[contacts[i].bodyApm + 1],
						infoB.InverseSquaredEdgeLengths[contacts[i].bodyBpmA], 
						contacts[i]
					);

				if(contacts[i].bodyBpmB >= infoB.ColliderVertices.Length - 2)
					contacts[i].bodyBpmB = 0;
			}
			else
			{
				
				//assign the normal
				contacts[i].normal = infoA.EdgeNormals[contacts[i].bodyBpmA];

				//test hybrid
				HybridContactTest
					(
						infoA.ColliderVertices[contacts[i].bodyBpmA + 1],
						infoA.ColliderVertices[contacts[i].bodyBpmB + 1], 
						infoB.ColliderVertices[contacts[i].bodyApm + 1],
						infoA.InverseSquaredEdgeLengths[contacts[i].bodyBpmA], 
						contacts[i]
					);
				
				if(contacts[i].bodyBpmB >= infoA.ColliderVertices.Length - 2)
					contacts[i].bodyBpmB = 0;
			}

			contacts[i].R = contacts[i].hitPoint - (Vector2)contacts[i].transformA.position;
			contacts[i].R2 = contacts[i].hitPoint - (Vector2)contacts[i].transformB.position;
		}
	}

	private void ParametricContactTest(Vector2 A, Vector2 B, Vector2 C, Vector2 prevA, Vector2 prevB, Vector2 prevC, Vector2 vAmod, Vector2 vBmod, int edgeIndex, JelloContact contact)
	{
		float toi, t1, t2;
		double quadA, quadB, quadC;
		
//		//this is the formulat for the parametric position of the hit, we will not use this because we want the earliest contact and can calculate q more easily off the results of the next method.
//		
//		//calculate parametric position                                                                                                           
//		//quadratic equation is Ax^2 + Bx + C = 0
//		//parametric position of impact formula is Cross(C - (A + q*(B - A), vA + q*(vB + vA)) = 0
//		//quadratic A = (A.x - B.x)*(vA.y + vB.y) + (B.y - A.y)*(vA.x + vB.x)
//		//quadratic B = vA.y*(C.x - B.x) + vB.y*(C.x - A.x) + vA.x*(B.y - C.y) + vB.x*(A.y - C.y)
//		//quadratic C = vA.y*(C.x - A.x) + vA.x*(A.y - C.y)
//		
//		q = Mathf.Infinity;
//		q1 = q2 = float.NaN;
//		
//		quadA = (prevA.x - prevB.x)*(vAmod.y + vBmod.y) + (prevB.y - prevA.y)*(vAmod.x + vBmod.x);
//		quadB = vAmod.y*(prevC.x - prevB.x) + vBmod.y*(prevC.x - prevA.x) + vAmod.x*(prevB.y - prevC.y) + vBmod.x*(prevA.y - prevC.y);
//		quadC = vAmod.y*(prevC.x - prevA.x) + vAmod.x*(prevA.y - prevC.y);
//		
//		JelloMathTools.SolveQuadratic (quadA, quadB, quadC, out q1, out q2);
//		
//		if(!float.IsNaN(q1) && q1 >= 0 && q1 <= 1)
//			q = q1;
//		
//		if(!float.IsNaN(q2) && q2 >= 0 && q2 <= 1 && q2 < q)
//			q = q2;
//		
//		if(q == Mathf.Infinity)
//			return;
		
		//now we need to make sure that the contact happens within this timestep (Time.FixedDeltaTime)
		//so calculate the parametric time of intersection and if within 0 to 1, then we have a valid intersection and this qualifies as a possible edge
		//for contact with this point
		
		//quadratic equation is Ax^2 + Bx + C = 0
		//parametric time of impact formula is Cross(C - (A + t*vA), (B + t*vB) - (A + t*vA)) = 0
		//quadratic A = vA.y*vB.x - vA.x*vB.y
		//quadratic B = vA.x*(C.y - B.y) + vA.y*(B.x - C.x) + vB.y*(C.x - A.x) + vB.x*(A.y - C.y)
		//quadratic C = A.x*(C.y - B.y) + A.y*(B.x - C.x) + B.y*C.x - B.x*C.y
		
		quadA = vAmod.y * vBmod.x - vAmod.x * vBmod.y;
		quadB = vAmod.x * (prevC.y - prevB.y) + vAmod.y * (prevB.x - prevC.x) + vBmod.y * (prevC.x - prevA.x) + vBmod.x * (prevA.y - prevC.y);
		quadC = prevA.x * (prevC.y - prevB.y) + prevA.y * (prevB.x - prevC.x) + prevB.y * prevC.x - prevB.x * prevC.y;
		
		toi = Mathf.Infinity;
		t1 = t2 = float.NaN;
		
		JelloMathTools.SolveQuadratic (quadA, quadB, quadC, out t1, out t2);
		
		if(!float.IsNaN(t1) && t1 >= 0 && t1 <= 1)
			toi = t1;
		
		if(!float.IsNaN(t2) && t2 >= 0 && t2 <= 1 && t2 < toi)
			toi = t2;
		
		if(toi == Mathf.Infinity)
			return;

		//now we know that there is an intersection within this time step, lets make sure that the intersection happens between points A and B.
		//A value between 0 and 1 passes and also gives us our parametric position of impact.
		//get q by projecting C onto the line at the time of impact.
		//line passing through (A + t*vA) with direction ((B + t*vB) - (A + t*vA))
		Vector2 AatTOI = prevA + toi * vAmod; 														
		Vector2 BatTOI = prevB + toi * vBmod;
		float q = Vector2.Dot(prevC - AatTOI, BatTOI - AatTOI);
		if(BatTOI - AatTOI != Vector2.zero)
			q /= (BatTOI - AatTOI).sqrMagnitude;

		if(q < 0f || q > 1f)
			return;

		//we've passed both the parametric position and parametric time checks, now lets use only the earliest time of impact for the contact point.
		if (toi < contact.toi)
		{
			//we have the earliest valid intersection so far, lets fill out the related details.
			contact.toi = toi;
			contact.bodyBpmA = edgeIndex;
			contact.bodyBpmB = edgeIndex + 1;
			contact.scalarAB = q;

			//here we get the point on line AB and it is not quite as accurate, but it will at least account for the  
			contact.hitPoint = A * (1 - q) + B * q;


			//the following gets the more accurate hit point, but doesnt account the mtv of points A and B.
			//having separate A and B mtvs sounds ok at first, but then considering the extra normalization and misfit with closest line method, it seems a bit difficult/not plausable to implement.
//			contact.hitPoint = AatTOI * (1 - q) + BatTOI * q;
//			contact.hitPoint += (C - prevC) * toi;


			contact.mtv = contact.hitPoint - C;
		}
	}

	private void FindParametricContactDetails(SupplementaryColliderInfo infoA, SupplementaryColliderInfo infoB, ref List<JelloContact> contacts)
	{
		Vector2 A, B, prevA, prevB, vA, vB;

		//after looping through each point and checking for potential collisions, loop through one more time and get fine detail collision.
		for (int i = 0; i < Mathf.Max (infoA.ColliderVertices.Length - 2, infoB.ColliderVertices.Length - 2); i++)
		{	
			//handle the first collider
			if(i < infoA.ColliderVertices.Length - 2)
			{
				//get the information for the infoa edge
				A = infoA.ColliderVertices[i + 1];
				B = infoA.ColliderVertices[i + 2];
				prevA = infoA.PrevColliderVertices[i + 1];
				prevB = infoA.PrevColliderVertices[i + 2];
				vA = A - prevA;
				vB = B - prevB;
				
				//test collision for each infoB contact against the infoa edge
				for(int a = 0; a < contacts.Count; a++) 
				{
					if(contacts[a].colliderA == infoA.collider) //this is where it could be beneficial to have separate lists, its not a big check, but it would also save on an iteration...
						continue;

					ParametricContactTest
						(
							A,
							B, 
							infoB.ColliderVertices[contacts[a].bodyApm + 1], 
							prevA, 
							prevB, 
							infoB.PrevColliderVertices[contacts[a].bodyApm + 1],
							vA - (infoB.ColliderVertices[contacts[a].bodyApm + 1] - infoB.PrevColliderVertices[contacts[a].bodyApm + 1]),
							vB - (infoB.ColliderVertices[contacts[a].bodyApm + 1] - infoB.PrevColliderVertices[contacts[a].bodyApm + 1]),
							i, 
							contacts[a]
						);
				}
			}
			
			//handle the second collider
			if(i < infoB.ColliderVertices.Length - 2)
			{
				A = infoB.ColliderVertices[i + 1];
				B = infoB.ColliderVertices[i + 2];
				prevA = infoB.PrevColliderVertices[i + 1];
				prevB = infoB.PrevColliderVertices[i + 2];
				vA = A - prevA;
				vB = B - prevB;

				for(int a = 0; a < contacts.Count; a++) 
				{
					if(contacts[a].colliderA == infoB.collider)
						continue;

					ParametricContactTest
						(
							A,
							B, 
							infoA.ColliderVertices[contacts[a].bodyApm + 1], 
							prevA, 
							prevB, 
							infoA.PrevColliderVertices[contacts[a].bodyApm + 1],
							vA - (infoA.ColliderVertices[contacts[a].bodyApm + 1] - infoA.PrevColliderVertices[contacts[a].bodyApm + 1]),
							vB - (infoA.ColliderVertices[contacts[a].bodyApm + 1] - infoA.PrevColliderVertices[contacts[a].bodyApm + 1]),
							i, 
							contacts[a]
						);
				}
			}
		}
	}

	private void FindPenetratingPoints(SupplementaryColliderInfo infoA, SupplementaryColliderInfo infoB, ref List<JelloContact> contacts)
	{
		//TODO i have an idea of how to optimise this a bit maybe.
		//have separate lists for each body and iterate better over each of them...


		//loop through both bodies at the same time, identifying penetrating points
		for (int i = 0; i < Mathf.Max ( infoA.ColliderVertices.Length - 2, infoB.ColliderVertices.Length - 2); i++)
		{
			//check for potential collisions in both bodies
			if(i < infoA.ColliderVertices.Length - 2)
			{
				//only consider this point if it is insdie the other body's AABB and inside the other body.
				if(infoB.AABB.contains(infoA.ColliderVertices[i + 1]) && JelloShapeTools.Contains(infoB.ColliderVertices, infoA.ColliderVertices[i + 1]))
				{
					contacts.Add (new JelloContact());
					contacts[contacts.Count - 1].penetration = Mathf.Infinity;
					contacts[contacts.Count - 1].toi = Mathf.Infinity;
					contacts[contacts.Count - 1].bodyApm = i;
					contacts[contacts.Count - 1].bodyA = infoA.body;
					contacts[contacts.Count - 1].colliderA = infoA.collider; 
					contacts[contacts.Count - 1].rigidbodyA = infoA.rigidbody;
					contacts[contacts.Count - 1].transformA = infoA.transform;
					contacts[contacts.Count - 1].bodyB = infoB.body;
					contacts[contacts.Count - 1].colliderB = infoB.collider;
					contacts[contacts.Count - 1].rigidbodyB = infoB.rigidbody;
					contacts[contacts.Count - 1].transformB = infoB.transform;
				}
			}
			if(i < infoB.ColliderVertices.Length - 2)
			{
				//only consider this point if it is insdie the other body's AABB and inside the other body.
				if(infoA.AABB.contains(infoB.ColliderVertices[i + 1]) && JelloShapeTools.Contains(infoA.ColliderVertices, infoB.ColliderVertices[i + 1]))
				{
					contacts.Add (new JelloContact());
					contacts[contacts.Count - 1].penetration = Mathf.Infinity;
					contacts[contacts.Count - 1].toi = Mathf.Infinity;
					contacts[contacts.Count - 1].bodyApm = i;
					contacts[contacts.Count - 1].bodyA = infoB.body;
					contacts[contacts.Count - 1].colliderA = infoB.collider;
					contacts[contacts.Count - 1].rigidbodyA = infoB.rigidbody;
					contacts[contacts.Count - 1].transformA = infoB.transform;
					contacts[contacts.Count - 1].bodyB = infoA.body;
					contacts[contacts.Count - 1].colliderB = infoA.collider;
					contacts[contacts.Count - 1].rigidbodyB = infoA.rigidbody;
					contacts[contacts.Count - 1].transformB = infoA.transform;
				}
			}
		}
	}


	/// <summary>
	/// Applies the collision impulse to resolve the contact generated by the collision detection system.
	/// </summary>
	/// <param name="contact">Contact representing a single point of a collision between two colliders</param>
	public void ApplyCollisionImpulse(JelloContact contact) //TODO comment the contents of this method
	{
		if (contact.penetration > PenetrationThreshold)
			return;

		//first get required info from each body
		Vector2 vap = Vector2.zero;
		Vector2 vbp = Vector2.zero;
		float aMass = Mathf.Infinity;
		float b2MassSum = Mathf.Infinity;
		float invma = 0f;
		float invmb = 0f;
	
		if(contact.bodyA != null)
		{
			vap = contact.bodyA.getEdgePointMass(contact.bodyApm).velocity;
			invma = contact.bodyA.getEdgePointMass(contact.bodyApm).InverseMass;
			aMass = contact.bodyA.getEdgePointMass(contact.bodyApm).Mass;

			if(!contact.bodyA.IsAwake)
			{
				aMass = Mathf.Infinity;
				invma = 1f;
			}
		}
		else if (contact.rigidbodyA != null)
		{
			vap = contact.rigidbodyA.velocity + contact.rigidbodyA.angularVelocity * Mathf.Deg2Rad * new Vector2(-contact.R.y, contact.R.x);

			aMass = contact.rigidbodyA.mass;
			
			if(contact.rigidbodyA.mass != 0f)
				invma = 1f / contact.rigidbodyA.mass;

			if(!contact.rigidbodyA.IsAwake())
			{
				aMass = Mathf.Infinity;
				invma = 1f;
			}
		}
		
		if(contact.bodyB != null)
		{
			vbp = contact.bodyB.getPointOnEdgeVelocity(contact.bodyBpmA, contact.scalarAB);
			invmb = contact.bodyB.getEdgePointMass(contact.bodyBpmA).InverseMass + contact.bodyB.getEdgePointMass(contact.bodyBpmB).InverseMass;
			b2MassSum =  (contact.bodyB.getEdgePointMass(contact.bodyBpmA).Mass + contact.bodyB.getEdgePointMass(contact.bodyBpmB).Mass);

			if(!contact.bodyB.IsAwake)
			{
				b2MassSum = Mathf.Infinity;
				invmb = 1f;
			}
		}
		else if (contact.rigidbodyB != null)
		{
			vbp = contact.rigidbodyB.velocity + contact.rigidbodyB.angularVelocity * Mathf.Deg2Rad * new Vector2(-contact.R2.y, contact.R2.x);
			
			b2MassSum = contact.rigidbodyB.mass;
			if(contact.rigidbodyB.mass != 0f)
				invmb = 1f / contact.rigidbodyB.mass;

			if(!contact.rigidbodyB.IsAwake())
			{
				b2MassSum = Mathf.Infinity;
				invmb = 1f;
			}
		}


		//handle the minimum translation vector.
		Vector2 mtv;
		SupplementaryColliderInfo infoA = TrackedColliderDictionary[contact.colliderA.GetInstanceID()];
		SupplementaryColliderInfo infoB = TrackedColliderDictionary[contact.colliderB.GetInstanceID()];
		if (aMass == Mathf.Infinity)
		{
			if(contact.bodyB != null)
			{
				if(contact.bodyB.getEdgePointMass(contact.bodyBpmA).Mass != Mathf.Infinity)
				{
					mtv = -contact.mtv * (contact.penetration * (1.0f - contact.scalarAB) + mtvModifierGlobal + contact.bodyB.mtvModifier);
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].x) < Mathf.Abs(mtv.x))
						infoB.largestMTVs[contact.bodyBpmA].x = mtv.x;
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].y) < Mathf.Abs(mtv.y))
						infoB.largestMTVs[contact.bodyBpmA].y = mtv.y;
				}
				if(contact.bodyB.getEdgePointMass(contact.bodyBpmB).Mass != Mathf.Infinity)
				{
					mtv = -contact.mtv * (contact.penetration  * contact.scalarAB + mtvModifierGlobal + contact.bodyB.mtvModifier);
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmB].x) < Mathf.Abs(mtv.x))
						infoB.largestMTVs[contact.bodyBpmB].x = mtv.x;
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmB].y) < Mathf.Abs(mtv.y))
						infoB.largestMTVs[contact.bodyBpmB].y = mtv.y;
				}
			}
			else if(contact.rigidbodyB != null)
			{
//				contact.colliderB.transform.position -= (Vector3)(contact.normal * (contact.penetration + 0.001f)); //TODO find a better way to handle this. this only happens with static vs rigidbody
				mtv = -contact.mtv * (contact.penetration + mtvModifierGlobal);
				if(Mathf.Abs(infoB.largestMTVs[0].x) < Mathf.Abs(mtv.x))
					infoB.largestMTVs[0].x = mtv.x;
				if(Mathf.Abs(infoB.largestMTVs[0].y) < Mathf.Abs(mtv.y))
					infoB.largestMTVs[0].y = mtv.y;
			}
		}
		else if (b2MassSum == Mathf.Infinity)
		{
			
			if(contact.bodyA != null)
			{
				if(contact.bodyA.getEdgePointMass(contact.bodyApm).Mass != Mathf.Infinity)
				{
					mtv = contact.mtv * (contact.penetration + mtvModifierGlobal + contact.bodyA.mtvModifier);
					if(Mathf.Abs(infoA.largestMTVs[contact.bodyApm].x) < Mathf.Abs(mtv.x))
						infoA.largestMTVs[contact.bodyApm].x = mtv.x;
					if(Mathf.Abs(infoA.largestMTVs[contact.bodyApm].y) < Mathf.Abs(mtv.y))
						infoA.largestMTVs[contact.bodyApm].y = mtv.y;
				}
			}
			else if (contact.rigidbodyA != null)//TODO should also check rigidbody masses for infiniy...
			{
				mtv = contact.mtv * (contact.penetration + mtvModifierGlobal);
				if(Mathf.Abs( infoA.largestMTVs[0].x ) < Mathf.Abs(mtv.x))
					infoA.largestMTVs[0].x = mtv.x;
				if(Mathf.Abs( infoA.largestMTVs[0].y ) < Mathf.Abs(mtv.y))
					infoA.largestMTVs[0].y = mtv.y;
			}
		}
		else
		{	
			if(contact.bodyA != null)
			{
				if(contact.bodyA.getEdgePointMass(contact.bodyApm).Mass != Mathf.Infinity)
				{
					mtv = contact.mtv * (contact.penetration * (contact.bodyB != null ? b2MassSum / (aMass + b2MassSum) : 1f) + mtvModifierGlobal + contact.bodyA.mtvModifier);
					if(Mathf.Abs(infoA.largestMTVs[contact.bodyApm].x) < Mathf.Abs(mtv.x))
						infoA.largestMTVs[contact.bodyApm].x = mtv.x;
					if(Mathf.Abs(infoA.largestMTVs[contact.bodyApm].y) < Mathf.Abs(mtv.y))
						infoA.largestMTVs[contact.bodyApm].y = mtv.y;
				}
			}
			//else if(contact.rigidbodyA != null)
			//{
			//contact.colliderA.transform.position += (Vector3)(contact.normal * Amove);
			//}
			
			if(contact.bodyB != null)
			{
				float Bmove = contact.penetration * (contact.bodyA != null ? aMass / (aMass + b2MassSum) : 1f) ;
				if(contact.bodyB.getEdgePointMass(contact.bodyBpmA).Mass != Mathf.Infinity)
				{
					mtv = -contact.mtv * (Bmove * (1.0f - contact.scalarAB) + mtvModifierGlobal + contact.bodyB.mtvModifier);
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].x) < Mathf.Abs(mtv.x))
						infoB.largestMTVs[contact.bodyBpmA].x = mtv.x;
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].y) < Mathf.Abs(mtv.y))
						infoB.largestMTVs[contact.bodyBpmA].y = mtv.y;
				}
				if(contact.bodyB.getEdgePointMass(contact.bodyBpmB).Mass != Mathf.Infinity)
				{
					mtv = -contact.mtv * (Bmove * contact.scalarAB + mtvModifierGlobal + contact.bodyB.mtvModifier);
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].x) < Mathf.Abs(mtv.x))
						infoB.largestMTVs[contact.bodyBpmA].x = mtv.x;
					if(Mathf.Abs(infoB.largestMTVs[contact.bodyBpmA].y) < Mathf.Abs(mtv.y))
						infoB.largestMTVs[contact.bodyBpmA].y = mtv.y;
				}
			}
			//else if(contact.rigidbodyB != null)
			//{
			//contact.colliderB.transform.position -= (Vector3)(contact.normal * Bmove);
			//}
		}
		
		
		//collision impulse follows.
		
		Vector2 vab = vap - vbp;
		
		if(Vector2.Dot(vab, contact.normal) >= 0f) //no collision impulse if colliding points aren't moving towards eachother.
			return;
		
		float bounciness = 0f;
		float friction = 0f;
		
		if(contact.colliderA.sharedMaterial != null)
		{
			bounciness += contact.colliderA.sharedMaterial.bounciness;
			friction = contact.colliderA.sharedMaterial.friction;
		}
		else
		{
			bounciness += defaultPhysicsMaterial.bounciness;
			friction = defaultPhysicsMaterial.friction;
		}
		
		if(contact.colliderB.sharedMaterial != null)
		{
			bounciness += contact.colliderB.sharedMaterial.bounciness;
			if(contact.colliderB.sharedMaterial.friction < friction)
				friction = contact.colliderB.sharedMaterial.friction;
		}
		else
		{
			bounciness += defaultPhysicsMaterial.bounciness;
			if(defaultPhysicsMaterial.friction < friction)
				friction = defaultPhysicsMaterial.friction;
		}

		bounciness = Mathf.Max (0f, bounciness);
		
		//just using 1 for moment of  inertia unless infinite mass... for now. //TODO get correct moment of inertia?
		
		float invMomentInertiaA;
		float invMomentInertiaB;
		
		
		#if (UNITY_4_3_2 || UNITY_4_3_3 || UNITY_4_3_4)
		invMomentInertiaA = aMass == Mathf.Infinity ? 0f : 1f;
		invMomentInertiaB = b2MassSum == Mathf.Infinity ? 0f : 1f; 
		#else
		if(contact.bodyA == null && contact.rigidbodyA != null && contact.rigidbodyA.inertia != 0f)
			invMomentInertiaA = 1 / contact.rigidbodyA.inertia;
		else
			invMomentInertiaA = aMass == Mathf.Infinity ? 0f : 1f;
		if(contact.bodyB == null && contact.rigidbodyB != null && contact.rigidbodyB.inertia != 0f)
			invMomentInertiaB = 1 / contact.rigidbodyB.inertia;
		else
			invMomentInertiaB = b2MassSum == Mathf.Infinity ? 0f : 1f;
		#endif
		
		float denomA = 0f;
		float denomB = 0f;
		if(contact.bodyA == null)
		{
			denomA = JelloVectorTools.CrossProduct(contact.R, contact.normal);
			denomA *= denomA * invMomentInertiaA;
		}
		if(contact.bodyB == null)
		{
			denomB = JelloVectorTools.CrossProduct(contact.R2, contact.normal);
			denomB *= denomB * invMomentInertiaB;
		}
		float j = -Vector2.Dot (vab, contact.normal) * ( 1 + bounciness);
		j /= invma + invmb + denomA + denomB;
		
		float fNumerator = Vector2.Dot(vab, contact.tangent);
		float f = fNumerator / (invma + invmb + denomA + denomB);
		f = Mathf.Clamp(f, -friction * Mathf.Abs(j), friction * Mathf.Abs(j));

		if(contact.bodyA != null)
		{	
			infoA.queuedVelocities[contact.bodyApm] += (j * contact.normal - f * contact.tangent) * invma;
			infoA.numEngagments[contact.bodyApm]++;
		}
		else if (contact.rigidbodyA != null && !contact.rigidbodyA.isKinematic)
		{
			//do this now, but consider averaging after averaging common edges and points etc...
			contact.rigidbodyA.velocity += (j * contact.normal - f * contact.tangent) * invma / contact.numContacts;
			contact.rigidbodyA.angularVelocity += (JelloVectorTools.CrossProduct(contact.R, j * contact.normal) - JelloVectorTools.CrossProduct(contact.R, f * contact.tangent)) * Mathf.Rad2Deg / contact.numContacts;
			
			
//			infoA.queuedVelocities[contact.bodyApm] += (j * contact.normal - f * contact.tangent) * invma;
//			infoA.queuedAngularVelocities[contact.bodyApm] += (JelloVectorTools.CrossProduct(contact.R, j * contact.normal) - JelloVectorTools.CrossProduct(contact.R, f * contact.tangent)) * Mathf.Rad2Deg;
//			infoA.numEngagments[contact.bodyApm]++;
		}

		if(contact.bodyB != null)
		{
			infoB.queuedVelocities[contact.bodyBpmA] -= (j * contact.normal - f * contact.tangent) * invmb * (1f - contact.scalarAB);
			infoB.queuedVelocities[contact.bodyBpmB] -= (j * contact.normal - f * contact.tangent) * invmb * contact.scalarAB;
			infoB.numEngagments[contact.bodyBpmA]++;
			infoB.numEngagments[contact.bodyBpmB]++;
		}
		else if (contact.rigidbodyB != null && !contact.rigidbodyB.isKinematic)
		{
			contact.rigidbodyB.velocity -= (j * contact.normal - f * contact.tangent) * invmb / contact.numContacts;
			contact.rigidbodyB.angularVelocity -= (JelloVectorTools.CrossProduct(contact.R2, j * contact.normal) - JelloVectorTools.CrossProduct(contact.R2, f * contact.tangent)) * Mathf.Rad2Deg / contact.numContacts;
			
//			infoB.queuedVelocities[contact.bodyBpmA] -= (j * contact.normal - f * contact.tangent) * invmb * (1f - contact.scalarAB);
//			infoB.queuedVelocities[contact.bodyBpmB] -= (j * contact.normal - f * contact.tangent) * invmb * contact.scalarAB;
//			infoB.queuedAngularVelocities[contact.bodyBpmA] -= (JelloVectorTools.CrossProduct(contact.R2, j * contact.normal) - JelloVectorTools.CrossProduct(contact.R2, f * contact.tangent)) * Mathf.Rad2Deg * (1f - contact.scalarAB);
//			infoB.queuedAngularVelocities[contact.bodyBpmB] -= (JelloVectorTools.CrossProduct(contact.R2, j * contact.normal) - JelloVectorTools.CrossProduct(contact.R2, f * contact.tangent)) * Mathf.Rad2Deg * contact.scalarAB;
//			infoB.numEngagments[contact.bodyBpmA]++;
//			infoB.numEngagments[contact.bodyBpmB]++;
		}
	}

	#endregion
	
	#region DEBUG VISUALIZATION
	
	/// <summary>
	/// Draw all of the JelloBody objects simlated by the JelloWorld in debug mode, for quick visualization of the entire scene.
	/// </summary>
	public void debugDrawAllBodies()
	{
		for (int i = 0; i < mBodies.Count; i++)
			mBodies[i].debugDrawMe();
	} 
	
	#endregion
}

/// <summary>
/// Class containing collision contact information.
/// Represents a collision contact between a single point on one Collider2D and a single edge on another Collider2D.
/// </summary>
public class JelloContact	
{
	/// <summary>
	/// The first JelloBody, if any,  involved in the JelloContact.
	/// </summary>
	public JelloBody bodyA;

	/// <summary>
	/// The second JelloBody, if any, involved in the JelloContact.
	/// </summary>
	public JelloBody bodyB;

	/// <summary>
	/// The first Collider2D involved in the JelloContact.
	/// </summary>
	public Collider2D colliderA;

	/// <summary>
	/// The second Collider2D involved in the JelloContact.
	/// </summary>
	public Collider2D colliderB;

	/// <summary>
	/// The first Rigidbody2D, if any, involved in the JelloContact.
	/// </summary>
	public Rigidbody2D rigidbodyA;

	/// <summary>
	/// The second Rigidbody2D, if any, involved in the JelloContact.
	/// </summary>
	public Rigidbody2D rigidbodyB;

	/// <summary>
	/// The first Transform involved in the JelloContact.
	/// </summary>
	public Transform transformA;

	/// <summary>
	/// The second Transform involved in the JelloContact.
	/// </summary>
	public Transform transformB;

	/// <summary>
	/// The index of the vertex of JelloContact.colliderA that is penetrating into JelloContact.colliderB.
	/// </summary>
	public int bodyApm = -1;

	/// <summary>
	/// This with JelloContact.bodyBpmB represents the edge on JelloContact.colliderB that is being penetrated by JelloContact.colliderA.
	/// </summary>
	public int bodyBpmA = -1;

	/// <summary>
	/// This with JelloContact.bodyBpmA represents the edge on the JelloContact.colliderB that is being penetrated by Jellobontact.colliderA.
	/// </summary>
	public int bodyBpmB = -1;

	/// <summary>
	/// The point in global space where the JelloContact occurs.
	/// The point of impact.
	/// </summary>
	public Vector2 hitPoint = Vector2.zero;

	/// <summary>
	/// The distance vector from the center of JelloContact.colliderA to JelloContact.hitPoint.
	/// </summary>
	public Vector2 R = Vector2.zero; 

	/// <summary>
	/// The distance vector from the center of JelloContact.colliderB to JelloContact.hitPoint.
	/// </summary>
	public Vector2 R2 = Vector2.zero;

	/// <summary>
	/// How far along JelloContact.colliderB penetrated edge the JelloContact.hitPoint is.
	/// Normalized [0,1] with 0 being completely at JelloContact.bodyBpmA position and 1 being at JelloContact.bodyBpmB position.
	/// </summary>
	public float scalarAB = 0f;

	/// <summary>
	/// How far the JellContact.colliderA penetrating vertex penetrates into the penetrated edge of JelloContact.colliderB.
	/// </summary>
	public float penetration = 0f;

	/// <summary>
	/// The number JelloContact objects shared between JelloContact.colliderA and JelloContact.colliderB.
	/// </summary>
	public float numContacts = 0f;

	/// <summary>
	/// Whether or not to ignore the resolution of this JelloContact.
	/// </summary>
	public bool ignore = false;

	/// <summary>
	/// The parametric time of impact. 0 is the start of the FixedUpdate(), 1 is the end of the FixedUpdate().
	/// If the toi is less than 0 or greater than 1, the impact did not occur during this FixedUpdate().
	/// </summary>
	public float toi = 0f;

	/// <summary>
	/// The minimum translation vector. This represents the distance from penetrating point to the hitpoint.
	/// </summary>
	public Vector2 mtv = Vector2.zero;

	/// <summary>
	/// The normal vector of the JelloContact.
	/// The normalized vector perpendicular to the penetrated edge of JelloContact.colliderB.
	/// </summary>
	private Vector2 mNormal = Vector2.zero;

	/// <summary>
	/// The tangent vector of the JelloContact.
	/// </summary>
	private Vector2 mTanget = Vector2.zero;


	/// <summary>
	/// Constructor.
	/// </summary>
	public JelloContact(){}

	/// <summary>
	/// Clear the JelloContact.
	/// </summary>
	public void Clear() 
	{ 
		bodyA = bodyB = null; 
		bodyApm = bodyBpmA = bodyBpmB = -1; 
		hitPoint = R2 = R = mNormal = mTanget = Vector2.zero; 
		scalarAB = penetration = 0f; 
		colliderA = colliderB = null;
		rigidbodyA = rigidbodyB = null;
		transformA = transformB = null;
	}

	/// <summary>
	/// Get the other JelloBody, given a JelloBody.
	/// </summary>
	/// <param name="body">The given JelloBody.</param>
	/// <returns>The other JelloBody. Null if given JelloBody does not belong to this JelloContact.</returns>
	public JelloBody GetOtherBody(JelloBody body)
	{
		if(body == bodyA)
			return bodyB;
		else if(body == bodyB)
			return bodyA;
		else 
			return null;
	}

	/// <summary>
	/// Get the other Collider2D, given a Collider2D.
	/// </summary>
	/// <param name="collider">The given Collider2D.</param>
	/// <returns>The other Collider2D. Null if the given Collider2D does not belong to this JelloContact.</returns>
	public Collider2D GetOtherCollider(Collider2D collider)
	{
		if(collider == colliderA)
			return colliderB;
		else if(collider == colliderB)
			return colliderA;
		else 
			return null;
	}

	/// <summary>
	/// Get the other Transform, given a Transform.
	/// </summary>
	/// <param name="transform">The given Transform.</param>
	/// <returns>The other Transform. Null if the given Transform does not belong to this JelloContact.</returns>
	public Transform GetOtherTransform(Transform transform)
	{
		if(transform == transformA)
			return transformB;
		else if(transform == transformB)
			return transformA;
		else 
			return null;
	}

	/// <summary>
	/// Compare another JelloContact to this one to see if they are between the same two Collider2D objects and have the same vaule for JelloContact.bodyApm.
	/// To be used for checking for persistence of a JelloContact.
	/// </summary>
	/// <param name="conact">The JelloConact to test against.</param>
	public bool Compare(JelloContact conact)
	{
		if(this.colliderA != conact.colliderA || this.bodyApm != conact.bodyApm || this.colliderB != conact.colliderB)
			return false;
		else
			return true;
	}

	/// <summary>
	/// Get or set the normal vector.
	/// The normal vector of the JelloContact.
	/// The normalized vector perpendicular to the penetrated edge of JelloContact.colliderB.
	/// JelloContact.tangent will also be set.
	/// </summary>
	/// <value>The normal.</value>
	public Vector2 normal
	{
		get{ return mNormal; }
		set
		{
			mNormal = value;
			mTanget = JelloVectorTools.getPerpendicular(mNormal);
		}
	}

	/// <summary>
	/// Get the tangent vector.
	/// </summary>
	/// <value>The tangent.</value>
	public Vector2 tangent
	{
		get{ return mTanget; }
	}

}

/// <summary>
/// Collision manifold.
/// Represents a collision between two Collider2D objects
/// Is just a set of JelloContact objects between two Collider2D objects.
/// </summary>
public class JelloCollision
{
	/// <summary>
	/// The JelloContact objects that make up this JelloCollision.
	/// </summary>
	public JelloContact[] contacts;

	/// <summary>
	/// Whether to ignore the resolution of this JelloCollision.
	/// </summary>
	public bool ignore = false;

	/// <summary>
	/// Constructor.
	/// </summary>
	public JelloCollision()
	{
		contacts = new JelloContact[0];
	}

	/// <summary>
	/// Constructor. 
	/// Sizes the JelloCollision.contacts array to fit the needed number of JelloContact objects.
	/// </summary>
	/// <param name="numContacts">The needed number JelloContact objects.</param>
	public JelloCollision(int numContacts)
	{
		contacts = new JelloContact[numContacts];
	}

	/// <summary>
	/// Constructor assigning a predefined JelloContact array to JelloCollision.contacts.
	/// </summary>
	/// <param name="contactArray">The JelloContact array.</param>
	public JelloCollision(JelloContact[] contactArray)
	{
		contacts = contactArray;
	}

	/// <summary>
	/// Clear this JelloCollision.
	/// </summary>
	public void Clear()
	{
		contacts = null;
	}

	/// <summary>
	/// Get the other JelloBody, given a JelloBody.
	/// Samples the first entry in JelloCollision.contacts.
	/// </summary>
	/// <param name="body">The given JelloBody.</param>
	/// <returns>The other JelloBody. Null if the given JelloBody does not belong to this JelloCollision or if JelloCollision.contacts is null.</returns>
	public JelloBody GetOtherBody(JelloBody body)
	{
		if(contacts == null)
			return null;

		if(body == contacts[0].bodyA)
			return contacts[0].bodyB;
		else if(body == contacts[0].bodyB)
			return contacts[0].bodyA;
		else
			return null;
	}

	/// <summary>
	/// Get the other Collider2D, given a Collider2D.
	/// Samples the first entry in JelloCollision.contacts.
	/// </summary>
	/// <param name="collider">The given Collider2D.</param>
	/// <returns>The other Collider2D. Null if the given Collider2D does not belong to this JelloCollision or if JelloCollision.contacts is null.</returns>
	public Collider2D GetOtherCollider(Collider2D collider)
	{
		if(contacts == null)
			return null;

		if(collider == contacts[0].colliderA)
			return contacts[0].colliderB;
		else if(collider == contacts[0].colliderB)
			return contacts[0].colliderA;
		else
			return null;
	}

	/// <summary>
	/// Get the other Transform, given a Transform.
	/// Samples the first entry in JelloCollision.contacts.
	/// </summary>
	/// <param name="transform">The given Transform.</param>
	/// <returns>The other Transform. Null if the given Transform does not belong to this JelloCollision or if JelloCollision.contacts is null.</returns>
	public Transform GetOtherTransform(Transform transform)
	{
		if(contacts == null)
			return null;

		if(transform == contacts[0].transformA)
			return contacts[0].transformB;
		else if(transform == contacts[0].transformB)
			return contacts[0].transformA;
		else
			return null;
	}
}

/// <summary>
/// Supplementary Collider2D infomation.
/// This serves as a 'one size fits all' class that makes processing the collision and subsequent resolution
/// of different Collider2D types more manageable. 
/// </summary>
public class SupplementaryColliderInfo
{
	/// <summary>
	/// The amount of time elapsed since this was last updated by the simulation.
	/// Updates are set to happen when collisions occur.
	/// </summary>
	public float TimeSinceLastUpdate = Mathf.Infinity;// -1f;

	/// <summary>
	/// The previous position.
	/// </summary>
	public Vector3 prevPosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

	/// <summary>
	/// The previous rotation.
	/// </summary>
	public float prevRotation = Mathf.Infinity;

	/// <summary>
	/// The previous scale.
	/// </summary>
	public Vector3 prevScale = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

	/// <summary>
	/// The winding direction of the Collider2D.
	/// </summary>
	public JelloClosedShape.Winding winding = JelloClosedShape.Winding.Unassigned;

	/// <summary>
	/// The Axis Aligned Bounding Box for the Collider2D.
	/// </summary>
	public JelloAABB AABB = new  JelloAABB();

	/// <summary>
	/// The Collider2D vertices (perimeter).
	/// </summary>
	public Vector2[] ColliderVertices;

	/// <summary>
	/// The previous collider vertices (from last FixedUpdate()).
	/// </summary>
	public Vector2[] PrevColliderVertices;

	/// <summary>
	/// The normal vectors of each edge of the Collider2D.
	/// </summary>
	public Vector2[] EdgeNormals;

	/// <summary>
	/// The point normal vectors for each point of the Collider2D.
	/// </summary>
	public Vector2[] PointNormals;

	/// <summary>
	/// the inverse of the squared length of each edge of the Collider2D.
	/// </summary>
	public float[] InverseSquaredEdgeLengths;

	/// <summary>
	/// The largest calculated 'minumum translation vector'.
	/// Used for positional correction.
	/// </summary>
	public Vector2[] largestMTVs;

	/// <summary>
	/// The velocities to be added to each point of the Collider2D.
	/// Stored and applied after all JelloContact objects are resolved.
	/// </summary>
	public Vector2[] queuedVelocities;

	/// <summary>
	/// The number of JelloContact objects each point of the Collider2D is involved in.
	/// </summary>
	public int[] numEngagments;

	/// <summary>
	/// The Collider2D.
	/// </summary>
	public Collider2D collider;

	/// <summary>
	/// The JelloBody, if any, assosiated with the Collider2D.
	/// </summary>
	public JelloBody body;

	/// <summary>
	/// The Transform assosiated with the Collider2D.
	/// </summary>
	public Transform transform;

	/// <summary>
	/// The Rigidbody2D, if any, assosiated with the Collider2D.
	/// </summary>
	public Rigidbody2D rigidbody;

	//	public Vector2[] largestVelocities;
	//	public float[] queuedAngularVelocities;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="collider2D">The Collider2D to fill out information about.</param>
	/// <param name="jelloBody">The Jellobody assosiated with the Collider2D.</param>
	public SupplementaryColliderInfo (Collider2D collider2D, JelloBody jelloBody = null)
	{
		collider = collider2D;
		body = jelloBody;
		transform = collider2D.transform;
		rigidbody = collider2D.GetComponent<Rigidbody2D>();

		Update();
	}

	/// <summary>
	/// Update the information assosiated with the SupplementaryColliderInfo.collider.
	/// This is called every time a collision occurs.
	/// </summary>
	public void Update()
	{
		if(TimeSinceLastUpdate == 0f)
			return;



		//TODO if the body didnt move at all, dont update it?
//		if(body == null)
//		{
//		   if(transform.position == prevPosition && transform.eulerAngles.z == prevRotation && transform.localScale == prevScale)
//				return;
//		}
//		else if(ColliderVertices != null && ColliderVertices.Length != 0 && PrevColliderVertices != null && PrevColliderVertices.Length != 0)
//		{
//			bool allSame = true;
//			for(int i = 0; i < ColliderVertices.Length; i++)
//			{
//				if(ColliderVertices[i] != PrevColliderVertices[i])
//				{
//					allSame = false;
//					break;
//				}
//			}
//
//			if(allSame)
//				return;
//		}
		
		prevPosition = transform.position; //TODO i need to guess the previous positions if timesinceupdate is greater than 1?
		prevRotation = transform.eulerAngles.z; 
		prevScale = transform.localScale;


		if(collider.GetType() == typeof(PolygonCollider2D))
		{
			PolygonCollider2D polyCollider = (PolygonCollider2D)collider;
			
			PrimeCollisionInfo(polyCollider.points);
		}
		else if(collider.GetType() == typeof(BoxCollider2D))
		{
			BoxCollider2D boxCollider = (BoxCollider2D)collider;

			Vector2[] vertices = new Vector2[4];
			vertices[0] = boxCollider.offset + new Vector2(boxCollider.size.x, boxCollider.size.y) * 0.5f;
			vertices[1] = boxCollider.offset + new Vector2(boxCollider.size.x, -boxCollider.size.y) * 0.5f;
			vertices[2] = boxCollider.offset + new Vector2(-boxCollider.size.x, -boxCollider.size.y) * 0.5f;
			vertices[3] = boxCollider.offset + new Vector2(-boxCollider.size.x, boxCollider.size.y) * 0.5f;

			PrimeCollisionInfo(vertices);
		}
		else if(collider.GetType() == typeof(CircleCollider2D))
		{
			Vector2[] vertices = new Vector2[1];
			PrimeCollisionInfo(vertices);
		}
	
		//this prevents the info from being updated more than once per fixed update.
		TimeSinceLastUpdate = 0f;
	}

	/// <summary>
	/// Ready the SupplementaryColliderInfo to be used in the collision detection and resolution process.
	/// </summary>
	/// <param name="vertices">The vertices of the points of the Collider2D.</param>
	private void PrimeCollisionInfo(Vector2[] vertices) //TODO could possibly reduce the code required here by only modifying by a change in position, scale and/or angle.
	{	
		if(ColliderVertices == null || ColliderVertices.Length - 2 != vertices.Length)
		{
			ColliderVertices = new Vector2[vertices.Length + 2]; //add first and last to the ends of this array for ease of looping.
			PrevColliderVertices = new Vector2[ColliderVertices.Length];
			EdgeNormals = new Vector2[vertices.Length];
			PointNormals = new Vector2[vertices.Length];
			InverseSquaredEdgeLengths = new float[vertices.Length];
			largestMTVs = new Vector2[vertices.Length];
			queuedVelocities = new Vector2[vertices.Length];
			numEngagments = new int[vertices.Length];
//			largestVelocities = new Vector2[vertices.Length];
//			queuedAngularVelocities = new float[vertices.Length];
		}

		if(vertices.Length == 0)
			return;

//		PrevColliderVertices = ColliderVertices; //todo i think i need better info than this... i may need to upate each and every frame? that would suck!!!
		if(TimeSinceLastUpdate <= Time.fixedDeltaTime)
		{
			for(int i = 0; i < ColliderVertices.Length; i++) //TODO i need to guess the previous positions if timesinceupdate is greater than 1?
			{	
				PrevColliderVertices[i] = ColliderVertices[i];
			}
		}

		if(winding == JelloClosedShape.Winding.Unassigned)
		{
			if(JelloShapeTools.HasClockwiseWinding(vertices))
				winding = JelloClosedShape.Winding.Clockwise;
			else
				winding = JelloClosedShape.Winding.CounterClockwise;
		}
		
		for(int i = 0; i < vertices.Length; i++)
			ColliderVertices[i + 1] = transform.TransformPoint(vertices[i]);
		ColliderVertices[0] = ColliderVertices[ColliderVertices.Length - 2];
		ColliderVertices[ColliderVertices.Length - 1] = ColliderVertices[1];
		
		int windingModifier = winding == JelloClosedShape.Winding.Clockwise ? 1 : -1;
		for(int i = 0; i < vertices.Length; i++) //TODO only modify if certain values have changed...
		{	
			EdgeNormals[i] = JelloVectorTools.getPerpendicular( ColliderVertices[i + 2] - ColliderVertices[i + 1]) * windingModifier;
			InverseSquaredEdgeLengths[i] = EdgeNormals[i] != Vector2.zero ? 1 / EdgeNormals[i].sqrMagnitude : 0f;
			PointNormals[i] = JelloVectorTools.getPerpendicular(ColliderVertices[i + 1] - ColliderVertices[i] + ColliderVertices[i + 2] - ColliderVertices[i + 1]) * windingModifier;
		}

		UpdateAABB(ColliderVertices);


		//down here project backwards for previous collider verts if need be...
		if(TimeSinceLastUpdate >Time.fixedDeltaTime)// || TimeSinceLastUpdate == -1)
		{
			if(rigidbody == null)//no movement, so stays in place...
			{
				for(int i = 0; i < ColliderVertices.Length; i++)
					PrevColliderVertices[i] = ColliderVertices[i];
			}
			else if(body == null)//project back based on rigidbody velocity/angular velocity
			{
				//vbp = contact.rigidbodyB.velocity + contact.rigidbodyB.angularVelocity * Mathf.Deg2Rad * new Vector2(-contact.R2.y, contact.R2.x);
				for(int i = 0; i < ColliderVertices.Length; i++)
				{
					Vector2 R = ColliderVertices[i] - (Vector2)transform.position;
					Vector2 Vel =  rigidbody.velocity + rigidbody.angularVelocity * Mathf.Deg2Rad * new Vector2(-R.y, R.x);
					PrevColliderVertices[i] = ColliderVertices[i] - Vel * Time.fixedDeltaTime;
				}
			}
			else
			{
				for(int i = 0; i < body.EdgePointMassCount; i++)//TODO account for first and last buffers...
				{
					PrevColliderVertices[i + 1] = ColliderVertices[i + 1] - body.getPointMass(i).velocity * Time.fixedDeltaTime;
				}
				PrevColliderVertices[0] = PrevColliderVertices[PrevColliderVertices.Length - 2];
				PrevColliderVertices[PrevColliderVertices.Length -1] = PrevColliderVertices[1];
			}
		}
	}

	/// <summary>
	/// Update the SupplementaryColliderInfo only where needed.
	/// This is needed as MTV's are proccessed (minimum translation vector).
	/// </summary>
	/// <param name="index">The index of the point to update.</param>
	/// <param name="position">The new position of the point to update.</param>
	/// <param name="offset">The amount to offset all Collider2D points (Applied only if SupplementaryColliderInfo.body is null).</param>
	public void UpdateCollisionInfo(int index, Vector2 position, Vector2 offset)
	{
		if(body == null)
		{
			for(int i = 0; i < ColliderVertices.Length; i++)
				ColliderVertices[i] += offset;
			
			AABB.Max += offset;
			AABB.Min += offset;
		}
		else
		{
			if(index == 0)
			{
				ColliderVertices[1] = position;
				ColliderVertices[ColliderVertices.Length - 1] = position;
			}
			else if(index == body.polyCollider.points.Length - 1)
			{
				ColliderVertices[0] = position;
				ColliderVertices[ColliderVertices.Length - 2] = position;
			}
			else
			{
				ColliderVertices[index + 1] = position;
			}

			int windingModifier = body.Shape.winding == JelloClosedShape.Winding.Clockwise ? 1 : -1;
			PointNormals[index] = JelloVectorTools.getPerpendicular(ColliderVertices[index + 1] - ColliderVertices[index] + ColliderVertices[index + 2] - ColliderVertices[index + 1]) * windingModifier;
			EdgeNormals[index] = JelloVectorTools.getPerpendicular( ColliderVertices[index + 2] - ColliderVertices[index + 1]) * windingModifier;
			InverseSquaredEdgeLengths[index] = EdgeNormals[index] != Vector2.zero ? 1 / EdgeNormals[index].sqrMagnitude : 0f;
			
			int prev = index - 1 >= 0 ? index - 1 : EdgeNormals.Length - 1;
			PointNormals[prev] = JelloVectorTools.getPerpendicular(ColliderVertices[index] - ColliderVertices[prev] + ColliderVertices[index + 1] - ColliderVertices[index]) * windingModifier;
			EdgeNormals[prev] = JelloVectorTools.getPerpendicular( ColliderVertices[index + 1] - ColliderVertices[index]) * windingModifier;
			InverseSquaredEdgeLengths[prev] = EdgeNormals[prev] != Vector2.zero ? 1 / EdgeNormals[prev].sqrMagnitude : 0f;

			int next = index + 1 < EdgeNormals.Length ? index + 1 : 0;
			PointNormals[next] = JelloVectorTools.getPerpendicular(ColliderVertices[next + 1] - ColliderVertices[next] + ColliderVertices[next + 2] - ColliderVertices[next + 1]) * windingModifier;

			AABB.expandToInclude(ColliderVertices[index + 1]);
		}
	}

	/// <summary>
	/// Update SupplementaryColliderInfo.AABB.
	/// </summary>
	/// <param name="vertices">Vertices.</param>
	private void UpdateAABB(Vector2[] vertices)
	{
		AABB.clear();
		for(int i = 0; i < vertices.Length; i++)
			AABB.expandToInclude(vertices[i]);
	}

	/// <summary>
	/// Draws the SupplementaryColliderInfo.ColliderVertices, SupplementaryColliderInfo.EdgeNormals, and SupplementaryColliderInfo.PointNormals.
	/// </summary>
	public void DebugDrawMe()
	{
		for(int i = 0; i < ColliderVertices.Length - 2; i++)
		{
			Color color = Color.green;

			Debug.DrawLine(ColliderVertices[i + 1], ColliderVertices[i + 2], color);
			Debug.DrawRay((ColliderVertices[i + 1] + ColliderVertices[i + 2]) * 0.5f, EdgeNormals[i], color);

			color = Color.yellow;
			Debug.DrawRay(ColliderVertices[i + 1], PointNormals[i], color);
		
			color = Color.gray;
			Debug.DrawLine(PrevColliderVertices[i + 1], PrevColliderVertices[i + 2], color);
		}
	}


}

