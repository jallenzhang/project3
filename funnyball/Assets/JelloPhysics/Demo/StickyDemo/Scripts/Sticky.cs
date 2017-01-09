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
using System.Collections.Generic;

/// <summary>
/// Sticky.
/// Adds JelloJoints to the body on collision.
/// Because the joints are breakable this creates a sticky effect.
/// </summary>
public class Sticky : MonoBehaviour 
{
	/// <summary>
	/// The velocity at which the joints are broken.
	/// </summary>
	public float breakVelocity = 5f;

	/// <summary>
	/// Whether or not to create joints at the points of this body.
	/// </summary>
	public bool stickAtPoints = true;

	/// <summary>
	/// Wheter or not to create joints along the edges of this body.
	/// </summary>
	public bool stickAtEdges = false;

	/// <summary>
	/// The body to make sticky.
	/// </summary>
	private JelloBody body;

	/// <summary>
	/// The joints created by this script.
	/// </summary>
	private List<JelloJoint> joints = new List<JelloJoint>();

	// Use this for initialization
	void Start () 
	{
		//cache body
		body = GetComponent<JelloBody>();

		//subscribe the body's collision events
		body.JelloCollisionEvent += ProcessCollisionEvent;
	}

	void OnGUI()
	{
		//Allow for adjusting the break velocity at runtime.
		GUILayout.Space(20f);
		GUILayout.Label("Stickyness: " + breakVelocity);
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("+"))
		{
			breakVelocity++;
			for(int i = 0; i < joints.Count; i++)
			{
				//update all other joints as well.
				joints[i].breakVelocity = breakVelocity;
			}
		}
		if(GUILayout.Button("-") && breakVelocity > 0f)
		{
			breakVelocity--;
			for(int i = 0; i < joints.Count; i++)
			{
				//update all other joints as well
				joints[i].breakVelocity = breakVelocity;
			}
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Processes the collision event.
	/// </summary>
	/// <param name="jelloCollision">Jello collision.</param>
	private void ProcessCollisionEvent(JelloCollision jelloCollision)
	{
		if(!stickAtPoints && !stickAtEdges)
			return;

		//loop through each contact and see if a joint exists.
		//if not, create the joint.
		for(int i = 0; i < jelloCollision.contacts.Length; i++)
		{
			JelloContact contact = jelloCollision.contacts[i];

			bool skip = true;
			if(stickAtPoints && body == contact.bodyA)
				skip = false;
			if(stickAtEdges && body == contact.bodyB)
				skip = false;
			if(skip)
				continue;

			bool found = false;

			//see if this joint already exists
			for(int c = joints.Count - 1; c >= 0; c--)
			{
				JelloJoint joint = joints[c];

				//remove joints that have been destroyed
				if(joint.destroyed)
				{
					joints.Remove(joint);
					continue;
				}

				//i only want to know if there is a joint between the two bodies and if the point or edge is already taken.
				//should only need to use transform...?
				if(joint.TransformA != contact.transformA && joint.TransformB != contact.transformA)
					continue;
				if(joint.TransformA != contact.transformB && joint.TransformB != contact.transformB)
					continue;

				//at this point we know that they share the same transforms.

				//we also know that one of the bodies is the body that this script is attached to.


				if(contact.bodyA != null)
				{
					if(stickAtPoints && joint.bodyA == contact.bodyA) //this matters if i am alowing point joints
					{
						if(joint.affectedIndicesA.Length != 1) //joint bodya has multiple anchors
						{
							continue;
						}
						else if(joint.affectedIndicesA[0] != contact.bodyApm) //joint bodyA has a single anchor and is not the same
						{
							continue;
						}
					}

					//where am i handling rigidbodies or even static bodies??

					if(stickAtEdges && joint.bodyB != null)
					{
						if(joint.bodyB == contact.bodyA)//this matters if i am allowing edge joints
						{
							if(joint.affectedIndicesB.Length != 1)
							{
								continue;
							}
							else if(joint.affectedIndicesB[0] != contact.bodyApm)
							{
								continue;
							}
						}
					}
				}

				if(contact.bodyB != null)
				{
					if(stickAtEdges && joint.bodyA == contact.bodyB)
					{
						if(joint.affectedIndicesA.Length != 2)
						{
							continue;
						}
						else 
						{
							if(joint.affectedIndicesA[0] != contact.bodyBpmA && joint.affectedIndicesA[0] != contact.bodyBpmB)
							{	
								continue;
							}
							if(joint.affectedIndicesA[1] != contact.bodyBpmA && joint.affectedIndicesA[1] != contact.bodyBpmB)
							{
								continue;
							}
							if(joint.bodyB != null)
							{
								if(joint.affectedIndicesB.Length != 1)
								{
									continue;
								}
								else if(joint.affectedIndicesB[0] != contact.bodyApm)
								{
									continue;
								}
							}
						}
					}
					
					
					if(stickAtPoints && joint.bodyB != null)
					{
						if(joint.bodyB == contact.bodyB)
						{
							if(joint.affectedIndicesB.Length != 2)
							{
								continue;
							}
							else 
							{
								if(joint.affectedIndicesB[0] != contact.bodyBpmA && joint.affectedIndicesB[0] != contact.bodyBpmB)
								{	
									continue;
								}
								if(joint.affectedIndicesB[1] != contact.bodyBpmA && joint.affectedIndicesB[1] != contact.bodyBpmB)
								{
									continue;
								}
								if(joint.bodyA != null)
								{
									if(joint.affectedIndicesA.Length != 1)
									{
										continue;
									}
									else if(joint.affectedIndicesA[0] != contact.bodyApm)
									{
										continue;
									}
								}
								//else check if ra is the same?
							}
						}
					}
				}


				//must be the same
				found = true;
				break;
			}

			if(!found)//joint Doesn't exist, so create it!
			{
				if(!stickAtPoints && contact.transformA == transform)
					continue;
				if(!stickAtEdges && contact.transformB == transform)
					continue;

				JelloJoint joint = new JelloJoint();

				joint.TransformA = contact.transformA;
				joint.localAnchorA = joint.TransformA.InverseTransformPoint(contact.hitPoint);
				if(joint.bodyA != null)
				{
					int[] indices = new int[1];
					Vector2[] vertices = new Vector2[1];

					indices[0] = contact.bodyApm;
					vertices[0] = contact.bodyA.getPointMass(indices[0]).LocalPosition;
					joint.RebuildAnchor(contact.transformA.InverseTransformPoint(contact.hitPoint), true, false, indices, vertices);
				}

				joint.TransformB = contact.transformB;
				joint.localAnchorB = joint.TransformB.InverseTransformPoint(contact.hitPoint);
				if(contact.bodyB != null)
				{
					int[] indices = new int[2];
					Vector2[] vertices = new Vector2[2];

					indices[0] = contact.bodyBpmA;
					indices[1] = contact.bodyBpmB;

					vertices[0] = contact.bodyB.getPointMass(indices[0]).LocalPosition;
					vertices[1] = contact.bodyB.getPointMass(indices[1]).LocalPosition;
					joint.RebuildAnchor(joint.localAnchorB, false, false, indices, vertices);
				}

				joint.breakable = true;
				joint.breakVelocity = breakVelocity;
				body.AddJoint (joint);
				joints.Add(joint);
			}
		}
	}
}
