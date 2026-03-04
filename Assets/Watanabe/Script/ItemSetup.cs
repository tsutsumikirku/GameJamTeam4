using Unity.VisualScripting;
using UnityEngine;

public class ItemSetup : MonoBehaviour
{
    public SpownData[] spawnSlots;

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        foreach (var slot in spawnSlots)
        {
            int spawnCount =Random.Range(slot.minCount, slot.maxCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject prefab = GetRandomPrefab(slot);
                if (prefab == null)
                    continue;

                Vector3 pos = Vector3.zero;
                bool spawned = false;
                int tryCount = 80;

                while (tryCount > 0)
                {
                    pos = slot.point.position + new Vector3(Random.Range(-slot.rangeX, slot.rangeX), Random.Range(-slot.rangeY,slot.rangeY), 0);
                    if (!IsOverlapping(pos, prefab))
                    {
                        spawned = true;
                        break;
                    }
                    tryCount--;
                }

                if (spawned)
                {
                    Instantiate(prefab, pos, Quaternion.identity);
                }
            }     
        }
    }

    GameObject GetRandomPrefab(SpownData slot)
    {
        int totalWeight = 0;
        foreach (var item in slot.items)
            totalWeight += item.rate;

        int rand = Random.Range(0,totalWeight );
        int current = 0;
        foreach (var item in slot.items)
        {
            current += item.rate;

            if (rand < current)
                return item.prefab;
        }
        return slot.items[0].prefab;
    }

    bool IsOverlapping(Vector3 pos, GameObject prefab)
    {
        Collider2D col = prefab.GetComponent<Collider2D>();
        if (col == null) return false;

        Vector2 size = col.bounds.size;

        Collider2D hit = Physics2D.OverlapBox(
            pos,
            size, 
            0
        );

        return hit != null;
    }
}
