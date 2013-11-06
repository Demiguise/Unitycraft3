using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugGUI : MonoBehaviour {
	
	public bool debugEnable = true;
	private bool dragging = false;

	private List<Vector3> vertList = new List<Vector3>();
	
	public Material mat;
	private Vector3 originalMousePosition;
	private Vector3 curMousePosition;
	
	public Texture cursorTexture;
	
	void Start () {
		Screen.showCursor = false;
	}
	
	void OnGUI () {
		Rect cursorRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorTexture.width, cursorTexture.height);
		GUI.DrawTexture( cursorRect, cursorTexture);
	}
	
	void Update(){
		Vector3 mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0)) {
			originalMousePosition = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);
		}
		if (Input.GetMouseButton(0)) {
			curMousePosition = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);
			PopulateVertList();
			dragging = true;
		}
		if (Input.GetMouseButtonUp(0) ){
			dragging = false;
			originalMousePosition = Vector3.zero;
			curMousePosition = Vector3.zero;
		}
	}
	
	void PopulateVertList(){
		vertList.Clear();
		vertList.Add(originalMousePosition);
		vertList.Add(new Vector3(curMousePosition[0], originalMousePosition[1], 0));
		vertList.Add(curMousePosition);
		vertList.Add(new Vector3(originalMousePosition[0], curMousePosition[1], 0));
		vertList.Add(originalMousePosition);
	}
	
   	void OnPostRender() {
        if (!mat) {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
		if (dragging) {
			for (int i = 0 ; i < vertList.Count -1 ; i++ ) {
		        GL.PushMatrix();
		        mat.SetPass(0);
		        GL.LoadOrtho();
		        GL.Begin(GL.LINES);
		        GL.Color(Color.red);
		        GL.Vertex(vertList[i]);
		        GL.Vertex(vertList[i+1]);
		        GL.End();
		        GL.PopMatrix();
			}
		}
    }
}
