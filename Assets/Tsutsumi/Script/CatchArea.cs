using UnityEngine;

public class CatchArea : MonoBehaviour
{
    public bool isCrane = false;
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Item")return;
        if (isCrane)
        {
            // RigidBody2Dを無効化して子オブジェクトにする
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)            
            {
                rb.isKinematic = true;
                collision.transform.SetParent(transform);
            }
        }
        else
        {
            // RigidBody2Dを有効化して親から外す
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
                collision.transform.SetParent(null);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Item")return;
            // RigidBody2Dを有効化して親から外す
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
                collision.transform.SetParent(null);
            }
    }
}
