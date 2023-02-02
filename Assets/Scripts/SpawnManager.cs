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

    private Dictionary<int, GameObject> _MycGameObjDict;
    private Dictionary<int, GameObject> _HumGameObjDict;

    private List<GameObject> kust;


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

    void Start()
    {
        _MycGameObjDict = new Dictionary<int, GameObject>();
        _HumGameObjDict = new Dictionary<int, GameObject>();
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
            _MycGameObjDict.Add(instanceID, temp);
        
            return (instanceID, temp);
        }
        else if (spawnSpecification == "Hum"){
            temp = GameObject.Instantiate(_HumSpawn, position, transform.rotation);
            instanceID = temp.GetInstanceID();
            _HumGameObjDict.Add(instanceID, temp);

            return (instanceID, temp);
        } else {    
            Debug.LogError("Spawn Error: Improper spawn specification type");
            return (-1, null);
        }

    }

    public void DeSpawn(int instanceId, string despawnSpecification) {

        if (despawnSpecification == "Myc") {
            GameObject.Destroy(_MycGameObjDict[instanceId], 0.05f);
            _MycGameObjDict.Remove(instanceId);
        } else {
            GameObject.Destroy(_HumGameObjDict[instanceId], 0.05f);
            _HumGameObjDict.Remove(instanceId);
        }
    }
    
}
