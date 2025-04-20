using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public string poolTag;
    public float spawnRate = 1f;
    public Vector3 spawnArea = Vector3.one;

    private float nextSpawnTime;
    int objectsSpawnedCount=0;
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnObject()
    {
        if (objectsSpawnedCount > 4) return;
        // Calculate random position within spawn area
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        // Spawn object from pool
        ObjectPool.Instance.SpawnFromPool(poolTag, spawnPosition, Quaternion.identity);
        objectsSpawnedCount++;
    }

    // Visualize spawn area in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, spawnArea);
    }
}