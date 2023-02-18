using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    // ~ Instances and Variables ~

    private int row;
    private int col;

    private int maxHealth = 35;
    public int currHealth;
    [SerializeField] private HealthBar _healthbar;

    private Highlight setHighlight;

    private bool clickable;
    private bool selected;
    private bool spawnTime = false;
    private bool dying = false;

    // ~ Properties ~

    public int[] Coordinates {
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

    public bool Clickable {
        get {
            return clickable;
        }
        set {
            clickable = value;
        }
    }

    public bool SpawnTime {
        get {
            return spawnTime;
        }
        set {
            spawnTime = value;
        }
    }

    // ~ Methods ~ 
    void Awake()
    {
        setHighlight = gameObject.GetComponent<Highlight>();
    }

    void OnDestroy()
    {
        int biomeSpec = GameManager.Instance.CoordsToGridNode[(row, col)].SpecialClassifier;
        GameManager.Instance.SettlementBuilt[biomeSpec] = 0;

        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 0;
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = null;
        GameManager.Instance.removeSettlement(this.gameObject.GetInstanceID());
    }

    void Start()
    {
        // On awake, get your coordinate system! Possibly through the game manager
        // Also get the coordinate system of your "available" neighbors -- on update check if this changes for yourself
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;

        spawnTime = false;

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 3; // set to Settlement
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;

        currHealth = maxHealth;

        // Check what kind of grid we are on, update the static Human settlement list
        int biomeSpec = GameManager.Instance.CoordsToGridNode[(row, col)].SpecialClassifier;

        GameManager.Instance.SettlementBuilt[biomeSpec] = 1;

        dying = false;

        // Set the healthbar to max
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
    }

    void DestroySettlement() {
        Destroy(gameObject);
    }

    void Update()
    {
        if (currHealth <= 0 && !dying) {
            dying = true;
            gameObject.GetComponent<Animator>().SetTrigger("Death");
        }

        if(spawnTime) {
            spawnTime = false;

            // Cap the total number of humans to be spawned! Only want less than 20 to allow a spawn
            if(GameManager.Instance._HumanCount < 20)
                spawnHuman();
        }

    }

    void spawnHuman() {
        float randVal = Random.Range(0.0f, 1.0f);

        // If this works then the random chance worked!!! Spawn a human!
        if(randVal <= GameManager.Instance.SettlementSpawnChance) {
            for(int i = -1; i < 2; i++) {
                for(int j = -1; j < 2; j ++) {
                    if(i == 0 & j == 0) 
                        continue;

                    if (!(GameManager.Instance.CoordsToGridNode.ContainsKey((row + i, col + j)))) 
                        continue;

                    if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].Occupation == 0) {
                        GameManager.Instance.SettlementSpawnChance = 0.1f;
                        SpawnManager.Instance.Spawn(row + i, col + j, "Hum");

                        return; // leave immediately after spawning a guy in
                    }
                    
                }
            }
        }
    }

    public void Damage() {
        currHealth -= 5;

        // Set the healthbar to max
        _healthbar.UpdateHealthBar(maxHealth, currHealth);
    }

    void OnMouseOver() 
    {
        _healthbar.ToggleView(true); // show the health bar
    }

    void OnMouseExit() {
        _healthbar.ToggleView(false); // hide the health bar
    }

    // Code also applies to settlements
    void OnMouseDown() {
        if (clickable) {
            // If we are selected, we need to make sure the grid we are standing on is the one being selected instead
            GameManager.Instance.CoordsToGridNode[(row, col)].OnMouseDownHumCall();
        }
    }


}
