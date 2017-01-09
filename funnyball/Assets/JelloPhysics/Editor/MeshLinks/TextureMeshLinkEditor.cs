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

[CanEditMultipleObjects]
[CustomEditor(typeof(TextureMeshLink))]
public class TextureMeshLinkEditor : MeshLinkEditor 
{
	GUIContent updateMeshContent = new GUIContent("Update Mesh", "Make the mesh conform to the collider");
	SerializedProperty eTexture;

	protected override void Enabled()
	{
		base.Enabled();

		eTexture = serializedObject.FindProperty("texture");
	}


	protected override void DrawInspectorGUI()
	{
		base.DrawInspectorGUI();	

		serializedObject.Update();

		DrawOffsetScaleAngleEditorGUI();


		EditorGUI.showMixedValue = eTexture.hasMultipleDifferentValues;
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eTexture);
		if(EditorGUI.EndChangeCheck())
		{
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				TextureMeshLink link = (TextureMeshLink)serializedObject.targetObjects[i];


				Undo.RecordObject(link.gameObject.GetComponent<MeshRenderer>(), "TML renderer change");
				Undo.RecordObject(link.gameObject.GetComponent<MeshFilter>(), "TML filter change");

				if(eTexture.objectReferenceValue != null)
				{
					link.texture = (Texture2D)eTexture.objectReferenceValue;

					MeshRenderer renderer = link.GetComponent<MeshRenderer>();
					if(renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture != (Texture2D)eTexture.objectReferenceValue)
					{
						if(renderer.sharedMaterial == null)
							link.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
						
						link.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = (Texture2D)eTexture.objectReferenceValue;
					}
				}
				else
				{
					link.GetComponent<MeshRenderer>().sharedMaterial = null;
					link.LinkedMeshFilter.mesh = null;
				}

				
				link.Initialize(true);

				EditorUtility.SetDirty(link);
			}
		}
		EditorGUI.showMixedValue = false;


		if(GUILayout.Button(updateMeshContent, EditorStyles.miniButton))
		{
			Undo.RecordObjects(serializedObject.targetObjects, "TML update");

			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				TextureMeshLink link = (TextureMeshLink)serializedObject.targetObjects[i];

				Undo.RecordObject(link.gameObject.GetComponent<MeshRenderer>(), "TML renderer change");
				Undo.RecordObject(link.gameObject.GetComponent<MeshFilter>(), "TML filter change");
				
				link.Initialize(true);

				EditorUtility.SetDirty(link);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

}
