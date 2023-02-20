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

    }

    public void Spawn(int row, int col, string spawnSpecification) {
        float rowFloat = (float)row;
        float colFloat = (float)col;
        Vector3 position = new Vector3(rowFloat, 1.0f, colFloat);
        GameObject tempPar;

        // In order 
        // Create parent -> Affect child local position

        // Spawning -- Get the child
        if (spawnSpecification == "Myc"){
            tempPar = GameObject.Instantiate(_MycSpawn, _MycSpawn.transform.position, transform.rotation);
            GameObject temp = tempPar.transform.GetChild(0).gameObject;
            temp.transform.localPosition = position;

            Mycelium tempMyc = temp.GetComponent(typeof(Mycelium)) as Mycelium;
            GameManager.Instance.addMycelium(ref tempMyc);
    
        }
        else if (spawnSpecification == "Hum"){
            tempPar = GameObject.Instantiate(_HumSpawn, _HumSpawn.transform.position, _HumSpawn.transform.rotation);
            GameObject temp = tempPar.transform.GetChild(0).gameObject;
            temp.transform.localPosition = position;
            
            Human tempHum = temp.GetComponent(typeof(Human)) as Human;
            GameManager.Instance.addHuman(ref tempHum);

        } else if (spawnSpecification == "Settlement") {
            tempPar = GameObject.Instantiate(_Settlement, _Settlement.transform.position, transform.rotation);
            GameObject temp = tempPar.transform.GetChild(0).gameObject;
            temp.transform.localPosition = position;
            
            Settlement tempSet = temp.GetComponent(typeof(Settlement)) as Settlement;
            GameManager.Instance.addSettlement(ref tempSet);
        }
        else {    
            Debug.LogError("Spawn Error: Improper spawn specification type");
        }

    }
    
}
