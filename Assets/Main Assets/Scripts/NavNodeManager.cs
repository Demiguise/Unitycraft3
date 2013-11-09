using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavNodeManager {
	private List<NavNode> availableNodes = new List<NavNode>();
	private float defaultExtents;
	
	public NavNodeManager () {
		defaultExtents = 3f;
	}
	
	public void CreateSpawnNode () {
		CreateNavNode(new Vector3(0,0,0), defaultExtents); //Might set to an initial spawn location, or something.
		BeginFloodFill();
		FindNodeLinks();
	}
	
	public void RegenerateNavMesh () {
		Debug.Log("Deleting all available nodes");
		foreach(NavNode node in availableNodes) {
			node.DestroyNode();
		}
		availableNodes.Clear ();
		Debug.Log("<" + availableNodes.Count() + "> available nodes");
		CreateSpawnNode();
	}
	
	private void CreateNavNode (Vector3 nodePosition, float pExt, float nodeScale = 1f) {
		int childID = GenerateUID();
		availableNodes.Add(new NavNode(nodePosition, new Vector3(pExt * nodeScale, 0, pExt * nodeScale), childID));
	}
	
	private NavNode FindNavNodeFromPos (Vector3 requestedPosition) {
		float xPos = requestedPosition.x;
		float zPos = requestedPosition.z; 
		foreach (NavNode node in availableNodes.ToList()) {
			Vector3 nodePos = node.nodePosition;
			Vector3 nodeExt = node.nodeExtents/2;
			if (((nodePos.x + nodeExt.x) >= xPos) && ((nodePos.x - nodeExt.x) <= xPos)) {
				if (((nodePos.z + nodeExt.z) >= zPos) && ((nodePos.z - nodeExt.z) <= zPos)) {
					//Debug.Log("Found position " + requestedPosition + " in Node <" + node.uID + ">");
					return node;
				}
			}
		}
		return null;
	}
	
	private Vector3 GetDirectionFromFlag (int directionFlag) {
		Vector3 direction;
		switch (directionFlag) {
			case 1:
				direction = new Vector3(1,0,0);
				break;
			case 2:
				direction = new Vector3(0,0,-1);	
				break;
			case 3:
				direction = new Vector3(-1,0,0);
				break;
			case 4:
				direction = new Vector3(0,0,1);	
				break;
			default:
				Debug.Log("Direction flag wasn't set, defaulting to (0,0,0)");
				direction = Vector3.zero;
				break;
		}
		return direction;
	}
	
	private Vector3 RotateVector3RY (Vector3 rV, float degAngle) {
		float angle = degAngle * Mathf.Deg2Rad;
		Vector3 resultVector = new Vector3(0,0,0);
		List<Vector3> RYMatrix = new List<Vector3>(){new Vector3(Mathf.Cos(angle),0,-Mathf.Sin(angle)), new Vector3(0,1,0), new Vector3(Mathf.Sin(angle),0,Mathf.Cos(angle))};
		for(int i = 0 ; i < RYMatrix.Count() ; i++) {
			resultVector[i] = Vector3.Dot(rV, RYMatrix[i]);
		}
		return resultVector;
	}
	
	private Vector3 CalculateNewNodePos (Vector3 originalPos, Vector3 parentExtents, int directionFlag = 0, float curScale = 1f) {
		Vector3 newNodeLocation;
		Vector3 sDir = new Vector3(0,0,0);
		Vector3 direction = GetDirectionFromFlag(directionFlag);
		float curExt = (parentExtents.x*curScale);
		Vector3 scaleDirection = RotateVector3RY(direction, 90f);
		if ((curScale < 1f) && (parentExtents.x != curExt)) {
			sDir = new Vector3(scaleDirection.x * (curExt /2), scaleDirection.y * (curExt /2),scaleDirection.z * (curExt /2));
		}
		float distToPos = (parentExtents.x / 2) + (curExt / 2);
		newNodeLocation = new Vector3(originalPos.x + sDir.x +(direction.x * distToPos), originalPos.y + sDir.y + (direction.y * distToPos), originalPos.z + sDir.z + (direction.z * distToPos));
		return newNodeLocation;
	}
	
	public void BeginFloodFill () {
		while (true) {
			List<NavNode> activeNodes = FindActiveNodes();
			if (activeNodes.Count == 0) { break; }
			foreach (NavNode activeNode in activeNodes) {
				for ( int i = 1 ; i <= 4 ; i++ ) {
					Vector3 newNodePos = CalculateNewNodePos(activeNode.nodePosition, activeNode.nodeExtents, i);
					NavNode DuplicateNode = FindNavNodeFromPos(newNodePos);
					if (DuplicateNode == null) {
						if (CheckPosIsOnMap(newNodePos)) {
							float nodeScale = NodeScaling(newNodePos, activeNode.nodePosition, activeNode.nodeExtents, i);
							newNodePos = CalculateNewNodePos(activeNode.nodePosition, activeNode.nodeExtents, i, nodeScale);
							if (nodeScale != 0) {
								CreateNavNode(newNodePos, activeNode.nodeExtents.x, nodeScale);
							}
						}
					}
				}
				activeNode.TogglePropagation(false);
			}
		}
		Debug.Log("Navigation mesh completed. <" + availableNodes.Count.ToString() + "> nodes created.");
	}
	
	public void FindNodeLinks () {
		Vector3 initialDirection = new Vector3(1,0,1);
		float distance = 2.5f;
		foreach (NavNode node in availableNodes.ToList()) {
			for (int i = 0 ; i < 12 ; i++) { //THIS MAY NEED NORMALISATIONS
				Vector3 locCheck = node.nodePosition + (RotateVector3RY(initialDirection, (i * 30f)) * distance);
				NavNode linkedNode = FindNavNodeFromPos(locCheck);
				if (linkedNode != null) {
					node.AddNodeLink(linkedNode);
				}
			}
			//Debug.Log("Node <" + node.uID + "> has found (" + node.linkedNodes.Count + ") node(s) to link to");
		}
		Debug.Log("Links for all <" + availableNodes.Count + "> have been found");
	}
	
	public List<NavNode> FindTraversalMap (Vector3 start, Vector3 goal) {
		NavNode startNode = FindNavNodeFromPos(start);
		NavNode goalNode = FindNavNodeFromPos(goal);
		startNode.ToggleSelected(true);
		goalNode.ToggleSelected(true);
		List <NavNode> traversalMap = AStarPathfind(startNode, goalNode);
		if (traversalMap.Count == 0) {
			Debug.Log("Transverse complete. No path found.");
		}
		else {
			Debug.Log("Transverse complete. Path with ("+ traversalMap.Count +") has been found.");
		}
		return traversalMap;
	}
	
	private List<NavNode> AStarPathfind (NavNode start, NavNode goal) {
		
		//F score is the node's distance between it and the goal node. This is stored in the actual node.
		//G Score is the distance between the two nodes in question. This is also stored in the node.
		
		foreach (NavNode node in availableNodes) {
			node.ResetScores();
		}
		List<NavNode> closedSet = new List<NavNode>();
		List<NavNode> openSet = new List<NavNode>(availableNodes.ToList());
		List<NavNode> cameFrom = new List<NavNode>();
		
		NavNode currentNode;
		
		start.SetNavAttribs(0, (0 + FindMagnitude(start.nodePosition, goal.nodePosition)));
		
		Debug.Log("[Start] Node is <" + start.uID + "> with an fScore of (" + start.fScore + ").");
		
		while (openSet.Count != 0) {
			currentNode = LowestScoreNode(openSet);
			Debug.Log("[Current] Node is <" + currentNode.uID + "> with an fScore of (" + currentNode.fScore + ").");
			if (currentNode == goal) {
				cameFrom = ReconstructPath(cameFrom, currentNode);
				return cameFrom;
			}
			
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			
			if (currentNode.uID == 1){
				Debug.Log("Node 1 DETECTED");
			}
			
			Debug.Log("[Sets] Openset contains ("+ openSet.Count +") nodes. Closed set contains ("+ closedSet.Count + ") nodes.");
			foreach (NavNode neighbour in currentNode.linkedNodes) {
				int navId = neighbour.uID;
				if (navId == 1) { 
					Debug.Log("Lol");
				}
				float trialGScore = currentNode.gScore + FindMagnitude(currentNode.nodePosition, neighbour.nodePosition);
				float trialFScore = trialGScore + FindMagnitude(neighbour.nodePosition, goal.nodePosition);
				
				if ((CheckNodeInList(closedSet,neighbour) && (trialFScore >= neighbour.fScore))){
					Debug.Log("[Neighbour] Node <" + neighbour.uID + "> has been discarded.");
					continue;
				}
				
				if ((!CheckNodeInList(openSet, neighbour)) || (trialFScore < neighbour.fScore)) {
					Debug.Log("[Neighbour] Node <" + neighbour.uID + "> has been set to come from node <"+ currentNode.uID+">.");
					neighbour.SetNavAttribs(trialGScore, trialFScore, currentNode);
					if (!CheckNodeInList(openSet, neighbour)){
						openSet.Add(neighbour);
					}
				}
			}
		}
		return null;
	}
	
	private List<NavNode> ReconstructPath(List<NavNode> nodeMap, NavNode currentNode){
		List<NavNode> returnList = new List<NavNode>();
		if (currentNode.cameFrom != null) {
			returnList.Add(currentNode);
			ReconstructPath(returnList, currentNode.cameFrom);
			return (returnList);
		}
		else { 
			returnList.Add(currentNode);
			return returnList;
		}
	}	
	
	private bool CheckNodeInList (List<NavNode> nodeList, NavNode nodeToFind) {
		NavNode foundNode = nodeList.Find(node => node.uID == nodeToFind.uID);
		if (foundNode != null) {return true;}
		else {return false;}
	}
	
	private NavNode LowestScoreNode (List<NavNode> nodeList) {
		NavNode lowestNode = null;
		float lowestScore = 1100f;
		foreach (NavNode node in nodeList.ToList()) {
			if (node.fScore < lowestScore){
				lowestScore = node.fScore;
				lowestNode = node;
			}
		}
		return lowestNode;
	}
	
	private float FindMagnitude (Vector3 lhs, Vector3 rhs) {
		float magnitude = 0f;
		Vector3 toDist = lhs - rhs;
		magnitude = toDist.magnitude;
		return magnitude;
	}
	
	private float NodeScaling (Vector3 rPos, Vector3 pPos, Vector3 pExt, int directionFlag) {
		List<Vector3> navPosList = new List<Vector3>();
		//if (FindNavNodeFromPos(rPos) != null) { return 0f; }
		//if (CheckPosIsOnMap(rPos)) { return 0f; }
		float curScale = 1f;
		float minExt = (float)defaultExtents / 4f;
		while (!CheckNodeVerts(rPos, curScale)) {
			curScale /= 2;
			if ((curScale * pExt.x) <= minExt){ 
				curScale = 0; 
				break; 
			}
			rPos = CalculateNewNodePos(pPos, pExt, directionFlag, curScale);
		}
		return curScale;
	}
	
	private bool CheckNodeVerts (Vector3 rPos, float testScale) {
		float curExtents = defaultExtents * testScale;
		List<Vector3> vertexList = GenerateVertexList(rPos, testScale);
		foreach (Vector3 vertex in vertexList) {
			if (!CheckPosIsOnMap(rPos)) { return false; }
			if (FindNavNodeFromPos(rPos) != null) { return false; }
			for (int i = 0 ; i < vertexList.Count ; i++) {
				if (!CheckLOS(vertex, vertexList[i],testScale)) {
					return false;
				}
			}
		}
		return true;
	}
	
	private List<Vector3> GenerateVertexList (Vector3 rPos, float curScale = 1f) {
		List<Vector3> VertexList = new List<Vector3>();
		float radius = (defaultExtents*curScale) / 2;
		VertexList.Add(new Vector3(rPos.x + radius, rPos.y, rPos.z + radius));
		VertexList.Add(new Vector3(rPos.x - radius, rPos.y, rPos.z + radius));
		VertexList.Add(new Vector3(rPos.x + radius, rPos.y, rPos.z - radius));
		VertexList.Add(new Vector3(rPos.x - radius, rPos.y, rPos.z - radius));
		return VertexList;
	}
	
	private bool CheckLOS (Vector3 firstPos, Vector3 secondPos, float curScale = 1f) {
		if (firstPos == secondPos) { return true; }
		int[] worldLayer = {9};
		Vector3 rayHDirection = secondPos - firstPos;
		Ray rH = new Ray(firstPos, rayHDirection);
		RaycastHit objectHit;
		Physics.Raycast(rH, out objectHit, (defaultExtents * curScale), CreateLayerMask(worldLayer));
		if (objectHit.collider == null) {
			return true;
		}
		else { 
			//Debug.Log ("No LOS between " + firstPos + " and " + secondPos + ". They hit -> " + objectHit.collider.name);
			return false; 
		}
	}
	
	private bool CheckPosIsOnMap (Vector3 rPos) {
		Vector3 rayVPosition = rPos;
		int[] worldLayer = {9};
		rayVPosition.y = 10;
		Ray rV = new Ray (rayVPosition, new Vector3(0,-1,0));
		RaycastHit objectHit;
		Physics.Raycast(rV, out objectHit, 10, CreateLayerMask(worldLayer));
		if (objectHit.collider == null) {
			//Debug.Log("Position " + rPos + " is off map!");
			return false;
		}
		else {return true;}
	}
	
	private List<NavNode> FindActiveNodes() {
		List<NavNode> activeNodes = new List<NavNode>();
		foreach (NavNode node in availableNodes) {
			if (node.canPropagate == true) {
				activeNodes.Add(node);
			}
		}
		//Debug.Log("Found <" + activeNodes.Count + "> node(s) available for propagation");
		return activeNodes;
	}
	
	public int GenerateUID () {
		int uID = 0;
		if (availableNodes.Count > 0) {
			foreach (NavNode node in availableNodes.ToList()) {
				if (node.uID > uID) uID = node.uID;
			}
		}
		return (uID + 1);
	}
		
	int CreateLayerMask (int[] layers) {
		int layerMask = 1;
		foreach (int Layer in layers){
			int i = 1 << Layer;
			layerMask = layerMask | i;
		}
		return layerMask;
	}
	
}