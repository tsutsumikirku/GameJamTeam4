using UnityEngine;

public enum CraneType
{
    Standard,
    Speed,
    HammerCrusher,
    BombDropper
}

public class CraneController : MonoBehaviour
{
    [Header("‘€ìÝ’è")]
    [SerializeField] KeyCode moveLeft = KeyCode.A;
    [SerializeField] KeyCode moveRight = KeyCode.D;

    [Header("ƒNƒŒ[ƒ“Ý’è")]
    [SerializeField] CraneType craneType;
    [SerializeField] float moveSpeed = 5f;

    [Header("Œ©‚½–Ú")]
    [SerializeField] GameObject standardVisual;
    [SerializeField] GameObject speedVisual;
    [SerializeField] GameObject hammerVisual;
    [SerializeField] GameObject bombVisual;

    bool canControl = false;

    public CraneType CraneType
    {
        get => craneType;
        set
        {
            craneType = value;
            ApplyType();
        }
    }

    public void StartControl()
    {
        canControl = true;
    }

    public void StopControl()
    {
        canControl = false;
    }

    void Update()
    {
        if (!canControl) return;

        float input = 0f;
        if (Input.GetKey(moveLeft)) input = -1f;
        if (Input.GetKey(moveRight)) input = 1f;

        transform.position += Vector3.right * input * moveSpeed * Time.deltaTime;
    }

    void ApplyType()
    {
        standardVisual.SetActive(craneType == CraneType.Standard);
        speedVisual.SetActive(craneType == CraneType.Speed);
        hammerVisual.SetActive(craneType == CraneType.HammerCrusher);
        bombVisual.SetActive(craneType == CraneType.BombDropper);

        switch (craneType)
        {
            case CraneType.Standard: moveSpeed = 5f; break;
            case CraneType.Speed: moveSpeed = 8f; break;
            case CraneType.HammerCrusher: moveSpeed = 3f; break;
            case CraneType.BombDropper: moveSpeed = 4f; break;
        }
    }
}