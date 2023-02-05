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

        return NodeState.SUCCESS;
    }
}
