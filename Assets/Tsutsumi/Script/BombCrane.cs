using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BombCrane : MonoBehaviour, IClaneArm
{
    public Action OnArmActionEnd { get => onArmActionEnd; set => onArmActionEnd = value; }
    private Action onArmActionEnd;
    public Action OnArmReleaseEnd { get => onArmReleaseEnd; set => onArmReleaseEnd = value; }
    private Action onArmReleaseEnd;

    [Header("爆弾設定")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float fuseTime = 1.5f;
    [SerializeField] private float explosionForce = 50f; // 増強: デフォルト威力を上げる
    [SerializeField] private float explosionRadius = 3f;

    [Header("クレーンのインターバル設定")]
    [SerializeField] private float actionInterval = 3f; // アームアクションのインターバル時間

    private Coroutine bombRoutine;
    private GameObject currentBomb;

    public void OnArmStart()
    {
        if (bombRoutine != null)
        {
            return;
        }

        bombRoutine = StartCoroutine(BombRoutine());
    }

    public void OnArmEnd()
    {
        // ボムクレーンは投下後に処理が完了するため、終了入力は受け流す。
    }

    public void OnArmRelease()
    {
        OnArmReleaseAsync().Forget();
    }
    private async UniTask OnArmReleaseAsync()
    {
        await UniTask.Yield(); // 次のフレームまで待機
        await UniTask.WaitForSeconds(actionInterval); // アクションインターバルを待機
        onArmReleaseEnd?.Invoke();
    }

    private IEnumerator BombRoutine()
    {
        DropBomb();

        if (currentBomb != null)
        {
            yield return new WaitForSeconds(fuseTime);
            Explode();
        }

        bombRoutine = null;
        onArmActionEnd?.Invoke();
    }

    private void DropBomb()
    {
        if (bombPrefab == null || dropPoint == null)
        {
            return;
        }

        currentBomb = Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);
    }

    private void Explode()
    {
        if (currentBomb == null)
        {
            return;
        }

        Vector2 center = currentBomb.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRb) || targetRb.isKinematic)
            {
                continue;
            }

            Vector2 direction = (targetRb.position - center).normalized;
            direction = (direction + Vector2.up * 0.8f).normalized; // 上方向の補正を強めに

            float distance = Vector2.Distance(targetRb.position, center);
            float falloff = Mathf.Clamp01(1f - (distance / explosionRadius));
            // エッジでの力をある程度確保しつつ、中心付近は強くする
            float appliedMultiplier = Mathf.Lerp(0.6f, 1f, falloff);
            targetRb.AddForce(direction * explosionForce * appliedMultiplier, ForceMode2D.Impulse);
        }

        Destroy(currentBomb);
        currentBomb = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (dropPoint == null)
        {
            return;
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(dropPoint.position, explosionRadius);
    }
}
