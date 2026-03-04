using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    [Header("アーム参照")]
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftArm;

    [Header("移動速度")]
    [SerializeField] private float descendSpeed = 3f;
    [SerializeField] private float ascendSpeed = 4f;

    [Header("アーム角度（ローカルZ）")]
    [SerializeField] private float rightOpenAngle = -20f;
    [SerializeField] private float rightCloseAngle = -3f;
    [SerializeField] private float leftOpenAngle = 20f;
    [SerializeField] private float leftCloseAngle = 3f;

    [Header("アーム回転速度（度 / 秒）")]
    [SerializeField] private float closeAngularSpeed = 180f;
    [SerializeField] private float openAngularSpeed = 180f;

    [Header("演出待機")]
    [SerializeField] private float gripDelaySeconds = 0.15f;

    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }

    private Vector3 startPosition;
    private bool isDescending;
    private Tween rightArmTween;
    private Tween leftArmTween;

    private void Awake()
    {
        startPosition = transform.localPosition;
        AutoAssignArms();
    }

    private void OnDestroy()
    {
        rightArmTween?.Kill();
        leftArmTween?.Kill();
    }

    public void OnArmStart()
    {
        OnArmStartAsync().Forget();
    }

    public void OnArmEnd()
    {
        isDescending = false;
    }

    public void OnArmRelease()
    {
        OnArmReleaseAsync().Forget();
    }

    private async UniTask OnArmStartAsync()
    {
        if (!ValidateArms())
        {
            OnArmActionEnd?.Invoke();
            return;
        }

        startPosition = transform.localPosition;
        isDescending = true;

        await RotateArmsAsync(rightOpenAngle, leftOpenAngle, openAngularSpeed);

        while (isDescending)
        {
            transform.Translate(Vector3.down * descendSpeed * Time.deltaTime, Space.Self);
            await UniTask.Yield();
        }

        await RotateArmsAsync(rightCloseAngle, leftCloseAngle, closeAngularSpeed);

        if (gripDelaySeconds > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(gripDelaySeconds));
        }

        float ascendDuration = Vector3.Distance(transform.localPosition, startPosition) / Mathf.Max(0.01f, ascendSpeed);
        await transform.DOLocalMove(startPosition, ascendDuration)
            .SetEase(Ease.InOutSine)
            .AsyncWaitForCompletion();

        OnArmActionEnd?.Invoke();
    }

    private async UniTask OnArmReleaseAsync()
    {
        if (!ValidateArms())
        {
            OnArmReleaseEnd?.Invoke();
            return;
        }

        await RotateArmsAsync(rightOpenAngle, leftOpenAngle, openAngularSpeed);
        OnArmReleaseEnd?.Invoke();
    }

    private async UniTask RotateArmsAsync(float rightTargetZ, float leftTargetZ, float angularSpeed)
    {
        rightArmTween?.Kill();
        leftArmTween?.Kill();

        float rightDuration = Mathf.Abs(Mathf.DeltaAngle(rightArm.localEulerAngles.z, rightTargetZ)) / Mathf.Max(0.01f, angularSpeed);
        float leftDuration = Mathf.Abs(Mathf.DeltaAngle(leftArm.localEulerAngles.z, leftTargetZ)) / Mathf.Max(0.01f, angularSpeed);

        rightArmTween = rightArm.DOLocalRotate(new Vector3(0f, 0f, rightTargetZ), rightDuration).SetEase(Ease.InOutSine);
        leftArmTween = leftArm.DOLocalRotate(new Vector3(0f, 0f, leftTargetZ), leftDuration).SetEase(Ease.InOutSine);

        await UniTask.WhenAll(
            rightArmTween.AsyncWaitForCompletion().AsUniTask(),
            leftArmTween.AsyncWaitForCompletion().AsUniTask()
        );
    }

    private bool ValidateArms()
    {
        AutoAssignArms();

        if (rightArm != null && leftArm != null)
        {
            return true;
        }

        Debug.LogWarning("NormalCrane: rightArm / leftArm が未設定です。Inspectorで設定してください。", this);
        return false;
    }

    private void AutoAssignArms()
    {
        if (rightArm == null)
        {
            Transform foundRight = transform.Find("rightArm");
            if (foundRight != null)
            {
                rightArm = foundRight;
            }
        }

        if (leftArm == null)
        {
            Transform foundLeft = transform.Find("leftArm");
            if (foundLeft != null)
            {
                leftArm = foundLeft;
            }
        }
    }
}
