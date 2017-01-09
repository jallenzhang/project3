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
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The JelloSpringBody is held together by JelloSpring objects attached in a number of different ways (Though all from one JelloPointMass to another).
/// By default the JelloSpringBody perimeter (edge) JelloPointMass objects will be chanied together by JelloSpring objects, applying forces to each JelloPointMass in order to maintain the original distance between them.
/// Internal JelloSpring objects are used the same as above, but connect select perimeter (edge) JelloPointMass objects to internal JelloPointMass objects. Connections are chosen via triangulation of the JelloBody.Shape.
/// Additional "Custom" JelloSpring objects may be created by the user for finer control of the JelloSpringBody. These are user generated and connect any two JelloPointMass objects to eachother.
/// Shape matching will apply forces to each JelloPointMass (edge and internal) attempting to return it to its original local position.
/// </summary>
public class JelloSpringBody : JelloBody
{
    #region PRIVATE/PROTECTED VARIABLES
	
	/// <summary>
	/// Array of JelloSpring exerting force on this JelloBody.
	/// Connects the outlining JelloPointMass.
	/// </summary>
	[SerializeField]
	protected JelloSpring[] mEdgeSprings = new JelloSpring[0];

	/// <summary>
	/// Array of JelloSpring exerting force on this JelloBody.
	/// Connect some of the outlining JelloPointMassto internal JelloPointMass.
	/// These JelloSpring connections are chosen via triangulation.
	/// </summary>
	[SerializeField]
	protected JelloSpring[] mInternalSprings = new JelloSpring[0];

	/// <summary>
	/// Array of JelloSpring exerting force on this JelloBody.
	/// Set by user.
	/// </summary>
	[SerializeField]
	protected JelloSpring[] mCustomSprings = new JelloSpring[0];

    /// <summary>
    /// Whether or not shape matching is on.
	/// Shape matching applies spring forces to make any JelloPointMass return to its original (local) position.
    /// </summary>
	[SerializeField]
    protected bool mShapeMatchingOn = true;
	
	/// <summary>
	/// The default edge JelloSpring.stiffness.
	/// Used for edge JelloSpring.
	/// </summary>
	[SerializeField]
	protected float mDefaultEdgeSpringK = 600f;
	
	/// <summary>
	/// The default edge JelloSpring.damping.
	/// Used for edge JelloSpring.
	/// </summary>
	[SerializeField]
    protected float mDefaultEdgeSpringDamp = 80f;
	
	/// <summary>
	/// The shape spring force stiffness.
	/// Used for shape matching.
	/// </summary>
	[SerializeField]
    protected float mShapeSpringK = 600f;
	
	/// <summary>
	/// The shape spring force damping.
	/// Used for shape matching.
	/// </summary>
	[SerializeField]
    protected float mShapeSpringDamp = 40f;
	
	/// <summary>
	/// The default custom JelloSpring.stifness.
	/// Used for custom JelloSpring.
	/// </summary>
	[SerializeField]
	protected float mDefaultCustomSpringK = 200f;
	
	/// <summary>
	/// The default custom JelloSpring.damping.
	/// Used for custom JelloSpring
	/// </summary>
    [SerializeField]
	protected float mDefaultCustomSpringDamp = 40f;

	/// <summary>
	/// The default minternal JelloSpring.stiffness.
	/// Used for internal JelloSpring.
	/// </summary>
	[SerializeField]
	protected float mDefaultInternalSpringK = 200f;
	
	/// <summary>
	/// The default internal JelloSpring.damping.
	/// Used for internal JelloSpring.
	/// </summary>
	[SerializeField]
	protected float mDefaultInternalSpringDamp = 40f;

    #endregion
	
	#region PUBLIC PROPERTIES


	/// <summary>
	/// Gets the JelloSpring count.
	/// Includes edge, internal, and custom JelloSpring.
	/// </summary>
	public int SpringCount
	{
		get{ return mEdgeSprings.Length + mInternalSprings.Length + mCustomSprings.Length; }
	}

	/// <summary>
	/// Gets the edge JelloSpring count.
	/// </summary>
	public int EdgeSpringCount
	{
		get{ return mEdgeSprings.Length; }
	}

	/// <summary>
	/// Gets the internal JelloSpring count.
	/// </summary>
	public int InternalSpringCount
	{
		get{ return mInternalSprings.Length; }
	}

	/// <summary>
	/// Gets the edge JelloSpring count.
	/// </summary>
	public int CustomSpringCount
	{
		get{ return mCustomSprings.Length; }
	}
	
	/// <summary>
	/// Whether this JelloBody has shape matching forces enabled.
	/// Shape matching will apply spring forces to every JelloPointMass in order to try and retain the original shape.
	/// </summary>
	public bool ShapeMatching
	{
		get{ return mShapeMatchingOn; }
		set{ mShapeMatchingOn = value; }
	}
	
	/// <summary>
	/// How stiff the shape matching springs forces are.
	/// </summary>
	public float ShapeSpringStiffness
	{
		get{ return mShapeSpringK; }
		set{ mShapeSpringK = value; }
	}
	
	/// <summary>
	/// How much damping is applied via shape matching spring forces.
	/// </summary>
	public float ShapeSpringDamping
	{
		get{ return mShapeSpringDamp; }
		set{ mShapeSpringDamp = value; }
	}
	
	
	/// <summary>
	/// The default custom JelloSpring.stiffness.
	/// Will update every custom JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first custom spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// body.getCustomSpring(0).stiffness *= 2f;
	/// body.DefaultCustomSpringStiffness = 10f;
	/// ~~~
	public float DefaultCustomSpringStiffness
	{
		get{ return mDefaultCustomSpringK; }
		set
		{
			float oldSpringK = mDefaultCustomSpringK;
			mDefaultCustomSpringK = value;
			
			if(oldSpringK != mDefaultCustomSpringK)
				setUnchangedCustomSpringConstants(mDefaultCustomSpringK, mDefaultCustomSpringDamp, oldSpringK, mDefaultCustomSpringDamp);
		}
	}
	
	/// <summary>
	/// The default custom JelloSpring.damping.
	/// Will update every custom JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first custom spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// body.getCustomSpring(0).damping *= 2f;
	/// body.DefaultCustomSpringDamping = 10f;
	/// ~~~
	public float DefaultCustomSpringDamping
	{
		get{ return mDefaultCustomSpringDamp; }
		set
		{
			float oldSpringDamp = mDefaultCustomSpringDamp;
			mDefaultCustomSpringDamp = value;
			
			if(oldSpringDamp != mDefaultCustomSpringDamp)
				setUnchangedCustomSpringConstants(mDefaultCustomSpringK, mDefaultCustomSpringDamp, mDefaultCustomSpringK, oldSpringDamp);
		}
	}
	
	/// <summary>
	/// The default edge JelloSpring.stiffness.
	/// Will update every edge JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first edge spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// for(int i = 0; i < body.SpringCount; i++)
	/// {
	/// 	if(body.getSpringAtIndex(i).edgeSrping)
	/// 	{
	/// 		body.getSpringAtIndex(i).stiffness *= 2f;
	/// 		break;
	/// 	}
	/// }
	/// 
	/// body.DefaultEdgeSpringStiffness = 10f;
	/// ~~~
	public float DefaultEdgeSpringStiffness
	{
		get{ return mDefaultEdgeSpringK; }
		set
		{
			float oldSpringK = mDefaultEdgeSpringK;
			mDefaultEdgeSpringK = value;
			
			if(oldSpringK != mDefaultEdgeSpringK)
				setUnchangedEdgeSpringConstants(mDefaultEdgeSpringK, mDefaultEdgeSpringDamp, oldSpringK, mDefaultEdgeSpringDamp);
		}
	}
	
	/// <summary>
	/// The default edge JelloSpring.damping.
	/// Will update every edge JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first edge spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// for(int i = 0; i < body.SpringCount; i++)
	/// {
	/// 	if(body.getSpringAtIndex(i).edgeSrping)
	/// 	{
	/// 		body.getSpringAtIndex(i).damping *= 2f;
	/// 		break;
	/// 	}
	/// }
	/// 
	/// body.DefaultEdgeSpringDamping = 10f;
	/// ~~~
	public float DefaultEdgeSpringDamping
	{
		get{ return mDefaultEdgeSpringDamp; }
		set
		{
			float oldSpringDamp = mDefaultEdgeSpringDamp;
			mDefaultEdgeSpringDamp = value;
			
			if(oldSpringDamp != mDefaultEdgeSpringDamp)
				setUnchangedEdgeSpringConstants(mDefaultEdgeSpringK, mDefaultEdgeSpringDamp, mDefaultEdgeSpringK, oldSpringDamp);
		}
	}

	/// <summary>
	/// The default internal JelloSpring.stiffness.
	/// Will update every edge JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first internal spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// body.getInternalSpring(0).stiffness *= 2f;
	/// body.DefaultInternalSpringStiffness = 10f;
	/// ~~~
	public float DefaultInternalSpringStiffness
	{
		get{ return mDefaultInternalSpringK; }
		set
		{
			float oldSpringK = mDefaultInternalSpringK;
			mDefaultInternalSpringK = value;
			
			if(oldSpringK != mDefaultInternalSpringK)
				setUnchangedInternalSpringConstants(mDefaultInternalSpringK, mDefaultInternalSpringDamp, oldSpringK, mDefaultInternalSpringDamp);
		}
	}
	
	/// <summary>
	/// The default internal JelloSpring.damping.
	/// Will update every edge JelloSpring that has not been changed from default.
	/// </summary>
	/// 
	/// <dl class="example"><dt>Example</dt></dl>
	/// ~~~{.c}
	/// //change the first internal spring to a custom value and then modify all others.
	/// JelloSpringBody body;
	/// 
	/// body.getInternalSpring(0).damping *= 2f;
	/// body.DefaultInternalSpringDamping = 10f;
	/// ~~~
	public float DefaultInternalSpringDamping
	{
		get{ return mDefaultInternalSpringDamp; }
		set
		{
			float oldSpringDamp = mDefaultInternalSpringDamp;
			mDefaultInternalSpringDamp = value;
			
			if(oldSpringDamp != mDefaultInternalSpringDamp)
				setUnchangedInternalSpringConstants(mDefaultInternalSpringK, mDefaultInternalSpringDamp, mDefaultInternalSpringK, oldSpringDamp);
		}
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
	/// <param name="shape">New JelloClosedShape to implement</param>
	/// <param name="options">ShapeSettingOptions for setting the JelloClosedShape.</param>
	public override void setShape (JelloClosedShape shape, ShapeSettingOptions options = ShapeSettingOptions.None)
	{
		base.setShape (shape, options);

		if((options & ShapeSettingOptions.RebuildEdgeSprings) == ShapeSettingOptions.RebuildEdgeSprings)
		{
			clearEdgeSprings();
			buildEdgeSprings();
		}
		else if((options & ShapeSettingOptions.ClearEdgeSprings) == ShapeSettingOptions.ClearEdgeSprings)
		{
			clearEdgeSprings();
		}

		if((options & ShapeSettingOptions.RebuildInternalSprings) == ShapeSettingOptions.RebuildInternalSprings)
		{
			clearInternalSprings();
			BuildInternalSprings();
		}
		else if((options & ShapeSettingOptions.ClearInternalSprings) == ShapeSettingOptions.ClearInternalSprings)
		{
			clearInternalSprings();
		}

		if((options & ShapeSettingOptions.ClearCustomSprings) == ShapeSettingOptions.ClearCustomSprings)
		{
			clearCustomSprings();
		}

		updateSpringDistances();
	}

	public override void smartSetShape (JelloClosedShape shape, ShapeSettingOptions options, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		base.smartSetShape (shape, options, smartOptions);
	}

	protected override void processSmartSetShape (List<int[]> indexPairs, JelloClosedShape shape, ShapeSettingOptions options, SmartShapeSettingOptions smartOptions = SmartShapeSettingOptions.None)
	{
		base.processSmartSetShape (indexPairs, shape, options, smartOptions);

		List<int[]> indexPairsQueue = new List<int[]>();
		List<JelloSpring> tempSprings = new  List<JelloSpring>();

		if((options & ShapeSettingOptions.RebuildEdgeSprings) == ShapeSettingOptions.RebuildEdgeSprings)
		{
			clearEdgeSprings();
			buildEdgeSprings();
		}
		else if((options & ShapeSettingOptions.ClearEdgeSprings) == ShapeSettingOptions.ClearEdgeSprings)
		{
			clearEdgeSprings();
		}
		else
		{

			//find the first common point to the two shapes
			int index = -1;
			for(int i = 0; i < shape.EdgeVertexCount; i++)
			{
				if(indexPairs[i][1] != -1)
				{
					index = i;
					break;
				}
			}
			
			if(index == -1)//in this case, there are no common points to this shape at all. we can just create new edge springs...
			{
				clearEdgeSprings();
				buildEdgeSprings();
			}
			else
			{
				while(indexPairsQueue.Count < shape.EdgeVertexCount)
				{
					if(index >= shape.EdgeVertexCount)
						index = 0;
					
					indexPairsQueue.Add (indexPairs[index]);
					index++;
				}
				
				//see if edge is intact...
				for(int i = 0; i < shape.EdgeVertexCount; i++)//using edge point mass length, because we only want the edge indices
				{
					int next = i + 1 < shape.EdgeVertexCount ? i + 1 : 0; 
					
					//check if this edge is the same as the last.
					if(indexPairsQueue[i][1] != -1) //old doesnt equal -1
					{																				//then the next should equal this plus 1 or 0 if full wrap around?
						if(indexPairsQueue[next][1] == (indexPairsQueue[i][1] + 1 < mBaseShape.EdgeVertexCount ? indexPairsQueue[i][1] + 1 : 0)) //our edge is preserved from the old shape, lets move our old spring into place...
						{
							bool found = false;
							JelloSpring spring = null;
							
							//first check the expected position.
							if(indexPairsQueue[i][1] < mEdgeSprings.Length)
								spring = mEdgeSprings[indexPairsQueue[i][1]];
							
							if(spring != null && spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == indexPairsQueue[next][1])
								found = true;
							
							//if not in the expected position, check the rest of the positions...
							if(!found)
							{
								for(int a = 0; a < mEdgeSprings.Length; a++)
								{
									spring = mEdgeSprings[a];
									
									if(spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == indexPairsQueue[next][1])
									{
										found = true;
										break;
									}
								}
							}
							
							if(!found)//the spring could not be found, create a new one.
							{
								//float dist = Vector2.Distance(shape.EdgeVertices[indexPairsQueue[i][1]], shape.EdgeVertices[indexPairsQueue[next][1]]);
								spring = new JelloSpring(indexPairsQueue[i][1], indexPairsQueue[next][1], 0f, DefaultEdgeSpringStiffness, DefaultEdgeSpringDamping);
							}
							
							spring.pointMassA = indexPairsQueue[i][0];
							spring.pointMassB = indexPairsQueue[next][0];
							tempSprings.Add(spring);
						}
						else
						{	
							if(indexPairsQueue[next][1] == -1)//this is a new point.
							{
								//lets check if there is a spring here...
								bool found = false;
								
								
								//first check the expected position.
								JelloSpring spring = null;
								if(indexPairsQueue[i][1] < mEdgeSprings.Length)
									spring = mEdgeSprings[indexPairsQueue[i][1]];
								
								if(spring != null && (spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == (spring.pointMassA == mBaseShape.EdgeVertexCount - 1 ? spring.pointMassA + 1 : 0)))
								{
									found = true;
								}
								
								//we didnt find the spring in the expected position, lets look through the rest of the springs.
								if(!found)
								{
									for(int a = 0; a < mEdgeSprings.Length; a++)
									{
										spring = mEdgeSprings[a];
										
										if(spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == (spring.pointMassA == mBaseShape.EdgeVertexCount - 1 ? spring.pointMassA + 1 : 0))
										{
											found = true;
											break;
										}
									}
								}
								
								//no old spring found, lets create one...
								if(!found)
								{
									spring = new JelloSpring(indexPairsQueue[i][0], indexPairsQueue[next][0], 0f, DefaultEdgeSpringStiffness, DefaultEdgeSpringDamping);
								}
								
								spring.pointMassA = indexPairsQueue[i][0];
								spring.pointMassB = indexPairsQueue[next][0];
								//spring.length = Vector2.Distance(shape.EdgeVertices[spring.pointMassA], shape.EdgeVertices[spring.pointMassB]);
								
								float multiplier = spring.lengthMultiplier;
								
								
								//first assing spring to this one...
								tempSprings.Add (spring);
								//int nextnext = next + 1 < shape.EdgeVertexCount ? next + 1 : 0;
								
								//now look through the rest of the points until i find a common point and create those springs in the image of this one...
								for(int a = next; a < shape.EdgeVertexCount; a++)
								{
									int nextnext = a + 1 < shape.EdgeVertexCount ? a + 1 : 0;
									
									spring = new JelloSpring(indexPairsQueue[a][0], indexPairsQueue[nextnext][0], 0f, spring.stiffness, spring.damping);
									//spring.length = Vector2.Distance(shape.EdgeVertices[spring.pointMassA], shape.EdgeVertices[spring.pointMassB]);
									spring.lengthMultiplier = multiplier;
									
									tempSprings.Add (spring);
									
									i++;
									
									if(indexPairsQueue[nextnext][1] != -1)
										break;
								}
								
							}
							else//this is a vertex preserved from the old shape... in otherwords, there was a point mass deleted here.
							{
								//lets check if there is a spring here...
								bool found = false;
								
								
								//first check the expected position.
								JelloSpring spring = null;
								if(indexPairsQueue[i][1] < mEdgeSprings.Length)
									spring = mEdgeSprings[indexPairsQueue[i][1]];
								
								if(spring != null && (spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == (spring.pointMassA == mBaseShape.EdgeVertexCount - 1 ? spring.pointMassA + 1 : 0)))
								{
									found = true;
								}
								
								//we didnt find the spring in the expected position, lets look through the rest of the springs.
								if(!found)
								{
									for(int a = 0; a < mEdgeSprings.Length; a++)
									{
										spring = mEdgeSprings[a];
										
										if(spring.pointMassA == indexPairsQueue[i][1] && spring.pointMassB == (spring.pointMassA == mBaseShape.EdgeVertexCount - 1 ? spring.pointMassA + 1 : 0))
										{
											found = true;
											break;
										}
									}
								}
								
								//no old spring found, lets create one...
								if(!found)
								{
									spring = new JelloSpring(indexPairsQueue[i][0], indexPairsQueue[next][0], 0f, DefaultEdgeSpringStiffness, DefaultEdgeSpringDamping);
								}
								
								spring.pointMassA = indexPairsQueue[i][0];
								spring.pointMassB = indexPairsQueue[next][0];
								//spring.length = Vector2.Distance(shape.EdgeVertices[spring.pointMassA], shape.EdgeVertices[spring.pointMassB]);
								
								tempSprings.Add (spring);
							}
						}
					}
					else
					{
						JelloSpring spring = new JelloSpring(indexPairsQueue[i][0], indexPairsQueue[next][0], 0f, DefaultEdgeSpringStiffness, DefaultEdgeSpringDamping);
						//spring.length = Vector2.Distance(shape.EdgeVertices[spring.pointMassA], shape.EdgeVertices[spring.pointMassB]);
						
						tempSprings.Add (spring);
					}
				}
				
				mEdgeSprings = new JelloSpring[tempSprings.Count];
				int indexOffset = 0;
				
				for(int i = 0; i < tempSprings.Count; i++)
				{
					if(tempSprings[i].pointMassA == 0)
					{
						indexOffset = i;
						break;
					}
				}
				
				JelloSpring[] tempArray = new JelloSpring[tempSprings.Count];
				
				for(int i = 0; i < tempArray.Length; i++)
				{
					int a = i + indexOffset;
					if(a >= tempArray.Length)
						a -= tempArray.Length;
					
					tempArray[i] = tempSprings[a];
				}
				
				clearEdgeSprings();
				addSprings(tempArray, ref mEdgeSprings);
			}
		}


		if((options & ShapeSettingOptions.RebuildInternalSprings) == ShapeSettingOptions.RebuildInternalSprings)
		{
			clearInternalSprings();
			BuildInternalSprings();
		}
		else if((options & ShapeSettingOptions.ClearInternalSprings) == ShapeSettingOptions.ClearInternalSprings)
		{
			clearInternalSprings();
		}
		else if(mInternalSprings.Length > 0)
		{

			//now handle internal springs
			int[] tris = shape.Triangles;
			tempSprings.Clear();
			
			for(int i = 0; i < tris.Length; i+=3)
			{
				for(int t = 0; t < 3; t++)
				{
					int r = t + 1 < 3 ? t + 1 : 0;
					
					if(tris[ i + t ] < shape.EdgeVertexCount && tris[ i + r] < shape.EdgeVertexCount) //dont build edge springs
					{
						if(tris[ i + t ] != 0 && tris[ i + r ] != 0)
						{
							if(Mathf.Abs( tris[ i + t ] - tris[ i + r ] ) == 1)
							{
								continue;
							} 
						}
						else if(tris[ i + t ] == shape.EdgeVertexCount - 1 || tris[ i + r ] == shape.EdgeVertexCount - 1 || tris[ i + t ] == 1 || tris[ i + t ] == 1)
						{
							continue;
						}
					}
					
					bool exists = false;
					for(int a = 0; a < tempSprings.Count; a++)
					{
						if((tris[i + t] == tempSprings[a].pointMassA && tris[i + r] == tempSprings[a].pointMassB) || (tris[i + t] == tempSprings[a].pointMassB && tris[i + r] == tempSprings[a].pointMassA))
						{
							exists = true;
							break;
						}
					}
					
					if(exists)
						continue;
					else
						tempSprings.Add
							(
								new JelloSpring
								(
								tris[i + t], 
								tris[i + r], 
								0f,
								mDefaultInternalSpringK,
								mDefaultInternalSpringDamp
								)
								);
				}
			}
			
			//now compare our new internal springs to our old internal springs...
			for(int i = 0; i < tempSprings.Count; i++)
			{
				JelloSpring spring = tempSprings[i];
				int pairA = -1;
				int pairB = -1;
				
				for(int a = 0; a < indexPairs.Count; a++)
				{
					if(indexPairs[a][0] == spring.pointMassA)
						pairA = a;
					if(indexPairs[a][0] == spring.pointMassB)
						pairB = a;
					
					if(pairA != -1 && pairB != -1)
						break;
				}
				
				if(pairA == -1 || pairB == -1)
				{
					//this shouldnt be possible
					continue;
				}
				
				//check if there is an old point assosiated with each spring end point
				if(indexPairs[pairA][1] != -1 && indexPairs[pairB][1] != -1)
				{
					JelloSpring oldSpring;
					for(int a = 0; a < mInternalSprings.Length; a++)
					{
						oldSpring = mInternalSprings[a];
						
						if((oldSpring.pointMassA == indexPairs[pairA][1] && oldSpring.pointMassB  == indexPairs[pairB][1]) || (oldSpring.pointMassB == indexPairs[pairA][1] && oldSpring.pointMassA  == indexPairs[pairB][1]))
						{
							spring.damping = oldSpring.damping;
							spring.lengthMultiplier = oldSpring.lengthMultiplier;
							spring.stiffness = oldSpring.stiffness;
						}
					}
				}
			}
			
			//now set our new internal springs.
			clearInternalSprings();
			
			if(tempSprings.Count > 0)
				addSprings(tempSprings.ToArray(), ref mInternalSprings);
		}

		if((options & ShapeSettingOptions.ClearCustomSprings) == ShapeSettingOptions.ClearCustomSprings)
		{
			clearCustomSprings();
		}
		else
		{

			tempSprings.Clear();
			
			bool rebuildCustomSprings = (smartOptions & SmartShapeSettingOptions.RebuildInvalidatedCustomSprings) == SmartShapeSettingOptions.RebuildInvalidatedCustomSprings;
			
			//now handle custom springs
			for(int i = 0; i < mCustomSprings.Length; i++)
			{
				JelloSpring spring = mCustomSprings[i];
				
				int indexA = -1;
				int indexB = -1;
				
				for(int a = 0; a < indexPairs.Count; a++)
				{
					if(spring.pointMassA == indexPairs[a][1])
						indexA = indexPairs[a][0];
					if(spring.pointMassB == indexPairs[a][1])
						indexB = indexPairs[a][0];
					
					if(indexA != -1 && indexB != -1)
					{
						break;
					}
				}
				
				//here
				if(indexA == -1 || indexB == -1)
				{
					if(rebuildCustomSprings)
					{
						Vector2[] fullShape = new Vector2[shape.VertexCount];
						for(int c = 0; c < shape.VertexCount; c++)
							fullShape[c] = shape.getVertex(c);
						
						if(indexA == -1)
						{
							//rebuild the spring
							Vector2 pos = mBaseShape.getVertex(spring.pointMassA);
							
							int[] closestIndices = JelloShapeTools.GetClosestIndices(pos, fullShape, 2);
							
							//check if any of the indices are already in use
							for(int c = 0; c < closestIndices.Length; c++)
							{
								//already in use by index b
								if(indexB == closestIndices[c])
									continue;
								
								indexA = closestIndices[c];
								break;
							}
						}
						if(indexB == -1)
						{
							//rebuild the spring
							Vector2 pos = mBaseShape.getVertex(spring.pointMassB);
							
							int[] closestIndices = JelloShapeTools.GetClosestIndices(pos, fullShape, 2);
							
							//check if any of the indices are already in use
							for(int c = 0; c < closestIndices.Length; c++)
							{
								//already in use by index b
								if(indexA == closestIndices[c])
									continue;
								
								indexB = closestIndices[c];
								break;
							}
						}
					}
					else
					{
						continue;
					}
				}
				
				spring.pointMassA = indexA;
				spring.pointMassB = indexB;
				tempSprings.Add(spring);
			}
			
			clearCustomSprings();
			
			if(tempSprings.Count > 0)
				addSprings(tempSprings.ToArray(), ref mCustomSprings);
		}
	}
	
	#endregion
	    
	#region ACCUMULATING FORCES
	
	/// <summary>
	/// Accumulates the internal forces acting on the JelloPointMass objects of this body.
	/// This is called JelloWorld.Iterations times per Time.FixedUpdate by JelloWorld.update();
	/// </summary>
	/// <param name="deltaTime">Amount of time to calculate forces over.</param>
    public override void accumulateInternalForces(float deltaTime)
    {
		base.accumulateInternalForces(deltaTime);
        // spring forces.
		Vector2 force;
        for (int i = 0; i < mEdgeSprings.Length; i++)//Edge Springs
        {
			force = JelloVectorTools.calculateSpringForce
			(
					getPointMass(mEdgeSprings[i].pointMassA).Position, getPointMass(mEdgeSprings[i].pointMassA).velocity,
					getPointMass(mEdgeSprings[i].pointMassB).Position, getPointMass(mEdgeSprings[i].pointMassB).velocity,
					mEdgeSprings[i].scaledLength * mEdgeSprings[i].lengthMultiplier, 
					mEdgeSprings[i].stiffness, 
					mEdgeSprings[i].damping
			);

			getPointMass(mEdgeSprings[i].pointMassA).force += force;		
			getPointMass(mEdgeSprings[i].pointMassB).force -= force;

        }
		for (int i = 0; i < mInternalSprings.Length; i++)//Internal Springs
		{
			force = JelloVectorTools.calculateSpringForce
				(
					getPointMass(mInternalSprings[i].pointMassA).Position, getPointMass(mInternalSprings[i].pointMassA).velocity,
					getPointMass(mInternalSprings[i].pointMassB).Position, getPointMass(mInternalSprings[i].pointMassB).velocity,
					mInternalSprings[i].scaledLength * mInternalSprings[i].lengthMultiplier, 
					mInternalSprings[i].stiffness, 
					mInternalSprings[i].damping
					);
		
			getPointMass(mInternalSprings[i].pointMassA).force += force;		
			getPointMass(mInternalSprings[i].pointMassB).force -= force;
		}
		for (int i = 0; i < mCustomSprings.Length; i++)//Custom Springs
		{
			force = JelloVectorTools.calculateSpringForce
				(
					getPointMass(mCustomSprings[i].pointMassA).Position, getPointMass(mCustomSprings[i].pointMassA).velocity,
					getPointMass(mCustomSprings[i].pointMassB).Position, getPointMass(mCustomSprings[i].pointMassB).velocity,
					mCustomSprings[i].scaledLength * mCustomSprings[i].lengthMultiplier, 
					mCustomSprings[i].stiffness, 
					mCustomSprings[i].damping
					);
			
			getPointMass(mCustomSprings[i].pointMassA).force += force;		
			getPointMass(mCustomSprings[i].pointMassB).force -= force;
		}



//		//torsion spring forces...
//		float angularK = 200f;
//		for(int i = 0; i < mEdgeSprings.Length; i++)
//		{
//			JelloSpring spring = mEdgeSprings[i];
//			float angle1 = Vector2.Angle(getPointMass(spring.pointMassB).Position - getPointMass(spring.pointMassA).Position, Vector2.right) * Mathf.Deg2Rad;
////			Vector2 v1 = getPointMass(spring.pointMassB).Position - getPointMass(spring.pointMassA).Position;
////			float angle1 = Mathf.PI - Mathf.Atan2(v1.y, v1.x);
//			float angle2 = Vector2.Angle(Shape.getVertex(spring.pointMassB) - Shape.getVertex(spring.pointMassA), Vector2.right)  * Mathf.Deg2Rad;
////			Vector2 v2 = Shape.getVertex(spring.pointMassB) - Shape.getVertex(spring.pointMassA);
////			float angle2 = Mathf.PI - Mathf.Atan2(v2.y, v2.x);
//			float theta = angle2 - angle1;
//			//Debug.Log (theta);
//			float torque = -angularK * theta;
//
//			Vector2 midPoint = (getPointMass(spring.pointMassB).Position + getPointMass(spring.pointMassA).Position) * 0.5f;
//			Vector3 rx = getPointMass(spring.pointMassB).Position - midPoint;
//			force = (Vector2)(Vector3.Cross(new Vector3(0f, 0f, torque), rx) / (Vector3.Dot(rx,rx) != 0f ? Vector3.Dot(rx,rx) : 1f) * rx.magnitude);
//			getPointMass(spring.pointMassB).force += force * 0.5f;
//			getPointMass(spring.pointMassA).force -= force * 0.5f;
//		}
		
		
		
//		float angularK = 200000f;
//		for(int i = 0; i < mEdgePointMasses.Length; i++)
//		{
//			int next = i + 1 < mEdgePointMasses.Length ? i + 1 : 0;
//			int prev = i - 1 >= 0 ? i : mEdgePointMasses.Length - 1;
//			float currentAngle = Vector2.Angle(mEdgePointMasses[prev].Position - mEdgePointMasses[i].Position, mEdgePointMasses[next].Position - mEdgePointMasses[i].Position) * Mathf.Deg2Rad;
////			if(Vector3.Cross(mEdgePointMasses[prev].Position - mEdgePointMasses[i].Position, mEdgePointMasses[next].Position - mEdgePointMasses[i].Position).z < 0f)
////				currentAngle *= -1f;
//			float baseAngle = Vector2.Angle(mBaseShape.EdgeVertices[prev] - mBaseShape.EdgeVertices[i], mBaseShape.EdgeVertices[next] - mBaseShape.EdgeVertices[i]) * Mathf.Deg2Rad;
////			if(Vector3.Cross(mBaseShape.EdgeVertices[prev] - mBaseShape.EdgeVertices[i], mBaseShape.EdgeVertices[next] - mBaseShape.EdgeVertices[i]).z < 0f)
////				baseAngle *= -1f;
//			float theta = baseAngle - currentAngle;
//			//Debug.Log ("current angle: " + currentAngle + " - base angle: " + baseAngle + " = thetha: " + theta);
//			
//			
//			
////			thisAngle = Vector2.Angle(mBaseShape.EdgeVertices[i], (mEdgePointMasses[i].Position - Position));
////			if(Vector3.Cross(mBaseShape.EdgeVertices[i], (mEdgePointMasses[i].Position - Position)).z < 0f)
////				thisAngle *= -1f;
//			
//			
//			
//			
//			float torque = -angularK * theta;
//			
//			Vector2 midPoint = (mEdgePointMasses[i].Position + mEdgePointMasses[prev].Position) * 0.5f;
//			Vector3 rx = mEdgePointMasses[i].Position - midPoint;
//			//force = (Vector2)(Vector3.Cross(new Vector3(0f, 0f, torque), rx) / (Vector3.Dot(rx,rx) != 0f ? Vector3.Dot(rx,rx) : 1f) * rx.magnitude);
//			float forceScalar = torque / (rx != Vector3.zero ? rx.magnitude : 1);
//			Vector2 normal = JelloVectorTools.getPerpendicular(rx);
//			//Debug.Log(force);
//			mEdgePointMasses[i].force += normal * forceScalar;// * 0.25f;
//			mEdgePointMasses[prev].force -= normal * forceScalar;// * 0.25f;
//			
//			midPoint = (mEdgePointMasses[i].Position + mEdgePointMasses[next].Position) * 0.5f;
//			rx = mEdgePointMasses[next].Position - midPoint;
//			//force = (Vector2)(Vector3.Cross(new Vector3(0f, 0f, torque), rx) / (Vector3.Dot(rx,rx) != 0f ? Vector3.Dot(rx,rx) : 1f) * rx.magnitude);
//			forceScalar = torque / (rx != Vector3.zero ? rx.magnitude : 1);
//			normal = JelloVectorTools.getPerpendicular(rx);
//			//Debug.Log(force);
//			mEdgePointMasses[next].force += normal * forceScalar;// * 0.25f;
//			mEdgePointMasses[i].force -= normal * forceScalar;// * 0.25f;
//		}


        // shape matching springs aren't ever created like all other cases, but rather are simulated between the current point mass positions
		// and their respective base shape positions (transformed into world space). The spring is attempting to bring the distance between these
		// two points to zero.
        if (mShapeMatchingOn && mShapeSpringK > 0f)//Shape Matching Springs
        {
            for (int i = 0; i < PointMassCount; i++)
            {
				Vector2 pos = myTransform.TransformPoint(mBaseShape.getVertex(i));
				Vector2 prevPos = JelloVectorTools.transformVector(mBaseShape.getVertex (i), prevPosition, Scale, mLastAngle);
				Vector2 thisVelocity = (pos - prevPos) / deltaTime;

                force = JelloVectorTools.calculateSpringForce(getPointMass(i).Position, getPointMass(i).velocity,
					pos,
	             	thisVelocity,
              		0f, mShapeSpringK, mShapeSpringDamp);
																										
				//get globabl shape i velocity
				getPointMass(i).force += force * getPointMass(i).shapeMatchingMultiplier;
            }     
        }
    }
	
    #endregion
	
	#region HELPER  FUNCTIONS

	/// <summary>
	/// Modifies the solidity of this JellBody by a given percentage.
	/// Affects JelloSpring.stifness, JelloSpring.damping, ShapeMatchingStiffness an ShapeMatchingDamping.
	/// All JelloSpring.damping values are modified by one tenth of the percent passed in.
	/// </summary>
	/// <param name="percent">Percent to modify solidity by. Clamped between -100 and 100.</param>
	public override void modifySolidityByPercent(float percent)
	{
		base.modifySolidityByPercent(percent);

		percent = Mathf.Clamp(percent, -100f, 100f) / 100f;

		for(int i = 0; i < SpringCount; i++)
		{
			if(getSpring(i).stiffness <= 0f && percent > 0)
				getSpring(i).stiffness = 1f;
			else
				getSpring(i).stiffness *= 1 + percent;

			if(getSpring(i).damping <= 0f && percent > 0)
				getSpring(i).damping = 1f;
			else
				getSpring(i).damping *= 1 + percent * 0.1f;
		}

		if(mDefaultCustomSpringK <= 0f && percent > 0)
			mDefaultCustomSpringK = 1f;
		else
			mDefaultCustomSpringK *= 1 + percent;

		if(mDefaultCustomSpringDamp <= 0f && percent > 0)
			mDefaultCustomSpringDamp = 1f;
		else
			mDefaultCustomSpringDamp *= 1 + percent * 0.1f;

		if(mDefaultInternalSpringK <= 0f && percent > 0)
			mDefaultInternalSpringK = 1f;
		else
			mDefaultInternalSpringK *= 1 + percent;
		
		if(mDefaultInternalSpringDamp <= 0f && percent > 0)
			mDefaultInternalSpringDamp = 1f;
		else
			mDefaultInternalSpringDamp *= 1 + percent * 0.1f;

		if(mDefaultEdgeSpringK <= 0f && percent > 0)
			mDefaultEdgeSpringK = 1f;
		else
			mDefaultEdgeSpringK *= 1 + percent;

		if(mDefaultEdgeSpringDamp <= 0f && percent > 0)
			mDefaultEdgeSpringDamp = 1f;
		else
			mDefaultEdgeSpringDamp *= 1 + percent * 0.1f;

		if(mShapeSpringK <= 0f && percent > 0)
			mShapeSpringK = 1f;
		else
			mShapeSpringK *= 1 + percent;

		if(mShapeSpringDamp <= 0f && percent > 0)
			mShapeSpringDamp = 1f;
		else
			mShapeSpringDamp *= 1 + percent * 0.1f;
	}

	/// <summary>
	/// Clears every internal JelloPointMass.
	/// Will also clear every internal JelloSpring an any custom JelloSpring that is no longer valid.
	/// </summary>
	/// <param name="recenterBaseShape">If set to <c>true</c> recenter the JelloBody.Shape.</param>
	public override void clearInternalPointMasses (bool recenterBaseShape)
	{
		base.clearInternalPointMasses (recenterBaseShape);

		mInternalSprings = new JelloSpring[0];

		bool[] validity = new bool[mCustomSprings.Length];
		int num = 0;
		for(int i = 0; i < mCustomSprings.Length; i++)
		{
			if(mCustomSprings[i].pointMassA >= mEdgePointMasses.Length || mCustomSprings[i].pointMassB >= mEdgePointMasses.Length)
			{
				validity[i] = false;
			}
			else
			{
				validity[i] = true;
				num++;
			}
		}

		JelloSpring[] temp = new JelloSpring[num];
		num = 0;
		for(int i = 0; i < mCustomSprings.Length; i++)
		{
			if(validity[i])
			{
				temp[i - num] = mCustomSprings[i];
			}
			else
			{
				num++;
			}
		}

		mCustomSprings = temp;
	}

	/// <summary>
	/// Flips the JelloBody horizontally.
	/// Updates indices each JelloSpring.
	/// </summary>
	public override void FlipX ()
	{
		base.FlipX ();
		
		for(int i = 0; i < SpringCount; i++)
		{
			getSpring(i).pointMassA = PointMassCount - getSpring(i).pointMassA - 1;
			getSpring(i).pointMassB = PointMassCount - getSpring(i).pointMassB - 1;
		}
	}
	
	/// <summary>
	/// Flips the JelloBody vertically.
	/// Updates indices of each JelloSpring;
	/// </summary>
	public override void FlipY () 
	{
		base.FlipY ();
		
		for(int i = 0; i < SpringCount; i++)
		{
			getSpring(i).pointMassA = PointMassCount - getSpring(i).pointMassA - 1;
			getSpring(i).pointMassB = PointMassCount - getSpring(i).pointMassB - 1;
		}
	}

	/// <summary>
	/// Clears any invalid subcomponent from the JelloBody.
	/// Will check JelloJoint, JelloAttachPoint, and JelloSpring.
	/// </summary>
	public override void ClearInvalidSubComponents ()
	{
		base.ClearInvalidSubComponents ();

		ClearInvalidSprings();
	}
	
	/// <summary>
	/// Called any time the Scale property is modified
	/// Recalculates the JelloSpring.scaledLength of each JelloSpring;
	/// </summary>
	protected override void HandleScaleModification()
	{
		Vector2 posA;
		Vector2 posB;
		Vector2 diff;
		float dist;
		Vector2 mid;

		for(int i = 0; i < SpringCount; i++) //access all together
		{
			posA = mBaseShape.getVertex(getSpring(i).pointMassA);
			posB = mBaseShape.getVertex(getSpring(i).pointMassB);
			diff = posA - posB; 
			dist = getSpring(i).length * Vector2.Distance(posA, posB) / Vector2.Distance (Vector2.zero, diff);
			mid = (posA + posB) * 0.5f;
			posA = mid + (mid - posA).normalized * dist * 0.5f;
			posB = mid + (mid - posB).normalized * dist * 0.5f;

			getSpring(i).scaledLength = Vector2.Distance
			(
				Vector2.zero,
				new Vector2
				(
					(posA.x - posB.x) * Scale.x,
					(posA.y - posB.y) * Scale.y
				)
			); 
		}
	}
	#endregion

    #region DEBUG VISUALIZATION
	
	/// <summary>
	/// Draws the transformed JelloBody.Shape and each JelloSpring assosiated with this JelloBody.
	/// </summary>
    public override void debugDrawMe()
    {
		base.debugDrawMe();

		Color color = new Color(255f, 0f, 0f, 0.75f);
		
		if(World.showShapeMatching && mShapeMatchingOn)
		{
			Vector3[] debugVerts = new Vector3[mBaseShape.VertexCount];

			for(int i = 0; i < mBaseShape.VertexCount; i++)
				debugVerts[i] = myTransform.TransformPoint(mBaseShape.getVertex(i));

			for (int i = 0; i < PointMassCount; i++)
				Debug.DrawLine(getPointMass(i).Position, debugVerts[i], color);
		}

		if(World.showSprings)
		{
			Vector2 p1;
			Vector2 p2;
		
			for (int i = 0; i <  SpringCount; i++) //access all together
			{
				p1 = getPointMass(getSpring(i).pointMassA).Position;
				p2 = getPointMass(getSpring(i).pointMassB).Position;

				if(i < mEdgeSprings.Length)
					color = new Color(0f, 0f, 255f, 0.75f); //blue
				else
					color = new Color(255f, 0f, 0f, 0.75f);	//red

				Debug.DrawLine
					(
						new Vector3(p1.x, p1.y, transform.position.z), 
						new Vector3(p2.x, p2.y, transform.position.z),
						color
					);

				float dist = getSpring(i).scaledLength * getSpring(i).lengthMultiplier;
				Vector2 mid = (p1 + p2) * 0.5f;
				p1 = mid + (mid - p1).normalized * dist * 0.5f;
				p2 = mid + (mid - p2).normalized * dist * 0.5f;


				if(i < mEdgeSprings.Length)
					color = new Color(255, 0f, 255f, 0.75f); //majenta
				else
					color = new Color(255f, 255f, 0f, 0.75f); //yellow

				Debug.DrawLine
					(
						new Vector3(p1.x, p1.y, transform.position.z), 
						new Vector3(p2.x, p2.y, transform.position.z),
						color
					);
			}
		}
    }

    #endregion
	
    #region SPRINGS



	/// <summary>
	/// Adds a custom JelloSpring.
	/// </summary>
	/// <param name="spring">The new JelloSpring to add.</param>
	public void addCustomSpring (JelloSpring spring)
	{
		addSpring(spring, ref mCustomSprings);
	}

	/// <summary>
	/// Add multiple custom JelloSpring.
	/// </summary>
	/// <param name="springs">JelloSpring array to add to this JelloBody.</param>
	public void addCustomSprings (JelloSpring[] springs)
	{
		addSprings(springs, ref mCustomSprings);
	}


	/// <summary>
	/// Adds a new JelloSpring to the JelloSpring array passed in.
	/// </summary>
	/// <param name="spring">JelloSpring to add.</param>
	/// <param name="addToArray">JelloSpring array to add the new JelloSpring to</param>
	private void addSpring (JelloSpring spring, ref JelloSpring[] addToArray)
	{
		if(spring == null || spring.pointMassA < 0 || spring.pointMassA >= mBaseShape.VertexCount || spring.pointMassB < 0 || spring.pointMassB >= mBaseShape.VertexCount)
			return;

		Vector2 posA = mBaseShape.getVertex(spring.pointMassA);
		Vector2 posB = mBaseShape.getVertex(spring.pointMassB);
		Vector2 diff = posA - posB;
		float dist = spring.length * Vector2.Distance(posA, posB) / Vector2.Distance (Vector2.zero, diff);
		Vector2 mid = (posA + posB) * 0.5f;
		
		mid = (posA + posB) * 0.5f;
		posA = mid + (mid - posA).normalized * dist * 0.5f;
		posB = mid + (mid - posB).normalized * dist * 0.5f;
		
		spring.scaledLength = Vector2.Distance
			(
				Vector2.zero,
				new Vector2
				(
				(posA.x - posB.x) * Scale.x,
				(posA.y - posB.y) * Scale.y
				)
				); 
		
		//create a temporary array of springs at the new size
		JelloSpring[] oldSprings = new JelloSpring[addToArray.Length];
		
		//add springs already in the array to the temp array
		for(int i = 0; i < addToArray.Length; i++)
			oldSprings[i] = addToArray[i];
		
		addToArray = new JelloSpring[addToArray.Length + 1];


		for(int i = 0; i < oldSprings.Length; i++)
			addToArray[i] = oldSprings[i];
			
		addToArray[addToArray.Length - 1] = spring;

	}
	
	/// <summary>
	/// Adds one JelloSpring array to another.
	/// </summary>
	/// <param name="springs">JelloSpring array to be added.</param>
	/// <param name="addToArray">JelloSpring array to add to.</param>
	private void addSprings (JelloSpring[] springs, ref JelloSpring[] addToArray)
	{
		Vector2 posA;
		Vector2 posB;
		Vector2 diff;
		float dist;
		Vector2 mid;

		for(int i = 0; i < springs.Length; i++)
		{
			posA = mBaseShape.getVertex(springs[i].pointMassA);
			posB = mBaseShape.getVertex(springs[i].pointMassB);
			diff = posA - posB; 
			dist = springs[i].length * Vector2.Distance(posA, posB) / Vector2.Distance (Vector2.zero, diff);
			mid = (posA + posB) * 0.5f;
			posA = mid + (mid - posA).normalized * dist * 0.5f;
			posB = mid + (mid - posB).normalized * dist * 0.5f;
			
			springs[i].scaledLength = Vector2.Distance
				(
					Vector2.zero,
					new Vector2
					(
						(posA.x - posB.x) * Scale.x,
						(posA.y - posB.y) * Scale.y
					)
				); 
		}

		//create a temporary array of springs at the new size
		JelloSpring[] oldSprings = new JelloSpring[addToArray.Length];
		
		//add springs already in the array to the temp array
		for(int i = 0; i < addToArray.Length; i++)
			oldSprings[i] = addToArray[i];
		
		addToArray = new JelloSpring[oldSprings.Length + springs.Length];
		
		int a = 0;
		

		for(int i = 0; i < addToArray.Length; i++)
		{
			if(i < oldSprings.Length)
			{
				addToArray[i] = oldSprings[i];
			}
			else
			{
				addToArray[i] = springs[a];
				
				a++;
			}
		}	
	}
	
	/// <summary>
	/// Adds multiple JelloSpring objects with specified values.
	/// </summary>
	/// <param name="indices">
	/// The indices of the JelloPointMass that each JelloSpring will connect to
	/// Each JelloSpring uses two indices.
	/// </param>
	/// <param name="stiffness">The JelloSpring.stiffness of each JelloSpring to be added.</param>
	/// <param name="damping">The JelloSpring.damping of each JelloSpring to be added.</param>
	/// <param name="addToArray">The JelloSpring array to add each new JelloSpring to.</param>
	private void addSprings (int[] indices, float stiffness, float damping, ref JelloSpring[] addToArray)
	{	
		JelloSpring[] newSprings = new JelloSpring[indices.Length / 2];
		//float dist = 0f;

		for(int i = 0; i < newSprings.Length; i ++)
		{
			//dist = Vector2.Distance(mBaseShape.getVertex( indices[ 2 * i ] ), mBaseShape.getVertex( indices[2 * i + 1] ));
			
			newSprings[i] = new JelloSpring(indices[2 * i], indices[2 * i + 1], 0f, stiffness, damping);
		}
		
		addSprings(newSprings, ref addToArray);
	}

	/// <summary>
	/// Removes the JelloSpring from this JelloSpringBody.
	/// Will look at edge, internal, and custom JelloSpring objects.
	/// </summary>
	/// <param name="spring">JelloSpring to remove from this JelloSpringBody.</param>
	public void RemoveSpring(JelloSpring spring)
	{
		for(int i = 0; i < SpringCount; i++)
		{
			if(getSpring(i) == spring)
			{
				if(i < mEdgeSprings.Length)
					removeSpring(spring, ref mEdgeSprings);
				else if(i < mEdgeSprings.Length + mInternalSprings.Length)
					removeSpring(spring, ref mInternalSprings);
				else
					removeSpring(spring, ref mCustomSprings);

				break;
			}
		}
	}

	/// <summary>
	/// Removes the JelloSpring from the given JelloSpring array.
	/// </summary>
	/// <param name="spring">The JelloSpring to be removed.</param>
	/// <param name="removeFromArray">The JelloSpring array to remove the JelloSpring from.</param>
	private void removeSpring (JelloSpring spring, ref JelloSpring[] removeFromArray)
	{
		List<JelloSpring> tempSprings = new List<JelloSpring>();
		for(int i = 0; i < removeFromArray.Length; i++)
			if(spring != removeFromArray[i])
				tempSprings.Add (removeFromArray[i]);
		
		removeFromArray = tempSprings.ToArray();
	} 
	
	/// <summary>
	/// Updates each JelloSpring.length and JelloSpring.scaledLength according to JelloBody.Shape.
	/// </summary>
	private void updateSpringDistances()
	{
		Vector2 posA;
		Vector2 posB;
		Vector2 diff;
		float dist;
		Vector2 mid;

		for(int i = 0; i < SpringCount; i++)
		{
			getSpring(i).length = Vector2.Distance(mBaseShape.getVertex(getSpring(i).pointMassA), mBaseShape.getVertex(getSpring(i).pointMassB));

			posA = mBaseShape.getVertex(getSpring(i).pointMassA);
			posB = mBaseShape.getVertex(getSpring(i).pointMassB);
			diff = posA - posB; 
			dist = getSpring(i).length * Vector2.Distance(posA, posB) / Vector2.Distance (Vector2.zero, diff);
			mid = (posA + posB) * 0.5f;
			posA = mid + (mid - posA).normalized * dist * 0.5f;
			posB = mid + (mid - posB).normalized * dist * 0.5f;
			
			getSpring(i).scaledLength = Vector2.Distance
				(
					Vector2.zero,
					new Vector2
					(
						(posA.x - posB.x) * Scale.x,
						(posA.y - posB.y) * Scale.y
					)
				); 
		}
	}
	
    /// <summary>
    /// Clear each JelloSpring from the body. 
	/// (Edge, Internal, and Custom)
    /// </summary>
    public void clearAllSprings()
    {
        mEdgeSprings = new JelloSpring[0];
		mInternalSprings = new JelloSpring[0];
		mCustomSprings = new JelloSpring[0];
    }
	
	/// <summary>
	/// Clears each edge JelloSpring.
	/// </summary>
	public void clearEdgeSprings()
	{	
		mEdgeSprings = new JelloSpring[0];
	}

	/// <summary>
	/// Clears each internal JelloSpring.
	/// </summary>
	public void clearInternalSprings()
	{	
		mInternalSprings = new JelloSpring[0];
	}

	/// <summary>
	/// Clears each custom JelloSpring.
	/// </summary>
	public void clearCustomSprings()
	{	
		mCustomSprings = new JelloSpring[0];
	}

	/// <summary>
	/// Clears any invalid JelloSpring from the JelloSpringBody.
	/// </summary>
	public void ClearInvalidEdgeSprings()
	{
		if(mEdgeSprings != null)
		{
			List<JelloSpring> springs = new List<JelloSpring>();
			for(int i = 0; i < mEdgeSprings.Length; i++)
			{
				JelloSpring spring = mEdgeSprings[i];
				if(spring.pointMassA < 0 || spring.pointMassA >= EdgePointMassCount || spring.pointMassB < 0 || spring.pointMassB >= EdgePointMassCount)
				{
					Debug.LogWarning(name + " edge JelloSpring # " + i + " is invalid, removing.");
					continue;
				}

				springs.Add (spring);
			}

			mEdgeSprings = springs.ToArray();
		}
	}

	/// <summary>
	/// Clears any invalid internal JelloSpring from the JelloSpringBody.
	/// </summary>
	public void ClearInvalidInternalSprings()
	{
		if(mInternalSprings != null)
		{
			
			List<JelloSpring> springs = new List<JelloSpring>();
			for(int i = 0; i < mInternalSprings.Length; i++)
			{
				JelloSpring spring = mInternalSprings[i];
				if(spring.pointMassA < 0 || spring.pointMassA >= PointMassCount || spring.pointMassB < 0 || spring.pointMassB >= PointMassCount)
				{
					Debug.LogWarning(name + " internal JelloSpring # " + i + " is invalid, removing.");
					continue;
				}
				
				springs.Add (spring);
			}
			
			mInternalSprings = springs.ToArray();
		}
	}

	/// <summary>
	/// Clears any invalid custom JelloSpring from the JelloSpringBody.
	/// </summary>
	public void ClearInvalidCustomSprings()
	{
		if(mCustomSprings != null)
		{
			
			List<JelloSpring> springs = new List<JelloSpring>();
			for(int i = 0; i < mCustomSprings.Length; i++)
			{
				JelloSpring spring = mCustomSprings[i];
				if(spring.pointMassA < 0 || spring.pointMassA >= PointMassCount || spring.pointMassB < 0 || spring.pointMassB >= PointMassCount)
				{
					Debug.LogWarning(name + " custom JelloSpring # " + i + " is invalid, removing.");

					continue;
				}
				
				springs.Add (spring);
			}
			
			mCustomSprings = springs.ToArray();
		}
	}

	/// <summary>
	/// Clears any invalid JelloSpring from the JelloSpringBody.
	/// </summary>
	public void ClearInvalidSprings()
	{
		ClearInvalidEdgeSprings();
		ClearInvalidInternalSprings();
		ClearInvalidCustomSprings();
	}
	
	/// <summary>
	/// Builds a chain of JelloSpring object connecting the perimeter (edge) of the JelloBody.
	/// </summary>
    private void buildEdgeSprings()
    {	
		int[] indices = new int[mEdgePointMasses.Length * 2];
        for (int i = 0; i < mEdgePointMasses.Length; i++)
        {
            if (i < (mEdgePointMasses.Length - 1))
			{
				indices[2 * i] = i;
				indices[2 * i + 1] = i + 1;
			}
            else
			{
				indices[2 * i] = i;
				indices[2 * i + 1] = 0;
			}
        }
		addSprings(indices, mDefaultEdgeSpringK, mDefaultEdgeSpringDamp, ref mEdgeSprings);
    }

	/// <summary>
	/// Builds the JelloSpring objects connecting select perimeter (edge) JelloPointMass objects to each other and internal JelloPointMass objects.
	/// Connections are selected according to the triangulation of the JelloBody.Shape.
	/// </summary>
	public void BuildInternalSprings()
	{	
		int[] tris = mBaseShape.Triangles;
		List<JelloSpring> newSprings = new List<JelloSpring>();

		for(int i = 0; i < tris.Length; i+=3)
		{
			for(int t = 0; t < 3; t++)
			{
				int r = t + 1 < 3 ? t + 1 : 0;

				if(tris[ i + t ] < mBaseShape.EdgeVertexCount && tris[ i + r] < mBaseShape.EdgeVertexCount) //dont build edge springs
				{
					if(tris[ i + t ] != 0 && tris[ i + r ] != 0)
					{
						if(Mathf.Abs( tris[ i + t ] - tris[ i + r ] ) == 1)
						{
							continue;
						} 
					}
					else if(tris[ i + t ] == mBaseShape.EdgeVertexCount - 1 || tris[ i + r ] == mBaseShape.EdgeVertexCount - 1 || tris[ i + t ] == 1 || tris[ i + t ] == 1)
					{
						continue;
					}
				}

				bool exists = false;
				for(int a = 0; a < newSprings.Count; a++)
				{
					if((tris[i + t] == newSprings[a].pointMassA && tris[i + r] == newSprings[a].pointMassB) || (tris[i + t] == newSprings[a].pointMassB && tris[i + r] == newSprings[a].pointMassA))
					{
						exists = true;
						break;
					}
				}
				
				if(exists)
					continue;
				else
					newSprings.Add
						(
							new JelloSpring
							(
								tris[i + t], 
								tris[i + r], 
								0f,
								//Vector2.Distance(Vector2.zero, mBaseShape.getVertex(tris[i + t]) - mBaseShape.getVertex(tris[i + r])),
								mDefaultInternalSpringK,
								mDefaultInternalSpringDamp
							)
						);
			}
		}

		addSprings(newSprings.ToArray(), ref mInternalSprings);
	}
   
	#endregion

    #region ADJUSTING SPRING VALUES
	
    /// <summary>
    /// Change JelloSpring.stiffness and JelloSpring.damping for each perimeter (edge) JelloSpring but only for those that share the input oldStiffness and oldDamping values.
    /// </summary>
    /// <param name="stiffness">new JelloSpring.stiffness</param>
    /// <param name="damping">new JelloSpring.damping</param>
    /// <param name="oldStiffness">old JelloSpring.stiffness</param>
    /// <param name="oldDamping">old JelloSpring.damping</param>
    private void setUnchangedEdgeSpringConstants(float stiffness, float damping, float oldStiffness, float oldDamping )
    {
        // we know that the first n springs in the list are the edge springs.
        for (int i = 0; i < mEdgeSprings.Length; i++)
        {
			if(mEdgeSprings[i].stiffness == oldStiffness && mEdgeSprings[i].damping == oldDamping)
			{
				mEdgeSprings[i].stiffness = stiffness;
				mEdgeSprings[i].damping = damping;
			}
        }
    }
	
	/// <summary>
    /// Change the JelloSpring.stiffness and JelloSpring.damping for each perimeter(edge) JelloSpring.
    /// </summary>
    /// <param name="stiffness">The new JelloSpring.stiffness.</param>
    /// <param name="damping">The new JelloSpring.damping.</param>
    public void setEdgeSpringConstants(float stiffness, float damping)
    {
        // we know that the first n springs in the list are the edge springs.
		for (int i = 0; i < mEdgeSprings.Length; i++)
        {
			mEdgeSprings[i].stiffness = stiffness;
			mEdgeSprings[i].damping = damping;
        }
    }

	/// <summary>
	/// Change JelloSpring.stiffness and JelloSpring.damping for each internal JelloSpring but only for those that share the input oldStiffness and oldDamping values.
	/// </summary>
	/// <param name="stiffness">new JelloSpring.stiffness</param>
	/// <param name="damping">new JelloSpring.damping</param>
	/// <param name="oldStiffness">old JelloSpring.stiffness</param>
	/// <param name="oldDamping">old JelloSpring.damping</param>
	private void setUnchangedInternalSpringConstants(float stiffness, float damping, float oldStiffness, float oldDamping )
	{
		// we know that the first n springs in the list are the edge springs.
		for (int i = 0; i < mInternalSprings.Length; i++)
		{
			if(mInternalSprings[i].stiffness == oldStiffness && mInternalSprings[i].damping == oldDamping)
			{
				mInternalSprings[i].stiffness = stiffness;
				mInternalSprings[i].damping = damping;
			}
		}
	}
	
	/// <summary>
	/// Change the JelloSpring.stiffness and JelloSpring.damping for each internal JelloSpring.
	/// </summary>
	/// <param name="stiffness">The new JelloSpring.stiffness.</param>
	/// <param name="damping">The new JelloSpring.damping.</param>
	public void setInternalSpringConstants(float stiffness, float damping)
	{
		// we know that the first n springs in the list are the edge springs.
		for (int i = 0; i < mInternalSprings.Length; i++)
		{
			mInternalSprings[i].stiffness = stiffness;
			mInternalSprings[i].damping = damping;
		}
	}

	/// <summary>
	/// Change JelloSpring.stiffness and JelloSpring.damping for each custom JelloSpring but only for those that share the input oldStiffness and oldDamping values.
	/// </summary>
	/// <param name="stiffness">new JelloSpring.stiffness</param>
	/// <param name="damping">new JelloSpring.damping</param>
	/// <param name="oldStiffness">old JelloSpring.stiffness</param>
	/// <param name="oldDamping">old JelloSpring.damping</param>
	private void setUnchangedCustomSpringConstants(float stiffness, float damping, float oldStiffness, float oldDamping)
	{
		for(int i = 0; i < mCustomSprings.Length; i++)
		{
			if(mCustomSprings[i].stiffness == oldStiffness && mCustomSprings[i].damping == oldDamping)
			{
				mCustomSprings[i].stiffness = stiffness;
				mCustomSprings[i].damping = damping;
			}
		}
	}
	
	/// <summary>
	/// Change the JelloSpring.stiffness and JelloSpring.damping for each custom JelloSpring.
	/// </summary>
	/// <param name="stiffness">The new JelloSpring.stiffness.</param>
	/// <param name="damping">The new JelloSpring.damping.</param>
    public void setCustomSpringConstants(float stiffness, float damping)
    {
		for(int i = 0; i < mCustomSprings.Length; i++)
		{	
			mCustomSprings[i].stiffness = stiffness;
			mCustomSprings[i].damping = damping;
		}
    }
	
    #endregion
	
	#region SPRING ACCESS

	/// <summary>
	/// Gets the JelloSpring at specefied index.
	/// Arrays are accessed in order of Edge, Internal, then custom.
	/// </summary>
	/// <returns>The JelloSpring at index. Null if not found.</returns>
	/// <param name="index">Index</param>
	public JelloSpring getSpring(int index)
	{
		if(index >= 0)
		{

			if(index < mEdgeSprings.Length)
				return mEdgeSprings[index];
			else if(index < mEdgeSprings.Length + mInternalSprings.Length)
				return mInternalSprings[index - mEdgeSprings.Length];
			else if(index < mEdgeSprings.Length + mInternalSprings.Length + mCustomSprings.Length)
				return mCustomSprings[index - mEdgeSprings.Length - mInternalSprings.Length];
			else
				return null;
		}	
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Gets the edge JelloSpring at a specefied index.
	/// </summary>
	/// <returns>The edge JelloSpring, null if not found.</returns>
	/// <param name="index">The index.</param>
	public JelloSpring getEdgeSpring(int index) //TODO check references to this that may need to be changed.
	{
		if(index >= 0 && index < mEdgeSprings.Length)
			return mEdgeSprings[index];
		else
			return null;
	}

	/// <summary>
	/// Gets the internal JelloSpring array.
	/// </summary>
	/// <returns>The internal JelloSpring array.</returns>
	public JelloSpring[] getInternalSprings()
	{
		return mInternalSprings;
	}

	/// <summary>
	/// Sets the internal JelloSpring array.
	/// </summary>
	/// <param name="springs">JelloSpring array to replace the current internal JelloSpring array.</param>
	public void setInternalSprings(JelloSpring[] springs)
	{
		mInternalSprings = springs;
	}

	/// <summary>
	/// Gets the custom JelloSpring at a specefied index.
	/// </summary>
	/// <returns>The custom JelloSpring, null if not found.</returns>
	/// <param name="index">The index.</param>
	public JelloSpring getCustomSpring(int index)
	{
		if(index >= 0 && index < mCustomSprings.Length)
			return mCustomSprings[index];
		else
			return null;
	}

	/// <summary>
	/// Gets the internal JelloSpring at a specefied index.
	/// </summary>
	/// <returns>The internal JelloSpring, null if not found.</returns>
	/// <param name="index">The index.</param>
	public JelloSpring getInternalSpring(int index)
	{
		if(index >= 0 && index < mInternalSprings.Length)
			return mInternalSprings[index];
		else
			return null;
	}

	
	#endregion

	#if UNITY_EDITOR

	//TODO double check to see if i still need to do this or even if others are missing from this.
	/// <summary>
	/// Validate this instance.
	/// Used in editor to ensure logic inside seters are called.
	/// </summary>
	protected override void Validate()
	{
		base.Validate();

		DefaultEdgeSpringStiffness = mDefaultEdgeSpringK;
		DefaultEdgeSpringDamping = mDefaultEdgeSpringDamp;
		DefaultCustomSpringStiffness = mDefaultCustomSpringK;
		DefaultCustomSpringDamping = mDefaultCustomSpringDamp;
		DefaultInternalSpringStiffness = mDefaultInternalSpringK;
		DefaultInternalSpringDamping = mDefaultInternalSpringDamp;
	}
	
	#endif
}