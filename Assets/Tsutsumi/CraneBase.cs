
using System;
using DG.Tweening;
using UnityEditor.Callbacks;
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
                    craneState = CraneState.Returning;
                };
                // クレーンがアームアクション状態になったときの処理
                break;
            case CraneState.Returning:
                // クレーンが戻り状態になったときの処理
                    transform.DOMove(startPosition, Vector2.Distance(transform.position, startPosition) / returnSpeed).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        craneState = CraneState.ArmReleaseAction;
                    });
                break;
            case CraneState.ArmReleaseAction:
                // クレーンがアームリリースアクション状態になったときの処理
                currentArm.OnArmRelease();
                currentArm.OnArmReleaseEnd = () =>
                {
                    craneState = CraneState.Moving;
                };
                break;
        }
    }} 
    #endregion

    void Start()
    {
        craneState = CraneState.DontMove;
        Inittialize(craneType); // クレーンの初期化処理を呼び出す
        craneState = CraneState.Moving; // ゲーム開始時のクレーンの状態を移動に設定
        startPosition = transform.position; // クレーンの開始位置を保存
    }
    #region 初期化処理
    public void Inittialize(CraneType type)
    {
        craneType = type;
        // クレーンのタイプに応じた初期化処理をここに追加
        switch (craneType)
        {
            case CraneType.Standard:
                var data = Array.Find(craneDatas, c => c.craneType == CraneType.Standard);
                if (data != null)                {
                    // スタンダードクレーンのアームを有効化                    
                    data.craneArm.SetActive(true);
                }
                currentArm = data.craneArm.GetComponent<IClaneArm>();
                // スタンダードクレーンの初期化処理
                break;
            case CraneType.Speed:
                // スピードクレーンの初期化処理
                var speedData = Array.Find(craneDatas, c => c.craneType == CraneType.Speed);
                if (speedData != null)
                {
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
    #endregion

    #region クレーンの更新処理
    void Update()
    {
        switch (craneState)
        {
            case CraneState.Moving:
                MoveCrane();
                break;
            case CraneState.ArmAction:
                if(Input.GetKeyDown(actionKey))
                {
                    currentArm.OnArmEnd();
                }
                break;
            case CraneState.Returning:
                break;
            case CraneState.ArmReleaseAction:
                
                break;
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
            float clampedX = Mathf.Clamp(transform.position.x, -maxMoveX, maxMoveX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        else if(Input.GetKeyUp(actionKey))
        {
            craneState = CraneState.ArmAction;
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
    public GameObject craneArm;
}
public enum CraneMoveType
{
    Right,
    Left
}
public enum CraneType
{
    Standard,
    Speed,
    Hammer,
    Bomb
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