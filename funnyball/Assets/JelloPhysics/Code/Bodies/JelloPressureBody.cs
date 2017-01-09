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
using System.Text;
using UnityEngine;

/// <summary>
/// A subclass of JelloSpringBody, with the added element of pressurized gas inside the JelloBody.  
/// The amount of pressure can be adjusted at will to inflate / deflate the object. JelloSprinBody.ShapeMatching may prevent the JelloPressureBody from
/// deflating much smaller than the transformed JelloBody.Shape. 
/// </summary>
public class JelloPressureBody : JelloSpringBody
{
    #region PROTECTED/PRIVATE VARIABLES
	
	/// <summary>
	/// The volume (area) of the JelloPressureBody.
	/// </summary>
    private float mVolume = 1f;

	/// <summary>
	/// The inverse volume (inverse area) of the JelloPressureBody.
	/// </summary>
	private float mInverseVolume = 1f;

	/// <summary>
	/// The amount of gas inside the JelloPressureBody.
	/// </summary>
	[SerializeField]
    private float mGasAmount = 40f;
	
	/// <summary>
	/// An array of edge normals.
	/// Calculated each JelloPressureBody.accumulateInternalForces(). 
	/// </summary>
	[SerializeField]
    private Vector2[] mNormalList;
	
	/// <summary>
	/// An array of edge lengths.
	/// Calculated each JelloPressureBody.accumulateInternalForces().
	/// </summary>
	[SerializeField]
    private float[] mEdgeLengthList;

	/// <summary>
	/// The previous scale.
	/// </summary>
	private Vector3 prevScale = Vector3.zero;

	/// <summary>
	/// The scale ratio.
	/// Used to account for non-uniform scales.
	/// </summary>
	private float scaleRatio = 0f;
    
	#endregion
	
    #region PUBLIC PROPERTIES
	
    /// <summary>
    /// Amount of gas inside the body.
    /// </summary>
    public float GasAmount
    {
        set { mGasAmount = value; }
        get { return mGasAmount; }
    }
	
    /// <summary>
    /// Gets the last calculated volume for the body.
    /// </summary>
    public float Volume
    {
        get { return mVolume; }
    }
	
    #endregion
	
	#region SETTING SHAPE
	
	/// <summary>
	/// Set the JelloBody.Shape of this JelloBody to a new JelloClosedShape object.  This function 
	/// will remove any existing JelloPointMass objects, and replace them with new ones if
	/// the new JelloClosedShape has a different vertex count than the previous one.  In this case
	/// the JelloPointMass.Mass for each newly added JelloPointMass will be set to the JelloBody.Mass. Otherwise the JelloBody.Shape is just
	/// updated, not affecting any existing JelloPointMass other than by position.
	/// Any JelloJoint or AttachPoint made invalid by the new shape will be removed.
	/// If present, each internal JelloSpring will be removed and a new set will be built.
	/// If present, each custom spring will be removed.
	/// Will clear and replace every edge JelloSpring.
	/// Will update every JelloSpring rest length.
	/// </summary>
	/// <param name="shape">The new JelloClosedShape to implement</param>
	/// <param name="options">ShapeSettingOptions for setting the JelloClosedShape.</param>
	public override void setShape (JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None)
	{
		base.setShape (shape, options);
		
		mNormalList = new Vector2[mEdgePointMasses.Length];
        mEdgeLengthList = new float[mEdgePointMasses.Length];
	}

	public override void smartSetShape (JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		base.smartSetShape (shape, options, smartOptions);
		
		mNormalList = new Vector2[mEdgePointMasses.Length];
		mEdgeLengthList = new float[mEdgePointMasses.Length];
	}
	
	#endregion
	
    #region ACCUMULATING FORCES

	/// <summary>
	/// Accumulates the forces internal to the JelloPressureBody.
	/// Calculates forces from gas pressure.
	/// Base class will calculate internal, edge, and custom JelloSpring forces.
	/// This is called JelloWorld.Iterations times per Time.FixedUpdate by JelloWorld.update();
	/// </summary>
	/// <param name="deltaTime">Amount of time to calculate forces over.</param>
    public override void accumulateInternalForces(float deltaTime)
    {
		//spring forces calculated here
		base.accumulateInternalForces(deltaTime);
		// internal forces based on pressure equations.  we need 2 loops to do this.  one to find the overall volume of the
		// body, and 1 to apply forces.  we will need the normals for the edges in both loops, so we will cache them and remember them.
		int j;
		Vector2 edge;
		Vector2 pressureV;

		mVolume = 0f;

		if(prevScale != Scale)
		{
			scaleRatio = Vector2.Distance(Vector2.zero, (Vector2)Scale) * 0.8761f;
			prevScale = Scale;
		}

		// first calculate the volume of the body, and cache normals as we go.
		for (int i = 0; i < mEdgePointMasses.Length; i++)
		{
			j = i + 1 < mEdgePointMasses.Length ? i + 1 : 0;

			edge = JelloVectorTools.getPerpendicular(mEdgePointMasses[j].Position - mEdgePointMasses[i].Position);

			//cache edge length
			mEdgeLengthList[i] = Vector2.Distance(Vector2.zero, edge); //TODO consider incorporating collision info class?
		
			// cache normal
			mNormalList[i] = edge;
			if(mEdgeLengthList[i] != 0f)
				mNormalList[i] /= mEdgeLengthList[i];

			if(Shape.winding == JelloClosedShape.Winding.CounterClockwise)
				mNormalList[i] *= -1f;

			// add to volume
			mVolume += 0.5f * (mEdgePointMasses[j].Position.x - mEdgePointMasses[i].Position.x) * (mEdgePointMasses[j].Position.y + mEdgePointMasses[i].Position.y); 
		}

		mVolume = Mathf.Abs(mVolume);
		mInverseVolume = 1 / mVolume;

		// now loop through, adding forces!
		for (int i = 0; i < mEdgePointMasses.Length; i++)
		{
			j = i + 1 < mEdgePointMasses.Length ? i + 1 : 0;
			
			pressureV = mNormalList[i] * mInverseVolume *  mEdgeLengthList[i]  * mGasAmount * scaleRatio;

			mEdgePointMasses[ i ].force += pressureV;
			mEdgePointMasses[ j ].force += pressureV;
		}
    }
	
    #endregion

	#region HELPER FUNCTIONS

	//// <summary>
	/// Modifies the solidity of this JellBody by a given percentage.
	/// Affects JelloPressureBody.GasAmount.
	/// Affects JelloSpring.stifness, JelloSpring.damping, ShapeMatchingStiffness an ShapeMatchingDamping.
	/// All JelloSpring.damping values are modified by one tenth of the percent passed in.
	/// </summary>
	/// <param name="percent">Percent to modify solidity by. Clamped between -100 and 100.</param>
	public override void modifySolidityByPercent(float percent)
	{
		base.modifySolidityByPercent(percent);

		percent = Mathf.Clamp(percent, -100f, 100f) / 100f;

		if(mGasAmount <= 0f && percent > 0)
			mGasAmount = 1f;
		else
			mGasAmount *= 1 + percent;
	}

	#endregion

    #region DEBUG VISUALIZATION
	
	/// <summary>
	/// Draws the JelloPressureBody edge normal vectors.
	/// </summary>
    public override void debugDrawMe()
    {
        base.debugDrawMe();

		Color color = new Color(0, 255f, 0f, 0.75f);

		if(World.showPressureNormals)
		{
			int j;
			for (int i = 0; i < mEdgePointMasses.Length; i++)
			{
				j = i + 1 < mEdgePointMasses.Length ? i + 1 : 0;

				Vector2 norm = mNormalList[i];

				Debug.DrawRay
					(
						new Vector3 ((mEdgePointMasses[ i ].Position.x + mEdgePointMasses[ j ].Position.x) * 0.5f, (mEdgePointMasses[ i ].Position.y + mEdgePointMasses[ j ].Position.y) * 0.5f, transform.position.z), 
						norm * 0.25f,
						color
					);
			}
		}
    }
    
    #endregion

	#region NORMALS
	
	/// <summary>
	/// Gets the edge normal at the given index.
	/// </summary><returns>The edge normal vector.</returns>
	/// <param name="index">The index of edge normal.</param>
	public Vector2 getNormal (int index)
	{
		return mNormalList[index];
	}
	
	#endregion
}

