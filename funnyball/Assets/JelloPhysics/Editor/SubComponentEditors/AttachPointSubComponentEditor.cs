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

public class AttachPointSubComponentEditor : SubComponentEditor 
{
	public GUIContent addAttachPointContent = new GUIContent ("Add Attach Point");
	public GUIContent showAttachPointContent = new GUIContent("Attach Points");
	public GUIContent deleteAttachPointContent = new GUIContent ("X");//, "Delete this AttachPoint");

	SerializedProperty eAttachPoints;

	public AttachPointSubComponentEditor(Editor editor) : base(editor)
	{
		name = "Attach Point";
		handlePositions = new Vector3[4];
		handleSizes = new float[4];
		eAttachPoints = editor.serializedObject.FindProperty("mAttachPoints");
	}

	public override void DrawEditorGUI ()
	{
		multiEditing = mainEditor.serializedObject.isEditingMultipleObjects;

		if(multiEditing)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.HelpBox("Attach Points may not be edited when multiple GameObjects are selected", MessageType.Info);
			EditorGUI.indentLevel--;
			return;
		}

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button(addAttachPointContent, EditorStyles.miniButton))
		{
			newSubComponentState = AddSubComponentState.initiated;
		}

		if(GUILayout.Button(new GUIContent("Rebuild All"), EditorStyles.miniButton))
		{
			for(int i = 0; i < body.AttachPointCount; i++)
				body.GetAttachPoint(i).Rebuild(body.GetAttachPoint(i).point, body);
			
			if(editIndex != -1)
				SetEditIndex(editIndex);

			EditorUtility.SetDirty(body);

			SceneView.RepaintAll();
		}

		EditorGUILayout.EndHorizontal();


		if(newSubComponentState == AddSubComponentState.initiated)
		{
			EditorGUILayout.HelpBox("Click anywhere around the Jello Body" +
			                        "\nEsc to cancel", MessageType.Info);
		}

		EditorGUI.indentLevel++;

		string text = "";
		string tooltip = "";

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));
	
		//mainEditor.serializedObject.Update();

		for(int i = 0; i < body.AttachPointCount; i++)
		{
			text = "Attach Point # " + i;

			tooltip = text  +
				"\nindices: ";
			for(int n = 0; n < body.GetAttachPoint(i).affectedIndices.Length; n++)
			{
				tooltip +=  body.GetAttachPoint(i).affectedIndices[n] + ", ";
			}
			
			EditorGUILayout.BeginHorizontal();


			SerializedProperty eAttachPoint = eAttachPoints.GetArrayElementAtIndex(i);
			eAttachPoint.isExpanded = editIndex == i;
	
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(eAttachPoint, new GUIContent(text, tooltip));
			if(EditorGUI.EndChangeCheck())
			{
				if(!eAttachPoint.isExpanded)
					editIndex = -1;
				else if(eAttachPoint.isExpanded)
				{
					editIndex = i;
					SetEditIndex(i);
					SceneView.RepaintAll();
				}
			}
			
				
			//if(GUILayout.Button("↻", EditorStyles.miniButton, GUILayout.Width(20)))
			if(GUILayout.Button("R", EditorStyles.miniButton, GUILayout.Width(20)))
			{
				body.GetAttachPoint(i).Rebuild (body.GetAttachPoint(i).point,  body);
				
				if(editIndex == i)
					SetEditIndex(i);

				EditorUtility.SetDirty(body);
				
				SceneView.RepaintAll();
			}
				
			if(GUILayout.Button(deleteAttachPointContent, EditorStyles.miniButton, GUILayout.Width(20)))
			{
				body.RemoveAttachPoint(body.GetAttachPoint(i));
				
				if(i == editIndex)
					editIndex = -1;

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
					drawIndex =  i;
				
				prevTooltip = GUI.tooltip;
				
				SceneView.RepaintAll();
			}
				
			if(eAttachPoint.isExpanded)
				DrawEditAttachPointGUI();

		}

		EditorGUILayout.EndScrollView();

		if(newSubComponentState == AddSubComponentState.initiated)
		{
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape) //TODO include other hotkeys
			{
				newSubComponentState = AddSubComponentState.inactive;
				mainEditor.Repaint();
			}
		}

		//mainEditor.serializedObject.ApplyModifiedProperties();
	}

	public override void DrawSceneGUI ()
	{
		//modifiedInScene = false;

		if(multiEditing)
			return;

		DrawAttachPointSceneGUI();
	}

	public override void SetEditIndex(int index)
	{
		if(index >= body.AttachPointCount || index < 0)
			return;

		editIndex = index;

		handlePositions[0] = body.GetAttachPoint(editIndex).point;

		for(int i = 1; i < body.GetAttachPoint(index).affectedIndices.Length + 1; i++)
			handlePositions[i] = body.Shape.getVertex(body.GetAttachPoint(editIndex).affectedIndices[i - 1]);

		SceneView.RepaintAll();
	}

	public virtual void DrawEditAttachPointGUI()
	{
		SerializedProperty eAttachPoint = eAttachPoints.GetArrayElementAtIndex(editIndex);
		SerializedProperty eAttachedTransform = eAttachPoint.FindPropertyRelative("mAttachedTransform");
		SerializedProperty eRotate = eAttachPoint.FindPropertyRelative("rotate");
		SerializedProperty eAngle = eAttachPoint.FindPropertyRelative("transformAngle");
		SerializedProperty eIndices = eAttachPoint.FindPropertyRelative("affectedIndices");

		JelloAttachPoint attachPoint = body.GetAttachPoint(editIndex); 

		EditorGUILayout.PropertyField(eAttachedTransform, new GUIContent("Attached Transform"));//TODO move guicontents to top.

		if(eAttachedTransform.objectReferenceValue != null)
		{
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(eRotate, new GUIContent("Rotate"));

			if(eRotate.boolValue)
				EditorGUILayout.PropertyField(eAngle, new GUIContent("Angle"));

			EditorGUI.indentLevel--;
		}

		GUIStyle positionStyle = new GUIStyle(EditorStyles.label);
		if(eAttachPoint.FindPropertyRelative("point").prefabOverride)
		{
			//positionStyle = new GUIStyle(EditorStyles.boldLabel);
			positionStyle.fontStyle = FontStyle.Bold;
		}
		EditorGUILayout.LabelField("Local Position", handlePositions[0].ToString(), positionStyle);

		string contents = "";
		for(int i = 0; i < eIndices.arraySize; i++)
			contents += eIndices.GetArrayElementAtIndex(i).intValue.ToString() + ", ";

		EditorGUI.indentLevel++;
		EditorGUILayout.BeginHorizontal();
		GUIContent addLegContent = new GUIContent("+");
		GUIContent removeLegContent = new GUIContent("-");

		GUIStyle indicesStyle = new GUIStyle(EditorStyles.label);
		if(eIndices.prefabOverride)
		{
			//indicesStyle = new GUIStyle(EditorStyles.boldLabel);
			indicesStyle.fontStyle = FontStyle.Bold;
		}
		EditorGUILayout.LabelField("Indices", contents, indicesStyle);

		if(GUILayout.Button(removeLegContent) && attachPoint.affectedIndices.Length > 1)
		{
			attachPoint.Rebuild(attachPoint.point, attachPoint.body, true, attachPoint.affectedIndices.Length - 1);

			EditorUtility.SetDirty(body);

			SetEditIndex(editIndex);

			SceneView.RepaintAll();
		}
		if(GUILayout.Button(addLegContent) && attachPoint.affectedIndices.Length < 3)
		{
			attachPoint.Rebuild(attachPoint.point, attachPoint.body, true, attachPoint.affectedIndices.Length + 1);

			EditorUtility.SetDirty(body);
	
			SetEditIndex(editIndex);

			SceneView.RepaintAll();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
	}

	public void DrawAttachPointSceneGUI()
	{
		//TODO i have an error when reverting to prefab. If the handles are ind a different place, it sets dirty and messes up the seleced edit attach point.
		mainEditor.DrawPointMasses(body, false);

		Vector3 pos;
		
		//the attach point hovered over in the editor
		if(drawIndex >= 0 && drawIndex < body.AttachPointCount && body.GetAttachPoint(drawIndex).body != null)
		{
			pos = body.transform.TransformPoint(body.GetAttachPoint(drawIndex).point);

			Handles.color = Color.magenta;
			for(int i = 0; i < body.GetAttachPoint(drawIndex).affectedIndices.Length; i++)
			{
				Handles.DrawLine(pos, body.transform.TransformPoint(body.Shape.getVertex(body.GetAttachPoint(drawIndex).affectedIndices[i])));
				Handles.DotCap(3, 
				               body.transform.TransformPoint(body.Shape.getVertex(body.GetAttachPoint(drawIndex).affectedIndices[i])), 
				               Quaternion.identity, 
				               HandleUtility.GetHandleSize(body.transform.TransformPoint(body.Shape.getVertex(body.GetAttachPoint(drawIndex).affectedIndices[i]))) * 0.05f);
			}
			Handles.color = Color.blue;
			Handles.DotCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.075f);
		}
		
		//the attach point currently selected in the editor
		if(editIndex != -1 && editIndex < body.AttachPointCount && body.GetAttachPoint(editIndex).body != null)
		{	
			//handle sizes
			int num = body.GetAttachPoint(editIndex).affectedIndices.Length + 1;
			Vector3[] globalHandlePositions = new Vector3[num];
			globalHandlePositions[0] = body.transform.TransformPoint(handlePositions[0]);
			for(int i = 0; i < body.GetAttachPoint(editIndex).affectedIndices.Length; i++)
				globalHandlePositions[i + 1] = body.transform.TransformPoint(handlePositions[i + 1]);

			CalculateHandleSizes(globalHandlePositions);

			bool mouseUp = false;
			
			if(Event.current.type == EventType.mouseUp)
				mouseUp = true;
			
			Handles.color = Color.green;
			EditorGUI.BeginChangeCheck();
			handlePositions[0] = body.transform.InverseTransformPoint( Handles.FreeMoveHandle(body.transform.TransformPoint(handlePositions[0]), Quaternion.identity, handleSizes[0], Vector3.zero, Handles.CircleCap));
			if(EditorGUI.EndChangeCheck())
			{
				body.GetAttachPoint(editIndex).Rebuild(handlePositions[0], body, body.GetAttachPoint(editIndex).affectedIndices);
				SetEditIndex(editIndex);

				EditorUtility.SetDirty(body);
				//modified in scene
			}
		
			Handles.color = Color.white;
			Handles.DotCap(3, body.transform.TransformPoint(handlePositions[0]), Quaternion.identity, handleSizes[0] * 0.5f);
			//start at one because the point occupies the first position.
			//for(int i = 1; i < handlePositions.Length; i++)
			for(int i = 1; i < body.GetAttachPoint(editIndex).affectedIndices.Length + 1; i++)
			{
				Handles.color = Color.blue;
				handlePositions[i] = body.transform.InverseTransformPoint( Handles.FreeMoveHandle(body.transform.TransformPoint(handlePositions[i]), Quaternion.identity, handleSizes[i], Vector3.zero, Handles.CircleCap));
			
				if(mouseUp)
				{
					if((Vector2)handlePositions[i] != body.Shape.getVertex(body.GetAttachPoint(editIndex).affectedIndices[i - 1]))
					{
						Vector2[] points = new Vector2[body.Shape.VertexCount];
						for(int s = 0; s < body.Shape.VertexCount; s++)
							points[s] = body.Shape.getVertex(s);
						
						int index = JelloShapeTools.FindClosestVertexOnShape(handlePositions[i], points);

						bool occupied = false;
						for(int n = 0; n < body.GetAttachPoint(editIndex).affectedIndices.Length; n++)
							if(index == body.GetAttachPoint(editIndex).affectedIndices[n])
								occupied = true;

						if(!occupied)
						{
							body.GetAttachPoint(editIndex).affectedIndices[i - 1] = index;
							
							handlePositions[i] = body.Shape.getVertex(index);
							
							body.GetAttachPoint(editIndex).Rebuild(body.GetAttachPoint(editIndex).point, body, body.GetAttachPoint(editIndex).affectedIndices);

							handlePositions[0] = body.GetAttachPoint(editIndex).point;

							EditorUtility.SetDirty(body);
						}
						else
						{
							handlePositions[i] = body.Shape.getVertex(body.GetAttachPoint(editIndex).affectedIndices[i - 1]);
						}
					}
				}

				Handles.color = Color.black;
				Handles.DrawLine(body.transform.TransformPoint(handlePositions[0]), body.transform.TransformPoint( handlePositions[i]));
			}
		}
		
		//logic to add a new attach point
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
			
			if(Event.current.type == EventType.MouseUp)//TODO not working correctly...?
			{
				body.AddAttachPoint(new JelloAttachPoint(body.transform.InverseTransformPoint(mousePosWorld), body, true));
				newSubComponentState = AddSubComponentState.inactive;
				SetEditIndex(body.AttachPointCount - 1);

				EditorUtility.SetDirty(body);
			}
			
			SceneView.RepaintAll();
		}
	}
}
