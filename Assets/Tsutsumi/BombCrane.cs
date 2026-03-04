using System;
using UnityEngine;

public class BombCrane : MonoBehaviour, IClaneArm
{
    public Action OnArmActionEnd { get => onArmActionEnd; set => onArmActionEnd = value; }
    private Action onArmActionEnd;
    public Action OnArmReleaseEnd { get => onArmReleaseEnd; set => onArmReleaseEnd = value; }
    private Action onArmReleaseEnd;

    [Header("爆弾設定")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform dropPoint;       // 爆弾を落とす位置
    [SerializeField] float fuseTime = 1.5f;     // 爆発までの秒数
    [SerializeField] float explosionForce = 15f;
    [SerializeField] float explosionRadius = 3f;

    void Start()
    {
    }

    public void OnArmEnd()
    {
    }

    public void OnArmRelease()
    {
    }

    public void OnArmStart()
    {
    }
}
