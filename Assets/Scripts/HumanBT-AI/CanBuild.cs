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

        object t = GetData("canBuild");
        if (t == null) {
            int[] temp = new int[] {-1, -1};
            // Check if count requirements are met
            if(GameManager.Instance.HumanCountBiome1 >= 5 && Human.SettlemntBuilt[1] == 0) {
                // Check for the first available space to build our settlement!
                foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                    if(elem.Value.Occupation == 0) {
                        temp[0] = elem.Key.Item1;
                        temp[1] = elem.Key.Item2;

                        parent.parent.SetData("BuildCoords", temp);
                        parent.parent.SetData("Biome", 1);
                        return NodeState.SUCCESS;
                    }
                }
            }

            if(GameManager.Instance.HumanCountBiome2 >= 5 && Human.SettlemntBuilt[2] == 0) {
            // Check for the first available space to build our settlement!
                foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                    if(elem.Value.Occupation == 0) {
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

/*
    public void Build(int row, int col) {
        for(int i = -totalRange; i < totalRange + 1; i++) {
            for(int j = -totalRange)
        }
    }

    // Simply for taking a step closer to whatever destination is chosen
    public Move(int[] source, int[] destination) {

    }

    public (bool, int, int[]) canBuild() {

        int[] temp = new int[] {-1, -1};

        // Check if count requirements are met
        if(GameManager.Instance.HumanCountBiome1 >= 5 && settlementBuilt[1] == 0) {
            // Check for the first available space to build our settlement!
            foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                if(elem.Value.Occupation == 0) {
                    temp[0] = elem.Key.Item1;
                    temp[1] = elem.Key.Item2;

                    return(true, 1, temp);
                }
            }
            return (false, 1, temp);
        }

        if(GameManager.Instance.HumanCountBiome2 >= 5 && settlementBuilt[2] == 0) {
            // Check for the first available space to build our settlement!
            foreach (KeyValuePair<(int, int), GridNode> elem in GameManager.Instance.CoordsToGridNode){
                if(elem.Value.Occupation == 0) {
                    temp[0] = elem.Key.Item1;
                    temp[1] = elem.Key.Item2;

                    return(true, 2, temp);
                }
            }
            return (false, 2, temp);
        }


        return (false, -1, temp);
    }

    public void Build(int row, int col) {
        for(int i = -totalRange; i < totalRange + 1; i++) {
            for(int j = -totalRange)
        }
    }

    // Simply for taking a step closer to whatever destination is chosen
    public Move(int[] source, int[] destination) {

    }


*/