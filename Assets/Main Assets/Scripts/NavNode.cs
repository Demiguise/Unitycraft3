using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft;

public class NavNode {
	
	public Vector3 nodePosition;
	public Vector3 nodeExtents;
    public List<Vector3> nodeVertices = new List<Vector3>();
	public List<NavNode> linkedNodes = new List<NavNode>();
	public GameObject nodeVis;
	public int uID;
	public float fScore;
	public float gScore;
	public NavNode cameFrom;
	public bool canPropagate;
    private bool showDebugNodes;

	public NavNode (Vector3 initPosition, Vector3 initExtents, List<Vector3> vertexList, int initUID) {
		canPropagate = true;
        showDebugNodes = true;
		nodePosition = initPosition;
        nodeExtents = initExtents;
        nodeVertices = vertexList;
		uID = initUID;
		gScore = 1000f;
		fScore = 1000f;
        if (showDebugNodes) { CreateDebugNode(); }
	}

    public void DestroyNode()
    {
		Object.Destroy(nodeVis);
	}

    private void UpdateDebugNode(string function, NavNode node)
    {
        if (showDebugNodes)
        {
            nodeVis.GetComponent<DebuggingNodes>().SendMessage(function, node);
        }
    }

	public void SetNavAttribs (float newGScore, float newFScore, NavNode parentNode = null) {
		gScore = newGScore;
		fScore = newFScore;
		cameFrom = parentNode;
		UpdateDebugNode("DebugScores", this);
	}
	
	public void ResetScores () {
		gScore = 1000f;
		fScore = 1000f;
		cameFrom = null;
		UpdateDebugNode("DebugScores", this);
	}
	
	public void AddNodeLink(NavNode nodeToLink) {
		if (CheckForDuplicateNode(nodeToLink)){
			linkedNodes.Add(nodeToLink);
			UpdateDebugNodeLinks();
			nodeToLink.AddNodeLink(this);
		}
	}
	
	private bool CheckForDuplicateNode (NavNode nodeToLink) {
		NavNode dupeNode = linkedNodes.Find(node => node.uID == nodeToLink.uID);
		if (dupeNode == null) { return true; }
		else { return false; }
	}
	
	public void RemoveNodeLink(NavNode nodeToRemove) {
		linkedNodes.Remove(nodeToRemove);
		UpdateDebugNodeLinks();
	}
	
	private void UpdateDebugNodeLinks () {
        UpdateDebugNode("UpdateLinks", this);
	}
	
	private void CreateDebugNode () 
    {
        //Debug.Log("[" + uID + "][NavN] is creating a debug node at " + nodePosition + ". Vectors used are " + nodeVertices[0] + nodeVertices[1] + nodeVertices[2] + nodeVertices[3] + ".");
        Mesh mesh = new Mesh();
        Vector3[] debugVerts = new Vector3[4];
        for (int i = 0 ; i < nodeVertices.Count ; i++)
        {
            debugVerts[i] = nodeVertices[i] + Vector3.up;
        }
        mesh.name = "debugNodeMesh";
        mesh.vertices = debugVerts;
        mesh.triangles = new int[] { 0, 2, 3, 0, 3, 1};
        mesh.uv = GenerateUVCoordinates(nodeVertices);
        mesh.RecalculateNormals();
        nodeVis = new GameObject();
        nodeVis.AddComponent<DebuggingNodes>();
        nodeVis.AddComponent<MeshRenderer>();
        nodeVis.AddComponent<MeshFilter>();
        nodeVis.GetComponent<MeshFilter>().mesh = mesh;
        nodeVis.name = "Node [" + uID + "]";
        nodeVis.layer = 11;
        UpdateDebugNode("InitNavNode", this);
	}

    private Vector2[] GenerateUVCoordinates(List<Vector3> vertexList)
    {
        Vector2[] uvCoordinates = new Vector2[vertexList.Count];
        for (int i = 0; i < uvCoordinates.Length; i++)
        {
            uvCoordinates[i] = new Vector2(vertexList[i].x, vertexList[i].z);
        }
        return uvCoordinates;
    }

    public void TogglePropagation(bool state)
    {
		canPropagate = state;
	}
}