using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class ScanSurroundings : Node
{
    public override NodeState Evaluate()
    {
        Dictionary<int, Mycelium> tempMycRecord = new Dictionary<int, Mycelium>(); // keep track of all of our Mycelium we have already counted over

        // Look for Mycelium in range, get the total count per Mycelium (make sure we aren't double counting)
        foreach(KeyValuePair<int, Human> elem in GameManager.Instance.HumanGroup) {
            Human tempHum = elem.Value;
            int[] tempHumanCoords = tempHum.Coordinates;
            for(int i = -tempHum.TotalRange; i < tempHum.TotalRange + 1; i++) {
                for(int j = -tempHum.TotalRange; j < tempHum.TotalRange + 1; j++) {
                    if (i == 0 && j == 0)
                        continue;

                    if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((tempHumanCoords[0], tempHumanCoords[1]))))
                        continue;

                    // There's a Mycelium
                    if (GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])].Occupation == 1) {
                        GameObject newMyc = GameManager.Instance.CoordsToGridNode[(tempHumanCoords[0], tempHumanCoords[1])].Standing;
                        if(!(tempMycRecord[newMyc.GetInstanceID()])) {
                            tempMycRecord.Add(newMyc.GetInstanceID(), newMyc.GetComponent(typeof (Mycelium)) as Mycelium);
                        }
                    }
                }
            }
        }

        float _HumanToMyceliumRatio = (float)GameManager.Instance._HumanCount / (float)tempMycRecord.Count;

        parent.parent.SetData("Mycelium Dict", tempMycRecord);
        parent.parent.SetData("Scan Ratio", _HumanToMyceliumRatio);
        return NodeState.SUCCESS;


    }
}
