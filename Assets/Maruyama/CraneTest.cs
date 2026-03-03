using UnityEngine;

/// <summary>
/// GameManagerの簡易テスト版。クレーンの初期化と操作開始を行う。
/// </summary>
public class CraneTest : MonoBehaviour
{
    [Header("プレイ設定")]
    [SerializeField, Range(1, 2)] int playerCount = 1;

    [Header("P1")]
    [SerializeField] CraneController playerOneCrane;
    [SerializeField] CraneType playerOneCraneType;

    [Header("P2")]
    [SerializeField] CraneController playerTwoCrane;
    [SerializeField] CraneType playerTwoCraneType;

    /// <summary>
    /// クレーンの種類を設定し、操作を開始する。
    /// </summary>
    void Start()
    {
        playerOneCrane.CraneType = playerOneCraneType;
        playerOneCrane.StartControl();

        if (playerCount >= 2 && playerTwoCrane != null)
        {
            playerTwoCrane.CraneType = playerTwoCraneType;
            playerTwoCrane.StartControl();
        }
    }
}