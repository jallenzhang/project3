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
[CustomEditor(typeof(JelloPressureBody))]
public class JelloPressureBodyEditor : JelloSpringBodyEditor 
{
	SerializedProperty eGasAmount;
	
	public GUIContent pressureContent = new GUIContent("Amount Of Gas");//, "The amount of gas filling this body");
	
	public override void Enabled ()
	{
		base.Enabled ();
		
		eGasAmount = serializedObject.FindProperty("mGasAmount");
	}
	
	public override void DrawEditorGUI ()
	{
		base.DrawEditorGUI ();

		serializedObject.Update();
		
		EditorGUI.showMixedValue = eGasAmount.hasMultipleDifferentValues;
		EditorGUILayout.PropertyField(eGasAmount, pressureContent);
		EditorGUI.showMixedValue = false;
		
		serializedObject.ApplyModifiedProperties();
	}
	
	public override void CopyToSpringBody (JelloBody oldBody, JelloSpringBody newBody)
	{
		base.CopyToSpringBody (oldBody, newBody);
		
		JelloPressureBody old = (JelloPressureBody)oldBody;

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
