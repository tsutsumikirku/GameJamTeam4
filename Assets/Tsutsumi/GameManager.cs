using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 変数とプロパティの宣言
    // インスペクターから編集できる変数
    [SerializeField] private int gameTimeLimit; // ゲームの制限時間を管理する変数
    public static GameManager Instance;
    // ゲームの状態を管理するプロパティ
    public GameState CurrentGameState { get => currentGameState; private set => OnStateChange(value); }
    private GameState currentGameState;
    private bool isGamePaused; // ゲームが一時停止しているかどうかを管理する変数
    private int playerOneScore; // プレイヤーのスコアを管理する変数
    private int playerTwoScore; // プレイヤーのスコアを管理する変数
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
        
    }
    public void PlayerTwoItemGet(int score)
    {
        
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