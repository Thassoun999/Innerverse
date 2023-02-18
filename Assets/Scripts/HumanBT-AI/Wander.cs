using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Wander : Node
{
    public override NodeState Evaluate()
    {
        // Nothing is around us and there isn't anything to conquer, just wander and scan for next turn
        foreach(KeyValuePair<int, Human> elem in GameManager.Instance.HumanGroup) {
            Human tempHum = elem.Value;
            int[] tempHumanCoords = tempHum.Coordinates;
            GridNode start = GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])];

            List<GridNode> possibleDestinations = new List<GridNode>();

            for(int i = -tempHum.TotalRange; i < tempHum.TotalRange + 1; i++) {
                for(int j = -tempHum.TotalRange; j < tempHum.TotalRange + 1; j++) {
                    if(i == 0 && j == 0)
                        continue;

                    if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((tempHumanCoords[0] + i, tempHumanCoords[1] + j))))
                        continue;

                    // compile list of (empty) nodes to go to, we'll pick one at random
                    if (GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0] + i, tempHumanCoords[1] + j)].Occupation == 0) {
                        possibleDestinations.Add(GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0] + i, tempHumanCoords[1] + j)]);
                    }
                }
            }

            // Human is stuck, don't bother moving him
            if(possibleDestinations.Count == 0)
                continue;

            // Pick random path
            int index = Random.Range(0, possibleDestinations.Count);
            GridNode destination = possibleDestinations[index];

            // Find Path
            List<GridNode> path = A_Star(ref tempHum, destination);
            if(path == null)
                continue;

            // Move our solder to just... sit there!!
            Move(path, ref tempHum);
            
        }
        return NodeState.SUCCESS;
    }
    
    void Move(List<GridNode> path, ref Human agentToMove) {
        // We need to start by ensuring the grid we are standing on no longer references us in any way
        agentToMove.SetPath(ref path);

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
                destNode.parent = curr.parent;
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
                if(i == 0 && j == 0)
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

        while (currNode._Node != startNode._Node) {
            path.Add(currNode._Node);
            currNode = currNode.parent;
        }

        path.Reverse();
        return path;
    }

    float GetDistance(PathNode nodeA, PathNode nodeB) {
        if(nodeA == null || nodeB == null)
            return Mathf.Infinity;

        float dstX = Mathf.Pow((nodeB._Node.Coordinates[0] - nodeA._Node.Coordinates[0]), 2);
        float dstY = Mathf.Pow((nodeB._Node.Coordinates[1] - nodeA._Node.Coordinates[1]), 2);

        return Mathf.Sqrt(dstX + dstY);
    }

}
