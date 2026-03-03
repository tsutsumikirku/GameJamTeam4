using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CraneController : MonoBehaviour
{
    [Header("操作設定")]
    [SerializeField] KeyCode actionKey = KeyCode.Space;
    [SerializeField] Vector3 moveDirection = Vector3.right;

    [Header("クレーン設定")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float descendSpeed = 3f;
    [SerializeField] float descendTime = 2f;

    [Header("アーム設定 (自動取得されます)")]
    [SerializeField] float armMotorSpeed = 150f;
    [SerializeField] float maxMotorTorque = 1000f;

    // インスペクタでの設定不要。コード内で自動取得します。
    private HingeJoint2D leftArmJoint;
    private HingeJoint2D rightArmJoint;

    [Header("見た目")]
    [SerializeField] GameObject standardVisual;
    [SerializeField] GameObject speedVisual;
    [SerializeField] GameObject hammerVisual;
    [SerializeField] GameObject bombVisual;

    CraneType craneType;
    CraneState state = CraneState.Idle;
    bool canControl = false;
    bool isPaused = false;

    float stateTimer = 0f;
    Rigidbody2D rb;
    Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // クレーン本体は重力の影響を受けず、速度で直接動かすため Kinematic がおすすめ
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;
    }

    public CraneType CraneType
    {
        get => craneType;
        set { craneType = value; ApplyType(); }
    }

    public void StartControl()
    {
        canControl = true;
        state = CraneState.Moving;
        CloseArms();
    }

    public void StopControl()
    {
        canControl = false;
        state = CraneState.Idle;
        rb.linearVelocity = Vector2.zero; // 停止時は速度をリセット (Unity6以降は linearVelocity, 古いバージョンは velocity)
    }

    void Update()
    {
        if (!canControl || isPaused) return;

        // キーを離した時の判定は Update で行う（FixedUpdateだと取りこぼすことがあるため）
        if (state == CraneState.Moving && Input.GetKeyUp(actionKey))
        {
            state = CraneState.Descending;
            stateTimer = 0f;
        }
    }

    // 物理的な移動は FixedUpdate で行う
    void FixedUpdate()
    {
        if (!canControl || isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (state)
        {
            case CraneState.Moving:
                if (Input.GetKey(actionKey))
                {
                    rb.linearVelocity = moveDirection * moveSpeed;
                }
                else
                {
                    rb.linearVelocity = Vector2.zero; // キーを押していない時は止まる
                }
                break;

            case CraneState.Descending:
                rb.linearVelocity = Vector2.down * descendSpeed;
                stateTimer += Time.fixedDeltaTime;

                if (stateTimer >= descendTime)
                {
                    OpenArms();
                    rb.linearVelocity = Vector2.zero; // 下降停止
                    state = CraneState.Catching;
                    stateTimer = 0f;
                }
                break;

            case CraneState.Catching:
                rb.linearVelocity = Vector2.zero;
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= 1.0f)
                {
                    state = CraneState.Ascending;
                    stateTimer = 0f;
                }
                break;

            case CraneState.Ascending:
                rb.linearVelocity = Vector2.up * descendSpeed;
                stateTimer += Time.fixedDeltaTime;

                if (stateTimer >= descendTime)
                {
                    rb.linearVelocity = Vector2.zero; // 上昇停止
                    state = CraneState.Returning;
                }
                break;

            case CraneState.Returning:

                Vector3 target = new Vector3(startPosition.x, transform.position.y, transform.position.z);

                Vector2 dir = (target - transform.position).normalized;
                rb.linearVelocity = dir * moveSpeed;

                // ある程度近づいたら到着判定
                if (Vector2.Distance(transform.position, target) < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    state = CraneState.Releasing;
                    stateTimer = 0f;
                }
                break;

            case CraneState.Releasing:
                CloseArms();
                state = CraneState.Moving;
                break;
        }
    }

    // --- 子オブジェクト切り替え時にジョイントを取得する ---
    void ApplyType()
    {
        // ビジュアルの切り替え
        standardVisual.SetActive(craneType == CraneType.Standard);
        speedVisual.SetActive(craneType == CraneType.Speed);
        hammerVisual.SetActive(craneType == CraneType.HammerCrusher);
        bombVisual.SetActive(craneType == CraneType.BombDropper);

        GameObject activeVisual = null;
        switch (craneType)
        {
            case CraneType.Standard: moveSpeed = 5f; activeVisual = standardVisual; break;
            case CraneType.Speed: moveSpeed = 8f; activeVisual = speedVisual; break;
            case CraneType.HammerCrusher: moveSpeed = 3f; activeVisual = hammerVisual; break;
            case CraneType.BombDropper: moveSpeed = 4f; activeVisual = bombVisual; break;
        }

        if (activeVisual != null)
        {
            Debug.Log($"<color=cyan>[Crane] クレーン切り替え: {craneType} ({activeVisual.name})</color>");

            HingeJoint2D[] joints = activeVisual.GetComponentsInChildren<HingeJoint2D>();
            Debug.Log($"[Crane] 子オブジェクトから {joints.Length} 個の HingeJoint2D を検出しました。");

            if (joints.Length >= 2)
            {
                // X座標を比較して左右を判別
                if (joints[0].transform.localPosition.x < joints[1].transform.localPosition.x)
                {
                    leftArmJoint = joints[0];
                    rightArmJoint = joints[1];
                }
                else
                {
                    leftArmJoint = joints[1];
                    rightArmJoint = joints[0];
                }

                Debug.Log($"<color=green>[Crane] ジョイント割当成功: 左={leftArmJoint.name}(x:{leftArmJoint.transform.localPosition.x}), 右={rightArmJoint.name}(x:{rightArmJoint.transform.localPosition.x})</color>");
            }
            else
            {
                Debug.LogError($"<color=red>[Crane] エラー: {activeVisual.name} の中に HingeJoint2D が 2つ以上見つかりません！ (現在: {joints.Length}個)</color>");
            }
        }
        else
        {
            Debug.LogWarning("[Crane] ActiveVisual が null です。インスペクタの GameObject 割り当てを確認してください。");
        }
    }

    // --- アーム開閉 ---
    void OpenArms()
    {
        SetMotorSpeed(-armMotorSpeed, armMotorSpeed);
    }
    void CloseArms()
    {
        SetMotorSpeed(armMotorSpeed, -armMotorSpeed);
    }

    void SetMotorSpeed(float leftSpeed, float rightSpeed)
    {
        if (leftArmJoint != null)
        {
            JointMotor2D leftMotor = leftArmJoint.motor;
            leftMotor.motorSpeed = leftSpeed;
            leftMotor.maxMotorTorque = maxMotorTorque;
            leftArmJoint.motor = leftMotor;
            leftArmJoint.useMotor = true;
        }

        if (rightArmJoint != null)
        {
            JointMotor2D rightMotor = rightArmJoint.motor;
            rightMotor.motorSpeed = rightSpeed;
            rightMotor.maxMotorTorque = maxMotorTorque;
            rightArmJoint.motor = rightMotor;
            rightArmJoint.useMotor = true;
        }
    }
    private void StopArms()
    {
        if (leftArmJoint != null)
            leftArmJoint.useMotor = false;

        if (rightArmJoint != null)
            rightArmJoint.useMotor = false;
    }

    public void Pause()
    {
        isPaused = true;
    }
    public void Resume()
    {
        isPaused = false;
    }
}

// クレーンの種類を定義する列挙型
// 4種から選んだものだけをSetActive(True)にしているがPrefabのほうがいい気がします
public enum CraneType
{
    Standard,
    Speed,
    HammerCrusher,
    BombDropper
}

// ステートに Catching と Releasing を追加
public enum CraneState
{
    Idle,
    Moving,
    Descending,
    Catching,
    Ascending,
    Returning,
    Releasing
}