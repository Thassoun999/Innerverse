using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Fight : Node
{
    PathFinding pathfindingClass = new PathFinding();

    public override NodeState Evaluate()
    {
        if(GameManager.Instance.PlayerTurn)
            return NodeState.FAILURE;

        float humanToMycRatio = (float)GetData("Scan Ratio");
        
        // Only engage in combat if humans "generally" outnumber the mycelium they see in range
        // Also if there are ACTUALLY Mycelium to fight
        if (humanToMycRatio < 1 || humanToMycRatio == -1.0f) {
            return NodeState.FAILURE;
        }

        foreach(KeyValuePair<int, Human> elem in GameManager.Instance.HumanGroup) {
            Human tempHum = elem.Value;
            int[] tempHumanCoords = tempHum.Coordinates;
            GridNode start = GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])];
            for(int i = -tempHum.TotalRange; i < tempHum.TotalRange + 1; i++) {
                for(int j = -tempHum.TotalRange; j < tempHum.TotalRange + 1; j++) {
                    if (i == 0 && j == 0)
                        continue;

                    if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((tempHumanCoords[0], tempHumanCoords[1]))))
                        continue;

                    // There's a Mycelium
                    if (GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])].Occupation == 1) {
                        // Need to find an empty cell near that Mycelium, if there isn't one then continue
                        Mycelium tempMyc = GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])].Standing.GetComponent(
                            typeof(Mycelium)) as Mycelium;

                        // Find destination node
                        int[] coords = tempMyc.Coordinates;
                        GridNode destination = null;
                        for(int x = -1; x < 2; x++) {
                            for (int y = -1; y < 2; y++) {
                                if (x == 0 && y == 0)
                                    continue;
                                
                                if(!GameManager.Instance.CoordsToGridNode.ContainsKey((coords[0], coords[1])))
                                    continue;

                                if(GameManager.Instance.CoordsToGridNode[(coords[0] + x, coords[1] + y)].Occupation == 0) {
                                    destination = GameManager.Instance.CoordsToGridNode[(coords[0] + x, coords[1] + y)];
                                    break;
                                }

                            }
                        }

                        // No destination found
                        if (destination == null) {
                            continue;
                        }

                        // Find Path
                        List<GridNode> path = pathfindingClass.A_Star(ref tempHum, destination);
                        // No valid path to destination, just continue
                        if(path == null)
                            continue;

                        // Move our soldier!!!
                        Move(path, ref tempHum);

                        // ATTACK OUR MYCELIUM!!! (Since we are in range)
                        tempMyc.Damage();
                    }
                }
            }
        }

        return NodeState.SUCCESS;
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
