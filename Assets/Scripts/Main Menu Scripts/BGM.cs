using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM _instance;

    public static BGM Instance {
        get {
            if (_instance is null)
                Debug.LogError("Game Manager is NULL");
            
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this) { // need to ensure that this remains a singleton!
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
