using UnityEditor.Callbacks;
using UnityEngine;

public class HannmerBom : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f; // ノックバックの力
    [SerializeField] private float knockbackUpForce = 2f; // 上向
    [SerializeField] private LayerMask hittableLayers = ~0;
    private void OnCollisionStay2D(Collision2D collision)
    {
        var otherRb = collision.rigidbody ?? collision.collider.GetComponent<Rigidbody2D>();
        if (otherRb == null) return;
        // ハンマーから相手への方向を算出して力を加える
        Vector2 dir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
        Vector2 force = dir * knockbackForce + Vector2.up * knockbackUpForce;
        otherRb.AddForce(force, ForceMode2D.Impulse);
    }
}
