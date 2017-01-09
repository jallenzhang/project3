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

using System;
using UnityEngine;

/// <summary>
/// Some helpful math tools.
/// </summary>
public static class JelloMathTools
{
	#region QUADRATIC FUNCITONS 

	//TODO handle a=0 in both cases...

//	public static bool SolveQuadratic(double a, double b, double c, out float x1, out float x2)	
//	{
//		Debug.Log ("a " + a + " b " + b + " c " + c);
//
//		double sqrtpart = b * b - 4 * a * c;
//		x1 = x2 = float.NaN;
//		double x, img = 0;
//
//		if(a == 0 && b == 0)
//		{	
//			Debug.Log("No Solution: ");	
//			return false;
//		}
//		
//		if (sqrtpart > 0)	 //this will fail if a = 0...
//		{
//			x1 = (float)((-b + System.Math.Sqrt(sqrtpart)) / (2 * a));
//			
//			x2 = (float)((-b - System.Math.Sqrt(sqrtpart)) / (2 * a));
//			
//			//Debug.Log("Two Real Solutions: " + x1 + " or " + x2);	
//
//			return true;
//		}
//		else if (sqrtpart < 0)	
//		{
//			sqrtpart = -sqrtpart;
//			
//			x = -b / (2 * a);
//			
//			img = System.Math.Sqrt(sqrtpart) / (2 * a);
//
//			//Debug.Log("Two Imaginary Solutions: "  + x + " + " + img + "i or " + x + " - " + img + "i ");
//			return false;
//		}
//		else	
//		{	
//			x1 = (float)((-b + System.Math.Sqrt(sqrtpart)) / (2 * a));
//			//Debug.Log("One Real Solution: " + x1);	
//
//			return true;
//		}
//	}

	public static void SolveQuadratic(double a, double b, double c, out float x1, out float x2)
	{
		if(Math.Abs(a) < 0.000001)    // ==0
		{
			if(Math.Abs(b) > 0.000001)  // !=0
			{
				x1 = x2 = (float)( -c / b);
				return;
			}
			else// if(Math.Abs(c) > 0.00001)
			{
				x1=x2 = float.NaN;
				return;
			}
		}

		//Calculate the inside of the square root
		double insideSquareRoot = (b * b) - 4 * a * c;
		
		if (insideSquareRoot < 0)
		{
			//There is no solution
			x1 = float.NaN;
			x2 = float.NaN;
		}
		else
		{
			//Compute the value of each x
			//if there is only one solution, both x's will be the same
			double t = (-0.5 * (b + Math.Sign(b) * Math.Sqrt(insideSquareRoot)));
			x1 = (float)(c / t);
			x2 = (float)(t / a);
		}
	}

//	public static void SolveQuadratic(double a, double b, double c, out float x1, out float x2)
//	{
//		//Quadratic Formula: x = (-b +- sqrt(b^2 - 4ac)) / 2a
//		
//		//Calculate the inside of the square root
//		double insideSquareRoot = (b * b) - 4 * a * c;
//		
//		if (insideSquareRoot < 0)
//		{
//			//There is no solution
//			x1 = double.NaN;
//			x2 = double.NaN;
//		}
//		else
//		{
//			//Compute the value of each x
//			//if there is only one solution, both x's will be the same
//			double sqrt = Math.Sqrt(insideSquareRoot);
//			x1 = (-b + sqrt) / (2 * a);
//			x2 = (-b - sqrt) / (2 * a);
//		}
//	}

	#endregion
}