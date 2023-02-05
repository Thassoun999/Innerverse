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

    private Highlight setHighlight;

    private bool clickable;
    private bool selected;

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

    // ~ Methods ~ 
    void Awake()
    {
        setHighlight = gameObject.GetComponent<Highlight>();
    }

    void Start()
    {
        // On awake, get your coordinate system! Possibly through the game manager
        // Also get the coordinate system of your "available" neighbors -- on update check if this changes for yourself
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;

        // Set grid we are standing on to occupied
        GameManager.Instance.CoordsToGridNode[(row, col)].Occupation = 3; // set to Settlement
        GameManager.Instance.CoordsToGridNode[(row, col)].Standing = gameObject;

        currHealth = maxHealth;

        // Check what kind of grid we are on, update the static Human settlement list
        int biomeSpec = GameManager.Instance.CoordsToGridNode[(row, col)].SpecialClassifier;

        Human.SettlemntBuilt[biomeSpec] = 1;
    }

    void Update()
    {
        if (currHealth <= 0)
            Destroy(gameObject);

    }

    public void SpawnHuman() {

        Debug.Log("here I am");
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
                        Debug.Log("there I am");
                        SpawnManager.Instance.Spawn(row + i, col + j, "Hum");
                        if(GameManager.Instance.CoordsToGridNode[(row + i, col + j)].SpecialClassifier == 1)
                            GameManager.Instance.HumanCountBiome1++;
                        else if (GameManager.Instance.CoordsToGridNode[(row + i, col + j)].SpecialClassifier == 1)
                            GameManager.Instance.HumanCountBiome2++;

                        GameManager.Instance.SettlementSpawnChance = 0.1f;
                        return; // leave immediately after spawning a guy in
                    }
                    
                }
            }
        }
    }

    public void Damage() {
        Debug.Log("Settlement Hit!!!");
        currHealth -= 5;
        Debug.Log(currHealth);
    }


}
