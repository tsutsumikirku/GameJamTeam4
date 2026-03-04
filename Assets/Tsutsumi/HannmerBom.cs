using UnityEngine;

public class HannmerBom : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f; // ノックバックの力
    [SerializeField] private float knockbackUpForce = 2f; 
    [SerializeField] private Vector2 flyDirection = Vector2.right; // 飛ぶ方向
    public bool IsActive = false; // ハンマーボムが有効かどうか
    void OnTriggerStay2D(Collider2D collision)
    {
        if(!IsActive)return;
        if(collision.gameObject.tag == "Item")
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDirection = flyDirection.normalized;
                Vector2 knockback = knockbackDirection * knockbackForce * knockbackUpForce;
                rb.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
    }
}
