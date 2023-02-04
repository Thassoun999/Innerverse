using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Build : Node
{

    public Build() {

    }

    public override NodeState Evaluate()
    {
        int[] buildCoords = GetData("BuildCoords") as int[];
        int biomeSpecification = (int)GetData("Biome");

        SpawnManager.Instance.Spawn(buildCoords[0], buildCoords[1], "Settlement");

        return NodeState.SUCCESS;

    }
}
