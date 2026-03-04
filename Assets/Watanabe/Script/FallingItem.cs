using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public SpawnItemData[] items;
    public Transform spawnPoint;
    [Header("発生時間")]
    public float[] spawnTimes;
    [Header("X範囲")]
    public float rangeX = 5f;
    public float rangeY = 3f;
    float timer;
    int currentTimeIndex = 0;
    int initialCount;
    bool spawned = false;

    private void Start()
    {
        Invoke("SetCount", 0.5f);
    }

    private void SetCount()
    {
        initialCount = GameObject.FindGameObjectsWithTag("Item").Length;
        Debug.Log("初期配置数：" + initialCount);
    }

    private void Update()
    {
        if (spawned) return;
        if (spawnTimes == null  || spawnTimes.Length ==0) return;
        timer += Time.deltaTime;
        if (currentTimeIndex < spawnTimes.Length &&
            timer >= spawnTimes[currentTimeIndex])
        {
            SpawnItems();
            currentTimeIndex++;

            if (currentTimeIndex >= spawnTimes.Length)
                spawned = true;
        }
    }

    void SpawnItems()
    {
        int currentCount = GameObject.FindGameObjectsWithTag("Item").Length;
        int needSpawn = initialCount - currentCount;
        Debug.Log("現在数：" + currentCount);
        Debug.Log("不足数：" + needSpawn);
        if (needSpawn <= 0) return;
        for (int i = 0; i <needSpawn; i++)
        {
            GameObject prefab = GetRandomPrefab();
            if (prefab == null) continue;

            Vector3 pos;
            int tryCount = 80;
            while (tryCount > 0)
            {
                pos = spawnPoint.position +
                      new Vector3(
                          Random.Range(-rangeX, rangeX),
                          Random.Range(-rangeY, rangeY),
                          0
                      );

                if (!IsOverlapping(pos, prefab))
                {
                    Instantiate(prefab, pos, Quaternion.identity);
                    break;
                }

                tryCount--;
            }
        }
    }

    GameObject GetRandomPrefab()
    {
        //アイテムのrate数を全部足して最大値を計算
        float total = 0;
        foreach (var item in items)
            total += item.rate;

        //0～rateの合計の中からランダムに値を決める
        float rand = Random.Range(0, total);
        float current = 0;

        //アイテムのrateを足していき最初にrandの数値になったらそれを返す
        foreach (var item in items)
        {
            current += item.rate;

            if (rand <= current)
                return item.prefab;
        }
        //rateを足していっても最大値にならない場合　配列の0番目を選択
        return items[0].prefab;
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
