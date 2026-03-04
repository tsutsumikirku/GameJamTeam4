using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Hanmmer : MonoBehaviour, IClaneArm
{
    [SerializeField] 
    private Rigidbody2D rb;
    [SerializeField] 
    private float power = 5f; // ハンマーの下降速度
    [SerializeField]
    private float hanmmerSpeed = 5f; // ハンマーの下降スピード
    [SerializeField] 
    private float hannmerUpSpeed = 5f; // ハンマーの上昇スピード
    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }
    private Vector2 startPosition;
    private float startAngle = 0f; // ハンマーの回転角度
    [SerializeField]
    private float maxHannmerAngle = 90f; // ハンマーの最大回転角度
    [SerializeField] private float knockbackForce = 10f; // 吹っ飛ばす力
    [SerializeField] private float knockbackUpForce = 2f; // 上向きの付加力
    [SerializeField] private LayerMask hittableLayers = ~0; // ヒット判定するレイヤー（全部ならデフォルト）
    private bool isArmClose = false; // アームが閉じているかどうかのフラグ
    void Start()
    {
        startPosition = transform.localPosition; // クレーンの開始位置を保存
        startAngle = transform.localEulerAngles.z; // ハンマーの開始角度を保存
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }
    public void OnArmEnd()
    {
        isArmClose = false;
    }

    public void OnArmRelease()
    {
        OnArmReleaseAsync().Forget();
    }
    private async UniTask OnArmReleaseAsync()
    {
        await UniTask.Yield(); // 次のフレームまで待機
        OnArmReleaseEnd?.Invoke();
    }

    public void OnArmStart()
    {
        OnArmStartAsync().Forget();
    }
    private async UniTask OnArmStartAsync()
    {
        isArmClose = true;
        while (isArmClose)
        {
            transform.Translate(Vector2.down * hanmmerSpeed * Time.deltaTime); // ハンマーを下に動かす
            await UniTask.Yield(); // 次のフレームまで待機
        }

        // DOTween で回転（パワーを使って早くなるで線形に最大角度まで）
        await rb.transform.DOLocalRotate(new Vector3(0f, 0f, startAngle + maxHannmerAngle), 1f / power)
            .SetEase(Ease.Flash)// 緩急をつける
            .AsyncWaitForCompletion();

        await UniTask.Delay(500); // 停止待ち

        // 元の角度に戻す（1秒で線形）
        await rb.transform.DOLocalRotate(new Vector3(0f, 0f, startAngle), 1f)
            .SetEase(Ease.Linear)
            .AsyncWaitForCompletion();

        await transform.DOLocalMove(startPosition, Vector2.Distance(transform.localPosition, startPosition) / hannmerUpSpeed).SetEase(Ease.InOutSine).AsyncWaitForCompletion(); // ハンマーを開始位置に戻す
        OnArmActionEnd?.Invoke(); // 終了通知
    }
}
