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

//TODO add a max velocity?
//TODO have two kinematic modes? velocityfrompositions and positionsfromvelocity

/// <summary>
/// Contains base functionality for all JelloBody types in Jello Physics. All bodies are
/// made up of a JelloClosedShape geometry, and a list of JelloPointMass objects equal to the number of vertices in the
/// JelloClosedShape geometry.  The vertices are considered to be connected by lines in order, which creates the collision
/// volume for the JelloBody. Individual implementations of JelloBody handle forcing the body to keep it"s shape through
/// various methods such as JelloSpring objects and pressure forces.
/// </summary>
[RequireComponent (typeof (Rigidbody2D), typeof (PolygonCollider2D))]
public class JelloBody : MonoBehaviour
{

    #region PRIVATE / PROTECTED VARIABLES

	/// <summary>
	/// This JelloBody Transform cache.
	/// </summary>
	[SerializeField]
	protected Transform myTransform;

	/// <summary>
	/// The JelloPointMass positions in local space.
	/// Will populate this in the Update() funciton and send it to the MeshLink for mesh deformation.
	/// </summary>
	protected Vector2[] pointMassPositions;

	/// <summary>
	/// The JelloClosedShape that represents the geometry of this JelloBody.
	/// </summary>
	[SerializeField]
    protected JelloClosedShape mBaseShape;

	/// <summary>
	/// The edge JelloPointMass objects.
	/// </summary>
	[SerializeField]
	protected JelloPointMass[] mEdgePointMasses = new JelloPointMass[0];

	/// <summary>
	/// The internal JelloPointMass objects.
	/// These objects do not make up the outter edge and are not considered in collision detection/resolution.
	/// </summary>
	[SerializeField]
	protected JelloPointMass[] mInternalPointMasses = new JelloPointMass[0];

	/// <summary>
	/// Whether or not the JelloBody is static.
	/// A static JelloBody and all of its JelloPointMass objects will not accept or integrate any forces.
	/// </summary>
	[SerializeField]
    protected bool mIsStatic = true;
	
	/// <summary>
	/// Whether or not the JelloBody is kinematic.
	/// The position and angle of a kinematic JelloBody will not change, but JelloPointMass objects will accept forces and integrate them.
	/// A non-kinematic JelloBody will have its position and angle determined by the average positions of its JelloPointMass objects in the
	/// JelloBody.derivePositionAndAngle() method.
	/// </summary>
	[SerializeField]
    protected bool mIsKinematic;
	
	/// <summary>
	/// Whether or not the JelloBody is a trigger.
	/// When true, the JelloBody functions as normal but ignores all collision.
	/// </summary>
	[SerializeField]
	protected bool mIsTrigger;
	
	/// <summary>
	/// Weather or not the JelloBody is awake.
	/// A sleeping JelloBody does not integrate forces but can still be collided against.
	/// A sleeping JelloBody will be woken up by collisions.
	/// </summary>
	[SerializeField]
	protected bool mIsAwake = true;
	
	/// <summary>
	/// The mass.
	/// </summary>
	[SerializeField]
	protected float mMass = 1f;
	
	/// <summary>
	/// The inverse mass.
	/// </summary>
	[SerializeField]
	protected float mInverseMass = 1f;

	//TODO create pivot change methods...
	/// <summary>
	/// The pivot offset.
	/// Required for shape matching with non-center pivot points.
	/// </summary>
	[SerializeField]
	public Vector2 pivotOffset;

	/// <summary>
	/// The previous position.
	/// Used to determine velocity in some cases
	/// (See JelloBody.derivePositionAndAngle())
	/// </summary>
	protected Vector2 prevPosition;
	
	/// <summary>
	/// The previous angle.
	/// Used for determining angular velocity in some cases.
	/// (See JelloBody.derivePositionAndAngle())
	/// </summary>
	protected float mLastAngle;

	/// <summary>
	/// The JelloAttachPoint objects assosiated with this JelloBody.
	/// </summary>
	[SerializeField]
	protected JelloAttachPoint[] mAttachPoints;

	/// <summary>
	/// The JelloJoint objects assosiated with this JelloBody.
	/// </summary>
	[SerializeField]
	protected JelloJoint[] mJoints;
	
	#endregion
	
	#region PUBLIC VARIABLES
	
	/// <summary>
	/// The Collider2D component used to interaact with Unity's 2D physics system.
	/// JelloBody objects are required to use the PolygonCollider2D.
	/// </summary>
	public PolygonCollider2D  polyCollider;
	
	/// <summary>
	/// Whether or not the JelloBody is affected by gravity.
	/// </summary>
	public bool affectedByGravity = true;
	
	/// <summary>
	/// The velocity of the JelloBody.
	/// </summary>
	public Vector2 velocity;
	
	/// <summary>
	/// The angular velocity (in radians) of the JelloBody.
	/// </summary>
	public float angularVelocity;
	
	/// <summary>
	/// How many iterations this JelloBody has been qualified to be put asleep.
	/// </summary>
	public int sleepCount;
	
	/// <summary>
	/// The gravity vector to be applied to the JelloBody when JelloBody.overrideWorldGravity is set to true.
	/// This allows the JelloBody to have gravity independant of Unity's Physics.gravity setting.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Change the gravity of a body when entering a trigger zone.
	/// 
	/// void OnTriggerEnter2D(Collider2D coll)
	/// {
	/// 	JelloBody body = coll.GetComponent<JelloBody>();
	/// 	if(body != null)
	/// 	{
	/// 		body.overrideWorldGravity = true;
	/// 		body.gravity = Vector.up;
	/// 	}
	/// }
	/// ~~~
	public Vector2 gravity = new Vector2(0, -9.81f);
	
	/// <summary>
	/// Whether or not this JelloBody is overriding Unity's Physics.gravity setting.
	/// If this and JelloBody.affectedByGravity are set to true, the JellBody.gravity setting will be applied to this JelloBody during JelloBody.accumulateExternalForces().
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Change the gravity of a body when entering a trigger zone.
	/// 
	/// void OnTriggerEnter2D(Collider2D coll)
	/// {
	/// 	JelloBody body = coll.GetComponent<JelloBody>();
	/// 	if(body != null)
	/// 	{
	/// 		body.overrideWorldGravity = true;
	/// 		body.gravity = Vector.up;
	/// 	}
	/// }
	/// ~~~
	public bool overrideWorldGravity = false;

	/// <summary>
	/// Wheter or not this JelloBody is disabled.
	/// A disabled JelloBody does not integrate forces and is not tested against for collisions. 
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Disable a body when it is too far away from a given point.
	/// //enable the body if it is close enough.
	/// JelloBody body;
	/// 
	/// if(Vector2.Distance(body.Position, givenPoint) > 100f)
	/// 	body.disabled = true;
	/// else
	/// 	body.disabled = false;
	/// ~~~
	public bool disabled; //TODO make sure this works as it should

	/// <summary>
	/// Each JelloCollision this JelloBody was involved in this FixedUpdate().
	/// Used to track and resolve JelloCollision events.
	/// </summary>
	public List<JelloCollision> collisions = new List<JelloCollision>();
	
	/// <summary>
	/// Each JelloCollision this body was involved in the previous FixedUpdate().
	/// Used to wake up any sleeping JelloBody that was colliding with this JelloBody when it woke up.
	/// </summary>
	public JelloCollision[] previousCollisions = new JelloCollision[0];
    
	/// <summary>
	/// The MeshLink.
	/// The go-between for the JelloPointMass positions of the JelloBody and the MeshFilter.mesh.
	/// </summary>
	public MeshLink meshLink;

	/// <summary>
	/// The minimum translation vector modifier for this JelloBody.
	/// When collisions occur, the JelloPointMass objects invloved are moved away from each until no longer penetrating before the collision impulse is applied.
	/// This modifier will increase or decrease the distance that the JelloPointMass objects are pushed (a negative value will leave some penetration.). 
	/// </summary>
	public float mtvModifier = 0f;

	#endregion
    
	#region PUBLIC PROPERTIES

	/// <summary>
	/// Gets the JelloWorld that this JelloBody belongs to.
	/// </summary>
	/// <value>The JelloWorld.</value>
	public static JelloWorld World
	{
		get{ return JelloWorld.World; }
	}
	
	/// <summary>
	/// Gets the number JelloAttachPoint objects are assosiated with this JelloBody.
	/// </summary>
	/// <value>The JelloAttachPoint count.</value>
	public int AttachPointCount
	{
		get
		{ 
			if(mAttachPoints == null)
				return 0;
			else
				return mAttachPoints.Length; 
		}
	}
	
	/// <summary>
	/// Gets the number of JelloJoint objects assosiated with this JelloBody.
	/// </summary>
	/// <value>The JelloJoint count.</value>
	public int JointCount
	{
		get
		{
			if(mJoints == null)
				return 0;
			else
				return mJoints.Length;
		}
	}

	/// <summary>
	/// Gets the number of JelloPointMass objects in the JelloBody.
	/// The sum of the edge and internal JelloPointMass objects.
	/// </summary>
	/// <value>The total JelloPointMass count.</value>
	public int PointMassCount
	{
		get { return mEdgePointMasses.Length + mInternalPointMasses.Length; }
	}

    /// <summary>
    /// Gets the number edge of JelloPointMass objects in the JelloBody.
    /// </summary>
	/// <value>The edge JelloPointMass count.</value>
    public int EdgePointMassCount
    {
        get { return mEdgePointMasses.Length; }
    }

	/// <summary>
	/// Gets the number of internal JelloPointMass objects in the JelloBody.
	/// </summary>
	/// <value>The internal JelloPointMass count.</value>
	public int InternalPointMassCount
	{
		get { return mInternalPointMasses.Length; }
	}
	
	/// <summary>
	/// Gets the JelloClosedShape that is the base geometry that builds the JelloBody. 
	/// </summary>
	/// <value>The base JelloClosedShape.</value>
	public JelloClosedShape Shape
	{
		get{ return mBaseShape; }
	}
	
	/// <summary>
	/// Gets the positions the JelloBody.Shape in global space.
	/// (Calculated each call)
	/// </summary>
	/// <value>The base JelloClosedShape positions in global space.</value>
	public Vector2[] GlobalShape//TODO check how/where this is being used... get rid of if not being used?
	{
		get 
		{
			return mBaseShape.transformVertices(Position, Angle, Scale);
		}
	}

    /// <summary>
	/// Gets / Sets whether this is a static JelloBody. 
	/// A static JelloBody and all of its JelloPointMass objects will not accept or integrate any forces.
	/// Setting static greatly improves performance on static bodies.
    /// </summary>
	/// <value>Whether or not the JelloBody is static.</value>
    public bool IsStatic
    {
        get { return mIsStatic; }
        set { mIsStatic = value; }
    }

    /// <summary>
    /// Gets / Sets whether this JelloBody is kinematically controlled.
	/// The position and angle of a kinematic JelloBody will not change, but JelloPointMass objects will accept forces and integrate them.
	/// A non-kinematic JelloBody will have its position and angle determined by the average positions of its JelloPointMass objects in the
	/// JelloBody.derivePositionAndAngle() method.
    /// </summary>
	/// <value>Whether or not the JelloBody is kinematic.</value>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Change a spring body to kinematic and change its position every frame.
	/// //The point masses will move with the set position via shape matching forces.
	/// JelloSpringBody springBody;
	/// 
	/// void Start()
	/// {
	/// 	springBody.IsKinematic = true;
	/// 	springBody.ShapeMatching = true;
	/// }
	/// 
	/// void FixedUpdate()
	/// {
	/// 	springBody.Position = springBody.Position + Vector2.up;
	/// }
	/// ~~~
    public bool IsKinematic
    {
        get { return mIsKinematic; }
        set { mIsKinematic = value; }
    }
	
	/// <summary>
	/// Gets / Sets whether or not this body responds to collisions.
	/// </summary>
	/// <value>Whether or not the JelloBody is a trigger.</value>
	 public bool IsTrigger
    {
        get { return mIsTrigger; }
        set { mIsTrigger = value; }
	}

	/// <summary>
	/// Gets / sets whether this JelloBody is awake.
	/// A sleeping JelloBody does not integrate forces but can still be collided against.
	/// A sleeping JelloBody will be woken up by collisions.
	/// </summary>
	/// <value>Whether or not the JelloBody is awake.</value>
	public bool IsAwake
	{
		get{ return mIsAwake; }
		set
		{
			if(value && !mIsAwake)	//wake up any other sleeping bodies touching this one when when woken up
			{
				mIsAwake = value;
				
				for(int i = 0; i < previousCollisions.Length; i++)
				{
					if(previousCollisions[i].GetOtherBody(this) != null)
						previousCollisions[i].GetOtherBody(this).IsAwake = true;
					else if (previousCollisions[i].GetOtherCollider(polyCollider).GetComponent<Rigidbody2D>() != null)
						previousCollisions[i].GetOtherCollider(polyCollider).GetComponent<Rigidbody2D>().WakeUp();
				}
			}
			else
			{ 
				mIsAwake = value;

				sleepCount = 0;

				if(!mIsAwake)
				{
					UpdateCollider();

					previousCollisions = collisions.ToArray();
					collisions.Clear();
				}
			}
		}
	}
	
	/// <summary>
	/// Gets The previous angle.
	/// Used for determining angular velocity in some cases.
	/// (See JelloBody.derivePositionAndAngle())
	/// </summary>
	/// <value>The previous angle for the JelloBody.</value>
	public float LastAngle
	{
		get{ return mLastAngle; }
	}
	
	/// <summary>
	/// Gets or sets the mass.
	/// Setting to a value greater than 10,000 will be considered static
	/// and JelloBody.Mass will be set to Mathf.Infinity and JelloBody.IsStatic will be set to true. 
	/// Any JelloPointMass objects belonging to this JelloBody who's JelloPointMass.Mass is equal to the JelloBody.Mass will also be 
	/// changed to the new value. 
	/// JelloBody.InverseMass and JelloPointMass.InverseMass will also be updated.
	/// <value>The mass of the JelloBody.</value>
	/// </summary>
	/// <value>The mass of the JelloBody.</value>
	public float Mass
	{
		get{ return mMass; }
		set
		{
			float oldMass = mMass;
			mMass = Mathf.Max(0f, value);
			if(mMass >= 10000)
			{
				mMass = Mathf.Infinity;
				IsStatic = true;
			}
			else
			{
				IsStatic = false;
			}

			mInverseMass =  (mMass > 0f && !IsStatic) ? 1f / value : 0f;
			//update unmodified pointmasses
			for(int i = 0; i < mEdgePointMasses.Length; i++)
				if(mEdgePointMasses[i].Mass == oldMass)
					mEdgePointMasses[i].Mass = value;
			for(int i = 0; i < mInternalPointMasses.Length; i++)
				if(mInternalPointMasses[i].Mass == oldMass)
					mInternalPointMasses[i].Mass = value;
		}
	}
	
	/// <summary>
	/// Gets the inverse of JelloBody.Mass.
	/// If JelloBody.Mass is infinity, the returned value will be zero.
	/// </summary>
	/// <value>The inverse of JelloBody.Mass for the JelloBody.</value>
	public float InverseMass
	{
		get{ return mInverseMass; }
	}
	
	/// <summary>
	/// Gets or sets the position of the JelloBody.
	/// This reads / modifies the transform.Position directly.
	/// Unless JelloBody.IsKinematic is set to true, the position will be automatically calculated in JelloBody.derivePositionAndAngle().
	/// </summary>
	/// <value>The position of the JelloBody.</value>
	public Vector2 Position
	{
		get { return myTransform.position; }
		set { myTransform.position = new Vector3 (value.x, value.y, transform.position.z); }
	}
	
	/// <summary>
	/// Gets or sets the angle of the JelloBody.
	/// This reads / modifies the transform.eulerAngles.z directly.
	/// Unless JelloBody.IsKinematic is set to true, the angle will be automatically calculated in JelloBody.derivePositionAndAngle().
	/// </summary>
	/// <value>The angle of the JelloBody.</value>
	public float Angle
	{
		get{ return myTransform.eulerAngles.z; }
		set{ myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y, value); }
	}
	
	/// <summary>
	/// Gets the angular velocity of the JelloBody (in degrees). Read only.
	/// </summary>
	/// <value>The anglular velocity of the JelloBody (in degrees).</value>
	public float AngularVelocityInDegrees
	{
		get{ return angularVelocity * Mathf.Rad2Deg; }
	}
	
	/// <summary>
	/// Gets or sets the scale of the JelloBody.
	/// This reads/modifies the transform.localScale directly.
	/// </summary>
	/// <value>The scale of the JelloBody.</value>
	public Vector3 Scale
	{
		get
		{
			return myTransform.localScale;
		}
		set
		{
			myTransform.localScale = value;
			HandleScaleModification();
		}
	}

    #endregion
	
	#region START AND UPDATE

	void Awake() //TODO make sure any/all of this is precomputed in editor.
	{
		myTransform = transform;
		polyCollider = (PolygonCollider2D)GetComponent<Collider2D>();

		//HACK this is to handle how ragespline keeps recreating the polygon collider with duplicate points
		if(meshLink != null && meshLink.meshLinkType == MeshLink.MeshLinkType.RageSplineMeshLink)
		{
			Vector2[] points = JelloShapeTools.RemoveDuplicatePoints(polyCollider.points);
			if(polyCollider.points.Length != points.Length)
				polyCollider.points = points;
		}

		//set pointmass positions to polygoncollider positions
		if(polyCollider.points.Length != mEdgePointMasses.Length)
		{
			JelloClosedShape shape = new JelloClosedShape(polyCollider.points, mBaseShape != null ? mBaseShape.InternalVertices : null, false);
			setShape(shape, ShapeSettingOptions.MovePointMasses);
		}
		else
		{
			Shape.changeVertices(polyCollider.points, Shape.InternalVertices);
			setShape(mBaseShape, ShapeSettingOptions.MovePointMasses);
		}
	
		Scale = Scale;
	}

	void Start()
	{
		World.addBody(this);

		//ClearInvalidSubComponents();//TODO can get rid of this here because it's also in setshape?

		if(mJoints != null)
			for(int i = 0; i < mJoints.Length; i++)
				World.addJoint(mJoints[i]);

		prevPosition = Position;
		mLastAngle = Angle;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
		polyCollider.isTrigger = true;
	}

	void Update()
	{
		if(!IsStatic && meshLink != null)
		{
			if( pointMassPositions == null || pointMassPositions.Length != mEdgePointMasses.Length + mInternalPointMasses.Length)
				pointMassPositions = new Vector2[mEdgePointMasses.Length + mInternalPointMasses.Length];
			
			for(int i = 0; i < mEdgePointMasses.Length; i++)
				pointMassPositions[i] = mEdgePointMasses[i].LocalPosition;
			for(int i = 0; i < mInternalPointMasses.Length; i++)
				pointMassPositions[i + mEdgePointMasses.Length] = mInternalPointMasses[i].LocalPosition;

			meshLink.UpdateMesh(pointMassPositions);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		triggered(coll);
	}

	void OnTriggerStay2D(Collider2D coll) //Not actually being called?
	{
		triggered(coll);
	}


	/// <summary>
	/// Because Unity does not allow per-collision filtering, the JelloBody.polyCollider.isTrigger is set to true and collision details are 
	/// generated manually. 
	/// </summary>
	/// <param name="coll">Collider2D that this JelloBody.polyCollider intersected overlaped.</param>
	private void triggered(Collider2D coll)
	{
//		Benchy.Begin("Decide if handle");

		if(coll == null) //this happens some times but i dont understand why...
			return;

		//only if other body didnt handle this collision
		bool handle = true;

		JelloBody otherBody = coll.GetComponent<JelloBody>(); //TODO where does disabled fit in here???

		if(mIsTrigger)
			handle = false;

		if(otherBody != null)
		{
			if(otherBody.IsTrigger ||  (mIsStatic && otherBody.IsStatic))
				handle = false;
			
			if(!IsAwake && (!otherBody.IsAwake || otherBody.IsStatic))
				handle = false;

			if(otherBody.IsAwake)
			{
				for(int i = 0; i < otherBody.collisions.Count; i++)
				{
					if(otherBody.collisions[i].GetOtherBody(otherBody) == this)
					{
						handle = false;
						collisions.Add (otherBody.collisions[i]);
						break;
					}
				}
			}
		}
		else
		{
			if(coll.isTrigger)
				handle = false;
		
			if(!IsAwake)
			{
				Rigidbody2D rbody = coll.GetComponent<Rigidbody2D>();
				if(rbody != null)
				{
					if(rbody.IsSleeping())
						handle = false;
				}
				else
				{
					handle = false;
				}
			}
		}
//		Benchy.End("Decide if handle");




		if(handle)
		{
			//wake up here?
			//			Benchy.Begin("Wakeup");
			//wake any sleeping bodies if need be
//			float wakeVelocity = 2f;
//			float wakeAngularVelocity = 2f;
			
			if(!mIsAwake)
			{
				if(otherBody != null && otherBody.IsAwake)
				{
					if(!otherBody.IsKinematic)
					{
						if(otherBody.velocity.sqrMagnitude > World.wakeVelocity * World.wakeVelocity || Mathf.Abs( otherBody.angularVelocity ) > World.wakeAngularVelocity)
						{
							mIsAwake = true;	 sleepCount = 0; 
						}
					}
					else
					{
						mIsAwake = true;	 sleepCount = 0;
					}
				}
				else
				{
					Rigidbody2D rbody = coll.GetComponent<Rigidbody2D>();
					if(rbody != null && !rbody.IsSleeping())
					{
						if(rbody.velocity.sqrMagnitude > World.wakeVelocity * World.wakeVelocity || Mathf.Abs( rbody.angularVelocity ) > World.wakeAngularVelocity)
						{
							mIsAwake = true; 	sleepCount = 0; 
						}
					}
				}
			}
			
			if(otherBody != null && !otherBody.IsAwake)
			{
				if(mIsAwake)
				{
					if(!mIsKinematic) //TODO think about using the collision relative velocity in order to wake a body up?
					{
						if(velocity.sqrMagnitude > World.wakeVelocity * World.wakeVelocity || Mathf.Abs( angularVelocity ) > World.wakeAngularVelocity)
						{
							otherBody.IsAwake = true;	otherBody.sleepCount = 0; 
						}
					}
					else
					{
						otherBody.IsAwake = true; otherBody.sleepCount = 0;
					}
				}
			}
			
			//			Benchy.End("Wakeup");






			SupplementaryColliderInfo Info;
			bool alreadyTracked = World.StartTrackingCollider(polyCollider, out Info, this);

			if(alreadyTracked)
				Info.Update();

			SupplementaryColliderInfo otherInfo;
			alreadyTracked = World.StartTrackingCollider(coll, out otherInfo, otherBody);

			if(alreadyTracked)
				otherInfo.Update();
//			Benchy.Begin("BodyCollide");
			//find fine detail of collision.
			List<JelloContact> contacts = new List<JelloContact>();
			Type t2 = coll.GetType();
			if(t2 == typeof(CircleCollider2D))
			{
				World.BodyCollide((CircleCollider2D)coll, Info, ref contacts);
			}
			else //polygon and box collisions handled here.
			{
				World.BodyCollide (Info, otherInfo, ref contacts);
			}

			if(contacts.Count <= 0)
				return;
//			Benchy.End("BodyCollide");

//			Benchy.Begin("CollisionEvent");
			collisions.Add (new JelloCollision(contacts.ToArray()));


			if(collisions[collisions.Count - 1].GetOtherBody(this) != null)
			{
				collisions[collisions.Count - 1].GetOtherBody(this).collisions.Add(collisions[collisions.Count - 1]);
				collisions[collisions.Count - 1].GetOtherBody(this).OnJelloCollision(collisions[collisions.Count - 1]);
			}
			OnJelloCollision(collisions[collisions.Count - 1]);


			if(collisions[collisions.Count - 1].ignore)
				return;
//			Benchy.End("CollisionEvent");

			//handle the collision
//			Benchy.Begin("Resolve");
			int numContacts = 0;
			for(int i = 0; i < collisions[collisions.Count - 1].contacts.Length; i++)
			{
				if(!collisions[collisions.Count - 1].contacts[i].ignore)
					numContacts++;
			}
			numContacts = numContacts > 0 ? numContacts : 1;

			//Debug.Log (numContacts);

			for(int i = 0; i < collisions[collisions.Count - 1].contacts.Length; i++)
			{
				if(collisions[collisions.Count - 1].contacts[i].ignore)
					continue;

				collisions[collisions.Count - 1].contacts[i].numContacts = numContacts;
			
				World.ApplyCollisionImpulse(collisions[collisions.Count - 1].contacts[i]);
			}
//			Benchy.End("Resolve");
			//TODO take into account circle collider collisions
			//TODO do this with velocities as well?
			for(int i = 0; i < Info.largestMTVs.Length; i++)
			{
				Vector2 mtv = Info.largestMTVs[i];
				if(mtv != Vector2.zero)
				{
					mEdgePointMasses[i].Position += Info.largestMTVs[i];

					//mEdgePointMasses[i].Position += Info.numEngagments[i] != 0 ? Info.largestMTVs[i] / Info.numEngagments[i] :  Info.largestMTVs[i];
					Info.UpdateCollisionInfo(i, mEdgePointMasses[i].Position, Vector2.zero);
					Info.largestMTVs[i] = Vector2.zero;
				}

				if(Info.numEngagments[i] != 0)
					mEdgePointMasses[i].velocity += Info.queuedVelocities[i] / Info.numEngagments[i];
				Info.numEngagments[i] = 0;
				Info.queuedVelocities[i] = Vector2.zero;
			}


			if(otherInfo != null && otherInfo.body != null)
			{

				for(int i = 0; i < otherInfo.largestMTVs.Length; i++)
				{
					Vector2 mtv = otherInfo.largestMTVs[i];
					if(mtv != Vector2.zero)
					{
						otherInfo.body.getEdgePointMass(i).Position += otherInfo.largestMTVs[i];

						//otherInfo.body.getEdgePointMass(i).Position += otherInfo.numEngagments[i] != 0 ? otherInfo.largestMTVs[i] / otherInfo.numEngagments[i] : otherInfo.largestMTVs[i];
						otherInfo.UpdateCollisionInfo(i, otherInfo.body.getEdgePointMass(i).Position, Vector2.zero);
						otherInfo.largestMTVs[i] = Vector2.zero;
					}

					if(otherInfo.numEngagments[i] != 0)
						otherInfo.body.getPointMass(i).velocity += otherInfo.queuedVelocities[i] / otherInfo.numEngagments[i];
					otherInfo.queuedVelocities[i] = Vector2.zero;
					otherInfo.numEngagments[i] = 0;
				}
			}
			else//make work with circles...
			{
				Vector2 mtv = otherInfo.largestMTVs[0];
				if(mtv != Vector2.zero) //for some reason this stops any more triggers from occuring. this could cause issues when a static jello body hits this and another non static jello body hits it in one frame.
				{
					otherInfo.transform.position += (Vector3)mtv;
					otherInfo.UpdateCollisionInfo(0, Vector2.zero, mtv);
					otherInfo.largestMTVs[0] = Vector2.zero;
				}

				//TODO handle with numengagments as well.
//				int num = 0;
//				Vector2 totalVelocity = Vector2.zero;
//				float totalAngularVelocity = 0f;
//				for(int i = 0; i < otherInfo.queuedVelocities.Length; i++)
//				{
//					if(otherInfo.numEngagments[i] != 0)
//					{
//						float invNum = 1 / otherInfo.numEngagments[i];
//
//						otherInfo.queuedVelocities[i] *= invNum;
//						totalVelocity += otherInfo.queuedVelocities[i];
//
//						otherInfo.queuedAngularVelocities[i] *= invNum;
//						totalAngularVelocity += otherInfo.queuedAngularVelocities[i];
//
//						otherInfo.numEngagments[i] = 0;
//						otherInfo.queuedVelocities[i] = Vector2.zero;
//						otherInfo.queuedAngularVelocities[i] = 0f;
//						num++;
//					}
//				}
//
//				if(num != 0)
//				{
//					float invNum = 1 / num;
//					otherInfo.collider2D.rigidbody2D.velocity += totalVelocity * invNum;
//					otherInfo.collider2D.rigidbody2D.angularVelocity += totalAngularVelocity *invNum;
//				}
			}
		}
	}

	#endregion


	#region COLLISION EVENTS
	
	/// <summary>
	/// Collision event handler Delegate.
	/// </summary>
	/// <param name="jelloCollision">The JelloCollision.</param>
	public delegate void CollisionEventHandler(JelloCollision jelloCollision);
	
	/// <summary>
	/// Occurs when a collision occurs.
	/// </summary>
	public event CollisionEventHandler JelloCollisionEvent;
	
	/// <summary>
	/// Raises the collision event.
	/// </summary>
	/// <param name="jelloCollision">The JelloCollision.</param>
	public virtual void OnJelloCollision(JelloCollision jelloCollision)
	{			
		//check if there are any subscribers
		if(JelloCollisionEvent != null)
		{
			//call the event
			JelloCollisionEvent(jelloCollision);
		}
	}
	
	/// <summary>
	/// Determines whether the JelloBody has JelloCollisionEvent subscribers.
	/// </summary>
	public virtual bool HasCollisionEventSubscribers()
	{
		return JelloCollisionEvent != null;
	}
	#endregion



    #region SETTING SHAPE
	
	/// <summary>
	/// Special instructions for what to do when setting the JelloBody.Shape.
	/// </summary>
	[Flags]
	public enum ShapeSettingOptions
	{
		/// <summary>
		/// No special instructions for setting the JelloBody.Shape.
		/// </summary>
		None = 0,
		/// <summary>
		/// Whether to move the JelloPointMass objects into the JelloBody.Shape positions.
		/// </summary>
		MovePointMasses = 1,
		/// <summary>
		/// Whether to clear all edge JelloSpring objects from the JelloSpringBody.
		/// </summary>
		ClearEdgeSprings = 2,
		/// <summary>
		/// Whether to clear and then rebuild the edge JelloSpring objects for the JelloSpringBody.
		/// </summary>
		RebuildEdgeSprings = 4,
		/// <summary>
		/// Whether to clear all internal JelloSpring objects from the JelloSpringBody.
		/// </summary>
		ClearInternalSprings = 8,
		/// <summary>
		/// Whether to clear and then rebuild the internal JelloSpring objects for the JelloSpringBody.
		/// </summary>
		RebuildInternalSprings = 16,
		/// <summary>
		/// Whether to clear all custom JelloSpring objects from the JelloSpringBody.
		/// </summary>
		ClearCustomSprings = 32,
	}

	/// <summary>
	/// Special instructions for what to do when 'smart' setting the JelloBody.Shape.
	/// </summary>
	[Flags]
	public enum SmartShapeSettingOptions
	{
		/// <summary>
		/// No special instruction for smartly setting the JelloBody.Shape
		/// </summary>
		None = 0,
		/// <summary>
		/// Attempt to rebuild any JelloAttachPoint that will be invalidated by the new JelloClosedShape.
		/// With this option deselected, any invalidated JelloAttachPoint will be removed from the JelloBody. 
		/// </summary>
		RebuildInvalidatedAttachPoints = 1,
		/// <summary>
		/// Attempt to rebuild any JelloJoint that will be invalidated by the new JelloClosedShape.
		/// With this option deselected, any invalidated JelloJoint will be removed from the JelloBody. 
		/// </summary>
		RebuildInvalidatedJoints = 2,
		/// <summary>
		/// Attempt to rebuild any custom JelloSpring that will be invalidated by the new JelloClosedShape.
		/// With this option deselected, any invalidated custom JelloSpring will be removed from the JelloBody. 
		/// </summary>
		RebuildInvalidatedCustomSprings = 4,
	}

    /// <summary>
    /// Sets JelloBody.Shape to a new JelloClosedShape object.
	/// Make sure your JelloShape does not contain any duplicate points ( Use JelloShapeTools.RemoveDuplicates() ).
	/// This method will remove any existing JelloPointMass objects, and replace them with new ones if
	/// the new shape has a different JelloClosedShape.VertexCount than the previous one. In this case
    /// the JelloPointMass.Mass for each newly added JelloPointMass will be set to JelloBody.Mass. 
	/// Otherwise the JelloBody.Shape is just updated, not affecting the existing JelloPointMass objects other than thier positions.
	/// Any JelloJoint, JelloSpring, or AttachPoint made invalid by the new shape will be removed.
    /// </summary>
    /// <param name="shape">New JelloClosedShape.</param>
	/// <param name="options">ShapeSettingOptions for setting the JelloClosedShape.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Change the shape of the body to match the shape of the collider collided against
	/// JelloSpringBody springBody;
	/// 
	/// void OnTriggerEnter2D(Collider2D coll)
	/// {
	/// 	if(coll.GetType() == typeof(PolygonCollider2D))
	/// 	{
	/// 		PolygonCollider2D polyColl = (PolygonCollider2D)coll;
	/// 		
	/// 		JelloClosedShape shape = new JelloClosedShape(polyColl.points, true);
	/// 		
	/// 		springBody.setShape(shape, true);
	/// 	}
	/// }
	/// ~~~
	public virtual void setShape(JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None)
    {	//TODO make this all work better...
        mBaseShape = shape;

		bool movePointMasses = (options & ShapeSettingOptions.MovePointMasses) == ShapeSettingOptions.MovePointMasses;

		setComponentReferences();
		
        if (mBaseShape.EdgeVertices.Length != mEdgePointMasses.Length)
        {
			Vector2 pos;
//			JelloPointMass[] oldPointMasses = mPointMasses;
			mEdgePointMasses = new JelloPointMass[mBaseShape.EdgeVertices.Length];

			//how to make this work so that it doesnt move the point masses... what about mesh link options?
			for (int i = 0; i < mBaseShape.EdgeVertices.Length; i++)
			{
//				if(i < oldPointMasses.Length)
//				{
//					mPointMasses[i] = oldPointMasses[i]; //retain as many of the original point masses?
//
//					if(movePointMasses)
//						mPointMasses[i].Position = myTransform.TransformPoint(mBaseShape.EdgeVertices[i]);
//				}
//				else
//				{
					if(movePointMasses)
						pos =  myTransform.TransformPoint(mBaseShape.EdgeVertices[i]);
					else
						pos = Position;

					mEdgePointMasses[i] = new JelloPointMass(Mass, pos, this, false);
//				}
			}
		}
		else if(movePointMasses)
		{
			for(int i = 0; i < EdgePointMassCount; i++)
				mEdgePointMasses[i].Position = myTransform.TransformPoint(mBaseShape.EdgeVertices[i]);
		}

		if (mBaseShape.InternalVertices.Length != mInternalPointMasses.Length)
		{
			Vector2 pos;
//			JelloPointMass[] oldPointMasses = mInternalPointMasses;
			mInternalPointMasses = new JelloPointMass[mBaseShape.InternalVertices.Length];
			
			//how to make this work so that it doesnt move the point masses... what about mesh link options?
			for (int i = 0; i < mBaseShape.InternalVertices.Length; i++)
			{
//				if(i < oldPointMasses.Length)
//				{
//					mInternalPointMasses[i] = oldPointMasses[i]; //retain as many of the original point masses?
//					
//					if(movePointMasses)
//						mInternalPointMasses[i].Position = myTransform.TransformPoint(mBaseShape.InternalVertices[i]);
//				}
//				else
//				{
					if(movePointMasses)
						pos =  myTransform.TransformPoint(mBaseShape.InternalVertices[i]);
					else
						pos = Position;
					
					mInternalPointMasses[i] = new JelloPointMass(Mass, pos, this, false);
//				}
			}
		}
		else if(movePointMasses)
		{
			for(int i = 0; i < mInternalPointMasses.Length; i++)
				mInternalPointMasses[i].Position = myTransform.TransformPoint(mBaseShape.InternalVertices[i]);
		}

		pivotOffset = JelloShapeTools.FindCenter(mBaseShape.EdgeVertices);
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].body = this;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].body = this;

		ClearInvalidSubComponents();
    }


	/// <summary>
	/// Smartly sets JelloBody.Shape to a new JelloClosedShape object.
	/// Make sure your JelloShape does not contain any duplicate points ( Use JelloShapeTools.RemoveDuplicates() ).
	/// This method will attemp to retain as much information as possible from the previous shape.
	/// The subcomponents that will be processed in order to try and retained are JelloPointMass, JelloJoint, JelloAttachPoint, and JelloSpring.
	/// </summary>
	/// <param name="shape">New JelloClosedShape.</param>
	/// <param name="options">ShapeSettingOptions for setting the JelloClosedShape.</param>
	/// <param name="smartOptions">SmartShapeSettingOptions for setting the JelloClosedShape.</param>
	public virtual void smartSetShape(JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{	
		//no need to run smart method if no shape yet assigned.
		if(mBaseShape == null)
		{
			setShape(shape, options);
			return;
		}

		setComponentReferences();

		//find common points between the old shape and the new one.
		List<int[]> indexPairs = new List<int[]>();

		//start with the edges...???
		for(int a = 0;  a < shape.EdgeVertexCount; a++)
		{
			bool found = false;
			for(int i = 0; i < mBaseShape.EdgeVertexCount; i++)
			{
				if(mBaseShape.EdgeVertices[i] == shape.EdgeVertices[a])
				{
					indexPairs.Add (new int[2]{a, i});
					found = true;
					break;
				}
			}

			if(!found)
				indexPairs.Add (new int[2]{a, -1});
		}

		for(int a = 0;  a < shape.InternalVertexCount; a++)
		{
			bool found = false;
			for(int i = 0; i < mBaseShape.InternalVertexCount; i++)
			{
				if(mBaseShape.InternalVertices[i] == shape.InternalVertices[a])
				{
					indexPairs.Add (new int[2]{a, i});
					found = true;
					break;
				}
			}
			
			if(!found)
				indexPairs.Add (new int[2]{a, -1});
		}

		bool movePointMasses = (options & ShapeSettingOptions.MovePointMasses) == ShapeSettingOptions.MovePointMasses;
	
		//reconfigure point masses
		JelloPointMass[] tempPointMasses = new JelloPointMass[shape.EdgeVertexCount];
		for(int i = 0; i < shape.EdgeVertexCount; i++)
		{
			//this is a new point, create a point mass here
			if(indexPairs[i][1] == -1)
			{
				Vector2 pos;
				
				if(movePointMasses)
					pos =  myTransform.TransformPoint(shape.EdgeVertices[indexPairs[i][0]]);
				else
					pos = Position;

				tempPointMasses[i] = new JelloPointMass(Mass, pos, this, false);
			}
			else//this point exists from the old shape, move that point mass into this index.
			{
				tempPointMasses[i] = mEdgePointMasses[indexPairs[i][1]];
			}
		}
		mEdgePointMasses = tempPointMasses;

		tempPointMasses = new JelloPointMass[shape.InternalVertexCount];
		for(int i = 0; i < shape.InternalVertexCount; i++)
		{
			//this is a new point, create a point mass here
			if(indexPairs[i + shape.EdgeVertexCount][1] == -1)
			{
				Vector2 pos;
				
				if(movePointMasses)
					pos =  myTransform.TransformPoint(shape.InternalVertices[indexPairs[i + shape.EdgeVertexCount][0]]);
				else
					pos = Position;
				
				tempPointMasses[i] = new JelloPointMass(Mass, pos, this, false);
			}
			else//this point exists from the old shape, move that point mass into this index.
			{
				tempPointMasses[i] = mInternalPointMasses[indexPairs[i + shape.EdgeVertexCount][1]];
			}
		}
		mInternalPointMasses = tempPointMasses;

		pivotOffset = JelloShapeTools.FindCenter(shape.EdgeVertices);

		//offset index pairs for internal points
		for(int i = 0;  i < shape.InternalVertexCount; i++)
		{
			indexPairs[shape.EdgeVertexCount + i][0] += shape.EdgeVertexCount;
			indexPairs[shape.EdgeVertexCount + i][1] += mBaseShape.EdgeVertexCount;
		}

//		//TODO remove this? it seems a bit redundant...
//		for(int i = 0; i < mEdgePointMasses.Length; i++)
//			mEdgePointMasses[i].body = this;
//		for(int i = 0; i < mInternalPointMasses.Length; i++)
//			mInternalPointMasses[i].body = this;


		processSmartSetShape(indexPairs, shape, options, smartOptions);

		ClearInvalidSubComponents(); //this is joints and attach points.... 

		mBaseShape = shape;
	}

	protected virtual void processSmartSetShape(List<int[]> indexPairs, JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		bool rebuildJoints = (smartOptions & SmartShapeSettingOptions.RebuildInvalidatedJoints) == SmartShapeSettingOptions.RebuildInvalidatedJoints;
		bool rebuildAttachPoints = (smartOptions & SmartShapeSettingOptions.RebuildInvalidatedAttachPoints) == SmartShapeSettingOptions.RebuildInvalidatedAttachPoints;

		if(mJoints != null)
		{

			//work with joints first...
			for(int i = 0; i < mJoints.Length; i++)
			{
				JelloJoint joint = mJoints[i];
				
				if(joint.bodyA == this)
				{
					for(int a = 0; a < joint.affectedIndicesA.Length; a++)
					{
						//Vector2 point = joint .GetAnchorPointA(true);
						bool found = false;
						
						for(int b = 0; b < indexPairs.Count; b++)
						{
							if(joint.affectedIndicesA[a] == indexPairs[b][1])	
							{
								joint.affectedIndicesA[a] = indexPairs[b][0];
								found = true;
								break;
							}
						}
						
						if(!found)
						{
							if(rebuildJoints)
							{
								//rebuild the joint..
								Vector2 pos = mBaseShape.getVertex(joint.affectedIndicesA[a]);
								Vector2[] fullShape = new Vector2[shape.VertexCount];
								for(int c = 0; c < shape.VertexCount; c++)
									fullShape[c] = shape.getVertex(c);
								
								int[] closestIndices = JelloShapeTools.GetClosestIndices(pos, fullShape, joint.affectedIndicesA.Length);
								
								bool assignedClosest = false;
								
								//check if any of the indices are already in use
								for(int c = 0; c < closestIndices.Length; c++)
								{
									//joint.affectedindicesA[a] is the current one... 
									//compare unchecked indices against their old position and compare checked indices against their new position.
									for(int d = 0; d < joint.affectedIndicesA.Length; d++)
									{
										//skip the index that we are currently working with.
										if(d == a)
											continue;
										
										if(d < a)//this index has been updated to the new shape so compare against indexPairs[index][0]
										{
											if(indexPairs[closestIndices[c]][0] == joint.affectedIndicesA[d])//in use
												continue;
										}
										else//d must be greater than a, so so this index is untouched and we should compare against indexPairs[index][1]
										{
											if(indexPairs[closestIndices[c]][1] == joint.affectedIndicesA[d])//in use
												continue;
										}
										
										//made it past the early continues, so the index must not be in use.
										joint.affectedIndicesA[a] = closestIndices[c];
										assignedClosest = true;
										break;
									}
									
									if(assignedClosest)
										break;
								}
								
								//dont forget to rebuild the joint!!!
								Vector2[] affectedVertices = new Vector2[joint.affectedIndicesA.Length];
								for(int q = 0; q < affectedVertices.Length; q++)
									affectedVertices[q] = shape.getVertex (joint.affectedIndicesA[q]);
								
								joint.RebuildAnchor(joint.localAnchorA, true, true, joint.affectedIndicesA, affectedVertices);
							}
							else
							{
								//this joint is invalid and will be removed in the clear invalid subcomponents method.
								mJoints[i] = null;
								break;
							}
						}
					}
				}
				else if(joint.bodyB == this)
				{
					
					for(int a = 0; a < joint.affectedIndicesB.Length; a++)
					{
						bool found = false;
						
						for(int b = 0; b < indexPairs.Count; b++)
						{
							if(joint.affectedIndicesB[a] == indexPairs[b][1])	
							{
								joint.affectedIndicesB[a] = indexPairs[b][0];
								found = true;
								break;
							}
						}
						
						if(!found)
						{
							if(rebuildJoints)
							{
								//rebuild the joint..
								Vector2 pos = mBaseShape.getVertex(joint.affectedIndicesB[a]);
								Vector2[] fullShape = new Vector2[shape.VertexCount];
								for(int c = 0; c < shape.VertexCount; c++)
									fullShape[c] = shape.getVertex(c);
								
								int[] closestIndices = JelloShapeTools.GetClosestIndices(pos, fullShape, joint.affectedIndicesB.Length);
								
								bool assignedClosest = false;
								
								//check if any of the indices are already in use
								for(int c = 0; c < closestIndices.Length; c++)
								{
									//joint.affectedindicesA[a] is the current one... 
									//compare unchecked indices against their old position and compare checked indices against their new position.
									for(int d = 0; d < joint.affectedIndicesB.Length; d++)
									{
										//skip the index that we are currently working with.
										if(d == a)
											continue;
										
										if(d < a)//this index has been updated to the new shape so compare against indexPairs[index][0]
										{
											if(indexPairs[closestIndices[c]][0] == joint.affectedIndicesB[d])//in use
												continue;
										}
										else//d must be greater than a, so so this index is untouched and we should compare against indexPairs[index][1]
										{
											if(indexPairs[closestIndices[c]][1] == joint.affectedIndicesB[d])//in use
												continue;
										}
										
										//made it past the early continues, so the index must not be in use.
										joint.affectedIndicesB[a] = closestIndices[c];
										assignedClosest = true;
										break;
									}
									
									if(assignedClosest)
										break;
								}
								
								//dont forget to rebuild the joint!!!
								Vector2[] affectedVertices = new Vector2[joint.affectedIndicesB.Length];
								for(int q = 0; q < affectedVertices.Length; q++)
									affectedVertices[q] = shape.getVertex(joint.affectedIndicesB[q]);
								
								joint.RebuildAnchor(joint.localAnchorB, false, true, joint.affectedIndicesB, affectedVertices);
							}
							else
							{
								//this joint is invalid and will be removed in the clear invalid subcomponents method.
								mJoints[i] = null;
								break;
							}
						}
					}
				}
			}
		}

		if(mAttachPoints != null)
		{

			//handle attach points now
			for(int i = 0; i < mAttachPoints.Length; i++)
			{
				JelloAttachPoint attachPoint = mAttachPoints[i];
				
				for(int a = 0; a < attachPoint.affectedIndices.Length; a++)
				{
					bool found = false;
					for(int b = 0; b < indexPairs.Count; b++)
					{
						if(attachPoint.affectedIndices[a] == indexPairs[b][1])
						{
							attachPoint.affectedIndices[a] = indexPairs[b][0];
							found = true;
							break;
						}
					}
					
					if(!found)
					{
						if(rebuildAttachPoints)
						{
							//rebuild the attach point..
							Vector2 pos = mBaseShape.getVertex(attachPoint.affectedIndices[a]);
							Vector2[] fullShape = new Vector2[shape.VertexCount];
							for(int c = 0; c < shape.VertexCount; c++)
								fullShape[c] = shape.getVertex(c);
							
							int[] closestIndices = JelloShapeTools.GetClosestIndices(pos, fullShape, attachPoint.affectedIndices.Length);
							
							bool assignedClosest = false;
							
							//check if any of the indices are already in use
							for(int c = 0; c < closestIndices.Length; c++)
							{
								//joint.affectedindicesA[a] is the current one... 
								//compare unchecked indices against their old position and compare checked indices against their new position.
								for(int d = 0; d < attachPoint.affectedIndices.Length; d++)
								{
									//skip the index that we are currently working with.
									if(d == a)
										continue;
									
									if(d < a)//this index has been updated to the new shape so compare against indexPairs[index][0]
									{
										if(indexPairs[closestIndices[c]][0] == attachPoint.affectedIndices[d])//in use
											continue;
									}
									else//d must be greater than a, so so this index is untouched and we should compare against indexPairs[index][1]
									{
										if(indexPairs[closestIndices[c]][1] == attachPoint.affectedIndices[d])//in use
											continue;
									}
									
									//made it past the early continues, so the index must not be in use.
									attachPoint.affectedIndices[a] = closestIndices[c];
									assignedClosest = true;
									break;
								}
								
								if(assignedClosest)
									break;
							}
							
							//all legs have been updated, not rebuild the attach point.
							attachPoint.Rebuild(attachPoint.point, this, attachPoint.affectedIndices, true);
						}
						else
						{
							//this joint is invalid and will be removed in the clear invalid subcomponents method.
							mAttachPoints[i] = null;
							break;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Moves JelloPointMass objects if set to and uptates the JelloBody.pivotOffset.
	/// Used by the Unity editor.
	/// </summary>
	/// <param name="movePointMasses">Whether to move the JelloPointMass.Position or each JelloPointMass into the positions of the JelloBody.Shape.</param>
	public void updateGlobalShape(bool movePointMasses = false) //TODO how does this work with internal points? do i still even need this?
	{	
		if(movePointMasses)
			for(int i = 0; i < mEdgePointMasses.Length; i++)
				mEdgePointMasses[i].Position = myTransform.TransformPoint(mBaseShape.EdgeVertices[i]);
		
		if(movePointMasses)
			for(int i = 0; i < mInternalPointMasses.Length; i++)
				mInternalPointMasses[i].Position = myTransform.TransformPoint(mBaseShape.InternalVertices[i]);
		
		pivotOffset = JelloShapeTools.FindCenter(mBaseShape.EdgeVertices);
	}

    #endregion

    #region SETTING MASS
    
	/// <summary>
    /// Set JelloBody.Mass and JelloPointMass.Mass for each JelloPointMass in this JelloBody.
    /// </summary>
    /// <param name="mass">New mass.</param>
    public void setMassAll(float mass)
    {
        for (int i = 0; i < mEdgePointMasses.Length; i++)
            mEdgePointMasses[i].Mass = mass;

		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].Mass = mass;

       Mass = mass;
    }

    /// <summary>
    /// Set JelloPointMass.Mass of the JelloPointMass at the given index.
    /// </summary>
    /// <param name="index">Index of the JelloPointMass.</param>
    /// <param name="mass">New mass.</param>
    public void setMassAtIndex( int index, float mass)
    {
		if(index >= 0)
		{
			if(index < mEdgePointMasses.Length)
				mEdgePointMasses[index].Mass = mass;
			else if(index < mEdgePointMasses.Length + mInternalPointMasses.Length)
				mInternalPointMasses[index].Mass = mass;
		}
    }

    /// <summary>
    /// Set the JelloPointMass.Mass for every JelloPointMass belonging to this JelloBody.
    /// </summary>
    /// <param name="masses">Array of masses (length MUST equal PointMasses.Count)</param>
    public void setMassFromArray(float[] masses)
    {
		if(masses.Length == PointMassCount)
		{
			for(int i = 0; i < PointMassCount; i++)
			{
				getPointMass(i).Mass = masses[i];
			}
		}
		else
		{
			Debug.LogWarning("masses array not the same length as PointMassCount, masses not assigned")	;
		}
    }
    
	#endregion

    #region DERIVING POSITION AND VELOCITY

    /// <summary>
    /// Derive the global JelloBody.Position and JelloBody.Angle of this body, based on the average of the JelloPointMass.Position of each JelloPointMass.
    /// This updates the JelloBody.Posision, JelloBody.Angle, JelloBody.velocity, and JelloBody.angular velocity properties.
    /// This is called by JelloWorld each Update() so usually a user does not need to call this.  
	/// Instead you can just access the JelloBody.Position, JelloBody.Angle, JelloBody.velocity, and JelloBody.angularVelocity properties.
	/// Note that if either JelloBody.IsStatic or JelloBody.IsKinematic are true, this method is exited immediately.
    /// </summary>
    /// <param name="deltaTime">The amount of time elapsed this iteration.</param>
    public void derivePositionAndAngle(float deltaTime)
    {
        // no need it this is a static body, or kinematically controlled.
        if (mIsStatic || mIsKinematic)
            return;

		pointMassPositions = new Vector2[mEdgePointMasses.Length];//TODO should this include internal point masses as well????
		for(int i =0; i < mEdgePointMasses.Length; i++)
			pointMassPositions[i] = mEdgePointMasses[i].Position;

		Vector2 center = JelloShapeTools.FindCenter(pointMassPositions) - JelloVectorTools.rotateVector( new Vector2(pivotOffset.x * Scale.x, pivotOffset.y * Scale.y), Angle);

		velocity = (center - Position) / deltaTime;

		Position = center;

		if(GetComponent<Rigidbody2D>().fixedAngle)
			return;

		// find the average angle of all of the masses.
		float angle = 0;
		int originalSign = 1;
		float originalAngle = 0;
		float thisAngle;
		float angleChange;
		Vector2 baseNorm;
		Vector2 curNorm;
		float dot;
		int thisSign;
		
		for (int i = 0; i < mEdgePointMasses.Length; i++)
		{
			baseNorm = mBaseShape.EdgeVertices[i].normalized;
			curNorm = (mEdgePointMasses[i].Position - Position).normalized;
			
			dot = Vector2.Dot(baseNorm, curNorm);
			if (dot > 1.0f) { dot = 1.0f; }
			if (dot < -1.0f) { dot = -1.0f; }
			
			thisAngle = Mathf.Acos(dot); //always positive, but you dont know which direction the angle is...
			
			//check here which direction the rotation faces...
			if (!JelloVectorTools.isCCW(ref baseNorm, ref curNorm)) { thisAngle = -thisAngle; }
			
			if (i == 0)
			{
				originalSign = (thisAngle >= 0.0f) ? 1 : -1;
				originalAngle = thisAngle;
			}
			else
			{
				thisSign = (thisAngle >= 0.0f) ? 1 : -1;
				
				if ((Mathf.Abs(thisAngle - originalAngle) > Mathf.PI) && (thisSign != originalSign))
				{
					thisAngle = (thisSign == -1) ? (Mathf.PI + Mathf.PI + thisAngle) :  -(Mathf.PI + Mathf.PI - thisAngle);
				}
			}
			
			angle += thisAngle;
		}

		angle /= mEdgePointMasses.Length;
		Angle = angle * Mathf.Rad2Deg;
		
		// now calculate the derived Omega, based on change in angle over time.
		angleChange = (angle - mLastAngle);
		if (Mathf.Abs(angleChange) >= 180f)
		{
			if (angleChange < 0f)
				angleChange += 360f;
			else
				angleChange -=  360f;
		}
		
		angularVelocity = angleChange * Mathf.Deg2Rad / deltaTime;
    }

	/// <summary>
	/// Gets the velocity of a point on the edge at the given index at a, given distance along the edge.
	/// </summary>
	/// <param name="index">The edge index.</param>
	/// <param name="scalarAB">The normalized distance of point along the edge [0,1].</param>
	/// <returns>The velocity of a point along the edge.</returns>
	public Vector2 getPointOnEdgeVelocity(int index, float scalarAB)
	{
		if(mIsStatic)
			return Vector2.zero;
		
		if(index < mEdgePointMasses.Length)
		{
			return mEdgePointMasses[index].velocity * (1.0f - scalarAB) + mEdgePointMasses[index + 1 < EdgePointMassCount ? index + 1 : 0].velocity * scalarAB;
		}
		else
		{
			return Vector2.zero;
		}
	}

    #endregion

    #region ACCUMULATING FORCES - TO BE INHERITED!
    
	/// <summary>
    /// This method adds all internal forces to JelloPointMass.force of each JelloPointMass in the JelloBody.
    /// These should be forces that try to modify/maintain the shape of the JelloBody.
	/// </summary>
	/// <param name="deltaTime">The amount of time elapsed this iteration.</param>
    public virtual void accumulateInternalForces(float deltaTime) 
	{
		if(IsStatic)
			return;

		//angular and linear damping
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].force -= GetComponent<Rigidbody2D>().drag * mEdgePointMasses[i].velocity;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].force -= GetComponent<Rigidbody2D>().drag * mEdgePointMasses[i].velocity;
	}

    /// <summary>
	/// This method adds all external forces to JelloPointMass.force of each JelloPointMass in the JelloBody.
    /// These forces are gravity, damping, etc.
    /// </summary>
    public virtual void accumulateExternalForces()
	{
		if(IsStatic)
			return;

		//gravity
		if(affectedByGravity && !mIsKinematic)
		{
			for(int i = 0; i < mEdgePointMasses.Length; i++)
				mEdgePointMasses[i].externalForce += (overrideWorldGravity ? gravity : Physics2D.gravity) * mEdgePointMasses[i].Mass;
			for(int i = 0; i < mInternalPointMasses.Length; i++)
				mInternalPointMasses[i].externalForce += (overrideWorldGravity ? gravity : Physics2D.gravity) * mInternalPointMasses[i].Mass;
		}
	}
	
    #endregion

    #region INTEGRATION
	
	/// <summary>
	/// Integrate the body over the elapsed time.
	/// Applies JelloPointMass.force to JelloPointMass.velocity and JelloPointMass.velocity to JelloPointMass.Position.
	/// This method determines JelloBody.velocity and JelloBody.angularVelocity when JelloBody.IsKinematic is set to true.
	/// This method is called regularly by the simulation and is called JelloWorld.Iterations times every FixedUpdate();
	/// </summary>
	/// <param name="deltaTime">The elapsed time.</param>
    public virtual void integrate(float deltaTime)
    {
       if (IsStatic) 
			return;
		
		//TODO have two kinematic modes
		if(IsKinematic)
		{
			velocity = (Position - prevPosition) / deltaTime;
			
			if(!GetComponent<Rigidbody2D>().fixedAngle)
				angularVelocity = ((Angle - mLastAngle) * Mathf.Deg2Rad) / deltaTime;	
		}
			
		for (int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].integrate(deltaTime);
		for (int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].integrate(deltaTime);

		
		prevPosition = Position;
		mLastAngle = Angle;
    }
	
	/// <summary>
	/// Clears all forces from the JelloPointMass objects.
	/// Called each solver iteration by the simulation.
	/// </summary>
	public virtual void ClearForces()
	{
		if(IsStatic)
			return;
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].force = Vector2.zero;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].force = Vector2.zero;
	}

	/// <summary>
	/// Clears the external forces.
	/// Called each FixedUpdate by the simulation.
	/// </summary>
	public virtual void ClearExternalForces()
	{
		if(IsStatic)
			return;
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].externalForce = Vector2.zero;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].externalForce = Vector2.zero;
	}

	/// <summary>
	/// Clears the persistant forces.
	/// Never called by the simulation, only by the user.
	/// </summary>
	public virtual void ClearPersistantForces()
	{
		if(IsStatic)
			return;
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].persistantForce = Vector2.zero;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].persistantForce = Vector2.zero;
	}

    #endregion
	
	#region ADDING FORCE/VELOCITY/IMPULSE/TORQUE
	
	/// <summary>
	/// Adds linear velocity to the JelloPointMass objects belonging to this JelloBody.
	/// </summary>
	/// <param name="velocity">The velocity to add.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Make the body jump
	/// JelloBody body;
	/// 
	/// void Update()
	/// {
	/// 	if(Input.GetKeyDown("space"))
	/// 	{
	/// 		body.AddLinearVelocity(Vector2.up * 2f);
	/// 	}
	/// }
	/// ~~~
	public void AddLinearVelocity(Vector2 velocity)
	{
		if(IsStatic)
			return;
		
		mIsAwake = true;
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)		
			mEdgePointMasses[i].velocity += velocity;
		for(int i = 0; i < mInternalPointMasses.Length; i++)		
			mInternalPointMasses[i].velocity += velocity;
	}
	
	/// <summary>
	/// Sets linear velocity of the JelloPointMass objects belonging to this JelloBody.
	/// </summary>
	/// <param name="velocity">The new velocity.</param>
	public void SetLinearVelocity(Vector2 velocity)
	{
		if(IsStatic)
			return;

		mIsAwake = true;
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].velocity = velocity;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].velocity = velocity;
	}
	
	/// <summary>
	/// Adds torque to the JelloBody.
	/// </summary>
	/// <param name="torque">The amount of torque to add.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Make the body roll
	/// JelloBody body;
	/// 
	/// void Update()
	/// {
	/// 	if(Input.GetKeyDown("space"))
	/// 	{
	/// 		body.AddTorque(10f);
	/// 	}
	/// }
	/// ~~~
	public void AddTorque(float torque)
	{
		if(IsStatic)
			return;
		
		mIsAwake = true;
		
		Vector3 rx;
		for(int i = 0; i < mEdgePointMasses.Length; i++)
		{
			rx = mEdgePointMasses[i].Position - Position;
			mEdgePointMasses[i].externalForce += (Vector2)(Vector3.Cross(new Vector3(0f, 0f, torque), rx) / Vector3.Dot(rx,rx) * rx.magnitude);
		}
		for(int i = 0; i < mInternalPointMasses.Length; i++)
		{
			rx = mInternalPointMasses[i].Position - Position;
			mInternalPointMasses[i].externalForce += (Vector2)(Vector3.Cross(new Vector3(0f, 0f, torque), rx) / Vector3.Dot(rx,rx) * rx.magnitude);
		}
	}

	/// <summary>
	/// Add force to the JelloBody (at center of mass, not creating any torque).
	/// </summary>
	/// <param name="force">The amount of force to add.</param>
	/// <param name="forceIsRelative">Whether the force is relative to the JelloBody (in local coordinates and affected by scale).</param>
	public virtual void AddForce(Vector2 force, bool forceIsRelative = false)
	{
		if(IsStatic)
			return;
		
		mIsAwake = true;
		
		if(forceIsRelative)
		{
			force = new Vector2(force.x * Scale.x, force.y * Scale.y);
			JelloVectorTools.rotateVector(ref force, Angle);
		}
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].externalForce += force;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].externalForce += force;
	}

	/// <summary>
	/// Add persistant force to the JelloBody (at center of mass, not creating any torque).
	/// Persistant forces are never cleared by the simulation and must be removed by the user via JelloBody.clearPersistantForces.
	/// </summary>
	/// <param name="force">The amount of force to add.</param>
	/// <param name="forceIsRelative">Whether the force is relative to the JelloBody (in local coordinates and affected by scale).</param>
	public virtual void AddPersistantForce(Vector2 force, bool forceIsRelative = false)
	{
		if(IsStatic)
			return;
		
		mIsAwake = true;
		
		if(forceIsRelative)
		{
			force = new Vector2(force.x * Scale.x, force.y * Scale.y);
			JelloVectorTools.rotateVector(ref force, Angle);
		}
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].persistantForce += force;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].persistantForce += force;
	}
	
	/// <summary>
	/// Add force at a specific point on the JelloBody.
	/// </summary>
	/// <param name="force">The Amount of force to be added.</param>
	/// <param name="point">The point at which to add the force at.</param>
	/// <param name="pointIsRelative">Whether the point is relative to the JelloBody (in local coordinates and affected by scale).</param>
	/// <param name="forceIsRelative">Whether the force is relative to the JelloBody (in local coordinates and affected by scale).</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //push the body towards the top of itself at the bottom like a rocket.
	/// JelloBody body;
	/// 
	/// void Update()
	/// {
	/// 	if(Input.GetKeyDown("space"))
	/// 	{
	/// 		body.AddForce(Vector2.up * 2f, -Vector2.up, true, true);
	/// 	}
	/// }
	/// ~~~
	public virtual void AddForce(Vector2 force, Vector2 point, bool pointIsRelative = false, bool forceIsRelative = false) //TODO come back to this and think how to handle for internal point masses
	{
		if(IsStatic)
			return;
		
		mIsAwake = true;
		
//		Vector3 rx;
//		float ratio;
//		for(int a = 0; a < PointMassCount; a++)
//		{
//			rx = mPointMasses[a].Position - Position;
//			ratio = ra == Vector2.zero ? 1f : rx.magnitude / ra.magnitude;
//			mPointMasses[a].force += force + (Vector2)(Vector3.Cross(Vector3.Cross(ra, force), rx) / Vector3.Dot(rx,rx) * ratio);// + ratio*rx);
//		}
		if(pointIsRelative)
		{
			// scale,rotate, and translate the point into global space.
			point = new Vector2(point.x * Scale.x, point.y * Scale.y);
			JelloVectorTools.rotateVector(ref point, Angle);
			point += Position;
		}
		
		if(forceIsRelative)
		{
			force = new Vector2(force.x * Scale.x, force.y * Scale.y);
			JelloVectorTools.rotateVector(ref force, Angle);
		}
		
		Vector2 hitPoint = Vector2.zero;
		float scalarAB = 0f;
		float scalarCD = 0f;
		
		bool hit = false;
		for(int i = 0; i < mEdgePointMasses.Length; i++)
		{
			if(JelloVectorTools.RayIntersectsSegment(point, force, mEdgePointMasses[i].Position, mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].Position, out hitPoint, out scalarAB, out scalarCD))
			{
				hit = true;

				mEdgePointMasses[i].externalForce += force * (scalarAB);
				mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].externalForce += force * (1f - scalarAB);

				break;
			}
		}
		
		if(!hit)
		{
			int pointA, pointB = 0;
			hitPoint = getClosestEdgePoint(point, out pointA, out pointB, out scalarAB);
		
			mEdgePointMasses[pointA].externalForce += force * (1.0f - scalarAB);
			mEdgePointMasses[pointB].externalForce += force * scalarAB;
		}
	}

	/// <summary>
	/// Adds force to at point on along the edge at the given index.
	/// </summary>
	/// <param name="force">The amount of force to add.</param>
	/// <param name="index">The edge Index.</param>
	/// <param name="scalarAB">The normalized distance of the point along the edge [0,1]</param>
	public virtual void AddForceToPointOnEdge(Vector2 force, int index, float scalarAB)
	{
		if(mIsStatic)
			return;
		
		if(index >= 0 && index < mEdgePointMasses.Length)
		{
			mEdgePointMasses[index].externalForce += force * (1.0f - scalarAB);
			mEdgePointMasses[index + 1 < mEdgePointMasses.Length ? index + 1 : 0].externalForce += force * scalarAB;
		}
	}

	/// <summary>
	/// Adds persistant force to at point on along the edge at the given index.
	/// Persistant forces are never cleared by the simulation and must be removed by the user via JelloBody.clearPersistantForces.
	/// </summary>
	/// <param name="force">The amount of force to add.</param>
	/// <param name="index">The edge Index.</param>
	/// <param name="scalarAB">The normalized distance of the point along the edge [0,1]</param>
	public virtual void AddPersistantForceToPointOnEdge(Vector2 force, int index, float scalarAB)
	{
		if(mIsStatic)
			return;
		
		if(index >= 0 && index < mEdgePointMasses.Length)
		{
			mEdgePointMasses[index].persistantForce += force * (1.0f - scalarAB);
			mEdgePointMasses[index + 1 < mEdgePointMasses.Length ? index + 1 : 0].persistantForce += force * scalarAB;
		}
	}

	/// <summary>
	/// Adds an impulse to the JelloBody at center of mass.
	/// </summary>
	/// <param name="impulse">The impulse to add.</param>
	/// <param name="impulseIsRelative">Whether or not the impulse is in local space.</param>
	public virtual void AddImpulse(Vector2 impulse, bool impulseIsRelative = false)
	{
		if(mIsStatic)
			return;
		
		mIsAwake = true;
		
		if(impulseIsRelative)
		{
			impulse = new Vector2(impulse.x * Scale.x, impulse.y * Scale.y);
			JelloVectorTools.rotateVector(ref impulse, Angle);
		}

		for(int i = 0; i < mEdgePointMasses.Length; i++)
			mEdgePointMasses[i].velocity += impulse;
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			mInternalPointMasses[i].velocity += impulse;
	}
	
	/// <summary>
	/// Adds an impulse to the JelloBody at a specified point.
	/// </summary>
	/// <param name="impulse">The impulse to add.</param>
	/// <param name="point">The point at which to apply the impulse.</param>
	/// <param name="pointIsRelative">Whether the point is in local space.</param>
	/// <param name="impulseIsRelative">Whether the impulse is in local space.</param>
	public virtual void AddImpulse(Vector2 impulse, Vector2 point, bool pointIsRelative = false, bool impulseIsRelative = false) //TODO think of how to make this work correctly with internal points as well
	{
		if(mIsStatic)
			return;
		
		mIsAwake = true;

		if(pointIsRelative)
		{
			// scale,rotate, and translate the point into global space.
			point = new Vector2(point.x * Scale.x, point.y * Scale.y);
			JelloVectorTools.rotateVector(ref point, Angle);
			point += Position;
		}
		
		if(impulseIsRelative)
		{
			impulse = new Vector2(impulse.x * Scale.x, impulse.y * Scale.y);
			JelloVectorTools.rotateVector(ref impulse, Angle);
		}
		
		Vector2 hitPoint = Vector2.zero;
		float scalarAB = 0f;
		float scalarCD = 0f;
		Vector2 ra = point - Position;
		bool hit = false;
		float j = 0f;
		float InverseMomentOfInertia = 1f;
		
		for(int i = 0; i < EdgePointMassCount; i++)
		{
			if(JelloVectorTools.RayIntersectsSegment(point, impulse, mEdgePointMasses[i].Position, mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].Position, out hitPoint, out scalarAB, out scalarCD))
			{
				hit = true;
					
				j = 1 / (mEdgePointMasses[i].InverseMass + mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].InverseMass) * 0.5f + 
					(ra.x * impulse.y - ra.y * impulse.x) * (ra.x * impulse.y - ra.y * impulse.x) * InverseMomentOfInertia;
				
				mEdgePointMasses[i].velocity += j * impulse * (scalarAB) * mEdgePointMasses[i].InverseMass;
				mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].velocity += j * impulse * (1f - scalarAB) * mEdgePointMasses[i + 1 < EdgePointMassCount ? i + 1 : 0].InverseMass;
				
				break;
			}
		}
		if(!hit)
		{
			int pointA, pointB = 0;
			
			hitPoint = getClosestEdgePoint(point, out pointA, out pointB, out scalarAB);
				
			j = 1 /(mEdgePointMasses[pointA].InverseMass + mEdgePointMasses[pointB].InverseMass) * 0.5f +
				(ra.x * impulse.y - ra.y * impulse.x) * (ra.x * impulse.y - ra.y * impulse.x) * (InverseMomentOfInertia);
			
			mEdgePointMasses[pointA].velocity += j * impulse * (1.0f - scalarAB) * mEdgePointMasses[pointA].InverseMass;
			mEdgePointMasses[pointB].velocity += j * impulse * scalarAB * mEdgePointMasses[pointB].InverseMass;
		}
	}
	
	#endregion
	
	#region FINDING POINTS ON BODY

    /// <summary>
	/// Find the point, along with information about the edge it resides on, on this JelloBody closest to a given global point. 
    /// </summary>
    /// <param name="point">The global point to test against.</param>
    /// <param name="hitPoint">The returned point on the JelloBody closest to the given global point.</param>
	/// <param name="normal">The returned normal of the edge that the hitPoint resides on.</param>
    /// <param name="pointA">The returned start index of the edge that the hitPoint resides on.</param>
    /// <param name="pointB">The returned end index of the edge that the hitPoint resides on.</param>
    /// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge that the hitPoint resides on [0,1].</param>
    /// <returns>The distance from the given global point to the hitPoint found on the JelloBody.</returns>
    public float getClosestEdgePoint( Vector2 point, out Vector2 hitPoint, out Vector2 normal, out int pointA, out int pointB, out float scalarAB)
    {
        hitPoint = Vector2.zero;
        pointA = -1;
        pointB = -1;
        scalarAB = 0f;
        normal = Vector2.zero;

        float closest = Mathf.Infinity;
		Vector2 tempHit = Vector2.zero;
		Vector2 tempNormal = Vector2.zero;
		float tempScalar = 0f;
		float dist = 0;
        
		for (int i = 0; i < mEdgePointMasses.Length; i++)
        {
            dist = getClosestPointOnEdge(point, i, out tempHit, out tempNormal, out tempScalar);
            
			if (dist < closest)
            {
                closest = dist;
                pointA = i;
				pointB = i < mEdgePointMasses.Length - 1 ? i + 1 : 0;
                scalarAB = tempScalar;
                normal = tempNormal;
                hitPoint = tempHit;
            }
        }

        return closest;
    }
	
    /// <summary>
	/// Find the point, along with information about the edge it resides on, on this JelloBody closest to a given global point.
    /// </summary>
    /// <param name="point">The global point to text against.</param>
    /// <param name="pointA">The returned start index of the edge the hitPoint resides on.</param>
    /// <param name="pointB">The returned end index of the edge the hitPoint resides on.</param>
	/// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge that the hitPoint resides on [0,1].</param>
    /// <returns>The point on JelloBody closest to the given global point.</returns>
    public Vector2 getClosestEdgePoint(Vector2 point, out int pointA, out int pointB, out float scalarAB)
    {
        Vector2 hitPoint = Vector2.zero;
        pointA = -1;
        pointB = -1;
        scalarAB = 0f;

        float closest = Mathf.Infinity;
		Vector2 tempHit = Vector2.zero;
		float tempScalar = 0f;
		float dist = 0;
        
		for (int i = 0; i < mEdgePointMasses.Length; i++)
        {
			dist = getClosestPointOnEdgeSquared(point, i, out tempHit, out tempScalar);
			
			if (dist < closest)
            {
                closest = dist;
                pointA = i;
				pointB = i < mEdgePointMasses.Length - 1 ? i + 1 : 0;
                scalarAB = tempScalar;
                hitPoint = tempHit;
            }
        }
		
        return hitPoint;
    }
	
    /// <summary>
	/// Find the point, along with information about the edge it resides on, on a specified edge of the JelloBody closest to a given global point.
    /// </summary>
    /// <param name="point">The global point to test against.</param>
	/// <param name="edgeIndex">The index of the edge to check against. For example, 0 = edge from JelloBody.GetPointMass(0).Position to JelloBody.GetPointMass(1).Position.</param>
    /// <param name="hitPoint">The returned point on edge.</param>
    /// <param name="normal">The returned normal of the edge in global space.</param>
	/// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge [0,1].</param>
    /// <returns>The distance from the global point to the hitPoint.</returns>
    public float getClosestPointOnEdge(Vector2 point, int edgeIndex, out Vector2 hitPoint, out Vector2 normal, out float scalarAB)
    {
		return JelloVectorTools.getClosestPointOnSegment
			(
				point, 
				mEdgePointMasses[edgeIndex].Position, 
				mEdgePointMasses[edgeIndex + 1 < mEdgePointMasses.Length ? edgeIndex + 1 : 0].Position, 
				out hitPoint, 
				out normal, 
				out scalarAB
			);
    }
	
	/// <summary>
	/// Find the point, along with information about the edge it resides on, on a specified edge of the JelloBody closest to a given global point.
    /// </summary>
    /// <param name="point">The global point to test against.</param>
	/// <param name="edgeIndex">The index of the edge to check against. For example, 0 = edge from JelloBody.GetPointMass(0).Position to JelloBody.GetPointMass(1).Position.</param>
	/// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge [0,1].</param>
	/// <returns>The closest point on the given edge.</returns>
	public Vector2 getClosestPointOnEge(Vector2 point, int edgeIndex, out float scalarAB)
	{
		return JelloVectorTools.getClosestPointOnSegment
			(
				point,
				mEdgePointMasses[edgeIndex].Position, 
				mEdgePointMasses[edgeIndex + 1 < mEdgePointMasses.Length ? edgeIndex + 1 : 0].Position,
				out scalarAB
			);
	}

    /// <summary>
	/// Find the point, along with information about the edge it resides on, on a specified edge of the JelloBody closest to a given global point.
    /// </summary>
    /// <param name="point">The global point to test against.</param>
	/// <param name="edgeIndex">The index of the edge to check against. For example, 0 = edge from JelloBody.GetPointMass(0).Position to JelloBody.GetPointMass(1).Position.</param>
	/// <param name="hitPoint">The returned point on edge.</param>
	/// <param name="normal">The returned normal of the edge in global space.</param>
	/// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge [0,1].</param>
	/// <returns>The squared distance from the global point to the hitPoint.</returns>
    public float getClosestPointOnEdgeSquared(Vector2 point, int edgeIndex, out Vector2 hitPoint, out Vector2 normal, out float scalarAB)
    {
		return JelloVectorTools.getClosestPointOnSegmentSquared
			(
				point,
				mEdgePointMasses[edgeIndex].Position, 
				mEdgePointMasses[edgeIndex + 1 < mEdgePointMasses.Length ? edgeIndex + 1 : 0].Position,
				out hitPoint,
				out normal,
				out scalarAB
			);
    }
	
    /// <summary>
	/// Find the point, along with information about the edge it resides on, on a specified edge of the JelloBody closest to a given global point.
    /// </summary>
	/// <param name="point">The global point to test against.</param>
	/// <param name="edgeIndex">The index of the edge to check against. For example, 0 = edge from JelloBody.GetPointMass(0).Position to JelloBody.GetPointMass(1).Position.</param>
	/// <param name="hitPoint">The returned normal of the edge in global space.</param>
	/// <param name="scalarAB">The normalized distance (of the hitPoint) along the edge [0,1].</param>
	/// <returns>The squared distance from the global point to the hitPoint.</returns>
    public float getClosestPointOnEdgeSquared(Vector2 point, int edgeIndex, out Vector2 hitPoint, out float scalarAB)
    {
		return JelloVectorTools.getClosestPointOnSegmentSquared
			(
				point,
				mEdgePointMasses[edgeIndex].Position, 
				mEdgePointMasses[edgeIndex + 1 < mEdgePointMasses.Length ? edgeIndex + 1 : 0].Position,
				out hitPoint,
				out scalarAB
			);
    }

    /// <summary>
    /// Find the closest JelloPointMass in this JelloBody to a given global point.
    /// </summary>
    /// <param name="point">The global point to test against.</param>
    /// <param name="distance">The returned distance from the JelloPointMass to the given global point.</param>
    /// <returns>The index of the closest JelloPointMass.</returns>
    public int getClosestPointMass(Vector2 point, out float distance)
    {
        //float closestSQD = Mathf.Infinity;
		distance = Mathf.Infinity;
        int index = -1;
		float dist;
		
        for (int i = 0; i < mEdgePointMasses.Length; i++)
        {
			dist = (point - mEdgePointMasses[i].Position).sqrMagnitude;
            if (dist < distance)
            {
                distance = dist;
                index = i;
            }
        }
		for(int i = 0; i < mInternalPointMasses.Length; i++)
		{
			dist = (point - mInternalPointMasses[i].Position).sqrMagnitude;
			if (dist < distance)
			{
				distance = dist;
				index = i + mEdgePointMasses.Length;
			}
		}

        distance = Mathf.Sqrt(distance);
        return index;
    }
	
	/// <summary>
	/// Find the closest JelloPointMass in this JelloBody to a given global point.
    /// </summary>
	/// <param name="point">The global point to test against.</param>
	/// <param name="includeInternal">Whether to include internal JelloPointMass objects in the search.</param>
    /// <returns>The index of the closest JelloPointMass.</returns>
	public int getClosestPointMass(Vector2 point, bool includeInternal = true)
    {
        float distance = Mathf.Infinity;
        int index = -1;
		float dist;
		
        for (int i = 0; i < mEdgePointMasses.Length; i++)
        {
			dist = (point - mEdgePointMasses[i].Position).sqrMagnitude;
            if (dist < distance)
            {
                distance = dist;
                index = i;
            }
        }
		if(includeInternal)
		{
			for(int i = 0; i < mInternalPointMasses.Length; i++)
			{
				dist = (point - mInternalPointMasses[i].Position).sqrMagnitude;
				if (dist < distance)
				{
					distance = dist;
					index = i + mEdgePointMasses.Length;
				}
			}
		}
		
        return index;
    }

	/// <summary>
	/// Gets the closest JelloPointMass objects to a given global point.
	/// </summary>
	/// <param name="point">The global point to test against.</param>
	/// <param name="returnNum">The number of JelloPointMass objects to find.</param>
	/// <param name="includeInternal">Whether to include internal JelloPointMass objects in the search.</param>
	/// <returns>The indices of the closest JelloPointMass objects.</returns>
	public int[] getClosestPointMasses(Vector2 point, int returnNum, bool includeInternal = true)
	{
		returnNum = Mathf.Min( returnNum, mEdgePointMasses.Length + (includeInternal ? mInternalPointMasses.Length : 0));

		int[] indices = new int[returnNum];
		float[] distances = new float[returnNum];
		for(int i = 0; i < returnNum; i++)
			distances[i] = Mathf.Infinity;
		//int index = -1;
		float dist;

		for (int i = 0; i < mEdgePointMasses.Length; i++)
		{
			dist = (point - mEdgePointMasses[i].Position).sqrMagnitude;
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
		if(includeInternal)
		{
			for(int i = 0; i < mInternalPointMasses.Length; i++)
			{
				dist = (point - mInternalPointMasses[i].Position).sqrMagnitude;

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
						indices[a] = i + mEdgePointMasses.Length;
						break;
					}
				}



//				if (dist < distance)
//				{
//					distance = dist;
//					index = i + mEdgePointMasses.Length;
//				}
			}
		}
		
		return indices;
	}

	#endregion

	#region ATTACH POINTS

	/// <summary>
	/// Update the JelloAttachPoint objects assosiated with this JelloBody.
	/// This is called regularly by JelloWorld.
	/// </summary>
	public void UpdateAttachPoints()
	{
		if(mAttachPoints == null)
			return;
		
		for(int i = 0; i < mAttachPoints.Length; i++)
		{
			mAttachPoints[i].Update (false);
		}
	}
	
	/// <summary>
	/// Add a JelloAttachPoint to this JelloBody.
	/// </summary>
	/// <param name="attachPoint">The JelloAttachPoint to add.</param>
	public void AddAttachPoint(JelloAttachPoint attachPoint) //TODO make alternative method sigantures.
	{
		if(mAttachPoints == null)
			mAttachPoints = new JelloAttachPoint[0];
		
		for(int i = 0; i < mAttachPoints.Length; i++)//dont add the same point twice.
		{	
			if(mAttachPoints[i] == attachPoint)
				return;
		}
		
		JelloAttachPoint[] oldPoints = mAttachPoints; 
		mAttachPoints = new JelloAttachPoint[mAttachPoints.Length + 1];
		for(int i =0; i < oldPoints.Length; i++)
		{
			mAttachPoints[i] = oldPoints[i];
		}
		mAttachPoints[mAttachPoints.Length - 1] = attachPoint;
	}
	
	/// <summary>
	/// Remove a JelloAttachPoint from this JelloBody.
	/// </summary>
	/// <param name="attachPoint">The JelloAttachPoint to remove.</param>
	public void RemoveAttachPoint(JelloAttachPoint attachPoint)
	{
		List<JelloAttachPoint> tempAttachPoints = new List<JelloAttachPoint>();
		for(int i = 0; i < mAttachPoints.Length; i++)
			if(attachPoint != mAttachPoints[i])
				tempAttachPoints.Add (mAttachPoints[i]);
		
		mAttachPoints = tempAttachPoints.ToArray();
	}
	
	/// <summary>
	/// Get the JelloAttachPoint at the given index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The JelloAttachPoint at the given index, null if index is out of range.</returns>
	public JelloAttachPoint GetAttachPoint(int index)
	{
		if(index < 0 || index >= mAttachPoints.Length)
			return null;
		
		return mAttachPoints[index];
	}

	/// <summary>
	/// Remove any invalid JelloAttachPoint from the JelloBody.
	/// </summary>
	public void ClearInvalidAttachPoints()
	{
		//Remove any invalid attach points.
		if(mAttachPoints != null)
		{
			bool skip = false;

			List<JelloAttachPoint> attachPoints = new List<JelloAttachPoint>();
			for(int i = 0; i < mAttachPoints.Length; i++)
			{
				JelloAttachPoint attachPoint = mAttachPoints[i];
				skip = false;

				if(attachPoint == null)
					continue;

				for(int a = 0; a < attachPoint.affectedIndices.Length; a++)
				{
					if(attachPoint.affectedIndices[a] > PointMassCount || attachPoint.affectedIndices[a] < 0)
					{
						skip = true;

						Debug.LogWarning(name + " JelloAttachPoint  # " + i + " is invalid, removing.");

						break; //attachpoint is invalid!
					}
				}
				
				if(skip)
					continue;
				
				attachPoints.Add (attachPoint);
			}
			mAttachPoints = attachPoints.ToArray();
		}
	}

	#endregion

	#region JOINTS

	/// <summary>
	/// Add a JelloJoint to this JelloBody.
	/// </summary>
	/// <param name="joint">The JelloJoint to add.</param>
	public void AddJoint(JelloJoint joint) //TODO make alternative method sigantures.
	{
		if(mJoints == null)
			mJoints = new JelloJoint[0];
		
		for(int i = 0; i < mJoints.Length; i++)
		{	
			if(mJoints[i] == joint)
				return;
		}
		
		bool rigidB = joint.bodyB == null && joint.rigidbodyB != null;
		bool rigidA = joint.bodyA == null && joint.rigidbodyA != null;
		//only check for similar if a rigidbody
		if(rigidB || rigidA)
		{
			bool similar = false;
			//add similar to a list?
			List<JelloJoint> similarJoints = new List<JelloJoint>();
			
			for(int i = 0; i < mJoints.Length; i++)
			{
				similar = false;
				if(rigidB)
				{
					if(joint.TransformB == mJoints[i].TransformB)
					{
						similar = true;
					}
					else if(joint.TransformB == mJoints[i].TransformA)
					{
						similar = true;
					}
				}
				else if(rigidA)
				{
					if(joint.TransformA == mJoints[i].TransformA)
					{
						similar = true;					
					}
					else if(joint.TransformA == mJoints[i].TransformB)
					{
						similar = true;
					}
				}
				
				if(similar)
					similarJoints.Add (mJoints[i]);
			}
			
			for(int i = 0; i < similarJoints.Count; i++)
			{
				similarJoints[i].numSimilar = similarJoints.Count + 1;
			}
			
			joint.numSimilar = similarJoints.Count + 1;
		}
		
		JelloJoint[] oldJoints = mJoints; 
		mJoints = new JelloJoint[mJoints.Length + 1];
		for(int i =0; i < oldJoints.Length; i++)
		{
			mJoints[i] = oldJoints[i];
		}
		mJoints[mJoints.Length - 1] = joint;
		
		if(Application.isPlaying)
			World.addJoint(joint);
	}
	
	/// <summary>
	/// Remove a JelloJoint from this JelloBody.
	/// </summary>
	/// <param name="joint">The JelloJoint to remove.</param>
	public void RemoveJoint(JelloJoint joint)
	{
		bool contains = false;

		List<JelloJoint> tempJoints = new List<JelloJoint>();
		for(int i = 0; i < mJoints.Length; i++)
		{
			if(joint != mJoints[i])
			{
				tempJoints.Add (mJoints[i]);
			}
			else
			{
				//do something
				contains = true;
			}
		}
		

		if(!contains)
			return;

		mJoints = tempJoints.ToArray();

		
		bool rigidB = joint.bodyB == null && joint.rigidbodyB != null;
		bool rigidA = joint.bodyA == null && joint.rigidbodyA != null;
		//only check for similar if a rigidbody
		if(rigidB || rigidA)
		{
			bool similar = false;
			//add similar to a list?
			List<JelloJoint> similarJoints = new List<JelloJoint>();
			
			for(int i = 0; i < mJoints.Length; i++)
			{
				similar = false;
				
				if(rigidB)
				{
					if(joint.TransformB == mJoints[i].TransformB)
					{
						similar = true;
					}
					else if(joint.TransformB == mJoints[i].TransformA)
					{
						similar = true;
					}
				}
				else if(rigidA)
				{
					if(joint.TransformA == mJoints[i].TransformA)
					{
						similar = true;
					}
					else if(joint.TransformA == mJoints[i].TransformB)
					{
						similar = true;
					}
				}
				
				if(similar)
					similarJoints.Add (mJoints[i]);
			}
			
			for(int i = 0; i < similarJoints.Count; i++)
			{
				similarJoints[i].numSimilar = similarJoints.Count;
			}
			
			joint.numSimilar = similarJoints.Count;
		}
		
		if(Application.isPlaying && !applicationIsQuitting)
			World.removeJoint(joint);
	}
	
	/// <summary>
	/// Gets the JelloJoint at the given index.
	/// </summary>
	/// <param name="index">The index of the JelloJoint.</param>
	/// <returns>The JelloJoint, null if index is out of range.</returns>
	public JelloJoint GetJoint(int index)
	{
		if(index < 0 || mJoints == null || index >= mJoints.Length)
			return null;
		
		return mJoints[index];
	}

	/// <summary>
	/// Clears any invalid JelloJoint from the JelloBody.
	/// </summary>
	public void ClearInvalidJoints()
	{	
		if(mJoints != null)
		{
			bool skip = false;

			//Remove any invalid joints.
			List<JelloJoint> joints = new List<JelloJoint>();
			for(int i = 0; i < mJoints.Length; i++)
			{
				JelloJoint joint = mJoints[i];
				skip = false;

				if(joint == null)
					continue;

				if(joint.bodyA != null)
				{
					for(int a = 0; a < joint.affectedIndicesA.Length; a++)
					{
						if(joint.affectedIndicesA[a] > PointMassCount || joint.affectedIndicesA[a] < 0)
						{
							Debug.LogWarning(name + " JelloJoint  # " + i + " is invalid, removing.");
							skip = true;
							break;
						}
					}
				}
				
				if(joint.bodyB != null)
				{
					for(int a = 0; a < joint.affectedIndicesB.Length; a++)
					{
						if(joint.affectedIndicesB[a] > PointMassCount || joint.affectedIndicesB[a] < 0)
						{
							Debug.LogWarning(name + " JelloJoint  # " + i + " is invalid, removing.");
							skip = true;
							break;
						}
					}
				}

				if(skip)
					continue;
				
				joints.Add (joint);
			}
			
			mJoints = joints.ToArray();
		}
	}


	#endregion

	#region POINT MASSES

	/// <summary>
	/// Get a JelloPointMass from this JelloBody. 
	/// 0 is the index of the first edge JelloPointMass and JelloBody.EdgePointMassCount is the index of the first internal JelloPointMass (if any exist).
	/// </summary>
	/// <param name="index">The index of the JelloPointMass.</param>
	/// <returns>The JelloPointMass at the given index, null if index is out of range.</returns>
	public JelloPointMass getPointMass(int index)
	{
		if(index >= 0)
		{
			if(index < mEdgePointMasses.Length)
				return mEdgePointMasses[index];
			else if(index < mEdgePointMasses.Length + mInternalPointMasses.Length)
				return mInternalPointMasses[ index - mEdgePointMasses.Length];
			else
				return null;
		} 
		else
		{
			return null;
		}
	}
	
	/// <summary>
	/// Get an edge JelloPointMass from this JelloBody.
	/// </summary>
	/// <param name="index">The index of the edge JelloPointMass.</param>
	/// <returns>The edge JelloPointMass at the given index, null if index is out of range.</returns>
	public JelloPointMass getEdgePointMass(int index)
	{
		if(index >= 0 && index < mEdgePointMasses.Length)
			return mEdgePointMasses[index];
		else 
			return null;
	}
	
	/// <summary>
	/// Get an internal JelloPointMass from this JelloBody.
	/// </summary>
	/// <param name="index">The index of the internal JelloPointMass.</param>
	/// <returns>The internal JelloPointMass at the given index, null if index is out of range.</returns>
	public JelloPointMass getInternalPointMass(int index)
	{
		if(index >= 0 && index < mInternalPointMasses.Length)
			return mInternalPointMasses[index];
		else 
			return null;
	}
	
	/// <summary>
	/// Set the edge JelloPointMass at the given index.
	/// </summary>
	/// <param name="pointMass">The new JelloPointMass.</param>
	/// <param name="index">The index that the new JelloPointMass will occupy. Operation will fail if index is out of range.</param>
	public void setEdgePointMass(JelloPointMass pointMass, int index)
	{
		if(index >= 0 && index < mEdgePointMasses.Length)
			mEdgePointMasses[index] = pointMass;
	}
	
	/// <summary>
	/// Set the internal JelloPointMass at the given index.
	/// </summary>
	/// <param name="pointMass">The new JelloPointMass.</param>
	/// <param name="index">The index that the new JelloPointMass will occupy. Operation will fail if index is out of range.</param>
	public void setInternalPointMass(JelloPointMass pointMass, int index)
	{
		if(index >= 0 && index < mInternalPointMasses.Length)
			mInternalPointMasses[index] = pointMass;
	}
	
	/// <summary>
	/// Add an internal JelloPointMass to this JelloBody.
	/// This will also add an internal vertex to JelloBody.Shape.InternalVertices.
	/// </summary>
	/// <param name="pointMass">The new JelloPointMass to add.</param>
	/// <param name="recenterBaseShape">Whether to recenter JelloBody.Shape.</param>
	public void addInternalPointMass(JelloPointMass pointMass, bool recenterBaseShape)
	{
		if(mBaseShape == null)
			return;

		if(mBaseShape.addInternalVertex(pointMass.LocalPosition))
		{
			JelloPointMass[] oldMasses = mInternalPointMasses;
			mInternalPointMasses = new JelloPointMass[oldMasses.Length + 1];
			for(int i = 0; i < oldMasses.Length; i++)
				mInternalPointMasses[i] = oldMasses[i];
			
			mInternalPointMasses[mInternalPointMasses.Length - 1] = pointMass;
			
			mBaseShape.finish(recenterBaseShape);
		}
	}

	
	/// <summary>
	/// Clear the internal JelloPointMasses from this JelloBody and the JelloClosedShape.InternalVertices from JelloBody.Shape.
	/// </summary>
	/// <param name="recenterBaseShape">Whether recenter JelloBody.Shape.</param>
	public virtual void clearInternalPointMasses(bool recenterBaseShape)
	{
		mInternalPointMasses = new JelloPointMass[0];
		mBaseShape.clearInternalVertices();
		mBaseShape.finish(recenterBaseShape);
	}
	
	/// <summary>
	/// At the given index, remove the internal JelloPointMass from this JelloBody and the internal vertex from JelloBody.Shape.InternalVertices.
	/// </summary>
	/// <param name="index">The index of internal JelloPointMass to be removed.</param>
	/// <param name="recenterBaseShape">Whether to recenter JelloBody.Shape.</param>
	public virtual void removeInternalPointMass(int index, bool recenterBaseShape)
	{
		if(index < 0 && index >= mInternalPointMasses.Length)
			return;

		List<JelloPointMass> tempMasses = new List<JelloPointMass>();
		for(int i = 0; i < mInternalPointMasses.Length; i++)
			if(i != index)
				tempMasses.Add (mInternalPointMasses[i]);

		mInternalPointMasses = tempMasses.ToArray();
		
		mBaseShape.removeInternalVertex(index);
		
		mBaseShape.finish(recenterBaseShape);

		ClearInvalidSubComponents();
	}

	/// <summary>
	/// At the given index, smartly remove the internal JelloPointMass from this JelloBody and the internal vertex from JelloBody.Shape.InternalVertices.
	/// Smartly means that most edits made to the JelloBody will be retained and is done via the JelloBody.smartSetShape() method.
	/// </summary>
	/// <param name="index">The index of internal JelloPointMass to be removed.</param>
	/// <param name="recenterBaseShape">Whether to recenter JelloBody.Shape.</param>
	/// <param name="options">ShapeSettingOptions on how Subcomponents should be modified as the JelloPointMass and JelloClosedShape vertex is removed.</param>
	/// <param name="smartOptions">SmartShapeSettingOptions on how Subcomponents should be modified as the JelloPointMass and JelloClosedShape vertex is removed.</param>
	public virtual void smartRemoveInternalPointMass(int index, bool recenterBaseShape, ShapeSettingOptions options = ShapeSettingOptions.None, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		if(index < 0 && index >= mInternalPointMasses.Length)
			return;

		Vector2[] tempItnernalVertices = new Vector2[mBaseShape.InternalVertexCount - 1];
		int a = 0;
		for(int i = 0; i < mBaseShape.InternalVertexCount; i++)
		{
			if(i != index)
			{
				tempItnernalVertices[a] = mBaseShape.InternalVertices[i];
				a++;
			}
		}

		JelloClosedShape newShape = new JelloClosedShape(mBaseShape.EdgeVertices, tempItnernalVertices, recenterBaseShape);

		smartSetShape(newShape, options, smartOptions);
	}




	/// <summary>
	/// Add an internal JelloPointMass to this JelloBody.
	/// This will also add an internal vertex to JelloBody.Shape.InternalVertices.
	/// Smartly means that most edits made to the JelloBody will be retained and is done via the JelloBody.smartSetShape() method.
	/// </summary>
	/// <param name="pointMass">The new JelloPointMass to add.</param>
	/// <param name="recenterBaseShape">Whether to recenter JelloBody.Shape.</param>
	/// <param name="options">ShapeSettingOptions on how Subcomponents should be modified as the JelloPointMass and JelloClosedShape vertex is added .</param>
	/// <param name="smartOptions">SmartShapeSettingOptions options on how Subcomponents should be modified as the JelloPointMass and JelloClosedShape vertex is added.</param>
	public void smartAddInternalPointMass(JelloPointMass pointMass, bool recenterBaseShape, ShapeSettingOptions options = ShapeSettingOptions.None, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		if(mBaseShape == null)
			return;

		JelloClosedShape newShape = new JelloClosedShape(mBaseShape.EdgeVertices, mBaseShape.InternalVertices, false);

		if(!newShape.addInternalVertex(pointMass.LocalPosition))
			return;

		newShape.finish(recenterBaseShape);

		smartSetShape(newShape, options, smartOptions);

		mInternalPointMasses[mInternalPointMasses.Length - 1] = pointMass;
	}

	#endregion

    #region HELPER FUNCTIONS

	/// <summary>
	/// Set the JelloBody.Position and JelloBody.Angle and telport all JelloPointMass objects with it.
	/// </summary>
	/// <param name="position">The JelloBody.Position.</param>
	/// <param name="angle">The JelloBody.Angle.</param>
	/// <param name="useBaseShape">Whether to set the JelloPointMass objects to the transformed JelloBody.Shape.Vertices position or to teleport them based on their current positions.</param>
	/// <param name="clearForceAndVelocity">Whether to clear the force and velocity of the JelloPointMass objects.</param>
	public void SetPositionAngleAll(Vector2 position, float angle, bool useBaseShape, bool clearForceAndVelocity)
	{
		if(useBaseShape)
		{
			Position = position;
			Angle = angle;
		
			for(int i = 0; i < mEdgePointMasses.Length; i++)
			{
				mEdgePointMasses[i].LocalPosition = mBaseShape.EdgeVertices[i];
				if(clearForceAndVelocity)
				{
					mEdgePointMasses[i].force = Vector2.zero;
					mEdgePointMasses[i].velocity = Vector2.zero;
				}
			}
			for(int i = 0; i < mInternalPointMasses.Length; i++)
			{
				mInternalPointMasses[i].LocalPosition = mBaseShape.InternalVertices[i];
				if(clearForceAndVelocity)
				{
					mInternalPointMasses[i].force = Vector2.zero;
					mInternalPointMasses[i].velocity = Vector2.zero;
				}
			}
		}
		else
		{
			Vector2 deltaPosition = position - Position;
			float deltaAngle = angle - Angle;
			Angle = angle;
			Position = position;

			for(int i = 0; i < mEdgePointMasses.Length; i++)
			{
				mEdgePointMasses[i].Position += deltaPosition;
				mEdgePointMasses[i].Position = Position + JelloVectorTools.rotateVector(mEdgePointMasses[i].Position - Position, deltaAngle); 
				if(clearForceAndVelocity)
				{
					mEdgePointMasses[i].force = Vector2.zero;
					mEdgePointMasses[i].velocity = Vector2.zero;
				}
			}
			for(int i = 0; i < mInternalPointMasses.Length; i++)
			{
				mInternalPointMasses[i].Position += deltaPosition;
				mInternalPointMasses[i].Position = Position + JelloVectorTools.rotateVector(mInternalPointMasses[i].Position - Position, deltaAngle); 
				if(clearForceAndVelocity)
				{
					mInternalPointMasses[i].force = Vector2.zero;
					mInternalPointMasses[i].velocity = Vector2.zero;
				}
			}
		}
	}

	/// <summary>
	/// Set the component references for this JelloBody.
	/// RigidBody, Transform, PolygonCollider2D, and MeshLink.
	/// </summary>
	public void setComponentReferences()
	{
		myTransform = transform;
		polyCollider = GetComponent<PolygonCollider2D>();
		meshLink = GetComponent<MeshLink>();
	}

	/// <summary>
	/// Modify the solidity of this JelloBody by a given percent.
	/// All spring damping values are modified by one tenth of the percent passed in.
	/// </summary>
	/// <param name="percent">The percent to modify solidity by.</param>
	public virtual void modifySolidityByPercent(float percent){}

	/// <summary>
	/// Set the the PolygonCollider2D.points to the current JelloPointMass.LocalPosition values for this JelloBody.
	/// </summary>
	public void UpdateCollider()
	{
		if( pointMassPositions == null || pointMassPositions.Length != EdgePointMassCount)
			pointMassPositions = new Vector2[EdgePointMassCount];                    

		for(int i = 0; i < mEdgePointMasses.Length; i++)
			pointMassPositions[i] = getEdgePointMass(i).LocalPosition;

		polyCollider.points = pointMassPositions;
	}

    /// <summary>
    /// Detect if a global point is inside this JelloBody.
	/// Using JelloBody.polyCollider.overlapPoint(point) will be faster but might not be
	/// 100% accurate because JelloPointMass.Position is updated multiple times
	/// per FixedUpdate() where as the JelloBody.polyCollider is only updated once per FixedUpdate().
    /// </summary>
    /// <param name="point">The global point to test.</param>
    /// <returns>true if point is inside the JelloBody, false if not.</returns>
    public bool contains(ref Vector2 point)
    {
        // basic idea: draw a line from the point to a point known to be outside the body.  count the number of
        // lines in the polygon it intersects.  if that number is odd, we are inside.  if it"s even, we are outside.
        // in this implementation we will always use a line that moves off in the positive X direction from the point
        // to simplify things.

        bool inside = false;
		int h = mEdgePointMasses.Length - 1;
		
        for (int i = 0; i < mEdgePointMasses.Length; h = i++)
        {
			if
			(
				mEdgePointMasses[h].Position.y >= point.y != mEdgePointMasses[i].Position.y >= point.y
				&&
				point.x <= (mEdgePointMasses[h].Position.x - mEdgePointMasses[i].Position.x) * (point.y - mEdgePointMasses[i].Position.y) / (mEdgePointMasses[h].Position.y - mEdgePointMasses[i].Position.y) + mEdgePointMasses[i].Position.x 
			)
				inside= !inside;
        }
		
        return inside;
    }

	/// <summary>
	/// Detect if a global point is inside this JelloBody.
	/// Using JelloBody.polyCollider.overlapPoint(point) will be faster but might not be
	/// 100% accurate because JelloPointMass.Position is updated multiple times
	/// per FixedUpdate() where as the JelloBody.polyCollider is only updated once per FixedUpdate().
	/// </summary>
	/// <param name="point">The global point to test.</param>
	/// <returns>true if point is inside the JelloBody, false if not.</returns>
    public bool contains(Vector2 point)
    {
        return contains(ref point);
    }




	/// <summary>
	/// Flip the JelloBody about its local y-axis.
	/// </summary>
	public virtual void FlipX ()	//TODO check if this  still works //TODO should/does this this do something to the mesh link?
	{
		JelloPointMass[] tempMasses = new JelloPointMass[mEdgePointMasses.Length];
		Vector2[] tempVertices = new Vector2[polyCollider.points.Length];
		
		for(int i = 0; i < mEdgePointMasses.Length; i++)
		{
			mEdgePointMasses[i].Position = new Vector2(mEdgePointMasses[i].Position.x + 2 * (Position.x - mEdgePointMasses[i].Position.x), mEdgePointMasses[i].Position.y);
			tempMasses[mEdgePointMasses.Length - i - 1] = mEdgePointMasses[i];

			polyCollider.points[i] = new Vector2(polyCollider.points[i].x + 2 * (Position.x - polyCollider.points[i].x), polyCollider.points[i].y);
			tempVertices[polyCollider.points.Length - i - 1] = polyCollider.points[i];
		}

		mEdgePointMasses = tempMasses;
		polyCollider.points = tempVertices;
		pivotOffset = new Vector2(-pivotOffset.x, pivotOffset.y);

		tempMasses = new JelloPointMass[mInternalPointMasses.Length];
		for(int i = 0; i < mInternalPointMasses.Length; i++)
		{
			mInternalPointMasses[i].Position = new Vector2(mInternalPointMasses[i].Position.x + 2 * (Position.x - mInternalPointMasses[i].Position.x), mInternalPointMasses[i].Position.y);
			tempMasses[mInternalPointMasses.Length - i - 1] = mInternalPointMasses[i];
		}
		mInternalPointMasses = tempMasses;

		mBaseShape.flipX();
	}
	
	/// <summary>
	/// Flip the body about its local x-axis.
	/// </summary>
	public virtual void FlipY ()
	{
		JelloPointMass[] tempMasses = new JelloPointMass[mEdgePointMasses.Length];
		Vector2[] tempVertices = new Vector2[polyCollider.points.Length];

		for(int i = 0; i < mEdgePointMasses.Length; i++)
		{
			mEdgePointMasses[i].Position = new Vector2(mEdgePointMasses[i].Position.x, mEdgePointMasses[i].Position.y + 2 * (Position.y - mEdgePointMasses[i].Position.y));
			tempMasses[mEdgePointMasses.Length - i - 1] = mEdgePointMasses[i];

			polyCollider.points[i] = new Vector2(polyCollider.points[i].x, polyCollider.points[i].y + 2 * (Position.y - polyCollider.points[i].y));
			tempVertices[polyCollider.points.Length - i - 1] = polyCollider.points[i];
		}
		
		mEdgePointMasses = tempMasses;
		polyCollider.points = tempVertices;
		pivotOffset = new Vector2(pivotOffset.x, -pivotOffset.y);

		tempMasses = new JelloPointMass[mInternalPointMasses.Length];
		for(int i = 0; i < mInternalPointMasses.Length; i++)
		{
			mInternalPointMasses[i].Position = new Vector2(mInternalPointMasses[i].Position.x, mInternalPointMasses[i].Position.y + 2 * (Position.y - mInternalPointMasses[i].Position.y));
			tempMasses[mInternalPointMasses.Length - i - 1] = mEdgePointMasses[i];
		}
		mInternalPointMasses = tempMasses;

		mBaseShape.flipY();
	}
	
	/// <summary>
	/// Destroy this JelloBody and remove it from the JelloWorld.
	/// </summary>
	public void Destroy()
	{
		//should i null everything out first?

		if(!applicationIsQuitting)
			World.removeBody(this);

		Destroy (gameObject);
	}

	/// <summary>
	/// Called any time the JelloBody.Scale property is set.
	/// </summary>
	protected virtual void HandleScaleModification(){}

	/// <summary>
	/// Clears any invalid subcomponent from the JelloBody.
	/// Will check JelloJoint, JelloAttachPoint, and JelloSpring.
	/// </summary>
	public virtual void ClearInvalidSubComponents()
	{
		ClearInvalidJoints();
		ClearInvalidAttachPoints();
	}

	#endregion
	
    #region DEBUG VISUALIZATION
	
    /// <summary>
    /// This function draws the points to the screen as lines, showing several things:
	/// Edge JelloPointMass objects connected via grey lines.
	/// JelloBody.Position and JelloBody.Angle depicted by an "L" in grey.
	/// JelloPointMass.velocity via black lines.
	/// JelloBody.Shape in white.
	/// JelloBody.Shape (transformed into global space) in yellow.	
    /// </summary>
    public virtual void debugDrawMe() //TODO draw internal point masses?
	{
		Vector3[] debugVerts = new Vector3[mEdgePointMasses.Length];
		Color color = Color.grey;

		for (int i = 0; i < mEdgePointMasses.Length; i++)
			debugVerts[i] = new Vector3(mEdgePointMasses[i].Position.x, mEdgePointMasses[i].Position.y, myTransform.position.z);
		
		for(int i = 0; i < debugVerts.Length; i++)
		{
			if(World.showPointMassPositions)
			{
				color = Color.grey;

				Debug.DrawLine(debugVerts[i], debugVerts[i + 1 < debugVerts.Length ? i + 1 : 0], color);

				Vector2 up = JelloVectorTools.rotateVector(new Vector2(1f * Scale.x, 0f), Angle) * 0.5f;
				Vector2 right = JelloVectorTools.rotateVector(new Vector2(0f ,1f * Scale.y), Angle) * 0.5f;

				Debug.DrawRay(myTransform.position, new Vector3(up.x, up.y, 0f), color);
				Debug.DrawRay(myTransform.position, new Vector3(right.x, right.y, 0f), color);
			}
			
			if(World.showVelocities)
			{
				color = new Color(0, 0, 0, 0.75f);

				Debug.DrawRay(debugVerts[i], mEdgePointMasses[i].velocity * 0.25f, color);
			}
		}

		if(World.showBaseShape || World.showXformedBaseShape)
		{
			for(int i = 0; i < mBaseShape.EdgeVertexCount; i++)
			{
				if(World.showBaseShape)
				{
					color = new Color(255f, 255, 255, 0.75f);
					Debug.DrawLine(myTransform.position + (Vector3)mBaseShape.EdgeVertices[i], myTransform.position + (Vector3)mBaseShape.EdgeVertices[i + 1 < mBaseShape.EdgeVertexCount ? i + 1 : 0], color);
				}
				if(World.showXformedBaseShape)
				{
					color = new Color(255f, 255, 0, 0.75f);
					Debug.DrawLine(myTransform.TransformPoint(mBaseShape.EdgeVertices[i]), myTransform.TransformPoint(mBaseShape.EdgeVertices[i + 1 < mBaseShape.EdgeVertexCount ? i + 1 : 0]), color);
				}	
			}
		}
	}	

    #endregion

	bool applicationIsQuitting = false;
	
	void OnApplicationQuit()
	{
		applicationIsQuitting = true;
	}

	//TODO add ondestroy and onvisible etc...
	void OnDestroy()
	{
		#if UNITY_EDITOR
		destroying = true;
		#endif

		//TODO should i also null values here?
	}
	
	#if UNITY_EDITOR
	
	//this is to ensure editor made changes will trigger the setters of properties
	
	bool destroying;
	
	
	
	void OnValidate()
	{
		if(!destroying)
			Validate();
	}
	
	protected virtual void Validate() //TODO make sure all necessary values are validated
	{
		Mass = mMass;
	}
	
	#endif
	
}