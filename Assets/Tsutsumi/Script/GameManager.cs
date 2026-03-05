using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 変数とプロパティの宣言
    // インスペクターから編集できる変数
    [SerializeField] private int gameTimeLimit; // ゲームの制限時間を管理する変数
    public static GameManager Instance;
    // ゲームの状態を管理するプロパティ
    public GameState CurrentGameState { get => currentGameState; set => OnStateChange(value); }
    private GameState currentGameState;
    private bool isGamePaused; // ゲームが一時停止しているかどうかを管理する変数
    private int playerOneScore; // プレイヤーのスコアを管理する変数
    private int playerTwoScore; // プレイヤーのスコアを管理する変数
    public CraneType PlayerOneCraneType;
    public CraneType PlayerTwoCraneType;
    public InGameCharacter PlayerOneCharacter;
    public InGameCharacter PlayerTwoCharacter;
    private IPauseResume[] pauseResumeObjects;
    public bool isSingle = false;
    // ゲームのデータを管理する変数
    #endregion

    #region ゲーム状態が変わったときの処理
    private void OnStateChange(GameState newState)
    {
        currentGameState = newState;
        // ゲーム状態が変わったときの処理をここに追加
        switch (currentGameState)
        {
            case GameState.OutGame:
                OnOutGame();
                break;
            case GameState.InGame:
                OnInGame();
                break;
            case GameState.Result:
                OnResult();
                break;
        }
    }
    
    // アウトゲームのステートに変化した際に呼び出される関数
    private void OnOutGame()
    {
        
    }
    // インゲームのステートに変化した際に呼び出される関数
    private void OnInGame()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
    }
    // リザルトのステートに変化した際に呼び出される関数
    private void OnResult()
    {
        
    }
    #endregion

    #region アイテムの管理
    // アイテムの管理に関するコードをここに追加
    public void PlayerOneItemGet(int score)
    {
        playerOneScore += score;
        if (score > 0)
        PlayerOneCharacter.Smile().Forget();
        else 
        PlayerOneCharacter.Sad().Forget();
    }
    public void PlayerTwoItemGet(int score)
    {
        playerTwoScore += score;
        if (score > 0)
        PlayerTwoCharacter.Smile().Forget();
        else 
        PlayerTwoCharacter.Sad().Forget();
    }

    #endregion

    #region ゲームの一時停止と再開
    public void PauseGame()
    {
        isGamePaused = true;
        pauseResumeObjects = Array.ConvertAll(FindObjectsByType<GameObject>(FindObjectsSortMode.None), obj => obj.GetComponent<IPauseResume>());
        foreach (var item in pauseResumeObjects)
        {
            if (item != null)
            {
                item.Pause();
            }
        }
    }
    public void ResumeGame()
    {
        isGamePaused = false;
        foreach (var item in pauseResumeObjects)
        {
            if (item != null)
            {
                item.Resume();
            }
        }
    }
    #endregion

    #region シングルトンの実装
    // シングルトンの実装
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}

public enum GameState
{
    OutGame,
    InGame,
    Result
}