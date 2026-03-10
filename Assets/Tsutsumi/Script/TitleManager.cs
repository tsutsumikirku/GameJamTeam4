using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private float idleTimeLimit;
    [SerializeField] private Image BlackOut;
    [SerializeField] private CanvasGroup canvasGroup;
    private float idleTimer;
    private bool isMoviePlaying;
    void Start()
    {
        GameManager.Instance.CurrentGameState = GameState.OutGame;
    }
    void Update()
    {
        idleTimer += Time.deltaTime; // 毎フレーム、アイドルタイマーを更新
        if (idleTimer >= idleTimeLimit && !isMoviePlaying) // アイドルタイムが制限時間を超え、かつムービーが再生されていない場合
        {
            MovieStart(); // アイドルタイムが制限時間を超えた場合、ムービーを開始
            isMoviePlaying = true;
        }
    }
    public void OnInput()
    {
        idleTimer = 0f; // 入力があった場合、アイドルタイマーをリセット
        if (isMoviePlaying) // ムービーが再生されている場合
        {
            MovieEnd(); // ムービーを終了
            isMoviePlaying = false;
        }
    }
    public void MovieStart()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.2f);
    }
    public void MovieEnd()
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.2f).OnComplete(() => canvasGroup.gameObject.SetActive(false));
    }
}
