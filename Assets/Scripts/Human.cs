using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    // ~ Instances and Variables ~

    private int row;
    private int col;
    private int maxHealth = 10;
    private int currHealth;

    // ~ Properties ~
    public int[] Coordinates{
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

    // ~ Methods ~

    // Awake is called before the game starts -- use this to set up references (does not need to be enabled)
    void Awake()
    {

    }

    // Start is called before the first frame update (script is enabled)
    void Start()
    {
        // On awake, get your coordinate system! Possibly through the game manager
        // Also get the coordinate system of your "available" neighbors -- on update check if this changes for yourself
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;

        currHealth = maxHealth;
    }

    void OnDestroy()
    {
        GameManager.Instance.removeHuman(this.gameObject.GetInstanceID());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
