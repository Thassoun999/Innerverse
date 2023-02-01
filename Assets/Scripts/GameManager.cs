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


    // ~ Methods ~

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
        isPlayerTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void advanceTurn() {
        // Perform selected actions based on whose turn it is
        
        // Mycelium actions
        if (isPlayerTurn) {
            // ...
        } else { // Human actions
            // ...
        }
        
        
        isPlayerTurn = !isPlayerTurn; // switch our boolean
        actions.Clear(); // empty our actions list for the next turn group

    }
}
