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
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(JelloBody))]
public class JelloBodyEditor : Editor 
{
	
	SerializedProperty eKinematic;
	SerializedProperty eGravity;
	SerializedProperty eOverrideGravity;
	SerializedProperty eTrigger;
	SerializedProperty eAffectedByGravity;
	SerializedProperty eMass;
	SerializedProperty eAwake;

	public static bool showPivot = false;
	public MeshLink.MeshLinkType meshLinkType = MeshLink.MeshLinkType.None;
	public List<SubComponentEditor> subEditors;
	public static int currentSubEditor;


	public JelloBody tar;
	public bool retainBodyInformation = true;
	public ColliderType colliderType;
	public enum ColliderType { Static, Pressure, Spring };
	public Vector3 prevPosition;
	public Vector3 prevRotation;
	public Vector3 prevScale;
	public Vector2 pivot;
	public bool hasMeshLink;
	public JelloBody.ShapeSettingOptions shapeSettingOptions = JelloBody.ShapeSettingOptions.MovePointMasses | JelloBody.ShapeSettingOptions.RebuildEdgeSprings;
	public JelloBody.SmartShapeSettingOptions smartShapeSettingOptions = JelloBody.SmartShapeSettingOptions.RebuildInvalidatedAttachPoints | JelloBody.SmartShapeSettingOptions.RebuildInvalidatedCustomSprings | JelloBody.SmartShapeSettingOptions.RebuildInvalidatedJoints;


	//TODO have a show tooltip tick-box

	public GUIContent massContent = new GUIContent("Mass");//, "Mass of this body");
	public GUIContent useGravContent = new GUIContent("Use Gravity");//, "Deselect to have body ignore gravity");
	public GUIContent overrideGravContent = new GUIContent("Override");//, "Use this to set custom per-body gravity" +
																												//"\n(Overrides world gravity)");
	public GUIContent awakeContent = new GUIContent("Is Awake");
	public GUIContent customGravContent = new GUIContent("Custom Gravity");//, "Direction and strength of gravity on this body");
	public GUIContent triggerContent = new GUIContent("Is Trigger");//, "Select this to have body ignore collision forces but still 'trigger' the collision events");
	public GUIContent kinematicContent = new GUIContent("Is Kinematic");//, "Select this to have body position and angle only be set explicitly" +
																													//"\n(Will ignore force and torque)" +
																													//"\nVelocity and Angular velocity will be calculated from its change in position" +
																													//"\nSoft bodies will still be soft and will move via Shape Matching");
	public GUIContent pivotFoldoutContent = new GUIContent("Pivot Point");//, "Expand this to modify" +
																													//"\nMultiediting only works with the Center button" +
																													//"\nNote: using an off-center pivot for a soft body may cause the simulation to explode");
	public GUIContent centerPivotContent = new GUIContent("Center");//, "Click to set the pivot point to the average of this body's JelloPointMass' positions");
	public GUIContent applyPivotContent = new GUIContent("Apply");//, "Apply changes to the pivot point");
	public GUIContent cancelPivotContent = new GUIContent("Cancel");//, "Cancel any changes to the pivot point");
	public GUIContent keepInfoContent = new GUIContent("Keep Info");//, "Retain relevant information from this body into the new type");
	public GUIContent changeTypeContent = new GUIContent("Apply");//, "Click to change the current type to the selected type");
	public GUIContent bodyTypeContent = new GUIContent("JelloBody Type");//, "The kind of body this is" +
																												//"\nSelect a diffrent type and click change to change what kind of body this is");
	public GUIContent meshLinkTypeContent = new GUIContent("Mesh Link Type");//, "Mesh Links are used to interact with the mesh" +
																																//"\nSelect a different type and click change to link with a diffrent type");



	void OnEnable()
	{
		Enabled();
	}
	
	public virtual void Enabled()
	{
		eKinematic = serializedObject.FindProperty("mIsKinematic");
		eGravity = serializedObject.FindProperty("gravity");
		eOverrideGravity = serializedObject.FindProperty("overrideWorldGravity");
		eTrigger = serializedObject.FindProperty("mIsTrigger");
		eAffectedByGravity = serializedObject.FindProperty("affectedByGravity");
		eMass = serializedObject.FindProperty("mMass");
		eAwake = serializedObject.FindProperty("mIsAwake");

		for(int i = 0; i < serializedObject.targetObjects.Length; i++)
		{
			tar = (JelloBody)serializedObject.targetObjects[i];
			tar.setComponentReferences();
		}

		tar = (JelloBody)serializedObject.targetObject;
		SetEnumToBodyType(tar);
		GetBodyMeshLinkType(tar);
		pivot = Vector2.zero;

		subEditors = new List<SubComponentEditor>();
		subEditors.Add (new SubComponentEditor(this));
		subEditors.Add (new PointMassSubComponentEditor(this));
		subEditors.Add (new AttachPointSubComponentEditor(this));
		subEditors.Add (new JointSubComponentEditor(this));
	}
	
	public override void OnInspectorGUI()
	{
		DrawEditorGUI();
		DrawEditorGUITwo();
	}
	
	
	public void OnSceneGUI() 
	{
		DrawSceneGUI();
	}
	
	//todo add onenable and ondisable events to body.
	public virtual void DrawEditorGUI()
	{
		//check polycollider vs pointmasscount
		if(!Application.isPlaying) //TODO have this be handled by a class that extends the polycollider and recognises changes?
		{
			for(int b = 0; b < targets.Length; b++)
			{
				JelloBody body = (JelloBody)targets[b];
				body.setComponentReferences();
				body.polyCollider.points = JelloShapeTools.RemoveDuplicatePoints(body.polyCollider.points);


				JelloClosedShape shape = new JelloClosedShape(body.polyCollider.points, null, false);

				if(body.Shape != null)
				{
					for(int i = 0; i < body.Shape.InternalVertexCount; i++)
						shape.addInternalVertex(body.Shape.InternalVertices[i]);

					shape.finish(false);
				}

				if(shape.EdgeVertexCount != body.Shape.EdgeVertexCount || shape.InternalVertexCount != body.Shape.InternalVertexCount)
					body.smartSetShape(shape, JelloBody.ShapeSettingOptions.MovePointMasses, smartShapeSettingOptions);
				else
					body.setShape(shape, JelloBody.ShapeSettingOptions.MovePointMasses); 

				//will i need to do this for constraints as well?
				for(int i = 0; i < body.AttachPointCount; i++)
				{
					body.GetAttachPoint(i).UpdateEditorMode();
				}
			}
		}
		
		serializedObject.Update();
		
		EditorGUI.showMixedValue = eMass.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eMass, massContent);
		if(EditorGUI.EndChangeCheck())
		{
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				JelloBody b = (JelloBody)serializedObject.targetObjects[i];
				b.Mass = eMass.floatValue;
			}
			serializedObject.UpdateIfDirtyOrScript();
		}
		EditorGUI.showMixedValue = false;

		if(!tar.IsStatic)
		{	
			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.showMixedValue =  eAffectedByGravity.hasMultipleDifferentValues;
			EditorGUILayout.PropertyField(eAffectedByGravity, useGravContent);
			EditorGUI.showMixedValue = false;
				
			EditorGUILayout.EndHorizontal();
			
			if(eAffectedByGravity.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUI.showMixedValue = eOverrideGravity.hasMultipleDifferentValues;
				EditorGUILayout.PropertyField(eOverrideGravity, overrideGravContent);
				EditorGUI.showMixedValue = false;
				
				if(eOverrideGravity.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUI.showMixedValue = eGravity.hasMultipleDifferentValues;
					EditorGUILayout.PropertyField(eGravity, customGravContent);
					EditorGUI.showMixedValue = false;
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			
			EditorGUI.showMixedValue = eKinematic.hasMultipleDifferentValues;
			EditorGUILayout.PropertyField(eKinematic, kinematicContent);
			EditorGUI.showMixedValue = false;
		}
		
		EditorGUI.showMixedValue = eTrigger.hasMultipleDifferentValues;
		EditorGUILayout.PropertyField(eTrigger, triggerContent);
		EditorGUI.showMixedValue = false;

		EditorGUI.showMixedValue = eAwake.hasMultipleDifferentValues;
		EditorGUILayout.PropertyField(eAwake, awakeContent);
		EditorGUI.showMixedValue = false;
		
		if(tar.meshLink == null || tar.meshLink.canModifyPivotPoint)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();


			SerializedProperty ePivotOffset = serializedObject.FindProperty("pivotOffset");
			GUIStyle pivotStyle = new GUIStyle(EditorStyles.foldout);
			if(ePivotOffset.prefabOverride)
				pivotStyle.fontStyle = FontStyle.Bold;

			showPivot = EditorGUILayout.Foldout(showPivot, pivotFoldoutContent, pivotStyle);
			
			if(GUILayout.Button(centerPivotContent, EditorStyles.miniButton))
			{
				Undo.RecordObjects(serializedObject.targetObjects, "Center Pivot");
				for(int i = 0; i < serializedObject.targetObjects.Length; i++)
				{
					JelloBody jb = (JelloBody)serializedObject.targetObjects[i];
					Undo.RecordObject(jb.gameObject.transform, "Center Pivot transform");
					if(jb.meshLink != null)
					{
						Undo.RecordObject(jb.meshLink, "Center Pivot mesh");
						Undo.RecordObject(jb.polyCollider, "Center Pivot collider");
						Undo.RecordObject(jb.GetComponent<Renderer>(), "Center Pivot renderer");
						Undo.RecordObject(jb.meshLink.LinkedMeshFilter, "Center Pivot filter");
					}
				}

				for(int i = 0; i < serializedObject.targetObjects.Length; i++)
				{
					JelloBody bod = (JelloBody)serializedObject.targetObjects[i];
					CenterPivot(bod);
					EditorUtility.SetDirty(bod.gameObject);
					EditorUtility.SetDirty(bod.meshLink);
					//serializedObject.UpdateIfDirtyOrScript();
				}
				SceneView.RepaintAll();
			}
			
			EditorGUILayout.EndHorizontal();

			if(showPivot)
			{
				if(!serializedObject.isEditingMultipleObjects)
				{
					pivot = EditorGUILayout.Vector2Field("Position", pivot);
					
					if(pivot != Vector2.zero)
					{
						EditorGUILayout.BeginHorizontal();	
					
						if(GUILayout.Button(applyPivotContent, EditorStyles.miniButton))
						{
							JelloBody jb = (JelloBody)serializedObject.targetObject;
							Undo.RecordObject(jb, "Change Pivot");
							Undo.RecordObject(jb.gameObject.transform, "Change Pivot transform");
							if(jb.meshLink != null)
							{
								Undo.RecordObject(jb.meshLink, "Change Pivot mesh");
								Undo.RecordObject(jb.polyCollider, "Change Pivot collider");
								Undo.RecordObject(jb.GetComponent<Renderer>(), "Change Pivot renderer");
								Undo.RecordObject(jb.meshLink.LinkedMeshFilter, "Change Pivot filter");
							}

							ChangePivot(tar);
							EditorUtility.SetDirty(tar.gameObject);
							EditorUtility.SetDirty(tar.meshLink);
//							serializedObject.UpdateIfDirtyOrScript();
						}
						if(GUILayout.Button(cancelPivotContent, EditorStyles.miniButton))
							pivot = Vector2.zero;
						
						
						SceneView.RepaintAll();
						EditorGUILayout.EndHorizontal();
					}
				}
				else
				{
					EditorGUILayout.HelpBox("Pivot Points may only be centered when multiple Game Objects are selected", MessageType.Info);
				}
						
			}
			EditorGUI.indentLevel--;
		}

		serializedObject.ApplyModifiedProperties();
	}

	
	public virtual void DrawEditorGUITwo()
	{
		serializedObject.Update();

		if(tar.GetComponent<MeshLink>() != null)
			hasMeshLink = true;
		else
			hasMeshLink = false;
		
		EditorGUILayout.BeginHorizontal();

		SerializedProperty eMeshLink = serializedObject.FindProperty("meshLink");
		GUIStyle meshStyle = new GUIStyle(EditorStyles.popup);
		if(eMeshLink.prefabOverride)
			meshStyle.fontStyle = FontStyle.Bold;

		meshLinkType = (MeshLink.MeshLinkType)EditorGUILayout.EnumPopup(meshLinkTypeContent, meshLinkType, meshStyle);
		
		bool showChangeButton = false;

		if(hasMeshLink)
		{	
			if(tar.GetComponent<MeshLink>().meshLinkType != meshLinkType)
				showChangeButton = true;
		}
		else if(meshLinkType != MeshLink.MeshLinkType.None)
		{
			showChangeButton = true;
		}
		
		if(showChangeButton)
		{
			if(GUILayout.Button(changeTypeContent, EditorStyles.miniButton))
			{
				for(int i = 0; i < serializedObject.targetObjects.Length; i++)
					ChangeMeshLink((JelloBody)serializedObject.targetObjects[i]);
			}
		}
		
		EditorGUILayout.EndHorizontal();
		
		colliderType = (ColliderType)EditorGUILayout.EnumPopup(bodyTypeContent, colliderType, EditorStyles.miniButton);
		
		EditorGUILayout.BeginHorizontal();

		if(CompareEnumStateWithType(tar))
		{
			EditorGUI.indentLevel++;
			
			if(GUILayout.Button(new GUIContent(changeTypeContent), EditorStyles.miniButton )) //todo make this change all targets
			{	
				JelloBody[] oldTargets = new JelloBody[targets.Length];
				for(int i = 0; i < serializedObject.targetObjects.Length; i++)
				{
					JelloBody t = (JelloBody)serializedObject.targetObjects[i];
					
					ChangeBodyType(t, retainBodyInformation);
					
					oldTargets[i] = t;
				}
				
				for(int i = 0; i < oldTargets.Length; i++)
				{
					Undo.DestroyObjectImmediate (oldTargets[i]);
				}
				
				return;
			}
			
			retainBodyInformation = EditorGUILayout.Toggle(keepInfoContent, retainBodyInformation);
			
			EditorGUI.indentLevel--;
		}
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		EditorGUILayout.BeginHorizontal();

		string[] options = new string[subEditors.Count];
		for(int i = 0; i < options.Length; i++)
			options[i] = subEditors[i].name;
		currentSubEditor = EditorGUILayout.Popup("SubComponent", currentSubEditor, options);

		EditorGUILayout.EndHorizontal();
		subEditors[currentSubEditor].DrawEditorGUI();

		serializedObject.ApplyModifiedProperties();
	}

	public virtual void DrawSceneGUI()
	{
		DrawPivotSceneGUI();

		if(currentSubEditor > subEditors.Count - 1)
			currentSubEditor = 0;

		subEditors[currentSubEditor].DrawSceneGUI();
	}

	public bool CompareEnumStateWithType(JelloBody t)
	{	
		if(colliderType == ColliderType.Static && t.GetComponent<JelloBody>().GetType().ToString() == "JelloBody")
			return false;
		if(colliderType == ColliderType.Spring && t.GetComponent<JelloBody>().GetType().ToString() == "JelloSpringBody")
			return false;
		if(colliderType == ColliderType.Pressure && t.GetComponent<JelloBody>().GetType().ToString() == "JelloPressureBody")
			return false;
		
		return true;
	}
	
	public void SetEnumToBodyType(JelloBody t)
	{
		if(t.GetComponent<JelloBody>().GetType().ToString() == "JelloBody")
			colliderType = ColliderType.Static;
		if(t.GetComponent<JelloBody>().GetType().ToString() == "JelloSpringBody")
			colliderType = ColliderType.Spring;
		if(t.GetComponent<JelloBody>().GetType().ToString() == "JelloPressureBody")
			colliderType = ColliderType.Pressure;
	}
	
	
	//TODO find a better way to handle this //extend the transform editor?
	public virtual void HandleBodyMovedInEditor(JelloBody tar)
	{
		//		if(Application.isPlaying)
		//			return;
		//		
		//		for(int a = 0; a < serializedObject.targetObjects.Length; a++)
		//		{
		//			JelloBody t = (JelloBody)serializedObject.targetObjects[a];
		//			if(t.EdgePointMassCount == 0)
		//				continue;
		//			
		//			Vector2[] points = new Vector2[t.EdgePointMassCount];
		//			
		//			points = t.Shape.transformVertices(t.Position, t.Angle, t.Scale);
		//			
		//			for(int i = 0; i < t.EdgePointMassCount; i++)
		//				t.getEdgePointMass(i).Position = points[i];
		//			
		//			EditorUtility.SetDirty(t);
		//		}
	}


	#region POINT MASS

	public void DrawPointMasses(JelloBody body, bool editable)
	{
		Handles.color = new Color(0.75f, 0.75f, 0.2f, 0.5f);
		
		for(int i = 0; i < body.Shape.EdgeVertexCount; i++)
		{
			Vector2 pos = body.transform.TransformPoint (body.Shape.EdgeVertices[i]);
			if(editable)
			{
				int hot = GUIUtility.hotControl;
				
				Handles.FreeMoveHandle(pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f, Vector3.zero, Handles.DotCap);
				if(GUIUtility.hotControl != hot)
				{
					if(currentSubEditor == 1)//point mass editor!
					{
						subEditors[currentSubEditor].SetEditIndex(i);
						Repaint();
					}
				}
			}
			else
			{
				Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				Handles.DotCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f);
			}
		}

		for(int i = 0; i < body.Shape.InternalVertexCount; i++)
		{
			Handles.color = new Color(0.75f, 0f, 0.75f, 0.5f);
			Vector2 pos = body.transform.TransformPoint (body.Shape.InternalVertices[i]);
			
			if(editable)
			{
				int hot = GUIUtility.hotControl;

				EditorGUI.BeginChangeCheck();
				pos = Handles.FreeMoveHandle(pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f, Vector3.zero, Handles.DotCap);
				if(EditorGUI.EndChangeCheck())
				{
					if(!JelloShapeTools.Contains(body.Shape.EdgeVertices, body.transform.InverseTransformPoint(pos)) || JelloShapeTools.PointOnPerimeter(body.Shape.EdgeVertices, body.transform.InverseTransformPoint(pos)))
					{
						JelloClosedShape newShape = new JelloClosedShape(body.Shape.EdgeVertices, null, false);

						for(int a = 0; a < body.Shape.InternalVertexCount; a++)
						{
							//dont add this point
							if(a == i)
								continue;

							newShape.addInternalVertex(body.Shape.InternalVertices[a]);
						}

						newShape.finish(false);

						body.smartSetShape(newShape, JelloBody.ShapeSettingOptions.MovePointMasses, smartShapeSettingOptions);

						EditorUtility.SetDirty(body);
						GUIUtility.hotControl = 0;
						subEditors[currentSubEditor].SetEditIndex(-1);
						Repaint();
						break;
					}

					body.Shape.changeInternalVertexPosition(i, body.transform.InverseTransformPoint(pos));
					body.Shape.finish(false);

					EditorUtility.SetDirty(body);

				}
				if(GUIUtility.hotControl != hot && GUIUtility.hotControl != 0)
				{

					if(currentSubEditor == 1)//point mass editor!
					{
						subEditors[currentSubEditor].SetEditIndex(i + body.EdgePointMassCount);
						Repaint();
					}
				}
			}
			else
			{
				Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				Handles.DotCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f);
			}

	
		}

	}

	#endregion



	#region PIVOT

	public void DrawPivotSceneGUI()
	{
		if(showPivot && pivot != Vector2.zero)
		{
			Vector3	pos = new Vector3(tar.transform.position.x + pivot.x, tar.transform.position.y + pivot.y, tar.transform.position.z);
			Handles.color = new Color(1, 0, 0, 0.75f);
			Handles.SphereCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.165f);
		}
	}

	public void ChangePivot(JelloBody t)
	{
		
		t.polyCollider.points = JelloShapeTools.RemoveDuplicatePoints(t.polyCollider.points);

		t.Shape.changeVertices(t.polyCollider.points, t.Shape.InternalVertices);

		Vector2 diff = pivot;
		diff = new Vector2(diff.x / t.Scale.x, diff.y / t.Scale.y);
		diff = JelloVectorTools.rotateVector( diff, -t.Angle);
		

		if(t.meshLink != null)
		{
			MonoBehaviour monoBehavior;
			if(t.meshLink.UpdatePivotPoint(diff, out monoBehavior))
				EditorUtility.SetDirty(monoBehavior);
		}

		for(int i = 0; i < t.Shape.VertexCount; i++)
			t.Shape.setVertex(i, t.Shape.getVertex(i) - diff);
		
		t.polyCollider.points = t.Shape.EdgeVertices;

		t.transform.position += (Vector3)JelloVectorTools.rotateVector(new Vector2(diff.x * t.Scale.x, diff.y * t.Scale.y), t.Angle);
		if(t.transform.childCount > 0)
			for(int i = 0; i < t.transform.childCount; i++)
				t.transform.GetChild(i).position -= (Vector3)diff;
		
		if(t.JointCount > 0)
			for(int i = 0; i < t.JointCount; i++)
				t.GetJoint (i).localAnchorA -= diff;
		
		if(t.AttachPointCount > 0)
			for(int i = 0; i < t.AttachPointCount; i++)
				t.GetAttachPoint(i).point -= diff;

		t.updateGlobalShape(true);
		
		EditorUtility.SetDirty(t);
		
		pivot = Vector2.zero;
	}
	
	public void CenterPivot(JelloBody t)
	{
		Vector2 center = new Vector2();

		t.polyCollider.points = JelloShapeTools.RemoveDuplicatePoints(t.polyCollider.points);
		t.Shape.changeVertices(t.polyCollider.points, t.Shape.InternalVertices);

				
		center = JelloShapeTools.FindCenter(t.Shape.EdgeVertices);//using vertices instead of collider.points because need of assigning entire array at once


		if(t.meshLink != null)
		{
			MonoBehaviour monoBehavior;
			if(t.meshLink.UpdatePivotPoint(center, out monoBehavior))
				EditorUtility.SetDirty(monoBehavior);
		}

		for(int i = 0; i < t.Shape.VertexCount; i++)
			t.Shape.setVertex(i, t.Shape.getVertex(i) - center);
		
		t.polyCollider.points = t.Shape.EdgeVertices;
		
		t.transform.position += (Vector3)JelloVectorTools.rotateVector(new Vector2(center.x * t.Scale.x, center.y * t.Scale.y), t.Angle);
		if(t.transform.childCount > 0)
			for(int i = 0; i < t.transform.childCount; i++)
				t.transform.GetChild(i).position -= (Vector3)center;

		if(t.JointCount > 0)
			for(int i = 0; i < t.JointCount; i++)
				t.GetJoint (i).localAnchorA -= center;

		if(t.AttachPointCount > 0)
			for(int i = 0; i < t.AttachPointCount; i++)
				t.GetAttachPoint(i).point -= center;
		
		t.updateGlobalShape(true);
		
		EditorUtility.SetDirty(t);

		pivot = Vector2.zero;
	}

	#endregion

	#region MESH LINK

	public void GetBodyMeshLinkType(JelloBody t)
	{
		if(t.meshLink == null)
			t.meshLink = t.GetComponent<MeshLink>();
		if(t.meshLink == null)
			return;
		
		foreach(MeshLink.MeshLinkType mltype in (MeshLink.MeshLinkType[])Enum.GetValues(typeof(MeshLink.MeshLinkType)))
		{
			if(t.meshLink.GetType().ToString() == mltype.ToString())
			{
				t.meshLink.meshLinkType = mltype;
				break;
			}
		}
		
		meshLinkType = t.meshLink.meshLinkType;
	}

	public void ChangeMeshLink(JelloBody t)
	{	
		if(meshLinkType != MeshLink.MeshLinkType.None && typeof(MeshLink).Assembly.GetType(meshLinkType.ToString()) == null)
		{
			GetBodyMeshLinkType(t);

			Debug.LogWarning("The selected MeshLink type does not exist." +
				"\nIf you are attempting to use the RageSplineMeshLink or tk2dToolKitMeshLink, decompress the MeshLink.zip archive in the JelloPhysics folder" +
				"\nIf you are implementing your own MeshLink, ensure that it derives from MeshLink and its class name and the name listed in the MeshLink.MeshLinkType enum match.");
			return;
		}

		MeshLink[] links = t.gameObject.GetComponents<MeshLink>();
		
		if(links.Length > 0)
		{
			if(links[0].meshLinkType == meshLinkType)
				return;
			
			for(int i = 0; i < links.Length; i++)
				Undo.DestroyObjectImmediate(links[i]);
		}	
		
		if(meshLinkType == MeshLink.MeshLinkType.None)
		{
			t.meshLink = null;
		}	
		else
		{
			Undo.RecordObject(t.gameObject, "Add Mesh Link");
			t.meshLink = (MeshLink)UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(t.gameObject, "Assets/Editor/Bodies/JelloBodyEditor.cs (742,27)", meshLinkType.ToString());
			
			t.meshLink.Initialize();
			
			EditorUtility.SetDirty(t.meshLink);
			EditorUtility.SetDirty(t.gameObject);
		}
		
		EditorUtility.SetDirty(t);
	}

	#endregion



	
#region CHANGE BODY TYPE

	public void ChangeBodyType(JelloBody t, bool retainInfo)
	{	
		switch (colliderType)
		{
		case ColliderType.Static :
			var b = Undo.AddComponent<JelloBody>(t.gameObject);
			if(retainInfo)
				CopyToStaticBody(t, b);
			break;
		case ColliderType.Pressure :
			var bo = Undo.AddComponent<JelloPressureBody>(t.gameObject);
			if(retainInfo)
				CopyToPressureBody(t, bo);
			break;
		case ColliderType.Spring :
			var bod = Undo.AddComponent<JelloSpringBody>(t.gameObject);
			if(retainInfo)
				CopyToSpringBody(t, bod);
			break;
		}
	}
	
	public void CopyToStaticBody(JelloBody oldBody, JelloBody newBody) //TODO make sure these all are up to date with any new variables that need assigning
	{
		newBody.affectedByGravity = oldBody.affectedByGravity;
		newBody.disabled = oldBody.disabled;
		newBody.gravity = oldBody.gravity;
		newBody.IsAwake = oldBody.IsAwake;
		newBody.IsKinematic = oldBody.IsKinematic;
		newBody.IsStatic = tar.IsStatic;
		newBody.IsStatic = true;
		newBody.IsTrigger = oldBody.IsTrigger;
		newBody.overrideWorldGravity = oldBody.overrideWorldGravity;
		newBody.pivotOffset = oldBody.pivotOffset;
		newBody.setComponentReferences();
		newBody.polyCollider = oldBody.polyCollider;
		
		if(oldBody.meshLink != null)
		{
			newBody.meshLink = oldBody.meshLink;
			newBody.meshLink.body = newBody;
		}
		
		if(oldBody.Shape != null)
		{
			newBody.setShape(oldBody.Shape, shapeSettingOptions);
			newBody.Mass = Mathf.Infinity;
		}
		
		for(int i = 0; i < oldBody.EdgePointMassCount; i++)
		{
			newBody.setEdgePointMass(oldBody.getEdgePointMass(i), i);
			newBody.getEdgePointMass(i).Mass = Mathf.Infinity;
			newBody.getEdgePointMass(i).body = newBody;
		}
		for(int i = 0; i < oldBody.InternalPointMassCount; i++)
		{
			newBody.setInternalPointMass(oldBody.getInternalPointMass(i), i);
			newBody.getInternalPointMass(i).Mass = Mathf.Infinity;
			newBody.getInternalPointMass(i).body = newBody;
		}
		for(int i = 0; i < oldBody.AttachPointCount; i++)
		{
			newBody.AddAttachPoint(oldBody.GetAttachPoint(i));
			newBody.GetAttachPoint(i).body = newBody;
		}
		for(int i = 0; i < oldBody.JointCount; i++)
		{
			newBody.AddJoint(oldBody.GetJoint(i));
			newBody.GetJoint(i).bodyA = newBody;
		}
		
		EditorUtility.SetDirty(newBody);
	}
	
	public virtual void CopyToPressureBody(JelloBody oldBody, JelloPressureBody newBody)
	{
		newBody.affectedByGravity = oldBody.affectedByGravity;
		newBody.disabled = oldBody.disabled;
		newBody.gravity = oldBody.gravity;
		newBody.IsAwake = oldBody.IsAwake;
		newBody.IsKinematic = oldBody.IsKinematic;
		newBody.IsStatic = false;
		newBody.IsTrigger = oldBody.IsTrigger;
		newBody.overrideWorldGravity = oldBody.overrideWorldGravity;
		newBody.pivotOffset = oldBody.pivotOffset;
		newBody.setComponentReferences();
		newBody.polyCollider = oldBody.polyCollider;
		
		if(oldBody.meshLink != null)
		{
			newBody.meshLink = oldBody.meshLink;
			newBody.meshLink.body = newBody;
		}
		
		if(oldBody.Shape != null)
		{
			newBody.setShape(oldBody.Shape, shapeSettingOptions);
			newBody.Mass = oldBody.Mass != Mathf.Infinity ? oldBody.Mass : 1f;
		}
		
		for(int i = 0; i < oldBody.EdgePointMassCount; i++)
		{
			newBody.setEdgePointMass(oldBody.getEdgePointMass(i), i);
			newBody.getEdgePointMass(i).body = newBody;
			
			if(oldBody.Mass == Mathf.Infinity)
				newBody.getEdgePointMass(i).Mass = 1f;
		}
		for(int i = 0; i < oldBody.InternalPointMassCount; i++)
		{
			newBody.setInternalPointMass(oldBody.getInternalPointMass(i), i);
			newBody.getInternalPointMass(i).body = newBody;
			
			if(oldBody.Mass == Mathf.Infinity)
				newBody.getInternalPointMass(i).Mass = 1f;
		}
		for(int i = 0; i < oldBody.AttachPointCount; i++)
		{
			newBody.AddAttachPoint(oldBody.GetAttachPoint(i));
			newBody.GetAttachPoint(i).body = newBody;
		}
		for(int i = 0; i < oldBody.JointCount; i++)
		{
			newBody.AddJoint(oldBody.GetJoint(i));
			newBody.GetJoint(i).bodyA = newBody;
		}
		
		EditorUtility.SetDirty(newBody);
	}
	
	public virtual void CopyToSpringBody(JelloBody oldBody, JelloSpringBody newBody)
	{
		newBody.affectedByGravity = oldBody.affectedByGravity;
		newBody.disabled = oldBody.disabled;
		newBody.gravity = oldBody.gravity;
		newBody.IsAwake = oldBody.IsAwake;
		newBody.IsKinematic = oldBody.IsKinematic;
		newBody.IsStatic = false;
		newBody.IsTrigger = oldBody.IsTrigger;
		newBody.overrideWorldGravity = oldBody.overrideWorldGravity;
		newBody.pivotOffset = oldBody.pivotOffset;
		newBody.setComponentReferences();
		newBody.polyCollider = oldBody.polyCollider;
		
		if(oldBody.meshLink != null)
		{
			newBody.meshLink = oldBody.meshLink;
			newBody.meshLink.body = newBody;
		}
		
		if(oldBody.Shape != null)
		{
			newBody.setShape(oldBody.Shape, shapeSettingOptions);
			newBody.Mass = oldBody.Mass != Mathf.Infinity ? oldBody.Mass : 1f;
		}
		
		for(int i = 0; i < oldBody.EdgePointMassCount; i++)
		{
			newBody.setEdgePointMass(oldBody.getEdgePointMass(i), i);
			newBody.getEdgePointMass(i).body = newBody;
			
			if(oldBody.Mass == Mathf.Infinity)
				newBody.getEdgePointMass(i).Mass = 1f;
		}
		for(int i = 0; i < oldBody.InternalPointMassCount; i++)
		{
			newBody.setInternalPointMass(oldBody.getInternalPointMass(i), i);
			newBody.getInternalPointMass(i).body = newBody;
			
			if(oldBody.Mass == Mathf.Infinity)
				newBody.getInternalPointMass(i).Mass = 1f;
		}
		for(int i = 0; i < oldBody.AttachPointCount; i++)
		{
			newBody.AddAttachPoint(oldBody.GetAttachPoint(i));
			newBody.GetAttachPoint(i).body = newBody;
		}
		for(int i = 0; i < oldBody.JointCount; i++)
		{
			newBody.AddJoint(oldBody.GetJoint(i));
			newBody.GetJoint(i).bodyA = newBody;
		}
		
		EditorUtility.SetDirty(newBody);
	}

#endregion

}
