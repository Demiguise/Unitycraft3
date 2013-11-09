using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavNode {
	
	public Vector3 nodePosition;
	public Vector3 nodeExtents;
	public List<NavNode> linkedNodes = new List<NavNode>();
	public GameObject nodeVis;
	public int uID;
	public float fScore;
	public float gScore;
	public NavNode cameFrom;
	public bool canPropagate;

	public NavNode (Vector3 initPosition, Vector3 initExtents, int initUID) {
		canPropagate = true;
		nodePosition = initPosition;
		nodeExtents = initExtents;
		uID = initUID;
		CreateDebugNode();
		gScore = 1000f;
		fScore = 1000f;
	}
	
	public void DestroyNode () {
		Object.Destroy(nodeVis);
	}
	public void SetNavAttribs (float newGScore, float newFScore, NavNode parentNode = null) {
		gScore = newGScore;
		fScore = newFScore;
		cameFrom = parentNode;
		nodeVis.GetComponent<DebuggingNodes>().SendMessage("DebugScores", this);
	}
	
	public void ResetScores () {
		gScore = 1000f;
		fScore = 1000f;
		cameFrom = null;
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
		nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateLinks", this.linkedNodes);
	}
	
	private void CreateDebugNode () {
		Vector3 debugNodePos = new Vector3(nodePosition.x, nodePosition.y + 1, nodePosition.z);
		nodeVis = (GameObject)GameObject.Instantiate(Resources.Load("nodeVisPlane"), debugNodePos, new Quaternion(0,0,0,0));
		nodeVis.GetComponent<DebuggingNodes>().SendMessage("InitNavNode", this);
	}
	
	public void TogglePropagation (bool state) {
		canPropagate = state;
		if (state) {
			nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.green);
		}
		else {
			nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.red);
		}
	}
	
	public void ToggleSelected (bool state) {
		if (state) {
			nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.red);
		}
		else {
			nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.green);
		}
	}

}