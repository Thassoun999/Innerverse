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

            // for(int i = -tempHum.TotalRange; i < tempHum)
            return NodeState.RUNNING;
        }
        return NodeState.RUNNING;
    }
}
