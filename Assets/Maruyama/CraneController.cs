using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        transform.position += Vector3.right * input * moveSpeed * Time.deltaTime;
    }
}