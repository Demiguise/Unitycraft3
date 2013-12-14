using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavNodeManager {
	private List<NavNode> availableNodes = new List<NavNode>();
	private float defaultExtents;
    private float minSubDivisionExtents;
    public int debugLevel = 0; //Should go from 0 (Nothing) to 5 (Everything)
    private int[] worldLayer = { 9 };
	
	private List<double> nodeLinksTimes = new List<double>();
	private List<double> checkVertLOSTimes = new List<double>();
	private List<double> nodeScalingTimes = new List<double>();
	
	
	public NavNodeManager () {
		defaultExtents = 3f;
        minSubDivisionExtents = defaultExtents / 3;
        //Debug.Log("Minimum Extents set to : " + minSubDivisionExtents + ".");
	}

    private void DebugLog(string service, string message, int debugLevelFlag)
    {
        if (debugLevel >= debugLevelFlag)
        {
            Debug.Log("[NavM][" + service + "] " + message);
        }
    }
	
    #region NavMeshCreation
    private void printBenchmarkTimes() {
        Debug.Log("[NavM] NodeVerts Total: " + checkVertLOSTimes.Sum() + "ms | " + checkVertLOSTimes.Min() + "ms => " + checkVertLOSTimes.Max() + "ms");
        //Debug.Log("[NavM] NodeLinks Total: " + nodeLinksTimes.Sum() + "ms | " + nodeLinksTimes.Min() + "ms => " + nodeLinksTimes.Max() + "ms");
        //Debug.Log("[NavM] NodeScale Total: " + nodeScalingTimes.Sum() + "ms | " + nodeScalingTimes.Min() + "ms => " + nodeScalingTimes.Max() + "ms");
	}

    public void BeginFloodFill()
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        //
        List<NavNode> activeNodes = FindActiveNodes(availableNodes);

        while ((activeNodes.Count != 0) && (availableNodes.Count < 250))
        {
            //Debug.Log("Current activeNode count is : " + activeNodes.Count+".");
            for (int i = 0; i < activeNodes.Count; i++)
            {
                NavNode curNode = activeNodes[i];
                for (int j = 1; j <= 4; j++)
                {
                    DebugLog("Main", "Navnode <" + curNode.uID + "> is attempting to create a node. Direction (" + j + ").", 2);
                    float curScale = 1f;
                    Vector3 nodeExtents = curNode.nodeExtents;
                    Vector3 nodePosition = CalculateNewNodePos(curNode.nodePosition, nodeExtents, j);
                    List<Vector3> nodeVertexList = GenerateVertexList(nodePosition, nodeExtents.x);
                    if (FindNavNodeFromPos(nodePosition) == null)
                    {
                        while ((!CheckVertexLOS(nodeVertexList)) || (!CheckVertsAreOnMap(nodeVertexList)))
                        {
                            if ((nodeExtents.x * curScale) <= minSubDivisionExtents)
                            {
                                DebugLog("Main", "Node's extents are below the mininmum :" + (nodeExtents.x * curScale), 1);
                                break;
                            }
                            DebugLog("Main", "Incrementing subdivision level", 1);
                            curScale /= 2;
                            nodePosition = CalculateNewNodePos(curNode.nodePosition, nodeExtents, j, curScale);
                            nodeVertexList = GenerateVertexList(nodePosition, nodeExtents.x, curScale);
                        }
                        if (((nodeExtents.x * curScale) >= minSubDivisionExtents) && (CheckVertexTear(nodeVertexList)))
                        {
                            DebugLog("Main", "Success. Current extent of node is above the minimum ("+ minSubDivisionExtents +") and the there are no tears between verticies.", 1);
                            CreateNavNode(nodePosition, nodeExtents.x, nodeVertexList, curScale);
                            curScale = 1f;
                        }
                        else
                        {
                            DebugLog("Main", "Failure. Current extent of node is " + (nodeExtents.x * curScale) + " and the minimum is (" + minSubDivisionExtents +"). The verts could also be torn. Discarding node.", 1);
                            curScale = 1f;
                        }
                    }
                    else { DebugLog("Main", "Discarding duplicate node", 1); }
                }
                curNode.TogglePropagation(false);
            }
            activeNodes = FindActiveNodes(availableNodes);
        }
        //
        stopWatch.Stop();
        Debug.Log("[NavM][Main] Navigation mesh completed. <" + availableNodes.Count.ToString() + "> nodes created in (" + stopWatch.Elapsed.TotalMilliseconds + ") ms.");
        printBenchmarkTimes();
    }

    private bool CheckVertexTear (List<Vector3> vertexList)
    {
        Vector3 startVertex = vertexList[0];
        Vector3 endVertex = vertexList[vertexList.Count - 1];
        Vector3[] vertArray = new Vector3[2] { startVertex, endVertex };
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                Vector3 dirToVertex = vertexList[j] - vertArray[i];
                DebugLog("VTear", "Vector from S:" + vertArray[i] + " to G:" + vertexList[j] + " is " + dirToVertex + ".", 4);
                if (Mathf.Abs(dirToVertex.y) > 2)
                {
                    DebugLog("VTear", "Vertex tearing found.", 3);
                    return false;
                }
            }
        }
        DebugLog("VTear", "No vertex tearing found.", 3);
        return true;
    }

    private bool CheckPosIsOnMap(Vector3 rPos)
    {
        Vector3 rayVPosition = rPos + (Vector3.up * 10);
        if (CastRay(rayVPosition, Vector3.down, 20, worldLayer).collider == null)
        {
            return false;
        }
        else { return true; }
    }

    private bool CheckVertsAreOnMap(List<Vector3> verticeList)
    {
        for (int i = 0 ; i < verticeList.Count ; i++ )
        {
            if (!CheckPosIsOnMap(verticeList[i]))
            {
                DebugLog("VertC", "Position " + verticeList[i] + " is off the map. Failing Mapcheck", 3);
                return false;
            }
        }
        DebugLog("VertC", "All positions are on map. Succeeding Mapcheck", 3);
        return true;
    }

    private bool CheckVertexLOS(List<Vector3> vertexList)
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        RaycastHit objectHit = new RaycastHit();
        Vector3 startVertex = vertexList[0];
        Vector3 endVertex = vertexList[vertexList.Count - 1];
        Vector3[] vertArray = new Vector3[2] { startVertex, endVertex };
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                if (vertArray[i] == vertexList[j]) { continue; }
                objectHit = CheckLOS(vertexList[j], vertArray[i], worldLayer);
                if ((objectHit.collider != null))
                {
                    if (objectHit.collider.tag == "Impassable")
                    {
                        stopWatch.Stop();
                        checkVertLOSTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
                        DebugLog("VertLOS", "Positions " + vertArray[i] + " and " + vertexList[j] + " have failed LOS checks. They hit object " + objectHit.collider.tag + ".", 3);
                        return false;
                    }
                    DebugLog("VertLOS", "Positions " + vertArray[i] + " and " + vertexList[j] + " hit object " + objectHit.collider.name + ".", 3);
                }
            }
        }
        stopWatch.Stop();
        checkVertLOSTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
        DebugLog("VertLOS", " All LOS checks are fine. Succeeding LOSCheck", 3);
        return true;
    }

    private RaycastHit CheckLOS(Vector3 fPos, Vector3 sPos, int[] layer)
    {
        RaycastHit objectHit = new RaycastHit();
        Vector3 dir = FindResultant(fPos, sPos);
        float dist = FindMagnitude(fPos, sPos);
        DebugLog("VertLOS", "Casting a ray from " + fPos + " to " + sPos + " in the direction of " + dir + " for " + dist + " units.", 4);
        objectHit = CastRay(fPos, dir, dist, layer);
        return objectHit;
    }

    public void RegenerateNavMesh () {
        Debug.Log("[NavM] Deleting all available nodes");
		foreach(NavNode node in availableNodes) {
			node.DestroyNode();
		}
		availableNodes.Clear ();
		CreateSpawnNode();
	}
	
	public void CreateSpawnNode () {
        GameObject spawnCube = GameObject.FindGameObjectWithTag("SPAWNCUBE");
        Vector3 spawnLoc = spawnCube.transform.position;
        spawnLoc.y = 0;
        CreateNavNode(spawnLoc, defaultExtents, GenerateVertexList(spawnLoc, defaultExtents)); //Might set to an initial spawn location, or something.
		BeginFloodFill();
		FindNodeLinks();
	}
	
	private void CreateNavNode (Vector3 nodePosition, float pExt, List<Vector3> vertexList, float nodeScale = 1f) {
		int childID = GenerateUID();
        Vector3 newExtents = new Vector3(pExt * nodeScale, 0, pExt * nodeScale);
        DebugLog("CreateNav", "Creating Navnode <" + childID + "> at " + nodePosition + ". Extents are set to " + newExtents + ".", 1);
        availableNodes.Add(new NavNode(nodePosition, newExtents, vertexList, childID));
	}
	
	private NavNode FindNavNodeFromPos (Vector3 requestedPosition) {
		float xPos = requestedPosition.x;
		float zPos = requestedPosition.z; 
		foreach (NavNode node in availableNodes.ToList()) {
			Vector3 nodePos = node.nodePosition;
			Vector3 nodeExt = node.nodeExtents/2;
			if (((nodePos.x + nodeExt.x) >= xPos) && ((nodePos.x - nodeExt.x) <= xPos)) {
				if (((nodePos.z + nodeExt.z) >= zPos) && ((nodePos.z - nodeExt.z) <= zPos)) {
                    DebugLog("PosC", "Position " + requestedPosition + " can be found in node <" + node.uID + ">.", 4);
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
		newNodeLocation = new Vector3(originalPos.x + sDir.x +(direction.x * distToPos), 0, originalPos.z + sDir.z + (direction.z * distToPos));
        newNodeLocation.y = GetVertexHeight(newNodeLocation);
		return newNodeLocation;
	}
	
	public void FindNodeLinks () {
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		stopWatch.Start ();
		Vector3 initialDirection = new Vector3(1,0,1);
		float distance = 1.5f;
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
		stopWatch.Stop ();
		nodeLinksTimes.Add (stopWatch.Elapsed.TotalMilliseconds);
        Debug.Log("[NavM] Links for all <" + availableNodes.Count + "> have been found");
	}
	
	private List<Vector3> GenerateVertexList (Vector3 rPos, float ext, float curScale = 1f) {
		List<Vector3> VertexList = new List<Vector3>();
        float radius = (ext * curScale) / 2;
		VertexList.Add(new Vector3(rPos.x + radius, 0, rPos.z + radius));
		VertexList.Add(new Vector3(rPos.x - radius, 0, rPos.z + radius));
		VertexList.Add(new Vector3(rPos.x + radius, 0, rPos.z - radius));
		VertexList.Add(new Vector3(rPos.x - radius, 0, rPos.z - radius));
        for (int i = 0 ; i < VertexList.Count() ; i++)
        {
            VertexList[i] += (Vector3.up * GetVertexHeight(VertexList[i]));
        }
        DebugLog("VGen", "[R" + rPos + "][" + radius + "] Vector list -> " + VertexList[0] + VertexList[1] + VertexList[2] + VertexList[3] + ".", 4);
		return VertexList;
	}

    private float GetVertexHeight(Vector3 position)
    {
        float vertexHeight;
        int initialPos = 30;
        Vector3 vPos = position + (Vector3.up * initialPos);
        RaycastHit objectHit = CastRay(vPos, Vector3.down, (initialPos * 1.5f), worldLayer);
        vertexHeight = objectHit.point.y;
        return vertexHeight;
    }

    private RaycastHit CastRay(Vector3 pos, Vector3 dir, float dist, int[] layer)
    {
        int layerMask = CreateLayerMask(layer);
        Ray rV = new Ray(pos, dir);
        RaycastHit objectHit;
        Physics.Raycast(rV, out objectHit, dist, layerMask);
        return objectHit;
    }

	private List<NavNode> FindActiveNodes(List<NavNode> inputList) {
        List<NavNode> activeNodes = new List<NavNode>();
        foreach (NavNode node in inputList)
        {
			if (node.canPropagate == true) {
                activeNodes.Add(node);
			}
		}
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
		foreach (int layer in layers){
			int i = 1 << layer;
			layerMask = layerMask | i;
		}
		return layerMask;
	}
    #endregion

    #region NavMeshSimplification
    #endregion

    #region AIPathfinding

    public List<Vector3> FindTraversalMap(Vector3 start, Vector3 goal, GameObject unit)
    {
        Debug.Log("[NavM] Start position: " + start + " | Goal Position: " + goal);
		ResetBenchmarkTimes();
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		stopWatch.Start();
		NavNode startNode = FindNavNodeFromPos(start);
		NavNode goalNode = FindNavNodeFromPos(goal);
		if ((startNode == null) || (goalNode == null)){
            Debug.Log("[NavM] Start or goal position is not covered by a node.");
			return null;
		}
		List <NavNode> traversalMap = AStarPathfind(startNode, goalNode);
		traversalMap.Reverse();
		HighlightTraversalMap(traversalMap, Color.white);
        List<Vector3> vectorMap = ConvertNodeToVector(traversalMap);
        HighlightVectorMap(vectorMap, Color.red);
        vectorMap.RemoveAt(0); //Deleting starting node, don't need it.
		vectorMap[vectorMap.Count - 1] = goal;
		stopWatch.Stop();
		if (traversalMap.Count == 0) {
            Debug.Log("[NavM] Transverse complete in (" + stopWatch.Elapsed.TotalMilliseconds + ") milliseconds. No path found.");
		}
		else {
            Debug.Log("[NavM] Transverse complete in (" + stopWatch.Elapsed.TotalMilliseconds + ") milliseconds. Path with (" + traversalMap.Count + " | S" + vectorMap.Count + ") has been found.");
		}
		return vectorMap;
	}
	
	private List<Vector3> ConvertNodeToVector (List<NavNode> nodelist){
		List<Vector3> returnList = new List<Vector3>();
		foreach(NavNode node in nodelist){
			returnList.Add(node.nodePosition);
		}
		return returnList;
	}

    private List<Vector3> CollisionRayTests(List<Vector3> vectorList, GameObject unit)
    {
        float localScale = 1.666f;
        float colliderXExtent = unit.GetComponent<BoxCollider>().size.x;
		List<Vector3> localVectorList = vectorList;
		for (int i = 0 ; i < localVectorList.Count - 1 ; i++) {
			Vector3 mainDirection = localVectorList[i+1] - localVectorList[i];
			Vector3 normalDirection = (RotateVector3RY(mainDirection.normalized, 90f));
            float pCollisionChange = FindReqCollisionChange(localVectorList[i], localVectorList[i + 1], (normalDirection * (colliderXExtent * localScale)));
            float nCollisionChange = FindReqCollisionChange(localVectorList[i], localVectorList[i + 1], (normalDirection * -(colliderXExtent * localScale)));
			float resultantChange = pCollisionChange - nCollisionChange;
            Debug.Log("[NavM][CollDect][" + i +"] Needed to move " + localVectorList[i] + " by " + resultantChange + " units");
			localVectorList[i+1] = MoveVector(localVectorList[i+1], normalDirection, (resultantChange * -1));
            Debug.DrawRay(((normalDirection * (colliderXExtent * localScale)) + localVectorList[i] + (Vector3.up * 2)), (localVectorList[i + 1] - localVectorList[i] + (Vector3.up * 2)), Color.yellow, 10);
            Debug.DrawRay(((normalDirection * -(colliderXExtent * localScale)) + localVectorList[i] + (Vector3.up * 2)), (localVectorList[i + 1] - localVectorList[i] + (Vector3.up * 2)), Color.yellow, 10);
			//NEEDS TO ACTUALLY RUN THE TEST AGAIN TO MAKE SURE IT WORKED, IF IT DIDN'T LOLBREAK
		}
		return vectorList;
	}

	private float FindReqCollisionChange (Vector3 curPos, Vector3 goalPos, Vector3 normalDirection, float initChange = 10f) {
		int[] worldLayer = {9};
		float change = 0f;
		float step = 1f;
		float maxChange = initChange;
		Vector3 testPos = goalPos;
		Vector3 mainDirection = testPos - curPos;
		Vector3 normalPos = curPos + normalDirection + (Vector3.up * 2);
		Ray normalRay = new Ray(normalPos, mainDirection);
		while (Physics.Raycast(normalRay, mainDirection.magnitude, CreateLayerMask(worldLayer))){
			testPos = MoveVector(testPos, (normalDirection * -1), step);
			mainDirection = testPos - curPos;
			normalRay = new Ray(normalPos, mainDirection);
			change += step;
			if (change == maxChange) {
				break;
			}
		}
		return change;
	}

	private Vector3 MoveVector (Vector3 origin, Vector3 direction, float distance){
		Vector3 returnVector3 = new Vector3();
		returnVector3.x = origin.x + (direction.x * distance);
		returnVector3.y = origin.y + (direction.y * distance);
		returnVector3.z = origin.z + (direction.z * distance);
		return returnVector3;
	}
	
	private void ResetBenchmarkTimes (){
		nodeLinksTimes.Clear();
		checkVertLOSTimes.Clear();
		nodeScalingTimes.Clear();
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
		
		//Debug.Log("[Start] Node is <" + start.uID + "> with an fScore of (" + start.fScore + ").");
		
		while (openSet.Count != 0) {

			currentNode = LowestScoreNode(openSet);
			//Debug.Log("[Current] Node is <" + currentNode.uID + "> with an fScore of (" + currentNode.fScore + ").");
			if (currentNode == goal) {
				ReconstructPath(ref cameFrom, currentNode);
				return cameFrom;
			}
			
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			
			//Debug.Log("[Sets] Openset contains ("+ openSet.Count +") nodes. Closed set contains ("+ closedSet.Count + ") nodes.");
			foreach (NavNode neighbour in currentNode.linkedNodes) {
				float trialGScore = currentNode.gScore + FindMagnitude(currentNode.nodePosition, neighbour.nodePosition);
				float trialFScore = trialGScore + FindMagnitude(neighbour.nodePosition, goal.nodePosition);
				bool nodeSet = CheckNodeInList(openSet, neighbour); //True if in Openset, false if in Closedset
				
				if ((nodeSet == false) && (trialFScore >= neighbour.fScore)){
					//Debug.Log("[Neighbour] Node <" + neighbour.uID + "> has been discarded.");
					continue;
				}
				
				if ((nodeSet == false) || (trialFScore < neighbour.fScore)) {
					//Debug.Log("[Neighbour] Node <" + neighbour.uID + "> has been set to come from node <"+ currentNode.uID+">.");
					neighbour.SetNavAttribs(trialGScore, trialFScore, currentNode);
					if (nodeSet == false){
						openSet.Add(neighbour);
					}
				}
			}
		}
		return null;
	}
	
	private void ReconstructPath(ref List<NavNode> nodeMap, NavNode currentNode){
		if (currentNode.cameFrom != null) {
			nodeMap.Add(currentNode);
			ReconstructPath(ref nodeMap, currentNode.cameFrom);
		}
		else { 
			nodeMap.Add(currentNode);
		}
	}
	
	private void HighlightTraversalMap (List<NavNode> nodeMap, Color highlightColour){
		for (int i = 0 ; i < nodeMap.Count - 1 ; i++ ){
			Vector3 modNodePos = nodeMap[i].nodePosition + (Vector3.up * 3);
			Vector3 directionToNode = (nodeMap[i+1].nodePosition + (Vector3.up * 3)) - modNodePos;
			Debug.DrawRay(modNodePos, directionToNode, highlightColour, 10);
			Debug.DrawRay(modNodePos, Vector3.up, highlightColour, 10);
		}
	}

	private void HighlightVectorMap (List<Vector3> vectorMap, Color highlightColour){
		for (int i = 0 ; i < vectorMap.Count - 1 ; i++ ){
			Vector3 modNodePos = vectorMap[i] + (Vector3.up * 3);
			Vector3 directionToNode = (vectorMap[i+1] + (Vector3.up * 3)) - modNodePos;
			Debug.DrawRay(modNodePos, directionToNode, highlightColour, 10);
			Debug.DrawRay(modNodePos, Vector3.up, highlightColour, 10);
		}
	}
	
	private bool CheckNodeInList (List<NavNode> nodeList, NavNode nodeToFind) {
		NavNode foundNode = nodeList.Find(node => node.uID == nodeToFind.uID);
		if (foundNode != null) {
			return true;
		}
		else {
			return false;
		}
	}
	
	private NavNode LowestScoreNode (List<NavNode> nodeList) {
		NavNode lowestNode = null;
		float lowestScore = 1100f;
		foreach(NavNode node in nodeList) {
			if(node.fScore < lowestScore){
				lowestNode = node;
				lowestScore = node.fScore;
			}
		}
		return lowestNode;
	}
	
	private float FindMagnitude (Vector3 fPos, Vector3 sPos) 
    {
        Vector3 toDist = FindResultant(fPos, sPos);
		return toDist.magnitude;
	}

    private Vector3 FindResultant(Vector3 fPos, Vector3 sPos)
    {
        Vector3 resultant = sPos - fPos;
        return resultant;
    }
    #endregion
}