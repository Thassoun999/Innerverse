using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mycelium : MonoBehaviour
{

    // ~ Instance and Variables ~

    // Every individual mecylium clump (1 per occupied grid node) will have their own private info
    private int row;
    private int col;
    private int maxHealth = 10;
    public int currHealth;

    private Highlight mycHighlight;
    private bool highlighting;
    private bool selected;

    public List<GridNode> unoccupiedGrids; // Grids to conquer
    public List<GridNode> occupiedGrids; // Grids containing people to attack

    private int totalRange = 2;

    private GridNode gridSelect = null;

    private bool actionReady = false;


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

    public bool ActionReady {
        set {
            actionReady = value;
        }
    }

    public GridNode GridSelect {
        set {
            gridSelect = value;
        }
    }

    // ~ Methods ~

    // Awake is called before the game starts -- use this to set up references (does not need to be enabled)
    void Awake()
    {
        mycHighlight = gameObject.GetComponent<Highlight>();
    }

    // Start is called before the first frame update (script is enabled)
    void Start()
    {
        
        // On start, get your coordinate system! Possibly through the game manager
        // Also get the coordinate system of your "available" neighbors -- on update check if this changes for yourself
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;
        currHealth = maxHealth;

        highlighting = false;
        selected = false;

        unoccupiedGrids = new List<GridNode>();
        occupiedGrids = new List<GridNode>();

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 1; // set to Mycelium
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;
    }
    
    void OnDestroy()
    {
        GameManager.Instance.removeMycelium(this.gameObject.GetInstanceID());
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0; // set to None
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null; // set to null
    }

    // Update is called once per frame
    void Update()
    {

        if (currHealth <= 0)
            Destroy(gameObject);

        if(GameManager.Instance.PlayerTurn) {

            // First check that we're not already poor
            if(GameManager.Instance.ActionPoints > 0){
                // actionReady only called when this Mycelium is selected AND grid is selected
                if(actionReady && GameManager.Instance.ActionPoints > 0) {
                    if(Input.GetKeyDown(KeyCode.A) && (GameManager.Instance.ActionPoints - 2) >= 0 && gridSelect.Occupation == 0) {
                        // Grow Action method here -- NEEDS TO BE AN UNOCCUPIED GRID
                        Debug.Log("Growing");
                        Grow();
                        // Action point spent here
                        GameManager.Instance.ActionPoints -= 2;
                        Reset();
                    } else if(Input.GetKeyDown(KeyCode.S) && (GameManager.Instance.ActionPoints - 10) >= 0 && gridSelect.Occupation == 2) {
                        // Basic Attack Action method here -- NEEDS TO BE AN OCCUPIED GRID
                        Attack();
                        //Action point spent here
                        GameManager.Instance.ActionPoints -= 10;
                        Reset();
                    }
                } else if (!actionReady && GameManager.Instance.IsSelecting == this) { // This statement should only be reached if no grid selected but Mycelium is
                    if(Input.GetKeyDown(KeyCode.D) && (GameManager.Instance.ActionPoints - 5) >= 0) {
                        Debug.Log("DEFENSE UP");

                        GameManager.Instance.ActionPoints -= 5;
                        Reset();
                    }
                }
            }
        }


        // End turn action
        if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
            GameManager.Instance.advanceTurn();
        }
    }

    void Grow() {
        // SPAWN a mycelium on a selected grid
        SpawnManager.Instance.Spawn(gridSelect.Coordinates[0], gridSelect.Coordinates[1], "Myc");
    }

    void Attack() {
        Human toAttack = gridSelect.Standing.GetComponent(typeof(Human)) as Human;
        toAttack.Damage();
    }

    void Reset() {
        actionReady = false;
        gridSelect = null;
        selected = false;
        mycHighlight.ToggleHighlight(false);
        GameManager.Instance.IsSelecting = null;

        // unhighlight the grid
        foreach(GridNode grid in unoccupiedGrids) {
            grid.GridHighlight.ToggleHighlightChoice(false, Color.green);
            grid.Clickable = false;
            grid.MyceliumSelect = null;
        }

        foreach(GridNode grid in occupiedGrids) {
            grid.GridHighlight.ToggleHighlightChoice(false, Color.red);
            grid.Clickable = false;
            grid.MyceliumSelect = null;
            Human inOccupied = grid.Standing.GetComponent(typeof(Human)) as Human;
            inOccupied.Clickable = true;
        }

        // Re-initialize
        unoccupiedGrids = new List<GridNode>();
        occupiedGrids = new List<GridNode>();
    }

    public void Damage() {
        Debug.Log("Mycelium Hit!!!");
        currHealth -= 5;
        Debug.Log(currHealth);
    }

    // The following will toggle the mycelium highlight
    void OnMouseOver()
    {
        if(GameManager.Instance.PlayerTurn){
            if(!highlighting && !selected) {
                highlighting = true;
                mycHighlight.ToggleHighlight(true);
            }
        }
    }
    
    void OnMouseExit()
    {
        if(GameManager.Instance.PlayerTurn){
            if(highlighting && !selected) {
                highlighting = false;
                mycHighlight.ToggleHighlight(false);
            }
        }

    }

    void OnMouseDown()
    {
        if(GameManager.Instance.PlayerTurn){
            // our object is being selected
            if (!selected) {

                // Change our selection if we have already selected something
                if (GameManager.Instance.IsSelecting != null) {
                    GameManager.Instance.IsSelecting.Reset();
                }

                GameManager.Instance.IsSelecting = this;
                // Re-initialize
                unoccupiedGrids = new List<GridNode>();
                occupiedGrids = new List<GridNode>();

                // Find valid neighboring grids that AREN'T obstacles, friendly, or out of bounds
                for(int i = -totalRange; i < totalRange + 1; i++) {
                    for(int j = -totalRange; j < totalRange + 1; j++){
                        if(i == 0 && j == 0) // don't highlight the grid we are standing on
                            continue;

                        // Check to see if we are not out of bounds (don't want to run into an error)
                        if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((row + i, col + j)))) {
                            Debug.Log("here");
                            //Debug.Log("row: " + (row + i)  + " col: " + (col + j));
                            continue;
                        }

                        if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 0) { // if unoccupied
                            unoccupiedGrids.Add(GameManager.Instance.CoordsToGridNode[(row + i, col + j)]);
                        } else if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 2) { // if human occupied
                            occupiedGrids.Add(GameManager.Instance.CoordsToGridNode[(row + i, col + j)]);
                        }
                        
                    }
                }

                // Highlight the grids in the necessary color and give them their mycelium select
                foreach(GridNode grid in unoccupiedGrids) {
                    grid.GridHighlight.ToggleHighlightChoice(true, Color.green);
                    grid.Clickable = true;
                    grid.MyceliumSelect = this;
                }

                foreach(GridNode grid in occupiedGrids) {
                    grid.GridHighlight.ToggleHighlightChoice(true, Color.red);
                    grid.Clickable = true;
                    grid.MyceliumSelect = this;
                    Human inOccupied = grid.Standing.GetComponent(typeof(Human)) as Human;
                    inOccupied.Clickable = true;
                }

                selected = true;
                mycHighlight.ToggleHighlight(true);
            } else { // our object is being deselected
                Reset();
            }
        }
    }
}
