using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyceliumScript : MonoBehaviour
{

     // ~ Instance and Variables ~

    // Every individual mecylium clump (1 per occupied grid node) will have their own private info
    private static int row;
    private static int col;


    // ~ Properties ~
    
    public static int[] Coordinates{
        get {
            return new int[] { row, col };
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

    // Awake is called when the script is instantied (per instance)
    void Awake()
    {
        // On awake, get your coordinate system! Possibly through the game manager
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
