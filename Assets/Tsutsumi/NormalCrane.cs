using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    [Header("アーム参照")]
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftArm;

    [Header("物理")]
    [SerializeField] private Rigidbody2D rb;

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

    [Header("上昇完了許容誤差")]
    [SerializeField] private float ascendStopDistance = 0.01f;

    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }

    private bool isDescending;
    private bool isAscending;
    private Tween rightArmTween;
    private Tween leftArmTween;
    private Vector2 startWorldPosition;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        startWorldPosition = rb != null ? rb.position : (Vector2)transform.position;
        AutoAssignArms();
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (isDescending)
        {
            Vector2 nextPosition = rb.position + (Vector2.down * descendSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);
            return;
        }

        if (isAscending)
        {
            Vector2 nextPosition = Vector2.MoveTowards(rb.position, startWorldPosition, ascendSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);

            if (Vector2.Distance(nextPosition, startWorldPosition) <= ascendStopDistance)
            {
                isAscending = false;
            }
        }
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
        if (!ValidateDependencies())
        {
            OnArmActionEnd?.Invoke();
            return;
        }

        startWorldPosition = rb.position;

        await RotateArmsAsync(rightOpenAngle, leftOpenAngle, openAngularSpeed);

        isDescending = true;
        await UniTask.WaitUntil(() => !isDescending);

        await RotateArmsAsync(rightCloseAngle, leftCloseAngle, closeAngularSpeed);

        if (gripDelaySeconds > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(gripDelaySeconds));
        }

        isAscending = true;
        await UniTask.WaitUntil(() => !isAscending);

        OnArmActionEnd?.Invoke();
    }

    private async UniTask OnArmReleaseAsync()
    {
        if (!ValidateDependencies())
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
            leftArmTween.AsyncWaitForCompletion().AsUniTask());
    }

    private bool ValidateDependencies()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        AutoAssignArms();

        if (rb != null && rightArm != null && leftArm != null)
        {
            return true;
        }

        Debug.LogWarning("NormalCrane: Rigidbody2D または rightArm / leftArm が未設定です。Inspectorで設定してください。", this);
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
