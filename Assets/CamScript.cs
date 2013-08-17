using UnityEngine;
using System.Collections;

public class CamScript : MonoBehaviour {
	
	public Rect bounds;
	public float scaleFactor = 0.1f;
	public float dragSpeed = 2;
	
	private Vector3 origin = new Vector3(0.5f, 0, 0.5f);
	private Vector2 mousePos = Vector2.zero;
	private Grapher1 grapher;
	private ParticleSystem.Particle[] currentPoints;
	private bool camLock = false;
	private bool canCamLock = true;
	private Vector3 dragOrigin;
	
	// Use this for initialization
	void Start () 
	{
		grapher = GameObject.Find("Graph1").GetComponent<Grapher1>();
		currentPoints = grapher.points;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		float mouseX = Input.GetAxis("Mouse X") * scaleFactor;
		float mouseY = Input.GetAxis("Mouse Y") * scaleFactor;
		float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * 3;
		
		/*
		if (grapher.points != currentPoints)
		{
			currentPoints = grapher.points;
			float xMin = Mathf.Infinity;
			float yMin = Mathf.Infinity;
			float xMax = -Mathf.Infinity;
			float yMax = -Mathf.Infinity;
			
			foreach (ParticleSystem.Particle point in currentPoints)
			{
				if (point.position.x < xMin)
				{
					xMin = point.position.x;	
				}
				if (point.position.y < yMin)
				{
					yMin = point.position.y;	
				}
				if (point.position.x > xMax)
				{
					xMax = point.position.x;	
				}
				if (point.position.y > yMax)
				{
					yMax = point.position.y;	
				}
			}
			
			origin.x = (xMin + xMax) / 2;
			origin.y = (yMin + yMax) / 2;
			
		}
		*/
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			camLock = !camLock;
			GameObject goOld = GameObject.Find("text");
			if (goOld != null)
			{
				Destroy(goOld);	
			}
			
			GameObject go = new GameObject("text");
			GUIText gt = go.AddComponent<GUIText>() as GUIText;
			gt.fontSize = 24;
			gt.anchor = TextAnchor.MiddleCenter;
			
			go.transform.position = new Vector3(0.5f, 0.3f, 0);
			
			GameObject.Destroy(go, 2);
			
			if (camLock)
			{
				gt.text = "Cam locked. Press SPACE to unlock or RIGHT-CLICK to pan";
			}
			else
			{
				gt.text = "Cam unlocked. Press SPACE to lock";
			}
		}
		
		if (camLock)
		{
			if (Input.GetMouseButtonDown(1))
	        {
				dragOrigin = Input.mousePosition;
	        }
			
			if (Input.GetMouseButton(1))
			{
				Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
	       		Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
				transform.Translate(move, Space.World);  
			}
		}
		else
		{
			if (mouseX != 0)
			{
				if (bounds.Contains(mousePos + new Vector2(mouseX, 0)))
				{
					mousePos.x = mousePos.x + mouseX;	
					Vector3 transPos = transform.position;
					transPos.x = -mousePos.x;
					transform.position = Vector3.Lerp(transform.position, transPos, 20*Time.deltaTime);
					transform.LookAt(origin);
				}
			}
			
			if (mouseY != 0)
			{
				if (bounds.Contains(mousePos + new Vector2(0, mouseY)))
				{
					mousePos.y = mousePos.y + mouseY;	
					Vector3 transPos = transform.position;
					transPos.y = -mousePos.y;
					transform.position = Vector3.Lerp(transform.position, transPos, 20*Time.deltaTime);
					transform.LookAt(origin);
				}
			}
		}
		
		if (mouseWheel != 0)
		{
			Vector3 transPos = transform.position;
			transPos.z += mouseWheel;
			transform.position = Vector3.Lerp(transform.position, transPos, 20*Time.deltaTime);
			transform.LookAt(origin);
		}
	}
}
