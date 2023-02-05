using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Conquer : Node 
{
    PathFinding pathfindingClass = new PathFinding();

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
                List<GridNode> path = pathfindingClass.A_Star(ref tempHum, destination);
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
                List<GridNode> path = pathfindingClass.A_Star(ref tempHum, destination);
                if(path == null)
                    continue;

                // Move our soldier
                Move(path, ref tempHum);
            }
        }

        return NodeState.SUCCESS;

    }

    float GetDistance(int[] coordsA, int[] coordsB) {
        float dstX = Mathf.Pow((coordsB[0] - coordsA[0]), 2);
        float dstY = Mathf.Pow((coordsB[1] - coordsA[1]), 2);

        return Mathf.Sqrt(dstX + dstY);
    }

    void Move(List<GridNode> path, ref Human agentToMove) {
        // We need to start by ensuring the grid we are standing on no longer references us in any way
        int[] coords = agentToMove.Coordinates;

        GameManager.Instance.CoordsToGridNode[(coords[0], coords[1])].Occupation = 0;
        GameManager.Instance.CoordsToGridNode[(coords[0], coords[1])].Standing = null;

        // Move from grid to grid
        foreach(GridNode node in path) {
            Vector3 targetPosition = new Vector3(node.gameObject.transform.localPosition.x, 0, node.gameObject.transform.localPosition.z);
            Vector3 velocity = Vector3.zero;
            agentToMove.gameObject.transform.localPosition = Vector3.SmoothDamp(
                agentToMove.gameObject.transform.localPosition,
                targetPosition,
                ref velocity,
                0.3f
            );
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

}