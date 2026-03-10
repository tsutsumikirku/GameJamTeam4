
using System;
using DG.Tweening;
using UnityEngine;

public class CraneBase : MonoBehaviour, IPauseResume
{
    #region インスペクターから編集可能な変数の宣言
    [Header("クレーンの移動方向")]
    [SerializeField] CraneMoveType moveDirection = CraneMoveType.Right;

    [Header("操作キー")]
    [SerializeField] KeyCode actionKey;

    [Header("移動範囲")]
    [SerializeField] float maxMoveX;
    
    [Header("移動速度")]
    [SerializeField] float moveSpeed = 5f;
    
    [Header("下降速度")]
    [SerializeField] float descendSpeed = 3f;

    [Header("上昇時間")]
    [SerializeField] float ascendTime = 2f;
    [Header("戻る速度の秒速")]
    [SerializeField] float returnSpeed = 2f;

    [Header("アームの回転時間")]
    [SerializeField] float armRotationTime = 1f; 

    [Header("クレーンのタイプ")]
    [SerializeField] CraneType craneType;

    [Header("クレーンのアーム")]
    [SerializeField] CraneArmData[] craneDatas;
    #endregion

    #region クレーンの状態やデータを管理する変数の宣言
    IClaneArm currentArm; // 現在のクレーンアーム

    CraneState craneStateData = CraneState.DontMove; // クレーンの状態を管理する変数

    Vector2 startPosition; // クレーンの開始位置

    #endregion

    #region プロパティ
    CraneState craneState{get => craneStateData; set
    {
        craneStateData = value;
        // クレーンの状態が変わったときの処理をここに追加
        switch (craneStateData)
        {
            case CraneState.Moving:
                // クレーンが移動状態になったときの処理
                break;
            case CraneState.ArmAction:
                currentArm.OnArmStart();
                currentArm.OnArmActionEnd = () =>
                {
                    Debug.Log("ArmActionEnd");
                    if(craneState == CraneState.DontMove)return;
                    craneState = CraneState.Returning;
                };
                // クレーンがアームアクション状態になったときの処理
                break;
            case CraneState.Returning:
                // クレーンが戻り状態になったときの処理
                    transform.DOMove(startPosition, Vector2.Distance(transform.position, startPosition) / returnSpeed).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        if(craneState == CraneState.DontMove)return;
                        craneState = CraneState.ArmReleaseAction;
                    }).SetUpdate(UpdateType.Fixed); // ポーズ中も動くようにする
                break;
            case CraneState.ArmReleaseAction:
                // クレーンがアームリリースアクション状態になったときの処理
                currentArm.OnArmRelease();
                currentArm.OnArmReleaseEnd = () =>
                {
                    if(craneState == CraneState.DontMove)return;
                    craneState = CraneState.Moving;
                };
                break;
        }
    }} 
    #endregion
    #region 初期化処理
    public void Inittialize(CraneType type)
    {
        craneType = type;
        startPosition = transform.position;
        // クレーンのタイプに応じた初期化処理をここに追加
        switch (craneType)
        {
            case CraneType.Standard:
                var data = Array.Find(craneDatas, c => c.craneType == CraneType.Standard);
                
                if (data != null)                {
                    // スタンダードクレーンのアームを有効化                    
                    data.craneArm.SetActive(true);
                    this.maxMoveX = data.craneData.maxMoveX;
                    this.moveSpeed = data.craneData.moveSpeed;
                    this.descendSpeed = data.craneData.descendSpeed;
                    this.ascendTime = data.craneData.ascendTime;
                    this.returnSpeed = data.craneData.returnSpeed;
                    this.armRotationTime = data.craneData.armRotationTime;
                }
                currentArm = data.craneArm.GetComponent<IClaneArm>();
                // スタンダードクレーンの初期化処理
                break;
            case CraneType.Speed:
                // スピードクレーンの初期化処理
                var speedData = Array.Find(craneDatas, c => c.craneType == CraneType.Speed);
                if (speedData != null)
                {
                    this.maxMoveX = speedData.craneData.maxMoveX;
                    this.moveSpeed = speedData.craneData.moveSpeed;
                    this.descendSpeed = speedData.craneData.descendSpeed;
                    this.ascendTime = speedData.craneData.ascendTime;
                    this.returnSpeed = speedData.craneData.returnSpeed;
                    this.armRotationTime = speedData.craneData.armRotationTime;
                    // スピードクレーンのアームを有効化                    
                    speedData.craneArm.SetActive(true);
                }
                currentArm = speedData.craneArm.GetComponent<IClaneArm>();
                break;
            case CraneType.Hammer:
                // ハンマークレーンの初期化処理
                var hammerData = Array.Find(craneDatas, c => c.craneType == CraneType.Hammer);
                if (hammerData != null)
                {
                    this.maxMoveX = hammerData.craneData.maxMoveX;
                    this.moveSpeed = hammerData.craneData.moveSpeed;
                    this.descendSpeed = hammerData.craneData.descendSpeed;
                    this.ascendTime = hammerData.craneData.ascendTime;
                    this.returnSpeed = hammerData.craneData.returnSpeed;
                    this.armRotationTime = hammerData.craneData.armRotationTime;
                    // ハンマークレーンのアームを有効化                    
                    hammerData.craneArm.SetActive(true);
                }
                currentArm = hammerData.craneArm.GetComponent<IClaneArm>();
                break;
            case CraneType.Bomb:
                // ボムクレーンの初期化処理
                var bombData = Array.Find(craneDatas, c => c.craneType == CraneType.Bomb);
                if (bombData != null)
                {
                    this.maxMoveX = bombData.craneData.maxMoveX;
                    this.moveSpeed = bombData.craneData.moveSpeed;
                    this.descendSpeed = bombData.craneData.descendSpeed;
                    this.ascendTime = bombData.craneData.ascendTime;
                    this.returnSpeed = bombData.craneData.returnSpeed;
                    this.armRotationTime = bombData.craneData.armRotationTime;
                    // ボムクレーンのアームを有効化                    
                    bombData.craneArm.SetActive(true);
                }
                currentArm = bombData.craneArm.GetComponent<IClaneArm>();
                break;
        }
    }
    // ゲーム開始時の処理
    public void GameStart()
    {
        craneState = CraneState.Moving;
    }
    public void GameEnd()
    {
        craneState = CraneState.DontMove;
    }
    #endregion

    #region クレーンの更新処理
    private void FixedUpdate()
    {
        switch (craneState)
        {
            case CraneState.Moving:
                MoveCrane();
                break;
            case CraneState.ArmAction:
                
                break;
            case CraneState.Returning:
                break;
            case CraneState.ArmReleaseAction:
                
                break;
        }
    }
    void Update()
    {
        if(craneState == CraneState.ArmAction)
        {
            if(Input.GetKeyDown(actionKey))
                
            {
                    currentArm.OnArmEnd();
                }
        }
        if(craneState == CraneState.Moving)
        {
            if(Input.GetKeyUp(actionKey))
            {
                craneState = CraneState.ArmAction;
            }
        }
    }
    #endregion

    #region クレーンの移動の更新処理
    private void MoveCrane()
    {
        if (Input.GetKey(actionKey))
        {
            float moveDirectionValue = (moveDirection == CraneMoveType.Right) ? 1f : -1f;
            transform.Translate(Vector3.right * moveDirectionValue * moveSpeed * Time.deltaTime);
            // 移動範囲を制限
            float clampedX;
            if(moveDirection == CraneMoveType.Right)
            {
                clampedX = Mathf.Clamp(transform.position.x, startPosition.x, maxMoveX);
            }
            else
            {
                clampedX = Mathf.Clamp(transform.position.x, -maxMoveX, startPosition.x);
            }
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }
    #endregion

    #region クレーンの

    #endregion


    #region ポーズ処理
    public void Pause()
    {
    }

    public void Resume()
    {
        
    }
    #endregion
}

[System.Serializable]
public class CraneArmData
{
    public CraneType craneType;
    public CraneDataObj craneData;
    public GameObject craneArm;
}
public enum CraneMoveType
{
    Right,
    Left
}
public enum CraneType
{
    Standard = 0,
    Speed = 1,
    Hammer = 2,
    Bomb = 3
}
public enum CraneState
{
    // 動けません
    DontMove,
    // 待機状態
    Moving,
    //　戻りちゅう
    Returning,
    // アームのアクション中
    ArmAction,
    ArmReleaseAction
}