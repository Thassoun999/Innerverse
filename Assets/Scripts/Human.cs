using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    // ~ Instances and Variables ~

    private int row;
    private int col;
    private int maxHealth = 10;
    public int currHealth;

    private Highlight humHighlight;

    private bool clickable;
    private bool selected;

    private int totalRange = 4;

    private static int[] settlementBuilt = new int[] {0, 0, 0}; // Default, Special 1, Special 2 (if 0 no settlement, if 1 yes settlement)

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

    public static int[] SettlemntBuilt{
        get {
            return settlementBuilt;
        }
        set {
            settlementBuilt = value;
        }
    }

    public bool Clickable {
        get {
            return clickable;
        }
        set {
            clickable = value;
        }
    }

    // ~ Methods ~

    // Awake is called before the game starts -- use this to set up references (does not need to be enabled)
    void Awake()
    {
        humHighlight = gameObject.GetComponent<Highlight>();
    }

    // Start is called before the first frame update (script is enabled)
    void Start()
    { 
        // On awake, get your coordinate system! Possibly through the game manager
        // Also get the coordinate system of your "available" neighbors -- on update check if this changes for yourself
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 2; // set to Human
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;

        currHealth = maxHealth;
    }

    void OnDestroy()
    {
        GameManager.Instance.removeHuman(this.gameObject.GetInstanceID());
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0; // set to None
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null; // set to null
    }

    // Update is called once per frame

    void Update()
    {
        if (currHealth <= 0)
            Destroy(gameObject);

        if(!GameManager.Instance.PlayerTurn) {
            Debug.Log("Human Turn");
        }
    }

    // Simply for taking a step closer to whatever destination is chosen

    /*
    public Move(int[] source, int[] destination) {

    }

    */
    public void Damage() {
        Debug.Log("Human hit!!!");
        currHealth -= 5;
        Debug.Log(currHealth);
    }

    void OnMouseDown() {
        if (clickable) {
            // If we are selected, we need to make sure the grid we are standing on is the one being selected instead
            GameManager.Instance.CoordsToGridNode[(row, col)].OnMouseDownHumCall();
        }
    }
}
