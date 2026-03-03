using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HammerCraneController : MonoBehaviour
{
    [Header("操作設定")]
    [SerializeField] KeyCode actionKey = KeyCode.Space;
    [SerializeField] Vector3 moveDirection = Vector3.right;

    [Header("クレーン設定")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float descendSpeed = 3f;
    [SerializeField] float descendTime = 2f;

    [Header("ハンマー設定")]
    [SerializeField] HingeJoint2D hammerJoint;
    [SerializeField] float swingMotorSpeed = 300f;   // 振り下ろしモーター速度
    [SerializeField] float returnMotorSpeed = -100f;  // 戻しモーター速度（符号はInspectorで調整）
    [SerializeField] float hammerTorque = 500f;
    [SerializeField] float swingTime = 0.3f;     // 振り下ろしにかける時間
    [SerializeField] float recoverTime = 0.5f;   // 戻しにかける時間

    Rigidbody2D rb;
    Vector3 startPosition;
    HammerState state = HammerState.Idle;
    float stateTimer = 0f;
    bool canControl = false;
    bool isPaused = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;
    }

    public void StartControl()
    {
        canControl = true;
        state = HammerState.Moving;
        StopHammerMotor();
    }

    public void StopControl()
    {
        canControl = false;
        state = HammerState.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;

    void Update()
    {
        if (!canControl || isPaused) return;

        if (state == HammerState.Moving && Input.GetKeyUp(actionKey))
        {
            state = HammerState.Descending;
            stateTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (!canControl || isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (state)
        {
            case HammerState.Moving:
                rb.linearVelocity = Input.GetKey(actionKey)
                    ? (Vector2)(moveDirection * moveSpeed)
                    : Vector2.zero;
                break;

            case HammerState.Descending:
                rb.linearVelocity = Vector2.down * descendSpeed;
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= descendTime)
                {
                    rb.linearVelocity = Vector2.zero;
                    SetHammerMotor(swingMotorSpeed);
                    state = HammerState.Swinging;
                    stateTimer = 0f;
                    Debug.Log("<color=orange>[Hammer] 振り下ろし！</color>");
                }
                break;

            case HammerState.Swinging:
                rb.linearVelocity = Vector2.zero;
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= swingTime)
                {
                    SetHammerMotor(returnMotorSpeed);
                    state = HammerState.Recovering;
                    stateTimer = 0f;
                    Debug.Log("<color=orange>[Hammer] 引き戻し</color>");
                }
                break;

            case HammerState.Recovering:
                rb.linearVelocity = Vector2.zero;
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= recoverTime)
                {
                    StopHammerMotor();
                    state = HammerState.Ascending;
                    stateTimer = 0f;
                }
                break;

            case HammerState.Ascending:
                rb.linearVelocity = Vector2.up * descendSpeed;
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= descendTime)
                {
                    rb.linearVelocity = Vector2.zero;
                    state = HammerState.Returning;
                }
                break;

            case HammerState.Returning:
                Vector3 target = new Vector3(startPosition.x, transform.position.y, 0f);
                Vector2 dir = (target - transform.position).normalized;
                rb.linearVelocity = dir * moveSpeed;
                if (Vector2.Distance(transform.position, target) < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    state = HammerState.Moving;
                }
                break;
        }
    }

    // =========================================================
    // ハンマー制御（シンプル版）
    // =========================================================

    void SetHammerMotor(float speed)
    {
        if (hammerJoint == null) return;

        JointMotor2D motor = hammerJoint.motor;
        motor.motorSpeed = speed;
        motor.maxMotorTorque = hammerTorque;
        hammerJoint.motor = motor;
        hammerJoint.useMotor = true;
    }

    void StopHammerMotor()
    {
        if (hammerJoint == null) return;
        hammerJoint.useMotor = false;
    }
}

public enum HammerState
{
    Idle,
    Moving,
    Descending,
    Swinging,
    Recovering,
    Ascending,
    Returning
}