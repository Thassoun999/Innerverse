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
    [SerializeField] GameObject _Settlement;


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

    public void Spawn(int row, int col, string spawnSpecification) {
        float rowFloat = (float)row;
        float colFloat = (float)col;
        Vector3 position = new Vector3(rowFloat, 1.0f, colFloat);
        GameObject temp;

        // Spawning
        if (spawnSpecification == "Myc"){
            temp = GameObject.Instantiate(_MycSpawn, position, transform.rotation);

            Mycelium tempMyc = temp.GetComponent(typeof(Mycelium)) as Mycelium;
            GameManager.Instance.addMycelium(ref tempMyc);
    
        }
        else if (spawnSpecification == "Hum"){
            temp = GameObject.Instantiate(_HumSpawn, position, transform.rotation);
            
            Human tempHum = temp.GetComponent(typeof(Human)) as Human;
            GameManager.Instance.addHuman(ref tempHum);

        } else if (spawnSpecification == "Settlement") {
            temp = GameObject.Instantiate(_Settlement, position, transform.rotation);
            
            Settlement tempSet = temp.GetComponent(typeof(Settlement)) as Settlement;
            GameManager.Instance.addSettlement(ref tempSet);
        }
        else {    
            Debug.LogError("Spawn Error: Improper spawn specification type");
        }

    }
    
}
