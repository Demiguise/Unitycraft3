using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebuggingNodes : MonoBehaviour {
	
	public int uID;
	public int[] linkedNodesUID;
    public Vector3 nodePosition;
    public Vector3 nodeExtents;
    public Vector3[] nodeVertices;
	private List<NavNode> linkedNodes = new List<NavNode>();
	public int generation;
	public int castDistance;
	public bool debugDraw;
	public float fScore;
	public float gScore;
	private NavNode parentNode;
	
	
	// Use this for initialization
	void Start () {
		debugDraw = false;
        UpdateColour(Color.red);
		this.renderer.enabled = true;
	}
	
	public void DebugScores (NavNode node) {
		fScore = node.fScore;
		gScore = node.gScore;
		parentNode = node.cameFrom;
	}
	
	void Update () {
		if ((parentNode != null) && (debugDraw == true)) { 
			Vector3 modParentPosition = parentNode.nodePosition;
			modParentPosition.y = 2;
			Vector3 modNodePosition = transform.position;
			modNodePosition.y = 2;
			Vector3 directionToLink = modParentPosition - modNodePosition;
			Debug.DrawRay(modNodePosition, directionToLink, Color.red, 0);
		}
	}
	
	public void InitNavNode (NavNode node) {
		this.uID = node.uID;
        this.nodePosition = node.nodePosition;
        this.nodeVertices = node.nodeVertices.ToArray();
        this.nodeExtents = node.nodeExtents;
	}
	
	public void UpdateLinks (NavNode node) {
		List<NavNode> localNewList = new List<NavNode>(node.linkedNodes);
		linkedNodes.Clear();
		linkedNodes = localNewList.OrderBy(n => n.uID).ToList();
		ConvertListToArray();
	}
	
	private void ConvertListToArray () {
		int nodeCount = linkedNodes.Count;
		linkedNodesUID = new int[nodeCount];
		for (int i = 0 ; i < nodeCount ; i++ ){
			linkedNodesUID[i] = linkedNodes[i].uID;
		}
	}
	
	public void UpdateColour (Color newColour) {
		renderer.material.color = newColour;
	}
	
	private void DrawDebugRays () {
		foreach (NavNode node in linkedNodes) {
			Vector3 vectorToNode =  new Vector3(node.nodePosition.x, node.nodePosition.y + 1, node.nodePosition.z) - transform.position;
			Debug.DrawRay(transform.position, vectorToNode, Color.red, 1);
		}
	}
}
