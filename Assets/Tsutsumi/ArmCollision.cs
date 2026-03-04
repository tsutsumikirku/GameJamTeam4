using UnityEngine;

public class ArmCollision : MonoBehaviour
{
    [SerializeField] NormalCrane normalCrane;
    [SerializeField] Hanmmer hanmmer;
    [SerializeField] private float impactThreshold = 10f; // これ以上の衝撃で OnArmEnd を呼ぶ
    private Rigidbody2D cachedRb;

    void Awake()
    {
        cachedRb = GetComponentInParent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // 通常クレーンは床に到達した時だけ停止
            if (normalCrane != null)
                normalCrane.OnArmEnd();

            // ハンマーは床接触で停止
            if (hanmmer != null)
                hanmmer.OnArmEnd();
            return;
        }

        // 通常クレーンはアイテム接触では止めない（掴み判定のため）

        // ハンマーのみ、Floor 以外との強衝突で停止
        if (hanmmer == null)
            return;

        float otherMass = collision.rigidbody != null ? collision.rigidbody.mass : 0f;
        float selfMass = cachedRb != null ? cachedRb.mass : 0f;
        float combinedMass = selfMass + otherMass;
        if (combinedMass <= 0f)
            combinedMass = 1f;

        float impact = collision.relativeVelocity.magnitude * combinedMass;
        if (impact >= impactThreshold)
        {
            hanmmer.OnArmEnd();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (normalCrane != null)
                normalCrane.OnArmEnd();
            if (hanmmer != null)
                hanmmer.OnArmEnd();
        }
    }
}
