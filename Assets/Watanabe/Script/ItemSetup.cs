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
            int spawnCount =
                Random.Range(slot.minCount, slot.maxCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject prefab = GetRandomPrefab(slot);

                if (prefab == null)
                    continue;

                Vector3 pos = slot.point.position;

                float randomX =
                    Random.Range(-slot.rangeX, slot.rangeX);

                pos.x += randomX;

                Instantiate(
                    prefab,
                    pos,
                    Quaternion.identity
                );
            }
        }
    }

    GameObject GetRandomPrefab(SpownData slot)
    {
        int totalWeight = 0;

        foreach (var item in slot.items)
            totalWeight += item.Rate;

        int rand = Random.Range(0,totalWeight );

        int current = 0;

        foreach (var item in slot.items)
        {
            current += item.Rate;

            if (rand < current)
                return item.prefab;
        }

        return slot.items[0].prefab;
    }
}
