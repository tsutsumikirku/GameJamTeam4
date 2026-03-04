using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    Vector2 defaultScale;
    void Start()
    {
        defaultScale = transform.localScale;
        transform.DOScale(Vector2.zero, 0.7f);
    }
    public void OnSceneChange(string sceneName)
    {
        transform.DOScale(defaultScale, 0.7f).OnComplete(() => SceneManager.LoadScene(sceneName));
    }
}
