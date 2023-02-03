using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnManager : MonoBehaviour
{
    // ~ Instances, Variables, and Prefabs ~
    private static SpawnManager _instance;
    [SerializeField] GameObject _MycSpawn;
    [SerializeField] GameObject _HumSpawn;


    // ~ Properties ~
    public static SpawnManager Instance
    {
        get 
        {
            if(_instance is null)
                Debug.LogError("Spawn Manager is NULL");

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

    public (int, GameObject) Spawn(int row, int col, string spawnSpecification) {
        float rowFloat = (float)row;
        float colFloat = (float)col;
        Vector3 position = new Vector3(rowFloat, 1.0f, colFloat);
        GameObject temp;
        int instanceID;

        // Spawning
        if (spawnSpecification == "Myc"){
            temp = GameObject.Instantiate(_MycSpawn, position, transform.rotation);
            instanceID = temp.GetInstanceID();

            Mycelium tempMyc = temp.GetComponent(typeof(Mycelium)) as Mycelium;
            GameManager.Instance.addMycelium(ref tempMyc);
        
            return (instanceID, temp);
        }
        else if (spawnSpecification == "Hum"){
            temp = GameObject.Instantiate(_HumSpawn, position, transform.rotation);
            instanceID = temp.GetInstanceID();
            
            Human tempHum = temp.GetComponent(typeof(Human)) as Human;
            GameManager.Instance.addHuman(ref tempHum);

            return (instanceID, temp);
        } else {    
            Debug.LogError("Spawn Error: Improper spawn specification type");
            return (-1, null);
        }

    }
    
}
