using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour {

    // ~ Instance and Variables ~

    private int row;
    private int col;
    private int occupation; // 0 for None, 1 for Mycelium, 2 for Human

    // ~ Properties ~

    public int[] Coordinates {
        get {
            return new int[] {row, col};
        }

        set {
            if (value.Length != 2) {
                Debug.LogError("Coordinate set given not valid! Please set as array of length 2 [row, col]!");
                return;
            }
            row = value[0];
            col = value[1];
            return;
        }
    }

    // ~ Methods ~

    void Start()
    {
        occupation = 0;
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;

    }

    void changeOccupation(int occupationChange) {
        if (occupationChange != 0 || occupationChange != 1 || occupationChange != 2) {
            Debug.LogError("changeOccupation Error: occupationChange variable not valid! Please revise!");
            return;
        }
        
        
        if (occupation == 1) {
            // Was Mycelium, no longer so decrement amount
            GameManager.Instance.MyceliumCount--;
        } else if (occupation == 2) {
            // Was Human, no longer so decrement amount
            GameManager.Instance.HumanCount--;
        }

        if (occupationChange == 1) {
            // Now is Mycelium, so increment amount
            GameManager.Instance.MyceliumCount++;
        } else if (occupationChange == 2){
            // Now is Human, so increment amount
            GameManager.Instance.HumanCount++;
        }

        occupation = occupationChange;
    }
}