using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CanBuild : Node
{
    public CanBuild() {

    }

    public override NodeState Evaluate()
    {
        if(GameManager.Instance.PlayerTurn)
            return NodeState.FAILURE;

        object t = GetData("canBuild");
        if (t == null) {
            int[] temp = new int[] {-1, -1};
            // Check if count requirements are met
            if(GameManager.Instance.HumanCountBiome1 >= 5 && GameManager.Instance.SettlementBuilt[1] == 0) {
                // Check for the first available space to build our settlement!
                foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                    if(elem.Value.Occupation == 0 && elem.Value.SpecialClassifier == 1) {
                        temp[0] = elem.Key.Item1;
                        temp[1] = elem.Key.Item2;

                        parent.parent.SetData("BuildCoords", temp);
                        parent.parent.SetData("Biome", 1);
                        return NodeState.SUCCESS;
                    }
                }
            }

            if(GameManager.Instance.HumanCountBiome2 >= 5 && GameManager.Instance.SettlementBuilt[2] == 0) {
            // Check for the first available space to build our settlement!
                foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                    if(elem.Value.Occupation == 0 && elem.Value.SpecialClassifier == 2) {
                        temp[0] = elem.Key.Item1;
                        temp[1] = elem.Key.Item2;

                        parent.parent.SetData("BuildCoords", temp);
                        parent.parent.SetData("Biome", 2);
                        return NodeState.SUCCESS;
                    }
                }
            }
        }


        return NodeState.FAILURE;
    }
}