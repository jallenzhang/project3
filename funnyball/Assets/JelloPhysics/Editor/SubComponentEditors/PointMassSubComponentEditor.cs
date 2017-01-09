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

public class PointMassSubComponentEditor : SubComponentEditor {

	public GUIContent shapeMatchingMultiplierContent = new GUIContent ("Multiplier");//, "Multiplier for shape matching forces applied to this point mass");
	public GUIContent massContent = new GUIContent("Mass");//, "Mass of this body");
	public GUIContent showEdgePointMassContent = new GUIContent("Edge Point Masses");//, "Expand this to view and edit each JelloPointMass" +
	//"\nClick on a JelloPointMass in the editor to view and edit more detailed information");
	public GUIContent showInernalPointMassContent = new GUIContent("Internal Point Masses");//, "Expand this to view and edit each JelloPointMass" +
	//"\nClick on a JelloPointMass in the editor to view and edit more detailed information");
	public GUIContent addPointMassContent = new GUIContent ("Add Internal PointMass");//, "Initiate Internal PointMass creation" +
	//"\nAfter clicking this button, move cursor to the desired point on body and click" +
	//"\nInternal PointMasses must be inside the body");
	public GUIContent deletePointMassContent = new GUIContent ("X");//, "Delete this PointMass");
	public GUIContent forceInternalContent = new GUIContent("Force Internal");

	SerializedProperty eEdgePointMasses;
	SerializedProperty eInternalPointMasses;

	public PointMassSubComponentEditor(Editor editor) : base(editor)
	{
		name = "Point Masses";
		eEdgePointMasses = editor.serializedObject.FindProperty("mEdgePointMasses");
		eInternalPointMasses = editor.serializedObject.FindProperty("mInternalPointMasses");
	}

	public override void DrawEditorGUI ()
	{
		//mainEditor.serializedObject.Update();

		multiEditing = mainEditor.serializedObject.isEditingMultipleObjects;

		if(multiEditing)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.HelpBox("Point Masses may not be edited when multiple GameObjects are selected", MessageType.Info);
			EditorGUI.indentLevel--;
			return;
		}

		if(GUILayout.Button(addPointMassContent, EditorStyles.miniButton))
		{
			newSubComponentState = AddSubComponentState.initiated;
		}

		if(newSubComponentState == AddSubComponentState.initiated)
		{
			EditorGUILayout.HelpBox("Click anywhere inside the Jello Body" +
				"\nEsc to cancel", MessageType.Info);
		}

		EditorGUI.indentLevel++;
		DrawEdgePointMassFoldout();
		DrawInternalPointMassFoldout();
		EditorGUI.indentLevel--;

		if(newSubComponentState != AddSubComponentState.inactive )
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

		DrawPointMassSceneGUI();
	}



	public void DrawInternalPointMassFoldout()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eInternalPointMasses, showInernalPointMassContent);
		if(EditorGUI.EndChangeCheck())
		{
			if(eInternalPointMasses.isExpanded)
				eEdgePointMasses.isExpanded = false;
			SetEditIndex(-1);
			SceneView.RepaintAll();
		}
		
		if(eInternalPointMasses.isExpanded)
		{	
			EditorGUI.indentLevel++;
			
			if(body.InternalPointMassCount != 0)
			{
				int offset = body.EdgePointMassCount;
				string text = "";
				string tooltip = "";
				
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));
				
				for(int i = 0; i < body.InternalPointMassCount; i++)
				{
					text = "Point Mass # " + (i + offset);
					
					tooltip = text +
						"\nMass = " + body.getInternalPointMass(i).Mass;
					
					EditorGUILayout.BeginHorizontal();

					SerializedProperty ePointMass = eInternalPointMasses.GetArrayElementAtIndex(i);
					ePointMass.isExpanded = editIndex == i + offset;

					GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
					if(mainEditor.serializedObject.FindProperty("mBaseShape").FindPropertyRelative("mInternalVertices").GetArrayElementAtIndex(i).prefabOverride || ePointMass.prefabOverride)
						foldoutStyle.fontStyle = FontStyle.Bold;

					EditorGUI.BeginChangeCheck();
				   	ePointMass.isExpanded = EditorGUILayout.Foldout(ePointMass.isExpanded, new GUIContent(text, tooltip), foldoutStyle);
					if(EditorGUI.EndChangeCheck())
					{
						if(!ePointMass.isExpanded)
							SetEditIndex(-1);
						else
							SetEditIndex(i + offset);
					}
					
					if(GUILayout.Button(deletePointMassContent, EditorStyles.miniButton, GUILayout.Width(20f)))
					{
						//body.removeInternalPointMass(i, false);
						body.smartRemoveInternalPointMass(i, false, JelloBody.ShapeSettingOptions.MovePointMasses, mainEditor.smartShapeSettingOptions);

						EditorUtility.SetDirty(body);

						break;
					}
					
					EditorGUILayout.EndHorizontal();

					if(ePointMass.isExpanded)
						DrawEditPointMassGUI(eInternalPointMasses, i, true);

					if(Event.current.type == EventType.Repaint && GUI.tooltip != prevTooltip)
					{
						//mouse out
						if(prevTooltip != "")
							drawIndex = -1;
						
						//mouse over
						if(GUI.tooltip != "")
							drawIndex =  i + offset;
						
						prevTooltip = GUI.tooltip;
						
						SceneView.RepaintAll();
					}
					
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUI.indentLevel--;
		}
	}

	public virtual void DrawEdgePointMassFoldout()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eEdgePointMasses, showEdgePointMassContent);
		if(EditorGUI.EndChangeCheck())
		{
			if(eEdgePointMasses.isExpanded)
				eInternalPointMasses.isExpanded = false;
			SetEditIndex(-1);
			SceneView.RepaintAll();
		}

		if(eEdgePointMasses.isExpanded)
		{
			EditorGUI.indentLevel++;
			
			if(body.EdgePointMassCount != 0)
			{
				string text = "";
				string tooltip = "";
				
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));
				for(int i = 0; i < body.EdgePointMassCount; i++)
				{
					text = "Point Mass # " + i;
					
					tooltip = text +
						"\nMass = " + body.getEdgePointMass(i).Mass;

					SerializedProperty ePointMass = eEdgePointMasses.GetArrayElementAtIndex(i);
					ePointMass.isExpanded = editIndex == i;

					GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
					if(mainEditor.serializedObject.FindProperty("mBaseShape").FindPropertyRelative("mEdgeVertices").GetArrayElementAtIndex(i).prefabOverride || ePointMass.prefabOverride)
						foldoutStyle.fontStyle = FontStyle.Bold;


					EditorGUI.BeginChangeCheck();
					//EditorGUILayout.PropertyField(ePointMass, new GUIContent(text, tooltip));
					ePointMass.isExpanded = EditorGUILayout.Foldout(ePointMass.isExpanded, new GUIContent(text, tooltip), foldoutStyle);
					if(EditorGUI.EndChangeCheck())
					{
						if(!ePointMass.isExpanded)
							SetEditIndex(-1);
						else if(ePointMass.isExpanded)
							SetEditIndex(i);
					}

					if(ePointMass.isExpanded)
						DrawEditPointMassGUI(eEdgePointMasses, i, false);
					
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
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUI.indentLevel--;
		}
	}

	public override void SetEditIndex (int index)
	{
		if(index >= 0 && index < body.PointMassCount)
		{
			if(index < body.EdgePointMassCount)
			{
				eEdgePointMasses.isExpanded = true;
				eInternalPointMasses.isExpanded = false;
				scrollPos.y = EditorGUIUtility.singleLineHeight * index;
			}
			else
			{
				eEdgePointMasses.isExpanded = false;
				eInternalPointMasses.isExpanded = true;
				scrollPos.y = EditorGUIUtility.singleLineHeight * (index - body.EdgePointMassCount);
			}

			editIndex = index;

			SceneView.RepaintAll();
		}
	}

	protected virtual void DrawEditPointMassGUI(SerializedProperty pointMassArray, int index, bool isInternal)
	{
		SerializedProperty eMass = pointMassArray.GetArrayElementAtIndex(index).FindPropertyRelative("mMass");
		SerializedProperty eMultiplier = pointMassArray.GetArrayElementAtIndex(index).FindPropertyRelative("shapeMatchingMultiplier");
		SerializedProperty eForceInternal = pointMassArray.GetArrayElementAtIndex(index).FindPropertyRelative("forceInternal");

		SerializedProperty ePosition; 
		if(isInternal)
			ePosition = mainEditor.serializedObject.FindProperty("mBaseShape").FindPropertyRelative("mInternalVertices").GetArrayElementAtIndex(index);
		else
			ePosition = mainEditor.serializedObject.FindProperty("mBaseShape").FindPropertyRelative("mEdgeVertices").GetArrayElementAtIndex(index);

		if(!body.IsStatic)
			EditorGUILayout.PropertyField(eMass, massContent);

		if(body.GetType() == typeof(JelloSpringBody) || body.GetType() == typeof(JelloPressureBody))
		{
			JelloSpringBody b = (JelloSpringBody)body;
			
			if(b.ShapeMatching)
				EditorGUILayout.PropertyField(eMultiplier, shapeMatchingMultiplierContent);
		}

		if(isInternal)
			EditorGUILayout.PropertyField(eForceInternal, forceInternalContent);

		GUIStyle style = new GUIStyle(EditorStyles.label);
		if(ePosition.prefabOverride)
			style.fontStyle = FontStyle.Bold;

		EditorGUILayout.LabelField("Position", body.transform.TransformPoint(body.Shape.getVertex(editIndex)).ToString(), style);
	}

	protected void DrawPointMassSceneGUI()
	{
		mainEditor.DrawPointMasses(body, true);

		Vector3 pos;
		
		//hovered over point mass in editor
		if(drawIndex >= 0 && drawIndex < body.PointMassCount && body.getPointMass(drawIndex).body != null)
		{
			pos = new Vector3(body.Shape.getVertex(drawIndex).x, body.Shape.getVertex(drawIndex).y, 0f);
			pos = body.transform.TransformPoint(pos);
			
			Handles.color = Color.cyan;
			Handles.DotCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f);
		}
		
		//selected point mass in editor
		if(editIndex != -1 && editIndex < body.PointMassCount && body.getPointMass(editIndex).body != null)
		{
			pos = new Vector3(body.Shape.getVertex(editIndex).x, body.Shape.getVertex(editIndex).y, 0f);
			pos = body.transform.TransformPoint(pos);
			
			Handles.color = Color.green;
			Handles.DotCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f);
		}
		
		//logic to add a new internal pointmass
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
				//body.addInternalPointMass(new JelloPointMass(body.Mass, mousePosWorld, body, false), false);
				body.smartAddInternalPointMass(new JelloPointMass(body.Mass, mousePosWorld, body, false), false, JelloBody.ShapeSettingOptions.MovePointMasses, mainEditor.smartShapeSettingOptions);

				EditorUtility.SetDirty(body);

				newSubComponentState = AddSubComponentState.inactive;
			}
			
			SceneView.RepaintAll();
		}
	}
}
