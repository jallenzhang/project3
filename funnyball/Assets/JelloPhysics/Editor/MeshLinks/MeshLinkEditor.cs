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
using UnityEditorInternal;
using System.Reflection;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshLink))]
public class MeshLinkEditor :  Editor
{
	
	SerializedProperty eOrder;
	SerializedProperty eLayer;
	SerializedProperty eNormals;
	SerializedProperty eTangents;
	protected SerializedProperty eScale;
	protected SerializedProperty eAngle;
	protected SerializedProperty eOffset;

	private GUIContent sortingLayerContent = new GUIContent("Sorting Layer");
	private GUIContent sortingOrderContent = new GUIContent("Order in Layer");
	private GUIContent[] sortingPopupLayerContent;
	private int[] sortingLayerIDs;

	void OnEnable()
	{
		Enabled();
	}
	
	public override void OnInspectorGUI()
	{
		DrawInspectorGUI();
	}


	protected virtual void Enabled()
	{
		eNormals = serializedObject.FindProperty("CalculateNormals");
		eTangents = serializedObject.FindProperty("CalculateTangents");
		eScale = serializedObject.FindProperty("scale");
		eAngle = serializedObject.FindProperty("angle");
		eOffset = serializedObject.FindProperty("offset");
		eLayer = serializedObject.FindProperty("mSortingLayer");
		eOrder = serializedObject.FindProperty("mSortingOrder");



		string[] sortingLayerNames = GetSortingLayerNames();
		sortingLayerIDs = GetSortingLayerUniqueIDs();

		sortingPopupLayerContent = new GUIContent[sortingLayerNames.Length];
		for(int i = 0; i < sortingLayerNames.Length; i++)
		{
			sortingPopupLayerContent[i] = new GUIContent(sortingLayerNames[i]);
		}
	}

	protected virtual void DrawInspectorGUI()
	{
		serializedObject.Update();

		EditorGUI.showMixedValue = eLayer.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.IntPopup(eLayer, sortingPopupLayerContent, sortingLayerIDs, sortingLayerContent);
		if(EditorGUI.EndChangeCheck())
		{
			int id = 0;
			for(int i = 0; i < sortingLayerIDs.Length; i++)
			{
				if(eLayer.intValue == sortingLayerIDs[i])
				{
					id = i;
					break;
				}
			}

			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				MeshLink link  = (MeshLink)serializedObject.targetObjects[i];
				link.GetComponent<Renderer>().sortingLayerName = sortingPopupLayerContent[id].text;

				EditorUtility.SetDirty(link.GetComponent<Renderer>());
			}
		}
		EditorGUI.showMixedValue = false;

		EditorGUI.showMixedValue = eOrder.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eOrder, sortingOrderContent);
		if(EditorGUI.EndChangeCheck())
		{
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				MeshLink link  = (MeshLink)serializedObject.targetObjects[i];

				link.GetComponent<Renderer>().sortingOrder = eOrder.intValue;
				EditorUtility.SetDirty(link.GetComponent<Renderer>());
			}
		}
		EditorGUI.showMixedValue = false;

		EditorGUI.showMixedValue = eNormals.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eNormals);
		if(EditorGUI.EndChangeCheck())
		{
			if(!eNormals.boolValue)
				eTangents.boolValue = false;
		}
		EditorGUI.showMixedValue = false;

		EditorGUI.showMixedValue = eTangents.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eTangents);
		if(EditorGUI.EndChangeCheck())
		{
			if(eTangents.boolValue)
				eNormals.boolValue = true;
		}
		EditorGUI.showMixedValue = false;

		serializedObject.ApplyModifiedProperties();
	}


	protected string[] GetSortingLayerNames() 
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	
	protected int[] GetSortingLayerUniqueIDs() 
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
	}

	protected void DrawOffsetScaleAngleEditorGUI()
	{
		EditorGUI.showMixedValue = eOffset.hasMultipleDifferentValues; //TODO check what im setting dirty too...
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eOffset, new GUIContent("Offset"));
		if(EditorGUI.EndChangeCheck())
		{
			HandleOffsetChanged();
		}
		EditorGUI.showMixedValue = false;
		
		EditorGUI.showMixedValue = eScale.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eScale, new GUIContent("Scale"));
		if(EditorGUI.EndChangeCheck())
		{
			HandleScaleChanged();

		}
		EditorGUI.showMixedValue = false;
		
		EditorGUI.showMixedValue = eAngle.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eAngle, new GUIContent("Angle"));
		if(EditorGUI.EndChangeCheck())
		{
			HandleAngleChanged();
		}
	}

	protected virtual void HandleOffsetChanged()
	{
		for(int i = 0; i < serializedObject.targetObjects.Length; i++)
		{
			MeshLink link = (MeshLink)serializedObject.targetObjects[i];

			link.ApplyNewOffset(eOffset.vector2Value);
			
			EditorUtility.SetDirty(link.body);
			EditorUtility.SetDirty(link.LinkedMeshFilter.sharedMesh);
		}
	}
	
	protected virtual void HandleScaleChanged()
	{
		for(int i = 0; i < serializedObject.targetObjects.Length; i++)
		{
			MeshLink link = (MeshLink)serializedObject.targetObjects[i];

			link.scale = eScale.vector2Value;

			link.Initialize(true);
			
			EditorUtility.SetDirty(link.body);
			EditorUtility.SetDirty(link.LinkedMeshFilter.sharedMesh);
		}
	}
	
	protected virtual void HandleAngleChanged()
	{
		for(int i = 0; i < serializedObject.targetObjects.Length; i++)
		{
			MeshLink link = (MeshLink)serializedObject.targetObjects[i];

			link.angle = eAngle.floatValue;

			link.Initialize(true);
			
			EditorUtility.SetDirty(link.body);
			EditorUtility.SetDirty(link.LinkedMeshFilter.sharedMesh);
		}
	}
}
