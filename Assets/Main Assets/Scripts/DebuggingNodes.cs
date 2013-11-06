using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebuggingNodes : MonoBehaviour {
	
	public int uID;
	public int[] linkedNodesUID;
	private List<NavNode> linkedNodes = new List<NavNode>();
	public int generation;
	public int castDistance;
	public bool debugDraw;
	
	
	// Use this for initialization
	void Start () {
		debugDraw = true;
		this.renderer.material.color = Color.green;
	}
	
	public void InitNavNode (NavNode node) {
		this.uID = node.uID;
		this.transform.localScale = node.nodeExtents;
	}
	
	public void UpdateLinks (List<NavNode> linkedNodeList) {
		List<NavNode> localNewList = new List<NavNode>(linkedNodeList);
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
