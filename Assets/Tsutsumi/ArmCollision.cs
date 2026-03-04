using UnityEngine;

public class ArmCollision : MonoBehaviour
{
    [SerializeField] private NormalCrane normalCrane;
    [SerializeField] private Hanmmer hanmmer;

    [Header("床判定")]
    [SerializeField] private string floorTag = "Floor";
    [SerializeField] private LayerMask floorLayers;

    [Header("ハンマー衝撃停止")]
    [SerializeField] private float impactThreshold = 10f;

    private Rigidbody2D cachedRb;

    private void Awake()
    {
        cachedRb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsFloor(collision.transform))
        {
            StopArms();
            return;
        }

        // 通常クレーンはアイテム接触で止めない
        if (hanmmer == null)
        {
            return;
        }

        float selfMass = cachedRb != null ? cachedRb.mass : 0f;
        float otherMass = collision.rigidbody != null ? collision.rigidbody.mass : 0f;
        float mass = Mathf.Max(1f, selfMass + otherMass);
        float impact = collision.relativeVelocity.magnitude * mass;

        if (impact >= impactThreshold)
        {
            hanmmer.OnArmEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsFloor(collision.transform))
        {
            StopArms();
        }
    }

    private void StopArms()
    {
        if (normalCrane != null)
        {
            normalCrane.OnArmEnd();
        }

        if (hanmmer != null)
        {
            hanmmer.OnArmEnd();
        }
    }

    private bool IsFloor(Transform hit)
    {
        Transform current = hit;
        while (current != null)
        {
            GameObject go = current.gameObject;

            bool tagMatch = !string.IsNullOrEmpty(floorTag) && go.tag == floorTag;
            bool layerMatch = (floorLayers.value & (1 << go.layer)) != 0;

            if (tagMatch || layerMatch)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
