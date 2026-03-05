using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private CraneBase playerOneCrane;
    [SerializeField] private CraneBase playerTwoCrane;
    [SerializeField] private Image threeImage;
    [SerializeField] private Image twoImage;
    [SerializeField] private Image oneImage;
    [SerializeField] private Image goImage;
    [SerializeField] private InGameCharacter[] playerOneImage;
    [SerializeField] private InGameCharacter[] playerTwoImage;
    async UniTask Start()
    {
        playerOneCrane.Inittialize(GameManager.Instance.PlayerOneCraneType);
        if(!GameManager.Instance.isSingle)
        playerTwoCrane.Inittialize(GameManager.Instance.PlayerTwoCraneType);
        InGameCharacter playerOneCraneType = null;
        switch(GameManager.Instance.PlayerOneCraneType)
        {
            case CraneType.Standard:
                playerOneCraneType = playerOneImage[0];
                playerOneImage[0].gameObject.SetActive(true);
                break;
            case CraneType.Speed:
                playerOneCraneType = playerOneImage[1];
                playerOneImage[1].gameObject.SetActive(true);
                break;
            case CraneType.Hammer:
                playerOneCraneType = playerOneImage[2];
                playerOneImage[2].gameObject.SetActive(true);
                break;
            case CraneType.Bomb:
                playerOneCraneType = playerOneImage[3];
                playerOneImage[3].gameObject.SetActive(true);
                break;
        }
        Vector2 beforeScale = Vector2.zero;
        GameManager.Instance.PlayerOneCharacter = playerOneCraneType;
        if(GameManager.Instance.isSingle) {
            threeImage.gameObject.SetActive(true);
        beforeScale = threeImage.transform.localScale;
        threeImage.transform.localScale = Vector3.zero;
        threeImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        threeImage.gameObject.SetActive(false);
        twoImage.gameObject.SetActive(true);
        beforeScale = twoImage.transform.localScale;
        twoImage.transform.localScale = Vector3.zero;
        twoImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        twoImage.gameObject.SetActive(false);
        oneImage.gameObject.SetActive(true);
        beforeScale = oneImage.transform.localScale;
        oneImage.transform.localScale = Vector3.zero;
        oneImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        oneImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(true);
        beforeScale = goImage.transform.localScale;
        goImage.transform.localScale = Vector3.zero;
        goImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        goImage.gameObject.SetActive(false);
        TimeManager.instance.StartTimer();
        playerOneCrane.GameStart();
        GameManager.Instance.CurrentGameState = GameState.InGame;
            return;
        }
        InGameCharacter playerTwoCraneType = null;
        switch(GameManager.Instance.PlayerTwoCraneType)
        {            case CraneType.Standard:
                playerTwoCraneType = playerTwoImage[0];
                playerTwoImage[0].gameObject.SetActive(true);
                break;
            case CraneType.Speed:
                playerTwoCraneType = playerTwoImage[1];
                playerTwoImage[1].gameObject.SetActive(true);
                break;
            case CraneType.Hammer:
                playerTwoCraneType = playerTwoImage[2];
                playerTwoImage[2].gameObject.SetActive(true);
                break;
            case CraneType.Bomb:
                playerTwoCraneType = playerTwoImage[3];
                playerTwoImage[3].gameObject.SetActive(true);
                break;
        }
        GameManager.Instance.PlayerTwoCharacter = playerTwoCraneType;
        threeImage.gameObject.SetActive(true);
        beforeScale = threeImage.transform.localScale;
        threeImage.transform.localScale = Vector3.zero;
        threeImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        threeImage.gameObject.SetActive(false);
        twoImage.gameObject.SetActive(true);
        beforeScale = twoImage.transform.localScale;
        twoImage.transform.localScale = Vector3.zero;
        twoImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        twoImage.gameObject.SetActive(false);
        oneImage.gameObject.SetActive(true);
        beforeScale = oneImage.transform.localScale;
        oneImage.transform.localScale = Vector3.zero;
        oneImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        oneImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(true);
        beforeScale = goImage.transform.localScale;
        goImage.transform.localScale = Vector3.zero;
        goImage.transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        goImage.gameObject.SetActive(false);
        TimeManager.instance.StartTimer();
        GameManager.Instance.CurrentGameState = GameState.InGame;
        playerOneCrane.GameStart();
        playerTwoCrane.GameStart();
    }

}
