using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public SpawnItemData[] items;
    public Transform spawnPoint;
    [Header("スポーン間隔")]
    public float spawnInterval = 0.35f;
    [Header("X範囲")]
    public float rangeX = 5f;
    public float rangeY = 3f;
    float timer;
    int initialCount;
    public AudioSource audioSource;
    public AudioClip spawnSE;

    private void Start()
    {
        Invoke("SetCount", 0.5f);
    }

    private void SetCount()
    {
        initialCount = GameObject.FindGameObjectsWithTag("Item").Length;
        Debug.Log("初期配置数: " + initialCount);
    }

    private void Update()
    {
        if (initialCount <= 0) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOneItemIfNeeded();
        }
    }

    void SpawnOneItemIfNeeded()
    {
        int currentCount = GameObject.FindGameObjectsWithTag("Item").Length;
        int needSpawn = initialCount - currentCount;
        if (needSpawn <= 0) return;

        GameObject prefab = GetRandomPrefab();
        if (prefab == null) return;

        if (audioSource != null && spawnSE != null)
            audioSource.PlayOneShot(spawnSE);

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

    GameObject GetRandomPrefab()
    {
        // アイテムの rate を合計して抽選用の最大値を計算
        float total = 0;
        foreach (var item in items)
            total += item.rate;

        // 0 から rate 合計の範囲でランダム値を取得
        float rand = Random.Range(0, total);
        float current = 0;

        // rate を加算し、rand が入った範囲の prefab を返す
        foreach (var item in items)
        {
            current += item.rate;

            if (rand <= current)
                return item.prefab;
        }
        // 万一どれにも該当しない場合は先頭を返す
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
