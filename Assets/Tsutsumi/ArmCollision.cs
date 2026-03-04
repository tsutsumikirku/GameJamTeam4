using UnityEngine;

public class ArmCollision : MonoBehaviour
{
    [SerializeField] NormalCrane normalCrane;
    [SerializeField] Hanmmer hanmmer;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            if(normalCrane != null)
                normalCrane.OnArmEnd();
            if(hanmmer != null)
                hanmmer.OnArmEnd();
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
