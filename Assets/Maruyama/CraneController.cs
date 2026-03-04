// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class CraneController : MonoBehaviour
// {
//     [Header("����ݒ�")]
//     [SerializeField] KeyCode actionKey = KeyCode.Space;
//     [SerializeField] Vector3 moveDirection = Vector3.right;

//     [Header("�N���[���ݒ�")]
//     [SerializeField] float moveSpeed = 5f;
//     [SerializeField] float descendSpeed = 3f;
//     [SerializeField] float descendTime = 2f;

//     [Header("�A�[���ݒ� (�����擾����܂�)")]
//     [SerializeField] float armMotorSpeed = 150f;
//     [SerializeField] float maxMotorTorque = 1000f;

//     // �C���X�y�N�^�ł̐ݒ�s�v�B�R�[�h���Ŏ����擾���܂��B
//     private HingeJoint2D leftArmJoint;
//     private HingeJoint2D rightArmJoint;

//     [Header("������")]
//     [SerializeField] GameObject standardVisual;
//     [SerializeField] GameObject speedVisual;
//     [SerializeField] GameObject hammerVisual;
//     [SerializeField] GameObject bombVisual;

//     CraneType craneType;
//     CraneState state = CraneState.Idle;
//     bool canControl = false;
//     bool isPaused = false;

//     float stateTimer = 0f;
//     Rigidbody2D rb;
//     Vector3 startPosition;

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         // �N���[���{�̂͏d�͂̉e�����󂯂��A���x�Œ��ړ��������� Kinematic ����������
//         rb.bodyType = RigidbodyType2D.Kinematic;
//         startPosition = transform.position;
//     }

//     public CraneType CraneType
//     {
//         get => craneType;
//         set { craneType = value; ApplyType(); }
//     }

//     public void StartControl()
//     {
//         canControl = true;
//         state = CraneState.Moving;
//         CloseArms();
//     }

//     public void StopControl()
//     {
//         canControl = false;
//         state = CraneState.Idle;
//         rb.linearVelocity = Vector2.zero; // ��~���͑��x�����Z�b�g (Unity6�ȍ~�� linearVelocity, �Â��o�[�W������ velocity)
//     }

//     void Update()
//     {
//         if (!canControl || isPaused) return;

//         // �L�[�𗣂������̔���� Update �ōs���iFixedUpdate���Ǝ�肱�ڂ����Ƃ����邽�߁j
//         if (state == CraneState.Moving && Input.GetKeyUp(actionKey))
//         {
//             state = CraneState.Descending;
//             stateTimer = 0f;
//         }
//     }

//     // �����I�Ȉړ��� FixedUpdate �ōs��
//     void FixedUpdate()
//     {
//         if (!canControl || isPaused)
//         {
//             rb.linearVelocity = Vector2.zero;
//             return;
//         }

//         switch (state)
//         {
//             case CraneState.Moving:
//                 if (Input.GetKey(actionKey))
//                 {
//                     rb.linearVelocity = moveDirection * moveSpeed;
//                 }
//                 else
//                 {
//                     rb.linearVelocity = Vector2.zero; // �L�[�������Ă��Ȃ����͎~�܂�
//                 }
//                 break;

//             case CraneState.Descending:
//                 rb.linearVelocity = Vector2.down * descendSpeed;
//                 stateTimer += Time.fixedDeltaTime;

//                 if (stateTimer >= descendTime)
//                 {
//                     OpenArms();
//                     rb.linearVelocity = Vector2.zero; // ���~��~
//                     state = CraneState.Catching;
//                     stateTimer = 0f;
//                 }
//                 break;

//             case CraneState.Catching:
//                 rb.linearVelocity = Vector2.zero;
//                 stateTimer += Time.fixedDeltaTime;
//                 if (stateTimer >= 1.0f)
//                 {
//                     state = CraneState.Ascending;
//                     stateTimer = 0f;
//                 }
//                 break;

//             case CraneState.Ascending:
//                 rb.linearVelocity = Vector2.up * descendSpeed;
//                 stateTimer += Time.fixedDeltaTime;

//                 if (stateTimer >= descendTime)
//                 {
//                     rb.linearVelocity = Vector2.zero; // �㏸��~
//                     state = CraneState.Returning;
//                 }
//                 break;

//             case CraneState.Returning:

//                 Vector3 target = new Vector3(startPosition.x, transform.position.y, transform.position.z);

//                 Vector2 dir = (target - transform.position).normalized;
//                 rb.linearVelocity = dir * moveSpeed;

//                 // ������x�߂Â����瓞������
//                 if (Vector2.Distance(transform.position, target) < 0.1f)
//                 {
//                     rb.linearVelocity = Vector2.zero;
//                     state = CraneState.Releasing;
//                     stateTimer = 0f;
//                 }
//                 break;

//             case CraneState.Releasing:
//                 CloseArms();
//                 state = CraneState.Moving;
//                 break;
//         }
//     }

//     // --- �q�I�u�W�F�N�g�؂�ւ����ɃW���C���g���擾���� ---
//     void ApplyType()
//     {
//         // �r�W���A���̐؂�ւ�
//         standardVisual.SetActive(craneType == CraneType.Standard);
//         speedVisual.SetActive(craneType == CraneType.Speed);
//         hammerVisual.SetActive(craneType == CraneType.HammerCrusher);
//         bombVisual.SetActive(craneType == CraneType.BombDropper);

//         GameObject activeVisual = null;
//         switch (craneType)
//         {
//             case CraneType.Standard: moveSpeed = 5f; activeVisual = standardVisual; break;
//             case CraneType.Speed: moveSpeed = 8f; activeVisual = speedVisual; break;
//             case CraneType.HammerCrusher: moveSpeed = 3f; activeVisual = hammerVisual; break;
//             case CraneType.BombDropper: moveSpeed = 4f; activeVisual = bombVisual; break;
//         }

//         if (activeVisual != null)
//         {
//             Debug.Log($"<color=cyan>[Crane] �N���[���؂�ւ�: {craneType} ({activeVisual.name})</color>");

//             HingeJoint2D[] joints = activeVisual.GetComponentsInChildren<HingeJoint2D>();
//             Debug.Log($"[Crane] �q�I�u�W�F�N�g���� {joints.Length} �� HingeJoint2D �����o���܂����B");

//             if (joints.Length >= 2)
//             {
//                 // X���W���r���č��E�𔻕�
//                 if (joints[0].transform.localPosition.x < joints[1].transform.localPosition.x)
//                 {
//                     leftArmJoint = joints[0];
//                     rightArmJoint = joints[1];
//                 }
//                 else
//                 {
//                     leftArmJoint = joints[1];
//                     rightArmJoint = joints[0];
//                 }

//                 Debug.Log($"<color=green>[Crane] �W���C���g��������: ��={leftArmJoint.name}(x:{leftArmJoint.transform.localPosition.x}), �E={rightArmJoint.name}(x:{rightArmJoint.transform.localPosition.x})</color>");
//             }
//             else
//             {
//                 Debug.LogError($"<color=red>[Crane] �G���[: {activeVisual.name} �̒��� HingeJoint2D �� 2�ȏ㌩����܂���I (����: {joints.Length}��)</color>");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("[Crane] ActiveVisual �� null �ł��B�C���X�y�N�^�� GameObject ���蓖�Ă��m�F���Ă��������B");
//         }
//     }

//     // --- �A�[���J�� ---
//     void OpenArms()
//     {
//         SetMotorSpeed(-armMotorSpeed, armMotorSpeed);
//     }
//     void CloseArms()
//     {
//         SetMotorSpeed(armMotorSpeed, -armMotorSpeed);
//     }

//     void SetMotorSpeed(float leftSpeed, float rightSpeed)
//     {
//         if (leftArmJoint != null)
//         {
//             JointMotor2D leftMotor = leftArmJoint.motor;
//             leftMotor.motorSpeed = leftSpeed;
//             leftMotor.maxMotorTorque = maxMotorTorque;
//             leftArmJoint.motor = leftMotor;
//             leftArmJoint.useMotor = true;
//         }

//         if (rightArmJoint != null)
//         {
//             JointMotor2D rightMotor = rightArmJoint.motor;
//             rightMotor.motorSpeed = rightSpeed;
//             rightMotor.maxMotorTorque = maxMotorTorque;
//             rightArmJoint.motor = rightMotor;
//             rightArmJoint.useMotor = true;
//         }
//     }
//     private void StopArms()
//     {
//         if (leftArmJoint != null)
//             leftArmJoint.useMotor = false;

//         if (rightArmJoint != null)
//             rightArmJoint.useMotor = false;
//     }

//     public void Pause()
//     {
//         isPaused = true;
//     }
//     public void Resume()
//     {
//         isPaused = false;
//     }
//     public void OnFloorClane()
//     {
//         if(state == CraneState.Ascending)return;
//         stateTimer += descendTime;
//     }
// }

// �N���[���̎�ނ��`����񋓌^
// 4�킩��I�񂾂��̂�����SetActive(True)�ɂ��Ă��邪Prefab�̂ق��������C�����܂�
// public enum CraneType
// {
//     Standard,
//     Speed,
//     HammerCrusher,
//     BombDropper
// }

// �X�e�[�g�� Catching �� Releasing ��ǉ�
// public enum CraneState
// {
//     Idle,
//     Moving,
//     Descending,
//     Catching,
//     Ascending,
//     Returning,
//     Releasing
// }