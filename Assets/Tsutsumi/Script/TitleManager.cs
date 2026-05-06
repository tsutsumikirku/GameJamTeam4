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
}
