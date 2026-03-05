using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InGameCharacter : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite smileSprite;
    [SerializeField] private Sprite sadSprite;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = normalSprite;
    }
    public async UniTask Smile()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        GetComponent<SpriteRenderer>().sprite = smileSprite;
        await UniTask.Delay(1000, cancellationToken: cancellationTokenSource.Token);
        GetComponent<SpriteRenderer>().sprite = normalSprite;
    }
    public async UniTask Sad()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        GetComponent<SpriteRenderer>().sprite = sadSprite;
        await UniTask.Delay(1000, cancellationToken: cancellationTokenSource.Token);
        GetComponent<SpriteRenderer>().sprite = normalSprite;
    }
}
