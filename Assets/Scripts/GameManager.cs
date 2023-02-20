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
    private Dictionary<(int, int), GridNode> _CoordstoGridNode;

    public Dictionary<int ,Human> _HumanGroup; // instanceId to ref game object
    public int _HumanCount; //  Comparing this with the grid count and human count helps to know if mycelium wins
    public Dictionary<int, Mycelium> _MyceliumGroup; // instanceId to ref game object
    public int _MyceliumCount; // Comparing this with the grid count and human count helps to know if mycelium wins
    public Dictionary<int, Settlement> _SettlementGroup;
    public int _SettlementCount;
    
    private int maxActionPoionts = 15;
    private int currActionPoints;

    private Mycelium isSelecting; // Can't select multiple Mycelium at once!

    private int _HumanCountBiome1;
    private int _HumanCountBiome2;
    private int _MyceliumCountBiome1;
    private int _MyceliumCountBiome2;

    // (29; 0) Special Biome 2
    // (0; 29) Special Biome 1

    private float _settlementSpawnChance = 0.2f;

    public int[] settlementBuilt = new int[] {0, 0, 0}; // Default, Special 1, Special 2 (if 0 no settlement, if 1 yes settlement)

    private string humanAction;

    [SerializeField] private HumanBTree _EnemyAIManager;

    private bool _GameOver;
    
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

        set {
            _instance = value;
        }
    }

    public string HumanAction {
        get {
            return humanAction;
        }
        set {
            humanAction = value;
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

    public int[] SettlementBuilt{
        get {
            return settlementBuilt;
        }
        set {
            settlementBuilt = value;
        }
    }

    public bool GameOver {
        get {
            return _GameOver;
        }
    }

    // ~ Methods ~

    // Either on AWAKE or on START instantiate a starting number of humans and mycelium!

    void Awake()
    {
        if (_instance != null && _instance != this) { // need to ensure that this remains a singleton!
            Destroy(gameObject);
            return;
        }

        _instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        _CoordstoGridNode = new Dictionary<(int, int), GridNode>();
        GridNode[] grids;

        grids = FindObjectsOfType<GridNode>();

        isPlayerTurn = true; // always starts off as the player's turn (CHANGE THIS!)
        currActionPoints = maxActionPoionts;        
        
        // making the 2d map representation and the grid dictionary
        for (int i = 0; i < grids.Length; i++) {
            int[] coords = grids[i].Coordinates;
            _CoordstoGridNode[(coords[0], coords[1])] = grids[i];
        }

        List<int> rowList = new List<int>(); // temporary row list

        rowList.Sort(); // sort the rows

        // Declare dictionnaries
        _HumanGroup = new Dictionary<int, Human>();
        _MyceliumGroup = new Dictionary<int, Mycelium>();
        _SettlementGroup = new Dictionary<int, Settlement>();

        // Spawn our first Mycelium and make sure to add to necessary groups

        // THIS NEEDS TO BE CHANGED AND HANDLED MORE CLEANLY IN A FUNCTION OR SOMETHING -- DECIDE LATER
        // May not need this

        // Spawn Human very closeby
        SpawnManager.Instance.Spawn(5, 4, "Myc");
        //SpawnManager.Instance.Spawn(26, 26, "Myc");
        SpawnManager.Instance.Spawn(22, 24, "Hum");
        //SpawnManager.Instance.Spawn(7, 4, "Hum");
        SpawnManager.Instance.Spawn(25, 26, "Settlement");

        UIManager.Instance.EndTurnButton(true);
        _GameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Play the Enemy's Turn
        if(!isPlayerTurn && !_GameOver) {
            _EnemyAIManager.Evaluate();
        }
        
        // WIN-LOSE Conditions! Only check these if the game isn't over yet!
        if(!_GameOver) {
            // CHECK 1 -- Check to see if one side has a count of 0, the other side wins
            if(_MyceliumCount == 0) {
                _GameOver = true;
                UIManager.Instance.EndGame("Human");
            } else if(_HumanCount == 0 && _SettlementCount == 0) {
                _GameOver = true;
                UIManager.Instance.EndGame("Mycelium");
            }

            // CHECK 2 -- Check to see if >75% of the grid map is covered by Mycelium
            float ratio = (float)(_MyceliumCount) / (float)(CoordsToGridNode.Count);
            if(ratio >= 0.11) { // Rounds up to 99 or 100 Mycelium on the map
                _GameOver = true;
                UIManager.Instance.EndGame("Mycelium");
            }
        }
    }

    // May be a good idea to make this a coroutine -- deactivate player input and wait for animations to finish!
    public void AdvanceTurn() {
        // Change the turn and set to opposing action points
        currActionPoints = maxActionPoionts;

        // But keep input disabled until all animations are done! 
        // Moreso for human animations since they just seem to end their turn willy nilly

        // Unselect all Mycelium and Reset their action
        foreach(KeyValuePair<int, Mycelium> elem in _MyceliumGroup) {
            if(elem.Value.Selected) {
                elem.Value.Selected = false;
                elem.Value.Reset();
            }
        }

        if(isPlayerTurn) { // End of player turn -- turn off the button
            UIManager.Instance.EndTurnButton(false); 
        }

        // Every Settlement attempts to spawn a human! Only do this at the end of the human turn!
        if (!isPlayerTurn) {            
            // If Settlement spawns a human, reset the chance back to 10%
            foreach(KeyValuePair<int, Settlement> elem in _SettlementGroup) {
                elem.Value.SpawnTime = true; // will spawn on next Settlement update!
            }

            _settlementSpawnChance += 0.20f; // Increase chance of settlement spawning human per turn (by a percent)
        } 

        // go through grids and check to see how many humans and mycelium are on each biome 1 and 2
        CountInSpecialBiome();

        isPlayerTurn =! isPlayerTurn; // THIS NEEDS TO HAPPEN

        return;
    }

    public bool NoHumanMovement() {
        bool nowalking = true;
        foreach (KeyValuePair<int, Human> elem in _HumanGroup) {
            if (elem.Value.MoveActivated == true) {
                nowalking = false;
                break;
            }
        }

        return nowalking;
    }


    private void CountInSpecialBiome() {
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

    public void ResetInstance() {
        foreach(KeyValuePair<int, Human> elem in _HumanGroup) {
            Destroy(elem.Value.gameObject);
            _HumanCount--;
        }
        foreach(KeyValuePair<int, Mycelium> elem in _MyceliumGroup) {
            Destroy(elem.Value.gameObject);
            MyceliumCount--;
        }
        foreach(KeyValuePair<int, Settlement> elem in _SettlementGroup) {
            Destroy(elem.Value.gameObject);
            _SettlementCount--;
        }

        _SettlementGroup.Clear();
        _HumanGroup.Clear();
        _MyceliumGroup.Clear();

        foreach(KeyValuePair<(int, int), GridNode> elem in _CoordstoGridNode) {
            Destroy(elem.Value);
        }

        _CoordstoGridNode.Clear();
    }
}
