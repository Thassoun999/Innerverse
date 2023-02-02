using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    [SerializeField] GameObject _MycSpawn;
    [SerializeField] GameObject _HumSpawn;

    public static SpawnManager Instance
    {
        get 
        {
            if(_instance is null)
                Debug.LogError("Spawn Manager is NULL");

            return _instance;
        }
    }

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

        // Spawning
        if (spawnSpecification == "Myc")
            Object.Instantiate(_MycSpawn, position, transform.rotation);
        else if (spawnSpecification == "Hum")
            Object.Instantiate(_HumSpawn, position, transform.rotation);
        else    
            Debug.LogError("Spawn Error: Improper spawn specification type");
    }

    public void DeSpawn(int instanceId, ref Dictionary<int, GameObject> group) {
        Object.Destroy(group[instanceId], 0.05f);
    }
    
}
