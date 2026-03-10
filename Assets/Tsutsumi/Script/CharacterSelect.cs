using UnityEngine.UI;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private bool isPlayerOne = false; 
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Image[] select;
    [SerializeField] private SceneChanger sceneChanger;
    [SerializeField] private string nextSceneName;
    private int currentIndex = 0;
    void Start()
    {
        for(int i = 0; i < select.Length; i++)
        {
            if(i == currentIndex)
            {
                select[i].gameObject.SetActive(true);
            }
            else
            {
                select[i].gameObject.SetActive(false);
            }
        }
        characterImage.sprite = characterSprites[currentIndex];
    }
    public void SetCharacter(int index)
    {
        currentIndex = index;
        for(int i = 0; i < select.Length; i++)
        {
            if(i == currentIndex)
            {
                select[i].gameObject.SetActive(true);
            }
            else
            {
                select[i].gameObject.SetActive(false);
            }
        }
        characterImage.sprite = characterSprites[currentIndex];
    }
    public void NextScene()
    {
        if(isPlayerOne)
        {
            GameManager.Instance.PlayerOneCraneType = (CraneType)currentIndex;
        }
        else
        {
            GameManager.Instance.PlayerTwoCraneType = (CraneType)currentIndex;
        }
        sceneChanger.OnSceneChange(nextSceneName);
    }
}
