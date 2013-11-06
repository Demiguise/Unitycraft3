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
	public bool canPropagate;

	public NavNode (Vector3 initPosition, Vector3 initExtents, int initUID) {
		canPropagate = true;
		nodePosition = initPosition;
		nodeExtents = initExtents;
		uID = initUID;
		CreateDebugNode();
	}
	
	public void DestroyNode () {
		Object.Destroy(nodeVis);
	}
	
//  Hacky function for linked to figuring out if "Findnavnodefromposition" worked.
//	public void debugToggleSelected (bool state) {
//		if (state) { nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.yellow); }
//		else {
//			if (canPropagate) {
//				nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.green);
//			}
//			else {
//				nodeVis.GetComponent<DebuggingNodes>().SendMessage("UpdateColour", Color.red);
//			}
//		}
//	}
	
	public void AddNodeLink(NavNode nodeToLink) {
		linkedNodes.Add(nodeToLink);
		UpdateDebugNodeLinks();
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
		//nodeVis.GetComponent<DebuggingNodes>().SendMessage("InitNavNode", this);
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

}