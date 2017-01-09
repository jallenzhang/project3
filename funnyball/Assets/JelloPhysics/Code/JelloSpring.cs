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
/// JelloSpring that connects two JelloPointMass objects inside one JelloBody.
/// Forces are calculated during JelloSpringBody.accumulateInternalForces() which is called JelloWorld.Iterations times each Time.FixedUpdate.  
/// </summary>
[Serializable]
public class JelloSpring
{
	
	#region CONSTRUCTORS
	
	/// <summary>
	/// Initializes a new instance of the JelloSpring class.
	/// </summary>
    public JelloSpring()
    {
        pointMassA = pointMassB = 0;
        length = scaledLength = stiffness = damping = 0f;
		lengthMultiplier = 1f;
    }
	
	/// <summary>
	/// Initializes a new instance of the JelloSpring class.
	/// </summary>
	/// <param name="pmA">The index of first JelloPointMass this JelloSpring will be attached to.</param>
	/// <param name="pmB">The index of second JelloPointMass this JelloSpring will be attached to.</param>
	/// <param name="dist">The rest length of JelloSpring.</param>
	/// <param name="stiff">The JelloSpring.stiffness</param>
	/// <param name="damp">The JelloSpring.damping.</param>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Create a new spring and add it to a body 
	/// JelloSpringBody springBody;
	/// 
	/// JelloSpring spring = new JelloSpring(0, springBody.PointMassCount - 1, 5f, 100f, 20f);
	/// ~~~
    public JelloSpring(int pmA, int pmB, float dist, float stiff, float damp)
    {
        pointMassA = pmA;
        pointMassB = pmB;
        length = dist;
        stiffness = stiff;
        damping = damp;
		lengthMultiplier = 1f;
    }
	
	#endregion
	
	#region PUBLIC VARIABLES
	
    /// <summary>
	/// Index of the first JelloPointMass the JelloSpring is connected to.
    /// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the index of the first spring in the body to the last PointMass of the body
	/// JelloSpringBody springBody;
	/// 
	/// springBody.getSpringAtIndex(0).pointMassA = springBody.PointMassCount - 1;
	/// ~~~
	public int pointMassA;

    /// <summary>
	/// Index of the JelloPointMass the JelloSpring is connected to.
    /// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the index of the first spring in the body to the last PointMass of the body
	/// JelloSpringBody springBody;
	/// 
	/// springBody.getSpringAtIndex(0).pointMassB = springBody.PointMassCount - 1;
	/// ~~~
	public int pointMassB;

    /// <summary>
	/// The "rest length" (deisred length) of the JelloSpring. At this length, no force is exerted on either of the JelloPointMass objects.
    /// In relation to a JelloBody, this distance is stored at an un-scaled length.
    /// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //Set the springBody's first spring's length to zero
	/// JelloSpringBody springBody;
	/// 
	/// springBody.getSpringAtIndex(0).length = 0f;
	/// ~~~
	public float length;

	/// <summary>
	/// The "rest length" (deisred length) of the JelloSpring. At this length, no force is exerted on either of the JelloPointMass objects.
	/// In relation to a JelloBody, this distance is stored at a scaled length.
	/// </summary>
	public float scaledLength;

	/// <summary>
	/// A multiplier to increase or decrease the "rest length" of the JelloSpring.
	/// </summary>
	public float lengthMultiplier;

    /// <summary>
    /// The stiffness, or "strength" of the JelloSpring.
    /// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //double the stiffness of the first spring in the body
	/// JelloSpringBody springBody;
	/// 
	/// springBody.getSpringAtIndex(0).pointMassB *= 2f;
	/// ~~~
	public float stiffness;

    /// <summary>
    /// The coefficient for damping, to reduce overshoot.
    /// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the springBody's first spring's damping
	/// JelloSpringBody springBody;
	/// 
	/// springBody.getSpringAtIndex(0).damping = 10f;
	/// ~~~
	public float damping;

	#endregion
}


