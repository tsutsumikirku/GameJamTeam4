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
        if(collision.gameObject.CompareTag("Floor"))
        {
            if(normalCrane != null)
                normalCrane.OnArmEnd();
            if(hanmmer != null)
                hanmmer.OnArmEnd();
            return;
        }

        // Floor 以外との衝突で一定以上の力があればアーム終了
        float otherMass = collision.rigidbody != null ? collision.rigidbody.mass : 0f;
        float selfMass = cachedRb != null ? cachedRb.mass : 0f;
        float combinedMass = selfMass + otherMass;
        if (combinedMass <= 0f)
            combinedMass = 1f;
        float impact = collision.relativeVelocity.magnitude * combinedMass;
        if (impact >= impactThreshold)
        {
            if(normalCrane != null) normalCrane.OnArmEnd();
            if(hanmmer != null) hanmmer.OnArmEnd();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            if(normalCrane != null)                
            normalCrane.OnArmEnd();
            if(hanmmer != null)
                hanmmer.OnArmEnd();
        }
    }
}
