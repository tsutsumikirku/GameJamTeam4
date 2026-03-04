using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    [Header("操作設定")]
    [SerializeField] KeyCode actionKey = KeyCode.Space;
    [SerializeField] Vector3 moveDirection = Vector3.right;

    [Header("クレーン設定")]
    [SerializeField] float moveSpeed = 4f;

    [Header("爆弾設定")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform dropPoint;       // 爆弾を落とす位置
    [SerializeField] float fuseTime = 1.5f;     // 爆発までの秒数
    [SerializeField] float explosionForce = 15f;
    [SerializeField] float explosionRadius = 3f;

    Rigidbody2D rb;
    Vector3 startPosition;
    BombState state = BombState.Idle;
    bool canControl = false;
    bool isPaused = false;

    GameObject currentBomb;
    float fuseTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;
        StartControl();
    }

    public void StartControl()
    {
        canControl = true;
        state = BombState.Moving;
    }

    public void StopControl()
    {
        canControl = false;
        state = BombState.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;

    void Update()
    {
        if (!canControl || isPaused) return;

        if (state == BombState.Moving && Input.GetKeyUp(actionKey))
        {
            DropBomb();
            state = BombState.Waiting;
            fuseTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (!canControl || isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (state)
        {
            case BombState.Moving:
                rb.linearVelocity = Input.GetKey(actionKey)
                    ? (Vector2)(moveDirection * moveSpeed)
                    : Vector2.zero;
                break;

            case BombState.Waiting:
                rb.linearVelocity = Vector2.zero;
                fuseTimer += Time.fixedDeltaTime;

                if (fuseTimer >= fuseTime)
                {
                    Explode();
                    state = BombState.Returning;
                }
                break;

            case BombState.Returning:
                Vector3 target = new Vector3(startPosition.x, transform.position.y, 0f);
                Vector2 dir = (target - transform.position).normalized;
                rb.linearVelocity = dir * moveSpeed;

                if (Vector2.Distance(transform.position, target) < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    state = BombState.Moving;
                }
                break;
        }
    }

    void DropBomb()
    {
        if (bombPrefab == null || dropPoint == null) return;

        currentBomb = Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);
        Debug.Log("[Bomb] 爆弾投下！");
    }

    /// <summary>
    /// 爆弾を爆発させ、周囲のオブジェクトを吹き飛ばす。
    /// Unity2Dには AddExplosionForce がないため、手動で放射状の力を計算している。
    /// </summary>
    void Explode()
    {
        if (currentBomb == null) return;

        Vector2 center = currentBomb.transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Rigidbody2D>(out var targetRb) && !targetRb.isKinematic)
            {
                Vector2 dir = (targetRb.position - center).normalized;

                // 上向き成分を強制的に加える（爆発は上にも飛ばす）
                dir = (dir + Vector2.up * 0.5f).normalized;

                float dist = Vector2.Distance(targetRb.position, center);
                float falloff = 1f - (dist / explosionRadius);

                targetRb.AddForce(dir * explosionForce * falloff, ForceMode2D.Impulse);
            }
        }

        Debug.Log($"<color=red>[Bomb] 爆発！ {hits.Length}個に影響</color>");
        Destroy(currentBomb);
        currentBomb = null;
    }

    void OnDrawGizmos()
    {
        // 爆弾が存在するならその位置に表示
        if (currentBomb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentBomb.transform.position, explosionRadius);
        }
        // なければ投下位置に表示
        else if (dropPoint != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(dropPoint.position, explosionRadius);
        }
    }
}

public enum BombState
{
    Idle,
    Moving,
    Waiting,   // 爆弾落下中〜爆発待ち
    Returning
}