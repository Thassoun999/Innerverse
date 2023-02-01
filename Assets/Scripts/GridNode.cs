using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour {
    private int row;
    private int col;
    private enum occupationEnum {
        None,
        Human,
        Mycelium
    } 

    private occupationEnum occupation;

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

    void Start()
    {
        
    }
}