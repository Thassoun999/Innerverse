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
    private int currHealth;

    private int maxActionPoionts = 20;
    private int currActionPoints;

    private Highlight mycHighlight;
    private bool highlighting;
    private bool selected;

    private List<GridNode> unoccupiedGrids; // Grids to conquer
    private List<GridNode> occupiedGrids; // Grids containing people to attack

    private int totalRange = 1;

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
        currActionPoints = maxActionPoionts;

        highlighting = false;
        selected = false;

        unoccupiedGrids = new List<GridNode>();
        occupiedGrids = new List<GridNode>();

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 1; // set to Mycelium
    }
    
    void OnDestroy()
    {
        GameManager.Instance.removeMycelium(this.gameObject.GetInstanceID());
    }

    // Update is called once per frame
    void Update()
    {
        if(actionReady) {
            Debug.Log("GIVE ME INPUT!");
            Debug.Log(gridSelect);
        }
    }

    // The following will toggle the mycelium highlight
    void OnMouseOver()
    {
        if(!highlighting && !selected) {
            highlighting = true;
            mycHighlight.ToggleHighlight(true);
        }
    }
    
    void OnMouseExit()
    {
        if(highlighting && !selected) {
            highlighting = false;
            mycHighlight.ToggleHighlight(false);
        }
    }

    void OnMouseDown()
    {
        // our object is being selected
        if (!selected) {
            
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
                        //Debug.Log("here");
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
            }

            selected = true;
            mycHighlight.ToggleHighlight(true);
        } else { // our object is being deselected

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
            }

            selected = false;
            mycHighlight.ToggleHighlight(false);
            actionReady = false; // if we unselect ourselves then we're not performing any actions
            gridSelect = null; // also remove our grid select so if we re-select ourselves we don't have a saved grid
        }
    }
}
