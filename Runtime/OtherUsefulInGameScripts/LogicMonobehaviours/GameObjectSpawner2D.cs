using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameObjectSpawner2D : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnEntry
    {
        public GameObject prefab;
        public float probabilityWeight;
    }

    public Transform spawnParent = null;
    public List<SpawnEntry> objectsToSpawn;
    
    public Vector2 spawnRange;

    public List<GameObject> Spawn(int number)
    {
        List<GameObject> objects = new List<GameObject>();
        for(int i = 0; i < number; ++i)
        {
            GameObject prefab = PickPrefabRandom();
            objects.Add(Instantiate(prefab, PickPositoinRandom(), Quaternion.identity,spawnParent));
        }
        return objects;
    }

    protected GameObject PickPrefabRandom()
    {
        GameObject result = null;
        float pSum = objectsToSpawn.Select((x) => x.probabilityWeight).Sum();
        float randomNumber = Random.Range(0, pSum);

        float currentSum = 0;
        for(int i = 0; i < objectsToSpawn.Count; ++i)
        {
            currentSum += objectsToSpawn[i].probabilityWeight;
            if (currentSum >= randomNumber)
            {
                result = objectsToSpawn[i].prefab;
                break;
            }
        }

        return result;
    }

    protected Vector3 PickPositoinRandom()
    {
        Vector2 offset = new Vector2(Random.Range(-spawnRange.x / 2, spawnRange.x / 2),
            Random.Range(-spawnRange.y / 2, spawnRange.y / 2));

        return transform.position + (Vector3)offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(transform.position, spawnRange);
    }
}
