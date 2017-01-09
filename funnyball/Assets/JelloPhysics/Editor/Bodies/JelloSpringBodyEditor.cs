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
[CustomEditor(typeof(JelloSpringBody))]
public class JelloSpringBodyEditor : JelloBodyEditor 
{	
	public JelloSpringBody springTar;
	public static float percentSolidityChange;

	public GUIContent solidityContent = new GUIContent("Solidity");//, "Use the controls to the right to quickly adjust soft body settings");
	public GUIContent minusContent = new GUIContent("-  ");//, "Make this body softer");
	public GUIContent plusContent = new GUIContent("+ ");//, "Make this body harder");
	public GUIContent percentContent = new GUIContent(" % ");//, "Percent by which to change body solidity");


	public override void Enabled ()
	{
		base.Enabled ();
		
		springTar = (JelloSpringBody)serializedObject.targetObject;

		subEditors.Add (new SpringSubComponentEditor(this));
	}
	
	public override void DrawEditorGUI ()
	{
		base.DrawEditorGUI ();

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField(solidityContent, GUILayout.Width(50));
		
		if(GUILayout.Button(minusContent, EditorStyles.miniButton, GUILayout.Width(20)))
		{
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				JelloSpringBody sb = (JelloSpringBody)serializedObject.targetObjects[i];
				sb.modifySolidityByPercent(-percentSolidityChange);

				EditorUtility.SetDirty(sb);
			}
		}
		if(GUILayout.Button(plusContent, EditorStyles.miniButton, GUILayout.Width(20)))
		{
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				JelloSpringBody sb = (JelloSpringBody)serializedObject.targetObjects[i];
				sb.modifySolidityByPercent(percentSolidityChange);

				EditorUtility.SetDirty(sb);
			}
		}
		EditorGUIUtility.labelWidth = 20f;
		EditorGUIUtility.fieldWidth = 15f;
		percentSolidityChange = EditorGUILayout.FloatField (percentContent, Mathf.Clamp(percentSolidityChange, 0f, 100f));
		EditorGUIUtility.labelWidth = 0f;
		EditorGUIUtility.fieldWidth = 0f;

		EditorGUILayout.EndHorizontal();
	}


	public override void DrawSceneGUI ()
	{
		base.DrawSceneGUI ();
	}

	public override void CopyToPressureBody (JelloBody oldBody, JelloPressureBody newBody)
	{
		base.CopyToPressureBody (oldBody, newBody);
		
		JelloSpringBody old = (JelloSpringBody)oldBody;

		newBody.ShapeMatching = old.ShapeMatching;
		newBody.DefaultEdgeSpringStiffness = old.DefaultEdgeSpringStiffness;
		newBody.DefaultEdgeSpringDamping = old.DefaultEdgeSpringDamping;
		newBody.DefaultCustomSpringDamping = old.DefaultCustomSpringDamping;
		newBody.DefaultCustomSpringStiffness = old.DefaultCustomSpringStiffness;
		newBody.DefaultInternalSpringDamping = old.DefaultInternalSpringDamping;
		newBody.DefaultInternalSpringStiffness = old.DefaultInternalSpringStiffness;
		newBody.ShapeSpringDamping = old.ShapeSpringDamping;
		newBody.ShapeSpringStiffness = old.ShapeSpringStiffness;

		newBody.setInternalSprings(old.getInternalSprings());
		for(int i = 0; i < old.CustomSpringCount; i++)
		{
			newBody.addCustomSpring(old.getCustomSpring(i));
		}
	}
}
