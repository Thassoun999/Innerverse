using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mycelium : MonoBehaviour
{

    // ~ Instance and Variables ~

    // Every individual mecylium clump (1 per occupied grid node) will have their own private info
    [SerializeField] private List<GameObject> _ReinforcementMushrooms;
    private int row;
    private int col;
    private int maxHealth = 10;
    public int currHealth;
    [SerializeField] private HealthBar _healthbar;
    private bool reinforced; 

    private Highlight mycHighlight;
    private bool highlighting;
    private bool selected;

    public List<GridNode> unoccupiedGrids; // Grids to conquer
    public List<GridNode> occupiedGrids; // Grids containing people to attack

    private int totalRange = 2;

    private GridNode gridSelect = null;

    public bool actionReady = false;

    // Animator Controller
    private Animator _MycAnimator;

    // Sounds sources
    [SerializeField] private AudioSource[] _MycAudioSFXSources;


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

    public Animator MycAnimator {
        get {
            return _MycAnimator;
        }
    }

    public bool ActionReady {
        get {
            return actionReady;
        }
        set {
            actionReady = value;
        }
    }

    public GridNode GridSelect {
        get {
            return gridSelect;
        }
        set {
            gridSelect = value;
        }
    }

    public bool Selected {
        get {
            return selected;
        }
        set {
            selected = value;
        }
    }

    public bool Reinforced {
        get {
            return reinforced;
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
        reinforced = false;

        highlighting = false;
        selected = false;

        unoccupiedGrids = new List<GridNode>();
        occupiedGrids = new List<GridNode>();

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 1; // set to Mycelium
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;
    
        // Set the healthbar to max
        _healthbar.UpdateHealthBar(maxHealth, currHealth);

        _MycAnimator = GetComponent<Animator>();
        if(_MycAnimator != null) {
            _MycAnimator.SetTrigger("Grow");
        }

        // Play this on start (myc is growing)
        _MycAudioSFXSources[0].Play();

    }
    
    void OnDestroy()
    {
        if(!GameManager.Instance.GameOver){
            GameManager.Instance.removeMycelium(this.gameObject.GetInstanceID());
            GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0; // set to None
            GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null; // set to null
        }
    }

    // Update is called once per frame

    // Self destruct special action + Thorns passive special action needs to be implemented

    // NOTE: Mycelium only get special actions if at least ONE is on there
    void Update()
    {

        if (currHealth <= 0) {
            // Play death sound on grid not the player or it'll cut out on destroy
            GameManager.Instance.CoordsToGridNode[(row, col)].PlayMyceliumDeathSFX();
            Destroy(gameObject);
        }
    }

    public void Grow() {
        // SPAWN a mycelium on a selected grid
        SpawnManager.Instance.Spawn(gridSelect.Coordinates[0], gridSelect.Coordinates[1], "Myc");
        
        // We need this to update in realtime to know if we have our abilities or not
        if(gridSelect.SpecialClassifier == 1) {
            GameManager.Instance.MyceliumCountBiome1++;
        } else if(gridSelect.SpecialClassifier == 2) {
            GameManager.Instance.MyceliumCountBiome2++;
        }
        GameManager.Instance.ActionPoints -= 5;
        Reset();
        UIManager.Instance.DisableGameWheel();
    }

    public void Attack() {
        // We can attack Humans or Settlements please fix this...
        if(gridSelect.Occupation == 2) {
            Human toAttack = gridSelect.Standing.GetComponent(typeof(Human)) as Human;
            toAttack.Damage();
        } else if(gridSelect.Occupation == 3) {
            Settlement toAttack = gridSelect.Standing.GetComponent(typeof(Settlement)) as Settlement;
            toAttack.Damage();
        }
        GameManager.Instance.ActionPoints -= 6;

        // Play Myc Attack Animation
        if(_MycAnimator != null) {
            _MycAnimator.SetTrigger("Attack");
            // Play the sound effect
            _MycAudioSFXSources[2].Play();
        }

        // Play Grid Animation
        if(gridSelect.GridAnimator != null) {
            gridSelect.GridAnimator.SetTrigger("MycAttack");
        }
        Reset();
        UIManager.Instance.DisableGameWheel();
    }

    public void Reinforce() {
        if(reinforced) {
            return;
        }

        reinforced = true;
        currHealth += 5;
        GameManager.Instance.ActionPoints -= 3;

        // Spawn the two reinforce mushrooms and they will play their spawn animation
        _ReinforcementMushrooms[0].SetActive(true);
        _ReinforcementMushrooms[1].SetActive(true);

        // Play the sound effect
        _MycAudioSFXSources[1].Play();

        Reset();
        UIManager.Instance.DisableGameWheel();
    }

    public void SelfDestruct() {
        //Damage all enemies around us in a 1 grid radius
        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {
                if(i == 0 && j == 0)
                    continue;
                
                if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((row + i, col + j)))) {
                    continue;
                }

                GridNode gridCopy = GameManager.Instance.CoordsToGridNode[(row + i, col + j)];

                // Have spore animation play on every grid both occupied and non occupied
                gridCopy.GridAnimator.SetTrigger("MycAttack");

                // Play Mycelium SFX!!!

                // Human
                if(gridCopy.Occupation == 2) {
                    Human humanToDamage = gridCopy.Standing.GetComponent(typeof(Human)) as Human;
                    humanToDamage.Damage();
                } else if(gridCopy.Occupation == 3) { // Settlement
                    Settlement settlementToDamage = gridCopy.Standing.GetComponent(typeof(Settlement)) as Settlement;
                    settlementToDamage.Damage();
                }

            }
        }

        // Play self destruct animation
        GameManager.Instance.CoordsToGridNode[(row, col)].GridAnimator.SetTrigger("SelfDestruct");
        GameManager.Instance.CoordsToGridNode[(row, col)].PlaySelfDestructSFX();

        GameManager.Instance.ActionPoints -= 15;

        // Then destroy yourself
        Reset();
        UIManager.Instance.DisableGameWheel();
        Destroy(gameObject);
    }

    public void Reset() {
        actionReady = false;
        gridSelect = null;
        selected = false;
        mycHighlight.ToggleHighlight(false);
        GameManager.Instance.IsSelecting = null;

        // unhighlight the grid
        foreach(GridNode grid in unoccupiedGrids) {
            grid.GridHighlight.ToggleHighlightChoice(false, new Color(0.0f, 1.0f, 0.0f, 0.3f));
            grid.Clickable = false;
            grid.MyceliumSelect = null;
        }

        foreach(GridNode grid in occupiedGrids) {
            grid.GridHighlight.ToggleHighlightChoice(false, new Color(1.0f, 0.0f, 0.0f, 0.3f));
            grid.Clickable = false;
            grid.MyceliumSelect = null;
            if(grid.Standing.GetComponent<Human>() != null) {
                Human inOccupied = grid.Standing.GetComponent(typeof(Human)) as Human;
                inOccupied.Clickable = true;
            } else if(grid.Standing.GetComponent<Settlement>() != null) {
                Settlement inOccupied = grid.Standing.GetComponent(typeof(Settlement)) as Settlement;
                inOccupied.Clickable = true;
            }
        }

        // Re-initialize
        unoccupiedGrids = new List<GridNode>();
        occupiedGrids = new List<GridNode>();
    }

    public void Damage() {
        currHealth -= 9; // Change this to 10! Just need reference right now
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
    }

    // The following will toggle the mycelium highlight
    void OnMouseOver()
    {
        _healthbar.ToggleView(true); // show the health bar
        // Don't do anything if it's not the player's turn + if humans still moving
        if(!(GameManager.Instance.PlayerTurn && GameManager.Instance.NoHumanMovement()))
            return;

        if(GameManager.Instance.PlayerTurn){
            if(!highlighting && !selected) {
                highlighting = true;
                mycHighlight.ToggleHighlight(true);
            }
        }
    }
    
    void OnMouseExit()
    {
        _healthbar.ToggleView(false); // hide the health bar

        // Don't do anything if it's not the player's turn + if humans still moving
        if(!(GameManager.Instance.PlayerTurn && GameManager.Instance.NoHumanMovement()))
            return;

        if(GameManager.Instance.PlayerTurn){
            if(highlighting && !selected) {
                highlighting = false;
                mycHighlight.ToggleHighlight(false);
            }
        }

    }

    void OnMouseDown()
    {
        // The game's over, don't take clicks!
        if(GameManager.Instance.GameOver)
            return;

        // Don't do anything if it's not the player's turn + if humans still moving
        if(!(GameManager.Instance.PlayerTurn && GameManager.Instance.NoHumanMovement()))
            return;

        if(GameManager.Instance.PlayerTurn){
            // our object is being selected
            if (!selected) {
                _MycAudioSFXSources[3].Play();

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
                            continue;
                        }

                        if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 0) { // if unoccupied
                            unoccupiedGrids.Add(GameManager.Instance.CoordsToGridNode[(row + i, col + j)]);
                        } else if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 2 ||
                            GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 3) { // if human occupied
                            occupiedGrids.Add(GameManager.Instance.CoordsToGridNode[(row + i, col + j)]);
                        }
                        
                    }
                }

                // Highlight the grids in the necessary color and give them their mycelium select
                foreach(GridNode grid in unoccupiedGrids) {
                    grid.GridHighlight.ToggleHighlightChoice(true, new Color(0.0f, 1.0f, 0.0f, 0.3f));
                    grid.Clickable = true;
                    grid.MyceliumSelect = this;
                }

                foreach(GridNode grid in occupiedGrids) {
                    grid.GridHighlight.ToggleHighlightChoice(true, new Color(1.0f, 0.0f, 0.0f, 0.3f));
                    grid.Clickable = true;
                    grid.MyceliumSelect = this;
                    if(grid.Standing.GetComponent<Human>() != null) {
                        Human inOccupied = grid.Standing.GetComponent(typeof(Human)) as Human;
                        inOccupied.Clickable = true;
                    } else if(grid.Standing.GetComponent<Settlement>() != null) {
                        Settlement inOccupied = grid.Standing.GetComponent(typeof(Settlement)) as Settlement;
                        inOccupied.Clickable = true;
                    }
                }

                selected = true;
                mycHighlight.ToggleHighlight(true);
                UIManager.Instance.EnableAndUpdateGameWheel();
            } else { // our object is being deselected
                _MycAudioSFXSources[4].Play();
                Reset();
                UIManager.Instance.DisableGameWheel();
            }
        }
    }
}
