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
    [SerializeField] private HealthBar _healthbar;

    public bool clickable;
    private bool selected;

    // Walking varaibles for movement algorithm
    private int totalRange = 3;
    private bool moveActivated;
    private bool walking;
    private List<GridNode> path;
    private GridNode currNode;
    private Vector3 currTarget;
    private int pathInd;
    private int pathRange;
    private float recordedDistanceToNode;
    private Vector3 velocity;

    // Attacking parameters
    private int[] attackingCoords;
    private bool attackTime;

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

    public int TotalRange {
        get {
            return totalRange;
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

    public bool MoveActivated {
        get {
            return moveActivated;
        } 
        set {
            moveActivated = value;
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

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 2; // set to Human
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;

        // No need to update the humancountbiome here since advanceTurn calculates totals after a spawn happens
        currHealth = maxHealth;
        walking = false;
        moveActivated = false;
        attackTime = false;

        // Set the healthbar to max
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
    }

    void OnDestroy()
    {
        GameManager.Instance.CoordsToGridNode[(row, col)].GridAnimator.SetTrigger("HumAttack"); // The truck explodes!
        GameManager.Instance.removeHuman(this.gameObject.GetInstanceID());
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0; // set to None
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null; // set to null

    }

    // Update is called once per frame

    void Update()
    {
        if (currHealth <= 0)
            Destroy(gameObject);

        // Walking animation -- this needs to be changed into a forloop
        if(moveActivated) {
            if (pathInd < path.Count && pathRange > 0) {
                if(walking == false) { // Guaranteed to always go first -- Set Destination
                    
                    walking = true;
                    currNode = path[pathInd];

                    // Need the human's Y here to not sink it into the ground
                    currTarget = new Vector3(
                        currNode.gameObject.transform.localPosition.x,
                        gameObject.transform.localPosition.y,
                        currNode.gameObject.transform.localPosition.z
                    );
                } else if(walking == true && recordedDistanceToNode < 0.1f) {
                    // Move on to next grid
                    pathRange--;
                    pathInd++;
                    walking = false;
                }

                Vector3 adjustedAgentPos = new Vector3(gameObject.transform.localPosition.x, 0, gameObject.transform.localPosition.z);
                Vector3 adjustedDestPos = new Vector3(currNode.gameObject.transform.localPosition.x, 0, currNode.gameObject.transform.localPosition.z);
                recordedDistanceToNode = Vector3.Distance(adjustedAgentPos, adjustedDestPos);

                Vector3 directionAndMovement = Vector3.SmoothDamp(
                    gameObject.transform.localPosition,
                    currTarget,
                    ref velocity,
                    0.3f
                );

                gameObject.transform.localPosition = directionAndMovement; // moving
            } else { // walking done, need to solidify the value of our human
                int[] currNodeCoords = currNode.Coordinates;

                row = currNodeCoords[0];
                col = currNodeCoords[1];

                // SmoothDamp works with approximates, we need to snap to our last position and record it!
                gameObject.transform.localPosition = new Vector3(
                    (float)row, 
                    gameObject.transform.localPosition.y, 
                    (float)col
                );
                moveActivated = false;

                // This isn't just a move, but also an attack! Damage our Mycelium!
                if (attackTime) {
                    Attack();
                }
            }
        }
    }

    public void SetPath(ref List<GridNode> newPath) {

        path = newPath;
        moveActivated = true;

        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0;
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null; // set to null

        pathInd = 0;
        walking = false;

        pathRange = totalRange;
        recordedDistanceToNode = Mathf.Infinity;
        velocity = Vector3.zero;

        // Anticipate that grid being occupied (important so that no human's destination ends up being the same spot)
        int[] lastNodeCoords;

        // EITHER SET IT AS THE TOTAL RANGE OR THE PATH LENGTH IF PATH IS SHORTER!!!
        if(totalRange < path.Count) {
            lastNodeCoords = path[totalRange - 1].Coordinates;
        } else {
            lastNodeCoords = path[path.Count - 1].Coordinates; 
        }
        GameManager.Instance.CoordsToGridNode[(lastNodeCoords[0], lastNodeCoords[1])].Occupation = 2;
        GameManager.Instance.CoordsToGridNode[(lastNodeCoords[0], lastNodeCoords[1])].Standing = gameObject;

    }

    // Sets the target and enables attacking
    public void SetTarget(int[] coords) {
        attackTime = true;
        attackingCoords = coords;
    }

    public void Damage() {
        Debug.Log("Human hit!!!");
        currHealth -= 5;
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
        Debug.Log(currHealth);
    }

    public void MiniDamange() {
        Debug.Log("Human backfire!");
        currHealth -= 3;
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
        Debug.Log(currHealth);
    }

    public void Attack() {
        Mycelium tempMyc = GameManager.Instance.CoordsToGridNode[(attackingCoords[0], attackingCoords[1])].Standing.GetComponent(typeof(Mycelium)) as Mycelium;
        if(tempMyc) {
            // Grid Explosion Animation
            GameManager.Instance.CoordsToGridNode[(attackingCoords[0], attackingCoords[1])].GridAnimator.SetTrigger("HumAttack");
            tempMyc.Damage();
            attackTime = false;
        }

        // Take thorns damage if Mycelium unlocked power of special biome 2!
        if(GameManager.Instance.MyceliumCountBiome2 > 0) {
            MiniDamange();
        }

    }

    void OnMouseOver() 
    {
        _healthbar.ToggleView(true); // show the health bar
    }

    void OnMouseExit() {
        _healthbar.ToggleView(false); // hide the health bar
    }

    void OnMouseDown() {
        Debug.Log("CLIRNEOSGNBSUIOERBGIUESR");
        if (clickable) {
            // If we are selected, we need to make sure the grid we are standing on is the one being selected instead
            GameManager.Instance.CoordsToGridNode[(row, col)].OnMouseDownHumCall();
        }
    }
}
