using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    private enum VerticalMoveState
    {
        Idle,
        Descending,
        Ascending
    }

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
    [SerializeField] private float openAngularSpeed = 180f;
    [SerializeField] private float closeAngularSpeed = 180f;

    [Header("演出")]
    [SerializeField] private float gripDelaySeconds = 0.15f;
    [SerializeField] private float ascendStopDistance = 0.01f;

    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }

    private VerticalMoveState verticalMoveState = VerticalMoveState.Idle;
    private bool isActionRunning;
    private bool descendStopRequested;
    private Vector2 startWorldPosition;

    private Tween rightArmTween;
    private Tween leftArmTween;

    private void Awake()
    {
        TryResolveReferences();
        startWorldPosition = rb != null ? rb.position : (Vector2)transform.position;
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (verticalMoveState == VerticalMoveState.Descending)
        {
            rb.MovePosition(rb.position + Vector2.down * descendSpeed * Time.fixedDeltaTime);
            return;
        }

        if (verticalMoveState == VerticalMoveState.Ascending)
        {
            Vector2 next = Vector2.MoveTowards(rb.position, startWorldPosition, ascendSpeed * Time.fixedDeltaTime);
            rb.MovePosition(next);

            if (Vector2.Distance(next, startWorldPosition) <= ascendStopDistance)
            {
                verticalMoveState = VerticalMoveState.Idle;
            }
        }
    }

    public void OnArmStart()
    {
        if (isActionRunning)
        {
            return;
        }

        ArmActionSequenceAsync().Forget();
    }

    public void OnArmEnd()
    {
        if (verticalMoveState == VerticalMoveState.Descending)
        {
            descendStopRequested = true;
            verticalMoveState = VerticalMoveState.Idle;
        }
    }

    public void OnArmRelease()
    {
        ArmReleaseSequenceAsync().Forget();
    }

    private async UniTask ArmActionSequenceAsync()
    {
        if (!TryResolveReferences())
        {
            OnArmActionEnd?.Invoke();
            return;
        }

        isActionRunning = true;
        descendStopRequested = false;
        startWorldPosition = rb.position;

        await RotateArmsAsync(rightOpenAngle, leftOpenAngle, openAngularSpeed);

        verticalMoveState = VerticalMoveState.Descending;
        await UniTask.WaitUntil(() => descendStopRequested);

        await RotateArmsAsync(rightCloseAngle, leftCloseAngle, closeAngularSpeed);

        if (gripDelaySeconds > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(gripDelaySeconds));
        }

        verticalMoveState = VerticalMoveState.Ascending;
        await UniTask.WaitUntil(() => verticalMoveState == VerticalMoveState.Idle);

        isActionRunning = false;
        OnArmActionEnd?.Invoke();
    }

    private async UniTask ArmReleaseSequenceAsync()
    {
        if (!TryResolveReferences())
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

        float safeSpeed = Mathf.Max(0.01f, angularSpeed);
        float rightDuration = Mathf.Abs(Mathf.DeltaAngle(rightArm.localEulerAngles.z, rightTargetZ)) / safeSpeed;
        float leftDuration = Mathf.Abs(Mathf.DeltaAngle(leftArm.localEulerAngles.z, leftTargetZ)) / safeSpeed;

        rightArmTween = rightArm.DOLocalRotate(new Vector3(0f, 0f, rightTargetZ), rightDuration).SetEase(Ease.InOutSine);
        leftArmTween = leftArm.DOLocalRotate(new Vector3(0f, 0f, leftTargetZ), leftDuration).SetEase(Ease.InOutSine);

        await UniTask.WhenAll(
            rightArmTween.AsyncWaitForCompletion().AsUniTask(),
            leftArmTween.AsyncWaitForCompletion().AsUniTask());
    }

    private bool TryResolveReferences()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rightArm == null)
        {
            rightArm = transform.Find("rightArm");
        }

        if (leftArm == null)
        {
            leftArm = transform.Find("leftArm");
        }

        bool ok = rb != null && rightArm != null && leftArm != null;
        if (!ok)
        {
            Debug.LogWarning("NormalCrane: Rigidbody2D / rightArm / leftArm の参照が不足しています。", this);
        }

        return ok;
    }

    private void OnDestroy()
    {
        rightArmTween?.Kill();
        leftArmTween?.Kill();
    }
}
