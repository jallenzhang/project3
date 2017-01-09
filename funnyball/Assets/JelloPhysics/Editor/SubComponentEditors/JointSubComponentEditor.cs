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
using UnityEditor;

public class JointSubComponentEditor : SubComponentEditor 
{
	GUIContent snapJointContent = new GUIContent("Snap");
	GUIContent addJointLegContent = new GUIContent("+");
	GUIContent removeJointLegContent = new GUIContent("-");
	GUIContent jointBreakVelocityContent = new GUIContent("Break Velocity");
	GUIContent jointAnchorBcontent = new GUIContent("Global Position");
	GUIContent breakableContent = new GUIContent("Breakable");
	public GUIContent addJointContent = new GUIContent ("Add Joint");
	public GUIContent deleteJointContent = new GUIContent ("X");//, "Delete this AttachPoint");

	SerializedProperty eJoints;


	public JointSubComponentEditor(Editor editor) : base (editor)
	{
		name = "Joints";
		handlePositions = new Vector3[8];
		handleSizes = new float[8];

		eJoints = editor.serializedObject.FindProperty("mJoints");
	}

	public override void DrawEditorGUI ()
	{
		//mainEditor.serializedObject.Update();

		multiEditing = mainEditor.serializedObject.isEditingMultipleObjects;

		if(multiEditing)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.HelpBox("Joints may not be edited when multiple GameObjects are selected", MessageType.Info);
			EditorGUI.indentLevel--;
			return;
		}


		EditorGUI.indentLevel++;
		
		if(GUILayout.Button(addJointContent, EditorStyles.miniButton))
		{
			newSubComponentState = AddSubComponentState.initiated;
		}

		if(newSubComponentState == AddSubComponentState.initiated)
		{
			EditorGUILayout.HelpBox("Click anywhere around the Jello Body" +
			                        "\nEsc to cancel", MessageType.Info);
		}

		string text = "";
		string tooltip = "";

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));

		for(int i = 0; i < body.JointCount; i++)
		{
			text = "Joint # " + i;
			
			tooltip = text;
//				tooltip = text +
//					"\nfirst index = " + body.GetAttachPoint(i).triangleIndices[0];
//				tooltip = text +
//					"\nsecond index = " + body.GetAttachPoint(i).triangleIndices[1];
//				tooltip = text +
//					"\nthird index = " + body.GetAttachPoint(i).triangleIndices[2];
			
			EditorGUILayout.BeginHorizontal();

			SerializedProperty eJoint = eJoints.GetArrayElementAtIndex(i);

			//bool foldout = editIndex == i;
			eJoint.isExpanded = editIndex == i;

			EditorGUI.BeginChangeCheck();
			//foldout = EditorGUILayout.Foldout(foldout, new GUIContent(text, tooltip)); 
			EditorGUILayout.PropertyField(eJoint, new GUIContent(text, tooltip));
			if(EditorGUI.EndChangeCheck())
			{
				if(!eJoint.isExpanded)
					editIndex = -1;
				else if(eJoint.isExpanded)
				{
					editIndex = i;
					SetEditIndex(i);
					SceneView.RepaintAll();
				}
			}
		
			if(GUILayout.Button(deleteJointContent, EditorStyles.miniButton, GUILayout.Width(20)))
			{
				body.RemoveJoint(body.GetJoint(i));
				
				if(i == editIndex)
					editIndex = -1;
				if(i == drawIndex)
					drawIndex = -1;

				EditorUtility.SetDirty(body);

				break;
			}
			
			EditorGUILayout.EndHorizontal();
			
			if(Event.current.type == EventType.Repaint && GUI.tooltip != prevTooltip)
			{
				//mouse out
				if(prevTooltip != "")
					drawIndex = -1;
				
				//mouse over
				if(GUI.tooltip != "")
					drawIndex =  i;//?
				
				prevTooltip = GUI.tooltip;
				
				SceneView.RepaintAll();
			}
			
			if(eJoint.isExpanded)
			{
				DrawEditJointGUI();
			}
		}

		EditorGUILayout.EndScrollView();
			
		EditorGUI.indentLevel--;
		
		if(newSubComponentState == AddSubComponentState.initiated)
		{
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			{
				newSubComponentState = AddSubComponentState.inactive;
				mainEditor.Repaint();
			}
		}

		//mainEditor.serializedObject.ApplyModifiedProperties();
	}

	public override void DrawSceneGUI ()
	{
		if(multiEditing)
			return;

		DrawjointSceneGUI();
	}


	public override void SetEditIndex(int index)
	{
		JelloJoint joint = body.GetJoint(index);
		editIndex = index;
		int numA = joint.affectedIndicesA.Length;
		handlePositions[0] = handlePositions[1] = Vector2.zero;

		for(int i = 0; i < numA; i++)
		{
			handlePositions[i + 2] = body.Shape.getVertex(joint.affectedIndicesA[i]);
			handlePositions[0] += handlePositions[i + 2] * joint.scalarsA[i];
		}
		
		if(joint.TransformB != null)
		{
			if(joint.bodyB != null)
			{
				for(int i = 0; i < joint.affectedIndicesB.Length; i++)
				{
					handlePositions[numA + i + 2] = joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i]);
					handlePositions[1] += handlePositions[numA + i + 2] * joint.scalarsB[i];
				}
			}
			else
			{
				handlePositions[1] = joint.localAnchorB;
			}
		}
	}
	
	public virtual void DrawEditJointGUI() //TODO have some sort of snap all? and/or refresh all?
	{
		SerializedProperty eJoint = eJoints.GetArrayElementAtIndex(editIndex);
		SerializedProperty eBreakable = eJoint.FindPropertyRelative("breakable");
		SerializedProperty eVelocity = eJoint.FindPropertyRelative("breakVelocity");
		SerializedProperty eTransformB = eJoint.FindPropertyRelative("mTransformB");
		SerializedProperty eGlobalAnchorB = eJoint.FindPropertyRelative("globalAnchorB");
		SerializedProperty eLocalAnchorA = eJoint.FindPropertyRelative("localAnchorA");
		SerializedProperty eIndicesA = eJoint.FindPropertyRelative("affectedIndicesA");
		SerializedProperty eScalarsA = eJoint.FindPropertyRelative("scalarsA");
		SerializedProperty eLocalAnchorB = eJoint.FindPropertyRelative("localAnchorB");
		SerializedProperty eIndicesB = eJoint.FindPropertyRelative("affectedIndicesB");
		SerializedProperty eScalarsB = eJoint.FindPropertyRelative("scalarsB");


		JelloJoint joint = body.GetJoint(editIndex);

		EditorGUILayout.PropertyField(eBreakable, breakableContent);
		if(eBreakable.boolValue)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(eVelocity, jointBreakVelocityContent);
			EditorGUI.indentLevel--;
		}

		//TODO make anchor point bold

		GUIStyle anchorAStyle = new GUIStyle(EditorStyles.label);
		if(eIndicesA.prefabOverride || eScalarsA.prefabOverride || eLocalAnchorA.prefabOverride)
			anchorAStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Anchor Point", anchorAStyle); //TODO have toggle to always snap??? how would this work with multiple objects?
		if(GUILayout.Button(snapJointContent))
		{
			if(eTransformB.objectReferenceValue != null)
			{
				joint.TransformB.position -= joint.TransformB.TransformPoint(handlePositions[1]) - joint.TransformA.TransformPoint(handlePositions[0]); 	
			}
			else
			{
				eGlobalAnchorB.vector2Value = body.transform.TransformPoint(joint.GetAnchorPointA(true));
				SceneView.RepaintAll();
			}

			//EditorUtility.SetDirty(body);			//?
		}
		EditorGUILayout.EndHorizontal();

		GUIStyle localStyleA = new GUIStyle(EditorStyles.label);
		if(eIndicesA.prefabOverride || eScalarsA.prefabOverride || eLocalAnchorA.prefabOverride)
			localStyleA.fontStyle = FontStyle.Bold;

		EditorGUI.indentLevel++;
		EditorGUILayout.LabelField("Local Position", handlePositions[0].ToString(), localStyleA);
		string text = "";
		for(int i = 0; i < joint.affectedIndicesA.Length; i++)
			text += joint.affectedIndicesA[i].ToString() + ", ";

		GUIStyle indicesStyleA = new GUIStyle(EditorStyles.label);
		if(eIndicesA.prefabOverride)
		   indicesStyleA.fontStyle = FontStyle.Bold;

		GUIContent anchorIndicesContent = new GUIContent("Anchored Indices");
		EditorGUILayout.LabelField(anchorIndicesContent, indicesStyleA);
	
		EditorGUI.indentLevel++;
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField(text, indicesStyleA);
		if(GUILayout.Button(removeJointLegContent) && joint.affectedIndicesA.Length > 1)
		{
			joint.SetupAnchor(body.transform, joint.GetAnchorPointA(true), true, true, joint.affectedIndicesA.Length - 1);
			SetEditIndex(editIndex);

			EditorUtility.SetDirty(body);

			SceneView.RepaintAll();
		}
		if(GUILayout.Button(addJointLegContent) && joint.affectedIndicesA.Length < 3)
		{
			joint.SetupAnchor(body.transform, joint.GetAnchorPointA(true), true, true, joint.affectedIndicesA.Length + 1);
			SetEditIndex(editIndex);

			EditorUtility.SetDirty(body);

			SceneView.RepaintAll();
		}
		
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUI.indentLevel--;

		GUIStyle xformStyleB = new GUIStyle(EditorStyles.label);
		if(eTransformB.prefabOverride)
			xformStyleB.fontStyle = FontStyle.Bold;


		EditorGUILayout.LabelField("Connected Transform", xformStyleB);

		EditorGUI.indentLevel++;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eTransformB, new GUIContent());
		if(EditorGUI.EndChangeCheck())
		{
			Vector2 point = Vector2.zero;
			Transform xformB = (Transform)eTransformB.objectReferenceValue;
			if(xformB.GetComponent<Collider2D>() != null)
			{
				Vector2 anchorPositionA = body.transform.TransformPoint(handlePositions[0]);
				if(xformB.GetComponent<Collider2D>().OverlapPoint(anchorPositionA))
					point = xformB.InverseTransformPoint(anchorPositionA);
			}

			joint.SetupAnchor((Transform)eTransformB.objectReferenceValue, point, false, true);
			
			SetEditIndex(editIndex);

			EditorUtility.SetDirty(body);
			mainEditor.serializedObject.UpdateIfDirtyOrScript();

			SceneView.RepaintAll();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.BeginHorizontal();

		GUIStyle anchorStyleB = new GUIStyle(EditorStyles.label);
		if(eTransformB.prefabOverride || eIndicesB.prefabOverride || eScalarsB.prefabOverride || eLocalAnchorB.prefabOverride || eGlobalAnchorB.prefabOverride)
			anchorStyleB.fontStyle = FontStyle.Bold;

		EditorGUILayout.LabelField("Anchor Point", anchorStyleB);
		
		if(GUILayout.Button(snapJointContent))
		{
			if(eTransformB.objectReferenceValue != null)
			{
				joint.TransformA.position -= joint.TransformA.TransformPoint(handlePositions[0]) - joint.TransformB.TransformPoint(handlePositions[1]); 	
			}
			else
			{
				joint.TransformA.position -= joint.TransformA.TransformPoint(handlePositions[0]) - (Vector3)joint.globalAnchorB;
			}

			EditorUtility.SetDirty(body);
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.indentLevel++;
		if(eTransformB.objectReferenceValue != null)
		{
			GUIStyle positionStyleB = new GUIStyle(EditorStyles.label);
			if(eIndicesB.prefabOverride || eScalarsB.prefabOverride || eLocalAnchorB.prefabOverride)
				positionStyleB.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Local Position", handlePositions[1].ToString(), positionStyleB);

			if(joint.bodyB != null)
			{	
				text = "";
				for(int i = 0; i < joint.affectedIndicesB.Length; i++)
					text += joint.affectedIndicesB[i].ToString() + ", ";

				GUIStyle indicesStyleB = new GUIStyle(EditorStyles.label);
				if(eIndicesB.prefabOverride)
					indicesStyleB.fontStyle = FontStyle.Bold;

				EditorGUILayout.LabelField("Anchored Indices", indicesStyleB);

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(text, indicesStyleB);
				
				if(GUILayout.Button(removeJointLegContent) && joint.affectedIndicesB.Length > 1)
				{
					joint.SetupAnchor(joint.TransformB, joint.GetAnchorPointB(true), false, true, joint.affectedIndicesB.Length - 1);
					SetEditIndex(editIndex);

					EditorUtility.SetDirty(body);

					SceneView.RepaintAll();
				}
				if(GUILayout.Button(addJointLegContent) && joint.affectedIndicesA.Length < 3)
				{
					joint.SetupAnchor(joint.TransformB, joint.GetAnchorPointB(true), false, true, joint.affectedIndicesB.Length + 1);
					SetEditIndex(editIndex);

					EditorUtility.SetDirty(body);

					SceneView.RepaintAll();
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
		else
		{
			EditorGUILayout.PropertyField(eGlobalAnchorB, jointAnchorBcontent);
		}
		EditorGUI.indentLevel--;
	}

	//TODO draw some info for the other body as well? like an outline?
	public void DrawjointSceneGUI()
	{
		mainEditor.DrawPointMasses(body, false);

		Vector3 pos;
		
		//hovered over joint
		if(drawIndex >= 0 && drawIndex < body.JointCount && body.GetJoint(drawIndex).bodyA != null)
		{
			JelloJoint joint = body.GetJoint(drawIndex);

			Vector3 posA = body.transform.TransformPoint(joint.GetAnchorPointA(true));
			posA.z = body.transform.position.z;

			Handles.color = Color.magenta;
			for(int i = 0; i < joint.affectedIndicesA.Length; i++)
			{
				Handles.DrawLine(posA, body.transform.TransformPoint(body.Shape.getVertex(joint.affectedIndicesA[i])));
				Handles.DotCap(3, 
				               body.transform.TransformPoint(body.Shape.getVertex(joint.affectedIndicesA[i])), 
				               Quaternion.identity, 
				               HandleUtility.GetHandleSize(body.transform.TransformPoint(body.Shape.getVertex(joint.affectedIndicesA[i]))) * 0.05f);
			}
			Handles.color = Color.blue;
			Handles.DotCap(3, posA, Quaternion.identity, HandleUtility.GetHandleSize(posA) * 0.075f);

			Vector3 posB = joint.GetAnchorPointB(true);



			if(joint.TransformB !=  null)
			{
				posB = joint.TransformB.TransformPoint(posB);
				
				if(joint.bodyB != null)
				{
					if(joint.affectedIndicesB != null)
					{


						Handles.color = Color.magenta;
						for(int i = 0; i < body.GetJoint(drawIndex).affectedIndicesB.Length; i++)
						{
							Handles.DrawLine(posB, joint.bodyB.transform.TransformPoint(joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i])));
							Handles.DotCap(3, 
							               joint.bodyB.transform.TransformPoint(joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i])), 
							               Quaternion.identity, 
							               HandleUtility.GetHandleSize(joint.bodyB.transform.TransformPoint(joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i]))) * 0.05f);
						}
						Handles.color = Color.blue;
					}
				}
				
				
			}
			
			Handles.DotCap(3, posB, Quaternion.identity, HandleUtility.GetHandleSize(posB) * 0.075f);
			
			Handles.color = Color.red;
			Handles.DrawLine(posA, posB);
		}
		
		//selected joint
		if(editIndex >= 0 && editIndex < body.JointCount && body.GetJoint(editIndex).affectedIndicesA != null)
		{	
			JelloJoint joint = body.GetJoint(editIndex);


			int num = 2 + joint.affectedIndicesA.Length;
			if(joint.TransformB != null && joint.bodyB != null)
				num += joint.affectedIndicesB.Length;

			//first get handle sizes...
			//need global handle positions
			Vector3[] globalHandlePositions = new Vector3[num];

			globalHandlePositions[0] = body.transform.TransformPoint(handlePositions[0]);
			for(int i = 0; i < joint.affectedIndicesA.Length; i++)
				globalHandlePositions[i + 2] = body.transform.TransformPoint(handlePositions[i + 2]);
			
			
			if(joint.TransformB != null)
			{
				globalHandlePositions[1] = joint.TransformB.TransformPoint(handlePositions[1]);

				if(joint.bodyB != null)
				{
					for(int i = 0; i < joint.affectedIndicesB.Length; i++)
					{
						globalHandlePositions[i + 2 + joint.affectedIndicesA.Length] = joint.TransformB.TransformPoint(handlePositions[i + 2 + joint.affectedIndicesA.Length]);
					}
				}
			}
			else
			{
				globalHandlePositions[1] = joint.globalAnchorB;
			}

			CalculateHandleSizes(globalHandlePositions);
			
			bool mouseUp = false;
			
			if(Event.current.type == EventType.mouseUp)
				mouseUp = true;
			
			Handles.color = Color.cyan;
			
			EditorGUI.BeginChangeCheck();
			handlePositions[0] = body.transform.InverseTransformPoint( Handles.FreeMoveHandle(body.transform.TransformPoint(handlePositions[0]), Quaternion.identity, handleSizes[0], Vector3.zero, Handles.CircleCap));
			if(EditorGUI.EndChangeCheck())
			{
				Vector2[] affectedPoints = new Vector2[joint.affectedIndicesA.Length];
				for(int i = 0; i < affectedPoints.Length; i++)
					affectedPoints[i] = body.Shape.getVertex(joint.affectedIndicesA[i]);
				
				joint.RebuildAnchor
					(
						handlePositions[0], 
						true, 
						true,
						joint.affectedIndicesA,
						affectedPoints
						);
				SetEditIndex(editIndex);

				EditorUtility.SetDirty(body);

				SceneView.RepaintAll();
			}

			for(int i = 0; i < joint.affectedIndicesA.Length; i++)
			{
				Handles.color = Color.blue;
				handlePositions[i + 2] = body.transform.InverseTransformPoint(Handles.FreeMoveHandle(body.transform.TransformPoint(handlePositions[i + 2]), 
				                                                                                Quaternion.identity, 
				                                                                                handleSizes[i + 2],
				                                                                                Vector3.zero,  
				                                                                                Handles.CircleCap));
				
				if(mouseUp)
				{
					if((Vector2)handlePositions[i + 2] != body.Shape.getVertex(joint.affectedIndicesA[i]))
					{
						Vector2[] points = new Vector2[body.Shape.VertexCount];
						for(int s = 0; s < body.Shape.VertexCount; s++)
							points[s] = body.Shape.getVertex(s);
						
						int index = JelloShapeTools.FindClosestVertexOnShape(handlePositions[i + 2], points);
						bool indexInUse = false;
						
						for(int u = 0; u < joint.affectedIndicesA.Length; u++)
							if(index == joint.affectedIndicesA[u])
								indexInUse = true;
						
						if(!indexInUse)
						{
							joint.affectedIndicesA[i] = index;
							
							Vector2[] affectedVertices = new Vector2[joint.affectedIndicesA.Length];
							for(int v = 0; v < affectedVertices.Length; v++)
								affectedVertices[v] = body.Shape.getVertex(joint.affectedIndicesA[v]); 
							
							handlePositions[i + 2] = body.Shape.getVertex(index);
							
							
							joint.RebuildAnchor(joint.localAnchorA, true, true, null, affectedVertices);

							Vector2 newPosition = Vector2.zero;
							for(int v = 0; v < affectedVertices.Length; v++)
								newPosition += affectedVertices[v] * joint.scalarsA[v];

							handlePositions[0] = newPosition;

							EditorUtility.SetDirty(body);

							SceneView.RepaintAll();
						}
						else
						{
							handlePositions[i + 2] = body.Shape.getVertex(joint.affectedIndicesA[i]);
						}
					}
				}
				
				Handles.color = Color.black;
				Handles.DrawLine(body.transform.TransformPoint(handlePositions[0]), body.transform.TransformPoint( handlePositions[i + 2]));
			}
			
			//other object's GUI
			Handles.color = Color.magenta;
			
			if(joint.TransformB != null)
			{
				EditorGUI.BeginChangeCheck();
				handlePositions[1] = joint.TransformB.InverseTransformPoint( Handles.FreeMoveHandle(joint.TransformB.TransformPoint(handlePositions[1]), Quaternion.identity, handleSizes[1], Vector3.zero, Handles.CircleCap));
				if(EditorGUI.EndChangeCheck())
				{
					Vector2[] affectedPoints = new Vector2[0];
					
					if(joint.bodyB != null)
					{
						affectedPoints = new Vector2[joint.affectedIndicesB.Length];
						for(int i = 0; i < affectedPoints.Length; i++)
							affectedPoints[i] = joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i]);
					}
					
					joint.RebuildAnchor
						(
							handlePositions[1], 
							false,
							true,
							joint.affectedIndicesB,
							affectedPoints
							);
					
					SetEditIndex(editIndex);

					EditorUtility.SetDirty(body);

					SceneView.RepaintAll();
				}

				if(joint.bodyB != null && joint.affectedIndicesB != null)
				{	
					int numAffectedA = joint.affectedIndicesA.Length;
					for(int i = 0; i < joint.affectedIndicesB.Length; i++)
					{
						int offsetIndex = i + numAffectedA + 2;
						
						Handles.color = Color.red;
						handlePositions[offsetIndex] = joint.TransformB.InverseTransformPoint(Handles.FreeMoveHandle(joint.TransformB.TransformPoint(handlePositions[offsetIndex]), 
						                                                                                                  Quaternion.identity, 
						                                                                                                  handleSizes[offsetIndex], 
						                                                                                                  Vector3.zero, 
						                                                                                                  Handles.CircleCap));

						if(mouseUp)
						{
							if((Vector2)handlePositions[offsetIndex] != joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i]))
							{
								Vector2[] points = new Vector2[joint.bodyB.Shape.VertexCount];
								for(int s = 0; s < joint.bodyB.Shape.VertexCount; s++)
									points[s] = joint.bodyB.Shape.getVertex(s);
								
								int index = JelloShapeTools.FindClosestVertexOnShape(handlePositions[offsetIndex], points);
								bool indexInUse = false;
								
								for(int u = 0; u < joint.affectedIndicesB.Length; u++)
									if(index == joint.affectedIndicesB[u])
										indexInUse = true;
								
								
								if(!indexInUse)
								{
									joint.affectedIndicesB[i] = index;
									
									Vector2[] affectedVertices = new Vector2[joint.affectedIndicesB.Length];
									for(int v = 0; v < affectedVertices.Length; v++)
										affectedVertices[v] = joint.bodyB.Shape.getVertex(joint.affectedIndicesB[v]); 
									
									handlePositions[offsetIndex] = joint.bodyB.Shape.getVertex(index);
									

									joint.RebuildAnchor(joint.localAnchorB, false, true, null, affectedVertices);

									Vector2 newPosition = Vector2.zero;
									for(int v = 0; v < affectedVertices.Length; v++)
										newPosition += affectedVertices[v] * joint.scalarsB[v];
									handlePositions[1] = newPosition;

									EditorUtility.SetDirty(body);

									SceneView.RepaintAll();
								}
								else
								{
									handlePositions[offsetIndex] = joint.bodyB.Shape.getVertex(joint.affectedIndicesB[i]);
								}
							}
						}
						
						Handles.color = Color.grey;
						Handles.DrawLine(joint.bodyB.transform.TransformPoint(handlePositions[1]), joint.bodyB.transform.TransformPoint( handlePositions[offsetIndex]));
					}
				}
				
				Handles.color = Color.yellow;
				Handles.DrawLine(joint.TransformA.TransformPoint(handlePositions[0]), joint.TransformB.TransformPoint(handlePositions[1]));
			}
			else
			{
				//TODO this should be handlepositions[1]???
				EditorGUI.BeginChangeCheck();
				joint.globalAnchorB = Handles.FreeMoveHandle(joint.globalAnchorB, Quaternion.identity, handleSizes[1], Vector3.zero, Handles.CircleCap);
				if(EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(body);
				}
				Handles.color = Color.yellow;
				Handles.DrawLine(joint.TransformA.TransformPoint(handlePositions[0]), joint.globalAnchorB);
			}
		}
		
		//add new joint logic
		if(newSubComponentState == AddSubComponentState.initiated)
		{
			int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
			
			if(Event.current.type == EventType.Layout)
				HandleUtility.AddDefaultControl(controlID);
			
			pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin; //need where this ray intersects the zplane
			Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, body.transform.position.z));
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			float dist = 0f;
			plane.Raycast(ray, out dist);
			pos = ray.GetPoint(dist);
			Vector3 mousePosWorld = new Vector3(pos.x, pos.y, body.transform.position.z);
			
			Handles.color = Color.blue;
			
			Handles.CircleCap(3, mousePosWorld, Quaternion.identity, HandleUtility.GetHandleSize(mousePosWorld) * 0.15f);
			
			if(Event.current.type == EventType.MouseUp)
			{
				body.AddJoint(new JelloJoint(body.transform, null, body.transform.InverseTransformPoint(mousePosWorld), Vector2.zero, true, true));
				newSubComponentState = AddSubComponentState.inactive;
				SetEditIndex(body.JointCount - 1);

				EditorUtility.SetDirty(body);
			}
			
			SceneView.RepaintAll();
		}
	}
}
