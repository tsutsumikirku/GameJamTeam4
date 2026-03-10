using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
        void Awake()
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
    [SerializeField]AudioSource bgmSource;
    [SerializeField]AudioSource seSource;
    
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }
    public void PlaySE(AudioClip clip)
    {
        if (seSource == null)
        {
            seSource = gameObject.AddComponent<AudioSource>();
        }
        seSource.PlayOneShot(clip);
    }
}
