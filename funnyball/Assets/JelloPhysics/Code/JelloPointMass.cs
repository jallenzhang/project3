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
/// the most important class in JelloPhysics, every JelloBody is made up of JelloPointMass objects, connected to form
/// shapes. Each JelloPointMass can have its own mass, allowing for objects with different center-of-gravity.
/// </summary>
[Serializable]
public class JelloPointMass
{
	#region PRIVATE VARIABLES
	
	/// <summary>
	/// The mass.
	/// </summary>
	[SerializeField]
	private float mMass = 1f;

	/// <summary>
	/// The inverse of JelloPointMass.mMass.
	/// </summary>
	[SerializeField]
	private float mInverseMass = 1f;

	/// <summary>
	/// The global position.
	/// </summary>
	[SerializeField]
	private Vector2 mPosition;
	
	#endregion
	
    #region PUBLIC VARIABLES

    /// <summary>
    /// Force accumulation variable.  Reset to Zero after each call to JelloPointMass.integrate().
    /// </summary>
	public Vector2 force;

	/// <summary>
	/// External force accumulation variable. Reset only once per FixedUpdate as opposed to once per solver iteration.
	/// </summary>
	public Vector2 externalForce;

	/// <summary>
	/// Force accumulation variable that is never resety by the simulation, only by the user.
	/// </summary>
	public Vector2 persistantForce;

	/// <summary>
	/// The velocity.
	/// </summary>
	public Vector2 velocity;

	/// <summary>
	/// The JelloSpringBody.ShapeMatching force multiplier.
	/// The shape matching forces calculated for this JelloPointMass will be directly multiplied by this value. 
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Double the effect of shape matching forces on all pont masses in a given body
	/// JelloBody body;
	/// 
	/// for(int i = 0; i < body.PointMassCount; i++)
	/// {
	/// 	body.getPointMass(i).shapeMatchingMultiplier = 2f;
	/// }
	/// ~~~
	public float shapeMatchingMultiplier = 1f;

	/// <summary>
	/// If this JelloPointMass is an internal JelloPointMass and this value is set to true, it will be forced inside of the perimter of the JelloBody once per FixedUpdate().
	/// This takes extra computation, so leave this disabled unless you have problems with internal JelloPointMasses moving outside of your JelloBody.
	/// </summary>
	public bool forceInternal = false;
	
	/// <summary>
	/// The JelloBody that this JelloPointMass belongs to. Needed to derive JelloPointMass.LocalPosition.
	/// </summary>
	public JelloBody body;
    
	#endregion

    #region CONSTRUCTORS
	
	/// <summary>
	/// Initializes a new instance of the JelloPointMass class.
	/// </summary>
    public JelloPointMass(){}
	
	/// <summary>
	/// Initializes a new instance of the JelloPointMass class.
	/// </summary>
	/// <param name="mass">The mass of the JelloPointMass.</param>
	/// <param name="position">The position of the JelloPointMass.</param>
	/// <param name="b">The JelloBody that this JelloPointMass belongs to.</param>
	/// <param name="local">Whether the given position is local to the Given JelloBody.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //create a new JelloPointMass
	/// JelloBody body;
	/// 
	/// JelloPointMass pm = new JelloPointMass(1f, Vector2.Zero, body, true);
	/// ~~~
    public JelloPointMass(float mass, Vector2 position, JelloBody b, bool local)
    {
		body = b;
		Mass = mass;

		if(local)
			LocalPosition = position;
		else
			Position = position;
    }
    #endregion

    #region INTEGRATION

    /// <summary>
    /// Integrate JelloPointMass.force >> JelloPointMass.velocity >> JelloPointMass.Position, and set JelloPointMass.force to zero.
    /// This is usually called by the World.update() method, the user should not need to call it directly.
    /// </summary>
    /// <param name="deltaTime">The amount of time elapsed in seconds.</param>
	public void integrate (float deltaTime)
	{
		 if (mMass != Mathf.Infinity)
        {

			//main choice. others work well also.
			force += externalForce + persistantForce;
			velocity += force * mInverseMass * deltaTime;
			mPosition += velocity * deltaTime + 0.5f * force * mInverseMass * deltaTime * deltaTime;


			/*Vector2 nextPos = position + (position - prevPosition) + force * mInverseMass * deltaTime * deltaTime;
			prevPosition = position;
			position = nextPos;
			velocity = (position - prevPosition) / deltaTime;*/
        }
	}	
    #endregion
	
	#region PUBLIC PROPERTIES
	
	/// <summary>
	/// Get or set the mass. Setting updates JelloPointMass.InverseMass
	/// </summary>
	/// <value>The mass of this JelloPointMass.</value>
	public float Mass
	{
		get{ return mMass;}
		set
		{
			mMass = Mathf.Max(0f, value);
			
			if(mMass >= 10000)
				mMass = Mathf.Infinity;
			
			mInverseMass = mMass > 0f && mMass != Mathf.Infinity ? 1 / mMass : 0f;
		}
	}
	
	/// <summary>
	/// Get the inverse of JelloPointMass.Mass.
	/// </summary>
	/// <value>The inverse mass of this JelloPointMass.</value>
	public float InverseMass
	{
		get{ return mInverseMass; }
	}
	
	/// <summary>
	/// Get or set the local position of this JelloPointMass.
	/// This value not stored and is calculated each time LocalPosition is called.
	/// </summary>
	/// <value>The position of this JelloPointMass local to the JelloPointMass.body.</value>
	public Vector2 LocalPosition
	{
		get { return body.transform.InverseTransformPoint(mPosition); }
		set { mPosition = body.transform.TransformPoint(value); }
	}
	
	/// <summary>
	/// Get or set the global position of this JelloPointMass.
	/// </summary>
	/// <value>The global position fo this JelloPointMass</value>
	public Vector2 Position
	{
		get{ return mPosition; }
		set{ mPosition = value; }

	}
	#endregion
}

