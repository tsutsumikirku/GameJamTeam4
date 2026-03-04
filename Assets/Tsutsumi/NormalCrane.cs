using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    [SerializeField]HingeJoint2D rightHingeJoint2D;
    [SerializeField]HingeJoint2D leftHingeJoint2D;
    [SerializeField] float armPower = 100f; // アームの回転力 
    [SerializeField] float armDownSpeed = 5f; // アームの下降速度   
    [SerializeField] float armUpSpeed = 5f; // アームの上昇速度
    [SerializeField] float armRotationTime = 1f; // アームの回転時間
    private Vector2 startPosition; // クレーンの開始位置
    public Action OnArmActionEnd { get => onArmActionEnd; set => onArmActionEnd = value; }
    public Action OnArmReleaseEnd { get => onArmReleaseEnd; set => onArmReleaseEnd = value; }
    private Action onArmActionEnd;
    private Action onArmReleaseEnd;
    private Rigidbody2D rb;
    private Vector2 rightArmInitialPos;
    private Vector2 leftArmInitialPos;
    private bool isArmClose = false; // アームが開いているかどうかのフラグ
    private bool isArmClose2 = false;
    void Start()
    {
        startPosition = transform.localPosition; // クレーンの開始位置を保存
        rb = GetComponent<Rigidbody2D>(); 
        rightArmInitialPos = rightHingeJoint2D.transform.localPosition; // 右アームの初期位置を保存
        leftArmInitialPos = leftHingeJoint2D.transform.localPosition; // 左アームの初期位置を保存
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
        await OpenArm();
        await CloseArm();
        OnArmReleaseEnd?.Invoke();
    }   

    public void OnArmStart()
    {
        Debug.Log("OnArmStart");
        OnArmStartAsync().Forget();
    }
    private async UniTask OnArmStartAsync()
    {
        await OpenArm();
        isArmClose = true;
        while (isArmClose)
        {
            transform.Translate(Vector2.down * armDownSpeed * Time.deltaTime); // アームを下に動かす（速度は秒あたりのベロシティ）
            await UniTask.Yield();
        }
        rb.linearVelocity = Vector2.zero; // アームの動きを止める
        await CloseArm();
        await transform.DOLocalMove(startPosition, Vector2.Distance(transform.localPosition, startPosition) / armUpSpeed).SetEase(Ease.InOutSine).AsyncWaitForCompletion(); // クレーンを開始位置に戻す（速度は秒あたりのベロシティ）
        
        rb.linearVelocity = Vector2.zero; // クレーンの動きを止める
        OnArmActionEnd?.Invoke();
    }
    private async UniTask OpenArm()
    {
        isArmClose2 = false;
        rightHingeJoint2D.useMotor = true;
        leftHingeJoint2D.useMotor = true;
        var tweener = DOTween.To(() => rightHingeJoint2D.motor.motorSpeed, x =>
        {
            var r = rightHingeJoint2D.motor;
            r.motorSpeed = x;
            rightHingeJoint2D.motor = r;
            var l = leftHingeJoint2D.motor;
            l.motorSpeed = -x;
            leftHingeJoint2D.motor = l;
        }, armPower, armRotationTime).SetEase(Ease.Linear);
        await tweener.AsyncWaitForCompletion();
    } 
    private async UniTask CloseArm()
    {
        rightHingeJoint2D.useMotor = true;
        leftHingeJoint2D.useMotor = true;
        var tweener = DOTween.To(() => rightHingeJoint2D.motor.motorSpeed, x =>
        {
            var r = rightHingeJoint2D.motor;
            r.motorSpeed = x;
            rightHingeJoint2D.motor = r;
            var l = leftHingeJoint2D.motor;
            l.motorSpeed = -x;
            leftHingeJoint2D.motor = l;
        }, -armPower, armRotationTime).SetEase(Ease.Linear);
        await tweener.AsyncWaitForCompletion();
        ResetArm().Forget();
    }
    private async UniTask ResetArm()
    {
        isArmClose2 = true;
        while(isArmClose2)
        {
            rightHingeJoint2D.transform.localPosition = Vector2.Lerp(rightHingeJoint2D.transform.localPosition, rightArmInitialPos, Time.deltaTime * armRotationTime);
            leftHingeJoint2D.transform.localPosition = Vector2.Lerp(leftHingeJoint2D.transform.localPosition, leftArmInitialPos, Time.deltaTime * armRotationTime);
            await UniTask.Yield();
        }
    }

}
