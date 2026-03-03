using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public SpawnItemData[] item;
    public float spawnTime = 120f;
    public int minSpawnCount = 5;
    public int maxSpawnCount = 10;
    float timer;

    private void Update()
    {
        
        timer += Time.deltaTime;
        if(timer >= spawnTime)
        {

        }
    }
}
