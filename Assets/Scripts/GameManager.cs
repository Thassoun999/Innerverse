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
    private List<string> actions; // Based on Action Points and selections, add actions to list to execute
    private List<List<int>> _TwoDimensionalGridMap; // a 2D representation of isometric map on 2D list.
    private Dictionary<(int, int), GridNode> _CoordstoGridNode;

    private Dictionary<int ,Human> _HumanGroup; // instanceId to ref game object
    private int _HumanCount; //  Comparing this with the grid count and human count helps to know if mycelium wins
    private Dictionary<int, Mycelium> _MyceliumGroup; // instanceId to ref game object
    private int _MyceliumCount; // Comparing this with the grid count and human count helps to know if mycelium wins
    
    private int test;

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

    public int GridAmount
    {
        get {
            return GridAmount;
        }
        set {
            GridAmount = value;
            return;
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

    public List<string> Actions 
    {
        get {
            return actions;
        }
        set {
            actions = value;
            return;
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

        // Spawn our first Mycelium and make sure to add to necessary groups
        int referenceID;
        GameObject temp;

        (referenceID, temp) = SpawnManager.Instance.Spawn(1, 1, "Myc");
        _MyceliumGroup[referenceID] = GetComponent(typeof(Mycelium)) as Mycelium;
        _MyceliumCount++;
    }

    // Update is called once per frame
    void Update()
    {
        // WIN-LOSE Conditions!

        // CHECK 1 -- Check to see if one side has a count of 0, the other side wins

        // CHECK 2 -- Check to see if >75% of the grid map is covered (is occupied) -- Winner is whoever has higher count

        // CHECK 3 -- Check to see if all special biomes are controlled by either Mycelium or Human -- Winner is decided there


    }

    void advanceTurn() {
        // Perform selected actions based on whose turn it is
        
        // Mycelium actions
        if (isPlayerTurn) {
            // ...
        } else { // Human actions
            // ...
        }
        


        /*
        Have this occur in a coroutine
        isPlayerTurn = !isPlayerTurn; // switch our boolean
        actions.Clear(); // empty our actions list for the next turn group
        */
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
}
