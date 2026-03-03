using UnityEngine;

public class CraneTest : MonoBehaviour
{
    [SerializeField] CraneController playerOneCrane;
    [SerializeField] CraneType craneType;
    private void Start()
    {
        playerOneCrane.CraneType = craneType;
        playerOneCrane.StartControl();
    }
}

