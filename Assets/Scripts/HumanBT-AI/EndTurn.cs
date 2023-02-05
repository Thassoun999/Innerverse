using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EndTurn : Node
{
    public override NodeState Evaluate()
    {
        if(GameManager.Instance.PlayerTurn)
            return NodeState.FAILURE;
            
        // End your turn here!
        GameManager.Instance.PlayerTurn = true;

        // Clearing all the data
        ClearData("Scan Ratio");
        ClearData("Mycelium Dict");
        ClearData("Biome");
        ClearData("BuildCoords");

        return NodeState.SUCCESS;
    }
}
