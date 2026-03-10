using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI playerOneScoreText;
    [SerializeField] private TextMeshProUGUI playerTwoScoreText;
    [SerializeField] private Sprite[] characterWinImages;
    [SerializeField] private Sprite[] characterLoseImages;
    [SerializeField] private Sprite[] winImage;

    [SerializeField] private Image playerOneCharacterImage;
    [SerializeField] private Image playerTwoCharacterImage;
    [SerializeField] private Image resultPlayerOneImage;
    [SerializeField] private Image resultPlayerTwoImage;
    void Start()
    {
        GameManager.Instance.CurrentGameState = GameState.Result;
        int playerOneScore = GameManager.Instance.playerOneScore;
        int playerTwoScore = GameManager.Instance.playerTwoScore;

        playerOneScoreText.text = "Player 1 Score: " + playerOneScore;
        playerTwoScoreText.text = "Player 2 Score: " + playerTwoScore;

        // 勝敗の設定
        if (playerOneScore > playerTwoScore)
        {
            resultPlayerOneImage.sprite = winImage[0]; // プレイヤー1の勝利画像
            resultPlayerTwoImage.sprite = winImage[1]; // プレイヤー2の敗北画像
            playerOneCharacterImage.sprite = characterWinImages[(int)GameManager.Instance.PlayerOneCraneType];
            playerTwoCharacterImage.sprite = characterLoseImages[(int)GameManager.Instance.PlayerTwoCraneType];
        }
        else if (playerTwoScore > playerOneScore)
        {
            resultPlayerOneImage.sprite = winImage[1]; // プレイヤー1の敗北画像
            resultPlayerTwoImage.sprite = winImage[0]; // プレイヤー2の勝利画像
            playerOneCharacterImage.sprite = characterLoseImages[(int)GameManager.Instance.PlayerOneCraneType];
            playerTwoCharacterImage.sprite = characterWinImages[(int)GameManager.Instance.PlayerTwoCraneType];
        }
        else
        {
            resultPlayerOneImage.sprite = winImage[0]; // 引き分け画像
            resultPlayerTwoImage.sprite = winImage[0]; // 引き分け画像
            playerOneCharacterImage.sprite = characterWinImages[(int)GameManager.Instance.PlayerOneCraneType];
            playerTwoCharacterImage.sprite = characterWinImages[(int)GameManager.Instance.PlayerTwoCraneType];
        }
    }
}
