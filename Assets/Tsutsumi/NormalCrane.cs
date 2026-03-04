using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    [Header("左右のアーム")]
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftArm;

    [Header("下降/上昇速度")]
    [SerializeField] private float descendSpeed = 2.5f;
    [SerializeField] private float ascendSpeed = 3.5f;
    [SerializeField] private float maxDescendDistance = 4f;

    [Header("アームの角度設定")]
    [SerializeField] private float leftOpenAngle = 35f;
    [SerializeField] private float leftCloseAngle = 0f;
    [SerializeField] private float rightOpenAngle = -35f;
    [SerializeField] private float rightCloseAngle = 0f;

    [Header("アーム回転速度")]
    [SerializeField] private float armOpenSpeed = 220f;
    [SerializeField] private float armCloseSpeed = 260f;

    [Header("キャッチ後の待機時間")]
    [SerializeField] private float catchWaitSeconds = 0.2f;

    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }

    private Vector3 startLocalPosition;
    private bool isArmActionRunning;
    private bool shouldEndArmAction;

    void Awake()
    {
        startLocalPosition = transform.localPosition;
    }

    public void OnArmStart()
    {
        if (isArmActionRunning)
        {
            return;
        }

        isArmActionRunning = true;
        shouldEndArmAction = false;
        ArmStartAsync().Forget();
    }

    public void OnArmEnd()
    {
        if (!isArmActionRunning)
        {
            return;
        }

        shouldEndArmAction = true;
    }

    public void OnArmRelease()
    {
        ArmReleaseAsync().Forget();
    }

    private async UniTask ArmStartAsync()
    {
        OpenArmsInstant();

        while (!shouldEndArmAction)
        {
            transform.Translate(Vector3.down * descendSpeed * Time.deltaTime, Space.Self);

            float descendedDistance = Mathf.Abs(transform.localPosition.y - startLocalPosition.y);
            if (descendedDistance >= maxDescendDistance)
            {
                shouldEndArmAction = true;
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        isArmActionRunning = false;

        await CloseArmsAsync();

        if (catchWaitSeconds > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(catchWaitSeconds));
        }

        float ascendDuration = Vector3.Distance(transform.localPosition, startLocalPosition) / Mathf.Max(ascendSpeed, 0.01f);
        await transform.DOLocalMove(startLocalPosition, ascendDuration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Fixed)
            .AsyncWaitForCompletion();

        OnArmActionEnd?.Invoke();
    }

    private async UniTask ArmReleaseAsync()
    {
        await OpenArmsAsync();
        await UniTask.Yield();
        OnArmReleaseEnd?.Invoke();
    }

    private void OpenArmsInstant()
    {
        if (leftArm != null)
        {
            leftArm.DOKill();
            leftArm.localRotation = Quaternion.Euler(0f, 0f, leftOpenAngle);
        }

        if (rightArm != null)
        {
            rightArm.DOKill();
            rightArm.localRotation = Quaternion.Euler(0f, 0f, rightOpenAngle);
        }
    }

    private UniTask OpenArmsAsync()
    {
        float duration = Mathf.Abs(leftOpenAngle - leftCloseAngle) / Mathf.Max(armOpenSpeed, 0.01f);
        return RotateArmsAsync(leftOpenAngle, rightOpenAngle, duration);
    }

    private UniTask CloseArmsAsync()
    {
        float duration = Mathf.Abs(leftCloseAngle - leftOpenAngle) / Mathf.Max(armCloseSpeed, 0.01f);
        return RotateArmsAsync(leftCloseAngle, rightCloseAngle, duration);
    }

    private async UniTask RotateArmsAsync(float leftAngle, float rightAngle, float duration)
    {
        Tween leftTween = null;
        Tween rightTween = null;

        if (leftArm != null)
        {
            leftArm.DOKill();
            leftTween = leftArm.DOLocalRotate(new Vector3(0f, 0f, leftAngle), duration)
                .SetEase(Ease.OutSine)
                .SetUpdate(UpdateType.Fixed);
        }

        if (rightArm != null)
        {
            rightArm.DOKill();
            rightTween = rightArm.DOLocalRotate(new Vector3(0f, 0f, rightAngle), duration)
                .SetEase(Ease.OutSine)
                .SetUpdate(UpdateType.Fixed);
        }

        if (leftTween != null)
        {
            await leftTween.AsyncWaitForCompletion();
        }

        if (rightTween != null)
        {
            await rightTween.AsyncWaitForCompletion();
        }
    }
}
