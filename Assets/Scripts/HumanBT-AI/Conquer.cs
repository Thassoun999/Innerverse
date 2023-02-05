using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Conquer : Node 
{

    // Only engage in conquering if you're outnumbered (equivalent of retreating)
    public override NodeState Evaluate()
    {
        if(GameManager.Instance.PlayerTurn)
            return NodeState.FAILURE;

        float humanToMycRatio = (float)GetData("Scan Ratio");
        
        // Only engage in conquer if humans are outnumbered or there's no one around
        if(humanToMycRatio >= 1) {
            return NodeState.FAILURE;
        }

        int[] specialBiome1EdgeCoords = new int[] {0, 29};
        int[] specialBiome2EdgeCoords = new int[] {29, 0};

        foreach(KeyValuePair<int, Human> elem in GameManager.Instance.HumanGroup) {
            Human tempHum = elem.Value;
            int[] tempHumanCoords = tempHum.Coordinates;
            GridNode destination = null;

            // First for each Human we need to check which special biome we are closer to
            float distanceToBiome1 = GetDistance(tempHumanCoords, specialBiome1EdgeCoords);
            float distanceToBiome2 = GetDistance(tempHumanCoords, specialBiome2EdgeCoords);

            if(distanceToBiome1 >= distanceToBiome2) {
                // Go to Biome 2 (shorter distance)
                float shortestDistance = 1000000.0f;
                // Find a node that is unoccupied, of the biome type, and the closest to our Human
                foreach(KeyValuePair<(int,int), GridNode> gnode in GameManager.Instance.CoordsToGridNode) {
                    if(gnode.Value.Occupation == 0 && gnode.Value.SpecialClassifier == 2) {
                        if(GetDistance(tempHumanCoords, gnode.Value.Coordinates) < shortestDistance) {
                            destination = gnode.Value;
                            shortestDistance = GetDistance(tempHumanCoords, gnode.Value.Coordinates);
                        }
                    }
                }

                // No grid to go to, just stay put
                if(destination == null) {
                    continue;
                }

                // Find a path (if no valid path just continue and stay put)
                List<GridNode> path = A_Star(ref tempHum, destination);
                if(path == null)
                    continue;

                // Move our soldier
                Move(path, ref tempHum);

            } else {
                // Go to Biome 1 (shorter distance)
                float shortestDistance = 1000000.0f;
                // Find a node that is unoccupied, of the biome type, and the closest to our Human
                foreach(KeyValuePair<(int,int), GridNode> gnode in GameManager.Instance.CoordsToGridNode) {
                    if(gnode.Value.Occupation == 0 && gnode.Value.SpecialClassifier == 1) {
                        if(GetDistance(tempHumanCoords, gnode.Value.Coordinates) < shortestDistance) {
                            destination = gnode.Value;
                            shortestDistance = GetDistance(tempHumanCoords, gnode.Value.Coordinates);
                        }
                    }
                }

                // No grid to go to, just stay put
                if(destination == null) {
                    continue;
                }

                // Find a path (if no valid path just continue and stay put)
                List<GridNode> path = A_Star(ref tempHum, destination);
                if(path == null)
                    continue;

                // Move our soldier
                Move(path, ref tempHum);
            }
        }

        return NodeState.SUCCESS;

    }

    void Move(List<GridNode> path, ref Human agentToMove) {
        // We need to start by ensuring the grid we are standing on no longer references us in any way
        int[] coords = agentToMove.Coordinates;

        GameManager.Instance.CoordsToGridNode[(coords[0], coords[1])].Occupation = 0;
        GameManager.Instance.CoordsToGridNode[(coords[0], coords[1])].Standing = null;

        float walkCooldown = 2.0f;
        float currWalkCooldown = 0.0f;

        // Move from grid to grid
        foreach(GridNode node in path) {
            Vector3 targetPosition = new Vector3(node.gameObject.transform.localPosition.x, agentToMove.gameObject.transform.localPosition.y, node.gameObject.transform.localPosition.z);
            agentToMove.gameObject.transform.localPosition = targetPosition;
            while(currWalkCooldown < walkCooldown)
                currWalkCooldown += Time.deltaTime;
            currWalkCooldown = 0.0f;
        }

        // Once we are on last grid we need to update the grid we are standing on, our coords, everything
        GridNode copyNode = path[path.Count - 1];
        int[] copyNodeCoords = copyNode.Coordinates;

        agentToMove.Coordinates = copyNodeCoords;

        // We're doing this so we get the actual reference in case our copyNode is not an actual reference
        GameManager.Instance.CoordsToGridNode[(copyNodeCoords[0], copyNodeCoords[1])].Occupation = 2;
        GameManager.Instance.CoordsToGridNode[(copyNodeCoords[0], copyNodeCoords[1])].Standing = agentToMove.gameObject;

        return;
    }

    
    public class PathNode {
        public GridNode _Node;
        public int gCost;
        public int hCost;
        public PathNode parent;

        public PathNode(GridNode node) {
            this._Node = node;
            this.gCost = 1000000;
            this.hCost = 1000000;
            this.parent = null;
        }

        public int fCost {
            get {
                return this.gCost + this.hCost;
            }
        }


    }

    // Returns full path, it's up to the constraints of the individual to just go the amount of steps they CAN go to
    public List<GridNode> A_Star(ref Human agentToMove, GridNode destination) {
        int[] coords = agentToMove.Coordinates;

        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        PathNode startNode = new PathNode(GameManager.Instance.CoordsToGridNode[(coords[0], coords[1])]);
        PathNode destNode = new PathNode(destination);
        //Debug.Log(startNode._Node.Coordinates[0] + " " + startNode._Node.Coordinates[1]);
        //Debug.Log(destNode._Node.Coordinates[0] + " " + destNode._Node.Coordinates[1]);
        startNode.gCost = 0;
        startNode.hCost = (int)GetDistance(startNode, destNode);
        destNode.hCost = 0;
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            PathNode curr = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < curr.fCost || (openSet[i].fCost == curr.fCost && openSet[i].hCost < curr.hCost)) {
                    curr = openSet[i];
                }
            }

            openSet.Remove(curr);
            closedSet.Add(curr);

            if(curr._Node.Coordinates[0] == destNode._Node.Coordinates[0] && curr._Node.Coordinates[1] == destNode._Node.Coordinates[1]) {
                return RetracePath(startNode, destNode);
            }

            
            List<PathNode> neighbours = GetNeighbors(curr);

            foreach(PathNode neighbour in neighbours) {
                if(neighbour._Node.Occupation != 0 || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = curr.gCost + (int)GetDistance(curr, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = (int)GetDistance(neighbour, destNode);
                    neighbour.parent = curr;


                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }


            }
        }

        return null;
    }

    public List<PathNode> GetNeighbors(PathNode curr) {
        List<PathNode> neighbours = new List<PathNode>();
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++) {
                if(i ==0 && j == 0)
                    continue;

                int[] coords = curr._Node.Coordinates;
                if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((coords[0] + i, coords[1] + j))))
                    continue;
                else {
                    PathNode newPathNode = new PathNode(GameManager.Instance.CoordsToGridNode[(coords[0] + i, coords[1] + j)]);
                    neighbours.Add(newPathNode);
                }
            }
        }

        return neighbours;
    }

    List<GridNode> RetracePath(PathNode startNode, PathNode endNode) {
        List<GridNode> path = new List<GridNode>();
        PathNode currNode = endNode;
        GridNode currGridNode = currNode._Node;
        GridNode startGridNode = startNode._Node;

        while (currNode != null && currGridNode != startGridNode) {
            path.Add(currGridNode);
            currNode = currNode.parent;
        }

        path.Reverse();
        return path;
    }

    float GetDistance(PathNode nodeA, PathNode nodeB) {
        float dstX = Mathf.Pow((nodeB._Node.Coordinates[0] - nodeA._Node.Coordinates[0]), 2);
        float dstY = Mathf.Pow((nodeB._Node.Coordinates[1] - nodeA._Node.Coordinates[1]), 2);

        return Mathf.Sqrt(dstX + dstY);
    }

    float GetDistance(int[] coordsA, int[] coordsB) {
        float dstX = Mathf.Pow((coordsB[0] - coordsA[0]), 2);
        float dstY = Mathf.Pow((coordsB[1] - coordsA[1]), 2);

        return Mathf.Sqrt(dstX + dstY);
    }

}