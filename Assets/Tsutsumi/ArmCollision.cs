using UnityEngine;

public class ArmCollision : MonoBehaviour
{
    [SerializeField] NormalCrane normalCrane;
    [SerializeField] Hanmmer hanmmer;
    [SerializeField] private float impactThreshold = 10f; // これ以上の衝撃で OnArmEnd を呼ぶ

    [Header("床判定")]
    [SerializeField] private string floorTag = "Floor";
    [SerializeField] private string floorLayerName = "Floor";

    private int floorLayer = -1;
    private Rigidbody2D cachedRb;

    void Awake()
    {
        cachedRb = GetComponentInParent<Rigidbody2D>();
        floorLayer = LayerMask.NameToLayer(floorLayerName);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsFloor(collision.transform))
        {
            StopArms();
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
        if (IsFloor(collision.transform))
        {
            StopArms();
        }
    }

    private void StopArms()
    {
        if (normalCrane != null)
            normalCrane.OnArmEnd();
        if (hanmmer != null)
            hanmmer.OnArmEnd();
    }

    private bool IsFloor(Transform hitTransform)
    {
        Transform current = hitTransform;
        while (current != null)
        {
            GameObject go = current.gameObject;

            if (go.tag == floorTag)
                return true;

            if (floorLayer >= 0 && go.layer == floorLayer)
                return true;

            current = current.parent;
        }

        return false;
    }
}
