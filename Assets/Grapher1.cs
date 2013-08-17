using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;

public class Grapher1 : MonoBehaviour {
	
	[Range(10, 100)]
	public float resolution = 100;
	public ParticleSystem.Particle[] points;
	public GameObject Axis;
	
	private int currentResolution;	
	private ParticleSystem.Particle[] errorPoints;
	private string equation = "2 * x";
	
	// Selection grid variables
	private string[] selGrid = {"Parabola", "Sine", "Custom"};
	private int selGridIndex = 0;
	
	void Start()
	{
		//eval = GetComponent<Evaluator>();
		Axis.SetActive(false);
		
		errorPoints = new ParticleSystem.Particle[100];
		errorPoints[0].position = new Vector3(0,0.4f,0);
		errorPoints[1].position = new Vector3(0,0.3f,0);
		errorPoints[2].position = new Vector3(0,0.2f,0);
		errorPoints[3].position = new Vector3(0,0.1f,0);
		errorPoints[4].position = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (currentResolution != resolution || points == null)
		{
			CreatePoints();
		}
		
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 p = points[i].position;
			switch (selGridIndex)
			{
			case 0:
				p.y = Parabola(p);
				break;
			case 1:
				p.y = Sine(p);
				break;
			}
			points[i].position = p;
			Color c = points[i].color;
			c.g = p.y;
			points[i].color = c;
		}

		particleSystem.SetParticles(points, points.Length);
	}
	
	void CreatePoints()
	{
		try
		{
			currentResolution = (int)resolution;
			points = new ParticleSystem.Particle[currentResolution * currentResolution];
			float increment = 1f / (currentResolution - 1);
			int i = 0;
			for (int x = 0; x < currentResolution; x++)
			{
				for (int z = 0; z < currentResolution; z++)
				{
					Vector3 p = new Vector3(x * increment, 0f, z * increment);
					p.y = EvalutateEquation(equation.Replace("x", p.x.ToString()).Replace("z", p.z.ToString())
						.Trim().Replace(" ", ""));
					points[i].position = p;
					points[i].color = new Color(p.x, 0, p.z);
					points[i++].size = 0.1f;
				}
			}
		}
		catch
		{
			points = errorPoints;
		}
	}
	
	void OnGUI()
	{
		// Equation entry box and "Graph" button
		equation = GUI.TextField(new Rect(10, 10, 200, 30), equation.ToLower());
		if (GUI.Button(new Rect(220, 10, 60, 30), "Graph"))
		{
			CreatePoints();
		}
		
		// Slider to set Resolution
		resolution = GUI.HorizontalSlider (new Rect(10, 50, 200, 30), resolution, 10, 100);
		GUI.Label(new Rect(220, 50, 200, 30), "Resolution");
		
		// Axis toggle button
		if (GUI.Button(new Rect(10, 90, 200, 30), "Toggle Axis"))
		{
			Axis.SetActive(!Axis.activeInHierarchy);
		}
		
		// Selection grid
		selGridIndex = GUI.SelectionGrid(new Rect(10, 140, 200, 30), selGridIndex, selGrid, selGrid.Length);
	}
	
	#region Private Logic
	
	private float EvalutateEquation(string eq)
	{
		eq = eq.ToLowerInvariant();
		if (eq.Contains("="))
		{
			eq = eq.Substring(eq.IndexOf("=") + 1);	
		}
		
		if (eq.Contains("sin("))
		{
			int index = eq.IndexOf("sin(");
			int endIndex = eq.Substring(index).IndexOf(")");
			if (index == 0)
			{
				string ans = Mathf.Sin(float.Parse(eq.Substring(4, endIndex - 1))).ToString() + eq.Substring(endIndex + 1);	
				Debug.Log(ans.ToString());
				return EvalutateEquation(ans);
			}
			else
			{
				return EvalutateEquation(eq.Substring(0, index)) + EvalutateEquation(eq.Substring(index + 1));	
			}
		}
		else if (eq.Contains("^"))
		{
			int index = eq.IndexOf("^");
			return Mathf.Pow(EvalutateEquation(eq.Substring(0, index)), eq[index + 1]);	
		}
		else if (eq.Contains("*"))
		{
			int index = eq.IndexOf("*");
			return EvalutateEquation(eq.Substring(0, index)) * EvalutateEquation(eq.Substring(index + 1));	
		}
		else if (eq.Contains("/"))
		{
			int index = eq.IndexOf("/");
			return EvalutateEquation(eq.Substring(0, index)) / EvalutateEquation(eq.Substring(index + 1));	
		}
		else if (eq.Contains("+"))
		{
			int index = eq.IndexOf("+");
			return EvalutateEquation(eq.Substring(0, index)) + EvalutateEquation(eq.Substring(index + 1));	
		}
		else if (eq.Contains("-"))
		{
			int index = eq.IndexOf("-");
			return EvalutateEquation(eq.Substring(0, index)) - EvalutateEquation(eq.Substring(index + 1));	
		}
		
		return float.Parse(eq);
    }  
		
	private static float Linear (float x)
	{
		return x;	
	}
	
	private static float Parabola (Vector3 p){
		p.x += p.x - 1f;
		p.z += p.z - 1f;
		return 1f - p.x * p.x * p.z * p.z;
	}
	
	private static float Sine (Vector3 p){
		float t = Time.timeSinceLevelLoad;
		return 0.50f +
			0.25f * Mathf.Sin(4f * Mathf.PI * p.x + 4f * t) * Mathf.Sin(2f * Mathf.PI * p.z + t) +
			0.10f * Mathf.Cos(3f * Mathf.PI * p.x + 5f * t) * Mathf.Cos(5f * Mathf.PI * p.z + 3f * t) +
			0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}
	
	#endregion
}
