using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebuggingNodes : MonoBehaviour {
	
	public int[] linkedNodesUID;
	private List<NavNode> linkedNodes = new List<NavNode>();
	public int uID;
	public int generation;
	public int castDistance;
	public bool debugDraw;
	
	
	// Use this for initialization
	void Start () {
		debugDraw = true;
		this.renderer.material.color = Color.green;
	}
	
	public void InitLinkedList (List<NavNode> initNodeList) {
		System.Array.Resize(ref linkedNodesUID, 0);
		List<int> uIDList = new List<int>();
		foreach (NavNode node in initNodeList) {
			uIDList.Add(node.uID);
		}
		linkedNodesUID = uIDList.ToArray();
		linkedNodes = initNodeList;
	}
	
	public void UpdateLinks (List<NavNode> linkedNodeList) {
		linkedNodes.Clear();
		linkedNodes = linkedNodeList;
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
