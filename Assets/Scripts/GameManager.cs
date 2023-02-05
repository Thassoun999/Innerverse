using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton: globally accessible class that exists in the scene but only once (any other script can access it)

public class GameManager : MonoBehaviour
{
    // ~ Instance and Variables ~

    // Assuring that only the GameManager class can modify this and that it's shared through all instances of the GameManager class in different scripts
    // Static members can also be accessed directly from a class without instantiating an object of the class first.
    private static GameManager _instance;
    private bool isPlayerTurn;
    private List<List<int>> _TwoDimensionalGridMap; // a 2D representation of isometric map on 2D list.
    private Dictionary<(int, int), GridNode> _CoordstoGridNode;

    public Dictionary<int ,Human> _HumanGroup; // instanceId to ref game object
    public int _HumanCount; //  Comparing this with the grid count and human count helps to know if mycelium wins
    public Dictionary<int, Mycelium> _MyceliumGroup; // instanceId to ref game object
    public int _MyceliumCount; // Comparing this with the grid count and human count helps to know if mycelium wins
    public Dictionary<int, Settlement> _SettlementGroup;
    public int _SettlementCount;
    
    private int maxActionPoionts = 22;
    private int currActionPoints;

    private Mycelium isSelecting; // Can't select multiple Mycelium at once!

    private int _HumanCountBiome1;
    private int _HumanCountBiome2;
    private int _MyceliumCountBiome1;
    private int _MyceliumCountBiome2;

    // (29; 0) Special Biome 2
    // (0; 29) Special Biome 1

    private float _settlementSpawnChance = 0.1f;

    // ~ Properties ~

    // Encapsulation -- For other classes to access our unique instance, we are creating a public property with just a get option (get or set member variables of class)
    // This allows a layer of protection against overwriting our private static instance of GameManager (greater control of when and how a field is accessed)
    // Instance will also be static to make sure it is shared and consistent outside of this script
    // Here we have a Read-Only property
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game Manager is NULL");
            
            return _instance;
        }
    }

    public Dictionary<int, Mycelium> MyceliumGroup {
        get {
            return _MyceliumGroup;
        }
    }

    public Dictionary<int, Human> HumanGroup {
        get {
            return _HumanGroup;
        }
    }

    public Dictionary<int, Settlement> SettlementGroup {
        get {
            return _SettlementGroup;
        }
    }

    public int HumanCountBiome1 {
        get {
            return _HumanCountBiome1;
        }
        set {
            _HumanCountBiome1 = value;
        }
    }

    public int HumanCountBiome2 {
        get {
            return _HumanCountBiome2;
        }
        set {
            _HumanCountBiome2 = value;
        }
    }

    public int MyceliumCountBiome1 {
        get {
            return _MyceliumCountBiome1;
        }
        set {
            _MyceliumCountBiome1 = value;
        }
    }

    public int MyceliumCountBiome2 {
        get {
            return _MyceliumCountBiome2;
        }
        set {
            _MyceliumCountBiome2 = value;
        }
    }

    public int ActionPoints {
        get {
            return currActionPoints;
        }
        set {
            currActionPoints = value;
        }
    }

    public bool PlayerTurn
    {
        get {
            return isPlayerTurn;
        }
        set {
            isPlayerTurn = value;
            return;
        }
    }

    public float SettlementSpawnChance {
        get {
            return _settlementSpawnChance;
        }
        set {
            _settlementSpawnChance = value;
        }
    }

    public int MyceliumCount {
        get {
            return _MyceliumCount;
        }

        set {
            _MyceliumCount = value;
        }
    }

    public int HumanCount {
        get {
            return _HumanCount;
        }

        set {
            _HumanCount = value;
        }
    }

    public int SettlementCount {
        get {
            return _SettlementCount;
        }
        
        set {
            _SettlementCount = value;
        }
    }

    public Mycelium IsSelecting {
        get {
            return isSelecting;
        }
        set {
            isSelecting = value;
        }
    }

    public Dictionary<(int, int), GridNode> CoordsToGridNode {
        get {
            return _CoordstoGridNode;
        }
    }


    // ~ Methods ~

    // Either on AWAKE or on START instantiate a starting number of humans and mycelium!

    void Awake()
    {
        if (_instance != null) { // need to ensure that this remains a singleton!
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _CoordstoGridNode = new Dictionary<(int, int), GridNode>();
        _TwoDimensionalGridMap = new List<List<int>>();
        GridNode[] grids;

        grids = FindObjectsOfType<GridNode>();

        isPlayerTurn = true; // always starts off as the player's turn
        currActionPoints = maxActionPoionts;

        Dictionary<int, List<int>> tempDict = new Dictionary<int,  List<int>>(); // temporary dictionary we will convert to list later
        
        
        // making the 2d map representation and the grid dictionary
        for (int i = 0; i < grids.Length; i++) {
            int[] coords = grids[i].Coordinates;
            _CoordstoGridNode[(coords[0], coords[1])] = grids[i];

            if (tempDict.ContainsKey(coords[0])) {
                tempDict[coords[0]].Add(coords[1]); // we will sort this by row to have an adjacency list (grid map!)

            } else {
                tempDict[coords[0]] = new List<int>(){coords[1]};
            }
        }

        List<int> rowList = new List<int>(); // temporary row list
        foreach(int key in tempDict.Keys) {
            rowList.Add(key);
        }
        rowList.Sort(); // sort the rows

        // create our two dimensional grid map
        foreach(int row in rowList){
            _TwoDimensionalGridMap.Add(tempDict[row]); // add column list to row index
            _TwoDimensionalGridMap[row].Sort(); // sort the columns now
        }

        // Declare dictionnaries
        _HumanGroup = new Dictionary<int, Human>();
        _MyceliumGroup = new Dictionary<int, Mycelium>();
        _SettlementGroup = new Dictionary<int, Settlement>();

        // Spawn our first Mycelium and make sure to add to necessary groups

        // THIS NEEDS TO BE CHANGED AND HANDLED MORE CLEANLY IN A FUNCTION OR SOMETHING -- DECIDE LATER
        // May not need this

        // Spawn Human very closeby
        //SpawnManager.Instance.Spawn(5, 4, "Myc");
        SpawnManager.Instance.Spawn(26, 26, "Myc");
        SpawnManager.Instance.Spawn(22, 24, "Hum");
        SpawnManager.Instance.Spawn(25, 26, "Settlement");
    }

    // Update is called once per frame
    void Update()
    {
        // WIN-LOSE Conditions!

        // CHECK 1 -- Check to see if one side has a count of 0, the other side wins
        if(_MyceliumCount == 0) {
            Debug.Log("Humans Win!!!");
        } else if(_HumanCount == 0) {
            Debug.Log("Mycelium Wins!!!!!");
        }
        
        // CHECK 2 -- Check to see if >75% of the grid map is covered (is occupied) -- Winner is whoever has higher count
        float ratio = (float)(_MyceliumCount + _HumanCount) / (float)(CoordsToGridNode.Count);
        if(ratio >= 0.75) {
            if(_MyceliumCount > _HumanCount) {
                Debug.Log("Mycelium Wins!!!!!");
            } else if(_HumanCount > _MyceliumCount) {
                Debug.Log("Humans Win!!!");
            }
        }

        // CHECK 3 -- Check to see if all settlements have been destroyed, if this is done then the Mycelium win
        if(_SettlementCount == 0) {
            Debug.Log("Mycelium Wins!!!!!");
        }

    }

    public void advanceTurn() {
        // Change the turn and set to opposing action points
        isPlayerTurn =! isPlayerTurn;

        // go through grids and check to see how many humans and mycelium are on each biome 1 and 2
        countInSpecialBiome();

        // Every Settlement attempts to spawn a human!
        foreach(KeyValuePair<int, Settlement> elem in _SettlementGroup) {
            elem.Value.SpawnHuman();
        }

        _settlementSpawnChance += 0.45f; // Increase chance of settlement spawning human per turn
        // If Settlement spawns a human, reset the chance back to 10%
    }

    public void countInSpecialBiome() {
        int tempHumanBiome1Count = 0;
        int tempHumanBiome2Count = 0;
        int tempMycBiome1Count = 0;
        int tempMycBiome2Count = 0;

        foreach (KeyValuePair<(int, int), GridNode> elem in _CoordstoGridNode){
            if (elem.Value.SpecialClassifier == 1 && elem.Value.Standing != null){
                if(elem.Value.Standing.GetComponent<Human>() != null) {
                    tempHumanBiome1Count++;
                } else if(elem.Value.Standing.GetComponent<Mycelium>() != null) {
                    tempMycBiome1Count++;
                }
            } else if (elem.Value.SpecialClassifier == 2 && elem.Value.Standing != null) {
                if(elem.Value.Standing.GetComponent<Human>() != null) {
                    tempHumanBiome2Count++;
                } else if(elem.Value.Standing.GetComponent<Mycelium>() != null) {
                    tempMycBiome2Count++;
                }
            }
        }

        _HumanCountBiome1 = tempHumanBiome1Count;
        _HumanCountBiome2 = tempHumanBiome2Count;
        _MyceliumCountBiome1 = tempMycBiome1Count;
        _MyceliumCountBiome2 = tempMycBiome2Count;
    }

    public void addMycelium(ref Mycelium newMycelium) {
        _MyceliumGroup[newMycelium.gameObject.GetInstanceID()] = newMycelium;
        _MyceliumCount++;
        return;
    }

    public void removeMycelium(int instanceId) {
        _MyceliumGroup.Remove(instanceId);
        _MyceliumCount--;
        return;
    }

    public void addHuman(ref Human newHuman) {
        _HumanGroup[newHuman.gameObject.GetInstanceID()] = newHuman;
        _HumanCount++;
        return;
    }

    public void removeHuman(int instanceId) {
        _HumanGroup.Remove(instanceId);
        _HumanCount--;
        return;
    }

    public void addSettlement(ref Settlement newSettlement) {
        _SettlementGroup[newSettlement.gameObject.GetInstanceID()] = newSettlement;
        _SettlementCount++;
        return;
    }

    public void removeSettlement(int instanceId) {
        _SettlementGroup.Remove(instanceId);
        _SettlementCount--;
        return;
    }
}
