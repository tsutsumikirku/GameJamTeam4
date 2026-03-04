using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] private NormalCrane normalCrane;
    void OnTriggerEnter2D(Collider2D collision)
    {                
            normalCrane.OnArmEnd();
    }
}
